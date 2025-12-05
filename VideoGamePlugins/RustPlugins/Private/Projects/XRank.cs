using Oxide.Core.Libraries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oxide.Plugins
{
    [Info("XRank", "Infinitynet.dk/Xray", "1.0.0")]
    [Description("Copyright Infinitynet.dk All Rights Reserved")]
    public class XRank : RustPlugin
    {
        #region Configuration
        public ulong[] StaffID = new ulong[] { 76561198066857380, 76561198023076226 };
        public const string DiscordWebHookRolesChanged = "https://discord.com/api/webhooks/977884480083750912/BdLZQ5sgSegSsxthqn4phVMkWpSY0Skivxw3RjBKmqfZA0UuE4u98-MU-MK3zn4KTmrr";
        public const ulong IconID = 76561198389709969;
        public const string Perm = "XRank.IsStaff";
        private const string VerifiedGroup = "verified";
        private const string VIPGroup1 = "vip1";
        private const string VIPGroup2 = "vip2";
        private const string VIPGroup3 = "vip3";
        #endregion Configuration

        void Loaded()
        {
            permission.RegisterPermission($"{Name}.{Perm}", this);

            timer.Every(60, () =>
            {
                foreach (BasePlayer player in BasePlayer.activePlayerList)
                {
                    CheckRank(player);
                }
            });
        }


        #region Helpers
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
        #endregion Helpers

        #region Broadcast notifications
        private void BroadcastMessage(string message) => Server.Broadcast($"<size=16>[<color=#4baffa>System</color>] {message}</size>", IconID);

        private void DiscordRankUpdate(ulong TargetID, string groupName, bool IsRemove)
        {
            IdNames.Clear();
            GetNameFromID(TargetID);
            GetPhotoFromID(TargetID);
            timer.Once(5f, () =>
            {

                if(!IsRemove)
                {
                    string body = "{\"content\":null,\"embeds\":[{\"description\":\"**RANK GRANTED**᲼" + groupName.ToUpper() + "\\n>>> **SERVER**᲼__" + ConVar.Server.hostname + "__\",\"color\":4960250,\"author\":{\"name\":\"" + IdNames[TargetID].Name + " - " + TargetID + "\",\"url\":\"https://steamcommunity.com/profiles/" + TargetID + "\",\"icon_url\":\"" + IdNames[TargetID].Photo + "\"}}],\"attachments\":[]}";
                    webrequest.Enqueue(DiscordWebHookRolesChanged, body, (code, response) => { }, this, RequestMethod.POST, new Dictionary<string, string> { { "Content-Type", "application/json" } });
                }
                else
                {
                    string body = "{\"content\":null,\"embeds\":[{\"description\":\"**RANK REVOKED**᲼" + groupName.ToUpper() + "\\n>>> **SERVER**᲼__" + ConVar.Server.hostname + "__\",\"color\":4960250,\"author\":{\"name\":\"" + IdNames[TargetID].Name + " - " + TargetID + "\",\"url\":\"https://steamcommunity.com/profiles/" + TargetID + "\",\"icon_url\":\"" + IdNames[TargetID].Photo + "\"}}],\"attachments\":[]}";
                    webrequest.Enqueue(DiscordWebHookRolesChanged, body, (code, response) => { }, this, RequestMethod.POST, new Dictionary<string, string> { { "Content-Type", "application/json" } });
                }
                IdNames.Clear();

            });
        }


        private void OnGroupChanged(string id, string groupName, bool IsRemove)
        {
            BasePlayer player;
            BasePlayer.TryFindByID(Convert.ToUInt64(id), out player);

            try
            {
                if (!IsRemove) { DiscordRankUpdate(player.userID, groupName, false); PrintWarning($"{player.displayName}({id}) >>> {groupName}"); }
                else if (IsRemove) { DiscordRankUpdate(player.userID, groupName, true); PrintWarning($"{player.displayName}({id}) <<< {groupName}"); }
            } catch { }

            try { if (IsStaff(player)) { return; } }
            catch { }
            switch (groupName)
            {
                case "verified":
                case "discordbooster":
                case "vip1":
                case "vip2":
                case "vip3":
                    if(player != null)
                    {
                        StringBuilder stringGroup = new StringBuilder(groupName);

                        stringGroup.Replace("verified", "Verificeret");
                        stringGroup.Replace("discordbooster", "Discord Booster");
                        stringGroup.Replace("vip1", "VIP");
                        stringGroup.Replace("vip2", "VIP+");
                        stringGroup.Replace("vip3", "VIP++");


                        if (!IsRemove) { BroadcastMessage($"Spilleren <color=#96c8fa>{player.displayName}</color> Har fået rollen <color=#96c8fa>{stringGroup}</color>"); }
                        else if (IsRemove) { BroadcastMessage($"Spilleren <color=#96c8fa>{player.displayName}</color> Er blevet frataget rollen <color=#96c8fa>{stringGroup}</color>"); }
                    }
                    break;
            }
        }

        void OnUserGroupAdded(string id, string groupName) => OnGroupChanged(id, groupName, false);
 
        void OnUserGroupRemoved(string id, string groupName) => OnGroupChanged(id, groupName, true);


        #endregion Broadcast notifications


        #region Helpers
        private bool IsVerified(BasePlayer player) => (permission.UserHasGroup(player.UserIDString, "discord") && permission.UserHasGroup(player.UserIDString, "steam"));
        private bool IsStaff(BasePlayer player) => StaffID.Contains(player.userID) && permission.UserHasPermission(player.UserIDString, $"{Name}.{Perm}");


        private void CheckRank(BasePlayer player)
        {
            if(IsStaff(player) && !permission.UserHasGroup(player.UserIDString, VIPGroup3)) { permission.AddUserGroup(player.UserIDString, VIPGroup3);}
   
            if(IsVerified(player) && !permission.UserHasGroup(player.UserIDString, VerifiedGroup)) { permission.AddUserGroup(player.UserIDString, VerifiedGroup);}
            else if (!IsVerified(player)) { permission.RemoveUserGroup(player.UserIDString, VerifiedGroup); }
        }
        #endregion Helpers



        void OnPlayerConnected(BasePlayer player) => CheckRank(player);
        void OnPlayerDisconnected(BasePlayer player, string reason) => CheckRank(player);
    }
}