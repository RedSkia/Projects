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
    [Info("XLogs", "Infinitynet.dk", "1.0.0")]
    [Description("Copyright Infinitynet.dk All Rights Reserved")]
    public class XLogs : RustPlugin
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

            public int ServerLogsUpdateFrequency { get; set; } = 5;
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


        #region Trigger Hooks
        object OnUserChat(IPlayer player, string message)
        {
            if (player.IsAdmin)
            {
                Log("Chat", $"AdminMessage|{player.Id}|{player.Name}|{message}");
            }
            else
            {
                Log("Chat", $"PlayerMessage|{player.Id}|{player.Name}|{message}");
            }
            return null;
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

        object OnPlayerViolation(BasePlayer player, AntiHackType type, float amount)
        {
            Log("Violations", $"Violation|{player.userID}|{player.displayName}|{type}|{amount}");
            return null;
        }

        //Give Item
        object OnServerMessage(string message, string name, string color, ulong id)
        {
            if (message.Contains("gave themselves"))
            {
                Log("ItemSpawned", $"ItemSpawned|{message}");
            }
            return null;
        }

        object OnServerCommand(ConsoleSystem.Arg arg)
        {
            string fullCommand = arg.cmd.FullName;

            if (!fullCommand.Contains("inventory.give"))
            {
                if (arg.Connection != null)
                {
                    Log("Commands", $"PlayerCommand|{arg.Player().userID}|{arg.Player().displayName}|{fullCommand}|{arg.FullString}");
                }
                else
                {
                    Log("Commands", $"ServerCommand|||{fullCommand}|{arg.FullString}");

                }
            }


            return null;
        }

        object OnPlayerDeath(BasePlayer player, HitInfo hitInfo)
        {
            try
            {
                BasePlayer killer = hitInfo.InitiatorPlayer;

                if (hitInfo.InitiatorPlayer != null)
                {
                    if (player == killer)
                    {
                        Log("Deaths", $"PlayerSuicide|{player.userID}|{player.displayName}");
                    }
                    else
                    {
                        Log("Deaths", $"PlayerKilled|{player.userID}|{player.displayName}|{hitInfo.InitiatorPlayer.userID}|{hitInfo.InitiatorPlayer.displayName}");
                    }
                }
            }
            catch
            {
                Log("Deaths", $"PlayerD&D|{player.userID}|{player.displayName}|{player.lastAttacker}|{player.lastAttacker.name}");
            }
            return null;
        }
        #endregion

        /*=====Functions=====*/

        //Log Server Stats
        void UpdateServerInfo()
        {
            try
            {
                int PlayersConnected = ServerInfo().Players;
                int TotalPlayerSlots = ServerInfo().MaxPlayers;
                int PlayersJoining = ServerInfo().Joining;
                int PlayersQueued = ServerInfo().Queued;
                int EntityCount = ServerInfo().EntityCount;
                float Fps = ServerInfo().Framerate;
                int SleeperCount = BasePlayer.sleepingPlayerList.Count;

                Dictionary<string, int> placeholders = new Dictionary<string, int>
                {
                    ["PlayersConnected"] = PlayersConnected,
                    ["TotalPlayerSlots"] = TotalPlayerSlots,
                    ["PlayersJoining"] = PlayersJoining,
                    ["PlayersQueued"] = PlayersQueued,
                    ["EntityCount"] = EntityCount,
                    ["Sleepers"] = SleeperCount,
                    ["Fps"] = System.Convert.ToInt32(Fps)
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