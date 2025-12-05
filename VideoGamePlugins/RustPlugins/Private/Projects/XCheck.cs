using Newtonsoft.Json;
using Oxide.Core;
using Oxide.Core.Libraries.Covalence;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

using Oxide.Core.Configuration;
using ConVar;
using Network;
using Oxide.Core.Libraries;
using System.Linq;

namespace Oxide.Plugins
{
    [Info("XCheck", "Infinitynet.dk/Xray", "1.0.0")]
    [Description("Copyright Infinitynet.dk All Rights Reserved")]
    public class XCheck : RustPlugin
    {
        #region Configuration
        private const string SteamApiKey = "xxxxxxxxxxxxxxxxxxx";
        private const bool UseXBan = true;

        public const string DiscordWebHookChecks = "https://discord.com/api/webhooks/973946162874810388/lrSenWDgdB8BynxXDF0lr-GbZv3UwM8lUg2eiv87fkf1Q7pZy4fHpOmWdqdTehNYhWe3";

        public const string TitleColor = "#4baffa";
        public const string MsgColor = "#96c8fa";

        private const int Minimum_DaysSinceBan = 180;
        private const int Maximum_Bans = 3;
        #endregion Configuration

        private static readonly Dictionary<ulong, CachedDetections> cachedDetections = new Dictionary<ulong, CachedDetections>();
        private class CachedDetections
        {
            public string Reason { get; set; }

            public Player player { get; set; }
            public class Player
            {
                public int NumberOfVACBans { get; set; }
                public int DaysSinceLastBan { get; set; }
                public int NumberOfGameBans { get; set; }
            }
        }

        #region Helpers
        private static Dictionary<ulong, SteamUserData> IdNames = new Dictionary<ulong, SteamUserData>();
        private class SteamUserData
        {
            public string Name { get; set; }
            public string Photo { get; set; }
        }
        
        public void GetNameFromID(ulong SteamID)
        {
            webrequest.Enqueue($"https://steamcommunity.com/profiles/{SteamID}/?xml=1", null, (code, response) =>
            {
                if (!SteamID.IsSteamId() || (code != 200 || response == null)) { return; }
                string value = ExtractString(response, "steamID");
                if (!IdNames.ContainsKey(SteamID)) { IdNames.Add(SteamID, new SteamUserData()); }
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

        private void DiscordCheckFailed(ulong TargetID, string Reason)
        {
            IdNames.Clear();
            GetNameFromID(TargetID);
            GetPhotoFromID(TargetID);
            timer.Once(5f, () =>
            {
                try
                {
                    string body = "{\"content\":null,\"embeds\":[{\"description\":\"**STATUS**᲼❌\\n>>> **REASON**᲼" + Reason + "\\n**SERVER**᲼__" + ConVar.Server.hostname + "__\",\"color\":4960250,\"author\":{\"name\":\"" + IdNames[TargetID].Name + " - " + TargetID + "\",\"url\":\"https://steamcommunity.com/profiles/" + TargetID + "\",\"icon_url\":\"" + IdNames[TargetID].Photo + "\"}}],\"attachments\":[]}";
                    webrequest.Enqueue(DiscordWebHookChecks, body, (code, response) => { }, this, RequestMethod.POST, new Dictionary<string, string> { { "Content-Type", "application/json" } });
                } catch { }

                IdNames.Clear();
            });
        }
        private void DiscordCheckPassed(ulong TargetID)
        {
            IdNames.Clear();
            GetNameFromID(TargetID);
            GetPhotoFromID(TargetID);
            timer.Once(5f, () =>
            {
                try
                {
                    string body = "{\"content\":null,\"embeds\":[{\"description\":\"**STATUS**᲼✅\\n>>> **SERVER**᲼__" + ConVar.Server.hostname + "__\",\"color\":4960250,\"author\":{\"name\":\"" + IdNames[TargetID].Name + " - " + TargetID + "\",\"url\":\"https://steamcommunity.com/profiles/" + TargetID + "\",\"icon_url\":\"" + IdNames[TargetID].Photo + "\"}}],\"attachments\":[]}";
                    webrequest.Enqueue(DiscordWebHookChecks, body, (code, response) => { }, this, RequestMethod.POST, new Dictionary<string, string> { { "Content-Type", "application/json" } });
                } catch { }

                IdNames.Clear();
            });
        }
        #endregion Helpers

        private void DoCheck(ulong ID, bool firstTime)
        {
            timer.Once(1f, () =>
            {
                if (cachedDetections.ContainsKey(ID))
                {
                    var conn = Network.Net.sv.connections.Where(x => x.userid == ID).ToArray()[0];
                    var key = cachedDetections[ID];
                    if (key.player.NumberOfVACBans + key.player.NumberOfGameBans >= Maximum_Bans)
                    {
                        if (UseXBan) { Server.Command($"xban.ban {ID},{key.Reason}"); }
                        else { Network.Net.sv.Kick(conn, "Du har for mange spil udelukkelser til at kunne spille på vores servere"); }
                        DiscordCheckFailed(ID, $"Spiller havde for mange spil udelukkelser. Udelukkelser ({key.player.NumberOfVACBans + key.player.NumberOfGameBans})");
                    }
                    if (key.player.DaysSinceLastBan > Minimum_DaysSinceBan) 
                    {
                        Network.Net.sv.Kick(conn, cachedDetections[ID].Reason);
                        DiscordCheckFailed(ID, $"Spilleren er nylig blevet i et spil inden for de sidste {Minimum_DaysSinceBan} dage. Sidste udelukkelse for ({key.player.DaysSinceLastBan}) Dage siden.");
                    }
                    return;
                }
         
     
                webrequest.Enqueue($"https://api.steampowered.com/ISteamUser/GetPlayerBans/v1/?key={SteamApiKey}&steamids={ID}", null, (code, response) =>
                {
                    if (code != 200 || response == null) { return; }
                    var playerData = JsonConvert.DeserializeObject<CachedDetections.Player>(response);
                    var conn = Network.Net.sv.connections.Find(x => x.userid == ID);
                    
                    if (playerData.NumberOfVACBans + playerData.NumberOfGameBans >= Maximum_Bans)
                    {
                        if (!cachedDetections.ContainsKey(ID))
                        {
                            cachedDetections.Add(ID, new CachedDetections
                            {
                                Reason = $"Du har for mange spil <color={TitleColor}>udelukkelser</color> til at kunne spille på vores servere",
                                player = playerData
                            });
                        }

                        if (UseXBan) { Server.Command($"xban.ban {ID},{cachedDetections[ID].Reason}"); } 
                        else { Network.Net.sv.Kick(conn, cachedDetections[ID].Reason); }
                    
                        DiscordCheckFailed(ID, $"Spiller havde for mange spil udelukkelser. Udelukkelser ({playerData.NumberOfVACBans+playerData.NumberOfGameBans})");
                        return;
                    }

                    PrintToChat(playerData.DaysSinceLastBan.ToString());
                    if (playerData.DaysSinceLastBan > Minimum_DaysSinceBan)
                    {
                        if (!cachedDetections.ContainsKey(ID))
                        {
                            cachedDetections.Add(ID, new CachedDetections
                            {
                                Reason = $"Du er for nylig blevet udelukket i et spil inden for de sidste <color={TitleColor}>{Minimum_DaysSinceBan}</color> dage og må derfor vente med at spille på vores severe",
                                player = playerData
                            });
                        }
                        Network.Net.sv.Kick(conn, cachedDetections[ID].Reason);
                        DiscordCheckFailed(ID, $"Spilleren er nylig blevet i et spil inden for de sidste {Minimum_DaysSinceBan} dage. Sidste udelukkelse for ({playerData.DaysSinceLastBan}) Dage siden.");
                        return;
                    }
                    if(firstTime) { DiscordCheckPassed(ID); }
                }, this, RequestMethod.GET);
            });
        }
        private void CanClientLogin(Network.Connection conn) => DoCheck(conn.userid, true);
        private void OnPlayerRespawned(BasePlayer player) => DoCheck(player.userID, false);
    }
}