using Oxide.Core;
using Oxide.Core.Libraries;
using Oxide.Core.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Oxide.Plugins
{
    [Info("XAd", "InfinityNet/Xray", "1.0.0")]
    [Description("Copyright Infinitynet.dk All Rights Reserved")]
    public class XAd : RustPlugin
    {
        public static XAd Instance { get; set; }

        #region Configuration
        public static ulong[] StaffID = new ulong[] { 76561198066857380, 76561198023076226 };
        public const string Perm = "XAd.skip";
        public const ulong IconID = 76561198389709969;

        public const int AdInterval_VIP = 3600;
        public const int AdInterval_Booster = 1800;
        public const int AdInterval_Verified = 1200;
        public const int AdInterval_Default = 900;

        public const string TitleColor = "#4baffa";
        public const string MsgColor = "#96c8fa";

        private class Cfg_Messages
        {
            public static string ServerVote = $"Stem på serveren og modtag en belønning\nStart med at skrive <color={TitleColor}>/vote</color>";

            public static string BuyVIP = $"Bliv <color=#faaf19>VIP</color> for at få den ultimative oplevelse\nVip kan købes på vores hjemmeside <color={TitleColor}>Infinitynet.dk</color>";
            public static string BoostDiscord = "Boost vores discord server og modtag <color=#f47fff>Discord Booster</color> rang med alle dets fordele";
            public static string GetVerified = $"Bliv <color=#32c873>Verificeret</color>\nForbind din discord konto i kanalen <color={TitleColor}>Link-Konto</color>\nTilslut og vær med i vores steam gruppe\n<color={TitleColor}>Discord</color>: discord.gg/zBcSctH6rF\n<color={TitleColor}>Steam</color>: steamcommunity.com/groups/InfinityNet";
            public static string MissingSteam = $"Du er næsten <color=#32c873>Verificeret</color> du mangler blot at være med i vores steam gruppe\n<color={TitleColor}>Steam</color>: steamcommunity.com/groups/InfinityNet";
            public static string MissingDiscord = $"Du er næsten <color=#32c873>Verificeret</color> du mangler blot at forbinde din discord konto i kanalen <color={TitleColor}>Link-Konto</color>\n<color={TitleColor}>Discord</color>: discord.gg/zBcSctH6rF";
        }
        private class GroupList
        {
            public const string Staff = "administrator";
            public const string VIP3 = "vip3";
            public const string VIP2 = "vip2";
            public const string VIP1 = "vip1";
            public const string DiscordBooster = "discordbooster";
            public const string Verified = "verified";
            public const string Steam = "steam";
            public const string Discord = "discord";
        }


        private static string[] VipMessages = new string[]
        {
            Cfg_Messages.ServerVote,
        };
        private static string[] BoosterMessages = new string[]
        {
            Cfg_Messages.ServerVote,
            Cfg_Messages.BuyVIP,
        };
        private static string[] VerifiedMessages = new string[]
        {
            Cfg_Messages.ServerVote,
            Cfg_Messages.BuyVIP,
            Cfg_Messages.BoostDiscord,
        };
        private static string[] DiscordMessages = new string[]
        {
            Cfg_Messages.ServerVote,
            Cfg_Messages.BuyVIP,
            Cfg_Messages.BoostDiscord,
            Cfg_Messages.GetVerified,
            Cfg_Messages.MissingSteam,
        };
        private static string[] SteamMessages = new string[]
        {
            Cfg_Messages.ServerVote,
            Cfg_Messages.BuyVIP,
            Cfg_Messages.BoostDiscord,
            Cfg_Messages.GetVerified,
            Cfg_Messages.MissingDiscord,
        };

        private static string[] DefaultMessages = new string[]
        {
            Cfg_Messages.ServerVote,
            Cfg_Messages.BuyVIP,
            Cfg_Messages.BoostDiscord,
            Cfg_Messages.GetVerified,
        };
        #endregion Configuration

        Dictionary<ulong, int> messageLast = new Dictionary<ulong, int>();
  
        private void Loaded()
        {
            Instance = this;
            permission.RegisterPermission(Perm, this);


            timer.Every((float)AdInterval_VIP, () =>
            {
                var list = BasePlayer.activePlayerList.Where(x => x != null &&
                !Groups.IsStaff(x) && 
                Groups.IsVIP(x)
                ).ToList();
                foreach (BasePlayer player in list)
                {
                    if (!messageLast.ContainsKey(player.userID)) { messageLast.Add(player.userID, 0); }
                    if (messageLast[player.userID] >= VipMessages.Length) { messageLast[player.userID] = 0; }
                    SendAd(player, VipMessages[messageLast[player.userID]]); 
                    messageLast[player.userID]++; 
                }
            });

            timer.Every((float)AdInterval_Booster, () =>
            {
                var list = BasePlayer.activePlayerList.Where(x => x != null &&
                !Groups.IsStaff(x) &&
                !Groups.IsVIP(x) &&
                Groups.IsDiscordBooster(x)
                ).ToList();
                foreach (BasePlayer player in list)
                {
                    if (!messageLast.ContainsKey(player.userID)) { messageLast.Add(player.userID, 0); }
                    if (messageLast[player.userID] >= BoosterMessages.Length) { messageLast[player.userID] = 0; }
                    SendAd(player, BoosterMessages[messageLast[player.userID]]);
                    messageLast[player.userID]++;
                }
            });

            timer.Every((float)AdInterval_Verified, () =>
            {
                var list = BasePlayer.activePlayerList.Where(x => x != null &&
                !Groups.IsStaff(x) &&
                !Groups.IsVIP(x) &&
                !Groups.IsDiscordBooster(x) &&
                Groups.IsVerified(x)
                ).ToList();
                foreach (BasePlayer player in list)
                {
                    if (!messageLast.ContainsKey(player.userID)) { messageLast.Add(player.userID, 0); }
                    if (messageLast[player.userID] >= VerifiedMessages.Length) { messageLast[player.userID] = 0; }
                    SendAd(player, VerifiedMessages[messageLast[player.userID]]);
                    messageLast[player.userID]++;
                }
            });

            timer.Every((float)AdInterval_Default, () =>
            {
                var list = BasePlayer.activePlayerList.Where(x => x != null &&
                !Groups.IsStaff(x) &&
                !Groups.IsVIP(x) &&
                !Groups.IsDiscordBooster(x) &&
                !Groups.IsVerified(x) &&
                (Groups.IsSteam(x) || Groups.IsDiscord(x))
                ).ToList();
                foreach (BasePlayer player in list)
                {
                    if(Groups.IsSteam(player) && !Groups.IsDiscord(player))
                    {
                        if (!messageLast.ContainsKey(player.userID)) { messageLast.Add(player.userID, 0); }
                        if (messageLast[player.userID] >= SteamMessages.Length) { messageLast[player.userID] = 0; }
                        SendAd(player, SteamMessages[messageLast[player.userID]]);
                        messageLast[player.userID]++;
                    }

                    if (Groups.IsDiscord(player) && !Groups.IsSteam(player))
                    {
                        if (!messageLast.ContainsKey(player.userID)) { messageLast.Add(player.userID, 0); }
                        if (messageLast[player.userID] > DiscordMessages.Length) { messageLast[player.userID] = 0; }
                        SendAd(player, DiscordMessages[messageLast[player.userID]]);
                        messageLast[player.userID]++;
                    }
                }
            });

            timer.Every((float)AdInterval_Default, () =>
            {
                var list = BasePlayer.activePlayerList.Where(x => x != null &&
                !Groups.IsStaff(x) &&
                !Groups.IsVIP(x) &&
                !Groups.IsDiscordBooster(x) &&
                !Groups.IsVerified(x) &&
                !Groups.IsSteam(x) &&
                !Groups.IsDiscord(x)
                ).ToList();
                foreach (BasePlayer player in list)
                {
                    if (!messageLast.ContainsKey(player.userID)) { messageLast.Add(player.userID, 0); }
                    if (messageLast[player.userID] >= DefaultMessages.Length) { messageLast[player.userID] = 0; }
                    SendAd(player, DefaultMessages[messageLast[player.userID]]);
                    messageLast[player.userID]++;
                }
            });
        }

        #region Helpers
        private class Groups
        {
            public static bool IsStaff(BasePlayer player)
            {
                if (StaffID.Contains(player.userID) && Instance.permission.UserHasGroup(player.UserIDString, GroupList.Staff) && Instance.permission.UserHasPermission(player.UserIDString, Perm)) { return true; }
                return false;
            }
            public static bool IsVIP(BasePlayer player)
            {
                if (Instance.permission.UserHasGroup(player.UserIDString, GroupList.VIP3) ||
                    Instance.permission.UserHasGroup(player.UserIDString, GroupList.VIP2) ||
                    Instance.permission.UserHasGroup(player.UserIDString, GroupList.VIP1)) { return true; }
                return false;
            }
            public static bool IsDiscordBooster(BasePlayer player)
            {
                if (Instance.permission.UserHasGroup(player.UserIDString, GroupList.DiscordBooster)) { return true; }
                return false;
            }
            public static bool IsVerified(BasePlayer player)
            {
                if (Instance.permission.UserHasGroup(player.UserIDString, GroupList.Verified)) { return true; }
                return false;
            }
            public static bool IsSteam(BasePlayer player)
            {
                if (Instance.permission.UserHasGroup(player.UserIDString, GroupList.Steam)) { return true; }
                return false;
            }
            public static bool IsDiscord(BasePlayer player)
            {
                if (Instance.permission.UserHasGroup(player.UserIDString, GroupList.Discord)) { return true; }
                return false;
            }
        }

        #endregion Helpers

        private void SendAd(BasePlayer player, string message)
        {
            try { player.SendConsoleCommand("chat.add", 0, IconID, $"<size=16>[<color={TitleColor}>System</color>] {message}</size>"); }
            catch { Instance.SendReply(player, message, 2, IconID); }
        }
    }
}