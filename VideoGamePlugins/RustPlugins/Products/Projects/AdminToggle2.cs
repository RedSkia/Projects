using CompanionServer;
using ConVar;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Oxide.Core;
using Oxide.Core.Libraries;
using Oxide.Core.Libraries.Covalence;
using Oxide.Core.Plugins;
using Oxide.Game.Rust.Cui;
using Oxide.Plugins;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;

namespace Oxide.Plugins
{
    [Info("Admin Toggle", "Xray", "3.0.7")]
    [Description("Allows admins with permission to toggle between player & admin mode")]
    public class AdminToggle2 : RustPlugin
    {
        #region Fields
        private const bool DEBUG = false;
        private const string SignatureColor = "faaf19";
        private static AdminToggle2 Instance { get; set; }
        #endregion Fields

        #region Configuration
        private static Configuration _config;
        private class Configuration
        {
            [JsonProperty("Modes")]
            public Modes[] modes { get; set; }
        }
        private class Modes
        {
            [JsonProperty("Permission")]
            public string permission { get; set; }

            [JsonProperty("Priority")]
            public int priority { get; set; }

            [JsonProperty("Master Level CAREFUL (Enabling this feature overrides the priority system & can override any limitations or other modes)")]
            public bool isMaster { get; set; }

            [JsonProperty("Settings")]
            public ModeSettings settings { get; set; }
        }
        private class ModeSettings
        {
            [JsonProperty("Toggle Commands")]
            public string[] commands { get; set; }

            [JsonProperty("Require Reason")]
            public bool requireReason { get; set; }

            [JsonProperty("Separate Inventorys")]
            public bool separateInventorys { get; set; }

            [JsonProperty("Teleport back")]
            public bool teleport { get; set; }

            [JsonProperty("Autorun")]
            public AutoRun autorun { get; set; }

            [JsonProperty("Blocking")]
            public Blocking blocking { get; set; }

            [JsonProperty("Interface")]
            public UserInterface ui { get; set; }

            [JsonProperty("Logging")]
            public Logging logging { get; set; }

            [JsonProperty("Notification")]
            public Notification notification { get; set; }

            [JsonProperty("Outfit (Shortname, SkinID)")]
            public Dictionary<string, ulong> outfit { get; set; }

        }
        #endregion Configuration
        #region Configuration Classes
        private class AutoRun
        {
            [JsonProperty("On Admin")]
            public Admin admin { get; set; }
            public class Admin
            {
                [JsonProperty("Commands")]
                public string[] commands { get; set; }

                [JsonProperty("ThirdParty")]
                public ThirdParty thirdParty { get; set; }
            }

            [JsonProperty("On Player")]
            public Player player { get; set; }
            public class Player
            {
                [JsonProperty("Commands")]
                public string[] commands { get; set; }

                [JsonProperty("ThirdParty")]
                public ThirdParty thirdParty { get; set; }
            }
        }
        private class Blocking
        {
            [JsonProperty("On Admin")]
            public Admin admin { get; set; }
            public class Admin
            {
                [JsonProperty("Commands")]
                public string[] commands { get; set; }

                [JsonProperty("Actions")]
                public Actions actions { get; set; }

                [JsonProperty("ThirdParty")]
                public ThirdParty thirdParty { get; set; }
            }

            [JsonProperty("On Player")]
            public Player player { get; set; }
            public class Player
            {
                [JsonProperty("Commands")]
                public string[] commands { get; set; }

                [JsonProperty("ThirdParty")]
                public ThirdParty thirdParty { get; set; }
            }
        }
        private class Actions
        {

            [JsonProperty("Settings")]
            public Settings settings { get; set; }
            public class Settings
            {
                [JsonProperty("All")]
                public bool all { get; set; }

                [JsonProperty("List")]
                public List list { get; set; }
                public class List
                {
                    [JsonProperty("Can Hurt Players")]
                    public bool canHurt { get; set; }

                    [JsonProperty("Can Demolish")]
                    public bool canDemolish { get; set; }

                    [JsonProperty("Can Build")]
                    public bool canBuild { get; set; }

                    [JsonProperty("Can Drop Items")]
                    public bool canDropItems { get; set; }
                }
            }
        }
        private class ThirdParty
        {
            [JsonProperty("Plugins")]
            public Plugins plugins { get; set; }
            public class Plugins
            {
                [JsonProperty("All")]
                public bool all { get; set; }

                public enum SupportedPlugins
                {
                    Backpacks,
                    Freeze,
                    AdminRadar,
                    Godmode,
                    InventoryViewer,
                    Spectate,
                    Vanish,
                    PlayerAdministration
                };
                [JsonProperty("Specfic Plugin")]
                public Dictionary<SupportedPlugins, bool> list = new Dictionary<SupportedPlugins, bool>();
            }
        }
        private class UserInterface
        {
            public enum MenuDocks
            {
                Right,
                BottomLeft
            };
            public enum PanelDocks
            {
                Top,
                Bottom
            };


            [JsonProperty("Button")]
            public Button button { get; set; }
            public class Button
            {
                [JsonProperty("Enable")]
                public bool enable { get; set; }

                [JsonProperty("Settings")]
                public Settings settings { get; set; }
                public class Settings
                {
                    [JsonProperty("Active Color")]
                    public string activeColor { get; set; }

                    [JsonProperty("Inactive Color")]
                    public string inactiveColor { get; set; }

                    [JsonProperty("Active Text")]
                    public string activeText { get; set; }

                    [JsonProperty("Inactive Text")]
                    public string inactiveText { get; set; }
                }
            }

            [JsonProperty("Panel")]
            public Panel panel { get; set; }
            public class Panel
            {
                [JsonProperty("Enable")]
                public bool enable { get; set; }

                [JsonProperty("Settings")]
                public Settings settings { get; set; }
                public class Settings
                {
                    [JsonProperty("Text")]
                    public string text { get; set; }

                    [JsonProperty("text Color")]
                    public string textColor { get; set; }

                    [JsonProperty("Dock (Bottom, Top)")]
                    public string dock { get; set; }

                    [JsonProperty("Pulse Duration (-1 to disable)")]
                    public int duration { get; set; }
                }
            }

            [JsonProperty("Menu")]
            public Menu menu { get; set; }
            public class Menu
            {
                [JsonProperty("Enable")]
                public bool enable { get; set; }

                [JsonProperty("Settings")]
                public Settings settings { get; set; }
                public class Settings
                {
                    [JsonProperty("Buttons Active Color")]
                    public string activeColor { get; set; }

                    [JsonProperty("Buttons Inactive Color")]
                    public string inactiveColor { get; set; }

                    [JsonProperty("Dock (Right, BottomLeft)")]
                    public string dock { get; set; }
                }
            }
        }
        private class Logging
        {
            [JsonProperty("Exclude")]
            public bool exclude { get; set; }

            [JsonProperty("File")]
            public File file { get; set; }
            public class File
            {
                [JsonProperty("Activation")]
                public Activation activation { get; set; }
                public class Activation
                {
                    [JsonProperty("Enable")]
                    public bool enable { get; set; }

                    [JsonProperty("Settings")]
                    public Settings settings { get; set; }
                    public class Settings
                    {
                        [JsonProperty("Clear On Wipe")]
                        public bool wipeClear { get; set; }
                    }
                }

                [JsonProperty("Time")]
                public Time time { get; set; }
                public class Time
                {
                    [JsonProperty("Enable")]
                    public bool enable { get; set; }

                    [JsonProperty("Settings")]
                    public Settings settings { get; set; }
                    public class Settings
                    {
                        [JsonProperty("Clear On Wipe")]
                        public bool wipeClear { get; set; }
                    }
                }
            }

            [JsonProperty("Discord")]
            public Discord discord { get; set; }
            public class Discord
            {
                [JsonProperty("Enable")]
                public bool enable { get; set; }

                [JsonProperty("Settings")]
                public Settings settings { get; set; }
                public class Settings
                {
                    [JsonProperty("Webhook")]
                    public string webhook { get; set; }

                    [JsonProperty("Embed Color")]
                    public long embedColor { get; set; }
                }
            }
        }
        private class Notification
        {
            [JsonProperty("Chat Icon (STEAMID64)")]
            public object icon { get; set; }

            [JsonProperty("Chat Text Size")]
            public float size { get; set; }

            [JsonProperty("Chat Target Color (HEX)")]
            public string targetColor { get; set; }

            [JsonProperty("Chat Activate Color (HEX)")]
            public string activateColor { get; set; }

            [JsonProperty("Chat Activate Text")]
            public string activateText { get; set; }

            [JsonProperty("Chat Deactivate Color (HEX)")]
            public string deactivateColor { get; set; }

            [JsonProperty("Chat Deactivate Text")]
            public string deactivateText { get; set; }

            [JsonProperty("Global")]
            public Global global { get; set; }
            public class Global
            {
                [JsonProperty("Enable")]
                public bool enable { get; set; }

                [JsonProperty("Settings")]
                public Settings settings { get; set; }
                public class Settings
                {
                    [JsonProperty("Activate Message")]
                    public string activateMessage { get; set; }

                    [JsonProperty("Deactivate Message")]
                    public string deactivateMessage { get; set; }
                }
            }

            [JsonProperty("Self")]
            public Self self { get; set; }
            public class Self
            {
                [JsonProperty("Chat")]
                public Chat chat { get; set; }
                public class Chat
                {
                    [JsonProperty("Enable")]
                    public bool enable { get; set; }

                    [JsonProperty("Settings")]
                    public Settings settings { get; set; }
                    public class Settings
                    {
                        [JsonProperty("Activate Message")]
                        public string activateMessage { get; set; }

                        [JsonProperty("Deactivate Message")]
                        public string deactivateMessage { get; set; }
                    }
                }

                [JsonProperty("Popup")]
                public Popup popup { get; set; }
                public class Popup
                {
                    [JsonProperty("Enable")]
                    public bool enable { get; set; }

                    [JsonProperty("Settings")]
                    public Settings settings { get; set; }
                    public class Settings
                    {
                        [JsonProperty("Activate Message")]
                        public string activateMessage { get; set; }

                        [JsonProperty("Deactivate Message")]
                        public string deactivateMessage { get; set; }
                    }
                }

                [JsonProperty("Sound")]
                public Sound sound { get; set; }
                public class Sound
                {
                    [JsonProperty("Enable")]
                    public bool enable { get; set; }

                    [JsonProperty("Settings")]
                    public Settings settings { get; set; }
                    public class Settings
                    {
                        [JsonProperty("Activate Sound")]
                        public string activateSound { get; set; }
                    }
                }
            }
        }
        #endregion Configuration Classes
        #region Configuration Helpers
        /*On Config Load*/
        protected override void LoadConfig()
        {
            try
            {
                base.LoadConfig();
                _config = Config.ReadObject<Configuration>();
            }
            catch
            {
                PrintWarning("Configuration invalid resetting to default");
                LoadDefaultConfig();
            }
        }
        /*On Config Write New*/
        protected override void LoadDefaultConfig()
        {
            _config = GetDefaultConfig();
            Config.WriteObject<Configuration>(_config, true);
        }
        protected override void SaveConfig() => Config.WriteObject(_config, true);
        private Configuration GetDefaultConfig()
        {
            return new Configuration
            {
                modes = new Modes[]
                {
                    new Modes
                    {
                        permission = "master",
                        priority = 999,
                        isMaster = false,
                        settings = new ModeSettings
                        {
                            requireReason = false,
                            separateInventorys = true,
                            teleport = true,
                            commands = new string[]
                            {
                                "admin",
                                "mode"
                            },
                            outfit = new Dictionary<string, ulong>
                            {
                                { "hoodie", 1234567890 },
                                { "pants", 1234567890 },
                                { "shoes.boots", 1234567890 },
                            },
                            autorun = new AutoRun
                            {
                                admin = new AutoRun.Admin
                                {
                                    commands = new string[]
                                    {
                                        "god 1",
                                        "admintime 12",
                                        "adminclouds 0",
                                        "adminfog 0",
                                        "adminrain 0",
                                    },
                                    thirdParty = new ThirdParty
                                    {
                                        plugins = new ThirdParty.Plugins
                                        {
                                            all = false,
                                            list = new Dictionary<ThirdParty.Plugins.SupportedPlugins, bool>
                                            {
                                                [ThirdParty.Plugins.SupportedPlugins.AdminRadar] = false,
                                                [ThirdParty.Plugins.SupportedPlugins.Godmode] = false,
                                                [ThirdParty.Plugins.SupportedPlugins.Vanish] = false,
                                            },
                                        }
                                    },
                                },
                                player = new AutoRun.Player
                                {
                                    commands = new string[]
                                    {
                                        "god 0",
                                        "admintime -1",
                                        "adminclouds -1",
                                        "adminfog -1",
                                        "adminrain -1",
                                    },
                                    thirdParty = new ThirdParty
                                    {
                                        plugins = new ThirdParty.Plugins
                                        {
                                            all = false,
                                            list = new Dictionary<ThirdParty.Plugins.SupportedPlugins, bool>
                                            {
                                                [ThirdParty.Plugins.SupportedPlugins.AdminRadar] = false,
                                                [ThirdParty.Plugins.SupportedPlugins.Godmode] = false,
                                                [ThirdParty.Plugins.SupportedPlugins.Vanish] = false,
                                            },
                                        }
                                    },
                                },
                            },
                            blocking = new Blocking
                            {
                                admin = new Blocking.Admin
                                {
                                    actions = new Actions
                                    {
                                        settings = new Actions.Settings
                                        {
                                            all = true,
                                            list = new Actions.Settings.List
                                            {
                                                canBuild = false,
                                                canDemolish = false,
                                                canDropItems = false,
                                                canHurt = false,
                                            },
                                        },
                                    },
                                    commands = new string[]
                                    {
                                        "kickall",
                                        "restart",
                                        "stop",
                                        "giveall",
                                        "givebpall",
                                        "kickall",
                                        "kickall",
                                    },
                                    thirdParty = new ThirdParty
                                    {
                                        plugins = new ThirdParty.Plugins
                                        {
                                            all = false,
                                            list = new Dictionary<ThirdParty.Plugins.SupportedPlugins, bool>
                                            {
                                                [ThirdParty.Plugins.SupportedPlugins.Backpacks] = false,
                                                [ThirdParty.Plugins.SupportedPlugins.Freeze] = false,
                                                [ThirdParty.Plugins.SupportedPlugins.AdminRadar] = false,
                                                [ThirdParty.Plugins.SupportedPlugins.Godmode] = false,
                                                [ThirdParty.Plugins.SupportedPlugins.InventoryViewer] = false,
                                                [ThirdParty.Plugins.SupportedPlugins.Spectate] = false,
                                                [ThirdParty.Plugins.SupportedPlugins.Vanish] = false,
                                                [ThirdParty.Plugins.SupportedPlugins.PlayerAdministration] = false,
                                            }
                                        }
                                    }
                                },
                                player = new Blocking.Player
                                {
                                    commands = new string[]
                                    {

                                    },
                                    thirdParty = new ThirdParty
                                    {
                                        plugins = new ThirdParty.Plugins
                                        {
                                            all = true,
                                            list = new Dictionary<ThirdParty.Plugins.SupportedPlugins, bool>
                                            {
                                                [ThirdParty.Plugins.SupportedPlugins.Backpacks] = false,
                                                [ThirdParty.Plugins.SupportedPlugins.Freeze] = false,
                                                [ThirdParty.Plugins.SupportedPlugins.AdminRadar] = false,
                                                [ThirdParty.Plugins.SupportedPlugins.Godmode] = false,
                                                [ThirdParty.Plugins.SupportedPlugins.InventoryViewer] = false,
                                                [ThirdParty.Plugins.SupportedPlugins.Spectate] = false,
                                                [ThirdParty.Plugins.SupportedPlugins.Vanish] = false,
                                                [ThirdParty.Plugins.SupportedPlugins.PlayerAdministration] = false,
                                            }
                                        }
                                    },

                                }

                            },
                            notification = new Notification
                            {
                                icon = -1,
                                activateColor = "#19e119",
                                activateText = "Activated",
                                deactivateColor = "#e11919",
                                deactivateText = "Deactivated",
                                targetColor = "#4be1fa",
                                size = 16,
                                global = new Notification.Global
                                {
                                    enable = false,
                                    settings = new Notification.Global.Settings
                                    {
                                        activateMessage = "{player}{on}{reason}",
                                        deactivateMessage = "{player}{off}"
                                    }
                                },
                                self = new Notification.Self
                                {
                                    chat = new Notification.Self.Chat
                                    {
                                        enable = true,
                                        settings = new Notification.Self.Chat.Settings
                                        {
                                            activateMessage = "{player}{on}",
                                            deactivateMessage = "{player}{off}",
                                        },
                                    },
                                    popup = new Notification.Self.Popup
                                    {
                                        enable = false,
                                        settings = new Notification.Self.Popup.Settings
                                        {
                                            activateMessage = "{player}{on}",
                                            deactivateMessage = "{player}{off}",
                                        }
                                    },
                                    sound = new Notification.Self.Sound
                                    {
                                        enable = false,
                                        settings = new Notification.Self.Sound.Settings
                                        {
                                            activateSound = "assets/prefabs/misc/easter/painted eggs/effects/eggpickup.prefab"
                                        }
                                    }
                                }
                            },
                            logging = new Logging
                            {
                                exclude = true,
                                file = new Logging.File
                                {
                                    activation = new Logging.File.Activation
                                    {
                                        enable = false,
                                        settings = new Logging.File.Activation.Settings
                                        {
                                            wipeClear = true,
                                        }
                                    },
                                    time = new Logging.File.Time
                                    {
                                        enable = false,
                                        settings = new Logging.File.Time.Settings
                                        {
                                            wipeClear = true,
                                        }
                                    }
                                },
                                discord = new Logging.Discord
                                {
                                    enable = false,
                                    settings = new Logging.Discord.Settings
                                    {
                                        webhook = "https://support.discord.com/hc/en-us/articles/228383668-Intro-to-Webhooks",
                                        embedColor = 3315400,
                                    }
                                },
                            },
                            ui = new UserInterface
                            {
                                button = new UserInterface.Button
                                {
                                    enable = true,
                                    settings = new UserInterface.Button.Settings
                                    {
                                        activeColor = "0 0.5 0 0.8",
                                        activeText = "Activated",
                                        inactiveColor = "0.5 0 0 0.8",
                                        inactiveText = "Disabled",
                                    }
                                },
                                panel = new UserInterface.Panel
                                {
                                    enable = false,
                                    settings = new UserInterface.Panel.Settings
                                    {
                                        textColor = "0.88 0.09 0.09 1",
                                        text = "A D M I N   M O D E      A C T I V A T E D",
                                        dock = $"{UserInterface.PanelDocks.Top}",
                                        duration = -1,
                                    }
                                },
                                menu = new UserInterface.Menu
                                {
                                    enable = false,
                                    settings = new UserInterface.Menu.Settings
                                    {
                                        activeColor = "0 0.5 0 0.8",
                                        inactiveColor = "0.5 0 0 0.8",
                                        dock = $"{UserInterface.MenuDocks.BottomLeft}",
                                    }
                                }
                            },
                        }
                    },
                },
            };
        }
        #endregion Configuration Helpers

        #region Initialization
        private void Init()
        {
            Instance = this;
            RegisterHelper.Load.Permission();
            RegisterHelper.Load.Commands();
        }
        private class RegisterHelper
        {
            public class Load
            {
                public static void Permission()
                {
                    foreach (var perm in CoreHelper.Permission.GetAll())
                    {
                        Instance.permission.RegisterPermission($"{Instance.Name}.{perm}", Instance);
                    }
                }
                public static void Commands()
                {
                    foreach (var chatCommand in CommandHelper.GetAll())
                    {
                        Instance.cmd.AddChatCommand(chatCommand, Instance, CoreHelper.Toggle);
                    }
                }
            }
            public class Unload
            {

            }
        }
        #endregion Initialization

        #region Level Helpers
        private class CoreHelper
        {
            internal static void Set(BasePlayer player, SetMode mode, string command, string reason, string[] args)
            {
                var level = CoreHelper.Level.Get(player);
                if (level == null)
                {
                    ChatHelper.SendMessage(player, Instance.LangKey(player, "NoPerm"), ChatHelper.ChatType.self, 0, new ChatHelper.FormatData
                    {
                        command = $"/{command}",
                        reason = reason,
                        player = player,
                    });
                    return;
                }


                Instance.PrintWarning("HERE");
                if (args.Count() > 3 && level.isMaster)
                {
                    bool setArg = args?[0].ToString().ToLower() == "set";
                    var target = BasePlayer.Find(args?[1]);
                    var targetMode = args?[2];
                    var targetPriority = args?[3];
                    if(!setArg) { Instance.SendReply(player, "Invailed Syntax");  return; }
                    if(target == null)
                    {
                        Instance.SendReply(player, "Invailed player");
                        return;
                    }
                    if (targetMode.ToLower() != SetMode.Admin.ToString().ToLower() || targetMode.ToLower() != SetMode.Player.ToString().ToLower()) 
                    {
                        Instance.SendReply(player, "Invailed Mode");
                        return;
                    }
                    if(!_config.modes.Any(x => Convert.ToInt32(targetPriority) == x.priority))
                    {
                        Instance.SendReply(player, "Invailed Priority");
                        return;
                    }
                    
                    Instance.SendReply(player, $"Force Set\n{target.displayName}\n{targetMode}\n{targetPriority}");
                }
                else if (args.Count() > 0 && level.isMaster)
                {
                    Instance.SendReply(player, "Invailed Syntax");
                    return;
                }
                
                /*
                if (!(bool)level?.settings.commands.Contains(command))
                {
                    ChatHelper.SendMessage(player, Instance.LangKey(player, "WorngCommand"), ChatHelper.ChatType.self, 0, new ChatHelper.FormatData
                    {
                        command = $"/{command}",
                        reason = reason,
                        player = player,
                    });
                }*/


                if (!AuthKeys.ContainsKey(player.userID)) { AuthKeys.Add(player.userID, new ToggleCounter()); }
                if (AuthKeys[player.userID].Admin > 1 || AuthKeys[player.userID].Player > 1)
                {
                    //UNLOAD PLAYER ERROR
                    return;
                }
                switch (mode)
                {
                    case SetMode.Player:
                        AuthKeys[player.userID].Player += 1;
                        AuthKeys[player.userID].Admin = 0;
                        Auth.Set(player, SetMode.Player);
                        break;

                    case SetMode.Admin:
                        AuthKeys[player.userID].Admin += 1;
                        AuthKeys[player.userID].Player = 0;
                        Auth.Set(player, SetMode.Admin);
                        break;
                }
            }


            internal protected enum SetMode
            {
                Player,
                Admin
            }
            private static Dictionary<ulong, ToggleCounter> AuthKeys = new Dictionary<ulong, ToggleCounter>();
            private class ToggleCounter
            {
                public int Admin { get; set; }
                public int Player { get; set; }
            }
            internal static void Toggle(BasePlayer player, string command, string[] args)
            {
                switch (player?.IsAdmin)
                {
                    case true:
                        Set(player, SetMode.Player, command, null, args);
                        break;
                    case false:
                        Set(player, SetMode.Admin, command, null, args);
                        break;
                }
            }
            private class Auth
            {
                public static SetMode Get(BasePlayer player)
                {
                    if (player.IsAdmin) return SetMode.Admin;
                    else return SetMode.Player;
                }
                public static void Set(BasePlayer player, SetMode mode)
                {
                    switch (mode)
                    {
                        case SetMode.Player:
                            ServerUsers.Set(player.userID, ServerUsers.UserGroup.None, player.displayName, "");
                            player.SetPlayerFlag(BasePlayer.PlayerFlags.IsAdmin, false);
                            player.Connection.authLevel = 0;
                            ServerUsers.Save();
                            break;

                        case SetMode.Admin:
                            ServerUsers.Set(player.userID, ServerUsers.UserGroup.Owner, player.displayName, "");
                            player.SetPlayerFlag(BasePlayer.PlayerFlags.IsAdmin, true);
                            player.Connection.authLevel = 2;
                            ServerUsers.Save();
                            break;
                    }
                }
            }
            public class Permission
            {
                public static string[] GetAll() => _config.modes.Select(x => x.permission).ToArray();
            }
            internal class Level
            {
                public static Modes Get(BasePlayer player)
                {
                    foreach (var mode in _config.modes)
                    {
                        if (Instance.permission.UserHasPermission(player.UserIDString, $"{Instance.Name}.{mode.permission}") && !_config.modes.Any(x => x.priority > mode.priority))
                        {
                            return mode;
                        }
                    }
                    return null;
                }
            }
        }
        #endregion Level Helpers

        #region Commands
        private class CommandHelper
        {
            public static string[] GetAll() => _config.modes.SelectMany(x => x.settings.commands).ToArray();
        }
        #endregion Commands



        #region Chat Helpers
        private class ChatHelper
        {
            public class FormatData
            {
                public BasePlayer player { get; set; }
                public string reason { get; set; }
                public string on { get; set; }
                public string off { get; set; }
                public string command { get; set; }
            }
            public enum ChatType
            {
                self,
                global
            }

            public static string TextEncodeing(double TextSize, string HexColor, string text, bool bold)
            {
                var s = TextSize <= 0 ? 1 : TextSize;
                var c = HexColor.StartsWith("#") ? HexColor : $"#{HexColor}";
                if (bold) { return $"<size={s}><color={c}><b>{text}</b></color></size>"; }
                else { return $"<size={s}><color={c}>{text}</color></size>"; }
            }
            public static string FormatChat(FormatData data, string message)
            {
                return message
                    .Replace("{player}", data?.player.displayName)
                    .Replace("{id}", data?.player.UserIDString)
                    .Replace("{level}", CoreHelper.Level.Get(data?.player).permission)
                    .Replace("{reason}", data?.reason)
                    .Replace("{on}", data?.on)
                    .Replace("{off}", data?.off)
                    .Replace("{command}", data?.command);
            }
            public static void SendMessage(BasePlayer player, string message, ChatType chat, ulong icon, FormatData data)
            {
                switch (chat)
                {
                    case ChatType.global:
                        player.SendConsoleCommand("chat.add", 2, icon, FormatChat(new FormatData
                        {
                            reason = data.reason,
                            on = data.on,
                            off = data.off,
                            command = data.command,
                            player = data.player,
                        }, $"{TextEncodeing(16, SignatureColor,Instance.Name,true)}{TextEncodeing(16, "ffffff", $": {message}", false)}"));
                        break;
                    case ChatType.self:
                        Instance.PrintToChat("2");
                        player.SendConsoleCommand("chat.add", 0, icon, FormatChat(new FormatData
                        {
                            reason = data.reason,
                            on = data.on,
                            off = data.off,
                            command = data.command,
                            player = data.player,
                        }, $"{TextEncodeing(16, SignatureColor, Instance.Name, true)}{TextEncodeing(16, "ffffff", $": {message}", false)}"));
                        break;
                }
            }
        }
        #endregion Chat Helpers
        #region Localization
        private string LangKey(BasePlayer player, string Key) => lang.GetMessage(Key, this, player.UserIDString);
        protected override void LoadDefaultMessages()
        {
            lang.RegisterMessages(new Dictionary<string, string>
            {
                ["ForceSetLevel"] = $"{ChatHelper.TextEncodeing(14,"ffffff", $"You have been swiched level to", false)} {ChatHelper.TextEncodeing(14, SignatureColor, "{level}", true)}",
                ["SetLevelSelf"] = $"{ChatHelper.TextEncodeing(14, "ffffff", $"You swiched level to", false)} {ChatHelper.TextEncodeing(14, SignatureColor, "{level}", true)}",
                ["SetLevelGlobal"] = $"{ChatHelper.TextEncodeing(14, SignatureColor, "{player}", true)} {ChatHelper.TextEncodeing(14, "ffffff", $"has swiched level to", false)} {ChatHelper.TextEncodeing(14, SignatureColor, "{level}", true)}",
                ["WorngCommand"] = $"{ChatHelper.TextEncodeing(14, SignatureColor, "{player}", true)} {ChatHelper.TextEncodeing(14, "ffffff", "Cannot use command:", false)} {ChatHelper.TextEncodeing(14, SignatureColor, "{command}", true)} {ChatHelper.TextEncodeing(14, "ffffff", "with current admin level", false)} {ChatHelper.TextEncodeing(14, SignatureColor, "{level}", true)}",
                ["NoPerm"] = $"{ChatHelper.TextEncodeing(14, "ffffff", $"Insufficient permission to use command:", false)} {ChatHelper.TextEncodeing(14, SignatureColor, "{command}", true)}",
            }, this, "en");
        }
        #endregion Localization
    }
}