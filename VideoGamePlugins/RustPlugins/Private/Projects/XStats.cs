using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Oxide.Core;
using Oxide.Game.Rust.Cui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/*
TO DO
KILL - KD Not working
WEAPON - Accuracy & Head Accuracy
ROUND NUMBERS
*/


namespace Oxide.Plugins
{
    [Info("XStats", "Infinitynet.dk", "1.0.0")]
    [Description("Copyright Infinitynet.dk All Rights Reserved")]
    public class XStats : RustPlugin
    {
        private const int UpdateDelaySeconds = 1;
        public const int TableLimit = 16;

        /*UI Colors*/
        public const string BlueColor = "0 0.5 0.75 0.75";
        public const string BlurColor = "0 0.1 0.1 0.7";
        public const string TransparentColor = "0 0 0 0";
        public const string BlurColorMat = "assets/content/ui/uibackgroundblur.mat";

        public static ulong[] StaffID = new ulong[] { 76561198066857380, 76561198023076226 };
        public const ulong IconID = 76561198389709969;
        public const string Perm = "XStats.admin";

        public static string[] TriggerCommands = new string[] { "stat", "stats", "statistik" };


        Timer tempTimer;
        public static XStats Instance { get; set; }
        #region Configuration


        public static Achievements achievements = new Achievements();
        public class Achievements
        {
            public Resources resources = new Resources();
            public Raiding raiding = new Raiding();
            public Kill kill = new Kill();
            public Weapon weapon = new Weapon();
            public Items items = new Items();
            public Events events = new Events();
            public Travel travel = new Travel();
            public Player player = new Player();
            public Structures structures = new Structures();

            public class Resources
            {
                public readonly int Total = 100000;
                public readonly int Skulls = 100;
            }
            public class Raiding
            {
                public int TotalUsed = 100;
            }
            public class Kill
            {
                public int Longest = 300;
                public int LongestHead = 300;
                public int Kills = 50;
                public int KillsHead = 50;
                public int KillsNpc = 100;
                public int KillsAnimal = 200;
            }
            public class Weapon
            {
                public int ShotsFired = 10000;
                public int ShotsHit = 1000;
                public int ShotsHitHead = 200;
            }
            public class Items
            {
                public int Crafted = 200;
                public int Research = 100;
                public int Recycled = 500;
                public int Found = 500;
                public int StoleFromPlayers = 100;
            }
            public class Events
            {
                public int PetrolHeliKills = 10;
                public int BradleyKills = 20;
                public int Airdrops = 50;
            }
            public class Travel
            {
                public int Total = 100000;
                public int Walking = 10000;
                public int Sailing = 10000;
                public int Driving = 10000;
                public int Flying = 10000;
            }
            public class Player
            {
                public int Revives = 20;
            }
            public class Structures
            {
                public int Upgraded = 100;
                public int DemolishedRaided = 50;
                public int Built = 200;
                public int Demolished = 100;
            }
        }


        public static class ShortNames
        {
            public class Resources
            {
                public const string AnimalFat = "fat.animal";
                public const string BoneFragments = "bone.fragments";
                public const string Charcoal = "charcoal";
                public const string Cloth = "cloth";
                public const string CrudeOil = "crude.oil";
                public const string DieselBarrel = "diesel_barrel";
                public const string Explosives = "explosives";
                public const string Fertilizer = "fertilizer";
                public const string GunPowder = "gunpowder";
                public const string HighQualityMetal = "metal.refined";
                public const string HighQualityMetalOre = "hq.metal.ore";
                public const string HorseDung = "horsedung";
                public const string HumanSkull = "skull.human";
                public const string Leather = "leather";
                public const string LowGradeFuel = "lowgradefuel";
                public const string MetalFragments = "metal.fragments";
                public const string MetalOre = "metal.ore";
                public const string Scrap = "scrap";
                public const string Stone = "stones";
                public const string Sulfur = "sulfur";
                public const string SulfurOre = "sulfur.ore";
                public const string Wood = "wood";
            }
        }
        #endregion Configuration

        public static bool IsStaff(BasePlayer player)
        {
            if (StaffID.Contains(player.userID) && Instance.permission.UserHasPermission(player.UserIDString, Perm)) { return true; }
            return false;
        }

        #region Init
        void OnServerInitialized(bool initial)
        {
            timer.Every(UpdateDelaySeconds, () =>
            {
                if (BasePlayer.activePlayerList != null)
                {
                    foreach (BasePlayer player in BasePlayer.activePlayerList)
                    {
                        UpdateDistance(player);
                        UpdatePlaytime(player);
                        UpdateAchievements(player);
                    }
                }
            });
        }

        void Loaded()
        {
            Instance = this;
            permission.RegisterPermission(Perm, this);
            Interface.Oxide.DataFileSystem.WriteObject(($"XSTATS/ACHIEVEMENTS"), achievements, true);
            foreach(var command in TriggerCommands) { cmd.AddChatCommand(command, this, CommandShowStatsUI); }
            foreach (BasePlayer player in BasePlayer.activePlayerList.Where(x => !PlayerStats.ContainsKey(x.userID)))
            {
                LoadData(player.userID);
            }
        }
        void Unload()
        {
            SaveData();
            try { tempTimer.Destroy(); }
            catch { }
        }
        void OnServerSave()
        {
            SaveData();
        }


        void OnPlayerConnected(BasePlayer player)
        {
            LoadData(player.userID);
        }

        void OnPlayerDisconnected(BasePlayer player, string reason)
        {
            SaveData(player.userID);
            PlayerStats.Remove(player.userID);
        }
        #endregion Init

        #region Stats Management
        private readonly Dictionary<ulong, StatsData> PlayerStats = new Dictionary<ulong, StatsData>();
        public class StatsData
        {
            public Achievements achievements { get; set; }
            public class Achievements
            {

                public Resources resources { get; set; }
                public Raiding raiding { get; set; }
                public Kill kill { get; set; }
                public Weapon weapon { get; set; }
                public Items items { get; set; }
                public Events events { get; set; }
                public Travel travel { get; set; }
                public Player player { get; set; }
                public Structures structures { get; set; }

                public class Resources
                {
                    public bool Total { get; set; } = false;
                    public bool Skulls { get; set; } = false;
                }
                public class Raiding
                {
                    public bool TotalUsed { get; set; } = false;
                }
                public class Kill
                {
                    public bool Longest { get; set; } = false;
                    public bool LongestHead { get; set; } = false;
                    public bool Kills { get; set; } = false;
                    public bool KillsHead { get; set; } = false;
                    public bool KillsNpc { get; set; } = false;
                    public bool KillsAnimal { get; set; } = false;
                }
                public class Weapon
                {
                    public bool ShotsFired { get; set; } = false;
                    public bool ShotsHit { get; set; } = false;
                    public bool ShotsHitHead { get; set; } = false;
                }
                public class Items
                {
                    public bool Crafted { get; set; } = false;
                    public bool Research { get; set; } = false;
                    public bool Recycled { get; set; } = false;
                    public bool Found { get; set; } = false;
                    public bool StoleFromPlayers { get; set; } = false;
                }
                public class Events
                {
                    public bool PetrolHeliKills { get; set; } = false;
                    public bool BradleyKills { get; set; } = false;
                    public bool Airdrops { get; set; } = false;
                }
                public class Travel
                {
                    public bool Total { get; set; } = false;
                    public bool Walking { get; set; } = false;
                    public bool Sailing { get; set; } = false;
                    public bool Driving { get; set; } = false;
                    public bool Flying { get; set; } = false;
                }
                public class Player
                {
                    public bool Revives { get; set; } = false;
                }
                public class Structures
                {
                    public bool Upgraded { get; set; } = false;
                    public bool DemolishedRaided { get; set; } = false;
                    public bool Built { get; set; } = false;
                    public bool Demolished { get; set; } = false;
                }
            }

            public Stats stats { get; set; }
            public class Stats
            {
                public Resources resources { get; set; }
                public Raiding raiding { get; set; }
                public Kill kill { get; set; }
                public Weapon weapon { get; set; }
                public Items items { get; set; }
                public Events events { get; set; }
                public Travel travel { get; set; }
                public Player player { get; set; }
                public Structures structures { get; set; }


                public class Resources
                {
                    public int AnimalFat { get; set; }
                    public int BoneFragments { get; set; }
                    public int CharCoal { get; set; }
                    public int Cloth { get; set; }
                    public int CrudeOil { get; set; }
                    public int DieselBarrel { get; set; }
                    public int Explosives { get; set; }
                    public int Fertilizer { get; set; }
                    public int GunPowder { get; set; }
                    public int HorseDung { get; set; }
                    public int HighQualityMetal { get; set; }
                    public int HumanSkulls { get; set; }
                    public int Leather { get; set; }
                    public int Lowgrade { get; set; }
                    public int Metal { get; set; }
                    public int Scrap { get; set; }
                    public int Stone { get; set; }
                    public int Sulfur { get; set; }
                    public int Wood { get; set; }
                }
                public class Raiding
                {
                    public int RocketsFired { get; set; }
                    public int C4Used { get; set; }
                    public int SatchelChargeUsed { get; set; }
                }
                public class Kill
                {
                    public float KD { get; set; }
                    public float LongestKill { get; set; }
                    public float LongestHeadshotKill { get; set; }
                    public int Kills { get; set; }
                    public int HeadKills { get; set; }
                    public int Deaths { get; set; }
                    public int NpcKills { get; set; }
                    public int AnimalKills { get; set; }
                }
                public class Weapon
                {
                    public float HitAccuracy { get; set; }
                    public int HeadShotsHitAccuracy { get; set; }
                    public int ShotsFired { get; set; }
                    public int ShotsHit { get; set; }
                    public int HeadShotsHit { get; set; }
                }
                public class Items
                {
                    public int Crafted { get; set; }
                    public int Research { get; set; }
                    public int Recycled { get; set; }
                    public int Found { get; set; }
                    public int StoleFromPlayers { get; set; }
                }
                public class Events
                {
                    public int PetrolHeliKills { get; set; }
                    public int BradleyKills { get; set; }
                    public int Airdrops { get; set; }
                }
                public class Travel
                {
                    public float Walking { get; set; }
                    public float Sailing { get; set; }
                    public float Driving { get; set; }
                    public float Flying { get; set; }
                }
                public class Player
                {
                    public double PlaytimeHours { get; set; }
                    public int Revives { get; set; }
                }
                public class Structures
                {
                    public int Upgraded { get; set; }
                    public int DemolishedRaided { get; set; }
                    public int Built { get; set; }
                    public int Demolished { get; set; }
                }
            }
        }

        private void CreateStats(ulong id)
        {
            if (!PlayerStats.ContainsKey(id))
            {
                PlayerStats.Add(id, DefaultStats());
            }
        }
        private StatsData DefaultStats()
        {
            return new StatsData
            {
                achievements = new StatsData.Achievements
                {
                    events = new StatsData.Achievements.Events
                    {
                        Airdrops = false,
                        BradleyKills = false,
                        PetrolHeliKills = false,
                    },
                    items = new StatsData.Achievements.Items
                    {
                        Crafted = false,
                        Found = false,
                        Recycled = false,
                        Research = false,
                        StoleFromPlayers = false,
                    },
                    kill = new StatsData.Achievements.Kill
                    {
                        KillsAnimal = false,
                        Kills = false,
                        KillsHead = false,
                        KillsNpc = false,
                        Longest = false,
                        LongestHead = false,
                    },
                    player = new StatsData.Achievements.Player
                    {
                        Revives = false,
                    },
                    raiding = new StatsData.Achievements.Raiding
                    {
                        TotalUsed = false,
                    },
                    resources = new StatsData.Achievements.Resources
                    {
                        Skulls = false,
                        Total = false,
                    },
                    structures = new StatsData.Achievements.Structures
                    {
                        Built = false,
                        Demolished = false,
                        DemolishedRaided = false,
                        Upgraded = false,
                    },
                    travel = new StatsData.Achievements.Travel
                    {
                        Driving = false,
                        Flying = false,
                        Sailing = false,
                        Total = false,
                        Walking = false,
                    },
                    weapon = new StatsData.Achievements.Weapon
                    {
                        ShotsFired = false,
                        ShotsHit = false,
                        ShotsHitHead = false,
                    }
                },
                stats = new StatsData.Stats
                {
                    events = new StatsData.Stats.Events
                    {
                        Airdrops = 0,
                        BradleyKills = 0,
                        PetrolHeliKills = 0
                    },
                    items = new StatsData.Stats.Items
                    {
                        Crafted = 0,
                        Found = 0,
                        Recycled = 0,
                        Research = 0,
                        StoleFromPlayers = 0,
                    },
                    kill = new StatsData.Stats.Kill
                    {
                        AnimalKills = 0,
                        Deaths = 0,
                        HeadKills = 0,
                        KD = 0,
                        Kills = 0,
                        LongestHeadshotKill = 0,
                        LongestKill = 0,
                        NpcKills = 0,
                    },
                    player = new StatsData.Stats.Player
                    {
                        Revives = 0,
                    },
                    raiding = new StatsData.Stats.Raiding
                    {
                        C4Used = 0,
                        RocketsFired = 0,
                        SatchelChargeUsed = 0,
                    },
                    resources = new StatsData.Stats.Resources
                    {
                        AnimalFat = 0,
                        BoneFragments = 0,
                        CharCoal = 0,
                        Cloth = 0,
                        CrudeOil = 0,
                        DieselBarrel = 0,
                        Explosives = 0,
                        Fertilizer = 0,
                        GunPowder = 0,
                        HighQualityMetal = 0,
                        HorseDung = 0,
                        HumanSkulls = 0,
                        Leather = 0,
                        Lowgrade = 0,
                        Metal = 0,
                        Scrap = 0,
                        Stone = 0,
                        Sulfur = 0,
                        Wood = 0,
                    },
                    structures = new StatsData.Stats.Structures
                    {
                        Built = 0,
                        Demolished = 0,
                        DemolishedRaided = 0,
                        Upgraded = 0,
                    },
                    travel = new StatsData.Stats.Travel
                    {
                        Driving = 0,
                        Flying = 0,
                        Sailing = 0,
                        Walking = 0,
                    },
                    weapon = new StatsData.Stats.Weapon
                    {
                        HitAccuracy = 0,
                        HeadShotsHitAccuracy = 0,
                        HeadShotsHit = 0,
                        ShotsFired = 0,
                        ShotsHit = 0,
                    }
                }

            };
        }



        private void LoadData(ulong id)
        {
            if (id > 0 && id.IsSteamId())
            {
                if (PlayerStats.ContainsKey(id)) { SaveData(id); }
                if (!Interface.Oxide.DataFileSystem.ExistsDatafile($"XSTATS/USER/{id}")) { return; }
                StatsData data = Interface.Oxide.DataFileSystem.ReadObject<StatsData>($"XSTATS/USER/{id}");
                if (data == null) { PrintWarning($"CORRUPT DATA FILE ({id})"); return; }
                if(!PlayerStats.ContainsKey(id)) { PlayerStats.Add(id, data); }
            }
        }
        private void SaveData(ulong id = 0)
        {
            if (id > 0 && id.IsSteamId())
            {
                Interface.Oxide.DataFileSystem.WriteObject(($"XSTATS/USER/{id}"), PlayerStats[id], true);
            }
            else
            {
                foreach (var user in PlayerStats)
                {
                    Interface.Oxide.DataFileSystem.WriteObject(($"XSTATS/USER/{user.Key}"), user.Value, true);
                }
            }
        }
        #endregion Stats Management

        #region Helpers
        private void OnResourcesGather(BasePlayer player, Item item)
        {
            if (!player.userID.IsSteamId()) { return; }
            CreateStats(player.userID);
            switch (item.info.shortname)
            {
                case ShortNames.Resources.AnimalFat: PlayerStats[player.userID].stats.resources.AnimalFat += item.amount; break;
                case ShortNames.Resources.BoneFragments: PlayerStats[player.userID].stats.resources.BoneFragments += item.amount; break;
                case ShortNames.Resources.Charcoal: PlayerStats[player.userID].stats.resources.CharCoal += item.amount; break;
                case ShortNames.Resources.Cloth: PlayerStats[player.userID].stats.resources.Cloth += item.amount; break;
                case ShortNames.Resources.CrudeOil: PlayerStats[player.userID].stats.resources.CrudeOil += item.amount; break;
                case ShortNames.Resources.DieselBarrel: PlayerStats[player.userID].stats.resources.DieselBarrel += item.amount; break;
                case ShortNames.Resources.Explosives: PlayerStats[player.userID].stats.resources.Explosives += item.amount; break;
                case ShortNames.Resources.Fertilizer: PlayerStats[player.userID].stats.resources.Fertilizer += item.amount; break;
                case ShortNames.Resources.GunPowder: PlayerStats[player.userID].stats.resources.GunPowder += item.amount; break;
                case ShortNames.Resources.HighQualityMetal: PlayerStats[player.userID].stats.resources.HighQualityMetal += item.amount; break;
                case ShortNames.Resources.HighQualityMetalOre: PlayerStats[player.userID].stats.resources.HighQualityMetal += item.amount; break;
                case ShortNames.Resources.HorseDung: PlayerStats[player.userID].stats.resources.HorseDung += item.amount; break;
                case ShortNames.Resources.HumanSkull: PlayerStats[player.userID].stats.resources.HumanSkulls += item.amount; break;
                case ShortNames.Resources.Leather: PlayerStats[player.userID].stats.resources.Leather += item.amount; break;
                case ShortNames.Resources.LowGradeFuel: PlayerStats[player.userID].stats.resources.Lowgrade += item.amount; break;
                case ShortNames.Resources.MetalFragments: PlayerStats[player.userID].stats.resources.Metal += item.amount; break;
                case ShortNames.Resources.MetalOre: PlayerStats[player.userID].stats.resources.Metal += item.amount; break;
                case ShortNames.Resources.Scrap: PlayerStats[player.userID].stats.resources.Scrap += item.amount; break;
                case ShortNames.Resources.Stone: PlayerStats[player.userID].stats.resources.Stone += item.amount; break;
                case ShortNames.Resources.Sulfur: PlayerStats[player.userID].stats.resources.Sulfur += item.amount; break;
                case ShortNames.Resources.SulfurOre: PlayerStats[player.userID].stats.resources.Sulfur += item.amount; break;
                case ShortNames.Resources.Wood: PlayerStats[player.userID].stats.resources.Wood += item.amount; break;
            }
        }


        private enum SortType 
        {
            ascending,
            descending
        }
        private enum OrderType
        {
            key,
            value
        }
        private Dictionary<string, double> SortTable(Dictionary<string, double> table, SortType sortType, OrderType orderType)
        {
            switch (sortType)
            {
                case SortType.ascending:
                    if (orderType == OrderType.key) { return table.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value); }
                    if (orderType == OrderType.value) { return table.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value); }
                    break;
                case SortType.descending:
                    if (orderType == OrderType.key) { return table.OrderByDescending(x => x.Key).ToDictionary(x => x.Key, x => x.Value); }
                    if (orderType == OrderType.value) { return table.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value); }
                    break;
            }
            return null;
        }
        private Dictionary<string, double> ProcessTable(object table)
        {
            Dictionary<string, double> listResult = new Dictionary<string, double>();

            for (int i = 0; i < table.GetType().GetProperties().ToArray().Count(); i++)
            {
                try
                {
                    var index = table.GetType().GetProperties().ToArray()[i];
                    var data = (JObject)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(table));
                    var value = Math.Round((float)data[index.Name], 1);
                    listResult.Add(index.Name, value);
                }
                catch { }
            }
            return listResult;
        }


        private void UpdateAchievements(BasePlayer player)
        {

            if (!player.userID.IsSteamId()) { return; }
            CreateStats(player.userID);

            int resourcesTotal = 0;
            foreach (PropertyInfo p in PlayerStats[player.userID].stats.resources.GetType().GetProperties())
            {
                var data = (JObject)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(PlayerStats[player.userID].stats.resources));
                resourcesTotal += ((int)data[p.Name]);
            }

            int raidingTotal = 0;
            foreach (PropertyInfo p in PlayerStats[player.userID].stats.raiding.GetType().GetProperties())
            {
                var data = (JObject)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(PlayerStats[player.userID].stats.raiding));
                raidingTotal += ((int)data[p.Name]);
            }

            int travelTotal = 0;
            foreach (PropertyInfo p in PlayerStats[player.userID].stats.travel.GetType().GetProperties())
            {
                var data = (JObject)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(PlayerStats[player.userID].stats.travel));
                travelTotal += ((int)data[p.Name]);
            }


            #region Checks
            /*resources*/
            if (resourcesTotal > achievements.resources.Total) { PlayerStats[player.userID].achievements.resources.Total = true; }
            else { PlayerStats[player.userID].achievements.resources.Total = false; }

            if (PlayerStats[player.userID].stats.resources.HumanSkulls > achievements.resources.Skulls) { PlayerStats[player.userID].achievements.resources.Skulls = true; }
            else { PlayerStats[player.userID].achievements.resources.Skulls = false; }


            /*raiding*/
            if (raidingTotal > achievements.raiding.TotalUsed) { PlayerStats[player.userID].achievements.raiding.TotalUsed = true; }
            else { PlayerStats[player.userID].achievements.raiding.TotalUsed = false; }


            /*kill*/
            if (PlayerStats[player.userID].stats.kill.LongestKill > achievements.kill.Longest) { PlayerStats[player.userID].achievements.kill.Longest = true; }
            else { PlayerStats[player.userID].achievements.kill.Longest = false; }

            if (PlayerStats[player.userID].stats.kill.LongestHeadshotKill > achievements.kill.LongestHead) { PlayerStats[player.userID].achievements.kill.LongestHead = true; }
            else { PlayerStats[player.userID].achievements.kill.LongestHead = false; }

            if (PlayerStats[player.userID].stats.kill.Kills > achievements.kill.Kills) { PlayerStats[player.userID].achievements.kill.Kills = true; }
            else { PlayerStats[player.userID].achievements.kill.Kills = false; }

            if (PlayerStats[player.userID].stats.kill.HeadKills > achievements.kill.KillsHead) { PlayerStats[player.userID].achievements.kill.KillsHead = true; }
            else { PlayerStats[player.userID].achievements.kill.KillsHead = false; }

            if (PlayerStats[player.userID].stats.kill.NpcKills > achievements.kill.KillsHead) { PlayerStats[player.userID].achievements.kill.KillsNpc = true; }
            else { PlayerStats[player.userID].achievements.kill.KillsNpc = false; }

            if (PlayerStats[player.userID].stats.kill.AnimalKills > achievements.kill.KillsAnimal) { PlayerStats[player.userID].achievements.kill.KillsAnimal = true; }
            else { PlayerStats[player.userID].achievements.kill.KillsAnimal = false; }


            /*weapon*/
            if (PlayerStats[player.userID].stats.weapon.ShotsFired > achievements.weapon.ShotsFired) { PlayerStats[player.userID].achievements.weapon.ShotsFired = true; }
            else { PlayerStats[player.userID].achievements.weapon.ShotsFired = false; }

            if (PlayerStats[player.userID].stats.weapon.ShotsHit > achievements.weapon.ShotsHit) { PlayerStats[player.userID].achievements.weapon.ShotsHit = true; }
            else { PlayerStats[player.userID].achievements.weapon.ShotsHit = false; }

            if (PlayerStats[player.userID].stats.weapon.HeadShotsHit > achievements.weapon.ShotsHitHead) { PlayerStats[player.userID].achievements.weapon.ShotsHitHead = true; }
            else { PlayerStats[player.userID].achievements.weapon.ShotsHitHead = false; }


            /*items*/
            if (PlayerStats[player.userID].stats.items.Crafted > achievements.items.Crafted) { PlayerStats[player.userID].achievements.items.Crafted = true; }
            else { PlayerStats[player.userID].achievements.items.Crafted = false; }

            if (PlayerStats[player.userID].stats.items.Research > achievements.items.Research) { PlayerStats[player.userID].achievements.items.Research = true; }
            else { PlayerStats[player.userID].achievements.items.Research = false; }

            if (PlayerStats[player.userID].stats.items.Recycled > achievements.items.Recycled) { PlayerStats[player.userID].achievements.items.Recycled = true; }
            else { PlayerStats[player.userID].achievements.items.Recycled = false; }

            if (PlayerStats[player.userID].stats.items.Found > achievements.items.Found) { PlayerStats[player.userID].achievements.items.Found = true; }
            else { PlayerStats[player.userID].achievements.items.Found = false; }

            if (PlayerStats[player.userID].stats.items.StoleFromPlayers > achievements.items.StoleFromPlayers) { PlayerStats[player.userID].achievements.items.StoleFromPlayers = true; }
            else { PlayerStats[player.userID].achievements.items.StoleFromPlayers = false; }

            /*events*/
            if (PlayerStats[player.userID].stats.events.PetrolHeliKills > achievements.events.PetrolHeliKills) { PlayerStats[player.userID].achievements.events.PetrolHeliKills = true; }
            else { PlayerStats[player.userID].achievements.events.PetrolHeliKills = false; }

            if (PlayerStats[player.userID].stats.events.BradleyKills > achievements.events.BradleyKills) { PlayerStats[player.userID].achievements.events.BradleyKills = true; }
            else { PlayerStats[player.userID].achievements.events.BradleyKills = false; }

            if (PlayerStats[player.userID].stats.events.Airdrops > achievements.events.Airdrops) { PlayerStats[player.userID].achievements.events.Airdrops = true; }
            else { PlayerStats[player.userID].achievements.events.Airdrops = false; }


            /*travel*/
            if (travelTotal > achievements.travel.Total) { PlayerStats[player.userID].achievements.travel.Total = true; }
            else { PlayerStats[player.userID].achievements.travel.Total = false; }

            if (PlayerStats[player.userID].stats.travel.Walking > achievements.travel.Walking) { PlayerStats[player.userID].achievements.travel.Walking = true; }
            else { PlayerStats[player.userID].achievements.travel.Walking = false; }

            if (PlayerStats[player.userID].stats.travel.Sailing > achievements.travel.Sailing) { PlayerStats[player.userID].achievements.travel.Sailing = true; }
            else { PlayerStats[player.userID].achievements.travel.Sailing = false; }

            if (PlayerStats[player.userID].stats.travel.Driving > achievements.travel.Driving) { PlayerStats[player.userID].achievements.travel.Driving = true; }
            else { PlayerStats[player.userID].achievements.travel.Driving = false; }

            if (PlayerStats[player.userID].stats.travel.Flying > achievements.travel.Flying) { PlayerStats[player.userID].achievements.travel.Flying = true; }
            else { PlayerStats[player.userID].achievements.travel.Flying = false; }


            /*player*/
            if (PlayerStats[player.userID].stats.player.Revives > achievements.player.Revives) { PlayerStats[player.userID].achievements.player.Revives = true; }
            else { PlayerStats[player.userID].achievements.player.Revives = false; }


            /*structures*/
            if (PlayerStats[player.userID].stats.structures.Upgraded > achievements.structures.Upgraded) { PlayerStats[player.userID].achievements.structures.Upgraded = true; }
            else { PlayerStats[player.userID].achievements.structures.Upgraded = false; }

            if (PlayerStats[player.userID].stats.structures.DemolishedRaided > achievements.structures.DemolishedRaided) { PlayerStats[player.userID].achievements.structures.DemolishedRaided = true; }
            else { PlayerStats[player.userID].achievements.structures.DemolishedRaided = false; }

            if (PlayerStats[player.userID].stats.structures.Built > achievements.structures.Built) { PlayerStats[player.userID].achievements.structures.Built = true; }
            else { PlayerStats[player.userID].achievements.structures.Built = false; }

            if (PlayerStats[player.userID].stats.structures.Demolished > achievements.structures.Demolished) { PlayerStats[player.userID].achievements.structures.Demolished = true; }
            else { PlayerStats[player.userID].achievements.structures.Demolished = false; }
            #endregion Checks
        }

        private static double ValueTranslator(string key, double value)
        {
            switch (key)
            {
                case "Walking":
                case "Sailing":
                case "Driving":
                case "Flying": return value / 1000;
            }
            return value;
        }
        private static string LangTranslator(string key)
        {
            switch (key)
            {
                case "resources": return "Ressourcer";
                    case "AnimalFat": return "Dyre fedt";
                    case "BoneFragments": return "Knoglefragmenter";
                    case "CharCoal": return "Trækul";
                    case "Cloth": return "Stof";
                    case "CrudeOil": return "Råolie";
                    case "DieselBarrel": return "Diesel tønder";
                    case "Explosives": return "Sprængstoffer";
                    case "Fertilizer": return "Gødning";
                    case "GunPowder": return "Krudt";
                    case "HorseDung": return "Heste gødning";
                    case "HighQualityMetal": return "Kvalitets metal";
                    case "HumanSkulls": return "Kranier";
                    case "Leather": return "Læder";
                    case "Lowgrade": return "Brændstof";
                    case "Metal": return "Metalfragmenter";
                    case "Scrap": return "Skrot";
                    case "Stone": return "Sten";
                    case "Sulfur": return "Svovl";
                    case "Wood": return "Træ";

                case "raiding": return "Røverriger";
                    case "RocketsFired": return "Raketter affyret";
                    case "C4Used": return "C4 brugt";
                    case "SatchelChargeUsed": return "Hjemmelavet C4 brugt";

                case "kill": return "Drab";
                    case "KD": return "KD-forhold";
                    case "LongestKill": return "Længste drab";
                    case "Kills": return "Totalte drab";
                    case "HeadKills": return "Drab med hovedeskud";
                    case "Deaths": return "Gange døde";
                    case "NpcKills": return "Npc'er dræbt";
                    case "AnimalKills": return "Dyr dræbt";

                case "weapon": return "Våben";
                    case "HitAccuracy": return "Præcision";
                    case "HeadShotsHitAccuracy": return "Hovedeskud præcision";
                    case "ShotsFired": return "Skud affyret";
                    case "ShotsHit": return "Skud der ramte";
                    case "HeadShotsHit": return "Hovedeskud der ramte";

                case "items": return "Genstande";
                    case "Crafted": return "Genstande lavet";
                    case "Research": return "Genstande forsket";
                    case "Recycled": return "Genstande gendbrugt";
                    case "Found": return "Genstande fundet";
                    case "StoleFromPlayers": return "Genstande stjålet";

                case "events": return "Begivenheder";
                    case "PetrolHeliKills": return "Militær helikoptere nedlagt";
                    case "BradleyKills": return "Militær kampvogne nedlagt";
                    case "Airdrops": return "Forsyning kasser fundet";

                case "travel": return "Rejse";
                    case "Walking": return "Afstand gået (km)";
                    case "Sailing": return "Afstand sejlet (km)";
                    case "Driving": return "Afstand kørt (km)";
                    case "Flying": return "Afstand fløjet (km)";


                case "player": return "Karakter";
                    case "PlaytimeHours": return "Tid spillet (timer)";
                    case "Revives": return "Spillere reddet";

                case "structures": return "Bygninger";
                    case "Upgraded": return "Opgraderet";
                    case "DemolishedRaided": return "Ødelagt";
                    case "Built": return "Bygget";
                    case "Demolished": return "Nedrevet";
            }
            return "NaN";
        }
        #endregion Helpers

        #region Oxide Hooks

        /*Resources*/
        void OnDispenserBonus(ResourceDispenser dispenser, BasePlayer player, Item item) => OnResourcesGather(player, item);
        void OnDispenserGather(ResourceDispenser dispenser, BaseEntity entity, Item item) => OnResourcesGather((entity as BasePlayer), item);
        void OnCollectiblePickup(Item item, BasePlayer player, CollectibleEntity entity) => OnResourcesGather(player, item);


        /*Raiding*/
        void OnRocketLaunched(BasePlayer player, BaseEntity entity)
        {
            if (!player.userID.IsSteamId()) { return; }
            CreateStats(player.userID);
            PlayerStats[player.userID].stats.raiding.RocketsFired++;
        }
        void OnExplosiveThrown(BasePlayer player, BaseEntity entity, ThrownWeapon item)
        {
            if (!player.userID.IsSteamId()) { return; }
            CreateStats(player.userID);
            if (item.ShortPrefabName.Contains("explosive.timed")) { PlayerStats[player.userID].stats.raiding.C4Used++; }
            if (item.ShortPrefabName.Contains("explosive.satchel")) { PlayerStats[player.userID].stats.raiding.SatchelChargeUsed++; }
        }


        /*Killing*/
        void OnPlayerDeath(BasePlayer victim, HitInfo hitInfo)
        {

            try
            {
                if (victim.IsNpc && hitInfo != null)
                {
                    var attacker = hitInfo.InitiatorPlayer;
                    if (!attacker.userID.IsSteamId()) { return; }
                    CreateStats(attacker.userID);
                    PlayerStats[attacker.userID].stats.kill.NpcKills++;
                    return;
                }
                /*Downed*/
                if (hitInfo == null)
                {
                    var attacker = victim.lastAttacker.ToPlayer();
                    if (!attacker.userID.IsSteamId() || !victim.userID.IsSteamId()) { return; }
                    CreateStats(attacker.userID); CreateStats(victim.userID);
                    PlayerStats[attacker.userID].stats.kill.Kills++;
                    PlayerStats[victim.userID].stats.kill.Deaths++;
                    var distance = Vector3.Distance(attacker.transform.position, victim.transform.position);
                    if (distance > PlayerStats[attacker.userID].stats.kill.LongestKill) { PlayerStats[attacker.userID].stats.kill.LongestKill = distance; }
                    return;
                }

                if (hitInfo.isHeadshot)
                {
                    var attacker = hitInfo.InitiatorPlayer;
                    if (!attacker.userID.IsSteamId() || !victim.userID.IsSteamId()) { return; }
                    if (attacker.userID == victim.userID) { return; }
                    CreateStats(attacker.userID); CreateStats(victim.userID);
                    PlayerStats[attacker.userID].stats.kill.HeadKills++;
                    PlayerStats[victim.userID].stats.kill.Deaths++;
                    var distance = Vector3.Distance(attacker.transform.position, victim.transform.position);
                    if (distance > PlayerStats[attacker.userID].stats.kill.LongestHeadshotKill) { PlayerStats[attacker.userID].stats.kill.LongestHeadshotKill = distance; }
                    return;
                }

                if (!hitInfo.InitiatorPlayer.userID.IsSteamId() || !victim.userID.IsSteamId()) { return; }
                if (hitInfo.InitiatorPlayer.userID == victim.userID) { return; }
                CreateStats(hitInfo.InitiatorPlayer.userID); CreateStats(victim.userID);
                PlayerStats[hitInfo.InitiatorPlayer.userID].stats.kill.Kills++;
                PlayerStats[victim.userID].stats.kill.Deaths++;
                if (Vector3.Distance(hitInfo.InitiatorPlayer.transform.position, victim.transform.position) > PlayerStats[hitInfo.InitiatorPlayer.userID].stats.kill.LongestHeadshotKill)
                { PlayerStats[hitInfo.InitiatorPlayer.userID].stats.kill.LongestHeadshotKill = Vector3.Distance(hitInfo.InitiatorPlayer.transform.position, victim.transform.position); }
            }
            catch { }
        }
        void OnEntityDeath(BaseAnimalNPC animal, HitInfo hitInfo)
        {
            if (hitInfo == null) { return; }
            var attacker = hitInfo.InitiatorPlayer;
            if (attacker == null || !attacker.userID.IsSteamId()) { return; }
            CreateStats(attacker.userID);
            PlayerStats[attacker.userID].stats.kill.AnimalKills++;
        }


        /*Weapon*/
        void OnWeaponFired(BaseProjectile projectile, BasePlayer player, ItemModProjectile mod, ProtoBuf.ProjectileShoot projectiles)
        {
            if (!player.userID.IsSteamId()) { return; }
            CreateStats(player.userID);
            PlayerStats[player.userID].stats.weapon.ShotsFired++;
        }
        void OnEntityTakeDamage(BasePlayer victim, HitInfo hitInfo)
        {
            try
            {
                if (hitInfo == null) { return; }

                if (hitInfo.DidHit)
                {
                    var attacker = hitInfo.InitiatorPlayer;
                    if (!attacker.userID.IsSteamId() || !victim.userID.IsSteamId()) { return; }
                    CreateStats(attacker.userID);
                    if (hitInfo.isHeadshot) { PlayerStats[attacker.userID].stats.weapon.HeadShotsHit++; }
                    else { PlayerStats[attacker.userID].stats.weapon.ShotsHit++; }
                }
            } catch { }
        }

        /*Items*/
        void OnItemCraftFinished(ItemCraftTask task, Item item)
        {
            /*
            var player = task.owner;
            if (!player.userID.IsSteamId()) { return; }
            CreateStats(player.userID);
            PlayerStats[player.userID].stats.items.Crafted++;
            */
        }
        void OnItemResearch(ResearchTable table, Item targetItem, BasePlayer player)
        {
            if (!player.userID.IsSteamId()) { return; }
            CreateStats(player.userID);
            PlayerStats[player.userID].stats.items.Research++;
        }

        private Dictionary<ulong, uint> ActiveRecyclers = new Dictionary<ulong, uint>();
        void OnRecyclerToggle(Recycler recycler, BasePlayer player)
        {
            if (!player.userID.IsSteamId()) { return; }
            CreateStats(player.userID);
            if (!ActiveRecyclers.ContainsKey(player.userID)) { ActiveRecyclers.Add(player.userID, 0); }
            //ActiveRecyclers[player.userID] = recycler.net.ID;
        }

        /*
         * !! NOTE !!
         * 'OnRecycleItem' will stop working on Saturday, 31 December 2022. Please ask the author to update to 'OnItemRecycle(Item item, Recycler recycler)
        */
        void OnRecycleItem(Recycler recycler, Item item)
        {
            /*
            if (recycler.net.ID == null || recycler.net.ID == 0) { return; }
            var Recycles = ActiveRecyclers.Where(x => x.Value == recycler.net.ID).FirstOrDefault();
            if (recycler.net.ID == Recycles.Value)
            {
                PlayerStats[Recycles.Key].stats.items.Recycled++;
            }
            */
        }


        private Dictionary<ulong, ActiveLooterData> ActiveLooter = new Dictionary<ulong, ActiveLooterData>();
        private class ActiveLooterData
        {
            public uint netID;
            public ulong entityOwnerID;
        }
        void OnLootEntity(BasePlayer looter, BaseEntity entity)
        {
            if (!looter.userID.IsSteamId()) { return; }
            var container = (entity as StorageContainer);
            if (container != null)
            {
                if (!ActiveLooter.ContainsKey(looter.userID)) { ActiveLooter.Add(looter.userID, new ActiveLooterData()); }
                //ActiveLooter[looter.userID].netID = container.net.ID;
                ActiveLooter[looter.userID].entityOwnerID = container.OwnerID.IsSteamId() ? container.OwnerID : 0;
            }
            var x = container.net.ID;
        }
        void OnItemRemovedFromContainer(ItemContainer container, Item item)
        {
            //var i = ActiveLooter.Where(x => x.Value.netID == container.uid - 1).FirstOrDefault();
         
            try
            {
                /*
                if (i.Value.entityOwnerID > 0 && i.Value.entityOwnerID.IsSteamId())
                {
                    if (!i.Key.IsSteamId()) { return; }
                    CreateStats(i.Key);
                    if(i.Key != i.Value.entityOwnerID) { PlayerStats[i.Key].stats.items.StoleFromPlayers++; }
                }
                else
                {
                    if (!i.Key.IsSteamId()) { return; }
                    CreateStats(i.Key);
                    PlayerStats[i.Key].stats.items.Found++;
                }
                */
            } catch { }



   
        }



        /*Events*/
        void OnEntityDeath(BradleyAPC bradley, HitInfo hitInfo)
        {
            try
            {
                if (hitInfo == null) { return; }
                var attacker = hitInfo.InitiatorPlayer;
                if (!attacker.userID.IsSteamId()) { return; }
                CreateStats(attacker.userID);
                PlayerStats[attacker.userID].stats.events.BradleyKills++;
            } catch { }
        }
        void OnEntityDeath(BaseHelicopter petrolHeli, HitInfo hitInfo)
        {
            try
            {
                if (hitInfo == null) { return; }
                var attacker = hitInfo.InitiatorPlayer;
                if (!attacker.userID.IsSteamId()) { return; }
                CreateStats(attacker.userID);
                PlayerStats[attacker.userID].stats.events.PetrolHeliKills++;
            } catch { }
      
        }
        void OnLootEntityEnd(BasePlayer player, SupplyDrop supplyDrop)
        {
            if (player == null || supplyDrop == null) { return; }
            if (!player.userID.IsSteamId()) { return; }
            CreateStats(player.userID);
            PlayerStats[player.userID].stats.events.Airdrops++;
        }



        /*Travel*/
        private Dictionary<ulong, TravelData> TravelDistance = new Dictionary<ulong, TravelData>();
        private class TravelData
        {
            public TravelState type { get; set; }
            public Vector3 LastPosition { get; set; }
        }
        private enum TravelState
        {
            Walking,
            Sailing,
            Driving,
            Flying
        }
        void OnEntityMounted(BaseMountable entity, BasePlayer player)
        {
            if (!player.userID.IsSteamId()) { return; }
            CreateStats(player.userID);
            if (!TravelDistance.ContainsKey(player.userID)) { TravelDistance.Add(player.userID, new TravelData()); }
            switch (entity.ShortPrefabName)
            {
                case "miniheliseat":
                case "minihelipassenger":
                case "transporthelipilot":
                case "transporthelicopilot":
                    UpdateDistance(player);
                    TravelDistance[player.userID].type = TravelState.Flying;
                    break;

                case "modularcardriverseat":
                case "modularcarpassengerseatleft":
                case "modularcarpassengerseatright":
                case "modularcarpassengerseatlesslegroomleft":
                case "modularcarpassengerseatlesslegroomright":
                    UpdateDistance(player);
                    TravelDistance[player.userID].type = TravelState.Driving;
                    break;

                case "smallboatdriver":
                case "smallboatpassenger":
                case "standingdriver":
                    UpdateDistance(player);
                    TravelDistance[player.userID].type = TravelState.Sailing;
                    break;

                default:
                    UpdateDistance(player);
                    TravelDistance[player.userID].type = TravelState.Walking;
                    break;
            }
        }
        void OnEntityDismounted(BaseMountable entity, BasePlayer player)
        {
            if (!player.userID.IsSteamId()) { return; }
            CreateStats(player.userID);
            if (!TravelDistance.ContainsKey(player.userID)) { TravelDistance.Add(player.userID, new TravelData()); }
            UpdateDistance(player);
            TravelDistance[player.userID].type = TravelState.Walking;
        }
        void UpdateDistance(BasePlayer player)
        {
            if (!player.userID.IsSteamId()) { return; }
            CreateStats(player.userID);
            if (!TravelDistance.ContainsKey(player.userID)) { TravelDistance.Add(player.userID, new TravelData()); };
            if (player.IsAdmin && player.IsFlying) { TravelDistance[player.userID].LastPosition = player.transform.position; return; }


            var distance = Vector3.Distance(player.transform.position, TravelDistance[player.userID].LastPosition);

            switch (TravelDistance[player.userID].type)
            {
                case TravelState.Flying:
                    PlayerStats[player.userID].stats.travel.Flying += distance;
                    break;
                case TravelState.Driving:
                    PlayerStats[player.userID].stats.travel.Driving += distance;
                    break;
                case TravelState.Sailing:
                    PlayerStats[player.userID].stats.travel.Sailing += distance;
                    break;
                case TravelState.Walking:
                    PlayerStats[player.userID].stats.travel.Walking += distance;
                    break;
            }
            TravelDistance[player.userID].LastPosition = player.transform.position;
        }



        /*Player*/
        private Dictionary<ulong, DateTime> Playtime = new Dictionary<ulong, DateTime>();
        void UpdatePlaytime(BasePlayer player)
        {
            if (!player.userID.IsSteamId()) { return; }
            CreateStats(player.userID);
            if (!Playtime.ContainsKey(player.userID)) { Playtime.Add(player.userID, DateTime.UtcNow); }
            var hours = (DateTime.UtcNow - Playtime[player.userID]).TotalHours;
            Playtime[player.userID] = DateTime.UtcNow;
            PlayerStats[player.userID].stats.player.PlaytimeHours += hours;
        }
        void OnPlayerRevive(BasePlayer reviver, BasePlayer target)
        {
            if (!reviver.userID.IsSteamId() || !target.userID.IsSteamId()) { return; }
            CreateStats(reviver.userID);
            PlayerStats[reviver.userID].stats.player.Revives++;
        }



        /*Structures*/
        void OnStructureUpgrade(BaseCombatEntity entity, BasePlayer player, BuildingGrade.Enum grade)
        {
            if (!player.userID.IsSteamId()) { return; }
            CreateStats(player.userID);
            PlayerStats[player.userID].stats.structures.Upgraded++;
        }
        void OnEntityBuilt(Planner plan, GameObject structure)
        {
            var player = plan.GetOwnerPlayer();

            if (!player.userID.IsSteamId()) { return; }
            CreateStats(player.userID);
            PlayerStats[player.userID].stats.structures.Built++;
        }
        void OnStructureDemolish(BaseCombatEntity entity, BasePlayer player, bool immediate)
        {
            if (!entity.OwnerID.IsSteamId() || !player.userID.IsSteamId()) { return; }
            CreateStats(player.userID);

            if (entity.OwnerID == player.userID)
            {
                PlayerStats[player.userID].stats.structures.Demolished++;
            }
            else
            {
                PlayerStats[player.userID].stats.structures.DemolishedRaided++;
            }
        }
        void OnEntityDeath(BaseCombatEntity entity, HitInfo hitInfo)
        {
            if((entity is BuildingBlock || entity is Door) && hitInfo != null)
            {
                var attacker = hitInfo?.InitiatorPlayer;
                if(attacker == null) { return; }
                if (!entity.OwnerID.IsSteamId() || !attacker.userID.IsSteamId()) { return; }
                CreateStats(attacker.userID);
                PlayerStats[attacker.userID].stats.structures.DemolishedRaided++;
            }
        }
        #endregion Oxide Hooks


        private static Dictionary<ulong, UIDataClass> UIData = new Dictionary<ulong, UIDataClass>();
        private class UIDataClass
        {
            public bool IsActive { get; set; }
            public string activeTable { get; set; }
            public Dictionary<string, double> Table { get; set; }
            public int tablePage { get; set; } = 0;
            public int tableMaxPages { get; set; } = 0;

            public OrderType tableOrder { get; set; } = OrderType.key;
            public SortType tableSort { get; set; } = SortType.descending;

        }



        private void CommandShowStatsUI(BasePlayer player, string command, string[] args)
        {
            if (IsStaff(player))
            {
                if (args == null || args.Length < 1 || args[0] == null) {
                    if (!UIData.ContainsKey(player.userID)) { UIData.Add(player.userID, new UIDataClass()); }
                    UIData[player.userID].Table = null; UIData[player.userID].IsActive = false;
                    UI.Draw(player);
                    return;
                }

                var target = BasePlayer.Find(args[0]);
                if (target != null) //Connected to server Active Player
                {
                    LoadData(target.userID);
                    if (!target.userID.IsSteamId()) { SendReply(player, "Kunne ikke finde target!"); return; }
                    if (!Interface.Oxide.DataFileSystem.ExistsDatafile($"XSTATS/USER/{target.userID}")) { SendReply(player, "Kunne ikke finde target!"); return;  }
                    if (!UIData.ContainsKey(player.userID)) { UIData.Add(player.userID, new UIDataClass()); }
                    UIData[player.userID].Table = null; UIData[player.userID].IsActive = false;
                    UI.Draw(player, 0, target);

                }
                else //Get offline Player Data
                {
           
                    ulong offlineID = args[0].IsSteamId() ? Convert.ToUInt64(args[0]) : 0;
                    if(!offlineID.IsSteamId()) { SendReply(player, "Kunne ikke finde target!"); return; }

                    if (!Interface.Oxide.DataFileSystem.ExistsDatafile($"XSTATS/USER/{offlineID}")) { SendReply(player, "Kunne ikke finde target!"); return; }
                    LoadData(player.userID);
                    LoadData(offlineID);
                    if (!UIData.ContainsKey(player.userID)) { UIData.Add(player.userID, new UIDataClass()); }
                    UIData[player.userID].Table = null; UIData[player.userID].IsActive = false;
                    UI.Draw(player, offlineID);
                }


            }
            else
            {
                UI.Draw(player);
            }
        }

        private static string FormatName(string name) => name.ToLower().EndsWith("s") ? char.ToUpper(name[0]) + name.Substring(1).ToLower() : char.ToUpper(name[0]) + name.Substring(1).ToLower() + "'s";
        #region UI
        public class UI
        {
            public static void Destroy(BasePlayer player)
            {
                if (!UIData.ContainsKey(player.userID)) { UIData.Add(player.userID, new UIDataClass()); }
                CuiHelper.DestroyUi(player, "XStats_MainPanel");
                UIData[player.userID].IsActive = false;
            }
            public static void Draw(BasePlayer player, ulong target = 0, BasePlayer targetPlayer = null)
            {
                var targetID = targetPlayer == null ? (target > 0 && target.IsSteamId()) ? target : player.userID.Get() : targetPlayer.userID.Get();
                Instance.CreateStats(player.userID);
                if (!UIData.ContainsKey(player.userID)) { UIData.Add(player.userID, new UIDataClass()); }


                if (UIData[player.userID].Table == null)
                {
                    UIData[player.userID].activeTable = "resources";
                    UIData[player.userID].tablePage = 0;
                    var count = Instance.PlayerStats[player.userID].stats.resources.GetType().GetProperties().Count();
                    UIData[player.userID].tableMaxPages = count == TableLimit ? 0 : (count / TableLimit);
                    UIData[player.userID].Table = Instance.ProcessTable(Instance.PlayerStats[targetID].stats.resources);
                }



                var playerName = targetPlayer != null ? FormatName(targetPlayer.displayName) : (target > 0 && target.IsSteamId()) ? target.ToString() : FormatName(player.displayName);
                CuiElementContainer container = new CuiElementContainer();

                const double TopNav_height = 0.1;
                if (!UIData[player.userID].IsActive)
                {
                    CuiHelper.DestroyUi(player, "XStats_MainPanel");
                    const int MainPanel_width = 375;
                    const int MainPanel_widthMarginLeft = 0;
                    const int MainPanel_widthMarginRight = 0;

                    const int MainPanel_height = 275;
                    const int MainPanel_heightMarginTop = 0;
                    const int MainPanel_heightMarginBottom = 55;
                    container.Add(new CuiPanel
                    {
                        Image =
                        {
                            Color = $"{BlurColor}",
                            Material = $"{BlurColorMat}",
                        },
                        RectTransform =
                        {
                            AnchorMin = "0.5 0.5",
                            AnchorMax = "0.5 0.5",
                            OffsetMin = $"{-MainPanel_width+MainPanel_widthMarginLeft} {-MainPanel_height+MainPanel_heightMarginBottom}",
                            OffsetMax = $"{MainPanel_width-MainPanel_widthMarginRight} {MainPanel_height-MainPanel_heightMarginTop}",
                        },
                        CursorEnabled = true,
                    }, $"Hud", $"XStats_MainPanel");

                    #region TopNav
                    const double TopNav_CloseBtnWidth = 0.06;
                    container.Add(new CuiPanel
                    {
                        Image =
                        {
                            Color = $"{TransparentColor}",
                        },
                        RectTransform =
                        {
                            AnchorMin = $"0 {1-TopNav_height}",
                            AnchorMax = "1 1",
                            OffsetMax = "0 0"
                        },
                        CursorEnabled = false,
                    }, "XStats_MainPanel", "XStats_MainPanel_TopNav");


                    container.Add(new CuiLabel
                    {
                        RectTransform =
                        {
                            AnchorMin = "0.015 0",
                            AnchorMax = $"{1-TopNav_CloseBtnWidth} 1",
                        },
                        Text =
                        {
                            Text = $"{playerName} - STATISTIK",
                            Align = TextAnchor.MiddleLeft,
                            FontSize = 25
                        }
                    }, "XStats_MainPanel_TopNav");

                    container.Add(new CuiButton
                    {
                        Button =
                        {
                            Color = $"{BlueColor}",
                            Command = $"xstats destroy.ui"
                        },
                        RectTransform =
                        {
                            AnchorMin = $"{1-TopNav_CloseBtnWidth} 0.1",
                            AnchorMax = $"0.995 0.9",
                            OffsetMin = "0 0",
                            OffsetMax = "0 0",
                        },
                        Text =
                        {
                            Color = "1 1 1 1",
                            Text = "X",
                            FontSize = 20,
                            Align = TextAnchor.MiddleCenter,
                        }
                    }, "XStats_MainPanel_TopNav");
                    #endregion TopNav
                }

                

                #region SideNav 
                CuiHelper.DestroyUi(player, "XStats_MainPanel_SideNav");
                container.Add(new CuiPanel
                {
                    Image =
                    {
                        Color = $"{TransparentColor}",
                    },
                    RectTransform =
                    {
                        AnchorMin = $"0 0",
                        AnchorMax = $"0.15 {1-TopNav_height}",
                        OffsetMax = "0 0"
                    },
                    CursorEnabled = false,
                }, "XStats_MainPanel", "XStats_MainPanel_SideNav");



                float TotalProperties = Instance.PlayerStats[player.userID].stats.GetType().GetProperties().Count();
                float SideNavBtnWidth = 1 / TotalProperties;

                const double BtnSpacer = 0.005;
                for (int i = 0; i < Instance.PlayerStats[player.userID].stats.GetType().GetProperties().Count(); i++)
                {
                    var index = Instance.PlayerStats[player.userID].stats.GetType().GetProperties()[i];



                    container.Add(new CuiButton
                    {
                        Button =
                        {
                            Color = $"0 0 0 0",
                            Command = $"xstats statistics.table.{index.Name} {targetID}"
                        },
                        Text =
                        {
                            Text = $"{LangTranslator(index.Name)}",
                            Align = TextAnchor.MiddleCenter,
                            FontSize = 16
                        },
                        RectTransform =
                        {
                            AnchorMin = $"0 {(1-SideNavBtnWidth)-SideNavBtnWidth*i}",
                            AnchorMax = $"1 {1-(SideNavBtnWidth*i)-BtnSpacer}",
                            OffsetMin = "0 0",
                            OffsetMax = "0 0"
                        }
                    }, "XStats_MainPanel_SideNav");

                    if (index.Name == UIData[player.userID].activeTable)
                    {
                        container.Add(new CuiPanel
                        {
                            Image =
                            {
                                Color = "0 0.5 0.75 0.75",
                            },
                            RectTransform =
                            {
                                AnchorMin = $"0.965 {(1-SideNavBtnWidth)-SideNavBtnWidth*i}",
                                AnchorMax = $"1 {1-(SideNavBtnWidth*i)-BtnSpacer}",
                                OffsetMax = "0 0"
                            },
                            CursorEnabled = false,
                        }, "XStats_MainPanel_SideNav");
                    }
                }


                #endregion SideNav

                
                #region Table
                CuiHelper.DestroyUi(player, "XStats_MainPanel_Table");
                CuiHelper.DestroyUi(player, "XStats_MainPanel_Table_TopLine");
                CuiHelper.DestroyUi(player, "XStats_MainPanel_Table_SideLine");
                container.Add(new CuiPanel
                {
                    Image =
                    {
                        Color = $"{BlurColor}"
                    },
                    RectTransform =
                    {
                        AnchorMin = $"0.15 0.05",
                        AnchorMax = $"1 0.825",
                        OffsetMax = "0 0"
                    },
                    CursorEnabled = false,
                }, "XStats_MainPanel", "XStats_MainPanel_Table");


                


                CuiHelper.DestroyUi(player, "XStats_MainPanel_Table_Row_Title");
                container.Add(new CuiPanel
                {
                    Image =
                    {
                        Color = $"{BlurColor}",
                    },
                    RectTransform =
                    {
                        AnchorMin = $"0.15 0.825",
                        AnchorMax = $"1 {1-TopNav_height-0.005}",
                        OffsetMax = "0 0"
                    },
                    CursorEnabled = false,
                }, "XStats_MainPanel", "XStats_MainPanel_Table_Row_Title");
                container.Add(new CuiButton
                {
                    Button =
                    {
                        Color = $"{TransparentColor}",
                        Command = (UIData[player.userID].tableSort == SortType.ascending) ? $"xstats table.sort.key.descending" :  $"xstats table.sort.key.ascending"
                    },
                    Text =
                    {
                        Color = "1 1 1 1",
                        Text = (UIData[player.userID].tableOrder == OrderType.key && UIData[player.userID].tableSort == SortType.ascending) ? "KEY ▼" : "KEY ▲",
                        Align = TextAnchor.MiddleCenter,
                        FontSize = 16,
                    },
                    RectTransform =
                    {
                        AnchorMin = "0.2 0",
                        AnchorMax = "0.4 1",
                        OffsetMax = "0 0",
                        OffsetMin = "0 0"
                    }
                }, "XStats_MainPanel_Table_Row_Title");
                container.Add(new CuiButton
                {
                    Button =
                    {
                            Color = $"{TransparentColor}",
                        Command = (UIData[player.userID].tableSort == SortType.ascending) ? $"xstats table.sort.value.descending" :  $"xstats table.sort.value.ascending"
                    },
                    Text =
                    {
                        Color = "1 1 1 1",
                        Text = (UIData[player.userID].tableOrder == OrderType.value && UIData[player.userID].tableSort == SortType.ascending) ? "VALUE ▼" : "VALUE ▲",
                        Align = TextAnchor.MiddleCenter,
                        FontSize = 16,
                    },
                    RectTransform =
                    {
                        AnchorMin = "0.6 0",
                        AnchorMax = "0.8 1",
                        OffsetMax = "0 0",
                        OffsetMin = "0 0"
                    }
                }, "XStats_MainPanel_Table_Row_Title");

                DrawTable(container, player.userID, UIData[player.userID].Table);





                CuiHelper.DestroyUi(player, "XStats_MainPanel_Table_BottomNav");
                container.Add(new CuiPanel
                {
                    Image =
                    {
                        Color = $"{BlurColor}"
                    },
                    RectTransform =
                    {
                        AnchorMin = $"0.15 0",
                        AnchorMax = $"1 0.05",
                        OffsetMax = "0 0"
                    },
                    CursorEnabled = false,
                }, "XStats_MainPanel", "XStats_MainPanel_Table_BottomNav");


                container.Add(new CuiButton
                {
                    Button =
                    {
                        Color = UIData[player.userID].tablePage > 0 ? $"0 0.5 0.75 0.4" : $"1 1 1 0.1",
                        Command = $"xstats table.page.prev"
                    },
                    Text =
                    {
                        Text = "<<",
                        Align = TextAnchor.MiddleCenter,
                        FontSize = 16,
                    },
                    RectTransform =
                    {
                        AnchorMin = $"0.35 0.1",
                        AnchorMax = $"0.40 0.9",
                        OffsetMax = "0 0",
                        OffsetMin = "0 0"
                    },
                }, "XStats_MainPanel_Table_BottomNav");

                container.Add(new CuiLabel
                {
                    Text =
                    {
                        Text = $"{UIData[player.userID].tablePage+1}/{UIData[player.userID].tableMaxPages+1}",
                        Align = TextAnchor.MiddleCenter,
                        FontSize = 20,
                    },
                    RectTransform =
                    {
                        AnchorMin = $"0.40 0",
                        AnchorMax = $"0.50 1",
                        OffsetMax = "0 0",
                        OffsetMin = "0 0"
                    },
                }, "XStats_MainPanel_Table_BottomNav");


                container.Add(new CuiButton
                {
                    Button =
                    {
                        Color = UIData[player.userID].tablePage < UIData[player.userID].tableMaxPages ?  $"0 0.5 0.75 0.4" : $"1 1 1 0.1",
                        Command = $"xstats table.page.next"
                    },
                    Text =
                    {
                        Text = ">>",
                        Align = TextAnchor.MiddleCenter,
                        FontSize = 16,
                    },
                    RectTransform =
                    {
                        AnchorMin = $"0.5 0.1",
                        AnchorMax = $"0.55 0.9",
                        OffsetMax = "0 0",
                        OffsetMin = "0 0"
                    },
                }, "XStats_MainPanel_Table_BottomNav");
                #endregion Table


                CuiHelper.AddUi(player, container);
                UIData[player.userID].IsActive = true;

            }

            private static void DrawTable(CuiElementContainer container, ulong userID, Dictionary<string, double> dataObject)
            {
                const double rowHeight = (1 / (float)TableLimit);


                var i = 0;
  
     
                foreach (var p in dataObject.Skip(TableLimit * UIData[userID].tablePage).Take(TableLimit))
                {

                    container.Add(new CuiPanel
                    {
                        Image =
                        {
                            Color = i%2 == 0 ? "0 0 0 0.4" : "0 0 0 0.55"
                        },
                        RectTransform =
                        {
                            AnchorMin = $"0.01 {((1-rowHeight)-rowHeight*i)}",
                            AnchorMax = $"0.99 {1-(rowHeight*i)}",
                            OffsetMax = "0 0",
                            OffsetMin = "0 0"
                        },
                        CursorEnabled = false,
                    }, "XStats_MainPanel_Table", $"XStats_MainPanel_Table_Row_{i}");

                    container.Add(new CuiLabel
                    {
                        Text = {
                            Text = $"{LangTranslator(p.Key)}",
                            FontSize = 14,
                            Align = TextAnchor.MiddleCenter,
                        },
                        RectTransform =
                        {
                            AnchorMin = $"0.1 0",
                            AnchorMax = $"0.5 1",
                            OffsetMax = "0 0",
                            OffsetMin = "0 0"
                        },
                    }, $"XStats_MainPanel_Table_Row_{i}");



                    container.Add(new CuiLabel
                    {
                        Text = {
                                Text = $"{ValueTranslator(p.Key, p.Value)}",
                                FontSize = 14,
                                Align = TextAnchor.MiddleCenter,
                            },
                        RectTransform =
                            {
                                AnchorMin = $"0.6 0",
                                AnchorMax = $"0.8 1",
                                OffsetMax = "0 0",
                                OffsetMin = "0 0"
                            },

                    }, $"XStats_MainPanel_Table_Row_{i}");
                    i++;
                }
            }
        }

        #endregion UI
        #region UI Command Handler
        [ConsoleCommand("xstats")]
        private void HandleUICommand(ConsoleSystem.Arg arg)
        {
            var player = arg?.Player();
            var handler = arg?.Args[0];
            var targetID = arg.HasArgs(2) && arg.Args[1] != null ? Convert.ToUInt64(arg?.Args[1]) : player.userID.Get();
            if(player == null || handler == null) { return; }
            Instance.CreateStats(player.userID);
            if (!UIData.ContainsKey(player.userID)) { UIData.Add(player.userID, new UIDataClass()); }
            if (handler.Contains("statistics.table")) { UIData[player.userID].activeTable = handler.Split('.').Last(); }



            switch (handler) 
            {


                case "table.sort.key.descending":
                    UIData[player.userID].tableOrder = OrderType.key; UIData[player.userID].tableSort = SortType.descending;
                    UIData[player.userID].Table = SortTable(UIData[player.userID].Table, SortType.descending, OrderType.key); break;
                case "table.sort.key.ascending":
                    UIData[player.userID].tableOrder = OrderType.key; UIData[player.userID].tableSort = SortType.ascending;
                    UIData[player.userID].Table = SortTable(UIData[player.userID].Table, SortType.ascending, OrderType.key); break;
                case "table.sort.value.descending":
                    UIData[player.userID].tableOrder = OrderType.value; UIData[player.userID].tableSort = SortType.descending;
                    UIData[player.userID].Table = SortTable(UIData[player.userID].Table, SortType.descending, OrderType.value); break;
                case "table.sort.value.ascending":
                    UIData[player.userID].tableOrder = OrderType.value; UIData[player.userID].tableSort = SortType.ascending;
                    UIData[player.userID].Table = SortTable(UIData[player.userID].Table, SortType.ascending, OrderType.value); break;

                case "table.page.next":
                    if(UIData[player.userID].tablePage < UIData[player.userID].tableMaxPages)
                    {
                        UIData[player.userID].tablePage++;
                    }
                    break;
                case "table.page.prev":
                    if (UIData[player.userID].tablePage > 0)
                    {
                        UIData[player.userID].tablePage--;
                    }
                    break;

                case "statistics.table.resources":
                    UIData[player.userID].tablePage = 0;
                    var count1 = Instance.PlayerStats[player.userID].stats.resources.GetType().GetProperties().Count();
                    UIData[player.userID].tableMaxPages = count1 == TableLimit ? 0 : (count1 / TableLimit);
                    UIData[player.userID].Table = Instance.ProcessTable(Instance.PlayerStats[targetID].stats.resources);  break;
                case "statistics.table.raiding":
                    UIData[player.userID].tablePage = 0;
                    var count2 = Instance.PlayerStats[player.userID].stats.raiding.GetType().GetProperties().Count();
                    UIData[player.userID].tableMaxPages = count2 == TableLimit ? 0 : (count2 / TableLimit);
                    UIData[player.userID].Table = Instance.ProcessTable(Instance.PlayerStats[targetID].stats.raiding); break;
                case "statistics.table.kill":
                    UIData[player.userID].tablePage = 0;
                    var count3 = Instance.PlayerStats[player.userID].stats.kill.GetType().GetProperties().Count();
                    UIData[player.userID].tableMaxPages = count3 == TableLimit ? 0 : (count3 / TableLimit);
                    UIData[player.userID].Table = Instance.ProcessTable(Instance.PlayerStats[targetID].stats.kill); break;
                case "statistics.table.weapon":
                    UIData[player.userID].tablePage = 0;
                    var count4 = Instance.PlayerStats[player.userID].stats.weapon.GetType().GetProperties().Count();
                    UIData[player.userID].tableMaxPages = count4 == TableLimit ? 0 : (count4 / TableLimit);
                    UIData[player.userID].Table = Instance.ProcessTable(Instance.PlayerStats[targetID].stats.weapon); break;
                case "statistics.table.items":
                    UIData[player.userID].tablePage = 0;
                    var count5 = Instance.PlayerStats[player.userID].stats.items.GetType().GetProperties().Count();
                    UIData[player.userID].tableMaxPages = count5 == TableLimit ? 0 : (count5 / TableLimit);
                    UIData[player.userID].Table = Instance.ProcessTable(Instance.PlayerStats[targetID].stats.items); break;
                case "statistics.table.events":
                    UIData[player.userID].tablePage = 0;
                    var count6 = Instance.PlayerStats[player.userID].stats.events.GetType().GetProperties().Count();
                    UIData[player.userID].tableMaxPages = count6 == TableLimit ? 0 : (count6 / TableLimit);
                    UIData[player.userID].Table = Instance.ProcessTable(Instance.PlayerStats[targetID].stats.events); break;
                case "statistics.table.travel":
                    UIData[player.userID].tablePage = 0;
                    var count7 = Instance.PlayerStats[player.userID].stats.travel.GetType().GetProperties().Count();
                    UIData[player.userID].tableMaxPages = count7 == TableLimit ? 0 : (count7 / TableLimit);
                    UIData[player.userID].Table = Instance.ProcessTable(Instance.PlayerStats[targetID].stats.travel); break;
                case "statistics.table.player":
                    UIData[player.userID].tablePage = 0;
                    var count8 = Instance.PlayerStats[player.userID].stats.player.GetType().GetProperties().Count();
                    UIData[player.userID].tableMaxPages = count8 == TableLimit ? 0 : (count8 / TableLimit);
                    UIData[player.userID].Table = Instance.ProcessTable(Instance.PlayerStats[targetID].stats.player); break;
                case "statistics.table.structures":
                    UIData[player.userID].tablePage = 0;
                    var count9 = Instance.PlayerStats[player.userID].stats.structures.GetType().GetProperties().Count();
                    UIData[player.userID].tableMaxPages = count9 == TableLimit ? 0 : (count9 / TableLimit);
                    UIData[player.userID].Table = Instance.ProcessTable(Instance.PlayerStats[targetID].stats.structures); break;
            }
            if(handler != "destroy.ui") { UI.Draw(player, targetID); return; }
            UI.Destroy(player);

        }
        #endregion UI Command Handler
    }
}