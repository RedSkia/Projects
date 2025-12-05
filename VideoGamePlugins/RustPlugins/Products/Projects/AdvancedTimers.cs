using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Oxide.Core;
using Oxide.Core.Libraries;
using Oxide.Game.Rust.Cui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Oxide.Plugins
{
    [Info("AdvancedTimers", "Xray", "0.0.1")]
    [Description("Allows admins with permission to initiate scheduled countdown upon completion can perform commands or actions")]

    /*
     *
     *
     *       _____  _____  ______      _____  ______ _      ______           _____ ______ 
     *      |  __ \|  __ \|  ____|    |  __ \|  ____| |    |  ____|   /\    / ____|  ____|
     *      | |__) | |__) | |__ ______| |__) | |__  | |    | |__     /  \  | (___ | |__   
     *      |  ___/|  _  /|  __|______|  _  /|  __| | |    |  __|   / /\ \  \___ \|  __|  
     *      | |    | | \ \| |____     | | \ \| |____| |____| |____ / ____ \ ____) | |____ 
     *      |_|    |_|  \_\______|    |_|  \_\______|______|______/_/    \_\_____/|______|
     */
    /*
    * SOFTWARE LICENCE AGREEMENT OF AdvancedTimers
    *   The following Software License Agreement constitutes a legally binding agreement between You, (the "Buyer") and infinitynet.dk/Xray (the "Vendor").
    *   This Software License Agreement by infinitynet.dk/Xray (the "Vendor") grants the right to use AdvancedTimers (the "Software") respecting the license limitations and restrictions below.
    *   By acquiring a copy of AdvancedTimers (the "Software"), You, the original licensee is legally bound to the license (the "License") Disclaimers, Limitations and Restrictions.
    *   Failing to comply with any of the terms of this Software license Agreement (the "License"), the license ("License") shall be terminated.
    *   On termination of the license ("License"), You are legally obliged to stop all use of AdvancedTimers (the "Software") immediately.
    *   Please read this Software License Agreement (the "License") thoroughly before acquiring a copy of AdvancedTimers (the "Software").
    * 
    * GRANT OF LICENCE
    *   This agreement (the "License") by infinitynet.dk/Xray (the "Vendor") grants the Licensee exclusive, non-transferable, personal-use of the license (the "License") to use this AdvancedTimers (the "Software").
    * 
    * LICENSE DISCLAIMERS, LIMITATIONS AND RESTRICTIONS
    *   AdvancedTimers (the "Software") is provided "AS IS". infinitynet.dk/Xray (the "Vendor") will support, maintain and repair AdvancedTimers (the "Software"), but without any timeframe or guarantees in general.
    *   AdvancedTimers (the "Software") may not be modified, changed, adapted, reverse-engineered, de-compiled, Reproduced, Distributed, Resold, Published or shared without the authority of the infinitynet.dk/Xray (the "Vendor")
    *   AdvancedTimers (the "Software") license (the "License") is restricted to a maximum of 5 installations per license (the "License"). The Licensee may only install AdvancedTimers (the "Software") on computer installations owned or leased by the Licensee.
    */
    public class AdvancedTimers : RustPlugin
    {
        private static AdvancedTimers Instance { get; set; } = new AdvancedTimers();

        private const string SignatureColor = "faaf19";
        private const string DefaultDateTimeFormat = "d:H:m:s";
        private readonly string MissingPerm = $"<size=16>[<color=#{SignatureColor}>AdvancedTimers</color>] Missing permission</size>";
        #region Configuration Init
        protected override void LoadConfig()
        {
            base.LoadConfig();
            try
            {
                _config = JsonConvert.DeserializeObject<Configuration>(JsonConvert.SerializeObject(Config.ReadObject<Configuration>()));
                if (_config == null)
                {
                    PrintWarning($"Configuration is invalid or empty (resetting to default)");
                    LoadDefaultConfig();
                }
            }
            catch
            {
                PrintWarning($"Configuration is corrupted (resetting to default)");
                LoadDefaultConfig();
            }
        }
        protected override void SaveConfig()
        {
            base.SaveConfig();
            try
            {
                Config.WriteObject(_config, true);
            }
            catch
            {
                PrintWarning($"Configuration Failed To Save (resetting to default)");
                LoadDefaultConfig();
            }
        }
        protected override void LoadDefaultConfig()
        {
            PrintWarning($"Creating new configuration");
            Config.WriteObject(GetDefaultConfig(), true);
        }
        #endregion Configuration InitManualCreatorFields
        #region Configuration
        private enum UILocation
        {
            Top,
            LeftTop,
            RightTop,
            LeftMiddle,
            RightMiddle,
            BottomLeft,
            AboveHotbar,
        }
        private enum UILayer
        {
            Overall,
            Overlay,
            Hud,
            HudMenu,
            Under,
        }

        public class InitiateAt
        {
            public string StartAt { get; set; }
            public string EndAt { get; set; }
        }

        public class TimerSize
        {
            public int Height { get; set; }
            public int Width { get; set; }

            [JsonProperty(Required = Required.Always, PropertyName = "ProgressBar Height Percentage 0.0 to 1.0")]
            public float ProgressBarHeight { get; set; }

            [JsonProperty(Required = Required.Always, PropertyName = "Countdown Label Font Size 1 to 25")]
            public int CountdownLabelHeight { get; set; }
        }

        private static Configuration _config;
        private class Configuration
        {
            [JsonProperty(Required = Required.Always, PropertyName = "How often should it check for Scheduled Timers (seconds)")]
            public float CheckDuration { get; set; }

            [JsonProperty(Required = Required.Always, PropertyName = "Timer Permission to Initiate manual Timer")]
            public string Permission { get; set; }

            [JsonProperty(Required = Required.Always, PropertyName = "Timer Tags (KEY, TagMessage) The Keys MUST BE unique")]
            public Dictionary<string, string> Tags { get; set; }

            [JsonProperty(Required = Required.Always, PropertyName = "Scheduled Timers")]
            public List<ScheduledTimers> Timers { get; set; }
            public class ScheduledTimers
            {
                [JsonProperty(Required = Required.Always, PropertyName = "Enable This Timer")]
                public bool IsEnabled { get; set; }

                [JsonProperty(Required = Required.Always, PropertyName = "Timer Setup")]
                public CoreSetup core { get; set; }
                public class CoreSetup
                {
                    [JsonProperty(Required = Required.Always, PropertyName = "Pick Date & Time For Scheduled Executions Set Value To (-1 To Disable) (Day, Hour, Minute, Second) Example (10:20:30:0)")]
                    public InitiateAt[] initiateAt { get; set; }

                    [JsonProperty(Required = Required.Always, PropertyName = "Give Notification At X Seconds")]
                    public int NotifyAt { get; set; }

                    [JsonProperty(Required = Required.Always, PropertyName = "Notification Prefab Sound")]
                    public string Sound { get; set; }

                    [JsonProperty(Required = Required.Always, PropertyName = "Commands To Be Executed Upon Completion")]
                    public string[] Commands { get; set; }
                }

                [JsonProperty(Required = Required.Always, PropertyName = "Timer UI")]
                public UISettings ui { get; set; }
                public class UISettings
                {

                    [JsonProperty(Required = Required.Always, PropertyName = "Timer UI Color Settings")]
                    public Colors colors { get; set; }
                    public class Colors
                    {
                        [JsonProperty(Required = Required.Always, PropertyName = "Message Text Background Color")]
                        public string MessageBackColor { get; set; }

                        [JsonProperty(Required = Required.Always, PropertyName = "Countdown Label Text Background Color")]
                        public string LabelBackColor { get; set; }

                        [JsonProperty(Required = Required.Always, PropertyName = "Progress Bar Track Color")]
                        public string PbarTrack { get; set; }

                        [JsonProperty(Required = Required.Always, PropertyName = "Progress Bar Line Color")]
                        public string PbarLine { get; set; }
                    }

                    [JsonProperty(Required = Required.Always, PropertyName = "Timer UI Layer Hud Is Recommended (Overall, Overlay, Hud, HudMenu, Under)")]
                    public string Layer { get; set; }

                    [JsonProperty(Required = Required.Always, PropertyName = "Timer Location (Top, LeftTop, RightTop, LeftMiddle, RightMiddle, BottomLeft, AboveHotbar)")]
                    public string Location { get; set; }

                    [JsonProperty(Required = Required.Always, PropertyName = "Timer Fade Delay")]
                    public float FadeDelay { get; set; }

                    [JsonProperty(Required = Required.Always, PropertyName = "Timer Size Settings")]
                    public TimerSize Size { get; set; }
                }

                [JsonProperty(Required = Required.Always, PropertyName = "Timer Message")]
                public MessageSettings msg { get; set; }
                public class MessageSettings
                {
                    [JsonProperty(Required = Required.Always, PropertyName = "Timer Tag")]
                    public string Tag { get; set; }

                    [JsonProperty(Required = Required.Always, PropertyName = "Timer Message")]
                    public string Message { get; set; }

                    [JsonProperty(Required = Required.Always, PropertyName = "Timer Message NewLine")]
                    public bool NewLine { get; set; }
                }
            }
        }

        private Configuration GetDefaultConfig()
        {
            return new Configuration
            {
                CheckDuration = 60,
                Permission = "Initiate.timer",
                Tags = new Dictionary<string, string>
                {
                    {"Event","<size=20>[<color=#19964b>Event</color>]</size>"},
                    {"Information","<size=20>[<color=#197daf>Information</color>]</size>"},
                    {"Warning","<size=20>[<color=#c83232>Warning</color>]</size>"},
                },
                Timers = new List<Configuration.ScheduledTimers>
                {
                    new Configuration.ScheduledTimers
                    {
                        IsEnabled = false,
                        core = new Configuration.ScheduledTimers.CoreSetup
                        {
                            NotifyAt = 300,
                            Sound = "assets/prefabs/tools/pager/effects/beep.prefab",
                            initiateAt = new InitiateAt[]
                            {
                                new InitiateAt
                                {
                                    StartAt = DateTime.Now.ToString(DefaultDateTimeFormat),
                                    EndAt = DateTime.Now.AddHours(1.0).ToString(DefaultDateTimeFormat),
                                }
                            },
                            Commands = new string[] {
                                "say Timer Expired"
                            },
                        },
                        msg = new Configuration.ScheduledTimers.MessageSettings
                        {
                            Message = "<size=20><color=#ffffff>My Epic Event</color></size>",
                            Tag = "Event",
                            NewLine = false,
                        },
                        ui = new Configuration.ScheduledTimers.UISettings
                        {
                            Layer = UILayer.Hud.ToString(),
                            Location = UILocation.Top.ToString(),
                            FadeDelay = 0.5f,
                            Size = new TimerSize
                            {
                                Height = 70,
                                Width = 120,
                                ProgressBarHeight = 0.1f,
                                CountdownLabelHeight = 14,
                            },
                            colors = new Configuration.ScheduledTimers.UISettings.Colors
                            {
                                LabelBackColor = "0 0 0 0.525",
                                MessageBackColor = "0 0 0 0.525",
                                PbarLine = "0 1 0 0.750",
                                PbarTrack = "1 0 0 0.500"
                            }
                        },
                    }
                }
            };
        }
        #endregion Configuration

        #region Init
        private void Loaded()
        {
            if (_config != null && Config.Exists())
            {
                permission.RegisterPermission($"{Name}.{_config.Permission}", this);
                timer.Every(_config.CheckDuration, () =>
                {
                    CheckTimers();
                });
            }
            else { PrintWarning("Failed to initiate TimerManagment Due to No Or Corrupted Config\nPlease Reload Plugin"); }
        }

        private void Unload()
        {
            foreach(var i in RunningTimers.Values)
            {
                i.tickTimer.Destroy();
            }
            foreach (BasePlayer player in BasePlayer.allPlayerList)
            {
                foreach (var i in RunningTimers.Keys) { UI.Destroy(player, i); }
            }
            foreach(var i in PlayerLock) 
            {
                i.Value.Kill();
                UnlockPlayer(BasePlayer.FindByID(i.Key));
            }

            RunningTimers.Clear();
            ManualCreatorFields.Clear();
        }
        #endregion Init

        #region Core
        private static Dictionary<int, TimerData> RunningTimers = new Dictionary<int, TimerData>();
        private class TimerData
        {
            public bool HasNotified { get; set; } = false;
            public TimerState State { get; set; }
            public int secs { get; set; }
            public Timer tickTimer { get; set; }
            public Configuration.ScheduledTimers data { get; set; }
            public int Ticks { get; set; }
            public int secsLeft { get; set; }
        }

        private enum TimerState
        {
            Running,
            Starting,
            Done
        }
        
        private void CheckTimers()
        {
            //CleanUp
            foreach(var timer in RunningTimers.ToArray()) { if (timer.Value.State == TimerState.Done) { RunningTimers.Remove(timer.Key); } }

            //Add Again
            for (int i = 0; i < _config.Timers.Count; i++)
            {
                var timer = _config.Timers[i];
                if (timer.IsEnabled && !RunningTimers.ContainsKey(i))
                {
                    foreach (var Point in timer.core.initiateAt)
                    {
                        if (IsTime(new InitiateAt { StartAt = Point.StartAt, EndAt = Point.EndAt }) && !RunningTimers.ContainsKey(i))
                        {
                            var format = GetTimeFormat(new InitiateAt { StartAt = Point.StartAt, EndAt = Point.EndAt });
                            var start = TimeSpan.Parse(Point.StartAt.Replace("-1", "0"));
                            var end = TimeSpan.Parse(Point.EndAt.Replace("-1", "0"));
                            int secs = (int)(end - start).TotalSeconds;
  
                            RunningTimers.Add(i, new TimerData
                            {
                                State = TimerState.Starting,
                                secs = secs,
                                data = timer,
                                tickTimer = null,
                                secsLeft = secs,
                                Ticks = 0,
                            });
                        }
                    }
                }
            }
            TimerSystem.RunTimers();
        }

        private class TimerSystem
        {
            public static void RunTimers()
            {
                foreach (var key in RunningTimers.Keys.ToArray())
                {
                    var timer = RunningTimers[key];
                    if (timer.State != TimerState.Running) 
                    {
                        timer.State = TimerState.Running;

                        //Pre Clear UI
                        foreach (var player in BasePlayer.allPlayerList.ToArray()) { UI.Destroy(player, key); }
                        //Create Base UI
                        foreach (var player in BasePlayer.allPlayerList.ToArray())
                        {
                            UI.DrawTimer(player, key);
                            UI.UpdateProgresbar(player, key, 0);
                            UI.UpdateTimeLabel(player, key, GetTimeLeft(timer.secsLeft));
                        }


                        timer.tickTimer = Instance.timer.Repeat(1f, timer.secs + 1, () =>
                        {
                            timer.Ticks++;
                            timer.secsLeft--;
                            var percentage = GetPercentage(timer.Ticks, timer.secs * 10);

                            if(timer.secsLeft == timer.data.core.NotifyAt && !timer.HasNotified)
                            {
                                timer.HasNotified = true;
                                Instance.PrintToChat($"<size=16>[<color=#{SignatureColor}>AdvancedTimers</color>] Beware a <color=#{SignatureColor}>{timer.data.msg.Tag}</color> Timer is about to execute!</size>");
                                //Play Sound
                                foreach (var player in BasePlayer.allPlayerList.ToArray())
                                {
                                    PlaySound(player, timer.data.core.Sound);
                                }
                            }

                            //Update Dynamic UI
                            foreach (var player in BasePlayer.allPlayerList.ToArray())
                            {
                                UI.UpdateProgresbar(player, key, percentage);
                                UI.UpdateTimeLabel(player, key, GetTimeLeft(timer.secsLeft));
                            }

                            if (timer.secsLeft < 0)
                            {
                                //Destroy UI
                                foreach (var player in BasePlayer.allPlayerList.ToArray()) 
                                { 
                                    UI.Destroy(player, key);
                                }

                                foreach (var cmd in timer.data.core.Commands)
                                {
                                    Instance.Server.Command(cmd.Split(' ')[0], cmd.Replace(cmd.Split(' ')[0], "").Trim());
                                }
                                timer.State = TimerState.Done;
                            }
                        });
                    }
                }
            }

            /*Private Helpers*/
            private static string GetTimeLeft(int secs)
            {
                var ts = TimeSpan.FromSeconds(secs);
                string str =
                    ts.Days > 0 ? $"{ts.Days}d {ts.Hours}h {ts.Minutes}m {ts.Seconds}s" :
                    ts.Hours > 0 ? $"{ts.Hours}h {ts.Minutes}m {ts.Seconds}s" :
                    ts.Minutes > 0 ? $"{ts.Minutes}m {ts.Seconds}s" :
                    ts.Seconds > 0 ? $"{ts.Seconds}s" : "Done";
                return str;
            }
            private static float GetPercentage(float current, float maximum) => ((current / maximum) * 10f);
        }
        #endregion Core

        #region Helpers
        private static Dictionary<ulong, BaseEntity> PlayerLock = new Dictionary<ulong, BaseEntity>();
        private static void LockPlayer(BasePlayer player)
        {
            const string prefab = "assets/bundled/prefabs/static/chair.invisible.static.prefab";

            if(!PlayerLock.ContainsKey(player.userID)) { PlayerLock.Add(player.userID, new BaseEntity()); }


            var PlayerPos = player.transform.position;
            var PlayerRot = player.GetNetworkRotation();
            var MountOjb = GameManager.server.CreateEntity(prefab, new Vector3(PlayerPos.x, PlayerPos.y+0.165f, PlayerPos.z), PlayerRot);
            MountOjb.Spawn();
            MountOjb.name = "AdvancedTimers_CustomChair";
            MountOjb.OwnerID = player.userID;
            player.SetMounted((BaseMountable)MountOjb);
            PlayerLock[player.userID] = MountOjb;
        }
        private static void UnlockPlayer(BasePlayer player)
        {
            if (PlayerLock.ContainsKey(player.userID))
            {
                try { PlayerLock[player.userID].Kill(); PlayerLock[player.userID] = null; } catch { }
            }

            player.DismountObject();
            player.EnsureDismounted();
        }
        private static bool HasPerm(BasePlayer player) => Instance.permission.UserHasPermission(player.UserIDString, $"{Instance.Name}.{_config.Permission}");
        private static DateTime StringToTime(string time, string format) => DateTime.ParseExact(time, format, System.Globalization.CultureInfo.InvariantCulture);

        private string GetTimeFormat(InitiateAt initiateAt)
        {
            var start = initiateAt.StartAt.Split(':');
            var end = initiateAt.EndAt.Split(':');

            var StartDay = start?[0];
            var StartHour = start?[1];
            var StartMinute = start?[2];
            var StartSecond = start?[3];

            var EndDay = end?[0];
            var EndHour = end?[1];
            var EndMinute = end?[2];
            var EndSecond = end?[3];

            if (StartSecond == "-1" || EndSecond == "-1") { PrintWarning("Error Timer cannot have seconds set to -1"); }

            if ((StartDay == "-1" && EndDay == "-1") && (StartHour == "-1" && EndHour == "-1") && (StartMinute == "-1" && EndMinute == "-1")) { return "s"; }
            else if ((StartDay == "-1" && EndDay == "-1") && (StartHour == "-1" && EndHour == "-1")) { return "m:s"; }
            else if ((StartDay == "-1" && EndDay == "-1")) { return "H:m:s"; }
            else { return DefaultDateTimeFormat; }
        }

        /// <param name="initiateAt"></param>
        /// <returns>
        /// <list type="table"><see cref="InitiateAt.StartAt"/> In <see cref="TimeSpan"/> Index 0</list>
        /// <list type="table"><see cref="InitiateAt.EndAt"/> In <see cref="TimeSpan"/> Index 1</list>
        /// </returns>
        private TimeSpan[] ProcessTime(InitiateAt initiateAt)
        {
            var Start = initiateAt.StartAt.Split(':');
            int StartDay =   Start[0] != "-1" ? Convert.ToInt32(Start[0]) : 0;
            int StartHour =  Start[1] != "-1" ? Convert.ToInt32(Start[1]) : 0;
            int StartMin =   Start[2] != "-1" ? Convert.ToInt32(Start[2]) : 0;
            int StartSec =   Start[3] != "-1" ? Convert.ToInt32(Start[3]) : 0;

            var End = initiateAt.EndAt.Split(':');
            int EndDay =  End[0] != "-1" ? Convert.ToInt32(End[0]) : 0;
            int EndHour = End[1] != "-1" ? Convert.ToInt32(End[1]) : 0;
            int EndMin =  End[2] != "-1" ? Convert.ToInt32(End[2]) : 0;
            int EndSec =  End[3] != "-1" ? Convert.ToInt32(End[3]) : 0;
            return new TimeSpan[]
            {
                new TimeSpan(StartDay, StartHour, StartMin, StartSec),
                new TimeSpan(EndDay, EndHour, EndMin, EndSec),
            };
        }
        private bool IsTime(InitiateAt initiateAt)
        {
            var format = GetTimeFormat(new InitiateAt { StartAt = initiateAt.StartAt, EndAt = initiateAt.EndAt });

            TimeSpan nowResult =
                format == "d:H:m:s" ? new TimeSpan(DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second) :
                format == "H:m:s" ? new TimeSpan(0, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second) :
                format == "m:s" ? new TimeSpan(0, 0, DateTime.Now.Minute, DateTime.Now.Second) :
                format == "s" ? new TimeSpan(0, 0, 0, DateTime.Now.Second) : new TimeSpan(0, 0, 0, 0);

            var result = ProcessTime(new InitiateAt
            {
                StartAt = initiateAt.StartAt,
                EndAt = initiateAt.EndAt,
            });
            return (nowResult >= result[0] && nowResult <= result[1]);
        }

        private static string GetTag(string tag)
        {
            if (_config.Tags.ContainsKey(tag))
            {
                return $"{_config.Tags[tag]}";
            }
            else
            {
                return $"Tag: <color=#ff0000>{tag}</color> Not Found\n";
            }
        }

        private static void PlaySound(BasePlayer player, string prefab)
        {
            if (!string.IsNullOrEmpty(prefab))
            {
                var effect = new Effect(prefab, player, 0, Vector3.zero, Vector3.forward);
                EffectNetwork.Send(effect, player.net.connection);
                effect.Clear(true);
            }
        }

        /*Max 25*/
        private static float TextScale(float size) => (size > 25 ? 25 : size <= 10 ? size / 40 : size > 15 && size <= 20 ? size / 55 : size / 57);
        #endregion Helpers

        [ChatCommand("createtimer")]
        private void DrawCommandTimer(BasePlayer player)
        {
            if(!HasPerm(player)) { SendReply(player, MissingPerm); return;}
            else
            {
                UI.Manual.DrawMenu(player, "", false);
            }
        }

        #region Manual Menu Core
        private static Dictionary<ulong, FieldData> ManualCreatorFields = new Dictionary<ulong, FieldData>();
        private class FieldData
        {
            public bool SaveToCfg { get; set; } = false;
            public string StartAt { get; set; } = null;
            public string EndAt { get; set; } = null;
            public int NotifyAt { get; set; } = 0;
            public string Sound { get; set; } = null;
            public string[] Commands { get; set; } = null;
            public string UILayer { get; set; } = null;
            public string UILocation { get; set; } = null;
            public float FadeDelay { get; set; } = 0;
            public int[] HeightWidth { get; set; } = null;
            public float PBarHeight { get; set; } = 0;
            public int LabelHeight { get; set; } = 0;
            public string TimerTag { get; set; } = null;
            public string TimerMessage { get; set; } = null;
            public string MessageColor { get; set; } = null;
            public string LabelColor { get; set; } = null;
            public string PbarTrackColor { get; set; } = null;
            public string PbarLineColor { get; set; } = null;
            public bool NewLine { get; set; } = false;
        }

        #region Commands Handlers
        [ConsoleCommand("advancedtimers.onclose")]
        private void OnClose(ConsoleSystem.Arg arg)
        {
            var player = arg.Player();
            if (!HasPerm(player)) { SendReply(player, MissingPerm); return; }
            else
            {
                if (ManualCreatorFields.ContainsKey(player.userID)) { ManualCreatorFields.Remove(player.userID); }
                CuiHelper.DestroyUi(player, "AdvancedTimers_ManualCreator");
                UnlockPlayer(player);
            }
        }

        [ConsoleCommand("advancedtimers.togglecheckbox")]
        private void OnToggleCheckBox(ConsoleSystem.Arg arg)
        {
            var player = arg.Player();
            if (!HasPerm(player)) { SendReply(player, MissingPerm); return; }
            else
            {
                if (!ManualCreatorFields.ContainsKey(player.userID)) { ManualCreatorFields.Add(player.userID, new FieldData()); }
                if (ManualCreatorFields.ContainsKey(player.userID))
                {
                    /*Opposite Toggle*/
                    switch ((bool)ManualCreatorFields[player.userID].SaveToCfg)
                    {
                        case false:
                            ManualCreatorFields[player.userID].SaveToCfg = true;
                            break;

                        case true:
                            ManualCreatorFields[player.userID].SaveToCfg = false;
                            break;

                    }
                }
                UI.Manual.DrawMenu(player, "", true);
            }
        }

        [ConsoleCommand("advancedtimers.submitfield")]
        private void OnSubmitField(ConsoleSystem.Arg arg)
        {
            var player = arg.Player();
            if (!HasPerm(player)) { SendReply(player, MissingPerm); return; }
            else
            {
                var key = arg?.Args[0];
                if (!ManualCreatorFields.ContainsKey(player.userID)) { ManualCreatorFields.Add(player.userID, new FieldData()); }

                if (ManualCreatorFields.ContainsKey(player.userID) && arg.HasArgs(2))
                {
                    switch (key)
                    {
                        case "start":
                            ManualCreatorFields[player.userID].StartAt = null;
                            ManualCreatorFields[player.userID].StartAt = arg?.Args[1];
                            break;

                        case "end":
                            ManualCreatorFields[player.userID].EndAt = null;
                            ManualCreatorFields[player.userID].EndAt = arg?.Args[1];
                            break;

                        case "notify":
                            ManualCreatorFields[player.userID].NotifyAt = 0;
                            if (!(bool)(arg?.Args[1].All(char.IsDigit))) { UI.Manual.DrawMenu(player, "Notify At Must Be In Seconds!", false); return; }
                            ManualCreatorFields[player.userID].NotifyAt = Convert.ToInt32(arg?.Args[1]);
                            break;

                        case "sound":
                            ManualCreatorFields[player.userID].Sound = null;
                            ManualCreatorFields[player.userID].Sound = arg?.Args[1];
                            break;

                        case "commands":
                            ManualCreatorFields[player.userID].Commands = null;
                            ManualCreatorFields[player.userID].Commands = string.Join(" ", arg.Args.Skip(1)).Split(',');
                            break;

                        case "msgbackcolor":
                            ManualCreatorFields[player.userID].MessageColor = null;
                            ManualCreatorFields[player.userID].MessageColor = arg?.Args[1];
                            break;

                        case "labelbackcolor":
                            ManualCreatorFields[player.userID].LabelColor = null;
                            ManualCreatorFields[player.userID].LabelColor = arg?.Args[1];
                            break;

                        case "pbartrackcolor":
                            ManualCreatorFields[player.userID].PbarTrackColor = null;
                            ManualCreatorFields[player.userID].PbarTrackColor = arg?.Args[1];
                            break;

                        case "pbarlinecolor":
                            ManualCreatorFields[player.userID].PbarLineColor = null;
                            ManualCreatorFields[player.userID].PbarLineColor = arg?.Args[1];
                            break;

                        case "layer":
                            ManualCreatorFields[player.userID].UILayer = null;
                            if (!Enum.IsDefined(typeof(UILayer), arg?.Args[1])) { UI.Manual.DrawMenu(player, "UI Layer Isn't Valid!", false); return; }
                            ManualCreatorFields[player.userID].UILayer = arg?.Args[1];
                            break;

                        case "location":
                            ManualCreatorFields[player.userID].UILocation = null;
                            if (!Enum.IsDefined(typeof(UILocation), arg?.Args[1])) { UI.Manual.DrawMenu(player, "UI Location Isn't Valid!", false); return; }
                            ManualCreatorFields[player.userID].UILocation = arg?.Args[1];
                            break;

                        case "fade":
                            ManualCreatorFields[player.userID].FadeDelay = 0;
                            try { ManualCreatorFields[player.userID].FadeDelay = float.Parse(arg?.Args[1]); }
                            catch { UI.Manual.DrawMenu(player, "FadeDelay Must Be In Seconds! E.g.(0.25)", false); return; }
                            break;

                        case "heightwidth":
                            ManualCreatorFields[player.userID].HeightWidth = null;
                            var i = string.Join(" ", arg.Args.Skip(1)).Split(',');
                            if (!i.All(x => x.All(char.IsDigit))) { UI.Manual.DrawMenu(player, "Height,Width Must Be Numbers!", false); return; }
                            ManualCreatorFields[player.userID].HeightWidth = new int[] { Convert.ToInt32(i[0]), Convert.ToInt32(i[1]) };
                            break;

                        case "pbarheight":
                            ManualCreatorFields[player.userID].PBarHeight = 0;
                            try { ManualCreatorFields[player.userID].PBarHeight = float.Parse(arg?.Args[1]); }
                            catch { UI.Manual.DrawMenu(player, "ProgressBar Height Must Be Numbers & In Percentage(0.0 - 1.0)!", false); return; }
                            break;

                        case "labelsize":
                            ManualCreatorFields[player.userID].LabelHeight = 0;
                            if (!(bool)arg?.Args[1].All(char.IsDigit)) { UI.Manual.DrawMenu(player, "Label Height Must Be Numbers (1-25)!", false); return; }
                            ManualCreatorFields[player.userID].LabelHeight = Convert.ToInt32(arg?.Args[1]);
                            break;

                        case "timertag":
                            ManualCreatorFields[player.userID].TimerTag = null;
                            if (!_config.Tags.ContainsKey(arg?.Args[1])) { UI.Manual.DrawMenu(player, "Tag Not Found In Config!", false); return; }
                            ManualCreatorFields[player.userID].TimerTag = arg?.Args[1];
                            break;

                        case "timermessage":
                            ManualCreatorFields[player.userID].TimerMessage = null;
                            ManualCreatorFields[player.userID].TimerMessage = arg?.Args[1];
                            break;

                        case "newline":
                            ManualCreatorFields[player.userID].NewLine = false;
                            ManualCreatorFields[player.userID].NewLine =
                                arg?.Args[1] == "True" ? true :
                                arg?.Args[1] == "true" ? true : false;
                            break;
                    }
                }
            }
        }

        [ConsoleCommand("advancedtimers.submittimer")]
        private void OnSubmitTimer(ConsoleSystem.Arg arg)
        {
            var player = arg.Player();
            if (!HasPerm(player)) { SendReply(player, MissingPerm); return; }
            else
            {
                var values = ManualCreatorFields[player.userID];
                var defaultValues = GetDefaultConfig().Timers.FirstOrDefault();
                if (values.StartAt == null) { UI.Manual.DrawMenu(player, "Start Time Empty!", false); return; }
                if (values.EndAt == null) { UI.Manual.DrawMenu(player, "End Time Empty!", false); return; }
                if (values.UILocation == null) { UI.Manual.DrawMenu(player, "Timer Location Empty!", false); return; }
                if (values.HeightWidth == null) { UI.Manual.DrawMenu(player, "Timer Height or Witdh Empty!", false); return; }
                if (values.TimerTag == null) { UI.Manual.DrawMenu(player, "Timer Tag Empty!", false); return; }
                if (values.TimerMessage == null) { UI.Manual.DrawMenu(player, "Timer Message Empty!", false); return; }

                InitiateAt point = new InitiateAt { StartAt = values.StartAt, EndAt = values.EndAt };

                var start = TimeSpan.Parse(point.StartAt.Replace("-1", "0"));
                var end = TimeSpan.Parse(point.EndAt.Replace("-1", "0"));
                int secs = (int)(end - start).TotalSeconds;

                var timer = new Configuration.ScheduledTimers
                {
                    IsEnabled = true,
                    core = new Configuration.ScheduledTimers.CoreSetup
                    {
                        initiateAt = new InitiateAt[] { point != null ? point : defaultValues.core.initiateAt.FirstOrDefault() },
                        NotifyAt = values.NotifyAt,
                        Commands = values.Commands != null ? values.Commands : defaultValues.core.Commands,
                        Sound = values.Sound != null ? values.Sound : defaultValues.core.Sound,
                    },
                    msg = new Configuration.ScheduledTimers.MessageSettings
                    {
                        Message = values.TimerMessage != null ? values.TimerMessage : defaultValues.msg.Message,
                        NewLine = values.NewLine,
                        Tag = values.TimerTag,
                    },
                    ui = new Configuration.ScheduledTimers.UISettings
                    {
                        FadeDelay = values.FadeDelay,
                        Layer = values.UILayer != null ? values.UILayer : defaultValues.ui.Layer,
                        Location = values.UILocation != null ? values.UILocation : defaultValues.ui.Location,
                        Size = new TimerSize
                        {
                            Height = values.HeightWidth[0],
                            Width = values.HeightWidth[1],
                            CountdownLabelHeight = values.LabelHeight,
                            ProgressBarHeight = values.PBarHeight,
                        },
                        colors = new Configuration.ScheduledTimers.UISettings.Colors
                        {
                            LabelBackColor = values.LabelColor != null ? values.LabelColor : defaultValues.ui.colors.LabelBackColor,
                            MessageBackColor = values.MessageColor != null ? values.MessageColor : defaultValues.ui.colors.MessageBackColor,
                            PbarLine = values.PbarLineColor != null ? values.PbarLineColor : defaultValues.ui.colors.PbarLine,
                            PbarTrack = values.PbarTrackColor != null ? values.PbarTrackColor : defaultValues.ui.colors.PbarTrack,
                        },
                    }
                };

                if (values.SaveToCfg)
                {
                    _config.Timers.Add(timer);
                    Config.WriteObject(_config, true);
                }
                else
                {
                    RunningTimers.Add(RunningTimers.Keys.Count + 1, new TimerData
                    {
                        State = TimerState.Starting,
                        secs = secs,
                        tickTimer = null,
                        secsLeft = secs,
                        Ticks = 0,
                        data = timer,
                    });
                }

                OnClose(arg);
            }
        }
        #endregion Commands Handlers
        #endregion Manual Menu Core

        private class UI
        {
            public class Manual
            {
                public static void DrawCheckbox(BasePlayer player, CuiElementContainer container)
                {
                    if (!ManualCreatorFields.ContainsKey(player.userID)) { ManualCreatorFields.Add(player.userID, new FieldData()); }

                    

                    CuiHelper.DestroyUi(player, "AdvancedTimers_ManualCreator_CheckboxButton");
                    /*Single Run CheckBox Button*/
                    container.Add(new CuiButton
                    {
                        Button =
                        {
                            Command = $"advancedtimers.togglecheckbox {container}",
                            Color = "0 0 0 0.725",
                        },
                        Text =
                        {
                            Align = TextAnchor.MiddleCenter,
                            Color = "1 1 1 1",
                            Text = ManualCreatorFields[player.userID].SaveToCfg ? "Save To Cfg: <color=#00ff00>✓</color>" : "Save To Cfg: <color=#ff0000>✕</color>",
                            FontSize = 16,
                        },
                        RectTransform =
                        {
                            AnchorMin = "0.70 0.015",
                            AnchorMax = "0.985 0.065",
                            OffsetMax = "0 0"
                        }
                    }, "AdvancedTimers_ManualCreator_FieldsBox", "AdvancedTimers_ManualCreator_CheckboxButton");
                }
                public static void DrawMenu(BasePlayer player, string PopupMsg, bool UpdateCheckBox)
                {
                    CuiElementContainer container = new CuiElementContainer();

                    LockPlayer(player);

                    if (PopupMsg != "")
                    {
                        DrawPopup(player, container, PopupMsg);
                        CuiHelper.AddUi(player, container);
                        return;
                    }

                    if (UpdateCheckBox)
                    {
                        DrawCheckbox(player, container);
                        CuiHelper.AddUi(player, container);
                        return;
                    }

                    CuiHelper.DestroyUi(player, $"AdvancedTimers_ManualCreator");
                    container.Add(new CuiPanel
                    {
                        Image = { Color = "0 0 0 0.925" },
                        RectTransform =
                        {
                            AnchorMin = "0.5 0.5",
                            AnchorMax = "0.5 0.5",
                            OffsetMin = "-200 -275",
                            OffsetMax = "200 325",
                        },
                        CursorEnabled = true,
                    }, $"Overall", $"AdvancedTimers_ManualCreator");

                    container.Add(new CuiPanel
                    {
                        Image =
                        {
                            Color = "0 0 0 0.825"
                        },
                        RectTransform =
                        {
                            AnchorMin = "0 0.925",
                            AnchorMax = "1 1",
                            OffsetMax = "0 0"
                        },
                        CursorEnabled = false,
                    }, "AdvancedTimers_ManualCreator", "AdvancedTimers_ManualCreator_TopNav");
                    container.Add(new CuiButton
                    {
                        Button =
                        {
                            Color = "1 0 0 0.35",
                            Command = "advancedtimers.onclose",
                        },
                        Text =
                        {
                            Align = TextAnchor.MiddleCenter,
                            Color = "1 1 1 1",
                            Text = "X",
                            FontSize = 16,
                        },
                        RectTransform =
                        {
                            AnchorMin = "0.9 0.2",
                            AnchorMax = "0.99 0.9",
                            OffsetMax = "0 0"
                        }
            
          
                    }, "AdvancedTimers_ManualCreator_TopNav", "AdvancedTimers_ManualCreator_TopNav_CloseButton");
                    container.Add(new CuiLabel
                    {
                        Text =
                        {
                            Text = $"[<color=#{SignatureColor}>AdvancedTimers</color>] Manual Creator",
                            Align = TextAnchor.MiddleCenter,
                            Color = "1 1 1 1",
                            FontSize = 20,
                        },
                        RectTransform =
                        {
                            AnchorMin = "0.1 0",
                            AnchorMax = "0.9 1",
                            OffsetMax = "0 0"
                        },
                    }, "AdvancedTimers_ManualCreator_TopNav", "AdvancedTimers_ManualCreator_TopNav_Label");


                    container.Add(new CuiPanel
                    {
                        Image =
                        {
                            Color = "0 0 0 0"
                        },
                        RectTransform =
                        {
                            AnchorMin = "0 0",
                            AnchorMax = "1 0.925",
                            OffsetMax = "0 0"
                        },
                        CursorEnabled = false,
                    }, "AdvancedTimers_ManualCreator", "AdvancedTimers_ManualCreator_FieldsBox");

                    //Height 0.70 w/ Line
                    const float Height = 0.051f;
                    const float LineHeight = 0.0012f;

                    DrawInputField(container, "AdvancedTimers_ManualCreator_FieldsBox", $"0 {Math.Abs((Height *  1) - 1)}", $"1 {Math.Abs((Height *  0) - 1) - LineHeight}", "Start At (d:h:m:s)", "advancedtimers.submitfield start");
                    DrawInputField(container, "AdvancedTimers_ManualCreator_FieldsBox", $"0 {Math.Abs((Height *  2) - 1)}", $"1 {Math.Abs((Height *  1) - 1) - LineHeight}", "End At (d:h:m:s)", "advancedtimers.submitfield end");
                    DrawInputField(container, "AdvancedTimers_ManualCreator_FieldsBox", $"0 {Math.Abs((Height *  3) - 1)}", $"1 {Math.Abs((Height *  2) - 1) - LineHeight}", "Notify At (secs)", "advancedtimers.submitfield notify");
                    DrawInputField(container, "AdvancedTimers_ManualCreator_FieldsBox", $"0 {Math.Abs((Height *  4) - 1)}", $"1 {Math.Abs((Height *  3) - 1) - LineHeight}", "Sound (prefab)", "advancedtimers.submitfield sound");
                    DrawInputField(container, "AdvancedTimers_ManualCreator_FieldsBox", $"0 {Math.Abs((Height *  5) - 1)}", $"1 {Math.Abs((Height *  4) - 1) - LineHeight}", "Commands (,)", "advancedtimers.submitfield commands");
                    DrawInputField(container, "AdvancedTimers_ManualCreator_FieldsBox", $"0 {Math.Abs((Height *  6) - 1)}", $"1 {Math.Abs((Height *  5) - 1) - LineHeight}", "Msg Back Color", "advancedtimers.submitfield msgbackcolor");
                    DrawInputField(container, "AdvancedTimers_ManualCreator_FieldsBox", $"0 {Math.Abs((Height *  7) - 1)}", $"1 {Math.Abs((Height *  6) - 1) - LineHeight}", "Label Back Color", "advancedtimers.submitfield labelbackcolor");
                    DrawInputField(container, "AdvancedTimers_ManualCreator_FieldsBox", $"0 {Math.Abs((Height *  8) - 1)}", $"1 {Math.Abs((Height *  7) - 1) - LineHeight}", "Pbar Track Color", "advancedtimers.submitfield pbartrackcolor");
                    DrawInputField(container, "AdvancedTimers_ManualCreator_FieldsBox", $"0 {Math.Abs((Height *  9) - 1)}", $"1 {Math.Abs((Height *  8) - 1) - LineHeight}", "Pbar Line Color", "advancedtimers.submitfield pbarlinecolor");
                    DrawInputField(container, "AdvancedTimers_ManualCreator_FieldsBox", $"0 {Math.Abs((Height * 10) - 1)}", $"1 {Math.Abs((Height *  9) - 1) - LineHeight}", "UI Layer", "advancedtimers.submitfield layer");
                    DrawInputField(container, "AdvancedTimers_ManualCreator_FieldsBox", $"0 {Math.Abs((Height * 11) - 1)}", $"1 {Math.Abs((Height * 10) - 1) - LineHeight}", "Timer Location", "advancedtimers.submitfield location");
                    DrawInputField(container, "AdvancedTimers_ManualCreator_FieldsBox", $"0 {Math.Abs((Height * 12) - 1)}", $"1 {Math.Abs((Height * 11) - 1) - LineHeight}", "Fade Delay", "advancedtimers.submitfield fade");
                    DrawInputField(container, "AdvancedTimers_ManualCreator_FieldsBox", $"0 {Math.Abs((Height * 13) - 1)}", $"1 {Math.Abs((Height * 12) - 1) - LineHeight}", "Height Width (,)", "advancedtimers.submitfield heightwidth");
                    DrawInputField(container, "AdvancedTimers_ManualCreator_FieldsBox", $"0 {Math.Abs((Height * 14) - 1)}", $"1 {Math.Abs((Height * 13) - 1) - LineHeight}", "PBar Height (% 0.0-1.0)", "advancedtimers.submitfield pbarheight");
                    DrawInputField(container, "AdvancedTimers_ManualCreator_FieldsBox", $"0 {Math.Abs((Height * 15) - 1)}", $"1 {Math.Abs((Height * 14) - 1) - LineHeight}", "Label Size (Max 25)", "advancedtimers.submitfield labelsize");
                    DrawInputField(container, "AdvancedTimers_ManualCreator_FieldsBox", $"0 {Math.Abs((Height * 16) - 1)}", $"1 {Math.Abs((Height * 15) - 1) - LineHeight}", "Timer Tag", "advancedtimers.submitfield timertag");
                    DrawInputField(container, "AdvancedTimers_ManualCreator_FieldsBox", $"0 {Math.Abs((Height * 17) - 1)}", $"1 {Math.Abs((Height * 16) - 1) - LineHeight}", "Timer Message", "advancedtimers.submitfield timermessage");
                    DrawInputField(container, "AdvancedTimers_ManualCreator_FieldsBox", $"0 {Math.Abs((Height * 18) - 1)}", $"1 {Math.Abs((Height * 17) - 1) - LineHeight}", "Newline (true,false)", "advancedtimers.submitfield newline");

                    /*Line*/
                    container.Add(new CuiPanel
                    {
                        Image = { Color = "0.980 0.686 0.098 1" },
                        RectTransform =
                        {
                            AnchorMin = $"0 0.0785",
                            AnchorMax = $"1 0.08",
                            OffsetMax = "0 0"
                        },
                        CursorEnabled = false,
                    }, "AdvancedTimers_ManualCreator_FieldsBox");

                    DrawCheckbox(player, container);

                    /*Submit Button*/
                    container.Add(new CuiButton
                    {
                        Button =
                        {
                            Command = "advancedtimers.submittimer",
                            Color = "0 1 0 0.35",
                        },
                        Text =
                        {
                            Align = TextAnchor.MiddleCenter,
                            Color = "1 1 1 1",
                            Text = "Submit",
                            FontSize = 16,
                        },
                        RectTransform =
                        {
                            AnchorMin = "0.0175 0.015",
                            AnchorMax = "0.685 0.065",
                            OffsetMax = "0 0"
                        }


                    }, "AdvancedTimers_ManualCreator_FieldsBox", "AdvancedTimers_ManualCreator_SubmitButton");

                    CuiHelper.AddUi(player, container);
                }

                /*Private Helpers*/
                private static void DrawInputField(CuiElementContainer container, string parent, string aMin, string aMax, string text, string command)
                {
                    /*Panel*/
                    container.Add(new CuiPanel
                    {
                        Image =
                        {
                            Color = "0 0 0 0.75"
                        },
                        RectTransform =
                        {
                            AnchorMin = aMin,
                            AnchorMax = aMax,
                            OffsetMax = "0 0"
                        },
                        CursorEnabled = false,
                    }, parent, "AdvancedTimers_ManualCreator_FieldsBox_Panel");

                    /*Input Field Panel*/
                    container.Add(new CuiPanel
                    {
                        Image = { Color = "0.25 0.25 0.25 0.75" },
                        RectTransform =
                        {
                            AnchorMin = "0.4 0.2",
                            AnchorMax = "0.99 0.8",
                            OffsetMax = "0 0"
                        },
                        CursorEnabled = false,
                    }, "AdvancedTimers_ManualCreator_FieldsBox_Panel", "AdvancedTimers_ManualCreator_FieldsBox_InputPanel");

                    /*Real Input Field*/
                    container.Add(new CuiElement
                    {
                        Parent = "AdvancedTimers_ManualCreator_FieldsBox_InputPanel",
                        Components =
                        {
                            new CuiInputFieldComponent
                            {
                                Color = "1 1 1 1",
                                Align = UnityEngine.TextAnchor.MiddleLeft,
                                FontSize = 12,
                                CharsLimit = 64,
                                Command = command,
                            },
                            new CuiRectTransformComponent
                            {
                                AnchorMin = "0.01 0",
                                AnchorMax = "1 1",
                                OffsetMax = "0 0"
                            }
                        }
                    });

                    /*Line*/
                    container.Add(new CuiPanel
                    {
                        Image = { Color = "0.980 0.686 0.098 1" },
                        RectTransform =
                        {
                            AnchorMin = $"0 {Convert.ToDouble(aMax.Split(' ')[1])}",
                            AnchorMax = $"1 {Convert.ToDouble(aMax.Split(' ')[1])+0.0012f}",
                            OffsetMax = "0 0"
                        },
                        CursorEnabled = false,
                    }, parent);

                    /*Text*/
                    container.Add(new CuiLabel
                    {
                        Text =
                        {
                            Align = TextAnchor.MiddleLeft,
                            Color = "1 1 1 1",
                            Text = text,
                        },
                        RectTransform =
                        {
                            AnchorMin = "0.01 0",
                            AnchorMax = "0.4 1",
                            OffsetMax = "0 0"
                        },
                    }, "AdvancedTimers_ManualCreator_FieldsBox_Panel");
                }
                private static void DrawPopup(BasePlayer player, CuiElementContainer container, string Message)
                {
                    CuiHelper.DestroyUi(player, "AdvancedTimers_ManualCreator_Popup");
                    container.Add(new CuiPanel
                    {
                        Image = { Color = "0 0 0 0.925" },
                        RectTransform =
                        {
                            AnchorMin = "0 0",
                            AnchorMax = "1 1",
                            OffsetMin = "0 0",
                            OffsetMax = "0 0",
                        },
                        CursorEnabled = true,
                    }, $"AdvancedTimers_ManualCreator", $"AdvancedTimers_ManualCreator_Popup");

                    container.Add(new CuiLabel
                    {
                        Text =
                        {
                            Text = $"{Message}",
                            Align = TextAnchor.MiddleCenter,
                            Color = "1 1 1 1",
                            FontSize = 24,
                        },
                        RectTransform =
                        {
                            AnchorMin = "0 0",
                            AnchorMax = "1 1",
                            OffsetMax = "0 0"
                        },
                    }, "AdvancedTimers_ManualCreator_Popup", "AdvancedTimers_ManualCreator_Popup_Label");

                    Instance.timer.Once(2f, () => { CuiHelper.DestroyUi(player, "AdvancedTimers_ManualCreator_Popup"); });
                }
            }

            public static void Destroy(BasePlayer player, int key)
            {
                CuiHelper.DestroyUi(player, $"AdvancedTimers_Box_{key}");
                CuiHelper.DestroyUi(player, $"AdvancedTimers_Box_{key}");
                CuiHelper.DestroyUi(player, $"AdvancedTimers_TitleBox_{key}");
                CuiHelper.DestroyUi(player, $"AdvancedTimers_TitleBox_Label_{key}");
                CuiHelper.DestroyUi(player, $"AdvancedTimers_TimeBox_{key}");
                CuiHelper.DestroyUi(player, $"AdvancedTimers_TimeBox_Label_{key}");
                CuiHelper.DestroyUi(player, $"AdvancedTimers_ProgessbarTrack_{key}");
                CuiHelper.DestroyUi(player, $"AdvancedTimers_ProgessbarLine_{key}");
            }

            public static void DrawTimer(BasePlayer player, int Index)
            {
                UI.Destroy(player, Index);
                CuiElementContainer container = new CuiElementContainer();

                DrawElements.Container(container, Index);
                DrawElements.Title(container, Index);
                DrawElements.TimeLabel(container, Index);
                DrawElements.ProgressBar.Track(container, Index);
                DrawElements.ProgressBar.Line(container, Index, 0.0f);

                CuiHelper.AddUi(player, container);
            }

            public static void UpdateTimeLabel(BasePlayer player, int Index, string Value)
            {
                var timer = RunningTimers[Index];
                CuiHelper.DestroyUi(player, $"AdvancedTimers_TimeBox_Label_{Index}");
                CuiElementContainer container = new CuiElementContainer();
                container.Add(new CuiLabel
                {
                    RectTransform =
                    {
                        AnchorMin = "0 0",
                        AnchorMax = "1 1",
                        OffsetMax = "0 0"
                    },
                    Text =
                    {
                        Text = Value,
                        Color = "1 1 1 1",
                        Align = UnityEngine.TextAnchor.MiddleCenter
                    }
                }, $"AdvancedTimers_TimeBox_{Index}", $"AdvancedTimers_TimeBox_Label_{Index}");
                CuiHelper.AddUi(player, container);
            }

            public static void UpdateProgresbar(BasePlayer player, int Index, float Value)
            {
                var i = Value > 1 ? 1 : Value < 0 ? 0 : Value;
                var timer = RunningTimers[Index];
                CuiHelper.DestroyUi(player, $"AdvancedTimers_ProgessbarLine_{Index}");
                CuiElementContainer container = new CuiElementContainer();
                container.Add(new CuiPanel
                {
                    Image = { Color = timer.data.ui.colors.PbarLine },
                    RectTransform =
                            {
                                AnchorMin = "0 0",
                                AnchorMax = $"{i} 1",
                                OffsetMax = "0 0"
                            },
                    CursorEnabled = false,
                }, $"AdvancedTimers_ProgessbarTrack_{Index}", $"AdvancedTimers_ProgessbarLine_{Index}");
                CuiHelper.AddUi(player, container);
            }

            private class DrawElements
            {
                public static void Container(CuiElementContainer container, int Index)
                {
                    /*
                     * Anchor = Width(Min, Max) Height(Min, Max) In Percentage
                     * Offset = Width(Min, Max) Height(Min, Max) In Pixels
                     */
                    var timer = RunningTimers[Index];

                    #region Location
                    var aMin =
                        timer.data.ui.Location == UILocation.Top.ToString() ? "0.5 1" :
                        timer.data.ui.Location == UILocation.LeftTop.ToString() ? "0 1" :
                        timer.data.ui.Location == UILocation.RightTop.ToString() ? "1 1" :
                        timer.data.ui.Location == UILocation.LeftMiddle.ToString() ? "0 0.5" :
                        timer.data.ui.Location == UILocation.RightMiddle.ToString() ? "1 0.5" :
                        timer.data.ui.Location == UILocation.BottomLeft.ToString() ? "0 0" :
                        timer.data.ui.Location == UILocation.AboveHotbar.ToString() ? "0.5 0" : "";
                    var aMax =
                        timer.data.ui.Location == UILocation.Top.ToString() ? "0.5 1" :
                        timer.data.ui.Location == UILocation.LeftTop.ToString() ? "0 1" :
                        timer.data.ui.Location == UILocation.RightTop.ToString() ? "1 1" :
                        timer.data.ui.Location == UILocation.LeftMiddle.ToString() ? "0 0.5" :
                        timer.data.ui.Location == UILocation.RightMiddle.ToString() ? "1 0.5" :
                        timer.data.ui.Location == UILocation.BottomLeft.ToString() ? "0 0" :
                        timer.data.ui.Location == UILocation.AboveHotbar.ToString() ? "0.5 0" : "";

                    var oMin =
                        timer.data.ui.Location == UILocation.Top.ToString() ? $"{-timer.data.ui.Size.Width} {-timer.data.ui.Size.Height - 50}" :
                        timer.data.ui.Location == UILocation.LeftTop.ToString() ? $"0 {-timer.data.ui.Size.Height}" :
                        timer.data.ui.Location == UILocation.RightTop.ToString() ? $"{-timer.data.ui.Size.Width * 2} {-timer.data.ui.Size.Height}" :
                        timer.data.ui.Location == UILocation.LeftMiddle.ToString() ? $"0 {-timer.data.ui.Size.Height / 2}" :
                        timer.data.ui.Location == UILocation.RightMiddle.ToString() ? $"{-timer.data.ui.Size.Width * 2} {-timer.data.ui.Size.Height / 2}" :
                        timer.data.ui.Location == UILocation.BottomLeft.ToString() ? "0 0" :
                        timer.data.ui.Location == UILocation.AboveHotbar.ToString() ? $"{-timer.data.ui.Size.Width} {-timer.data.ui.Size.Height + 160}" : "";
                    var oMax =
                        timer.data.ui.Location == UILocation.Top.ToString() ? $"{timer.data.ui.Size.Width} {timer.data.ui.Size.Height - timer.data.ui.Size.Height - 50}" :
                        timer.data.ui.Location == UILocation.LeftTop.ToString() ? $"{timer.data.ui.Size.Width * 2} 0" :
                        timer.data.ui.Location == UILocation.RightTop.ToString() ? $"0 0" :
                        timer.data.ui.Location == UILocation.LeftMiddle.ToString() ? $"{timer.data.ui.Size.Width * 2} {timer.data.ui.Size.Height / 2}" :
                        timer.data.ui.Location == UILocation.RightMiddle.ToString() ? $"0 {timer.data.ui.Size.Height / 2}" :
                        timer.data.ui.Location == UILocation.BottomLeft.ToString() ? $"{timer.data.ui.Size.Width * 2} {timer.data.ui.Size.Height}" :
                        timer.data.ui.Location == UILocation.AboveHotbar.ToString() ? $"{timer.data.ui.Size.Width} {timer.data.ui.Size.Height - timer.data.ui.Size.Height + 160}" : "";
                    #endregion Location
                    container.Add(new CuiPanel
                    {
                        FadeOut = timer.data.ui.FadeDelay,
                        Image = { Color = "0 0 0 0", FadeIn = timer.data.ui.FadeDelay },
                        RectTransform = { AnchorMin = aMin, AnchorMax = aMax, OffsetMin = oMin, OffsetMax = oMax },
                        CursorEnabled = false,
                    }, $"{timer.data.ui.Layer}", $"AdvancedTimers_Box_{Index}");
                }

                public static void Title(CuiElementContainer container, int Index)
                {
                    var timer = RunningTimers[Index];

                    container.Add(new CuiPanel
                    {
                        FadeOut = timer.data.ui.FadeDelay,
                        Image = { Color = timer.data.ui.colors.MessageBackColor, FadeIn = timer.data.ui.FadeDelay },
                        RectTransform =
                        {
                            AnchorMin = $"0 {timer.data.ui.Size.ProgressBarHeight+TextScale(timer.data.ui.Size.CountdownLabelHeight)}",
                            AnchorMax = $"1 1",
                            OffsetMax = "0 0"
                        },
                        CursorEnabled = false,
                    }, $"AdvancedTimers_Box_{Index}", $"AdvancedTimers_TitleBox_{Index}");


                    container.Add(new CuiLabel
                    {
                        FadeOut = timer.data.ui.FadeDelay,
                        RectTransform =
                        {
                            AnchorMin = "0 0",
                            AnchorMax = "1 1",
                            OffsetMax = "0 0"
                        },
                        Text =
                        {
                            FadeIn = timer.data.ui.FadeDelay,
                            Text = timer.data.msg.NewLine ? $"{GetTag(timer.data.msg.Tag)}\n{timer.data.msg.Message}" : $"{GetTag(timer.data.msg.Tag)} {timer.data.msg.Message}",
                            Color = "1 1 1 1",
                            Align = UnityEngine.TextAnchor.MiddleCenter
                        }
                    }, $"AdvancedTimers_TitleBox_{Index}", $"AdvancedTimers_TitleBox_Label_{Index}");
                }

                public static void TimeLabel(CuiElementContainer container, int Index)
                {
                    var timer = RunningTimers[Index];

                    container.Add(new CuiPanel
                    {
                        FadeOut = timer.data.ui.FadeDelay,
                        Image = { Color = timer.data.ui.colors.LabelBackColor, FadeIn = timer.data.ui.FadeDelay, },
                        RectTransform =
                        {
                            AnchorMin = $"0 {timer.data.ui.Size.ProgressBarHeight}",
                            AnchorMax = $"1 {timer.data.ui.Size.ProgressBarHeight+TextScale(timer.data.ui.Size.CountdownLabelHeight)}",
                            OffsetMax = "0 0"
                        },
                        CursorEnabled = false,
                    }, $"AdvancedTimers_Box_{Index}", $"AdvancedTimers_TimeBox_{Index}");

                    container.Add(new CuiLabel
                    {
                        FadeOut = timer.data.ui.FadeDelay,
                        RectTransform =
                        {
                            AnchorMin = "0 0",
                            AnchorMax = "1 1",
                            OffsetMax = "0 0"
                        },
                        Text =
                        {
                            FadeIn = timer.data.ui.FadeDelay,
                            Text = "",
                            FontSize = timer.data.ui.Size.CountdownLabelHeight,
                            Color = "1 1 1 1",
                            Align = UnityEngine.TextAnchor.MiddleCenter
                        }
                    }, $"AdvancedTimers_TimeBox_{Index}", $"AdvancedTimers_TimeBox_Label_{Index}");
                }

                public class ProgressBar
                {
                    public static void Track(CuiElementContainer container, int Index)
                    {
                        var timer = RunningTimers[Index];
                        container.Add(new CuiPanel
                        {
                            FadeOut = timer.data.ui.FadeDelay,
                            Image = { Color = timer.data.ui.colors.PbarTrack, FadeIn = timer.data.ui.FadeDelay, },
                            RectTransform =
                            {
                                AnchorMin = "0 0",
                                AnchorMax = $"1 {timer.data.ui.Size.ProgressBarHeight}",
                                OffsetMax = "0 0"
                            },
                            CursorEnabled = false,
                        }, $"AdvancedTimers_Box_{Index}", $"AdvancedTimers_ProgessbarTrack_{Index}");
                    }
                    public static void Line(CuiElementContainer container, int Index, float value)
                    {
                        var timer = RunningTimers[Index];
                        var i = value > 1 ? 1 : value < 0 ? 0 : value;
                        container.Add(new CuiPanel
                        {
                            FadeOut = timer.data.ui.FadeDelay,
                            Image = { Color = timer.data.ui.colors.PbarLine, FadeIn = timer.data.ui.FadeDelay, },
                            RectTransform =
                            {
                                AnchorMin = "0 0",
                                AnchorMax = $"{i} 1",
                                OffsetMax = "0 0"
                            },
                            CursorEnabled = false,
                        }, $"AdvancedTimers_ProgessbarTrack_{Index}", $"AdvancedTimers_mProgessbarLine_{Index}");
                    }
                }
            }
        }
    }
}