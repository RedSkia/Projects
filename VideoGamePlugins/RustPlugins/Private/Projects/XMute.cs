using ConVar;
using Newtonsoft.Json;
using Oxide.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oxide.Plugins
{
    [Info("XMute", "Infinitynet.dk", "1.0.0")]
    [Description("Copyright Infinitynet.dk All Rights Reserved")]
    public class XMute : RustPlugin
    {
        #region Configuration

        private Configuration config;

        //Default Config
        public class Configuration
        {

            [JsonProperty("ServerLogs Update Frequency (Seconds)")]

            public int ServerMuteUpdateFrequency { get; set; } = 60;


            [JsonProperty("Cooldown Messsage (Seconds)")]

            public int ServerMuteMessageCooldown { get; set; } = 10;
        }

        protected override void LoadConfig()
        {
            base.LoadConfig();
            try
            {
                config = Config.ReadObject<Configuration>();
                if (config == null)
                {
                    LoadDefaultConfig();
                }
            }
            catch
            {
                LoadDefaultConfig();
            }
            SaveConfig();
        }

        protected override void LoadDefaultConfig()
        {
            string configPath = $"{Interface.Oxide.ConfigDirectory}{Path.DirectorySeparatorChar}{Name}.json";
            PrintWarning("[ERROR] Invalid Configuration Overwriting");
            config = new Configuration();
        }

        protected override void SaveConfig() => Config.WriteObject(config);

        #endregion Configuration


        #region Command Cooldown
        static Dictionary<ulong, PlayerCooldowns> CommandCooldown = new Dictionary<ulong, PlayerCooldowns>();

        private class PlayerCooldowns
        {
            public DateTime CommandCooldownExpire;

            public bool ExpireTimeSet = false;

        }


        bool SendMuteMessage(BasePlayer player, bool skipCooldown)
        {

            var currentTime = Convert.ToDateTime(DateTime.UtcNow.ToString("HH:mm:ss"));



            bool ReturnExpired = false;
            if (CommandCooldown.ContainsKey(player.userID))
            {

                if (!CommandCooldown[player.userID].ExpireTimeSet)
                {
                    CommandCooldown[player.userID].CommandCooldownExpire = currentTime.AddSeconds(config.ServerMuteMessageCooldown);
                    CommandCooldown[player.userID].ExpireTimeSet = true;
                }


                if (currentTime >= CommandCooldown[player.userID].CommandCooldownExpire || skipCooldown)
                {

                    CommandCooldown[player.userID].ExpireTimeSet = false;

                    ReturnExpired = true;

                    SendReply(player, $"{TextEncodeing(16, "c83232", "Server")}{TextEncodeing(14, "ffffff", ":")} {TextEncodeing(14, "ffffff", "You're muted for")} {TextEncodeing(14, "3296fa", $"{PlayerMute.GetReason(player.userID)}")} \n{TextEncodeing(14, "ffffff", "expires at")} {TextEncodeing(14, "3296fa", $"{PlayerMute.GetExpire(player.userID).Replace("UTC", TextEncodeing(14, "ffffff", "UTC"))}")}");

                }


            }
            else if (!CommandCooldown.ContainsKey(player.userID))
            {
                CommandCooldown.Add(player.userID, new PlayerCooldowns());
            }

            return ReturnExpired;
        }


        #endregion



        string TextEncodeing(double TextSize, string HexColor, string PlainText) => $"<size={TextSize}><color=#{HexColor}>{PlainText}</color></size>";

        private void Init()
        {

            timer.Every(config.ServerMuteUpdateFrequency, () =>
            {
                SortedList<ulong, string> list = Interface.Oxide.DataFileSystem.ReadObject<SortedList<ulong, string>>("XMute/List");


                foreach (var i in list)
                {
                    string[] args = i.Value.Split('|');
                    string reason = args[0];
                    string expire = args[1];

                    if (DateTimeOffset.UtcNow.ToUnixTimeSeconds() >= (Convert.ToInt64(expire)) && expire != "-1")
                    {
                        try
                        {
                            foreach (BasePlayer player in BasePlayer.activePlayerList)
                            {
                                if (player.userID == i.Key)
                                {
                                    SendReply(player, $"{TextEncodeing(16, "c83232", "Server")}{TextEncodeing(14, "ffffff", ":")} {TextEncodeing(14, "ffffff", "You have been")} {TextEncodeing(14, "3296fa", "Unmuted")}");

                                }
                            }
                        }
                        catch { }

                    }
                }

            });

        }


        public class PlayerMute
        {

            internal static string GetReason(ulong id)
            {
                SortedList<ulong, string> list = Interface.Oxide.DataFileSystem.ReadObject<SortedList<ulong, string>>("XMute/List");

                foreach (var i in list)
                {
                    string[] args = i.Value.Split('|');
                    string reason = args[0];
                    //string expire = args[1];
                    return reason.Replace(".", " ");
                }
                return "NULL";
            }
            internal static string GetExpire(ulong id)
            {
                SortedList<ulong, string> list = Interface.Oxide.DataFileSystem.ReadObject<SortedList<ulong, string>>("XMute/List");

                foreach (var i in list)
                {
                    string[] args = i.Value.Split('|');
                    //string reason = args[0];
                    string expire = args[1];

                    if (Convert.ToInt32(expire) <= 0 || expire == "-1") //Permanent
                    {
                        expire = "Never";
                    }
                    else //Timed
                    {
                        expire = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(expire)).ToString("HH:mm UTC dd/MM/yyyy");
                    }


                    return expire;
                }
                return "NULL";
            }


            internal static void Mute(ulong id, string args)
            {
                SortedList<ulong, string> list = Interface.Oxide.DataFileSystem.ReadObject<SortedList<ulong, string>>("XMute/List");

                if (args.Count() >= 1)
                {
                    if (list.ContainsKey(id))
                    {
                        list.Remove(id);
                        list.Add(id, args);
                        Interface.Oxide.DataFileSystem.WriteObject("XMute/List", list, true);
                    }
                    else
                    {
                        list.Add(id, args);
                        Interface.Oxide.DataFileSystem.WriteObject("XMute/List", list, true);
                    }
                }
            }

            internal static void Unmute(ulong id)
            {
                SortedList<ulong, string> list = Interface.Oxide.DataFileSystem.ReadObject<SortedList<ulong, string>>("XMute/List");
                if (list.ContainsKey(id))
                {
                    list.Remove(id);
                    Interface.Oxide.DataFileSystem.WriteObject("XMute/List", list, true);

                }
            }

            internal static bool Muted(ulong id)
            {
                SortedList<ulong, string> list = Interface.Oxide.DataFileSystem.ReadObject<SortedList<ulong, string>>("XMute/List");

                if (list.ContainsKey(id))
                {
                    return true;
                }

                return false;
            }

        }



        #region Hooks
        void CanUpdateSign(BasePlayer player, Signage sign)
        {
            if (PlayerMute.Muted(player.userID))
            {
                try
                {
                    sign.DieInstantly();
                }
                catch { }
                SendMuteMessage(player, true);
            }
        }


        object OnPlayerChat(BasePlayer player, string message, Chat.ChatChannel channel)
        {

            if (PlayerMute.Muted(player.userID))
            {
                SendMuteMessage(player, true);
                return false;
            }
            return null;
        }

        object OnPlayerVoice(BasePlayer player, Byte[] data)
        {
            if (PlayerMute.Muted(player.userID))
            {
                SendMuteMessage(player, false);
                return false;
            }
            return null;
        }
        #endregion



        #region API
        [ConsoleCommand("xmute")]
        private void API_Mute(ConsoleSystem.Arg arg)
        {
            try
            {
                if (arg.HasArgs(2))
                {

                    ulong targetID = Convert.ToUInt64(arg.Args[0]);
                    var reason = arg.Args[1];
                    var expire = arg.Args[2];
                    if (Convert.ToInt32(expire) <= 0 || expire.ToString() == "-1")
                    {
                        PlayerMute.Mute(Convert.ToUInt64(targetID), $"{reason}|-1");
                    }
                    else
                    {
                        var expireOffset = DateTimeOffset.UtcNow.AddHours(Convert.ToInt32(arg.Args[2])).ToUnixTimeSeconds();
                        PlayerMute.Mute(Convert.ToUInt64(targetID), $"{reason}|{expireOffset}");
                    }

                    foreach (BasePlayer player in BasePlayer.activePlayerList)
                    {
                        if (player.userID == targetID)
                        {
                            SendReply(player, $"{TextEncodeing(16, "c83232", "Server")}{TextEncodeing(14, "ffffff", ":")} {TextEncodeing(14, "ffffff", "You have been")} {TextEncodeing(14, "3296fa", "Muted")}");
                        }
                    }
                }
            }
            catch { }



        }
        [ConsoleCommand("xunmute")]
        private void API_UNMute(ConsoleSystem.Arg arg)
        {
            try
            {
                if (arg.HasArgs(1))
                {

                    ulong targetID = Convert.ToUInt64(arg.Args[0]);

                    PlayerMute.Unmute(targetID);



                    foreach (BasePlayer player in BasePlayer.activePlayerList)
                    {
                        if (player.userID == targetID)
                        {
                            SendReply(player, $"{TextEncodeing(16, "c83232", "Server")}{TextEncodeing(14, "ffffff", ":")} {TextEncodeing(14, "ffffff", "You have been")} {TextEncodeing(14, "3296fa", "Unmuted")}");

                        }
                    }
                }
            }
            catch { }
        }
        #endregion

    }
}