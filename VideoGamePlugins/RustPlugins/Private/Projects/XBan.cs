using Oxide.Core;
using Oxide.Core.Libraries;
using Oxide.Core.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Oxide.Plugins
{
    [Info("XBan", "InfinityNet/Xray", "1.0.0")]
    [Description("Copyright Infinitynet.dk All Rights Reserved")]
    public class XBan : RustPlugin
    {
        public static bool DEBUG = false;
        #region Configuration
        public static XBan Instance { get; set; }
        public const string DiscordWebHookBan = "xxxxxxxx";
        public const string DiscordWebHookKick = "xxxxxxx";
        public const string DiscordWebHookBetterChatMute = "xxxxxxxxxx";


        public ulong[] StaffID = new ulong[] { 76561198066857380, 76561198023076226 };
        public const ulong IconID = 76561198389709969;
        public const string Perm = "XBan.use";
        public const string DiscordURL = "xxxxxxxxxx";
        public const string BansLocation = "BANS/BansDB";
        public const string TitleColor = "#4baffa";
        public const string MsgColor = "#96c8fa";
        #endregion Configuration
        #region Helpers
        public void DirectMessage(BasePlayer player, string message)
        {
            try { player.SendConsoleCommand("chat.add", 0, IconID, $"<size=18><color={TitleColor}>XBan</color> {message}</size>"); }
            catch { Instance.SendReply(player, message,2, IconID); }
        }
        private bool HasPerm(BasePlayer player) => (StaffID.Contains(player.userID) && permission.UserHasPermission(player.UserIDString, Perm));
        private void DiscordBanGiven(string TargetName, ulong TargetID, int BanDays, string BanReason, int TotalBans, string BanPhoto)
        {
            string timeLeft = BanDays/365 >= 200 ? "Permanent" : $"{BanDays} Dage";

            string body = "{\"content\":null,\"embeds\":[{\"description\":\"**BAN GIVEN᲼🔨**\\n>>> **SERVER**᲼__"+ConVar.Server.hostname+"__\\n**DURATION**᲼"+ timeLeft + "\\n**REASON**᲼"+BanReason+"\",\"color\":4960250,\"author\":{\"name\":\""+TargetName+" - "+TargetID+"\",\"url\":\"https://steamcommunity.com/profiles/"+TargetID+"\",\"icon_url\":\""+BanPhoto+"\"},\"footer\":{\"text\":\"Bans on record "+TotalBans+"\"}}],\"attachments\":[]}";
            webrequest.Enqueue(DiscordWebHookBan, body, (code, response) => { }, this, RequestMethod.POST, new Dictionary<string, string> { { "Content-Type", "application/json" } });  
        }
        private void DiscordBanRevoked(string TargetName, ulong TargetID, int BanIndex, string BanPhoto)
        {
            string body = "{\"content\":null,\"embeds\":[{\"description\":\"**BAN REVOKED᲼↩️**\\n>>> **SERVER**᲼__"+ConVar.Server.hostname+ "__\\n**INDEX**᲼"+BanIndex+"\",\"color\":4960250,\"author\":{\"name\":\""+TargetName+ " - "+TargetID+ "\",\"url\":\"https://steamcommunity.com/profiles/"+TargetID+"\",\"icon_url\":\""+BanPhoto+"\"}}],\"attachments\":[]}";
            webrequest.Enqueue(DiscordWebHookBan, body, (code, response) => { }, this, RequestMethod.POST, new Dictionary<string, string> { { "Content-Type", "application/json" } });
        }
        
        private static Dictionary<ulong, SteamUserData> IdNames = new Dictionary<ulong, SteamUserData>(10);
        private class SteamUserData
        {
            public string Name { get; set; }
            public string Photo { get; set; }
        }
        public void GetNameFromID(ulong SteamID)
        {
            webrequest.Enqueue($"https://steamcommunity.com/profiles/{SteamID}/?xml=1", null, (code, response) =>
            {
                if(!SteamID.IsSteamId() || (code != 200 || response == null)) { return; }
                string value = ExtractString(response, "steamID");
                if(!IdNames.ContainsKey(SteamID)) { IdNames.Add(SteamID, new SteamUserData()); }
                IdNames[SteamID].Name = value;
            }, this, RequestMethod.GET);
        }
        public void GetPhotoFromID(ulong SteamID)
        {
            IdNames.Clear();
            webrequest.Enqueue($"https://steamcommunity.com/profiles/{SteamID}/?xml=1", null, (code, response) =>
            {
                if (!SteamID.IsSteamId() || (code != 200 || response == null)) { return; }
                string value = ExtractString(response, "avatarFull");
                if (!IdNames.ContainsKey(SteamID)) { IdNames.Add(SteamID, new SteamUserData()); }
                IdNames[SteamID].Photo = value;
            }, this, RequestMethod.GET);
        }
        public string ExtractString(string s, string tag)
        {
            var startTag = "<" + tag + ">";
            int startIndex = s.IndexOf(startTag) + startTag.Length;
            int endIndex = s.IndexOf("</" + tag + ">", startIndex);
            return s.Substring(startIndex, endIndex - startIndex).Replace("<![CDATA[", "").Replace("]]>", "");
        }
        #endregion Helpers

        private void Loaded()
        {
            Instance = this;
            permission.RegisterPermission(Perm, this);
            BanSystem.LoadBans();
        }
        private void Unload() => BanSystem.SaveBans();

        #region Override Default BanSystem
        [ConsoleCommand("ban")]
        private void OnBanDefault(ConsoleSystem.Arg arg)
        {
            var player = arg?.Player();
            if ((arg.IsRcon) || (HasPerm(player)))
            {
                if (arg.IsRcon) { PrintWarning("USE xban.ban"); }
                else { DirectMessage(player, "USE\n<size=16><color=#96c8fa>xban.ban</color></size>"); }
            }
            return;
        }
        [ConsoleCommand("unban")]
        private void OnUnbanDefault(ConsoleSystem.Arg arg)
        {
            var player = arg?.Player();
            if ((arg.IsRcon) || (HasPerm(player)))
            {
                if (arg.IsRcon) { PrintWarning("USE xban.revoke"); }
                else { DirectMessage(player, "USE\n<size=16><color=#96c8fa>xban.revoke</color></size>"); }
            }
        }
        #endregion Override Default BanSystem


        [ConsoleCommand("xban.ban")]
        private void OnExecuteBan(ConsoleSystem.Arg arg)
        {
            var player = arg?.Player();
            try
            {
                var args = string.Join(" ", arg?.Args).Split(',');
                if ((arg.IsRcon && args.Length >= 2) || (HasPerm(player) && args.Length >= 2))
                {
                    var TargetID = string.Join(" ", arg?.Args).Split(',')[0];
                    var Reason = string.Join(" ", arg?.Args).Split(',')[1];
                    try
                    {
                        if(!TargetID.IsSteamId()) 
                        {
                            if (arg.IsRcon) { PrintWarning("Invalid Steam ID"); }
                            else { DirectMessage(player, "Invalid Steam ID"); }
                        }
                        else
                        {
                            var Duration = string.Join(" ", arg?.Args).Split(',')[2];
                            BanSystem.Ban(ulong.Parse(TargetID), Reason, Duration);
                        }
                    }
                    catch 
                    {
                        if (!TargetID.IsSteamId())
                        {
                            if (arg.IsRcon) { PrintWarning("Invalid Steam ID"); }
                            else { DirectMessage(player, "Invalid Steam ID"); }
                        }
                        else { BanSystem.Ban(ulong.Parse(TargetID), Reason); }
                    }
                }
                else if ((arg.IsRcon || HasPerm(player)))
                {
                    if (arg.IsRcon) { PrintWarning("Missing Args steamid,reason,1m2w3d4h"); }
                    else { DirectMessage(player, $"Missing Args\n<size=16><color={MsgColor}>steamid</color>,<color={MsgColor}>reason</color>,<color={MsgColor}>1m2w3d4h</color></size>"); }
                }
            }
            catch
            {
                if ((arg.IsRcon|| HasPerm(player)))
                {
                    if (arg.IsRcon) { PrintWarning("Missing Args steamid,reason,1m2w3d4h"); }
                    else { DirectMessage(player, $"Missing Args\n<size=16><color={MsgColor}>steamid</color>,<color={MsgColor}>reason</color>,<color={MsgColor}>1m2w3d4h</color></size>"); }
                }
            }
        }

        [ConsoleCommand("xban.revoke")]
        private void OnRevokeBan(ConsoleSystem.Arg arg)
        {
            var player = arg?.Player();
            try
            {
                if ((arg.IsRcon && arg?.Args.Length == 2) || (HasPerm(player) && arg?.Args.Length == 2))
                {
                    var TargetID = arg?.Args[0];
                    var BanIndex = arg?.Args[1];
                    try
                    {
                        if (!TargetID.IsSteamId())
                        {
                            if (arg.IsRcon) { PrintWarning("Invalid Steam ID"); }
                            else { DirectMessage(player, "Invalid Steam ID"); }
                        }
                        else
                        {
                            if (!BanSystem.ContainsBanIndex(ulong.Parse(TargetID), int.Parse(BanIndex)))
                            {
                                if (arg.IsRcon) { PrintWarning($"Failed To Revoke BanIndex: {BanIndex} On User ({TargetID})"); }
                                else { DirectMessage(player, $"<size=16>Failed To Revoke BanIndex: <color={MsgColor}>{BanIndex}</color> On User (<color={MsgColor}>{TargetID}</color>)</size>"); }
                            }
                            else { BanSystem.Revoke(ulong.Parse(TargetID), int.Parse(BanIndex)); }
                        }
                    }
                    catch
                    {
                        if (arg.IsRcon) { PrintWarning($"Failed To Revoke BanIndex: {BanIndex} On User ({TargetID})"); }
                        else { DirectMessage(player, $"<size=16>Failed To Revoke BanIndex: <color={MsgColor}>{BanIndex}</color> On User (<color={MsgColor}>{TargetID}</color>)</size>"); }
                    }
                }
                else if (arg.IsRcon || HasPerm(player))
                {
                    if (arg.IsRcon) { PrintWarning("Invalid Args (steamid,reason,1m2w3d4h)"); }
                    else { DirectMessage(player, $"Invalid Args\n<size=16><color={MsgColor}>steamid</color>,<color={MsgColor}>reason</color>,<color={MsgColor}>1m2w3d4h</color></size>"); }
                }
            }
            catch
            {
                if ((arg.IsRcon || HasPerm(player)))
                {
                    if (arg.IsRcon) { PrintWarning("Missing Args steamid banindex"); }
                    else { DirectMessage(player, $"Missing Args\n<size=16><color={MsgColor}>steamid</color> <color={MsgColor}>banindex</color></size>"); }
                }
            }
        }

        private class BanSystem
        {
            private static Dictionary<ulong, Records> BansDB = new Dictionary<ulong, Records>();
            public class Records
            {
                public Dictionary<int, Data> Bans = new Dictionary<int, Data>();
                public int TotalBans { get; set; } = 0;
                public class Data
                {
                    public string Reason { get; set; } = "";
                    public DateTime ExpireAt { get; set; } = DateTime.UtcNow;
                }
            }

            public static DateTime FormatTime(string Duration)
            {
                var LowDuration = Duration.ToLower().Trim();

                if (LowDuration == "-1") { return DateTime.UtcNow.AddYears(1000); }
                else
                {
                    int Years = 0;
                    int Months = 0;
                    int Weeks = 0;
                    int Days = 0;
                    int Hours = 0;

                    string combiner = "";
                    for (int i = 0; i < LowDuration.Length; i++)
                    {
                        if (char.IsDigit(LowDuration[i])) { combiner += LowDuration[i].ToString(); }
                        switch (LowDuration[i])
                        {
                            case 'y':
                                Years = Convert.ToInt32(combiner); combiner = ""; break;
                            case 'm':
                                Months = Convert.ToInt32(combiner); combiner = ""; break;
                            case 'w':
                                Weeks = Convert.ToInt32(combiner); combiner = ""; break;
                            case 'd':
                                Days = Convert.ToInt32(combiner); combiner = ""; break;
                            case 'h':
                                Hours = Convert.ToInt32(combiner); combiner = ""; break;
                        }
                    }
                    float resultDays = (Years * 365 + Months * 30 + Weeks * 7 + Days + Hours / 24);
                    return DateTime.UtcNow.AddDays(resultDays);
                }
            }

            public static Records.Data GetBan(ulong SteamID)
            {
                if (BansDB.ContainsKey(SteamID))
                {
                    var time = BansDB[SteamID].Bans.Max(x => x.Value.ExpireAt);
                    var ban = BansDB[SteamID].Bans.Where(x => x.Value.ExpireAt == time).FirstOrDefault();
                    return ban.Value;
                }
                return null;
            }

            public static bool IsBanned(ulong SteamID)
            {
                if (BansDB.ContainsKey(SteamID) && BansDB[SteamID].Bans.Any(x => x.Value.ExpireAt > DateTime.UtcNow)) { return true; }
                else { return false; }
            }

            public static double GetRemainingDays(ulong SteamID)
            {
                if (BanSystem.IsBanned(SteamID))
                {
                    var ban = BanSystem.GetBan(SteamID);
                    var days = (ban.ExpireAt - DateTime.UtcNow).TotalDays;
                    return days;
                }
                return 0;
            }

            public static string GetBanMsg(ulong SteamID)
            {
                if (BanSystem.IsBanned(SteamID))
                {
                    var ban = BanSystem.GetBan(SteamID);
                    string timeLeft = ban.ExpireAt.Year >= 200 ? "Permanent" : ban.ExpireAt.ToString("y:M:d:H");

                    string str = $"" +
                    $"Du er blevet udelukket af årsagen: <color={TitleColor}>{ban.Reason}</color> -- " +
                    $"Varighed: <color={TitleColor}>{timeLeft}</color> -- " +
                    $"Du kan anke din udelukkelse her: <color={TitleColor}>{DiscordURL.Replace("https://", "")}</color>";
                    return str;
                }
                return "";
            }

            public static bool ContainsBanIndex(ulong SteamID, int BanIndex) => (BansDB.ContainsKey(SteamID) && BansDB[SteamID].Bans.ContainsKey(BanIndex));
   

            public static void Ban(ulong SteamID, string reason, string Duration = "-1")
            {

                if(Instance.StaffID.Contains(SteamID)) { return; }
                IdNames.Clear();
                Instance.GetNameFromID(SteamID);
                Instance.GetPhotoFromID(SteamID);
                if (!BansDB.ContainsKey(SteamID)) { BansDB.Add(SteamID, new Records { }); }
                int BanIndex = BansDB[SteamID].Bans.Count + 1;
                BansDB[SteamID].TotalBans = BanIndex;
                BansDB[SteamID].Bans.Add(BanIndex, new Records.Data
                {
                    ExpireAt = FormatTime(Duration),
                    Reason = reason,
                });

                Instance.timer.Once(5f, () =>
                {
                    var Days = (FormatTime(Duration) - DateTime.UtcNow).TotalDays;
                    string DisplayDuration = Duration == "-1" ? $"- <color={TitleColor}>Permanent</color>" : $"i <color={TitleColor}>{Days}</color> Dage";
                    Instance.Server.Broadcast($"<size=14>Spilleren <color={TitleColor}>{IdNames[SteamID].Name}</color> (<color={TitleColor}>{SteamID}</color>) Er blevet udelukket af årsagen <color={TitleColor}>{reason.Trim()}</color> {DisplayDuration}", IconID);
                    Instance.DiscordBanGiven(IdNames[SteamID].Name, SteamID, (int)Days, reason, BansDB[SteamID].TotalBans, IdNames[SteamID].Photo);
                    IdNames.Clear();
                });

                SaveBans();
                try { BasePlayer.FindByID(SteamID).Kick(GetBanMsg(SteamID)); }
                catch { }
            }
            public static void Revoke(ulong SteamID, int BanIndex)
            {
                if(BansDB.ContainsKey(SteamID))
                {   
                    IdNames.Remove(SteamID);
                    Instance.GetNameFromID(SteamID);
                    Instance.GetPhotoFromID(SteamID);

                    if(BansDB[SteamID].TotalBans <= 1) 
                    { 
                        BansDB.Remove(SteamID);
                        Instance.timer.Once(1f, () =>
                        {
                            Instance.DiscordBanRevoked(IdNames[SteamID].Name, SteamID, BanIndex, IdNames[SteamID].Photo);
                            IdNames.Clear();
                        });
                    }
                    else if (ContainsBanIndex(SteamID,BanIndex))
                    { 
                        BansDB[SteamID].Bans.Remove(BanIndex);

                        Instance.timer.Once(1f, () =>
                        {
                            Instance.DiscordBanRevoked(IdNames[SteamID].Name, SteamID, BanIndex, IdNames[SteamID].Photo);
                            IdNames.Clear();
                        });
                    }
                    SaveBans();
                }
            }

            public static void LoadBans() 
            {
                if (!Interface.Oxide.DataFileSystem.ExistsDatafile(BansLocation) || Interface.Oxide.DataFileSystem.ReadObject<Dictionary<ulong, Records>>(BansLocation) == null)
                { Interface.Oxide.DataFileSystem.WriteObject(BansLocation, new Dictionary<ulong, Records>());}
                BansDB = Interface.Oxide.DataFileSystem.ReadObject<Dictionary<ulong, Records>>(BansLocation);
            }
            public static void SaveBans() => Interface.Oxide.DataFileSystem.WriteObject(BansLocation, BansDB);
        }

        private bool CanClientLogin(Network.Connection conn)
        {
            if (BanSystem.IsBanned(conn.userid))
            {
                timer.Once(1f, () => Network.Net.sv.Kick(conn, BanSystem.GetBanMsg(conn.userid), false));
            }
            return true;
        }



        void OnPlayerKicked(BasePlayer player, string reason)
        {
            if(reason.Contains("[PVE]"))
            {
                IdNames.Clear();
                Instance.GetPhotoFromID(Convert.ToUInt64(player.userID));
                Instance.timer.Once(5f, () =>
                {
                    string body = "{\"content\":null,\"embeds\":[{\"description\":\"**KICK GIVEN**᲼👟\\n>>> **SERVER**᲼__" + ConVar.Server.hostname + "__\\n**REASON**᲼" + reason + "\",\"color\":4960250,\"author\":{\"name\":\"" + player.displayName + " - " + player.userID + "\",\"url\":\"https://steamcommunity.com/profiles/" + player.userID + "\",\"icon_url\":\"" + IdNames[Convert.ToUInt64(player.userID)].Photo + "\"}}],\"attachments\":[]}";
                    webrequest.Enqueue(DiscordWebHookKick, body, (code, response) => { }, this, RequestMethod.POST, new Dictionary<string, string> { { "Content-Type", "application/json" } });
                    IdNames.Clear();
                });
            }

        }


        #region On Better Chat Mute
        private void OnBetterChatMuted(Core.Libraries.Covalence.IPlayer target, Core.Libraries.Covalence.IPlayer initiator, string reason)
        {
            IdNames.Clear();
            Instance.GetPhotoFromID(Convert.ToUInt64(target.Id));
            
            Instance.timer.Once(5f, () =>
            {
                string body = "{\"content\":null,\"embeds\":[{\"description\":\"**MUTE GIVEN**᲼🔨\\n>>> **SERVER**᲼__"+ ConVar.Server.hostname + "__\\n**DURATION**᲼Permanent\\n**REASON**᲼" + reason+"\",\"color\":4960250,\"author\":{\"name\":\""+target.Name+" - "+target.Id+ "\",\"url\":\"https://steamcommunity.com/profiles/"+target.Id+"\",\"icon_url\":\""+ IdNames[Convert.ToUInt64(target.Id)].Photo + "\"}}],\"attachments\":[]}";
                webrequest.Enqueue(DiscordWebHookBetterChatMute, body, (code, response) => { }, this, RequestMethod.POST, new Dictionary<string, string> { { "Content-Type", "application/json" } });
                IdNames.Clear();
            });
        }

        private void OnBetterChatTimeMuted(Core.Libraries.Covalence.IPlayer target, Core.Libraries.Covalence.IPlayer initiator, TimeSpan time, string reason)
        {
            IdNames.Clear();
            Instance.GetPhotoFromID(Convert.ToUInt64(target.Id));

            Instance.timer.Once(1f, () =>
            {
                string body = "{\"content\":null,\"embeds\":[{\"description\":\"**MUTE GIVEN**᲼🔨\\n>>> **SERVER**᲼__" + ConVar.Server.hostname + "__\\n**DURATION**᲼"+time.TotalDays+" Dage\\n**REASON**᲼" + reason + "\",\"color\":4960250,\"author\":{\"name\":\"" + target.Name + " - " + target.Id + "\",\"url\":\"https://steamcommunity.com/profiles/" + target.Id + "\",\"icon_url\":\"" + IdNames[Convert.ToUInt64(target.Id)].Photo + "\"}}],\"attachments\":[]}";
                webrequest.Enqueue(DiscordWebHookBetterChatMute, body, (code, response) => { }, this, RequestMethod.POST, new Dictionary<string, string> { { "Content-Type", "application/json" } });
                IdNames.Clear();
            });
        }

        private void OnBetterChatUnmuted(Core.Libraries.Covalence.IPlayer target, Core.Libraries.Covalence.IPlayer initiator)
        {
            IdNames.Clear();
            Instance.GetPhotoFromID(Convert.ToUInt64(target.Id));

            Instance.timer.Once(1f, () =>
            {
                string body = "{\"content\":null,\"embeds\":[{\"description\":\"**MUTE REVOKED**᲼↩️\\n>>> **SERVER**᲼__" + ConVar.Server.hostname + "__\",\"color\":4960250,\"author\":{\"name\":\"" + target.Name + " - " + target.Id + "\",\"url\":\"https://steamcommunity.com/profiles/" + target.Id + "\",\"icon_url\":\"" + IdNames[Convert.ToUInt64(target.Id)].Photo + "\"}}],\"attachments\":[]}";
                webrequest.Enqueue(DiscordWebHookBetterChatMute, body, (code, response) => { }, this, RequestMethod.POST, new Dictionary<string, string> { { "Content-Type", "application/json" } });
                IdNames.Clear();
            });
        }
        #endregion On Better Chat Mute
    }
}