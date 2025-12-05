using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Oxide.Core;
using Oxide.Core.Libraries;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oxide.Plugins
{

    [Info("XSync", "InfinityNet/Xray", "1.0.0")]
    [Description("Copyright Infinitynet.dk All Rights Reserved")]
    public class XSync : RustPlugin
    {
        private XSync Instance { get; set; }

        const string requestPath = "http://localhost/websitev2/api/StaffData.json";

        #region Init
        private void Loaded()
        {
            Instance = this;
            RequestData();
        }
        #endregion Init

        void OnPlayerConnected(BasePlayer player) => CheckRank(player);
        void OnUserGroupAdded(string id, string groupName) => PrintWarning($"{BasePlayer.FindByID(Convert.ToUInt64(id)).displayName} ({id}) Has been added to {groupName}");
        void OnUserGroupRemoved(string id, string groupName) => PrintWarning($"{BasePlayer.FindByID(Convert.ToUInt64(id)).displayName} ({id}) Has been removed from {groupName}");

        #region Helpers
        private void CheckRank(BasePlayer player)
        {
            if (StaffList.ContainsKey(player.userID))
            {
                permission.AddUserGroup(player.UserIDString, StaffList[player.userID].rank.ToString());
                permission.AddUserGroup(player.UserIDString, StaffList[player.userID].group.ToString());
            }
            else
            {
                foreach (var rank in (StaffMember.Rank[])Enum.GetValues(typeof(StaffMember.Rank)))
                {
                    permission.RemoveUserGroup(player.UserIDString, rank.ToString());
                }

                foreach (var group in (StaffMember.Group[])Enum.GetValues(typeof(StaffMember.Group)))
                {
                    permission.RemoveUserGroup(player.UserIDString, group.ToString());
                }
            }
        }
        private void RequestData()
        {
            Instance.webrequest.Enqueue(requestPath, null, (code, response) =>
            {
                if (code != 200 || response == null) { Instance.PrintWarning($"WEB | MISSING | {requestPath.ToLower().Replace("http://", "").Replace("https://", "")}"); return; }
                Instance.Puts($"WEB | LOADED | {requestPath.ToLower().Replace("http://", "").Replace("https://", "")}");

                JObject json = JObject.Parse(response);

                foreach (var team in JsonConvert.DeserializeObject<Dictionary<string, object>>(json["StaffList"].ToString()))
                {
                    foreach (var person in JsonConvert.DeserializeObject<Dictionary<string, object>>(json["StaffList"][team.Key].ToString()))
                    {
                        var Rank = (string)json["StaffList"][team.Key][person.Key]["rank"];
                        var Key = (ulong)json["StaffList"][team.Key][person.Key]["steamid"];
                        if (!StaffList.ContainsKey(Key))
                        {
                            StaffMember.Group group = new StaffMember.Group();
                            switch (team.Key)
                            {
                                case "EXECUTIVE":
                                    group = StaffMember.Group.EXECUTIVE;
                                    break;
                                case "INTERNAL":
                                    group = StaffMember.Group.INTERNAL;
                                    break;
                                case "EXTERNAL":
                                    group = StaffMember.Group.EXTERNAL;
                                    break;
                            }

                            StaffMember.Rank rank = new StaffMember.Rank();
                            switch (Rank)
                            {
                                case "CEO":
                                    rank = StaffMember.Rank.CEO;
                                    break;
                                case "Executive":
                                    rank = StaffMember.Rank.Executive;
                                    break;


                                case "Developer":
                                    rank = StaffMember.Rank.Developer;
                                    break;
                                case "Designer":
                                    rank = StaffMember.Rank.Designer;
                                    break;
                                case "Support":
                                    rank = StaffMember.Rank.Support;
                                    break;


                                case "HeadAdministrator":
                                    rank = StaffMember.Rank.HeadAdministrator;
                                    break;
                                case "Administrator":
                                    rank = StaffMember.Rank.Administrator;
                                    break;
                                case "SeniorModerator":
                                    rank = StaffMember.Rank.SeniorModerator;
                                    break;
                                case "Moderator":
                                    rank = StaffMember.Rank.Moderator;
                                    break;
                            }

                            StaffList.Add(Key, new StaffMember
                            {
                                rank = rank,
                                group = group
                            });
                        }
                    }
                }
            }, Instance, RequestMethod.GET, null, 1f);
        }
        #endregion Helpers

        #region Classes
        private static Dictionary<ulong, StaffMember> StaffList = new Dictionary<ulong, StaffMember>();
        private class StaffMember
        {
            public Rank rank { get; set; }
            public enum Rank
            {
                CEO,
                Executive,
                Developer,
                Designer,
                Support,
                HeadAdministrator,
                Administrator,
                SeniorModerator,
                Moderator,
            }
            public Group group { get; set; }
            public enum Group
            {
                EXECUTIVE,
                INTERNAL,
                EXTERNAL,
            }
        }
        #endregion Classes
    }
}