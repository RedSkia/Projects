using ConVar;
using Newtonsoft.Json;
using Oxide.Core;
using Oxide.Core.Libraries.Covalence;
using Oxide.Game.Rust.Cui;
using Rust;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;
using static ConVar.Admin;

namespace Oxide.Plugins
{
    [Info("XStats", "Infinitynet.dk/Xray", "1.0.0")]
    [Description("Copyright Infinitynet.dk All Rights Reserved")]
    public class XStats : RustPlugin
    {
        #region Configuration

        private Configuration config;


        //Default Config
        public class Configuration
        {

            [JsonProperty("Administrator Oxide Group")]
            public string Administrator_Group { get; set; } = "";

            [JsonProperty("Moderator Oxide Group")]
            public string Moderator_Group { get; set; } = "";




            [JsonProperty("Player Location Update Frequency (seconds)")]
            public int PlayerLocationUpdateFrequency { get; set; } = 5;

            [JsonProperty("Player Command Cooldown (seconds)")]
            public int PlayerCommandCooldown { get; set; } = 60;
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
            PrintWarning("[ERROR] Invalid Or Empty Configuration Overwriting");
            config = new Configuration();
        }

        protected override void SaveConfig() => Config.WriteObject(config);

        #endregion


        private const string WipePermission = "xstats.wipe";
        private const string ViewPermission = "xstats.view";
        private const bool Debugging = false;


        #region OnStartup

        private void Init()
        {
            StartTimer_PlayerLocationUpdateFrequency();
            permission.RegisterPermission(WipePermission, this);
            permission.RegisterPermission(ViewPermission, this);
        }


        void OnServerInitialized(bool initial)
        {
            foreach (BasePlayer player in BasePlayer.activePlayerList) PlayerStatistics.LoadStatistics(player.userID);

            #region Copyright Message
            if (initial) PrintWarning($"{Name} Created By Infinitynet.dk/Xray [Private Plugin]");
            #endregion
        }

        #endregion


        #region TOP Statistics Declaration

        public class TopPlayerStatistics
        {
            public static int MostKills = 0; public static string MostKills_playerName = "";
            public static int MostHeadshots = 0; public static string MostHeadshots_playerName = "";
            public static int LongestKill = 0; public static string LongestKill_playerName = "";
            public static float HighestKDR = 0; public static string HighestKDR_playerName = "";
            public static double MostTimePlayed = 0; public static string MostTimePlayed_playerName = "";
            public static int MostDistanceTraveled = 0; public static string MostDistanceTraveled_playerName = "";
            public static int MostBradleyTakedowns = 0; public static string MostBradleyTakedowns_playerName = "";
            public static int MostAnimalKills = 0; public static string MostAnimalKills_playerName = "";
            public static int MostLootStolen = 0; public static string MostLootStolen_playerName = "";


            internal static void ResetTopStats()
            {
                MostKills = 0; MostKills_playerName = "";
                MostHeadshots = 0; MostHeadshots_playerName = "";
                LongestKill = 0; LongestKill_playerName = "";
                HighestKDR = 0; HighestKDR_playerName = "";
                MostTimePlayed = 0; MostTimePlayed_playerName = "";
                MostDistanceTraveled = 0; MostDistanceTraveled_playerName = "";
                MostBradleyTakedowns = 0; MostBradleyTakedowns_playerName = "";
                MostAnimalKills = 0; MostAnimalKills_playerName = "";
                MostLootStolen = 0; MostLootStolen_playerName = "";
            }
        }

        #endregion


        #region Statistics Declaration
        static Dictionary<ulong, PlayerStatistics> cachedPlayerStatistics = new Dictionary<ulong, PlayerStatistics>();

        private class PlayerStatistics
        {
            public string PlayerName = "";
            #region Variables
            //PVP Stats
            public int LongestKill = 0;
            public int LongestHeadshotKill = 0;
            public int Kills = 0;
            public int Deaths = 0;
            public int ShortsFired = 0;
            public int ShortsHit = 0;
            public int HeadShotsHit = 0;
            public float KDR => Deaths == 0 ? Kills : (float)Math.Round(((float)Kills) / Deaths, 2);
            public float hitAccuracy => ShortsFired == 0 ? ShortsHit : (float)Math.Round(((float)ShortsHit) / ShortsFired, 2);
            public float headHitAccuracy => ShortsFired == 0 ? HeadShotsHit : (float)Math.Round(((float)HeadShotsHit) / ShortsFired, 2);
            public int RocketsFired = 0;
            public int C4used = 0;



            //PVE Stats
            public int ItemsCrafted = 0;
            public int ItemsResearch = 0;
            public int ItemsRecycled = 0;
            public int PlayersRevive = 0;
            public int Respawns = 0;
            public int ChatMessages = 0;
            public int VendingMachineTrades = 0;
            public int NpcKills = 0;
            public int BradleyKills = 0;
            public int AnimalKills = 0;



            //Gather Stats
            public int LootItemsStolen = 0;
            public int WoodGathered = 0;
            public int StoneGathered = 0;
            public int MetalOreGathered = 0;
            public int SulfurOreGathered = 0;
            public int HQMOreGathered = 0;
            public int AirdropsLooted = 0;



            //Building Stats
            public int FoundationRemoved = 0;
            public int FoundationBuild = 0;
            public int FoundationUpgrade = 0;



            //Travel Stats
            public int TotalDistanceTraveled => DistanceFlying + DistanceDriving + DistanceSailing + DistanceWalking;
            public int DistanceFlying = 0;
            public int DistanceDriving = 0;
            public int DistanceSailing = 0;
            public int DistanceWalking = 0;


            //Playtime Stats
            public double TotalPlaytimeHours = 0;

            public DateTime PlayerConnectedAt = DateTime.Now;
            public double PlaytimeTodayHours = 0;
            public double PlaytimeMonthHours = 0;

            public int CurrentDayIndex = 0;
            public int CurrentMonthIndex = 0;
            #endregion



            internal static void LoadStatistics(ulong id)
            {

                if (cachedPlayerStatistics.ContainsKey(id)) return;

                PlayerStatistics data = Interface.Oxide.DataFileSystem.ReadObject<PlayerStatistics>($"XStats/{id}");

                if (data == null) data = new PlayerStatistics();

                cachedPlayerStatistics.Add(id, data);
            }

            internal void SavePlayerStats(ulong id) => Interface.Oxide.DataFileSystem.WriteObject(($"XStats/{id}"), this, true);


            internal static void ResetPlayerStats(ulong id)
            {
                PlayerStatistics data = Interface.Oxide.DataFileSystem.ReadObject<PlayerStatistics>($"XStats/{id}");

                if (data == null) return;

                data = new PlayerStatistics();


                cachedPlayerStatistics = new Dictionary<ulong, PlayerStatistics>();

                data.SavePlayerStats(id);




            }


        }
        #endregion


        #region Custom Functions


        private void SendPlayerMessage(BasePlayer player, string message) => SendReply(player, message);
        string TextEncodeing(double TextSize, string HexColor, string PlainText) => $"<size={TextSize}><color=#{HexColor}>{PlainText}</color></size>";



        string GetIDfromName(string playerName)
        {
            foreach (BasePlayer player in BasePlayer.activePlayerList)
            {
                if (player.name == playerName)
                {
                    return player.userID.ToString();
                }
            }
            return null;
        }

        bool NameLastCharContains_S(string name) => (name.Last() != 's' || name.Last() != 'S');



        private void SaveSpecficPlyerData(ulong ID)
        {
            foreach (var player in cachedPlayerStatistics) player.Value.SavePlayerStats(ID);
        }
        private void SaveAllPlyerData()
        {
            PrintWarning("Saveing Data");
            foreach (var player in cachedPlayerStatistics) player.Value.SavePlayerStats(player.Key);
        }


        bool UserInGroup(BasePlayer player, string groupName) => permission.UserHasGroup(player.userID.ToString(), groupName);
        bool UserHasPermission(BasePlayer player, string permissionName) => permission.UserHasPermission(player.userID.ToString(), permissionName);


        bool IsRealAdmin(BasePlayer player, string group) => player.IsAdmin && UserInGroup(player, group);
        bool IsRealAdmin_Permission(BasePlayer player, string group, string permission) => player.IsAdmin && UserInGroup(player, group) && UserHasPermission(player, $"{permission}");

        bool IsRealMod(BasePlayer player, string group) => UserInGroup(player, group);
        bool IsRealMod_Permission(BasePlayer player, string group, string permission) => UserInGroup(player, group) && UserHasPermission(player, $"{permission}");



        #endregion




        #region Travel Declaration
        static Dictionary<ulong, LocationInfo> PlayerLocationInfo = new Dictionary<ulong, LocationInfo>();

        private class LocationInfo
        {
            public int OldPosX = 0;
            public int OldPosZ = 0;

            public int TotalMeters = 0;
        }
        #endregion

        #region Travel Timer Loop
        Timer Timer_PlayerLocationUpdateFrequency;
        bool Timer_PlayerLocationUpdateFrequency_Running;
        private void StartTimer_PlayerLocationUpdateFrequency()
        {
            if (!Timer_PlayerLocationUpdateFrequency_Running)
            {
                Timer_PlayerLocationUpdateFrequency = timer.Every(config.PlayerLocationUpdateFrequency, () =>
                {
                    Timer_PlayerLocationUpdateFrequency_Running = true;
                    foreach (BasePlayer player1 in BasePlayer.activePlayerList) UpdatePlayerDistanceTraveled(player1);

                });
            }
        }
        private void StopTimer_PlayerLocationUpdateFrequency()
        {
            Timer_PlayerLocationUpdateFrequency_Running = false;
            Timer_PlayerLocationUpdateFrequency.Destroy();
        }

        #endregion

        #region Travel Main Function

        private void UpdatePlayerDistanceTraveled(BasePlayer player)
        {

            PlayerStatistics.LoadStatistics(player.userID);

            //Core Distance Code
            if (PlayerLocationInfo.ContainsKey(player.userID))
            {
                //Reset Value ! IMPORTANT !
                PlayerLocationInfo[player.userID].TotalMeters = 0;

                if (((int)player.transform.position.x) != PlayerLocationInfo[player.userID].OldPosX || ((int)player.transform.position.z) != PlayerLocationInfo[player.userID].OldPosZ)
                {
                    var resultX = Math.Abs(PlayerLocationInfo[player.userID].OldPosX - player.transform.position.x);
                    var resultZ = Math.Abs(PlayerLocationInfo[player.userID].OldPosZ - player.transform.position.z);


                    PlayerLocationInfo[player.userID].OldPosX = ((int)player.transform.position.x);
                    PlayerLocationInfo[player.userID].OldPosZ = ((int)player.transform.position.z);


                    if (resultX > 1 && resultZ > 1)
                    {
                        var i = ((int)resultX) + ((int)resultZ);
                        PlayerLocationInfo[player.userID].TotalMeters = i / 2;
                    }
                    else if (resultX > 1 && resultZ < 1 || resultZ > 1 && resultX < 1)
                    {
                        var i = ((int)resultX) + ((int)resultZ);
                        PlayerLocationInfo[player.userID].TotalMeters = i;
                    }

                    if (Debugging) SendPlayerMessage(player, $"X = {((int)resultX)}\nY = {((int)resultZ)}\n\n{PlayerLocationInfo[player.userID].TotalMeters}");
                }
            }
            else
            {
                if (PlayerLocationInfo.ContainsKey(player.userID)) return;

                PlayerLocationInfo.Add(player.userID, new LocationInfo());
            }





            if (TravelMethod.ContainsKey(player.userID))
            {
                if (!TravelMethod[player.userID].Walking && !TravelMethod[player.userID].Driveing && !TravelMethod[player.userID].Sailing && !TravelMethod[player.userID].Flying)
                {
                    TravelMethod[player.userID].Walking = true;
                }

                //Flying
                if (TravelMethod[player.userID].Flying == true)
                {
                    if (cachedPlayerStatistics.ContainsKey(player.userID)) cachedPlayerStatistics[player.userID].DistanceFlying += PlayerLocationInfo[player.userID].TotalMeters;

                }

                //Driveing
                if (TravelMethod[player.userID].Driveing == true)
                {
                    if (cachedPlayerStatistics.ContainsKey(player.userID)) cachedPlayerStatistics[player.userID].DistanceDriving += PlayerLocationInfo[player.userID].TotalMeters;
                }

                //Sailing
                if (TravelMethod[player.userID].Sailing == true)
                {
                    if (cachedPlayerStatistics.ContainsKey(player.userID)) cachedPlayerStatistics[player.userID].DistanceSailing += PlayerLocationInfo[player.userID].TotalMeters;
                }

                //Walking
                if (TravelMethod[player.userID].Walking == true)
                {
                    if (cachedPlayerStatistics.ContainsKey(player.userID)) cachedPlayerStatistics[player.userID].DistanceWalking += PlayerLocationInfo[player.userID].TotalMeters;
                }

            }
            else
            {
                if (TravelMethod.ContainsKey(player.userID)) return;

                TravelMethod.Add(player.userID, new EntityMounted());
            }

        }





        static Dictionary<ulong, EntityMounted> TravelMethod = new Dictionary<ulong, EntityMounted>();
        private class EntityMounted
        {
            public bool Flying;
            public bool Driveing;
            public bool Sailing;
            public bool Walking;
        }
        void OnEntityDismounted(BaseMountable entity, BasePlayer player)
        {
            if (TravelMethod.ContainsKey(player.userID))
            {
                TravelMethod[player.userID].Flying = false;
                TravelMethod[player.userID].Driveing = false;
                TravelMethod[player.userID].Sailing = false;

                TravelMethod[player.userID].Walking = true;
                if (Debugging) SendPlayerMessage(player, $"Walking ? {TravelMethod[player.userID].Walking}");

            }
        }

        void OnEntityMounted(BaseMountable entity, BasePlayer player)
        {

            if (TravelMethod.ContainsKey(player.userID))
            {
                TravelMethod[player.userID].Flying = false;
                TravelMethod[player.userID].Driveing = false;
                TravelMethod[player.userID].Sailing = false;
                TravelMethod[player.userID].Walking = false;



                //Flying
                if (entity.ShortPrefabName == "miniheliseat" || entity.ShortPrefabName == "minihelipassenger" || entity.ShortPrefabName == "transporthelipilot" || entity.ShortPrefabName == "transporthelicopilot")
                {
                    TravelMethod[player.userID].Flying = true;
                    if (Debugging) SendPlayerMessage(player, $"Flying ? {TravelMethod[player.userID].Flying}");
                }
                //Driveing
                if (entity.ShortPrefabName == "modularcardriverseat" || entity.ShortPrefabName == "modularcarpassengerseatleft" || entity.ShortPrefabName == "modularcarpassengerseatright" || entity.ShortPrefabName == "modularcarpassengerseatlesslegroomleft" || entity.ShortPrefabName == "modularcarpassengerseatlesslegroomright")
                {
                    TravelMethod[player.userID].Driveing = true;
                    if (Debugging) SendPlayerMessage(player, $"Driveing ? {TravelMethod[player.userID].Driveing}");
                }
                //Sailing
                if (entity.ShortPrefabName == "smallboatdriver" || entity.ShortPrefabName == "smallboatpassenger" || entity.ShortPrefabName == "standingdriver")
                {
                    TravelMethod[player.userID].Sailing = true;
                    if (Debugging) SendPlayerMessage(player, $"Sailing ? {TravelMethod[player.userID].Sailing}");
                }

            }
            else
            {

                if (TravelMethod.ContainsKey(player.userID)) return;

                TravelMethod.Add(player.userID, new EntityMounted());
            }


        }

        #endregion



        #region Wipe Player Stats

        bool ResetStatsCommandConfirm = false;
        string[] ConfirmArgs;

        [ChatCommand("stats.confirm"), Permission(WipePermission)]
        private void ConfirmResetStatsCommand(BasePlayer player, string command, string[] args)
        {
            if (ResetStatsCommandConfirm && IsRealAdmin_Permission(player, config.Administrator_Group, WipePermission))
            {

                try
                {

                    if (cachedPlayerStatistics.ContainsKey(Convert.ToUInt64(ConfirmArgs[0])))
                    {
                        PlayerStatistics.ResetPlayerStats(Convert.ToUInt64(ConfirmArgs[0]));


                        SendPlayerMessage(player, $"Player Statistics Wiped For {ConfirmArgs[0]}");

                        PrintWarning($"Player Statistics Wiped For {ConfirmArgs[0]}");
                    }
                    else
                    {
                        SendPlayerMessage(player, $"SteamID {ConfirmArgs[0]} Not Found");
                    }

                }
                catch
                {
                    SendPlayerMessage(player, $"SteamID {ConfirmArgs[0]} Not Found");
                }


                ResetStatsCommandConfirm = false;
                ConfirmArgs = null;
            }
            else if (IsRealAdmin_Permission(player, config.Administrator_Group, WipePermission))
            {
                SendPlayerMessage(player, "You were too slow to do /stats.confirm");
            }

        }


        [ChatCommand("stats.wipe"), Permission(WipePermission)]
        private void ResetStatsCommand(BasePlayer player, string command, string[] args)
        {


            ResetStatsCommandConfirm = false;
            ConfirmArgs = null;
            if (args.Count() != 1 || args == null)
            {
                SendPlayerMessage(player, $"Usage : stats.wipe <SteamID64>");

            }
            else if (!ResetStatsCommandConfirm && IsRealAdmin_Permission(player, config.Administrator_Group, WipePermission))
            {
                SendPlayerMessage(player, $"Confirm To Wipe Statistics For {args} ?\nDo /stats.confirm");
                ResetStatsCommandConfirm = true;
                ConfirmArgs = args;

                //reset Timer
                timer.Once(10f, () =>
                {
                    if (ResetStatsCommandConfirm == true || ConfirmArgs != null)
                    {
                        ResetStatsCommandConfirm = false;
                        ConfirmArgs = null;
                        SendPlayerMessage(player, $"Too Slow");
                    }
                });
            }

        }
        #endregion



        #region Command Cooldown
        static Dictionary<ulong, PlayerCooldowns> CommandCooldown = new Dictionary<ulong, PlayerCooldowns>();

        private class PlayerCooldowns
        {
            public DateTime CommandCooldownExpire;

            public bool ExpireTimeSet = false;

        }


        bool CommandAvailable(BasePlayer player)
        {

            var currentTime = Convert.ToDateTime(DateTime.Now.ToString("HH:mm:ss"));
            bool ReturnValue = false;
            if (CommandCooldown.ContainsKey(player.userID))
            {

                if (!CommandCooldown[player.userID].ExpireTimeSet)
                {
                    CommandCooldown[player.userID].CommandCooldownExpire = currentTime.AddSeconds(config.PlayerCommandCooldown);
                    CommandCooldown[player.userID].ExpireTimeSet = true;
                }


                if (currentTime >= CommandCooldown[player.userID].CommandCooldownExpire)
                {

                    CommandCooldown[player.userID].ExpireTimeSet = false;

                    ReturnValue = true;
                }
                else
                {


                    //Cooldown Message
                    ReturnValue = false;
                    var TimeLeft = CommandCooldown[player.userID].CommandCooldownExpire - currentTime;

                    const string EnvironmentSpace = " ";
                    if (TimeLeft.Minutes < 1) SendPlayerMessage(player, $"{TextEncodeing(14, "3296fa", "Statistics")}{TextEncodeing(14, "ffffff", ":")}{EnvironmentSpace}{TextEncodeing(14, "ffffff", "Wait")}{EnvironmentSpace}{TextEncodeing(14, "3296fa", TimeLeft.Seconds.ToString())}{EnvironmentSpace}{TextEncodeing(14, "ffffff", "sec to use /stats commands again")}");
                    if (TimeLeft.Minutes >= 1) SendPlayerMessage(player, $"{TextEncodeing(14, "3296fa", "Statistics")}{TextEncodeing(14, "ffffff", ":")}{EnvironmentSpace}{TextEncodeing(14, "ffffff", "Wait")}{EnvironmentSpace}{TextEncodeing(14, "3296fa", config.PlayerCommandCooldown.ToString())}{EnvironmentSpace}{TextEncodeing(14, "ffffff", "sec to use /stats commands again")}");

                }

            }
            else
            {
                if (CommandCooldown.ContainsKey(player.userID)) return false;

                CommandCooldown.Add(player.userID, new PlayerCooldowns());

            }

            return ReturnValue;
        }

        #endregion




        //START OF COMMANDS

        [ChatCommand("stats")]
        private void ChatCommandHelp(BasePlayer player, string command, string[] args)
        {
            const string largeLine = "――――――――――――――――――――――――――――――――――――――――――――――――――――";




            if (IsRealAdmin_Permission(player, config.Administrator_Group, ViewPermission) || IsRealMod_Permission(player, config.Moderator_Group, ViewPermission)) // ADMIN OR MOD
            {
                SendPlayerMessage(player,
                $"{TextEncodeing(18, "ffffff", "Admin Statistics Commands")}\n" +

                $"{TextEncodeing(10, "ffffff", largeLine)}\n" +


                $"{TextEncodeing(14, "3296fa", "stats.pvp")} {TextEncodeing(14, "ffffff", "<SteamID>  Shows PVP Statistics")}" +
                $"\n{TextEncodeing(14, "3296fa", "stats.pve")} {TextEncodeing(14, "ffffff", "<SteamID>  Shows PVE Statistics")}" +
                $"\n{TextEncodeing(14, "3296fa", "stats.travel")} {TextEncodeing(14, "ffffff", "<SteamID>  Shows PVE Statistics")}" +
                $"\n{TextEncodeing(14, "3296fa", "stats.loot")} {TextEncodeing(14, "ffffff", "<SteamID>  Shows Looting Statistics")}" +
                $"\n{TextEncodeing(14, "3296fa", "stats.playtime")} {TextEncodeing(14, "ffffff", "<SteamID>  Shows Playtime Statisticss")}" +
                $"\n{TextEncodeing(14, "3296fa", "stats.top")} {TextEncodeing(14, "ffffff", "Shows Leaderboard Statistics")}" +


                $"\n{TextEncodeing(10, "ffffff", largeLine)}");
            }
            else // NOT ADMIN OR MOD
            {
                SendPlayerMessage(player,
                $"{TextEncodeing(18, "ffffff", "Statistics Commands")}\n" +

                $"{TextEncodeing(10, "ffffff", largeLine)}\n" +

                $"{TextEncodeing(14, "3296fa", "stats.pvp")} {TextEncodeing(14, "ffffff", "Shows Your PVP Statistics")}" +
                $"\n{TextEncodeing(14, "3296fa", "stats.pve")} {TextEncodeing(14, "ffffff", "Shows Your PVE Statistics")}" +
                $"\n{TextEncodeing(14, "3296fa", "stats.travel")} {TextEncodeing(14, "ffffff", "Shows Your Traveling Statistics")}" +
                $"\n{TextEncodeing(14, "3296fa", "stats.loot")} {TextEncodeing(14, "ffffff", "Shows Your Looting Statistics")}" +
                $"\n{TextEncodeing(14, "3296fa", "stats.playtime")} {TextEncodeing(14, "ffffff", "Shows Your Playtime Statistics")}" +
                $"\n{TextEncodeing(14, "3296fa", "stats.top")} {TextEncodeing(14, "ffffff", "Shows The Leaderboard Statistics")}" +



                $"\n{TextEncodeing(10, "ffffff", largeLine)}");
            }


        }



        #region PVP Command
        [ChatCommand("stats.pvp")]
        private void ChatCommandStatisticsPVP(BasePlayer player, string command, string[] args)
        {
            if (IsRealAdmin_Permission(player, config.Administrator_Group, ViewPermission) || IsRealMod_Permission(player, config.Moderator_Group, ViewPermission)) // ADMIN OR MOD
            {
                COMMAND_ChatCommandStatisticsPVP(player, command, args);
            }
            else // NOT ADMIN OR MOD DO Command Cooldown
            {
                if (!CommandCooldown.ContainsKey(player.userID))
                {
                    COMMAND_ChatCommandStatisticsPVP(player, command, args);

                    if (!CommandCooldown.ContainsKey(player.userID)) CommandCooldown.Add(player.userID, new PlayerCooldowns());


                    //Set Cooldown Manually
                    if (!CommandCooldown[player.userID].ExpireTimeSet)
                    {
                        var currentTime = Convert.ToDateTime(DateTime.Now.ToString("HH:mm:ss"));
                        CommandCooldown[player.userID].CommandCooldownExpire = currentTime.AddSeconds(config.PlayerCommandCooldown);
                        CommandCooldown[player.userID].ExpireTimeSet = true;
                    }
                }
                else if (CommandAvailable(player))
                {
                    COMMAND_ChatCommandStatisticsPVP(player, command, args);
                }
            }
        }

        private void COMMAND_ChatCommandStatisticsPVP(BasePlayer player, string command, string[] args)
        {
            ulong playerID = 0;
            string playerName = null;
            if (args != null && args.Length > 0)
            {
                if (IsRealAdmin_Permission(player, config.Administrator_Group, ViewPermission) || IsRealMod_Permission(player, config.Moderator_Group, ViewPermission)) // ADMIN OR MOD
                {
                    if (args[0].All(char.IsDigit))
                    {
                        ulong PlayeridArgs = Convert.ToUInt64(args[0]);

                        if (cachedPlayerStatistics.ContainsKey(PlayeridArgs)) // Cached Player Stats
                        {
                            playerID = PlayeridArgs;
                            playerName = PlayeridArgs.ToString();
                        }
                        else if (Interface.Oxide.DataFileSystem.ExistsDatafile($"{Name}/{PlayeridArgs}")) // Load From File
                        {
                            PlayerStatistics PlayerFileData = Interface.Oxide.DataFileSystem.ReadObject<PlayerStatistics>($"{Name}/{PlayeridArgs}");

                            playerID = PlayeridArgs;
                            playerName = PlayeridArgs.ToString();
                            PlayerStatistics.LoadStatistics(playerID);
                        }
                        else
                        {
                            SendPlayerMessage(player, "No File Data\nNo Cached Data");
                        }
                    }
                    else
                    {
                        SendPlayerMessage(player, "target steamID invalid\nUsage: /stats.pvp <SteamID>");
                    }
                }

            }
            else // Your Stats
            {
                playerID = player.userID;
                playerName = player.displayName;
            }

            //Save PlayerName In Data
            cachedPlayerStatistics[player.userID].PlayerName = player.displayName;

            if (playerID >= 1)
            {

                #region Stats Variables

                //PVP Stats
                string longestKill = cachedPlayerStatistics[playerID].LongestKill.ToString() + "m";
                string longestHeadshotKill = cachedPlayerStatistics[playerID].LongestHeadshotKill.ToString() + "m";
                string Kills = cachedPlayerStatistics[playerID].Kills.ToString();
                string Deaths = cachedPlayerStatistics[playerID].Deaths.ToString();
                string shortsFired = cachedPlayerStatistics[playerID].ShortsFired.ToString();
                string shortsHit = cachedPlayerStatistics[playerID].ShortsHit.ToString();
                string headShotsHit = cachedPlayerStatistics[playerID].HeadShotsHit.ToString();
                string KDR = cachedPlayerStatistics[playerID].KDR.ToString();
                string hitAccuracy = Math.Round(cachedPlayerStatistics[playerID].hitAccuracy * 100, 2) + "%";
                string headHitAccuracy = Math.Round(cachedPlayerStatistics[playerID].headHitAccuracy * 100, 2) + "%";
                string rocketsFired = cachedPlayerStatistics[playerID].RocketsFired.ToString();
                string C4used = cachedPlayerStatistics[playerID].C4used.ToString();

                #endregion

                PlayerStatistics.LoadStatistics(playerID);



                #region Command

                const string largeLine = "――――――――――――――――――――――――――――――――――――――――――――――――――――";
                const string EnvironmentSpace = " ";
                if (!playerName.All(char.IsDigit)) // If Not Steam ID
                {
                    if (NameLastCharContains_S(playerName)) // If name last char is S
                    {
                        #region Message
                        SendPlayerMessage(player,
                        $"{TextEncodeing(18, "ffffff", playerName)}<size=18>s</size>  {TextEncodeing(18, "3296fa", "PVP Statistics")}\n" +

                        $"{TextEncodeing(10, "ffffff", largeLine)}\n" +


                        $"{TextEncodeing(14, "3296fa", Kills)}{EnvironmentSpace}{TextEncodeing(14, "ffffff", "Kills")}" +
                        $"\n{TextEncodeing(14, "3296fa", longestKill)}{EnvironmentSpace}{TextEncodeing(14, "ffffff", "Longest Kill")}" +
                        $"\n{TextEncodeing(14, "3296fa", headShotsHit)}{EnvironmentSpace}{TextEncodeing(14, "ffffff", "Headshots")}" +
                        $"\n{TextEncodeing(14, "3296fa", longestHeadshotKill)}{EnvironmentSpace}{TextEncodeing(14, "ffffff", "Longest Headshot Kill")}" +
                        $"\n{TextEncodeing(14, "3296fa", hitAccuracy)}{EnvironmentSpace}{TextEncodeing(14, "ffffff", "Hit Accuracy")}" +
                        $"\n{TextEncodeing(14, "3296fa", headHitAccuracy)}{EnvironmentSpace}{TextEncodeing(14, "ffffff", "Headshot Accuracy")}" +
                        $"\n{TextEncodeing(14, "3296fa", shortsFired)}{EnvironmentSpace}{TextEncodeing(14, "ffffff", "Shorts Fired")}" +
                        $"\n{TextEncodeing(14, "3296fa", shortsHit)}{EnvironmentSpace}{TextEncodeing(14, "ffffff", "Shorts Hit")}" +
                        $"\n{TextEncodeing(14, "3296fa", rocketsFired)}{EnvironmentSpace}{TextEncodeing(14, "ffffff", "Rockets Fired")}" +
                        $"\n{TextEncodeing(14, "3296fa", C4used)}{EnvironmentSpace}{TextEncodeing(14, "ffffff", "C4 Used")}" +
                        $"\n{TextEncodeing(14, "3296fa", Deaths)}{EnvironmentSpace}{TextEncodeing(14, "ffffff", "Deaths")}" +
                        $"\n{TextEncodeing(14, "3296fa", KDR)}{EnvironmentSpace}{TextEncodeing(14, "ffffff", "KD Ratio")}" +


                        $"\n{TextEncodeing(10, "ffffff", largeLine)}");
                        #endregion

                    }
                    else if (!NameLastCharContains_S(playerName)) // If name last char is NOT S
                    {
                        #region Message
                        SendPlayerMessage(player,
                        $"{TextEncodeing(18, "ffffff", playerName)}<size=18></size>  {TextEncodeing(18, "3296fa", "PVP Statistics")}\n" +

                        $"{TextEncodeing(10, "ffffff", largeLine)}\n" +


                        $"{TextEncodeing(14, "3296fa", Kills)}{EnvironmentSpace}{TextEncodeing(14, "ffffff", "Kills")}" +
                        $"\n{TextEncodeing(14, "3296fa", longestKill)}{EnvironmentSpace}{TextEncodeing(14, "ffffff", "Longest Kill")}" +
                        $"\n{TextEncodeing(14, "3296fa", headShotsHit)}{EnvironmentSpace}{TextEncodeing(14, "ffffff", "Headshots")}" +
                        $"\n{TextEncodeing(14, "3296fa", longestHeadshotKill)}{EnvironmentSpace}{TextEncodeing(14, "ffffff", "Longest Headshot Kill")}" +
                        $"\n{TextEncodeing(14, "3296fa", hitAccuracy)}{EnvironmentSpace}{TextEncodeing(14, "ffffff", "Hit Accuracy")}" +
                        $"\n{TextEncodeing(14, "3296fa", headHitAccuracy)}{EnvironmentSpace}{TextEncodeing(14, "ffffff", "Headshot Accuracy")}" +
                        $"\n{TextEncodeing(14, "3296fa", shortsFired)}{EnvironmentSpace}{TextEncodeing(14, "ffffff", "Shorts Fired")}" +
                        $"\n{TextEncodeing(14, "3296fa", shortsHit)}{EnvironmentSpace}{TextEncodeing(14, "ffffff", "Shorts Hit")}" +
                        $"\n{TextEncodeing(14, "3296fa", rocketsFired)}{EnvironmentSpace}{TextEncodeing(14, "ffffff", "Rockets Fired")}" +
                        $"\n{TextEncodeing(14, "3296fa", C4used)}{EnvironmentSpace}{TextEncodeing(14, "ffffff", "C4 Used")}" +
                        $"\n{TextEncodeing(14, "3296fa", Deaths)}{EnvironmentSpace}{TextEncodeing(14, "ffffff", "Deaths")}" +
                        $"\n{TextEncodeing(14, "3296fa", KDR)}{EnvironmentSpace}{TextEncodeing(14, "ffffff", "KD Ratio")}" +


                        $"\n{TextEncodeing(10, "ffffff", largeLine)}");
                        #endregion
                    }
                }
                else // Target Stats
                {
                    #region Message
                    SendPlayerMessage(player,
                    $"{TextEncodeing(18, "ffffff", playerName)}<size=18></size>  {TextEncodeing(18, "3296fa", "PVP Statistics")}\n" +

                    $"{TextEncodeing(10, "ffffff", largeLine)}\n" +


                    $"{TextEncodeing(14, "3296fa", Kills)}{EnvironmentSpace}{TextEncodeing(14, "ffffff", "Kills")}" +
                    $"\n{TextEncodeing(14, "3296fa", longestKill)}{EnvironmentSpace}{TextEncodeing(14, "ffffff", "Longest Kill")}" +
                    $"\n{TextEncodeing(14, "3296fa", headShotsHit)}{EnvironmentSpace}{TextEncodeing(14, "ffffff", "Headshots")}" +
                    $"\n{TextEncodeing(14, "3296fa", longestHeadshotKill)}{EnvironmentSpace}{TextEncodeing(14, "ffffff", "Longest Headshot Kill")}" +
                    $"\n{TextEncodeing(14, "3296fa", hitAccuracy)}{EnvironmentSpace}{TextEncodeing(14, "ffffff", "Hit Accuracy")}" +
                    $"\n{TextEncodeing(14, "3296fa", headHitAccuracy)}{EnvironmentSpace}{TextEncodeing(14, "ffffff", "Headshot Accuracy")}" +
                    $"\n{TextEncodeing(14, "3296fa", shortsFired)}{EnvironmentSpace}{TextEncodeing(14, "ffffff", "Shorts Fired")}" +
                    $"\n{TextEncodeing(14, "3296fa", shortsHit)}{EnvironmentSpace}{TextEncodeing(14, "ffffff", "Shorts Hit")}" +
                    $"\n{TextEncodeing(14, "3296fa", rocketsFired)}{EnvironmentSpace}{TextEncodeing(14, "ffffff", "Rockets Fired")}" +
                    $"\n{TextEncodeing(14, "3296fa", C4used)}{EnvironmentSpace}{TextEncodeing(14, "ffffff", "C4 Used")}" +
                    $"\n{TextEncodeing(14, "3296fa", Deaths)}{EnvironmentSpace}{TextEncodeing(14, "ffffff", "Deaths")}" +
                    $"\n{TextEncodeing(14, "3296fa", KDR)}{EnvironmentSpace}{TextEncodeing(14, "ffffff", "KD Ratio")}" +


                    $"\n{TextEncodeing(10, "ffffff", largeLine)}");
                    #endregion
                }
                #endregion


                SaveSpecficPlyerData(player.userID);
            }
        }

        #endregion


        #region PVE Command

        [ChatCommand("stats.pve")]
        private void ChatCommandStatisticsPVE(BasePlayer player, string command, string[] args)
        {
            if (IsRealAdmin_Permission(player, config.Administrator_Group, ViewPermission) || IsRealMod_Permission(player, config.Moderator_Group, ViewPermission)) // ADMIN OR MOD
            {
                COMMAND_ChatCommandStatisticsPVE(player, command, args);
            }
            else // NOT ADMIN OR MOD DO Command Cooldown
            {
                if (!CommandCooldown.ContainsKey(player.userID))
                {
                    COMMAND_ChatCommandStatisticsPVE(player, command, args);

                    if (!CommandCooldown.ContainsKey(player.userID)) CommandCooldown.Add(player.userID, new PlayerCooldowns());


                    //Set Cooldown Manually
                    if (!CommandCooldown[player.userID].ExpireTimeSet)
                    {
                        var currentTime = Convert.ToDateTime(DateTime.Now.ToString("HH:mm:ss"));
                        CommandCooldown[player.userID].CommandCooldownExpire = currentTime.AddSeconds(config.PlayerCommandCooldown);
                        CommandCooldown[player.userID].ExpireTimeSet = true;
                    }
                }
                else if (CommandAvailable(player))
                {
                    COMMAND_ChatCommandStatisticsPVE(player, command, args);
                }
            }
        }

        private void COMMAND_ChatCommandStatisticsPVE(BasePlayer player, string command, string[] args)
        {

            ulong playerID = 0;
            string playerName = null;
            if (args != null && args.Length > 0)
            {
                if (IsRealAdmin_Permission(player, config.Administrator_Group, ViewPermission) || IsRealMod_Permission(player, config.Moderator_Group, ViewPermission)) // ADMIN OR MOD
                {
                    if (args[0].All(char.IsDigit))
                    {
                        ulong PlayeridArgs = Convert.ToUInt64(args[0]);

                        if (cachedPlayerStatistics.ContainsKey(PlayeridArgs)) // Cached Player Stats
                        {
                            playerID = PlayeridArgs;
                            playerName = PlayeridArgs.ToString();
                        }
                        else if (Interface.Oxide.DataFileSystem.ExistsDatafile($"{Name}/{PlayeridArgs}")) // Load From File
                        {
                            PlayerStatistics PlayerFileData = Interface.Oxide.DataFileSystem.ReadObject<PlayerStatistics>($"{Name}/{PlayeridArgs}");

                            playerID = PlayeridArgs;
                            playerName = PlayeridArgs.ToString();
                            PlayerStatistics.LoadStatistics(playerID);
                        }
                        else
                        {
                            SendPlayerMessage(player, "No File Data\nNo Cached Data");
                        }
                    }
                    else
                    {
                        SendPlayerMessage(player, "target steamID invalid\nUsage: /stats.pvp <SteamID>");
                    }
                }

            }
            else // Your Stats
            {
                playerID = player.userID;
                playerName = player.displayName;
            }

            //Save PlayerName In Data
            cachedPlayerStatistics[player.userID].PlayerName = player.displayName;

            if (playerID >= 1)
            {

                #region Stats Variables


                //PVE Stats
                string itemsCrafted = cachedPlayerStatistics[playerID].ItemsCrafted.ToString();
                string itemsResearch = cachedPlayerStatistics[playerID].ItemsResearch.ToString();
                string itemsRecycled = cachedPlayerStatistics[playerID].ItemsRecycled.ToString();
                string playersRevive = cachedPlayerStatistics[playerID].PlayersRevive.ToString();
                string Respawns = cachedPlayerStatistics[playerID].Respawns.ToString();
                string chatMessages = cachedPlayerStatistics[playerID].ChatMessages.ToString();
                string vendingMachineTrades = cachedPlayerStatistics[playerID].VendingMachineTrades.ToString();
                string npcKills = cachedPlayerStatistics[playerID].NpcKills.ToString();
                string bradleyKills = cachedPlayerStatistics[playerID].BradleyKills.ToString();
                string animalKills = cachedPlayerStatistics[playerID].AnimalKills.ToString();

                #endregion

                PlayerStatistics.LoadStatistics(playerID);



                #region Command

                const string largeLine = "――――――――――――――――――――――――――――――――――――――――――――――――――――";
                const string EnvironmentSpace = " ";
                if (!playerName.All(char.IsDigit)) // If Not Steam ID
                {
                    if (NameLastCharContains_S(playerName)) // If name last char is S
                    {
                        #region Message
                        SendPlayerMessage(player,
                        $"{TextEncodeing(18, "ffffff", playerName)}<size=18>s</size>  {TextEncodeing(18, "3296fa", "PVE Statistics")}\n" +

                        $"{TextEncodeing(10, "ffffff", largeLine)}\n" +


                        $"{TextEncodeing(14, "3296fa", itemsCrafted)}{EnvironmentSpace}{TextEncodeing(14, "ffffff", "Items Crafted")}" +
                        $"\n{TextEncodeing(14, "3296fa", itemsResearch)}{EnvironmentSpace}{TextEncodeing(14, "ffffff", "Items Researched")}" +
                        $"\n{TextEncodeing(14, "3296fa", itemsRecycled)}{EnvironmentSpace}{TextEncodeing(14, "ffffff", "Items Recycled")}" +
                        $"\n{TextEncodeing(14, "3296fa", playersRevive)}{EnvironmentSpace}{TextEncodeing(14, "ffffff", "Players Revived")}" +
                        $"\n{TextEncodeing(14, "3296fa", Respawns)}{EnvironmentSpace}{TextEncodeing(14, "ffffff", "Respawns")}" +
                        $"\n{TextEncodeing(14, "3296fa", vendingMachineTrades)}{EnvironmentSpace}{TextEncodeing(14, "ffffff", "Vending Machine Trades")}" +
                        $"\n{TextEncodeing(14, "3296fa", npcKills)}{EnvironmentSpace}{TextEncodeing(14, "ffffff", "Npcs Killed")}" +
                        $"\n{TextEncodeing(14, "3296fa", animalKills)}{EnvironmentSpace}{TextEncodeing(14, "ffffff", "Animals Killed")}" +
                        $"\n{TextEncodeing(14, "3296fa", bradleyKills)}{EnvironmentSpace}{TextEncodeing(14, "ffffff", "Bradleys Taken Down")}" +
                        $"\n{TextEncodeing(14, "3296fa", chatMessages)}{EnvironmentSpace}{TextEncodeing(14, "ffffff", "Chat Messages")}" +


                        $"\n{TextEncodeing(10, "ffffff", largeLine)}");
                        #endregion

                    }
                    else if (!NameLastCharContains_S(playerName)) // If name last char is NOT S
                    {
                        #region Message
                        SendPlayerMessage(player,
                        $"{TextEncodeing(18, "ffffff", playerName)}<size=18></size>  {TextEncodeing(18, "3296fa", "PVE Statistics")}\n" +

                        $"{TextEncodeing(10, "ffffff", largeLine)}\n" +


                        $"{TextEncodeing(14, "3296fa", itemsCrafted)}{EnvironmentSpace}{TextEncodeing(14, "ffffff", "Items Crafted")}" +
                        $"\n{TextEncodeing(14, "3296fa", itemsResearch)}{EnvironmentSpace}{TextEncodeing(14, "ffffff", "Items Researched")}" +
                        $"\n{TextEncodeing(14, "3296fa", itemsRecycled)}{EnvironmentSpace}{TextEncodeing(14, "ffffff", "Items Recycled")}" +
                        $"\n{TextEncodeing(14, "3296fa", playersRevive)}{EnvironmentSpace}{TextEncodeing(14, "ffffff", "Players Revived")}" +
                        $"\n{TextEncodeing(14, "3296fa", Respawns)}{EnvironmentSpace}{TextEncodeing(14, "ffffff", "Respawns")}" +
                        $"\n{TextEncodeing(14, "3296fa", vendingMachineTrades)}{EnvironmentSpace}{TextEncodeing(14, "ffffff", "Vending Machine Trades")}" +
                        $"\n{TextEncodeing(14, "3296fa", npcKills)}{EnvironmentSpace}{TextEncodeing(14, "ffffff", "Npcs Killed")}" +
                        $"\n{TextEncodeing(14, "3296fa", animalKills)}{EnvironmentSpace}{TextEncodeing(14, "ffffff", "Animals Killed")}" +
                        $"\n{TextEncodeing(14, "3296fa", bradleyKills)}{EnvironmentSpace}{TextEncodeing(14, "ffffff", "Bradleys Taken Down")}" +
                        $"\n{TextEncodeing(14, "3296fa", chatMessages)}{EnvironmentSpace}{TextEncodeing(14, "ffffff", "Chat Messages")}" +


                        $"\n{TextEncodeing(10, "ffffff", largeLine)}");
                        #endregion
                    }
                }
                else // Target Stats
                {
                    #region Message
                    SendPlayerMessage(player,
                    $"{TextEncodeing(18, "ffffff", playerName)}<size=18>s</size>  {TextEncodeing(18, "3296fa", "PVE Statistics")}\n" +

                    $"{TextEncodeing(10, "ffffff", largeLine)}\n" +


                    $"{TextEncodeing(14, "3296fa", itemsCrafted)}{EnvironmentSpace}{TextEncodeing(14, "ffffff", "Items Crafted")}" +
                    $"\n{TextEncodeing(14, "3296fa", itemsResearch)}{EnvironmentSpace}{TextEncodeing(14, "ffffff", "Items Researched")}" +
                    $"\n{TextEncodeing(14, "3296fa", itemsRecycled)}{EnvironmentSpace}{TextEncodeing(14, "ffffff", "Items Recycled")}" +
                    $"\n{TextEncodeing(14, "3296fa", playersRevive)}{EnvironmentSpace}{TextEncodeing(14, "ffffff", "Players Revived")}" +
                    $"\n{TextEncodeing(14, "3296fa", Respawns)}{EnvironmentSpace}{TextEncodeing(14, "ffffff", "Respawns")}" +
                    $"\n{TextEncodeing(14, "3296fa", vendingMachineTrades)}{EnvironmentSpace}{TextEncodeing(14, "ffffff", "Vending Machine Trades")}" +
                    $"\n{TextEncodeing(14, "3296fa", npcKills)}{EnvironmentSpace}{TextEncodeing(14, "ffffff", "Npcs Killed")}" +
                    $"\n{TextEncodeing(14, "3296fa", animalKills)}{EnvironmentSpace}{TextEncodeing(14, "ffffff", "Animals Killed")}" +
                    $"\n{TextEncodeing(14, "3296fa", bradleyKills)}{EnvironmentSpace}{TextEncodeing(14, "ffffff", "Bradleys Taken Down")}" +
                    $"\n{TextEncodeing(14, "3296fa", chatMessages)}{EnvironmentSpace}{TextEncodeing(14, "ffffff", "Chat Messages")}" +


                    $"\n{TextEncodeing(10, "ffffff", largeLine)}");
                    #endregion
                }
                #endregion


                SaveSpecficPlyerData(player.userID);
            }
        }

        #endregion


        #region Travel Command
        [ChatCommand("stats.travel")]
        private void ChatCommandStatisticsTRAVEL(BasePlayer player, string command, string[] args)
        {
            if (IsRealAdmin_Permission(player, config.Administrator_Group, ViewPermission) || IsRealMod_Permission(player, config.Moderator_Group, ViewPermission)) // ADMIN OR MOD
            {
                COMMAND_ChatCommandStatisticsTRAVEL(player, command, args);
            }
            else // NOT ADMIN OR MOD DO Command Cooldown
            {
                if (!CommandCooldown.ContainsKey(player.userID))
                {
                    COMMAND_ChatCommandStatisticsTRAVEL(player, command, args);

                    if (!CommandCooldown.ContainsKey(player.userID)) CommandCooldown.Add(player.userID, new PlayerCooldowns());


                    //Set Cooldown Manually
                    if (!CommandCooldown[player.userID].ExpireTimeSet)
                    {
                        var currentTime = Convert.ToDateTime(DateTime.Now.ToString("HH:mm:ss"));
                        CommandCooldown[player.userID].CommandCooldownExpire = currentTime.AddSeconds(config.PlayerCommandCooldown);
                        CommandCooldown[player.userID].ExpireTimeSet = true;
                    }
                }
                else if (CommandAvailable(player))
                {
                    COMMAND_ChatCommandStatisticsTRAVEL(player, command, args);
                }
            }
        }



        private void COMMAND_ChatCommandStatisticsTRAVEL(BasePlayer player, string command, string[] args)
        {
            UpdatePlayerDistanceTraveled(player);


            ulong playerID = 0;
            string playerName = null;
            if (args != null && args.Length > 0)
            {
                if (IsRealAdmin_Permission(player, config.Administrator_Group, ViewPermission) || IsRealMod_Permission(player, config.Moderator_Group, ViewPermission)) // ADMIN OR MOD
                {
                    if (args[0].All(char.IsDigit))
                    {
                        ulong PlayeridArgs = Convert.ToUInt64(args[0]);

                        if (cachedPlayerStatistics.ContainsKey(PlayeridArgs)) // Cached Player Stats
                        {
                            playerID = PlayeridArgs;
                            playerName = PlayeridArgs.ToString();
                        }
                        else if (Interface.Oxide.DataFileSystem.ExistsDatafile($"{Name}/{PlayeridArgs}")) // Load From File
                        {
                            PlayerStatistics PlayerFileData = Interface.Oxide.DataFileSystem.ReadObject<PlayerStatistics>($"{Name}/{PlayeridArgs}");

                            playerID = PlayeridArgs;
                            playerName = PlayeridArgs.ToString();
                            PlayerStatistics.LoadStatistics(playerID);
                        }
                        else
                        {
                            SendPlayerMessage(player, "No File Data\nNo Cached Data");
                        }
                    }
                    else
                    {
                        SendPlayerMessage(player, "target steamID invalid\nUsage: /stats.pvp <SteamID>");
                    }
                }

            }
            else // Your Stats
            {
                playerID = player.userID;
                playerName = player.displayName;
            }

            //Save PlayerName In Data
            cachedPlayerStatistics[player.userID].PlayerName = player.displayName;


            if (playerID >= 1)
            {

                #region Stats Variables


                //Distance Stats
                string distanceDriving = cachedPlayerStatistics[playerID].DistanceDriving + "m";
                string distanceFlying = cachedPlayerStatistics[playerID].DistanceFlying + "m";
                string distanceSailing = cachedPlayerStatistics[playerID].DistanceSailing + "m";
                string distanceWalking = cachedPlayerStatistics[playerID].DistanceWalking + "m";
                string totalDistanceTraveled = cachedPlayerStatistics[playerID].TotalDistanceTraveled + "m";

                #endregion

                PlayerStatistics.LoadStatistics(playerID);



                #region Command

                const string largeLine = "――――――――――――――――――――――――――――――――――――――――――――――――――――";
                const string EnvironmentSpace = " ";
                if (!playerName.All(char.IsDigit)) // If Not Steam ID
                {
                    if (NameLastCharContains_S(playerName)) // If name last char is S
                    {
                        #region Message
                        SendPlayerMessage(player,
                        $"{TextEncodeing(18, "ffffff", playerName)}<size=18>s</size>  {TextEncodeing(18, "3296fa", "Travel Statistics")}\n" +

                        $"{TextEncodeing(10, "ffffff", largeLine)}\n" +


                        $"{TextEncodeing(14, "3296fa", distanceWalking)}{EnvironmentSpace}{TextEncodeing(14, "ffffff", "Distance Ran")}" +
                        $"\n{TextEncodeing(14, "3296fa", distanceDriving)}{EnvironmentSpace}{TextEncodeing(14, "ffffff", "Distance Driven")}" +
                        $"\n{TextEncodeing(14, "3296fa", distanceSailing)}{EnvironmentSpace}{TextEncodeing(14, "ffffff", "Distance Sailed")}" +
                        $"\n{TextEncodeing(14, "3296fa", distanceFlying)}{EnvironmentSpace}{TextEncodeing(14, "ffffff", "Distance Flown")}" +
                        $"\n{TextEncodeing(14, "3296fa", totalDistanceTraveled)}{EnvironmentSpace}{TextEncodeing(14, "ffffff", "Total Distance Traveled")}" +

                        $"\n{TextEncodeing(10, "ffffff", largeLine)}");
                        #endregion

                    }
                    else if (!NameLastCharContains_S(playerName)) // If name last char is NOT S
                    {
                        #region Message
                        SendPlayerMessage(player,
                        $"{TextEncodeing(18, "ffffff", playerName)}<size=18></size>  {TextEncodeing(18, "3296fa", "Travel Statistics")}\n" +

                        $"{TextEncodeing(10, "ffffff", largeLine)}\n" +


                        $"{TextEncodeing(14, "3296fa", distanceWalking)}{EnvironmentSpace}{TextEncodeing(14, "ffffff", "Distance Ran")}" +
                        $"\n{TextEncodeing(14, "3296fa", distanceDriving)}{EnvironmentSpace}{TextEncodeing(14, "ffffff", "Distance Driven")}" +
                        $"\n{TextEncodeing(14, "3296fa", distanceSailing)}{EnvironmentSpace}{TextEncodeing(14, "ffffff", "Distance Sailed")}" +
                        $"\n{TextEncodeing(14, "3296fa", distanceFlying)}{EnvironmentSpace}{TextEncodeing(14, "ffffff", "Distance Flown")}" +
                        $"\n{TextEncodeing(14, "3296fa", totalDistanceTraveled)}{EnvironmentSpace}{TextEncodeing(14, "ffffff", "Total Distance Traveled")}" +

                        $"\n{TextEncodeing(10, "ffffff", largeLine)}");
                        #endregion
                    }
                }
                else // Target Stats
                {
                    #region Message
                    SendPlayerMessage(player,
                    $"{TextEncodeing(18, "ffffff", playerName)}<size=18></size>  {TextEncodeing(18, "3296fa", "Travel Statistics")}\n" +

                    $"{TextEncodeing(10, "ffffff", largeLine)}\n" +


                    $"{TextEncodeing(14, "3296fa", distanceWalking)}{EnvironmentSpace}{TextEncodeing(14, "ffffff", "Distance Ran")}" +
                    $"\n{TextEncodeing(14, "3296fa", distanceDriving)}{EnvironmentSpace}{TextEncodeing(14, "ffffff", "Distance Driven")}" +
                    $"\n{TextEncodeing(14, "3296fa", distanceSailing)}{EnvironmentSpace}{TextEncodeing(14, "ffffff", "Distance Sailed")}" +
                    $"\n{TextEncodeing(14, "3296fa", distanceFlying)}{EnvironmentSpace}{TextEncodeing(14, "ffffff", "Distance Flown")}" +
                    $"\n{TextEncodeing(14, "3296fa", totalDistanceTraveled)}{EnvironmentSpace}{TextEncodeing(14, "ffffff", "Total Distance Traveled")}" +

                    $"\n{TextEncodeing(10, "ffffff", largeLine)}");
                    #endregion
                }
                #endregion


                SaveSpecficPlyerData(player.userID);
            }
        }
        #endregion


        #region Loot Command
        [ChatCommand("stats.loot")]
        private void ChatCommandStatisticsLOOT(BasePlayer player, string command, string[] args)
        {
            if (IsRealAdmin_Permission(player, config.Administrator_Group, ViewPermission) || IsRealMod_Permission(player, config.Moderator_Group, ViewPermission)) // ADMIN OR MOD
            {
                COMMAND_ChatCommandStatisticsLOOT(player, command, args);
            }
            else // NOT ADMIN OR MOD DO Command Cooldown
            {
                if (!CommandCooldown.ContainsKey(player.userID))
                {
                    COMMAND_ChatCommandStatisticsLOOT(player, command, args);

                    if (!CommandCooldown.ContainsKey(player.userID)) CommandCooldown.Add(player.userID, new PlayerCooldowns());


                    //Set Cooldown Manually
                    if (!CommandCooldown[player.userID].ExpireTimeSet)
                    {
                        var currentTime = Convert.ToDateTime(DateTime.Now.ToString("HH:mm:ss"));
                        CommandCooldown[player.userID].CommandCooldownExpire = currentTime.AddSeconds(config.PlayerCommandCooldown);
                        CommandCooldown[player.userID].ExpireTimeSet = true;
                    }
                }
                else if (CommandAvailable(player))
                {
                    COMMAND_ChatCommandStatisticsLOOT(player, command, args);
                }
            }
        }


        private void COMMAND_ChatCommandStatisticsLOOT(BasePlayer player, string command, string[] args)
        {
            ulong playerID = 0;
            string playerName = null;
            if (args != null && args.Length > 0)
            {
                if (IsRealAdmin_Permission(player, config.Administrator_Group, ViewPermission) || IsRealMod_Permission(player, config.Moderator_Group, ViewPermission)) // ADMIN OR MOD
                {
                    if (args[0].All(char.IsDigit))
                    {
                        ulong PlayeridArgs = Convert.ToUInt64(args[0]);

                        if (cachedPlayerStatistics.ContainsKey(PlayeridArgs)) // Cached Player Stats
                        {
                            playerID = PlayeridArgs;
                            playerName = PlayeridArgs.ToString();
                        }
                        else if (Interface.Oxide.DataFileSystem.ExistsDatafile($"{Name}/{PlayeridArgs}")) // Load From File
                        {
                            PlayerStatistics PlayerFileData = Interface.Oxide.DataFileSystem.ReadObject<PlayerStatistics>($"{Name}/{PlayeridArgs}");

                            playerID = PlayeridArgs;
                            playerName = PlayeridArgs.ToString();
                            PlayerStatistics.LoadStatistics(playerID);
                        }
                        else
                        {
                            SendPlayerMessage(player, "No File Data\nNo Cached Data");
                        }
                    }
                    else
                    {
                        SendPlayerMessage(player, "target steamID invalid\nUsage: /stats.pvp <SteamID>");
                    }
                }

            }
            else // Your Stats
            {
                playerID = player.userID;
                playerName = player.displayName;
            }

            //Save PlayerName In Data
            cachedPlayerStatistics[player.userID].PlayerName = player.displayName;

            if (playerID >= 1)
            {

                #region Stats Variables


                //Loot Stats
                string lootItemsStolen = cachedPlayerStatistics[playerID].LootItemsStolen.ToString();
                string woodGathered = cachedPlayerStatistics[playerID].WoodGathered.ToString();
                string stoneGathered = cachedPlayerStatistics[playerID].StoneGathered.ToString();
                string metalOreGathered = cachedPlayerStatistics[playerID].MetalOreGathered.ToString();
                string sulfurOreGathered = cachedPlayerStatistics[playerID].SulfurOreGathered.ToString();
                string HQMOreGathered = cachedPlayerStatistics[playerID].HQMOreGathered.ToString();
                string airdropsLooted = cachedPlayerStatistics[playerID].AirdropsLooted.ToString();

                #endregion

                PlayerStatistics.LoadStatistics(playerID);



                #region Command

                const string largeLine = "――――――――――――――――――――――――――――――――――――――――――――――――――――";
                const string EnvironmentSpace = " ";
                if (!playerName.All(char.IsDigit)) // If Not Steam ID
                {
                    if (NameLastCharContains_S(playerName)) // If name last char is S
                    {
                        #region Message
                        SendPlayerMessage(player,
                        $"{TextEncodeing(18, "ffffff", playerName)}<size=18>s</size>  {TextEncodeing(18, "3296fa", "Loot Statistics")}\n" +

                        $"{TextEncodeing(10, "ffffff", largeLine)}\n" +


                        $"{TextEncodeing(14, "3296fa", woodGathered)}{EnvironmentSpace}{TextEncodeing(14, "ffffff", "Wood Gathered")}" +
                        $"\n{TextEncodeing(14, "3296fa", stoneGathered)}{EnvironmentSpace}{TextEncodeing(14, "ffffff", "Stone Gathered")}" +
                        $"\n{TextEncodeing(14, "3296fa", metalOreGathered)}{EnvironmentSpace}{TextEncodeing(14, "ffffff", "Metal Ore Gathered")}" +
                        $"\n{TextEncodeing(14, "3296fa", sulfurOreGathered)}{EnvironmentSpace}{TextEncodeing(14, "ffffff", "Sulfur Ore Gathered")}" +
                        $"\n{TextEncodeing(14, "3296fa", HQMOreGathered)}{EnvironmentSpace}{TextEncodeing(14, "ffffff", "HQM Ore Gathered")}" +
                        $"\n{TextEncodeing(14, "3296fa", lootItemsStolen)}{EnvironmentSpace}{TextEncodeing(14, "ffffff", "Loot Items Stolen")}" +
                        $"\n{TextEncodeing(14, "3296fa", airdropsLooted)}{EnvironmentSpace}{TextEncodeing(14, "ffffff", "Airdrops Looted")}" +


                        $"\n{TextEncodeing(10, "ffffff", largeLine)}");
                        #endregion

                    }
                    else if (!NameLastCharContains_S(playerName)) // If name last char is NOT S
                    {
                        #region Message
                        SendPlayerMessage(player,
                        $"{TextEncodeing(18, "ffffff", playerName)}<size=18></size>  {TextEncodeing(18, "3296fa", "Loot Statistics")}\n" +

                        $"{TextEncodeing(10, "ffffff", largeLine)}\n" +


                        $"{TextEncodeing(14, "3296fa", woodGathered)}{EnvironmentSpace}{TextEncodeing(14, "ffffff", "Wood Gathered")}" +
                        $"\n{TextEncodeing(14, "3296fa", stoneGathered)}{EnvironmentSpace}{TextEncodeing(14, "ffffff", "Stone Gathered")}" +
                        $"\n{TextEncodeing(14, "3296fa", metalOreGathered)}{EnvironmentSpace}{TextEncodeing(14, "ffffff", "Metal Ore Gathered")}" +
                        $"\n{TextEncodeing(14, "3296fa", sulfurOreGathered)}{EnvironmentSpace}{TextEncodeing(14, "ffffff", "Sulfur Ore Gathered")}" +
                        $"\n{TextEncodeing(14, "3296fa", HQMOreGathered)}{EnvironmentSpace}{TextEncodeing(14, "ffffff", "HQM Ore Gathered")}" +
                        $"\n{TextEncodeing(14, "3296fa", lootItemsStolen)}{EnvironmentSpace}{TextEncodeing(14, "ffffff", "Loot Items Stolen")}" +
                        $"\n{TextEncodeing(14, "3296fa", airdropsLooted)}{EnvironmentSpace}{TextEncodeing(14, "ffffff", "Airdrops Looted")}" +


                        $"\n{TextEncodeing(10, "ffffff", largeLine)}");
                        #endregion
                    }
                }
                else // Target Stats
                {
                    #region Message
                    SendPlayerMessage(player,
                    $"{TextEncodeing(18, "ffffff", playerName)}<size=18></size>  {TextEncodeing(18, "3296fa", "Loot Statistics")}\n" +

                    $"{TextEncodeing(10, "ffffff", largeLine)}\n" +


                    $"{TextEncodeing(14, "3296fa", woodGathered)}{EnvironmentSpace}{TextEncodeing(14, "ffffff", "Wood Gathered")}" +
                    $"\n{TextEncodeing(14, "3296fa", stoneGathered)}{EnvironmentSpace}{TextEncodeing(14, "ffffff", "Stone Gathered")}" +
                    $"\n{TextEncodeing(14, "3296fa", metalOreGathered)}{EnvironmentSpace}{TextEncodeing(14, "ffffff", "Metal Ore Gathered")}" +
                    $"\n{TextEncodeing(14, "3296fa", sulfurOreGathered)}{EnvironmentSpace}{TextEncodeing(14, "ffffff", "Sulfur Ore Gathered")}" +
                    $"\n{TextEncodeing(14, "3296fa", HQMOreGathered)}{EnvironmentSpace}{TextEncodeing(14, "ffffff", "HQM Ore Gathered")}" +
                    $"\n{TextEncodeing(14, "3296fa", lootItemsStolen)}{EnvironmentSpace}{TextEncodeing(14, "ffffff", "Loot Items Stolen")}" +
                    $"\n{TextEncodeing(14, "3296fa", airdropsLooted)}{EnvironmentSpace}{TextEncodeing(14, "ffffff", "Airdrops Looted")}" +


                    $"\n{TextEncodeing(10, "ffffff", largeLine)}");
                    #endregion
                }
                #endregion


                SaveSpecficPlyerData(player.userID);
            }
        }

        #endregion


        #region Playtime Command

        private void UpdatePlaytime(BasePlayer player)
        {

            #region Reset Check
            //Reset Month
            if (cachedPlayerStatistics[player.userID].CurrentMonthIndex != DateTime.Now.Month)
            {

                SendPlayerMessage(player, "Reset MONTH");
                cachedPlayerStatistics[player.userID].CurrentMonthIndex = DateTime.Now.Month;
                cachedPlayerStatistics[player.userID].PlaytimeMonthHours = 0;
            }


            //Reset Day
            if (cachedPlayerStatistics[player.userID].CurrentDayIndex != DateTime.Now.Day)
            {

                SendPlayerMessage(player, "Reset DAY");
                cachedPlayerStatistics[player.userID].CurrentDayIndex = DateTime.Now.Day;
                cachedPlayerStatistics[player.userID].PlaytimeTodayHours = 0;
            }

            #endregion




            var hours = (DateTime.Now - cachedPlayerStatistics[player.userID].PlayerConnectedAt).TotalHours;
            var hoursShort = String.Format("{0:0.0}", hours);


            cachedPlayerStatistics[player.userID].TotalPlaytimeHours += Convert.ToDouble(hoursShort);

            cachedPlayerStatistics[player.userID].PlaytimeTodayHours += Convert.ToDouble(hoursShort);
            cachedPlayerStatistics[player.userID].PlaytimeMonthHours += Convert.ToDouble(hoursShort);

        }


        [ChatCommand("stats.playtime")]
        private void ChatCommandStatisticsPLAYTIME(BasePlayer player, string command, string[] args)
        {
            if (IsRealAdmin_Permission(player, config.Administrator_Group, ViewPermission) || IsRealMod_Permission(player, config.Moderator_Group, ViewPermission)) // ADMIN OR MOD
            {
                COMMAND_ChatCommandStatisticsPLAYTIME(player, command, args);
            }
            else // NOT ADMIN OR MOD DO Command Cooldown
            {
                if (!CommandCooldown.ContainsKey(player.userID))
                {
                    COMMAND_ChatCommandStatisticsPLAYTIME(player, command, args);

                    if (!CommandCooldown.ContainsKey(player.userID)) CommandCooldown.Add(player.userID, new PlayerCooldowns());


                    //Set Cooldown Manually
                    if (!CommandCooldown[player.userID].ExpireTimeSet)
                    {
                        var currentTime = Convert.ToDateTime(DateTime.Now.ToString("HH:mm:ss"));
                        CommandCooldown[player.userID].CommandCooldownExpire = currentTime.AddSeconds(config.PlayerCommandCooldown);
                        CommandCooldown[player.userID].ExpireTimeSet = true;
                    }
                }
                else if (CommandAvailable(player))
                {
                    COMMAND_ChatCommandStatisticsPLAYTIME(player, command, args);
                }
            }
        }

        private void COMMAND_ChatCommandStatisticsPLAYTIME(BasePlayer player, string command, string[] args)
        {
            UpdatePlaytime(player);


            ulong playerID = 0;
            string playerName = null;
            if (args != null && args.Length > 0)
            {
                if (IsRealAdmin_Permission(player, config.Administrator_Group, ViewPermission) || IsRealMod_Permission(player, config.Moderator_Group, ViewPermission)) // ADMIN OR MOD
                {
                    if (args[0].All(char.IsDigit))
                    {
                        ulong PlayeridArgs = Convert.ToUInt64(args[0]);

                        if (cachedPlayerStatistics.ContainsKey(PlayeridArgs)) // Cached Player Stats
                        {
                            playerID = PlayeridArgs;
                            playerName = PlayeridArgs.ToString();
                        }
                        else if (Interface.Oxide.DataFileSystem.ExistsDatafile($"{Name}/{PlayeridArgs}")) // Load From File
                        {
                            PlayerStatistics PlayerFileData = Interface.Oxide.DataFileSystem.ReadObject<PlayerStatistics>($"{Name}/{PlayeridArgs}");

                            playerID = PlayeridArgs;
                            playerName = PlayeridArgs.ToString();
                            PlayerStatistics.LoadStatistics(playerID);
                        }
                        else
                        {
                            SendPlayerMessage(player, "No File Data\nNo Cached Data");
                        }
                    }
                    else
                    {
                        SendPlayerMessage(player, "target steamID invalid\nUsage: /stats.pvp <SteamID>");
                    }
                }

            }
            else // Your Stats
            {
                playerID = player.userID;
                playerName = player.displayName;
            }

            //Save PlayerName In Data
            cachedPlayerStatistics[player.userID].PlayerName = player.displayName;


            if (playerID >= 1)
            {

                #region Stats Variables

                //Playtime Stats
                string totalPlaytimeHours = cachedPlayerStatistics[playerID].TotalPlaytimeHours + "h";

                string playtimeTodayHours = cachedPlayerStatistics[playerID].PlaytimeTodayHours + "h";
                string playtimeMonthHours = cachedPlayerStatistics[playerID].PlaytimeMonthHours + "h";

                #endregion

                PlayerStatistics.LoadStatistics(playerID);



                #region Command

                const string largeLine = "――――――――――――――――――――――――――――――――――――――――――――――――――――";
                const string EnvironmentSpace = " ";
                if (!playerName.All(char.IsDigit)) // If Not Steam ID
                {
                    if (NameLastCharContains_S(playerName)) // If name last char is S
                    {
                        #region Message
                        SendPlayerMessage(player,
                        $"{TextEncodeing(18, "ffffff", playerName)}<size=18>s</size>  {TextEncodeing(18, "3296fa", "Playtime Statistics")}\n" +

                        $"{TextEncodeing(10, "ffffff", largeLine)}\n" +


                        $"{TextEncodeing(14, "3296fa", playtimeTodayHours)}{EnvironmentSpace}{TextEncodeing(14, "ffffff", "Hours Played Today")}" +
                        $"\n{TextEncodeing(14, "3296fa", playtimeMonthHours)}{EnvironmentSpace}{TextEncodeing(14, "ffffff", "Hours Played This Month")}" +
                        $"\n{TextEncodeing(14, "3296fa", totalPlaytimeHours)}{EnvironmentSpace}{TextEncodeing(14, "ffffff", "Total Hours Played")}" +


                        $"\n{TextEncodeing(10, "ffffff", largeLine)}");
                        #endregion

                    }
                    else if (!NameLastCharContains_S(playerName)) // If name last char is NOT S
                    {
                        #region Message
                        SendPlayerMessage(player,
                        $"{TextEncodeing(18, "ffffff", playerName)}<size=18></size>  {TextEncodeing(18, "3296fa", "Playtime Statistics")}\n" +

                        $"{TextEncodeing(10, "ffffff", largeLine)}\n" +


                        $"{TextEncodeing(14, "3296fa", playtimeTodayHours)}{EnvironmentSpace}{TextEncodeing(14, "ffffff", "Hours Played Today")}" +
                        $"\n{TextEncodeing(14, "3296fa", playtimeMonthHours)}{EnvironmentSpace}{TextEncodeing(14, "ffffff", "Hours Played This Month")}" +
                        $"\n{TextEncodeing(14, "3296fa", totalPlaytimeHours)}{EnvironmentSpace}{TextEncodeing(14, "ffffff", "Total Hours Played")}" +


                        $"\n{TextEncodeing(10, "ffffff", largeLine)}");
                        #endregion
                    }
                }
                else // Target Stats
                {
                    #region Message
                    SendPlayerMessage(player,
                    $"{TextEncodeing(18, "ffffff", playerName)}<size=18></size>  {TextEncodeing(18, "3296fa", "Playtime Statistics")}\n" +

                    $"{TextEncodeing(10, "ffffff", largeLine)}\n" +


                    $"{TextEncodeing(14, "3296fa", playtimeTodayHours)}{EnvironmentSpace}{TextEncodeing(14, "ffffff", "Hours Played Today")}" +
                    $"\n{TextEncodeing(14, "3296fa", playtimeMonthHours)}{EnvironmentSpace}{TextEncodeing(14, "ffffff", "Hours Played This Month")}" +
                    $"\n{TextEncodeing(14, "3296fa", totalPlaytimeHours)}{EnvironmentSpace}{TextEncodeing(14, "ffffff", "Total Hours Played")}" +


                    $"\n{TextEncodeing(10, "ffffff", largeLine)}");
                    #endregion
                }
                #endregion


                SaveSpecficPlyerData(player.userID);
            }
        }

        #endregion


        #region Top Command
        [ChatCommand("stats.top")]
        private void ChatCommandStatisticsLEADERBOARD(BasePlayer player, string command, string[] args)
        {
            if (IsRealAdmin_Permission(player, config.Administrator_Group, ViewPermission) || IsRealMod_Permission(player, config.Moderator_Group, ViewPermission)) // ADMIN OR MOD
            {
                COMMAND_ChatCommandStatisticsLEADERBOARD(player);
            }
            else // NOT ADMIN OR MOD DO Command Cooldown
            {
                if (!CommandCooldown.ContainsKey(player.userID))
                {
                    COMMAND_ChatCommandStatisticsLEADERBOARD(player);

                    if (!CommandCooldown.ContainsKey(player.userID)) CommandCooldown.Add(player.userID, new PlayerCooldowns());


                    //Set Cooldown Manually
                    if (!CommandCooldown[player.userID].ExpireTimeSet)
                    {
                        var currentTime = Convert.ToDateTime(DateTime.Now.ToString("HH:mm:ss"));
                        CommandCooldown[player.userID].CommandCooldownExpire = currentTime.AddSeconds(config.PlayerCommandCooldown);
                        CommandCooldown[player.userID].ExpireTimeSet = true;
                    }
                }
                else if (CommandAvailable(player))
                {
                    COMMAND_ChatCommandStatisticsLEADERBOARD(player);
                }
            }
        }

        private void COMMAND_ChatCommandStatisticsLEADERBOARD(BasePlayer player)
        {

            TopPlayerStatistics.ResetTopStats();

            //Save PlayerName In Data
            cachedPlayerStatistics[player.userID].PlayerName = player.displayName;

            SaveSpecficPlyerData(player.userID);

            const string largeLine = "――――――――――――――――――――――――――――――――――――――――――――――――――――";
            const string EnvironmentSpace = " ";

            #region Message
            SendPlayerMessage(player,


            $"{TextEncodeing(18, "ffffff", "Leaderboard")}{EnvironmentSpace}{TextEncodeing(18, "3296fa", "Statistics")}\n" +
            $"{TextEncodeing(10, "ffffff", largeLine)}\n" +

            $"{GetTopPlayerData("kills")}" +
            $"\n\n{GetTopPlayerData("headshots")}" +
            $"\n\n{GetTopPlayerData("longestkill")}" +
            $"\n\n{GetTopPlayerData("kdr")}" +
            $"\n\n{GetTopPlayerData("playtime")}" +
            $"\n\n{GetTopPlayerData("distance")}" +
            $"\n\n{GetTopPlayerData("bradleykills")}" +
            $"\n\n{GetTopPlayerData("animalkills")}" +
            $"\n\n{GetTopPlayerData("lootstolen")}" +


            $"\n{TextEncodeing(10, "ffffff", largeLine)}");
            #endregion



        }





        private string GetTopPlayerData(string selector)
        {

            #region Set Top Data
            var filePath = Interface.Oxide.DataFileSystem.GetFiles("XStats", "*.json");
            foreach (var x in filePath)
            {

                #region Get SteamID
                //Get Number
                string StringInput = x;
                string IntOutput = string.Empty;

                for (int i = 0; i < StringInput.Length; i++)
                {
                    if (Char.IsDigit(StringInput[i]))
                        IntOutput += StringInput[i];
                }

                var SteamID = IntOutput.Remove(0, 1);
                #endregion

                #region Set Leaderboard
                PlayerStatistics playerData = Interface.Oxide.DataFileSystem.ReadObject<PlayerStatistics>($"XStats/{SteamID}");


                if (playerData.Kills > TopPlayerStatistics.MostKills) { TopPlayerStatistics.MostKills = playerData.Kills; TopPlayerStatistics.MostKills_playerName = playerData.PlayerName; }

                if (playerData.HeadShotsHit > TopPlayerStatistics.MostHeadshots) { TopPlayerStatistics.MostHeadshots = playerData.HeadShotsHit; TopPlayerStatistics.MostHeadshots_playerName = playerData.PlayerName; }

                if (playerData.LongestKill > TopPlayerStatistics.LongestKill) { TopPlayerStatistics.LongestKill = playerData.LongestKill; TopPlayerStatistics.LongestKill_playerName = playerData.PlayerName; }

                if (playerData.KDR > TopPlayerStatistics.HighestKDR) { TopPlayerStatistics.HighestKDR = playerData.KDR; TopPlayerStatistics.HighestKDR_playerName = playerData.PlayerName; }

                if (playerData.TotalPlaytimeHours > TopPlayerStatistics.MostTimePlayed) { TopPlayerStatistics.MostTimePlayed = playerData.TotalPlaytimeHours; TopPlayerStatistics.MostTimePlayed_playerName = playerData.PlayerName; }

                if (playerData.TotalDistanceTraveled > TopPlayerStatistics.MostDistanceTraveled) { TopPlayerStatistics.MostDistanceTraveled = playerData.TotalDistanceTraveled; TopPlayerStatistics.MostDistanceTraveled_playerName = playerData.PlayerName; }

                if (playerData.BradleyKills > TopPlayerStatistics.MostBradleyTakedowns) { TopPlayerStatistics.MostBradleyTakedowns = playerData.BradleyKills; TopPlayerStatistics.MostBradleyTakedowns_playerName = playerData.PlayerName; }

                if (playerData.AnimalKills > TopPlayerStatistics.MostAnimalKills) { TopPlayerStatistics.MostAnimalKills = playerData.AnimalKills; TopPlayerStatistics.MostAnimalKills_playerName = playerData.PlayerName; }

                if (playerData.LootItemsStolen > TopPlayerStatistics.MostLootStolen) { TopPlayerStatistics.MostLootStolen = playerData.LootItemsStolen; TopPlayerStatistics.MostLootStolen_playerName = playerData.PlayerName; }

                #endregion



                #region Check IF EMPTY
                string blankName = TextEncodeing(14, "fcba03", "No Record Set Yet");

                if (TopPlayerStatistics.MostKills <= 0) TopPlayerStatistics.MostKills_playerName = blankName;
                if (TopPlayerStatistics.MostHeadshots <= 0) TopPlayerStatistics.MostHeadshots_playerName = blankName;
                if (TopPlayerStatistics.LongestKill <= 0) TopPlayerStatistics.LongestKill_playerName = blankName;
                if (TopPlayerStatistics.HighestKDR <= 0) TopPlayerStatistics.HighestKDR_playerName = blankName;
                if (TopPlayerStatistics.MostTimePlayed <= 0) TopPlayerStatistics.MostTimePlayed_playerName = blankName;
                if (TopPlayerStatistics.MostDistanceTraveled <= 0) TopPlayerStatistics.MostDistanceTraveled_playerName = blankName;
                if (TopPlayerStatistics.MostBradleyTakedowns <= 0) TopPlayerStatistics.MostBradleyTakedowns_playerName = blankName;
                if (TopPlayerStatistics.MostAnimalKills <= 0) TopPlayerStatistics.MostAnimalKills_playerName = blankName;
                if (TopPlayerStatistics.MostLootStolen <= 0) TopPlayerStatistics.MostLootStolen_playerName = blankName;
                #endregion
            }
            #endregion


            #region Return

            if (selector != null)
            {
                var i = selector.ToLower();

                if (i == "kills") return $"{TextEncodeing(14, "ffffff", "Most Kills")} {TextEncodeing(14, "3296fa", $"{TopPlayerStatistics.MostKills}")} {TextEncodeing(14, "ffffff", "By")} {TextEncodeing(14, "3296fa", $"{TopPlayerStatistics.MostKills_playerName}")}";

                if (i == "headshots") return $"{TextEncodeing(14, "ffffff", "Most Headshots")} {TextEncodeing(14, "3296fa", $"{TopPlayerStatistics.MostHeadshots}")} {TextEncodeing(14, "ffffff", "By")} {TextEncodeing(14, "3296fa", $"{TopPlayerStatistics.MostHeadshots_playerName}")}";

                if (i == "longestkill") return $"{TextEncodeing(14, "ffffff", "Longest Kill")} {TextEncodeing(14, "3296fa", $"{TopPlayerStatistics.LongestKill}m")} {TextEncodeing(14, "ffffff", "By")} {TextEncodeing(14, "3296fa", $"{TopPlayerStatistics.LongestKill_playerName}")}";

                if (i == "kdr") return $"{TextEncodeing(14, "ffffff", "Highest KD Ratio")} {TextEncodeing(14, "3296fa", $"{TopPlayerStatistics.HighestKDR}")} {TextEncodeing(14, "ffffff", "By")} {TextEncodeing(14, "3296fa", $"{TopPlayerStatistics.HighestKDR_playerName}")}";

                if (i == "playtime") return $"{TextEncodeing(14, "ffffff", "Most Time Played")} {TextEncodeing(14, "3296fa", $"{TopPlayerStatistics.MostTimePlayed}h")} {TextEncodeing(14, "ffffff", "By")} {TextEncodeing(14, "3296fa", $"{TopPlayerStatistics.MostTimePlayed_playerName}")}";

                if (i == "distance") return $"{TextEncodeing(14, "ffffff", "Most Distance Traveled")} {TextEncodeing(14, "3296fa", $"{TopPlayerStatistics.MostDistanceTraveled}m")} {TextEncodeing(14, "ffffff", "By")} {TextEncodeing(14, "3296fa", $"{TopPlayerStatistics.MostDistanceTraveled_playerName}")}";

                if (i == "bradleykills") return $"{TextEncodeing(14, "ffffff", "Most Bradley Takedowns")} {TextEncodeing(14, "3296fa", $"{TopPlayerStatistics.MostBradleyTakedowns}")} {TextEncodeing(14, "ffffff", "By")} {TextEncodeing(14, "3296fa", $"{TopPlayerStatistics.MostBradleyTakedowns_playerName}")}";

                if (i == "animalkills") return $"{TextEncodeing(14, "ffffff", "Most Animals Killed")} {TextEncodeing(14, "3296fa", $"{TopPlayerStatistics.MostAnimalKills}")} {TextEncodeing(14, "ffffff", "By")} {TextEncodeing(14, "3296fa", $"{TopPlayerStatistics.MostAnimalKills_playerName}")}";

                if (i == "lootstolen") return $"{TextEncodeing(14, "ffffff", "Most Loot Stolen")} {TextEncodeing(14, "3296fa", $"{TopPlayerStatistics.MostLootStolen}")} {TextEncodeing(14, "ffffff", "By")} {TextEncodeing(14, "3296fa", $"{TopPlayerStatistics.MostLootStolen_playerName}")}";

            }


            return "";
            #endregion
        }
        #endregion

        //END OF COMMANDS



        #region Hooks Events

        private void OnPlayerConnected(BasePlayer player)
        {
            PlayerStatistics.LoadStatistics(player.userID);

            if (cachedPlayerStatistics.ContainsKey(player.userID))
            {
                //Save PlayerName In Data
                cachedPlayerStatistics[player.userID].PlayerName = player.displayName;

                cachedPlayerStatistics[player.userID].PlayerConnectedAt = DateTime.Now;

                SaveSpecficPlyerData(player.userID);
            }
        }
        private void OnServerShutdown() => SaveAllPlyerData();



        private void OnPlayerDeath(BasePlayer victim, HitInfo info)
        {
            try
            {
                var killer = info.InitiatorPlayer;

                if (killer != null && victim != killer && !killer.IsNpc && info != null)
                {
                    if (cachedPlayerStatistics.ContainsKey(killer.userID) && !victim.IsNpc) cachedPlayerStatistics[killer.userID].Kills++;
                    if (cachedPlayerStatistics.ContainsKey(victim.userID)) cachedPlayerStatistics[victim.userID].Deaths++;

                    if (cachedPlayerStatistics.ContainsKey(killer.userID))
                    {
                        if (info.isHeadshot)
                        {
                            if (info.ProjectileDistance > cachedPlayerStatistics[killer.userID].LongestHeadshotKill)
                            {
                                cachedPlayerStatistics[killer.userID].LongestHeadshotKill = ((int)info.ProjectileDistance);
                            }

                            if (cachedPlayerStatistics[killer.userID].LongestHeadshotKill > cachedPlayerStatistics[killer.userID].LongestKill)
                            {
                                cachedPlayerStatistics[killer.userID].LongestKill = ((int)cachedPlayerStatistics[killer.userID].LongestHeadshotKill);

                            }
                        }
                        else if (info.ProjectileDistance > cachedPlayerStatistics[killer.userID].LongestKill)
                        {
                            cachedPlayerStatistics[killer.userID].LongestKill = ((int)info.ProjectileDistance);
                        }
                    }
                }

            } catch { }

        }

        private void OnWeaponFired(BaseProjectile projectile, BasePlayer player, ItemModProjectile mod, ProtoBuf.ProjectileShoot projectiles)
        {
            var AmmoType = mod.ammoType;

            if (AmmoType == Rust.AmmoTypes.SHOTGUN_12GUAGE || AmmoType == Rust.AmmoTypes.RIFLE_556MM || AmmoType == Rust.AmmoTypes.PISTOL_9MM || AmmoType == Rust.AmmoTypes.HANDMADE_SHELL)
            {
                if (cachedPlayerStatistics.ContainsKey(player.userID)) cachedPlayerStatistics[player.userID].ShortsFired++;
            }
        }

        private void OnRocketLaunched(BasePlayer player, BaseEntity entity)
        {
            if (cachedPlayerStatistics.ContainsKey(player.userID)) cachedPlayerStatistics[player.userID].RocketsFired++;
        }

        private void OnPlayerChat(BasePlayer player, string message, Chat.ChatChannel channel)
        {
            if (cachedPlayerStatistics.ContainsKey(player.userID)) cachedPlayerStatistics[player.userID].ChatMessages++;
        }

        private void OnEntityTakeDamage(BaseCombatEntity entity, HitInfo info)
        {
            try
            {
                if (entity != null)
                {
                    if (entity is BasePlayer || entity is BaseAnimalNPC)
                    {
                        if (info.DidHit)
                        {
                            if (cachedPlayerStatistics.ContainsKey(info.InitiatorPlayer.userID)) cachedPlayerStatistics[info.InitiatorPlayer.userID].ShortsHit++;
                        }
                        if (info.isHeadshot)
                        {

                            if (cachedPlayerStatistics.ContainsKey(info.InitiatorPlayer.userID)) cachedPlayerStatistics[info.InitiatorPlayer.userID].HeadShotsHit++;

                        }
                    }
                }
            }
            catch { }

        }


        void OnLootEntityEnd(BasePlayer player, BaseCombatEntity entity)
        {
            if (entity != null)
            {
                if (entity is SupplyDrop)
                {
                    var x = entity as LootContainer;
                    if (x.inventory.IsEmpty())
                    {
                        if (cachedPlayerStatistics.ContainsKey(player.userID)) cachedPlayerStatistics[player.userID].AirdropsLooted++;
                    }
                }
            }
        }


        private void OnEntityDeath(BaseCombatEntity entity, HitInfo info)
        {

            try
            {

                var ID = Convert.ToUInt64(GetIDfromName(entity.lastAttacker.name));


                if (entity is BradleyAPC)
                {
                    if (cachedPlayerStatistics.ContainsKey(ID)) cachedPlayerStatistics[ID].BradleyKills++;
                }


                if (entity is BuildingBlock && entity != null)
                {
                    if (cachedPlayerStatistics.ContainsKey(ID))
                    {
                        cachedPlayerStatistics[ID].FoundationRemoved++;
                    }




                }


                //Npc Killed
                //ScientistNPC = Heavy
                //ScientistNPCNew = Normal
                if (entity is ScientistNPC || entity is ScientistNPCNew || entity is NPCMurderer || entity.ShortPrefabName == "scarecrow" && entity != null)
                {
                    if (cachedPlayerStatistics.ContainsKey(ID)) cachedPlayerStatistics[ID].NpcKills++;
                }


                if (entity is BaseAnimalNPC && entity != null)
                {
                    if (cachedPlayerStatistics.ContainsKey(ID)) cachedPlayerStatistics[ID].AnimalKills++;
                }
            } catch { }





        }

        private void OnItemResearch(ResearchTable table, Item targetItem, BasePlayer player)
        {
            if (cachedPlayerStatistics.ContainsKey(player.userID)) cachedPlayerStatistics[player.userID].ItemsResearch++;
        }

        private void OnPlayerRevive(BasePlayer reviver, BasePlayer player)
        {
            if (cachedPlayerStatistics.ContainsKey(reviver.userID)) cachedPlayerStatistics[reviver.userID].PlayersRevive++;
        }

        private void OnPlayerKeepAlive(BasePlayer player, BasePlayer target)
        {
            if (cachedPlayerStatistics.ContainsKey(target.userID)) cachedPlayerStatistics[target.userID].PlayersRevive++;
        }

        private void OnPlayerRespawn(BasePlayer player)
        {
            if (cachedPlayerStatistics.ContainsKey(player.userID)) cachedPlayerStatistics[player.userID].Respawns++;
        }

        private void OnExplosiveThrown(BasePlayer player, BaseEntity entity, ThrownWeapon item)
        {
            if (item.name.Contains("explosive.timed.entity"))
            {
                if (cachedPlayerStatistics.ContainsKey(player.userID)) cachedPlayerStatistics[player.userID].C4used++;
            }
        }



        #region OnItem recycle
        static Dictionary<int, recyclerInfomation> recyclerInfo = new Dictionary<int, recyclerInfomation>();

        private class recyclerInfomation
        {
            public int recyclerInstanceID = 0;
            public ulong playerID = 0;
        }




        private void OnRecycleItem(Recycler recycler, Item item)
        {
   

            var recyclerID = recycler.GetInstanceID();

            if (recyclerInfo.ContainsKey(recyclerID))
            {

                if (recyclerID == recyclerInfo[recyclerID].recyclerInstanceID)
                {
                    //Recycle Count
                    if (cachedPlayerStatistics.ContainsKey(recyclerInfo[recyclerID].playerID)) cachedPlayerStatistics[recyclerInfo[recyclerID].playerID].ItemsRecycled++;
                }

            }
            else
            {

                if (recyclerInfo.ContainsKey(recyclerID)) return;

                recyclerInfo.Add(recyclerID, new recyclerInfomation());

                //Recycle Count
                if (cachedPlayerStatistics.ContainsKey(recyclerInfo[recyclerID].playerID)) cachedPlayerStatistics[recyclerInfo[recyclerID].playerID].ItemsRecycled++;
            }



        }
    
        private void OnRecyclerToggle(Recycler recycler, BasePlayer player)
        {

            var recyclerID = recycler.GetInstanceID();
            if (recyclerInfo.ContainsKey(recyclerID))
            {
                recyclerInfo[recyclerID].recyclerInstanceID = recycler.GetInstanceID();
                recyclerInfo[recyclerID].playerID = player.userID;
            }
        }


        #endregion


        private void OnItemCraftFinished(ItemCraftTask task, Item item)
        {
            if (cachedPlayerStatistics.ContainsKey(task.owner.userID)) cachedPlayerStatistics[task.owner.userID].ItemsCrafted++;
        }

        private void OnStructureUpgrade(BaseCombatEntity entity, BasePlayer player, BuildingGrade.Enum grade)
        {
            if (cachedPlayerStatistics.ContainsKey(player.userID)) cachedPlayerStatistics[player.userID].FoundationUpgrade++; 
        }

        private void OnConstructionPlace(BaseEntity entity, Construction component, Construction.Target constructionTarget, BasePlayer player)
        {
            if (cachedPlayerStatistics.ContainsKey(player.userID)) cachedPlayerStatistics[player.userID].FoundationBuild++;
        }

        private void OnBuyVendingItem(VendingMachine machine, BasePlayer player, int sellOrderId, int numberOfTransactions)
        {
            if (cachedPlayerStatistics.ContainsKey(player.userID)) cachedPlayerStatistics[player.userID].VendingMachineTrades++;
        }

        private void OnLootEntity(BasePlayer player, BaseEntity entity)
        {
            if (entity is BasePlayer && entity as BasePlayer != player && entity.OwnerID != player.userID)
            {
                if (cachedPlayerStatistics.ContainsKey(player.userID)) cachedPlayerStatistics[player.userID].LootItemsStolen++;
            }
            if (entity is LootContainer && entity.OwnerID != player.userID)
            {
                if (cachedPlayerStatistics.ContainsKey(player.userID)) cachedPlayerStatistics[player.userID].LootItemsStolen++;
            }
        }

        //Notes Wood
        private void OnDispenserGather(ResourceDispenser dispenser, BaseEntity entity, Item item)
        {


            if (entity != null && entity.ShortPrefabName != null && item != null && item.info.shortname != null && item.amount > 0)
            {
                var player = entity as BasePlayer;


                if (item.info.shortname == "wood")
                {
                    if (cachedPlayerStatistics.ContainsKey(player.userID)) cachedPlayerStatistics[player.userID].WoodGathered += item.amount;

                }

                if (item.info.shortname == "stones")
                {
                    if (cachedPlayerStatistics.ContainsKey(player.userID)) cachedPlayerStatistics[player.userID].StoneGathered += item.amount;
                }
                if (item.info.shortname == "metal.ore")
                {
                    if (cachedPlayerStatistics.ContainsKey(player.userID)) cachedPlayerStatistics[player.userID].MetalOreGathered += item.amount;
                }
                if (item.info.shortname == "sulfur.ore")
                {
                    if (cachedPlayerStatistics.ContainsKey(player.userID)) cachedPlayerStatistics[player.userID].SulfurOreGathered += item.amount;
                }



            }


        }

        //Bonus
        private void OnDispenserBonus(ResourceDispenser dispenser, BasePlayer player, Item item)
        {
            if (item != null && item.info.shortname != null && item.amount > 0)
            {

                if (item.info.shortname == "wood")
                {
                    if (cachedPlayerStatistics.ContainsKey(player.userID)) cachedPlayerStatistics[player.userID].WoodGathered += item.amount;

                }
                if (item.info.shortname == "stones")
                {
                    if (cachedPlayerStatistics.ContainsKey(player.userID)) cachedPlayerStatistics[player.userID].StoneGathered += item.amount;
                }
                if (item.info.shortname == "metal.ore")
                {
                    if (cachedPlayerStatistics.ContainsKey(player.userID)) cachedPlayerStatistics[player.userID].MetalOreGathered += item.amount;
                }
                if (item.info.shortname == "sulfur.ore")
                {
                    if (cachedPlayerStatistics.ContainsKey(player.userID)) cachedPlayerStatistics[player.userID].SulfurOreGathered += item.amount;
                }

                if (item.info.shortname == "hq.metal.ore")
                {
                    if (cachedPlayerStatistics.ContainsKey(player.userID)) cachedPlayerStatistics[player.userID].HQMOreGathered += item.amount;
                }




            }
        }

        //Spawn Ore Spwan Plants
        private void OnCollectiblePickup(Item item, BasePlayer player, CollectibleEntity entity)
        {
            if (entity != null && entity.ShortPrefabName != null && item != null && item.info.shortname != null && item.amount > 0)
            {

                if (item.info.shortname == "wood")
                {
                    if (cachedPlayerStatistics.ContainsKey(player.userID)) cachedPlayerStatistics[player.userID].WoodGathered += item.amount;
                }
                if (item.info.shortname == "stones")
                {
                    if (cachedPlayerStatistics.ContainsKey(player.userID)) cachedPlayerStatistics[player.userID].StoneGathered += item.amount;
                }
                if (item.info.shortname == "metal.ore")
                {
                    if (cachedPlayerStatistics.ContainsKey(player.userID)) cachedPlayerStatistics[player.userID].MetalOreGathered += item.amount;
                }
                if (item.info.shortname == "sulfur.ore")
                {
                    if (cachedPlayerStatistics.ContainsKey(player.userID)) cachedPlayerStatistics[player.userID].SulfurOreGathered += item.amount;
                }






            }






        }


        #endregion





        #region Unload Save Hooks

        void OnServerSave()
        {
            SaveAllPlyerData();
        }
        void OnPlayerDisconnected(BasePlayer player, string reason)
        {
            //Save PlayerName In Data
            cachedPlayerStatistics[player.userID].PlayerName = player.displayName;


            SaveSpecficPlyerData(player.userID);
            cachedPlayerStatistics.Remove(player.userID);

        }



        #endregion

    }
}