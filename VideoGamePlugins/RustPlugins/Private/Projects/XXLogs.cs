using Newtonsoft.Json;
using Oxide.Core;
using Oxide.Core.Libraries.Covalence;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using ConVar;
using Oxide.Core.Configuration;
using static ConVar.Admin;



namespace Oxide.Plugins
{
    [Info("XLogs", "Infinitynet.dk/Xray", "1.0.0")]
    [Description("Copyright Infinitynet.dk All Rights Reserved")]
    public class XXLogs : RustPlugin
    {
        #region Startup

        //Server Loaded
        void OnServerInitialized(bool initial)
        {
            #region Copyright Message
            if (initial) PrintWarning($"{Name} Created By Infinitynet.dk/Xray [Private Plugin]");
            #endregion
            UpdateServerInfo();
        }

        //Loaded LOOP
        void Loaded()
        {
            timer.Repeat(config.ServerLogsUpdateFrequency, 0, () => { UpdateServerInfo(); });
        }

        #endregion


        #region Configuration

        private Configuration config;

        //Default Config
        public class Configuration
        {
            [JsonProperty("ServerLogs Update Frequency (Seconds)")]
            public int ServerLogsUpdateFrequency { get; set; } = 60;


            [JsonProperty("Player Count Update Frequency (Seconds)")]
            public int PlayerCountUpdateFrequency { get; set; } = 3600;
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



        #region Player Lists

        void UpdateWipeList(ulong playerID)
        {
            List<ulong> iList = Interface.Oxide.DataFileSystem.ReadObject<List<ulong>>($"{Name}/WipePlayers");
            if (!iList.Contains(playerID)) { iList.Add(playerID); }
            Interface.Oxide.DataFileSystem.WriteObject($"{Name}/WipePlayers", iList);
        }

        void UpdateAllTimeList(ulong playerID)
        {
            List<ulong> iList = Interface.Oxide.DataFileSystem.ReadObject<List<ulong>>($"{Name}/AllTimePlayers");
            if (!iList.Contains(playerID)) { iList.Add(playerID); }
            Interface.Oxide.DataFileSystem.WriteObject($"{Name}/AllTimePlayers", iList);
        }

        DateTime TimeStamp_Wipe = DateTime.UtcNow;
        int OldWipeCount;
        public int CountWipePlayers()
        {
            var LastTimeStamp = (DateTime.UtcNow - TimeStamp_Wipe).TotalSeconds; //Time Limiter
            if (LastTimeStamp > config.PlayerCountUpdateFrequency)
            {
                TimeStamp_Wipe = DateTime.UtcNow;
                OldWipeCount = Interface.Oxide.DataFileSystem.ReadObject<List<ulong>>($"{Name}/WipePlayers").Count;
                return OldWipeCount;
            }
            return OldWipeCount;
        }

        DateTime TimeStamp_AllTime = DateTime.UtcNow;
        int OldAllTimeCount;
        public int CountAllTimePlayers()
        {
            var LastTimeStamp = (DateTime.UtcNow - TimeStamp_AllTime).TotalSeconds; //Time Limiter
            if (LastTimeStamp > config.PlayerCountUpdateFrequency)
            {
                TimeStamp_AllTime = DateTime.UtcNow;
                OldAllTimeCount = Interface.Oxide.DataFileSystem.ReadObject<List<ulong>>($"{Name}/AllTimePlayers").Count;
                return OldAllTimeCount;
            }
            return OldAllTimeCount;
        }

        #endregion Player Lists


        #region Trigger Hooks
        void OnUserChat(IPlayer player, string message)
        {
            if (player.IsAdmin)
            {
                Log("Chat", $"AdminMessage|{player.Id}|{player.Name}|{message}");
            }
            else
            {
                Log("Chat", $"PlayerMessage|{player.Id}|{player.Name}|{message}");
            }

        }

        void OnUserConnected(IPlayer player)
        {
            if (player.IsAdmin)
            {
                Log("Connections", $"AdminConnected|{player.Id}|{player.Name}|{player.Address}");
            }
            else
            {
                Log("Connections", $"PlayerConnected|{player.Id}|{player.Name}|{player.Address}");
            }

            UpdateWipeList(Convert.ToUInt64(player.Id)); //Add To List WIPE
            UpdateAllTimeList(Convert.ToUInt64(player.Id)); //Add To List ALLTIME
        }

        void OnUserDisconnected(IPlayer player)
        {
            if (player.IsAdmin)
            {
                Log("Disconnections", $"AdminDisconnected|{player.Id}|{player.Name}");
            }
            else
            {
                Log("Disconnections", $"PlayerDisconnected|{player.Id}|{player.Name}");
            }
        }

        void OnUserRespawned(IPlayer player)
        {
            if (player.IsAdmin)
            {
                Log("Respawns", $"AdminRespawned|{player.Id}|{player.Name}");
            }
            else
            {
                Log("Respawns", $"PlayerRespawned|{player.Id}|{player.Name}");
            }
        }

        void OnUserKicked(IPlayer player, string reason)
        {
            if (player.IsAdmin)
            {
                Log("Kicks", $"AdminKicked|{player.Id}|{player.Name}|{reason}");
            }
            else
            {
                Log("Kicks", $"PlayerKicked|{player.Id}|{player.Name}|{reason}");
            }
        }

        void OnUserBanned(string name, string id, string ipAddress, string reason)
        {
            Log("Bans", $"PlayerBanned|{id}|{name}|{ipAddress}|{reason}");
        }

        void OnPlayerReported(BasePlayer reporter, string targetName, string targetId, string subject, string message, string type)
        {
            if (reporter.IsAdmin)
            {
                Log("Reports", $"AdminReport|{reporter.userID}|{reporter.displayName}|{targetName}|{targetName}|{subject}|{message}");
            }
            else
            {
                Log("Reports", $"PlayerReport|{reporter.userID}|{reporter.displayName}|{targetName}|{targetName}|{subject}|{message}");
            }
        }

        void OnPlayerViolation(BasePlayer player, AntiHackType type, float amount)
        {
            Log("Violations", $"Violation|{player.userID}|{player.displayName}|{type}|{amount}");
        }

        //Give Item
        void OnServerMessage(string message, string name, string color, ulong id)
        {
            if (message.Contains("gave themselves"))
            {
                Log("ItemSpawned", $"ItemSpawned|{message}");
            }
        }

        void OnServerCommand(ConsoleSystem.Arg arg)
        {
            string fullCommand = arg.cmd.FullName;

            if (!fullCommand.Contains("inventory.give"))
            {
                if (arg.Connection != null)
                {
                    if (arg.Player().IsAdmin)
                    {
                        Log("Commands", $"AdminCommand|{arg.Player().userID}|{arg.Player().displayName}|{fullCommand}|{arg.FullString}");
                    }
                    else
                    {
                        Log("Commands", $"PlayerCommand|{arg.Player().userID}|{arg.Player().displayName}|{fullCommand}|{arg.FullString}");
                    }
                }
                else
                {
                    Log("Commands", $"ServerCommand|||{fullCommand}|{arg.FullString}");
                }
            }
        }

        void OnPlayerDeath(BasePlayer player, HitInfo hitInfo)
        {
            try
            {
                var victim = player != null && player is BasePlayer ? player : null;
                var killer = hitInfo.InitiatorPlayer != null && hitInfo.InitiatorPlayer is BasePlayer ? hitInfo.InitiatorPlayer : null;
                if ((victim == null || killer == null)) return;

                if (killer == victim) //Suicide
                {
                    if (victim.IsAdmin) { Log("Deaths", $"AdminSuicide|{victim.userID}|{victim.displayName}"); }
                    else { Log("Deaths", $"PlayerSuicide|{victim.userID}|{victim.displayName}"); }
                }

                //Regular Death
                if (victim.IsAdmin) { Log("Deaths", $"AdminKilled|{victim.userID}|{victim.displayName}|{killer.userID}|{killer.displayName}"); }
                else { Log("Deaths", $"PlayerKilled|{victim.userID}|{victim.displayName}|{killer.userID}|{killer.displayName}"); }
            }
            catch
            {

                //Downed & Dead
                var victim = player != null && player is BasePlayer ? player : null;
                var OldKiller = victim.lastAttacker as BasePlayer;
                if ((victim == null || OldKiller == null)) return;
                if (victim.IsAdmin) { Log("Deaths", $"AdminKilled|{victim.userID}|{victim.displayName}|{OldKiller.userID}|{OldKiller.displayName}"); }
                else { Log("Deaths", $"PlayerKilled|{victim.userID}|{victim.displayName}|{OldKiller.userID}|{OldKiller.displayName}"); }
            }

        }


        #region Raid Logs
        void OnExplosiveThrown(BasePlayer player, BaseEntity entity, ThrownWeapon item)
        {
            if (entity.OwnerID > 0 && entity.OwnerID != player.userID) { Log("Raid", $"Explosive|{player.userID}|{player.displayName}|{entity.OwnerID}|{MapPosition(player.transform.position)}"); }
        }

        void OnRocketLaunched(BasePlayer player, BaseEntity entity)
        {
            if (entity.OwnerID > 0 && entity.OwnerID != player.userID) { Log("Raid", $"Rocket|{player.userID}|{player.displayName}|{entity.OwnerID}|{MapPosition(player.transform.position)}"); }
        }

        void OnEntityDeath(BaseCombatEntity entity, HitInfo info)
        {
            try
            {
                if (entity is BuildingBlock && entity.OwnerID != info.InitiatorPlayer.userID)
                {
                    Log("Raid", $"Building|{info.InitiatorPlayer.userID}|{info.InitiatorPlayer.displayName}|{entity.OwnerID}|{MapPosition(info.InitiatorPlayer.transform.position)}|{entity.ShortPrefabName}");
                }

                if (entity is Door && entity.OwnerID != info.InitiatorPlayer.userID)
                {
                    Log("Raid", $"Deployable|{info.InitiatorPlayer.userID}|{info.InitiatorPlayer.displayName}|{entity.OwnerID}|{MapPosition(info.InitiatorPlayer.transform.position)}|{entity.ShortPrefabName}");
                }
            }
            catch { }

        }
        #endregion Raid Logs

        #endregion

        /*=====Functions=====*/

        string MapPosition(Vector3 position)
        {
            var chars = new string[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "AA", "AB", "AC", "AD", "AE", "AF", "AG", "AH", "AI", "AJ", "AK", "AL", "AM", "AN", "AO", "AP", "AQ", "AR", "AS", "AT", "AU", "AV", "AW", "AX", "AY", "AZ" };

            const float block = 146;

            float size = ConVar.Server.worldsize;
            float offset = size / 2;

            float xpos = position.x + offset;
            float zpos = position.z + offset;

            int maxgrid = (int)(size / block);

            float xcoord = Mathf.Clamp(xpos / block, 0, maxgrid - 1);
            float zcoord = Mathf.Clamp(maxgrid - (zpos / block), 0, maxgrid - 1);

            string pos = string.Concat(chars[(int)xcoord], (int)zcoord);

            return (pos);
        }


        //Log Server Stats
        void UpdateServerInfo()
        {
            try
            {

                int PlayersConnected = ServerInfo().Players;
                int TotalPlayerSlots = ServerInfo().MaxPlayers;
                int PlayersJoining = ServerInfo().Joining;
                int PlayersQueued = ServerInfo().Queued;
                int PlayersWipe = CountWipePlayers();
                int PlayersAllTime = CountAllTimePlayers();
                int EntityCount = ServerInfo().EntityCount;
                int SleeperCount = BasePlayer.sleepingPlayerList.Count;
                float Fps = ServerInfo().Framerate;
                string Map = ServerInfo().Map;
                int MapSeed = ConVar.Server.seed;
                int MapSize = ConVar.Server.worldsize;
                int ServerUptime = ServerInfo().Uptime;




                Dictionary<string, object> placeholders = new Dictionary<string, object>
                {
                    ["PlayersConnected"] = PlayersConnected,
                    ["TotalPlayerSlots"] = TotalPlayerSlots,
                    ["PlayersJoining"] = PlayersJoining,
                    ["PlayersQueued"] = PlayersQueued,
                    ["PlayersWipe"] = PlayersWipe,
                    ["PlayersAllTime"] = PlayersAllTime,
                    ["EntityCount"] = EntityCount,
                    ["Sleepers"] = SleeperCount,
                    ["Fps"] = Convert.ToInt32(Fps),
                    ["Map"] = Map,
                    ["MapSeed"] = MapSeed,
                    ["MapSize"] = MapSize,
                    ["Uptime"] = ServerUptime
                };

                //Saves Data
                Interface.Oxide.DataFileSystem.WriteObject($"{Name}/ServerInfo", placeholders);

            }
            catch { }
        }


        //Logs to File
        public void Log(string filename, object args)
        {
            LogToFile(filename, $"{DateTime.Now}|" + args, this, false);
        }
    }
}