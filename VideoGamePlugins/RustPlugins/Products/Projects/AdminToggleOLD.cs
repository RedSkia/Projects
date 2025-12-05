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
using System.Threading.Tasks;
using UnityEngine;

namespace Oxide.Plugins
{
    /*
    * SOFTWARE LICENCE AGREEMENT OF AdminToggle
    *   The following Software License Agreement constitutes a legally binding agreement between You, (the "Buyer") and infinitynet.dk/Xray (the "Vendor").
    *   This Software License Agreement by infinitynet.dk/Xray (the "Vendor") grants the right to use AdminToggle (the "Software") respecting the license limitations and restrictions below.
    *   By acquiring a copy of AdminToggle (the "Software"), You, the original licensee is legally bound to the license (the "License") Disclaimers, Limitations and Restrictions.
    *   Failing to comply with any of the terms of this Software license Agreement (the "License"), the license ("License") shall be terminated.
    *   On termination of the license ("License"), You are legally obliged to stop all use of AdminToggle (the "Software") immediately.
    *   Please read this Software License Agreement (the "License") thoroughly before acquiring a copy of AdminToggle (the "Software").
    * 
    * GRANT OF LICENCE
    *   This agreement (the "License") by infinitynet.dk/Xray (the "Vendor") grants the Licensee exclusive, non-transferable, personal-use of the license (the "License") to use this AdminToggle (the "Software").
    * 
    * LICENSE DISCLAIMERS, LIMITATIONS AND RESTRICTIONS
    *   AdminToggle (the "Software") is provided "AS IS". infinitynet.dk/Xray (the "Vendor") will support, maintain and repair AdminToggle (the "Software"), but without any timeframe or guarantees in general.
    *   AdminToggle (the "Software") may not be modified, changed, adapted, reverse-engineered, de-compiled, Reproduced, Distributed, Resold, Published or shared without the authority of the infinitynet.dk/Xray (the "Vendor")
    *   AdminToggle (the "Software") license (the "License") is restricted to a maximum of 5 installations per license (the "License"). The Licensee may only install AdminToggle (the "Software") on computer installations owned or leased by the Licensee.
    */

    [Info("Admin Toggle", "Xray", "2.0.11")]
    [Description("Allows admins with permission to toggle between player & admin mode")]
    public class AdminToggleOLD : RustPlugin
    {
        [PluginReference] private readonly Plugin Backpacks, Freeze, AdminRadar, Godmode, InventoryViewer, Spectate, Vanish, PlayerAdministration;

        #region Globals
        public static AdminToggleOLD Instance { get; set; }
        private string MasterPerm { get; set; }
        private string LimitedPerm { get; set; }

        private const string SignatureColor = "faaf19";

        #endregion Globals

        #region Configuration
        private static Configuration _config;
        private class Configuration
        {
            #region CORE
            [JsonProperty("Core Features")]
            public CoreConfig CORE { get; set; }
            public class CoreConfig
            {
                #region Permissions
                [JsonProperty("Permissions")]
                public CorePermissions Permissions { get; set; }
                public class CorePermissions
                {
                    [JsonProperty("Master Mode (No Limitations)")]
                    public string master { get; set; }

                    [JsonProperty("Limited Mode (Configurable limitations)")]
                    public string limited { get; set; }
                }
                #endregion Permissions

                #region Chat Command
                [JsonProperty("Chat Command")]
                public CoreCommands Commands { get; set; }
                public class CoreCommands
                {
                    [JsonProperty("Commands To Toggle")]
                    public string[] commands { get; set; }
                }
                #endregion Chat Command

                #region Modes
                [JsonProperty("Modes")]
                public CoreModes Modes { get; set; }
                public class CoreModes
                {
                    #region Master
                    [JsonProperty("Master")]
                    public CoreModeMaster Master { get; set; }
                    public class CoreModeMaster
                    {
                        [JsonProperty("Settings")]
                        public CoreModeMasterSettings Settings { get; set; }
                        public class CoreModeMasterSettings
                        {
                            [JsonProperty("Require Reason To Toggle")]
                            public bool requireReason { get; set; }

                            [JsonProperty("Separate Inventorys For Master & Player Mode")]
                            public bool separateInventorys { get; set; }

                            [JsonProperty("Upon Exiting Master Mode Teleport Back To Known Location")]
                            public bool teleport { get; set; }

                            [JsonProperty("Keep Auth Level On Disconnect")]
                            public bool keepAuth { get; set; }

                            [JsonProperty("Day Time Only")]
                            public bool dayOnly { get; set; }

                            [JsonProperty("Third-party Autoload Plugins")]
                            public bool autoload { get; set; }


                            [JsonProperty("Use Special Custom Outfit")]
                            public bool outfitSpecial { get; set; }

                            [JsonProperty("Use Normal Custom Outfit")]
                            public bool outfitNormal { get; set; }

                            [JsonProperty("Third-party Blocking Plugins (In player mode)")]
                            public bool blocking { get; set; }

                            [JsonProperty("Exclude Logging")]
                            public bool excludeLog { get; set; }
                        }
                    }
                    #endregion Master

                    #region Limited
                    [JsonProperty("Limited")]
                    public CoreModeLimited Limited { get; set; }
                    public class CoreModeLimited
                    {
                        [JsonProperty("Enable")]
                        public bool Enabled { get; set; }

                        [JsonProperty("Settings")]
                        public CoreModeLimitedSettings Settings { get; set; }
                        public class CoreModeLimitedSettings
                        {
                            [JsonProperty("Require Reason To Toggle")]
                            public bool requireReason { get; set; }

                            [JsonProperty("Separate Inventorys For Limited & Player Mode")]
                            public bool separateInventorys { get; set; }

                            [JsonProperty("Upon Exiting Limited Mode Teleport Back To Known Location")]
                            public bool teleport { get; set; }

                            [JsonProperty("Keep Auth Level On Disconnect")]
                            public bool keepAuth { get; set; }

                            [JsonProperty("Day Time Only")]
                            public bool dayOnly { get; set; }

                            [JsonProperty("Third-party Autoload Plugins")]
                            public bool autoload { get; set; }

                            [JsonProperty("Third-party Blocking Plugins (In player mode)")]
                            public bool blocking { get; set; }

                            [JsonProperty("Use Custom Outfit")]
                            public bool outfitNormal { get; set; }
                        }
                    }
                    #endregion Limited
                }
                #endregion Modes

                #region Interface
                [JsonProperty("Interface")]
                public CoreInterface Interface { get; set; }
                public class CoreInterface
                {
                    [JsonProperty("Enable Button")]
                    public bool enableButton { get; set; }

                    [JsonProperty("Enable Panel")]
                    public bool enablePanel { get; set; }

                    [JsonProperty("Enable Menu")]
                    public bool enableMenu { get; set; }
                }
                #endregion Interface

                #region Notification
                [JsonProperty("Notification")]
                public CoreNotification Notification { get; set; }
                public class CoreNotification
                {
                    [JsonProperty("Global")]
                    public CoreNotificationGlobal Global { get; set; }
                    public class CoreNotificationGlobal
                    {
                        [JsonProperty("Enable Broadcast")]
                        public bool enableBroadcast { get; set; }
                    }


                    [JsonProperty("Self")]
                    public CoreNotificationSelf Self { get; set; }
                    public class CoreNotificationSelf
                    {
                        [JsonProperty("Enable Chat Notification")]
                        public bool enableChat { get; set; }

                        [JsonProperty("Enable Popup Notification")]
                        public bool enablePopup { get; set; }

                        [JsonProperty("Enable Sound Notification")]
                        public bool enableSound { get; set; }
                    }
                }
                #endregion Notification

                #region Logging
                [JsonProperty("Logging")]
                public CoreLogging Logging { get; set; }
                public class CoreLogging
                {
                    [JsonProperty("Enable Discord Logging")]
                    public bool enableDiscord { get; set; }

                    [JsonProperty("Enable File Logging")]
                    public bool enableFile { get; set; }

                    [JsonProperty("Enable Time Logging")]
                    public bool enableTime { get; set; }
                }
                #endregion Logging
            }
            #endregion CORE

            #region INTERFACE
            [JsonProperty("Interface")]
            public ConfigInterface INTERFACE { get; set; }
            public class ConfigInterface
            {
                [JsonProperty("Button")]
                public ConfigInterfaceButton Button { get; set; }
                public class ConfigInterfaceButton
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


                [JsonProperty("Panel")]
                public ConfigInterfacePanel Panel { get; set; }
                public class ConfigInterfacePanel
                {
                    [JsonProperty("Text")]
                    public string text { get; set; }

                    [JsonProperty("text Color")]
                    public string textColor { get; set; }

                    [JsonProperty("Panel Dock (Bottom, Top)")]
                    public string dock { get; set; }

                    [JsonProperty("Pulse Duration (-1 to disable)")]
                    public int duration { get; set; }
                }



                [JsonProperty("Menu")]
                public ConfigInterfaceMenu Menu { get; set; }
                public class ConfigInterfaceMenu
                {
                    [JsonProperty("Buttons Active Color")]
                    public string activeColor { get; set; }

                    [JsonProperty("Buttons Inactive Color")]
                    public string inactiveColor { get; set; }

                    [JsonProperty("Menu Dock (Right, BottomLeft)")]
                    public string dock { get; set; }
                }
            }
            #endregion INTERFACE

            #region OUTFIT
            [JsonProperty("Outfit")]
            public ConfigOutfit OUTFIT { get; set; }
            public class ConfigOutfit
            {
                [JsonProperty("Special (Master Only)")]
                public ConfigOutfitSpecial OutfitSpecial { get; set; }
                public class ConfigOutfitSpecial
                {
                    [JsonProperty("Special Outfit (Shortname, SkinID)")]
                    public Dictionary<string, ulong> outfit { get; set; }
                }

                [JsonProperty("Normal")]
                public ConfigOutfitNormal OutfitNormal { get; set; }
                public class ConfigOutfitNormal
                {
                    [JsonProperty("Normal Outfit (Shortname, SkinID)")]
                    public Dictionary<string, ulong> outfit { get; set; }
                }
            }
            #endregion OUTFIT

            #region NOTIFICATION
            [JsonProperty("Notification")]
            public ConfigNotification NOTIFICATION { get; set; }
            public class ConfigNotification
            {
                [JsonProperty("Custom Chat Notification Icon (MUST BE STEAMID64) (-1 to disable)")]
                public object icon { get; set; }

                [JsonProperty("Global")]
                public ConfigNotificationGlobal Global { get; set; }
                public class ConfigNotificationGlobal
                {
                    [JsonProperty("Activate Message")]
                    public string activateMessage { get; set; }

                    [JsonProperty("Deactivate Message")]
                    public string deactivateMessage { get; set; }
                }

                [JsonProperty("Self")]
                public ConfigNotificationSelf Self { get; set; }
                public class ConfigNotificationSelf
                {
                    [JsonProperty("Chat")]
                    public ConfigNotificationSelfChat Chat { get; set; }
                    public class ConfigNotificationSelfChat
                    {
                        [JsonProperty("Activate Message")]
                        public string activateMessage { get; set; }

                        [JsonProperty("Deactivate Message")]
                        public string deactivateMessage { get; set; }
                    }

                    [JsonProperty("Popup")]
                    public ConfigNotificationSelfPopup Popup { get; set; }
                    public class ConfigNotificationSelfPopup
                    {
                        [JsonProperty("Activate Message")]
                        public string activateMessage { get; set; }

                        [JsonProperty("Deactivate Message")]
                        public string deactivateMessage { get; set; }
                    }

                    [JsonProperty("Sound")]
                    public ConfigNotificationSelfSound Sound { get; set; }
                    public class ConfigNotificationSelfSound
                    {
                        [JsonProperty("Activate Sound")]
                        public string activateSound { get; set; }
                    }
                }
            }
            #endregion NOTIFICATION

            #region LOGGING
            [JsonProperty("Logging")]
            public ConfigLogging LOGGING { get; set; }
            public class ConfigLogging
            {
                [JsonProperty("StaffTime")]
                public ConfigLoggingStaffTime StaffTime { get; set; }
                public class ConfigLoggingStaffTime
                {
                    [JsonProperty("Reset StaffTime Data On Wipe")]
                    public bool resetOnWipe { get; set; }
                }

                [JsonProperty("Discord")]
                public ConfigLoggingDiscord Discord { get; set; }
                public class ConfigLoggingDiscord
                {
                    [JsonProperty("Webhook")]
                    public string webHook { get; set; }

                    [JsonProperty("Embed Color")]
                    public long embedColor { get; set; }
                }
            }
            #endregion LOGGING

            #region ADMIN
            [JsonProperty("Admin")]
            public ConfigAdmin ADMIN { get; set; }
            public class ConfigAdmin
            {
                [JsonProperty("AutoLoad")]
                public ConfigAdminAutoLoad AutoLoad { get; set; }
                public class ConfigAdminAutoLoad
                {
                    [JsonProperty("Third-Party Plugins")]
                    public ConfigAdminAutoLoadPlugins Plugins { get; set; }
                    public class ConfigAdminAutoLoadPlugins
                    {

                        [JsonProperty("Autoload All Plugins")]
                        public bool autoloadAll { get; set; }

                        [JsonProperty("Autoload Specfic Plugins")]
                        public ConfigAdminAutoLoadPluginsList List { get; set; }
                        public class ConfigAdminAutoLoadPluginsList
                        {
                            [JsonProperty("Umod")]
                            public ConfigAdminAutoLoadPluginsListUmod Umod { get; set; }
                            public class ConfigAdminAutoLoadPluginsListUmod
                            {

                                [JsonProperty("Autoload Admin Radar")]
                                public bool loadAdminRadar { get; set; }

                                [JsonProperty("Autoload Godmode")]
                                public bool loadGodmode { get; set; }

                                [JsonProperty("Autoload Vanish")]
                                public bool loadVanish { get; set; }
                            }
                        }
                    }
                }



                #region LIMTIED MODE
                [JsonProperty("Limited Mode")]
                public ConfigLimitedMode LIMITED { get; set; }
                public class ConfigLimitedMode
                {
                    [JsonProperty("Limitations")]
                    public ConfigLimitedModeLimitations Limitations { get; set; }
                    public class ConfigLimitedModeLimitations
                    {
                        [JsonProperty("Actions")]
                        public ConfigLimitedModeLimitationsActions Actions { get; set; }
                        public class ConfigLimitedModeLimitationsActions
                        {
                            [JsonProperty("Enable All Actions")]
                            public bool enableAll { get; set; }

                            [JsonProperty("List")]
                            public ConfigLimitedModeLimitationsActionsList list { get; set; }
                            public class ConfigLimitedModeLimitationsActionsList
                            {
                                [JsonProperty("Can Grant Auth Levels")]
                                public bool grantAuth { get; set; }

                                [JsonProperty("Can Revoke Auth Levels")]
                                public bool revokeAuth { get; set; }

                                [JsonProperty("Can Ban Players")]
                                public bool canBan { get; set; }

                                [JsonProperty("Can Unban Players")]
                                public bool canUnBan { get; set; }

                                [JsonProperty("Can Kick Players")]
                                public bool canKick { get; set; }

                                [JsonProperty("Can Hurt Players")]
                                public bool canHurt { get; set; }

                                [JsonProperty("Can Demolish")]
                                public bool canDemolish { get; set; }

                                [JsonProperty("Can Build")]
                                public bool canBuild { get; set; }

                                [JsonProperty("Can Spawn Items")]
                                public bool canSpawnItems { get; set; }

                                [JsonProperty("Can Spawn Entities")]
                                public bool canSpawnEntities { get; set; }

                                [JsonProperty("Can Drop Items")]
                                public bool canDropItems { get; set; }

                                [JsonProperty("Can Change Server Time")]
                                public bool canChangeTime { get; set; }

                                [JsonProperty("Can Broadcast")]
                                public bool canBroadcast { get; set; }
                            }
                        }


                        [JsonProperty("Third-party Plugins")]
                        public ConfigLimitedModeLimitationsAPlugins Plugins { get; set; }
                        public class ConfigLimitedModeLimitationsAPlugins
                        {
                            [JsonProperty("Enable All Plugins")]
                            public bool enableAll { get; set; }

                            [JsonProperty("List")]
                            public ConfigLimitedModeLimitationsActionsList list { get; set; }
                            public class ConfigLimitedModeLimitationsActionsList
                            {
                                [JsonProperty("Umod")]
                                public ConfigLimitedModeLimitationsActionsListUmod Umod { get; set; }
                                public class ConfigLimitedModeLimitationsActionsListUmod
                                {
                                    [JsonProperty("Use Admin Radar")]
                                    public bool useAdminRadar { get; set; }

                                    [JsonProperty("Use Freeze")]
                                    public bool useFreeze { get; set; }

                                    [JsonProperty("Use Godmode")]
                                    public bool useGodmode { get; set; }

                                    [JsonProperty("Use Inventory Viewer")]
                                    public bool useInventoryViewer { get; set; }

                                    [JsonProperty("Use Player Administration")]
                                    public bool usePlayerAdministration { get; set; }

                                    [JsonProperty("Use Spectate")]
                                    public bool useSpectate { get; set; }

                                    [JsonProperty("Use Vanish")]
                                    public bool useVanish { get; set; }
                                }
                            }
                        }
                    }
                }
                #endregion LIMTIED MODE

            }

            #endregion ADMIN

            #region PLAYER
            [JsonProperty("Player")]
            public ConfigPlayer PLAYER { get; set; }
            public class ConfigPlayer
            {
                [JsonProperty("Blocking")]
                public ConfigPlayerBlocking Blocking { get; set; }
                public class ConfigPlayerBlocking
                {
                    [JsonProperty("Third-Party Plugins")]
                    public ConfigPlayerBlockingPlugins Plugins { get; set; }
                    public class ConfigPlayerBlockingPlugins
                    {
                        [JsonProperty("Block All Plugins")]
                        public bool blockAll { get; set; }

                        [JsonProperty("Block Specfic Plugins")]
                        public ConfigPlayerBlockingPluginsList List { get; set; }
                        public class ConfigPlayerBlockingPluginsList
                        {
                            [JsonProperty("Umod")]
                            public ConfigPlayerBlockingPluginsListUmod Umod { get; set; }
                            public class ConfigPlayerBlockingPluginsListUmod
                            {
                                [JsonProperty("Block Admin Radar")]
                                public bool blockAdminRadar { get; set; }

                                [JsonProperty("Block Freeze")]
                                public bool blockFreeze { get; set; }

                                [JsonProperty("Block Godmode")]
                                public bool blockGodmode { get; set; }

                                [JsonProperty("Block Inventory Viewer")]
                                public bool blockInventoryViewer { get; set; }

                                [JsonProperty("Block Player Administration")]
                                public bool blockPlayerAdministration { get; set; }

                                [JsonProperty("Block Spectate")]
                                public bool blockSpectate { get; set; }

                                [JsonProperty("Block Vanish")]
                                public bool blockVanish { get; set; }
                            }

                        }
                    }
                }
            }
            #endregion PLAYER
        }

        //On Config Load
        protected override void LoadConfig()
        {
            base.LoadConfig();
            try
            {
                _config = Config.ReadObject<Configuration>();
            }
            catch
            {
                PrintWarning("Configuration invalid resetting to default");
                LoadDefaultConfig();
            }
        }
        //On Config Write New
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
                CORE = new Configuration.CoreConfig
                {
                    Commands = new Configuration.CoreConfig.CoreCommands
                    {
                        commands = new string[]
                        {
                            "admin"
                        },
                    },
                    Permissions = new Configuration.CoreConfig.CorePermissions
                    {
                        master = "master.use",
                        limited = "limited.use",
                    },
                    Modes = new Configuration.CoreConfig.CoreModes
                    {
                        Master = new Configuration.CoreConfig.CoreModes.CoreModeMaster
                        {
                            Settings = new Configuration.CoreConfig.CoreModes.CoreModeMaster.CoreModeMasterSettings
                            {
                                autoload = true,
                                blocking = true,
                                dayOnly = true,
                                excludeLog = true,
                                outfitNormal = false,
                                outfitSpecial = false,
                                requireReason = false,
                                separateInventorys = true,
                                teleport = true,
                                keepAuth = false,
                            }
                        },
                        Limited = new Configuration.CoreConfig.CoreModes.CoreModeLimited
                        {
                            Enabled = false,
                            Settings = new Configuration.CoreConfig.CoreModes.CoreModeLimited.CoreModeLimitedSettings
                            {
                                autoload = false,
                                blocking = true,
                                dayOnly = false,
                                outfitNormal = false,
                                requireReason = true,
                                separateInventorys = true,
                                teleport = true,
                                keepAuth = false,
                            }
                        },
                    },
                    Interface = new Configuration.CoreConfig.CoreInterface
                    {
                        enableButton = true,
                        enableMenu = false,
                        enablePanel = false,
                    },
                    Notification = new Configuration.CoreConfig.CoreNotification
                    {
                        Global = new Configuration.CoreConfig.CoreNotification.CoreNotificationGlobal
                        {
                            enableBroadcast = false,
                        },
                        Self = new Configuration.CoreConfig.CoreNotification.CoreNotificationSelf
                        {
                            enableChat = true,
                            enablePopup = false,
                            enableSound = false,
                        },
                    },
                    Logging = new Configuration.CoreConfig.CoreLogging
                    {
                        enableFile = true,
                        enableDiscord = false,
                        enableTime = false,
                    },
                },
                INTERFACE = new Configuration.ConfigInterface
                {
                    Button = new Configuration.ConfigInterface.ConfigInterfaceButton
                    {
                        activeText = "Activated",
                        inactiveText = "Disabled",
                        activeColor = "0 0.5 0 0.8",
                        inactiveColor = "0.5 0 0 0.8",
                    },
                    Panel = new Configuration.ConfigInterface.ConfigInterfacePanel
                    {
                        dock = "Bottom",
                        duration = -1,
                        text = "A D M I N   M O D E      A C T I V A T E D",
                        textColor = "0.88 0.09 0.09 1",
                    },
                    Menu = new Configuration.ConfigInterface.ConfigInterfaceMenu
                    {
                        dock = "BottomLeft",
                        activeColor = "0 0.5 0 0.8",
                        inactiveColor = "0.5 0 0 0.8",
                    }
                },
                ADMIN = new Configuration.ConfigAdmin
                {
                    AutoLoad = new Configuration.ConfigAdmin.ConfigAdminAutoLoad
                    {
                        Plugins = new Configuration.ConfigAdmin.ConfigAdminAutoLoad.ConfigAdminAutoLoadPlugins
                        {
                            autoloadAll = false,
                            List = new Configuration.ConfigAdmin.ConfigAdminAutoLoad.ConfigAdminAutoLoadPlugins.ConfigAdminAutoLoadPluginsList
                            {
                                Umod = new Configuration.ConfigAdmin.ConfigAdminAutoLoad.ConfigAdminAutoLoadPlugins.ConfigAdminAutoLoadPluginsList.ConfigAdminAutoLoadPluginsListUmod
                                {
                                    loadAdminRadar = false,
                                    loadGodmode = false,
                                    loadVanish = false,
                                },
                            },
                        },
                    },
                    LIMITED = new Configuration.ConfigAdmin.ConfigLimitedMode
                    {
                        Limitations = new Configuration.ConfigAdmin.ConfigLimitedMode.ConfigLimitedModeLimitations
                        {
                            Actions = new Configuration.ConfigAdmin.ConfigLimitedMode.ConfigLimitedModeLimitations.ConfigLimitedModeLimitationsActions
                            {
                                enableAll = false,
                                list = new Configuration.ConfigAdmin.ConfigLimitedMode.ConfigLimitedModeLimitations.ConfigLimitedModeLimitationsActions.ConfigLimitedModeLimitationsActionsList
                                {
                                    grantAuth = false,
                                    revokeAuth = false,
                                    canBan = false,
                                    canBroadcast = false,
                                    canBuild = false,
                                    canChangeTime = false,
                                    canDemolish = false,
                                    canDropItems = false,
                                    canHurt = false,
                                    canKick = false,
                                    canSpawnEntities = false,
                                    canSpawnItems = false,
                                    canUnBan = false,
                                },
                            },
                            Plugins = new Configuration.ConfigAdmin.ConfigLimitedMode.ConfigLimitedModeLimitations.ConfigLimitedModeLimitationsAPlugins
                            {
                                enableAll = false,
                                list = new Configuration.ConfigAdmin.ConfigLimitedMode.ConfigLimitedModeLimitations.ConfigLimitedModeLimitationsAPlugins.ConfigLimitedModeLimitationsActionsList
                                {
                                    Umod = new Configuration.ConfigAdmin.ConfigLimitedMode.ConfigLimitedModeLimitations.ConfigLimitedModeLimitationsAPlugins.ConfigLimitedModeLimitationsActionsList.ConfigLimitedModeLimitationsActionsListUmod
                                    {
                                        useAdminRadar = false,
                                        useFreeze = false,
                                        useGodmode = false,
                                        useInventoryViewer = false,
                                        useSpectate = false,
                                        useVanish = false,
                                    },
                                },
                            },
                        }
                    }
                },
                PLAYER = new Configuration.ConfigPlayer
                {
                    Blocking = new Configuration.ConfigPlayer.ConfigPlayerBlocking
                    {
                        Plugins = new Configuration.ConfigPlayer.ConfigPlayerBlocking.ConfigPlayerBlockingPlugins
                        {
                            blockAll = true,
                            List = new Configuration.ConfigPlayer.ConfigPlayerBlocking.ConfigPlayerBlockingPlugins.ConfigPlayerBlockingPluginsList
                            {
                                Umod = new Configuration.ConfigPlayer.ConfigPlayerBlocking.ConfigPlayerBlockingPlugins.ConfigPlayerBlockingPluginsList.ConfigPlayerBlockingPluginsListUmod
                                {
                                    blockAdminRadar = false,
                                    blockPlayerAdministration = false,
                                    blockFreeze = false,
                                    blockGodmode = false,
                                    blockInventoryViewer = false,
                                    blockSpectate = false,
                                    blockVanish = false,
                                },
                            },
                        },
                    },
                },
                NOTIFICATION = new Configuration.ConfigNotification
                {
                    icon = -1,
                    Global = new Configuration.ConfigNotification.ConfigNotificationGlobal
                    {
                        activateMessage = "<size=16>{player} <color=#19e119>Activated</color> admin{noreasonend} with reason: {reason}</size>",
                        deactivateMessage = "<size=16>{player} <color=#e11919>Deactivated</color> admin</size>",
                    },
                    Self = new Configuration.ConfigNotification.ConfigNotificationSelf
                    {
                        Chat = new Configuration.ConfigNotification.ConfigNotificationSelf.ConfigNotificationSelfChat
                        {
                            activateMessage = "<size=16><color=#19e119>Activated</color></size>",
                            deactivateMessage = "<size=16><color=#e11919>Disabled</color></size>",
                        },
                        Popup = new Configuration.ConfigNotification.ConfigNotificationSelf.ConfigNotificationSelfPopup
                        {
                            activateMessage = "<size=16><color=#19e119>Activated</color></size>",
                            deactivateMessage = "<size=16><color=#e11919>Disabled</color></size>",
                        },
                        Sound = new Configuration.ConfigNotification.ConfigNotificationSelf.ConfigNotificationSelfSound
                        {
                            activateSound = "assets/prefabs/misc/easter/painted eggs/effects/eggpickup.prefab",
                        },
                    },
                },
                LOGGING = new Configuration.ConfigLogging
                {
                    StaffTime = new Configuration.ConfigLogging.ConfigLoggingStaffTime
                    {
                        resetOnWipe = false,
                    },
                    Discord = new Configuration.ConfigLogging.ConfigLoggingDiscord
                    {
                        webHook = "https://support.discord.com/hc/en-us/articles/228383668-Intro-to-Webhooks",
                        embedColor = 3315400
                    },
                },
                OUTFIT = new Configuration.ConfigOutfit
                {
                    OutfitSpecial = new Configuration.ConfigOutfit.ConfigOutfitSpecial
                    {
                        outfit = new Dictionary<string, ulong>
                        {
                            { "hoodie", 1234567890 },
                            { "pants", 1234567890 },
                            { "shoes.boots", 1234567890 }
                        },
                    },
                    OutfitNormal = new Configuration.ConfigOutfit.ConfigOutfitNormal
                    {
                        outfit = new Dictionary<string, ulong>
                        {
                            { "hoodie", 1234567890 },
                            { "pants", 1234567890 },
                            { "shoes.boots", 1234567890 }
                        },
                    },
                }
            };
        }
        #endregion Configuration

        #region Init
        private void Loaded()
        {
            Instance = this;

            try
            {
                MasterPerm = $"{Instance.Name}.{_config.CORE.Permissions.master}";
                LimitedPerm = $"{Instance.Name}.{_config.CORE.Permissions.limited}";

                permission.RegisterPermission(MasterPerm, this);
                if (_config.CORE.Modes.Limited.Enabled) { permission.RegisterPermission(LimitedPerm, this); }

            } catch { PrintWarning("Failed To Register Permissions - Try Deleteing Config (Outdated ?)"); }

            try
            {
                //Register All Chat Commands
                foreach (var command in _config.CORE.Commands.commands)
                {
                    cmd.AddChatCommand(command, this, ChatToggleAdmin);
                }
            } 
            catch
            {
                PrintWarning("Failed To Register Chat Commands - Try Deleteing Config (Outdated ?)");
            }

  
            try
            {
                //Load Staff Time
                StaffLoadFile();

                timer.Once(5f, () =>
                {
                    thirdParty.Load();
                    thirdParty.Subscribe();
                });
            } catch { }


            //Third-party Update Checker
            webrequest.Enqueue("https://codefling.com/capi/file-1803/?do=apicall", null, (code, response) =>
            {
                if (code != 200 || response == null) { return; }

                Root fileData = JsonConvert.DeserializeObject<Root>(response);
                if (S2V(fileData.file.file_version) > Version) { PrintWarning($"Update Available: ({Version} -> {fileData.file.file_version})"); }

            }, this, RequestMethod.GET);
        }
        #endregion Init

        #region WebRequest Codefling (Third Party)
        private Core.VersionNumber S2V(string v)
        {
            string[] parts = v.Split('.');
            return new Core.VersionNumber(Convert.ToInt16(parts[0]), Convert.ToInt16(parts[1]), Convert.ToInt16(parts[2]));
        }
        private class Root
        {
            public File file { get; set; }
        }
        private class File
        {
            public string file_id { get; set; }
            public string file_name { get; set; }
            public FileImage file_image { get; set; }
            public string file_version { get; set; }
            public string file_author { get; set; }
            public string file_price { get; set; }
            public string file_file_1 { get; set; }
        }
        private class FileImage
        {
            public string name { get; set; }
            public string url { get; set; }
            public string size { get; set; }
        }
        #endregion WebRequest Codefling (Third Party)

        #region Permissions
        private class HasPerm
        {
            public static bool MasterPerm(BasePlayer player) => Instance.permission.UserHasPermission(player.UserIDString, Instance.MasterPerm);
            public static bool LimitedPerm(BasePlayer player) => Instance.permission.UserHasPermission(player.UserIDString, Instance.LimitedPerm);
            public static bool AnyPerm(BasePlayer player) => MasterPerm(player) || LimitedPerm(player);
            public static bool BothPerm(BasePlayer player) => MasterPerm(player) && LimitedPerm(player);
        }
        #endregion Permissions

        #region Level Functions
        private class LevelFunctions
        {
            public static bool RequireReason(BasePlayer player)
            {
                if (!HasPerm.AnyPerm(player)) { return false; }
                //Both Handler Override To Master
                if (HasPerm.BothPerm(player) && _config.CORE.Modes.Master.Settings.requireReason) { return true; }
                if (!HasPerm.BothPerm(player) && HasPerm.MasterPerm(player) && _config.CORE.Modes.Master.Settings.requireReason) { return true; }
                if (!HasPerm.BothPerm(player) && HasPerm.LimitedPerm(player) && _config.CORE.Modes.Limited.Enabled && _config.CORE.Modes.Limited.Settings.requireReason) { return true; }
                return false; /*Else Handler*/
            }

            public static bool SeparateInventorys(BasePlayer player)
            {
                if (!HasPerm.AnyPerm(player)) { return false; }
                //Both Handler Override To Master
                if (HasPerm.BothPerm(player) && _config.CORE.Modes.Master.Settings.separateInventorys) { return true; }
                if (!HasPerm.BothPerm(player) && HasPerm.MasterPerm(player) && _config.CORE.Modes.Master.Settings.separateInventorys) { return true; }
                if (!HasPerm.BothPerm(player) && HasPerm.LimitedPerm(player) && _config.CORE.Modes.Limited.Enabled && _config.CORE.Modes.Limited.Settings.separateInventorys) { return true; }
                return false; /*Else Handler*/
            }

            public static bool TeleportBack(BasePlayer player)
            {
                if (!HasPerm.AnyPerm(player)) { return false; }
                //Both Handler Override To Master
                if (HasPerm.BothPerm(player) && _config.CORE.Modes.Master.Settings.teleport) { return true; }
                if (!HasPerm.BothPerm(player) && HasPerm.MasterPerm(player) && _config.CORE.Modes.Master.Settings.teleport) { return true; }
                if (!HasPerm.BothPerm(player) && HasPerm.LimitedPerm(player) && _config.CORE.Modes.Limited.Enabled && _config.CORE.Modes.Limited.Settings.teleport) { return true; }
                return false; /*Else Handler*/
            }

            public static bool SpecialOutfit(BasePlayer player)
            {
                //Both Handler Override To Master
                if (HasPerm.BothPerm(player) && _config.CORE.Modes.Master.Settings.outfitSpecial) { return true; }
                if (!HasPerm.BothPerm(player) && HasPerm.MasterPerm(player) && _config.CORE.Modes.Master.Settings.outfitSpecial) { return true; }
                return false; /*Else Handler*/
            }

            public static bool Outfit(BasePlayer player)
            {
                if (!HasPerm.AnyPerm(player)) { return false; }
                //Both Handler Override To Master
                if (HasPerm.BothPerm(player) && !_config.CORE.Modes.Master.Settings.outfitSpecial && _config.CORE.Modes.Master.Settings.outfitNormal) { return true; }
                if (!HasPerm.BothPerm(player) && HasPerm.MasterPerm(player) && !_config.CORE.Modes.Master.Settings.outfitSpecial && _config.CORE.Modes.Master.Settings.outfitNormal) { return true; }
                if (!HasPerm.BothPerm(player) && HasPerm.LimitedPerm(player) && _config.CORE.Modes.Limited.Enabled && _config.CORE.Modes.Limited.Settings.outfitNormal) { return true; }
                return false; /*Else Handler*/
            }

            public static bool DayOnly(BasePlayer player)
            {
                if (!HasPerm.AnyPerm(player)) { return false; }
                //Both Handler Override To Master
                if (HasPerm.BothPerm(player) && _config.CORE.Modes.Master.Settings.dayOnly) { return true; }
                if (!HasPerm.BothPerm(player) && HasPerm.MasterPerm(player) && _config.CORE.Modes.Master.Settings.dayOnly) { return true; }
                if (!HasPerm.BothPerm(player) && HasPerm.LimitedPerm(player) && _config.CORE.Modes.Limited.Enabled && _config.CORE.Modes.Limited.Settings.dayOnly) { return true; }
                return false; /*Else Handler*/
            }

            public static bool AutoLoadPlugins(BasePlayer player)
            {
                if (!HasPerm.AnyPerm(player)) { return false; }
                //Both Handler Override To Master
                if (HasPerm.BothPerm(player) && _config.CORE.Modes.Master.Settings.autoload) { return true; }
                if (!HasPerm.BothPerm(player) && HasPerm.MasterPerm(player) && _config.CORE.Modes.Master.Settings.autoload) { return true; }
                if (!HasPerm.BothPerm(player) && HasPerm.LimitedPerm(player) && _config.CORE.Modes.Limited.Enabled && _config.CORE.Modes.Limited.Settings.autoload) { return true; }
                return false; /*Else Handler*/
            }

            public static bool BlockPlayerPlugins(BasePlayer player)
            {
                if (!HasPerm.AnyPerm(player)) { return false; }
                //Both Handler Override To Master
                if (HasPerm.BothPerm(player) && _config.CORE.Modes.Master.Settings.blocking) { return true; }
                if (!HasPerm.BothPerm(player) && HasPerm.MasterPerm(player) && _config.CORE.Modes.Master.Settings.blocking) { return true; }
                if (!HasPerm.BothPerm(player) && HasPerm.LimitedPerm(player) && _config.CORE.Modes.Limited.Enabled && _config.CORE.Modes.Limited.Settings.blocking) { return true; }
                return false; /*Else Handler*/
            }

            public static bool CanLog(BasePlayer player)
            {
                if (_config.CORE.Logging.enableFile || _config.CORE.Logging.enableDiscord || _config.CORE.Logging.enableTime)
                {
                    if (!HasPerm.AnyPerm(player)) { return false; }
                    //Both Handler Override To Master
                    if (HasPerm.BothPerm(player) && !_config.CORE.Modes.Master.Settings.excludeLog) { return true; }
                    if (!HasPerm.BothPerm(player) && HasPerm.MasterPerm(player) && !_config.CORE.Modes.Master.Settings.excludeLog) { return true; }
                    if (!HasPerm.BothPerm(player) && HasPerm.LimitedPerm(player) && _config.CORE.Modes.Limited.Enabled) { return true; }
                    return false; /*Else Handler*/
                }
                return false;
            }

            public static bool KeepAuth(BasePlayer player)
            {
                if (!HasPerm.AnyPerm(player)) { return false; }
                //Both Handler Override To Master
                if (HasPerm.BothPerm(player) && _config.CORE.Modes.Master.Settings.keepAuth) { return true; }
                if (!HasPerm.BothPerm(player) && HasPerm.MasterPerm(player) && _config.CORE.Modes.Master.Settings.keepAuth) { return true; }
                if (!HasPerm.BothPerm(player) && HasPerm.LimitedPerm(player) && _config.CORE.Modes.Limited.Enabled && _config.CORE.Modes.Limited.Settings.keepAuth) { return true; }
                return false; /*Else Handler*/
            }
        }
        #endregion Level Functions

        private static Dictionary<ulong, StaffTime> _staffTime = new Dictionary<ulong, StaffTime>();
        #region Staff Time
        private class StaffTime
        {
            [JsonProperty("Name")]
            public string name { get; set; }

            [JsonProperty("Level (Master, Limited)")]
            public string level { get; set; }

            [JsonProperty("Time Spent In Admin Mode (hours, minutes, seconds)")]
            public TimeSpan playtime { get; set; }

            [JsonProperty("Last Activation")]
            public long lastActivation { get; set; }
        }

        private static void StaffTimeSave(BasePlayer player, bool force)
        {
            if (_config.CORE.Logging.enableTime)
            {
                if (!HasPerm.AnyPerm(player)) { return; }

                if (!_staffTime.ContainsKey(player.userID))
                {
                    _staffTime.Add(player.userID, new StaffTime()
                    {
                        name = player.displayName,
                        level = HasPerm.MasterPerm(player) ? "Master" : HasPerm.LimitedPerm(player) ? "Limited" : null,
                        lastActivation = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                        playtime = new TimeSpan(),
                    });
                }

                if (_staffTime.ContainsKey(player.userID))
                {
                    if (player.IsAdmin) { _staffTime[player.userID].lastActivation = DateTimeOffset.UtcNow.ToUnixTimeSeconds(); }

                    if (!player.IsAdmin || force)
                    {
                        if (LevelFunctions.CanLog(player))
                        {
                            //Update Name IF Changed
                            if (player.displayName != _staffTime[player.userID].name) { _staffTime[player.userID].name = player.displayName; }

                            //Update Level IF Changed
                            _staffTime[player.userID].level = HasPerm.MasterPerm(player) ? "Master" : HasPerm.LimitedPerm(player) ? "Limited" : null;

                            DateTimeOffset last = DateTimeOffset.FromUnixTimeSeconds(_staffTime[player.userID].lastActivation);

                            var result = DateTimeOffset.UtcNow.Subtract(last);

                            _staffTime[player.userID].playtime += TimeSpan.FromSeconds(Math.Round(result.TotalSeconds, 0));
                            _staffTime[player.userID].lastActivation = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                        }
                    }
                }
            }
        }

        private static void StaffSaveFile()
        {
            if (_config.CORE.Logging.enableTime) { Interface.Oxide.DataFileSystem.WriteObject($"{Instance.Name}/StaffTime", _staffTime); }
        }
        private static void StaffLoadFile()
        {
            if (!_config.CORE.Logging.enableTime) { return; }

            if (Interface.Oxide.DataFileSystem.ExistsDatafile($"{Instance.Name}/StaffTime"))
            {
                try
                {
                    _staffTime = Interface.Oxide.DataFileSystem.ReadObject<Dictionary<ulong, StaffTime>>($"{Instance.Name}/StaffTime");
                }
                catch
                {
                    Instance.PrintWarning("Staff Time File invalid resetting to default");
                    Interface.Oxide.DataFileSystem.WriteObject($"{Instance.Name}/StaffTime", new Dictionary<ulong, StaffTime>());
                }
            }
            else
            {
                Instance.PrintWarning("Staff Time File invalid resetting to default");
                Interface.Oxide.DataFileSystem.WriteObject($"{Instance.Name}/StaffTime", new Dictionary<ulong, StaffTime>());
            }
        }
        #endregion Staff Time

        #region Admin Functions

        private Dictionary<ulong, SwichingState> PlayerState = new Dictionary<ulong, SwichingState>();
        private class SwichingState
        {
            public bool IsSwiching { get; set; }
            public DateTime LastActivate { get; set; }
            public Timer t { get; set; }
        }

        Dictionary<ulong, Vector3> PlayerLocations = new Dictionary<ulong, Vector3>();

        //Replaced By Commands List
        //[ChatCommand("admin")]
        private void ChatToggleAdmin(BasePlayer player, string command, string[] args) => player.SendConsoleCommand("admin.toggle", args);

        [ConsoleCommand("admin.toggle")]
        private void ConsoleToggleAdmin(ConsoleSystem.Arg arg) => ToggleMode.Toggle(arg.Player(), arg?.Args);

        [ConsoleCommand("at.force")]
        private void ConsoleToggleForce(ConsoleSystem.Arg arg) => UnloadPlayer(arg.Player());

        #region Auth Toggle
        private class ToggleMode
        {
            const int LastActivateCooldown = 5;

            public static void Toggle(BasePlayer player, string[] args)
            {
                if (!HasPerm.AnyPerm(player)) { DirectMessage(player, "Missing permission"); return; };
                if (LevelFunctions.TeleportBack(player) && !Instance.PlayerLocations.ContainsKey(player.userID))
                { Instance.PlayerLocations.Add(player.userID, player.transform.position); }

                
                if (player.IsAdmin) { SetPlayer(player); }
                else { SetAdmin(player, args); }
            }

            public static void SetAdmin(BasePlayer player, string[] args)
            {
                if (player.IsCrawling() || player.IsDead() || player.IsWounded()) { DirectMessage(player, "Cannot set Admin Mode while downed"); return; }


                //Block Spam
                if (!Instance.PlayerState.ContainsKey(player.userID)) { Instance.PlayerState.Add(player.userID, new SwichingState { IsSwiching = false }); ; }
                if (Instance.PlayerState[player.userID].IsSwiching) { return; }

                //Remove Unused Perm
                if (!_config.CORE.Modes.Limited.Enabled) Instance.permission.RevokeUserPermission(player.UserIDString, Instance.LimitedPerm);


                if (Instance.PlayerState[player.userID].IsSwiching == false)
                {
                    //Have Perm
                    if (!HasPerm.AnyPerm(player)) { DirectMessage(player, "Missing permission"); return; };

                    //Log & require when reason If Enabled
                    if (LevelFunctions.RequireReason(player) && args == null) { DirectMessage(player, "Missing argument"); return; }
                    else if (LevelFunctions.RequireReason(player) && args != null)
                    {
                        List<string> textInput = new List<string>();
                        for (int i = 0; i < args.Count(); i++) { if (!textInput.Contains(args[i])) { textInput.Add(args[i]?.ToString()); } }
                        var reason = String.Join(" ", textInput.ToArray());

                        Instance.LogAction(player, reason);
                        Instance.PrintToDiscord(player, reason);
                        Instance.ServerBroadcast(player,
                            _config.NOTIFICATION.Global.activateMessage.
                            Replace("{player}", player.displayName).
                            Replace("{reason}", reason).
                            Replace("{noreasonend}", null));
                    }

                    //Cooldown
                    if (DateTime.Now < Instance.PlayerState[player.userID].LastActivate.AddSeconds(LastActivateCooldown)) { DirectMessage(player, "Please wait for server to update ConVars before toggling"); return; }
                    else { Instance.PlayerState[player.userID].LastActivate = DateTime.Now; }


                    Instance.PlayerState[player.userID].IsSwiching = true; //Swiching State

                    //Set Player Auth
                    ServerUsers.Set(player.userID, ServerUsers.UserGroup.Owner, player.displayName, "AdminToggle Set Admin");
                    player.SetPlayerFlag(BasePlayer.PlayerFlags.IsAdmin, true);
                    player.Connection.authLevel = 2;
                    ServerUsers.Save();
                    ServerUsers.Save();


                    #region Notify & Log
                    //Log with NO reason If Enabled
                    if (!LevelFunctions.RequireReason(player) && _config.CORE.Logging.enableDiscord) { Instance.PrintToDiscord(player, "**Feature Disabled**"); }
                    if (!LevelFunctions.RequireReason(player) && _config.CORE.Logging.enableFile) { Instance.LogAction(player, null); }
                    if (!LevelFunctions.RequireReason(player) && _config.CORE.Notification.Global.enableBroadcast)
                    {
                        var message = _config.NOTIFICATION.Global.activateMessage.Contains("{player}") ?
                            _config.NOTIFICATION.Global.activateMessage.Replace("{player}", player.displayName) : null;
                        message = message.Substring(0, message.IndexOf("{noreasonend}"));
                        Instance.ServerBroadcast(player, message);
                    }

                    //Notify User/Global If Enabled
                    if (!_config.CORE.Notification.Global.enableBroadcast)
                    {
                        if (_config.CORE.Notification.Self.enableChat) { DirectMessage(player, _config.NOTIFICATION.Self.Chat.activateMessage); }
                        Instance.ShowPopup(player, _config.NOTIFICATION.Self.Popup.activateMessage, 3);
                    }
                    #endregion Notify & Log

                    //Save Location 
                    if (LevelFunctions.TeleportBack(player) && Instance.PlayerLocations.ContainsKey(player.userID))
                    { Instance.PlayerLocations[player.userID] = player.transform.position; }


                    //Draw If Enabled
                    UI.Draw.Button(player);
                    UI.PanelStartPulsing(player);
                    UI.Draw.Menu(player);

                    //Load If Enabled Supports LevelFunctions
                    thirdParty.LoadPlugins(player);


                    //Save Inventory If Enabled
                    if (LevelFunctions.SeparateInventorys(player))
                    {
                        Inventory.Items.Caller.SaveInventory(player);
                        Inventory.Items.Caller.ClearInventory(player);
                    }

                    //Lock Backpack If SeparateInventorys
                    if (thirdParty.isLoaded.UseBackpacks && LevelFunctions.SeparateInventorys(player))
                    {
                        var MyBackpack = Instance.Backpacks.Call<Dictionary<ulong, ItemContainer>>("API_GetExistingBackpacks");
                        if (MyBackpack.ContainsKey(player.userID)) { try { MyBackpack[player.userID].SetLocked(true); } catch { } }
                    }



                    //Set Outfit If Enabled Supports LevelFunctions
                    Instance.SetAdminOutfit(player);


                    //Set Admin Weather for admins If Enabled
                    if (LevelFunctions.DayOnly(player))
                    {
                        Instance.timer.Once(1f, () =>
                        {
                            player.SendConsoleCommand("admintime 12");
                            player.SendConsoleCommand("adminclouds 0");
                            player.SendConsoleCommand("adminfog 0");
                            player.SendConsoleCommand("adminrain 0");
                        });
                    }


                    //Play Sound If Enabled
                    Instance.PlaySound(player, _config.NOTIFICATION.Self.Sound.activateSound);

                    //Save Time Spend In AdminMode If Enabled
                    StaffTimeSave(player, false);

                    Interface.CallHook("AdminToggleOnAdmin", player); //Custom Hook

                    Instance.PlayerState[player.userID].IsSwiching = false; //Swiching State
                }
            }

            public static void SetPlayer(BasePlayer player)
            {
                //Block Spam
                if (!Instance.PlayerState.ContainsKey(player.userID)) { Instance.PlayerState.Add(player.userID, new SwichingState { IsSwiching = false }); ; }
                if (Instance.PlayerState[player.userID].IsSwiching) { return; }

                //Remove Unused Perm
                if (!_config.CORE.Modes.Limited.Enabled) Instance.permission.RevokeUserPermission(player.UserIDString, Instance.LimitedPerm);


                if (Instance.PlayerState[player.userID].IsSwiching == false)
                {
                    //Have Perm
                    if (!HasPerm.AnyPerm(player)) { DirectMessage(player, "Missing permission"); return; };

                    //Cooldown
                    if (DateTime.Now < Instance.PlayerState[player.userID].LastActivate.AddSeconds(LastActivateCooldown)) { DirectMessage(player, "Please wait for server to update ConVars before toggling"); return; }
                    else { Instance.PlayerState[player.userID].LastActivate = DateTime.Now; }


                    Instance.PlayerState[player.userID].IsSwiching = true; //Swiching State


                    //Note
                    DirectMessage(player, "<size=13>NOTE: When toggling back to Player mode \nDo NOT activate noclip/fly-mode in the next 5 seconds \nOr you risk being Kicked by Easy Anti-Cheat.</size>");

                    try
                    {

                        //Teleport Player
                        if (LevelFunctions.TeleportBack(player) && Instance.PlayerLocations.ContainsKey(player.userID))
                        {
                            if (Instance.PlayerLocations[player.userID].y > TerrainMeta.HeightMap.GetHeight(Instance.PlayerLocations[player.userID]))
                            {
                                player.Teleport(new Vector3(
                                    Instance.PlayerLocations[player.userID].x,
                                    Instance.PlayerLocations[player.userID].y,
                                    Instance.PlayerLocations[player.userID].z));
                            }
                            else
                            {
                                player.Teleport(new Vector3(
                                    Instance.PlayerLocations[player.userID].x,
                                    TerrainMeta.HeightMap.GetHeight(Instance.PlayerLocations[player.userID]),
                                    Instance.PlayerLocations[player.userID].z));
                            }
                        }
                        else
                        {
                            if (player.transform.position.y > TerrainMeta.HeightMap.GetHeight(player.transform.position))
                            {
                                player.Teleport(new Vector3(
                                    player.transform.position.x,
                                    TerrainMeta.HeightMap.GetHeight(player.transform.position) + 0.5f,
                                    player.transform.position.z));
                            }
                            else
                            {
                                player.Teleport(new Vector3(
                                    player.transform.position.x,
                                    player.transform.position.y + 0.5f,
                                    player.transform.position.z));
                            }
                        }

                        Instance.PlayerState[player.userID].t = Instance.timer.Once(0.25f, () =>
                        {
                            //Disable Noclip If Flying
                            if (player.IsFlying) { player.SendConsoleCommand("noclip"); }
                        });


                    }
                    catch { }

                    //Reset Admin Weather for admins If Enabled
                    if (LevelFunctions.DayOnly(player))
                    {
                        //Reset Admin Weather
                        player.SendConsoleCommand("admintime -1");
                        player.SendConsoleCommand("adminclouds -1");
                        player.SendConsoleCommand("adminfog -1");
                        player.SendConsoleCommand("adminrain -1");
                    }

                    //Timer must be because of teleport
                    Instance.PlayerState[player.userID].t = Instance.timer.Once(0.75f, () =>
                    {
                        //MUST NOT FLY
                        if (!player.IsOnGround() || player.IsFlying)
                        {
                            Instance.PlayerState[player.userID].IsSwiching = false; //Swiching State FAILED
                            DirectMessage(player, "Cannot set Player Mode while flying");
                            Instance.PlayerState[player.userID].LastActivate = DateTime.Now.AddSeconds(-LastActivateCooldown); //Reset Cooldown
                            return;
                        }


                        //Set Player Auth
                        ServerUsers.Set(player.userID, ServerUsers.UserGroup.None, player.displayName, "AdminToggle Set Player");
                        player.SetPlayerFlag(BasePlayer.PlayerFlags.IsAdmin, false);
                        player.Connection.authLevel = 0;
                        ServerUsers.Save();


                        if (!_config.CORE.Notification.Global.enableBroadcast)
                        {
                            //Notify User If Enabled
                            if (_config.CORE.Notification.Self.enableChat) { DirectMessage(player, _config.NOTIFICATION.Self.Chat.deactivateMessage); }
                            Instance.ShowPopup(player, _config.NOTIFICATION.Self.Popup.deactivateMessage, 3);
                        }
                        else
                        {
                            Instance.ServerBroadcast(player, _config.NOTIFICATION.Global.deactivateMessage.Replace("{player}", player.displayName));
                        }

                        //Draw If Enabled
                        UI.Draw.Button(player);
                        UI.PanelStopPulsing(player);
                        UI.Draw.Menu(player);

                        //Disable If Enabled
                        thirdParty.UnloadPlugins(player, false);


                        //Remove Admin Outfit If Enabled
                        if (LevelFunctions.Outfit(player) || LevelFunctions.SpecialOutfit(player))
                        {
                            if (Inventory.Items.Caller.GetItemCount(player) > 0)
                            {
                                foreach (var i in player.inventory.containerWear.itemList) { i.Remove(); }
                            }
                        }


                        //Restore Inventory If Enabled
                        if (LevelFunctions.SeparateInventorys(player))
                        {
                            Inventory.Items.Caller.ClearInventory(player);
                            Inventory.Items.Caller.RestoreInventory(player);
                        }
                        Inventory.Items.Unlock(player); //Unlock Inventory


                        //Unlock Backpack If SeparateInventorys
                        if (thirdParty.isLoaded.UseBackpacks && LevelFunctions.SeparateInventorys(player))
                        {
                            var MyBackpack = Instance.Backpacks.Call<Dictionary<ulong, ItemContainer>>("API_GetExistingBackpacks");
                            if (MyBackpack.ContainsKey(player.userID)) { try { MyBackpack[player.userID].SetLocked(false); } catch { } }
                        }

                        //Save Time Spend In AdminMode If Enabled
                        StaffTimeSave(player, false);


                        Interface.CallHook("AdminToggleOnPlayer", player); //Custom Hook

                        Instance.PlayerState[player.userID].IsSwiching = false; //Swiching State

                        Instance.PlayerState[player.userID].t.Destroy();

                        Instance.PlayerLocations.Remove(player.userID); //Clear Location
                    });
                }
            }
            //ONLY BY UNLOAD
            public static void ForceSetPlayer(BasePlayer player)
            {

                //Not Have Perm Return Instant
                if (!HasPerm.AnyPerm(player)) { return; };



                //Unlock Backpack If SeparateInventorys
                if (thirdParty.isLoaded.UseBackpacks && LevelFunctions.SeparateInventorys(player))
                {
                    var MyBackpack = Instance.Backpacks.Call<Dictionary<ulong, ItemContainer>>("API_GetExistingBackpacks");
                    if (MyBackpack.ContainsKey(player.userID)) { try { MyBackpack[player.userID].SetLocked(false); } catch { } }
                }


                //Remove Admin Outfit If Enabled
                if (LevelFunctions.Outfit(player) || LevelFunctions.SpecialOutfit(player))
                {
                    if(Inventory.Items.Caller.GetItemCount(player) > 0)
                    {
                        foreach (var i in player.inventory.containerWear.itemList) { i.Remove(); }
                    } 
                }


                //Restore Inventory If Enabled
                if (LevelFunctions.SeparateInventorys(player))
                {
                    Inventory.Items.Caller.ClearInventory(player);
                    Inventory.Items.Caller.RestoreInventory(player);
                }
                Inventory.Items.Unlock(player); //Unlock Inventory

                //Disable If Enabled
                thirdParty.UnloadPlugins(player, true);


                //Reset Admin Weather for admins If Enabled
                if (LevelFunctions.DayOnly(player))
                {
                    //Reset Admin Weather
                    player.SendConsoleCommand("admintime -1");
                    player.SendConsoleCommand("adminclouds -1");
                    player.SendConsoleCommand("adminfog -1");
                    player.SendConsoleCommand("adminrain -1");
                }

        



                //MUST NOT FLY
                if (!player.IsOnGround() || player.IsFlying) { DirectMessage(player, "Cannot set Player Mode while flying"); return; }



                //Note
                DirectMessage(player, "<size=13>NOTE: When toggling back to Player mode \nDo NOT activate noclip/fly-mode in the next 5 seconds \nOr you risk being Kicked by Easy Anti-Cheat.</size>");

                if(!LevelFunctions.KeepAuth(player))
                {
                    //Set Player Auth
                    ServerUsers.Set(player.userID, ServerUsers.UserGroup.None, player.displayName, "AdminToggle Set Player FORCED");
                    player.SetPlayerFlag(BasePlayer.PlayerFlags.IsAdmin, false);
                    player.Connection.authLevel = 0;
                    ServerUsers.Save();
                }
            }
        }
        #endregion Auth Toggle

        public void SetAdminOutfit(BasePlayer player)
        {
            //Only Sets Outfit If Enabled
            if (LevelFunctions.SpecialOutfit(player))
            {
                try
                {
                    foreach (var i in _config.OUTFIT.OutfitSpecial.outfit)
                    {
                        try
                        {
                            Item item = ItemManager.CreateByName(i.Key, 1, i.Value);

                            player.inventory.GiveItem(item);
                            item.MoveToContainer(player.inventory.containerWear);
                        }
                        catch { }
                    }
                    Inventory.Items.LockContainer(player.inventory.containerWear);
                }
                catch { }
            }
            else if (LevelFunctions.Outfit(player))
            {
                try
                {
                    foreach (var i in _config.OUTFIT.OutfitNormal.outfit)
                    {
                        try
                        {
                            Item item = ItemManager.CreateByName(i.Key, 1, i.Value);
                            player.inventory.GiveItem(item);
                            item.MoveToContainer(player.inventory.containerWear);
                        }
                        catch { }
                    }
                    Inventory.Items.LockContainer(player.inventory.containerWear);
                }
                catch { }
            }
        }
        public void PlaySound(BasePlayer player, string prefab)
        {
            if (_config.CORE.Notification.Self.enableSound && !string.IsNullOrEmpty(prefab))
            {
                var effect = new Effect(prefab, player, 0, Vector3.zero, Vector3.forward);
                EffectNetwork.Send(effect, player.net.connection);
                effect.Clear(true);
            }
        }
        #endregion Admin Functions

        #region Oxide Hooks Limitations
        //On Client Command
        object OnServerCommand(ConsoleSystem.Arg arg)
        {
            var Command = arg.cmd.Name.ToLower();

            //Custom Hook For PlayerAdministrationCommand
            if (Command == "padmin")
            {
                if (thirdParty.perms.has.umod.PlayerAdministration(arg.Player()))
                {
                    thirdParty.isEnabled.PlayerAdministration = true; UI.Draw.Menu(arg.Player());
                }
                else { DirectMessage(arg.Player(), $"<size=13>Third-Party Permission Missing: {thirdParty.perms.has.umod.playeradministrationPerm}</size>"); }
            }

            //Custom Hook For PlayerAdministrationCommand
            if (Command == "closeui") { thirdParty.isEnabled.PlayerAdministration = false; UI.Draw.Menu(arg.Player()); }

            if (arg.Player() != null && arg.Player().IsConnected)
            {
                if (HasPerm.MasterPerm(arg.Player())) { return null; /*Execute*/ }

                switch (Command)
                {
                    #region Commands


                    case "ownerid":
                    case "moderatorid":
                        if (HasPerm.LimitedPerm(arg.Player()) && arg.Player().IsAdmin && _config.CORE.Modes.Limited.Enabled &&
                            (_config.ADMIN.LIMITED.Limitations.Actions.enableAll || _config.ADMIN.LIMITED.Limitations.Actions.list.grantAuth))
                        { return null; /*Execute*/ }
                        else { DirectMessage(arg.Player(), "Insufficient permission cannot grant auth level"); return false; /*Block*/}


                    case "removeowner":
                    case "removemoderator":
                        if (HasPerm.LimitedPerm(arg.Player()) && arg.Player().IsAdmin && _config.CORE.Modes.Limited.Enabled &&
                            (_config.ADMIN.LIMITED.Limitations.Actions.enableAll || _config.ADMIN.LIMITED.Limitations.Actions.list.revokeAuth))
                        { return null; /*Execute*/ }
                        else { DirectMessage(arg.Player(), "Insufficient permission cannot revoke auth level"); return false; /*Block*/}


                    case "givearm":
                    case "giving":
                    case "gave":
                    case "give":
                        if (HasPerm.LimitedPerm(arg.Player()) && arg.Player().IsAdmin && _config.CORE.Modes.Limited.Enabled &&
                            (_config.ADMIN.LIMITED.Limitations.Actions.enableAll || _config.ADMIN.LIMITED.Limitations.Actions.list.canSpawnItems))
                        { return null; /*Execute*/ }
                        else { DirectMessage(arg.Player(), "Insufficient permission cannot preform give"); return false; /*Block*/}



                    case "spawn":
                        if (HasPerm.LimitedPerm(arg.Player()) && arg.Player().IsAdmin && _config.CORE.Modes.Limited.Enabled &&
                            (_config.ADMIN.LIMITED.Limitations.Actions.enableAll || _config.ADMIN.LIMITED.Limitations.Actions.list.canSpawnEntities))
                        { return null; /*Execute*/ }
                        else { DirectMessage(arg.Player(), "Insufficient permission cannot preform spawn"); return false; /*Block*/}



                    case "time":
                        if (HasPerm.LimitedPerm(arg.Player()) && arg.Player().IsAdmin && _config.CORE.Modes.Limited.Enabled &&
                            (_config.ADMIN.LIMITED.Limitations.Actions.enableAll || _config.ADMIN.LIMITED.Limitations.Actions.list.canChangeTime))
                        { return null; /*Execute*/ }
                        else { DirectMessage(arg.Player(), "Insufficient permission cannot perform time change"); return false; /*Block*/}



                    case "say":
                        if (HasPerm.LimitedPerm(arg.Player()) && arg.Player().IsAdmin && _config.CORE.Modes.Limited.Enabled &&
                            (_config.ADMIN.LIMITED.Limitations.Actions.enableAll || _config.ADMIN.LIMITED.Limitations.Actions.list.canBroadcast))
                        { return null; /*Execute*/ }
                        else { DirectMessage(arg.Player(), "Insufficient permission cannot perform broadcast"); return false; /*Block*/}



                    case "ban":
                        if (HasPerm.LimitedPerm(arg.Player()) && arg.Player().IsAdmin && _config.CORE.Modes.Limited.Enabled &&
                            (_config.ADMIN.LIMITED.Limitations.Actions.enableAll || _config.ADMIN.LIMITED.Limitations.Actions.list.canBan))
                        { return null; /*Execute*/ }
                        else { DirectMessage(arg.Player(), "Insufficient permission cannot perform ban"); return false; /*Block*/}


                    case "unban":
                        if (HasPerm.LimitedPerm(arg.Player()) && arg.Player().IsAdmin && _config.CORE.Modes.Limited.Enabled &&
                            (_config.ADMIN.LIMITED.Limitations.Actions.enableAll || _config.ADMIN.LIMITED.Limitations.Actions.list.canUnBan))
                        { return null; /*Execute*/ }
                        else { DirectMessage(arg.Player(), "Insufficient permission cannot perform unban"); return false; /*Block*/}


                    case "kick":
                        if (HasPerm.LimitedPerm(arg.Player()) && arg.Player().IsAdmin && _config.CORE.Modes.Limited.Enabled &&
                            (_config.ADMIN.LIMITED.Limitations.Actions.enableAll || _config.ADMIN.LIMITED.Limitations.Actions.list.canKick))
                        { return null; /*Execute*/ }
                        else { DirectMessage(arg.Player(), "Insufficient permission cannot perform kick"); return false; /*Block*/}



                    case "entid":
                        if (HasPerm.LimitedPerm(arg.Player()) && arg.Player().IsAdmin && _config.CORE.Modes.Limited.Enabled &&
                            (_config.ADMIN.LIMITED.Limitations.Actions.enableAll || _config.ADMIN.LIMITED.Limitations.Actions.list.canDemolish))
                        { return null; /*Execute*/ }
                        else { DirectMessage(arg.Player(), "Insufficient permission cannot perform entity action"); return false; /*Block*/}

                    #endregion Commands

                    #region Permanently Blocked (Stupid Auth Commands)
                    case "kickall":
                    case "restart":
                    case "stop":
                    case "giveall":
                    case "givebpall":
                        DirectMessage(arg.Player(), "Permanently Disabled"); return false; /*Block*/
                        #endregion Permanently Blocked (Stupid Auth Commands)

                }
                return null; /*Execute Unknown Command*/
            }
            else
            {
                return null /*Execute Server Command*/;
            }
        }

        //On Damage Player & Building
        object OnEntityTakeDamage(BaseCombatEntity entity, HitInfo info)
        {
            //Attacker
            var player = info.InitiatorPlayer;

            if (player != null)
            {
                //On Hurt Players
                if (entity is BasePlayer)
                {
                    if (HasPerm.MasterPerm(player)) { return null; /*Execute*/ }
                    if (player == entity) { return null; /*Self Harm Execute*/ }

                    if (HasPerm.LimitedPerm(player) && player.IsAdmin && _config.CORE.Modes.Limited.Enabled &&
                       (_config.ADMIN.LIMITED.Limitations.Actions.enableAll || _config.ADMIN.LIMITED.Limitations.Actions.list.canHurt))
                    { return null; /*Execute In Admin*/ }
                    else if (player.IsAdmin) { DirectMessage(player, "Insufficient permission cannot hurt players"); return false; /*Block*/}
                    else { return null; /*Execute In Player*/}
                }

                //On Hurt Buildings
                if (entity is BuildingBlock || entity is Door)
                {
                    if (HasPerm.MasterPerm(player)) { return null; /*Execute*/ }
                    if (entity.OwnerID == player.userID) { return null; /*Self Damage Execute*/ }

                    if (HasPerm.LimitedPerm(player) && player.IsAdmin && _config.CORE.Modes.Limited.Enabled &&
                       (_config.ADMIN.LIMITED.Limitations.Actions.enableAll || _config.ADMIN.LIMITED.Limitations.Actions.list.canDemolish))
                    { return null; /*Execute In Admin*/ }
                    else if (player.IsAdmin) { DirectMessage(player, "Insufficient permission cannot damage/demolish buildings"); return false; /*Block*/}
                    else { return null; /*Execute In Player*/}
                }
            }

            return null; /*Execute Unknown*/
        }

        //On Item Drop
        void OnItemDropped(Item item, BaseEntity entity)
        {
            //Player
            var player = item.GetOwnerPlayer();

            if (player != null)
            {
                if (HasPerm.LimitedPerm(player) && player.IsAdmin && _config.CORE.Modes.Limited.Enabled &&
                  (!_config.ADMIN.LIMITED.Limitations.Actions.enableAll || !_config.ADMIN.LIMITED.Limitations.Actions.list.canDropItems))
                { item.DoRemove(); }
            }
        }

        //On Build
        object CanBuild(Planner planner, Construction prefab, Construction.Target target)
        {
            //Builder Player
            var player = planner.GetOwnerPlayer();
            if (player != null)
            {
                if (HasPerm.MasterPerm(player)) { return null; /*Execute*/ }


                if (HasPerm.LimitedPerm(player) && player.IsAdmin && _config.CORE.Modes.Limited.Enabled &&
                    (_config.ADMIN.LIMITED.Limitations.Actions.enableAll || _config.ADMIN.LIMITED.Limitations.Actions.list.canBuild))
                { return null; /*Execute In Admin*/ }
                else if (player.IsAdmin) { DirectMessage(player, "Insufficient permission cannot build"); return false; /*Block*/}
                else { return null; /*Execute In Player*/}

            }
            return null; /*Execute Unknown*/
        }

        //On Demolish
        bool CanDemolish(BasePlayer player, BuildingBlock block, BuildingGrade.Enum grade)
        {
            //Builder Player
            if (player != null)
            {
                if (HasPerm.MasterPerm(player)) { return true; /*Execute*/ }
                if (block.OwnerID == player.userID) { return true; /*Self Harm Execute*/ }


                if (HasPerm.LimitedPerm(player) && player.IsAdmin && _config.CORE.Modes.Limited.Enabled &&
                    (_config.ADMIN.LIMITED.Limitations.Actions.enableAll || _config.ADMIN.LIMITED.Limitations.Actions.list.canDemolish))
                { return true; /*Execute In Admin*/ }
                else if (player.IsAdmin) { DirectMessage(player, "Insufficient permission cannot demolish"); return false; /*Block*/}
                else { return true; /*Execute In Player*/}

            }
            return true; /*Execute Unknown*/
        }
        #endregion Oxide Hooks Limitations

        #region UI Menu Commands
        [ConsoleCommand("admintoggle.menu")]
        private void UiMenuCommander(ConsoleSystem.Arg arg)
        {
            var player = arg?.Player();
            var argument = arg?.Args?[0].ToLower();
            switch (argument)
            {
                case "clearinv":
                    if (LevelFunctions.SeparateInventorys(player)) 
                    {
                        if (LevelFunctions.SpecialOutfit(player) || LevelFunctions.Outfit(player))
                        {
                            player.inventory.containerMain.Clear();
                            player.inventory.containerBelt.Clear();
                        }
                        else
                        {
                            player.inventory.Strip();
                        }
                    }
                    else { DirectMessage(player, "Cannot clear inventory when SeparateInventorys is disabled"); }

                    break;

                case "vanish":
                    if (thirdParty.isLoaded.UseVanish && thirdParty.perms.has.umod.Vanish(player))
                    {
                        if (HasPerm.MasterPerm(player))
                        {
                            if (thirdParty.isLoaded.UseVanish) { player.Command("vanish"); }
                        }
                        else if (HasPerm.LimitedPerm(player))
                        {
                            if ((_config.ADMIN.LIMITED.Limitations.Plugins.enableAll || _config.ADMIN.LIMITED.Limitations.Plugins.list.Umod.useVanish) &&
                            thirdParty.isLoaded.UseVanish) { player.Command("vanish"); }
                        };
                    }
                    else { DirectMessage(player, $"<size=13>Third-Party Permission Missing: {thirdParty.perms.has.umod.vanishPerm}</size>"); }
                    break;

                case "god":
                    if (thirdParty.isLoaded.UseVanish && thirdParty.perms.has.umod.GodMode(player))
                    {
                        if (HasPerm.MasterPerm(player))
                        {
                            if (thirdParty.isLoaded.UseGodmode) { player.Command("god"); }
                        }
                        else if (HasPerm.LimitedPerm(player))
                        {
                            if ((_config.ADMIN.LIMITED.Limitations.Plugins.enableAll || _config.ADMIN.LIMITED.Limitations.Plugins.list.Umod.useGodmode) &&
                            thirdParty.isLoaded.UseGodmode) { player.Command("god"); }
                        }
                    }
                    else { DirectMessage(player, $"<size=13>Third-Party Permission Missing: {thirdParty.perms.has.umod.godmodePerm}</size>"); }
                    break;


                case "radar":
                    if (thirdParty.isLoaded.UseVanish && thirdParty.perms.has.umod.AdminRadar(player))
                    {
                        if (HasPerm.MasterPerm(player))
                        {
                            if (thirdParty.isLoaded.UseAdminRadar) { player.Command("radar"); }
                        }
                        else if (HasPerm.LimitedPerm(player))
                        {
                            if ((_config.ADMIN.LIMITED.Limitations.Plugins.enableAll || _config.ADMIN.LIMITED.Limitations.Plugins.list.Umod.useAdminRadar) &&
                            thirdParty.isLoaded.UseAdminRadar) { player.Command("radar"); }
                        }
                    }
                    else { DirectMessage(player, $"<size=13>Third-Party Permission Missing: {thirdParty.perms.has.umod.adminradarPerm}</size>"); }
                    break;

                case "playeradministrator":
                    if (HasPerm.MasterPerm(player))
                    {
                        if (thirdParty.isLoaded.UsePlayerAdministration) { player.Command("padmin"); }
                    }
                    else if (HasPerm.LimitedPerm(player))
                    {
                        if ((_config.ADMIN.LIMITED.Limitations.Plugins.enableAll || _config.ADMIN.LIMITED.Limitations.Plugins.list.Umod.usePlayerAdministration) &&
                        thirdParty.isLoaded.UsePlayerAdministration) { player.Command("padmin"); }
                    }

                    if (thirdParty.isLoaded.UseVanish && thirdParty.perms.has.umod.PlayerAdministration(player))
                    {
                        if (HasPerm.MasterPerm(player))
                        {
                            if (thirdParty.isLoaded.UsePlayerAdministration) { player.Command("padmin"); }
                        }
                        else if (HasPerm.LimitedPerm(player))
                        {
                            if ((_config.ADMIN.LIMITED.Limitations.Plugins.enableAll || _config.ADMIN.LIMITED.Limitations.Plugins.list.Umod.usePlayerAdministration) &&
                            thirdParty.isLoaded.UsePlayerAdministration) { player.Command("padmin"); }
                        }
                    }
                    else { DirectMessage(player, $"<size=13>Third-Party Permission Missing: {thirdParty.perms.has.umod.playeradministrationPerm}</size>"); }
                    break;
            }
        }
        #endregion UI Menu Commands
        private readonly Dictionary<ulong, Timer> UItimers = new Dictionary<ulong, Timer>();
        #region UI
        public class UI
        {
            public class Draw
            {
                public static void Button(BasePlayer player)
                {
                    CuiHelper.DestroyUi(player, "AdminToggleButton");
                    CuiElementContainer container = new CuiElementContainer();
                    #region Button
                    if (_config.CORE.Interface.enableButton)
                    {
                        if (LevelFunctions.RequireReason(player) && player.IsAdmin)
                        {
                            container.Add(new CuiPanel
                            {
                                Image = { Color = "0 0 0 0" },
                                RectTransform = { AnchorMin = "0.5 0", AnchorMax = "0.5 0", OffsetMin = "-265 18", OffsetMax = "-205 78" },
                                CursorEnabled = false,
                            }, "Overlay", "AdminToggleButton");

                            container.Add(new CuiButton
                            {
                                RectTransform = { AnchorMin = "0 0", AnchorMax = "1 1", OffsetMax = "0 0" },
                                Button = { Command = "admin.toggle", Color = player.IsAdmin ? _config.INTERFACE.Button.activeColor : _config.INTERFACE.Button.inactiveColor },
                                Text = { Text = player.IsAdmin ? _config.INTERFACE.Button.activeText : _config.INTERFACE.Button.inactiveText, Color = "1 1 1 1", Align = TextAnchor.MiddleCenter, FontSize = 15 }
                            }, "AdminToggleButton");
                        }
                        else if (!LevelFunctions.RequireReason(player))
                        {
                            container.Add(new CuiPanel
                            {
                                Image = { Color = "0 0 0 0" },
                                RectTransform = { AnchorMin = "0.5 0", AnchorMax = "0.5 0", OffsetMin = "-265 18", OffsetMax = "-205 78" },
                                CursorEnabled = false,
                            }, "Overlay", "AdminToggleButton");

                            container.Add(new CuiButton
                            {
                                RectTransform = { AnchorMin = "0 0", AnchorMax = "1 1", OffsetMax = "0 0" },
                                Button = { Command = "admin.toggle", Color = player.IsAdmin ? _config.INTERFACE.Button.activeColor : _config.INTERFACE.Button.inactiveColor },
                                Text = { Text = player.IsAdmin ? _config.INTERFACE.Button.activeText : _config.INTERFACE.Button.inactiveText, Color = "1 1 1 1", Align = TextAnchor.MiddleCenter, FontSize = 15 }
                            }, "AdminToggleButton");
                        }
                    }
                    #endregion Button
                    CuiHelper.AddUi(player, container);
                }


                public static void Panel(BasePlayer player)
                {
                    CuiHelper.DestroyUi(player, "AdminTogglePulsePanel");
                    CuiElementContainer container = new CuiElementContainer();

                    int fadeOutDuration = _config.INTERFACE.Panel.duration >= 1 ? _config.INTERFACE.Panel.duration : 1;

                    #region Panel
                    if (_config.CORE.Interface.enablePanel)
                    {
                        if (_config.INTERFACE.Panel.dock.ToLower() == "bottom")
                        {
                            container.Add(new CuiPanel
                            {
                                FadeOut = fadeOutDuration,
                                Image =
                                { Color = "1 1 1 0.16",  FadeIn = fadeOutDuration },
                                RectTransform =
                                {
                                    AnchorMin = "0.5 0",
                                    AnchorMax = "0.5 0",
                                    OffsetMin = "-199.5 0",
                                    OffsetMax = "180.5 18"
                                },
                                CursorEnabled = false,
                            }, "Hud", "AdminTogglePulsePanel");
                        }

                        if (_config.INTERFACE.Panel.dock.ToLower() == "top")
                        {
                            container.Add(new CuiPanel
                            {
                                FadeOut = fadeOutDuration,
                                Image =
                                { Color = "1 1 1 0.16",  FadeIn = fadeOutDuration },
                                RectTransform =
                                {
                                    AnchorMin = "0.5 0",
                                    AnchorMax = "0.5 0",
                                    OffsetMin = "-199.5 78",
                                    OffsetMax = "180.5 96"
                                },
                                CursorEnabled = false,
                            }, "Hud", "AdminTogglePulsePanel");
                        }


                        container.Add(new CuiLabel
                        {
                            FadeOut = fadeOutDuration,
                            Text =
                            {
                                FadeIn = fadeOutDuration,
                                Text = _config.INTERFACE.Panel.text,
                                Color = _config.INTERFACE.Panel.textColor,
                                Align = TextAnchor.MiddleCenter,
                            }
                        }, "AdminTogglePulsePanel", "AdminTogglePulsePanelLabel");
                    }
                    #endregion Panel
                    CuiHelper.AddUi(player, container);
                }


                public static void Menu(BasePlayer player)
                {
                    if (!player.IsAdmin) { CuiHelper.DestroyUi(player, "AdminToggleMenu"); return; }
                    CuiHelper.DestroyUi(player, "AdminToggleMenu");
                    CuiElementContainer container = new CuiElementContainer();
                    #region Menu
                    if (_config.CORE.Interface.enableMenu)
                    {
                        if (_config.INTERFACE.Menu.dock.ToLower() == "right")
                        {
                            container.Add(new CuiPanel
                            {
                                Image = { Color = "0 0 0 0" },
                                RectTransform = {
                                    AnchorMin = "0.5 0",
                                    AnchorMax = "0.5 0",
                                    OffsetMin = "185 18",
                                    OffsetMax = "425 78"
                                },
                                CursorEnabled = false,
                            }, "Hud", "AdminToggleMenu");
                        }

                        if (_config.INTERFACE.Menu.dock.ToLower() == "bottomleft")
                        {
                            container.Add(new CuiPanel
                            {
                                Image = { Color = "0 0 0 0" },
                                RectTransform = {
                                    AnchorMin = "0 0",
                                    AnchorMax = "0 0",
                                    OffsetMin = "18 15",
                                    OffsetMax = "258 75"
                                },
                                CursorEnabled = false,
                            }, "Overlay", "AdminToggleMenu");
                        }

                    }
                    #endregion Menu

                    CreateButton(container, "AdminToggleMenu", "admintoggle.menu clearinv", LevelFunctions.SeparateInventorys(player) ? "0.98 0.68 0.09 0.65" : "0 0 0 0.75", "Clear Inventory (Admin)", 12, "0 0.70", "1 1");

                    CreateButton(container, "AdminToggleMenu", "admintoggle.menu radar", !thirdParty.isLoaded.UseAdminRadar ? "0 0 0 0.75" : thirdParty.isEnabled.AdminRadar ? _config.INTERFACE.Menu.activeColor : _config.INTERFACE.Menu.inactiveColor, "Admin Radar", 12, "0 0", "0.5 0.35");
                    CreateButton(container, "AdminToggleMenu", "admintoggle.menu playeradministrator", !thirdParty.isLoaded.UsePlayerAdministration ? "0 0 0 0.75" : thirdParty.isEnabled.PlayerAdministration ? _config.INTERFACE.Menu.activeColor : _config.INTERFACE.Menu.inactiveColor, "Player Administration", 12, "0.5 0", "1 0.35");

                    CreateButton(container, "AdminToggleMenu", "admintoggle.menu god", !thirdParty.isLoaded.UseGodmode ? "0 0 0 0.75" : thirdParty.isEnabled.Godmode ? _config.INTERFACE.Menu.activeColor : _config.INTERFACE.Menu.inactiveColor, "God Mode", 12, "0 0.35", "0.5 0.70");
                    CreateButton(container, "AdminToggleMenu", "admintoggle.menu vanish", !thirdParty.isLoaded.UseVanish ? "0 0 0 0.75" : thirdParty.isEnabled.Vanish ? _config.INTERFACE.Menu.activeColor : _config.INTERFACE.Menu.inactiveColor, "Vanish", 12, "0.5 0.35", "1 0.70");



                    CuiHelper.AddUi(player, container);
                }
            }

            private static void CreateButton(CuiElementContainer container, string ParentItem, string BtnCommand, string BtnColor, string text, int TextSize, string aMin, string aMax)
            {
                container.Add(new CuiButton
                {
                    Button = { Color = BtnColor, Command = BtnCommand },
                    RectTransform = { AnchorMin = aMin, AnchorMax = aMax, OffsetMin = "0 0", OffsetMax = "0 0" },
                    Text = { Text = text, Color = "1 1 1 1", FontSize = TextSize, Align = UnityEngine.TextAnchor.MiddleCenter },
                }, ParentItem);
            }

            public static void PanelStartPulsing(BasePlayer player)
            {
                if (_config.CORE.Interface.enablePanel)
                {
                    int fadeOutDuration = _config.INTERFACE.Panel.duration >= 1 ? _config.INTERFACE.Panel.duration : 1;

                    //First Time
                    if (!Instance.UItimers.ContainsKey(player.userID))
                    {
                        UI.Draw.Panel(player);

                        Instance.timer.Once(fadeOutDuration, () =>
                        {
                            CuiHelper.DestroyUi(player, "AdminTogglePulsePanel");
                            CuiHelper.DestroyUi(player, "AdminTogglePulsePanelLabel");
                        });
                    }

                    //Loop
                    Instance.UItimers[player.userID] = Instance.timer.Every(fadeOutDuration * 2, () =>
                    {
                        if (!player.IsAdmin) { PanelStopPulsing(player); return; }

                        UI.Draw.Panel(player);

                        Instance.timer.Once(fadeOutDuration, () =>
                        {
                            CuiHelper.DestroyUi(player, "AdminTogglePulsePanel");
                            CuiHelper.DestroyUi(player, "AdminTogglePulsePanelLabel");
                        });
                    });
                }
            }

            public static void PanelStopPulsing(BasePlayer player)
            {
                try
                {
                    CuiHelper.DestroyUi(player, "AdminTogglePulsePanel");
                    CuiHelper.DestroyUi(player, "AdminTogglePulsePanelLabel");
                    Instance.UItimers[player.userID].Destroy();
                    Instance.UItimers.Remove(player.userID);
                }
                catch { }
            }
        }
        #endregion UI

        #region Inventory Class
        public class Inventory
        {
            public class Items
            {
                public static Items Caller = new Items();

                private Dictionary<ulong, SavedInventories> cachedInventories = new Dictionary<ulong, SavedInventories>();
                private class SavedInventories { public List<SavedItem> Items; }
                private class SavedItem
                {
                    public string ItemShortname { get; set; }
                    public int ItemPosition { get; set; }
                    public ItemContainer container { get; set; }
                    public ulong ItemskinID { get; set; }
                    public int ItemQuantity { get; set; }
                    public float ItemCondition { get; set; }
                    public float ItemMaxCondition { get; set; }
                    public int ItemAmmoQuantity { get; set; }
                    public string ItemAmmoType { get; set; }
                    public int ItemFrequency { get; set; }
                    public int ItemBlueprint { get; set; }
                    public SavedItem[] ItemContents { get; set; }
                }

                public int GetItemCount(BasePlayer player)
                {
                    if (cachedInventories.ContainsKey(player.userID))
                    {
                        return cachedInventories[player.userID].Items.Count;
                    }
                    return 0;
                }

                #region Save Inventory
                public void SaveInventory(BasePlayer player)
                {
                    if (cachedInventories.ContainsKey(player.userID)) { cachedInventories[player.userID].Items.Clear(); }
                    List<SavedItem> items = GetPlayerItems(player);
                    if (!cachedInventories.ContainsKey(player.userID)) { cachedInventories.Add(player.userID, new SavedInventories { }); }
                    cachedInventories[player.userID].Items = items;
                    ClearInventory(player);
                }
                private List<SavedItem> GetPlayerItems(BasePlayer player)
                {
                    List<SavedItem> PlayerItems = new List<SavedItem>();
                    List<Item> Items = new List<Item>();
                    player.inventory.GetAllItems(Items);
                    foreach(Item item in Items)
                    {
                        if(item != null)
                        {
                            var container = item.GetRootContainer();
                            PlayerItems.Add(ProcessItem(item, container));
                        }
                    }
                    return PlayerItems;
                }
                private SavedItem ProcessItem(Item item, ItemContainer container)
                {
                    SavedItem ProcessedItem = new SavedItem()
                    {
                        container = container,
                        ItemShortname = item.info.shortname,
                        ItemPosition = item.position,
                        ItemQuantity = item.amount,
                        ItemAmmoQuantity = item.GetHeldEntity() is BaseProjectile ? (item.GetHeldEntity() as BaseProjectile).primaryMagazine.contents : item.GetHeldEntity() is FlameThrower ? (item.GetHeldEntity() as FlameThrower).ammo : 0,
                        ItemAmmoType = item.GetHeldEntity() is BaseProjectile ? (item.GetHeldEntity() as BaseProjectile).primaryMagazine.ammoType.shortname : null,
                        ItemskinID = item.info.HasSkins ? item.skin : 0,
                        ItemCondition = item.condition,
                        ItemMaxCondition = item.maxCondition,
                        ItemBlueprint = item.IsBlueprint() ? item.blueprintTarget : 0,
                        ItemFrequency = ItemModAssociatedEntity<PagerEntity>.GetAssociatedEntity(item)?.GetFrequency() ?? -1,
                        ItemContents = item.contents?.itemList.Select(contentsItem => new SavedItem
                        {
                            ItemShortname = contentsItem.info.shortname,
                            ItemQuantity = contentsItem.amount,
                            ItemCondition = contentsItem.condition
                        }).ToArray()
                    };
                    return ProcessedItem;
                }
                #endregion Save Inventory

                #region Restore Inventory
                public void RestoreInventory(BasePlayer player)
                {
                    if (cachedInventories.ContainsKey(player.userID))
                    {
                        ClearInventory(player);
                        foreach (SavedItem savedItem in cachedInventories[player.userID].Items)
                        {
                            var i = CreateItem(savedItem);
                            GiveItem(player, i, savedItem.container);
                            i.MoveToContainer(savedItem.container, savedItem.ItemPosition);
                        }
                        cachedInventories.Remove(player.userID);
                    }
                }
                private void GiveItem(BasePlayer player, Item item, ItemContainer container) { if (item != null) { player.inventory.GiveItem(item, container); } }
                private Item CreateItem(SavedItem savedItem)
                {
                    Item item = ItemManager.CreateByName(savedItem.ItemShortname, savedItem.ItemQuantity, savedItem.ItemskinID);
                    item.condition = savedItem.ItemCondition;
                    item.maxCondition = savedItem.ItemMaxCondition;

                    if (savedItem.ItemFrequency > 0)
                    {
                        ItemModRFListener rfListener = item.info.GetComponentInChildren<ItemModRFListener>();
                        if (rfListener != null)
                        {
                            PagerEntity pagerEntity = BaseNetworkable.serverEntities.Find(item.instanceData.subEntity) as PagerEntity;
                            if (pagerEntity != null)
                            {
                                pagerEntity.ChangeFrequency(savedItem.ItemFrequency);
                                item.MarkDirty();
                            }
                        }
                    }


                    BaseProjectile weapon = item.GetHeldEntity() as BaseProjectile;
                    if (weapon != null)
                    {
                        if (!string.IsNullOrEmpty(savedItem.ItemAmmoType))
                            weapon.primaryMagazine.ammoType = ItemManager.FindItemDefinition(savedItem.ItemAmmoType);
                        weapon.primaryMagazine.contents = savedItem.ItemAmmoQuantity;
                    }

                    FlameThrower flameThrower = item.GetHeldEntity() as FlameThrower;
                    if (flameThrower != null)
                        flameThrower.ammo = savedItem.ItemAmmoQuantity;

                    if (savedItem.ItemContents != null)
                    {
                        foreach (SavedItem contentData in savedItem.ItemContents)
                        {
                            Item newContent = ItemManager.CreateByName(contentData.ItemShortname, contentData.ItemQuantity);
                            if (newContent != null)
                            {
                                newContent.condition = contentData.ItemCondition;
                                newContent.MoveToContainer(item.contents);
                            }
                        }
                    }
                    return item;
                }
                #endregion Restore Inventory

                public void ClearInventory(BasePlayer player)
                {
                    if (cachedInventories.ContainsKey(player.userID) && cachedInventories[player.userID].Items.Count > 0) { player.inventory.Strip(); }
                }

                #region Lock/Unlock Items
                public static void Lock(BasePlayer player)
                {
                    LockContainer(player.inventory.containerBelt);
                    LockContainer(player.inventory.containerMain);
                    LockContainer(player.inventory.containerWear);
                }
                public static void Unlock(BasePlayer player)
                {
                    UnlockContainer(player.inventory.containerBelt);
                    UnlockContainer(player.inventory.containerMain);
                    UnlockContainer(player.inventory.containerWear);
                }
                public static void LockContainer(ItemContainer container) => container.SetLocked(true);
                public static void UnlockContainer(ItemContainer container) => container.SetLocked(false);
                #endregion Lock/Unlock Items
            }
        }
        #endregion Inventory Class

        #region Third-party

        private static ThirdPartyPlugins thirdParty = new ThirdPartyPlugins();
        private class ThirdPartyPlugins
        {
            public IsLoaded isLoaded = new IsLoaded();
            public class IsLoaded
            {
                public bool UseBackpacks { get; set; }
                public bool UseFreeze { get; set; }
                public bool UseAdminRadar { get; set; }
                public bool UseGodmode { get; set; }
                public bool UseInventoryViewer { get; set; }
                public bool UseVanish { get; set; }
                public bool UsePlayerAdministration { get; set; }
                public bool UseSpectate { get; set; }
            }


            public EnabledState isEnabled = new EnabledState();
            public class EnabledState
            {
                public bool AdminRadar { get; set; }
                public bool Godmode { get; set; }
                public bool Vanish { get; set; }
                public bool PlayerAdministration { get; set; }
            }


            public ThirdPartyPerms perms = new ThirdPartyPerms();
            public class ThirdPartyPerms
            {
                public HasPerm has = new HasPerm();
                public class HasPerm
                {
                    public Umod umod = new Umod();
                    public class Umod
                    {
                        public readonly string adminradarPerm = "adminradar.allowed";
                        public bool AdminRadar(BasePlayer player) => Instance.permission.UserHasPermission(player.UserIDString, adminradarPerm);

                        public readonly string godmodePerm = "godmode.toggle";
                        public bool GodMode(BasePlayer player) => Instance.permission.UserHasPermission(player.UserIDString, godmodePerm);

                        public readonly string vanishPerm = "vanish.allow";
                        public bool Vanish(BasePlayer player) => Instance.permission.UserHasPermission(player.UserIDString, vanishPerm);

                        public readonly string playeradministrationPerm = "playeradministration.access.show";
                        public bool PlayerAdministration(BasePlayer player) => Instance.permission.UserHasPermission(player.UserIDString, playeradministrationPerm);
                    }
                }
            }

            public void LoadPlugins(BasePlayer player)
            {
                if (LevelFunctions.AutoLoadPlugins(player))
                {
                    Instance.timer.Once(1f, () =>
                    {
                        if (HasPerm.MasterPerm(player))
                        {
                            if ((_config.ADMIN.AutoLoad.Plugins.autoloadAll || _config.ADMIN.AutoLoad.Plugins.List.Umod.loadAdminRadar) &&
                            thirdParty.isLoaded.UseAdminRadar && Instance.AdminRadar.Call<bool>("IsRadar", player.UserIDString) == false)
                            {
                                if (thirdParty.perms.has.umod.AdminRadar(player)) { player.Command("radar"); }
                                else { DirectMessage(player, $"<size=13>Third-Party Permission Missing: {thirdParty.perms.has.umod.adminradarPerm}</size>"); }
                            }

                            if ((_config.ADMIN.AutoLoad.Plugins.autoloadAll || _config.ADMIN.AutoLoad.Plugins.List.Umod.loadGodmode) &&
                            thirdParty.isLoaded.UseGodmode && Instance.Godmode.Call<bool>("IsGod", player.UserIDString) == false)
                            {
                                if (thirdParty.perms.has.umod.GodMode(player)) { player.Command("god"); }
                                else { DirectMessage(player, $"<size=13>Third-Party Permission Missing: {thirdParty.perms.has.umod.godmodePerm}</size>"); }
                            }

                            if ((_config.ADMIN.AutoLoad.Plugins.autoloadAll || _config.ADMIN.AutoLoad.Plugins.List.Umod.loadVanish) &&
                            thirdParty.isLoaded.UseVanish && Instance.Vanish.Call<bool>("IsInvisible", player) == false)
                            {
                                if (thirdParty.perms.has.umod.Vanish(player)) { player.Command("vanish"); }
                                else { DirectMessage(player, $"<size=13>Third-Party Permission Missing: {thirdParty.perms.has.umod.vanishPerm}</size>"); }
                            }
                        }
                        else if (HasPerm.LimitedPerm(player))
                        {
                            if ((_config.ADMIN.AutoLoad.Plugins.autoloadAll || _config.ADMIN.AutoLoad.Plugins.List.Umod.loadAdminRadar) &&
                            (_config.ADMIN.LIMITED.Limitations.Plugins.enableAll || _config.ADMIN.LIMITED.Limitations.Plugins.list.Umod.useAdminRadar) &&
                            thirdParty.isLoaded.UseAdminRadar && Instance.AdminRadar.Call<bool>("IsRadar", player.UserIDString) == false)
                            {
                                if (thirdParty.perms.has.umod.AdminRadar(player)) { player.Command("radar"); }
                                else { DirectMessage(player, $"<size=13>Third-Party Permission Missing: {thirdParty.perms.has.umod.adminradarPerm}</size>"); }
                            }

                            if ((_config.ADMIN.AutoLoad.Plugins.autoloadAll || _config.ADMIN.AutoLoad.Plugins.List.Umod.loadGodmode) &&
                            (_config.ADMIN.LIMITED.Limitations.Plugins.enableAll || _config.ADMIN.LIMITED.Limitations.Plugins.list.Umod.useGodmode) &&
                            thirdParty.isLoaded.UseGodmode && Instance.Godmode.Call<bool>("IsGod", player.UserIDString) == false)
                            {
                                if (thirdParty.perms.has.umod.GodMode(player)) { player.Command("god"); }
                                else { DirectMessage(player, $"<size=13>Third-Party Permission Missing: {thirdParty.perms.has.umod.godmodePerm}</size>"); }
                            }

                            if ((_config.ADMIN.AutoLoad.Plugins.autoloadAll || _config.ADMIN.AutoLoad.Plugins.List.Umod.loadVanish) &&
                            (_config.ADMIN.LIMITED.Limitations.Plugins.enableAll || _config.ADMIN.LIMITED.Limitations.Plugins.list.Umod.useVanish) &&
                            thirdParty.isLoaded.UseVanish && Instance.Vanish.Call<bool>("IsInvisible", player) == false)
                            {
                                if (thirdParty.perms.has.umod.Vanish(player)) { player.Command("vanish"); }
                                else { DirectMessage(player, $"<size=13>Third-Party Permission Missing: {thirdParty.perms.has.umod.vanishPerm}</size>"); }
                            }
                        }
                    });
                }
            }

            public void UnloadPlugins(BasePlayer player, bool force)
            {
                if (LevelFunctions.BlockPlayerPlugins(player))
                {
                    if (force)
                    {
                        if ((_config.PLAYER.Blocking.Plugins.blockAll || _config.PLAYER.Blocking.Plugins.List.Umod.blockAdminRadar) &&
                        thirdParty.isLoaded.UseAdminRadar && Instance.AdminRadar.Call<bool>("IsRadar", player.UserIDString) == true)
                        {
                            if (thirdParty.perms.has.umod.AdminRadar(player)) { player.Command("radar"); }
                            else { DirectMessage(player, $"<size=13>Third-Party Permission Missing: {thirdParty.perms.has.umod.adminradarPerm}</size>"); }
                        }

                        if ((_config.PLAYER.Blocking.Plugins.blockAll || _config.PLAYER.Blocking.Plugins.List.Umod.blockGodmode) &&
                        thirdParty.isLoaded.UseGodmode && Instance.Godmode.Call<bool>("IsGod", player.UserIDString) == true)
                        {
                            if (thirdParty.perms.has.umod.GodMode(player)) { player.Command("god"); }
                            else { DirectMessage(player, $"<size=13>Third-Party Permission Missing: {thirdParty.perms.has.umod.godmodePerm}</size>"); }
                        }

                        if ((_config.PLAYER.Blocking.Plugins.blockAll || _config.PLAYER.Blocking.Plugins.List.Umod.blockVanish) &&
                        thirdParty.isLoaded.UseVanish && Instance.Vanish.Call<bool>("IsInvisible", player) == true)
                        {
                            if (thirdParty.perms.has.umod.Vanish(player)) { player.Command("vanish"); }
                            else { DirectMessage(player, $"<size=13>Third-Party Permission Missing: {thirdParty.perms.has.umod.vanishPerm}</size>"); }
                        }
                    }
                    else
                    {
                        Instance.timer.Once(1f, () =>
                        {
                            if ((_config.PLAYER.Blocking.Plugins.blockAll || _config.PLAYER.Blocking.Plugins.List.Umod.blockAdminRadar) &&
                            thirdParty.isLoaded.UseAdminRadar && Instance.AdminRadar.Call<bool>("IsRadar", player.UserIDString) == true)
                            {
                                if (thirdParty.perms.has.umod.AdminRadar(player)) { player.Command("radar"); }
                                else { DirectMessage(player, $"<size=13>Third-Party Permission Missing: {thirdParty.perms.has.umod.adminradarPerm}</size>"); }
                            }

                            if ((_config.PLAYER.Blocking.Plugins.blockAll || _config.PLAYER.Blocking.Plugins.List.Umod.blockGodmode) &&
                            thirdParty.isLoaded.UseGodmode && Instance.Godmode.Call<bool>("IsGod", player.UserIDString) == true)
                            {
                                if (thirdParty.perms.has.umod.GodMode(player)) { player.Command("god"); }
                                else { DirectMessage(player, $"<size=13>Third-Party Permission Missing: {thirdParty.perms.has.umod.godmodePerm}</size>"); }
                            }

                            if ((_config.PLAYER.Blocking.Plugins.blockAll || _config.PLAYER.Blocking.Plugins.List.Umod.blockGodmode) &&
                            thirdParty.isLoaded.UseVanish && Instance.Vanish.Call<bool>("IsInvisible", player) == true)
                            {
                                if (thirdParty.perms.has.umod.Vanish(player)) { player.Command("vanish"); }
                                else { DirectMessage(player, $"<size=13>Third-Party Permission Missing: {thirdParty.perms.has.umod.vanishPerm}</size>"); }
                            }
                        });
                    }
                }
            }

            public void Load()
            {
                if (Instance.Backpacks != null && Instance.Backpacks.IsLoaded) { isLoaded.UseBackpacks = true; }
                if (Instance.Freeze != null && Instance.Freeze.IsLoaded) { isLoaded.UseFreeze = true; }
                if (Instance.AdminRadar != null && Instance.AdminRadar.IsLoaded) { isLoaded.UseAdminRadar = true; }
                if (Instance.Godmode != null && Instance.Godmode.IsLoaded) { isLoaded.UseGodmode = true; }
                if (Instance.InventoryViewer != null && Instance.InventoryViewer.IsLoaded) { isLoaded.UseInventoryViewer = true; }
                if (Instance.Vanish != null && Instance.Vanish.IsLoaded) { isLoaded.UseVanish = true; }
                if (Instance.PlayerAdministration != null && Instance.PlayerAdministration.IsLoaded) { isLoaded.UsePlayerAdministration = true; }
                if (Instance.Spectate != null && Instance.Spectate.IsLoaded) { isLoaded.UseSpectate = true; }
            }

            public void Subscribe()
            {
                //Vanish
                if (thirdParty.isLoaded.UseVanish) { Instance.Subscribe("OnVanishDisappear"); Instance.Subscribe("OnVanishReappear"); }

                //Admin Radar
                if (thirdParty.isLoaded.UseAdminRadar) { Instance.Subscribe("OnRadarActivated"); Instance.Subscribe("OnRadarDeactivated"); }

                //Godmode
                if (thirdParty.isLoaded.UseGodmode) { Instance.Subscribe("OnGodmodeToggled"); }
            }

            public void UnSubscribe()
            {
                //Vanish
                try { Instance.Unsubscribe("OnVanishDisappear"); Instance.Subscribe("OnVanishReappear"); } catch { }

                //Admin Radar
                try { Instance.Subscribe("OnRadarActivated"); Instance.Subscribe("OnRadarDeactivated"); } catch { }

                //Godmode
                try { Instance.Unsubscribe("OnGodmodeToggled"); } catch { }
            }
        }
        #endregion Third-party

        #region Limitations
        void OnVanishReappear(BasePlayer player) { thirdParty.isEnabled.Vanish = false; UI.Draw.Menu(player); }

        //Block Vanish
        void OnVanishDisappear(BasePlayer player)
        {
            thirdParty.isEnabled.Vanish = true; UI.Draw.Menu(player);
            if (thirdParty.isLoaded.UseVanish)
            {
                if (HasPerm.BothPerm(player)) { return; /*Override To Master*/ }
                if (HasPerm.LimitedPerm(player) && player.IsAdmin && _config.CORE.Modes.Limited.Enabled &&
                (!_config.ADMIN.LIMITED.Limitations.Plugins.enableAll && !_config.ADMIN.LIMITED.Limitations.Plugins.list.Umod.useVanish))
                {
                    timer.Once(0.25f, () => {
                        Vanish.Call("Reappear", player);
                        DirectMessage(player, "Insufficient permission cannot use vanish");
                    });
                }
            }
            if (!player.IsAdmin) //Prevent Player Usage if bypassed Chat comamnds
            {
                if (LevelFunctions.BlockPlayerPlugins(player))
                {
                    if (_config.PLAYER.Blocking.Plugins.blockAll || _config.PLAYER.Blocking.Plugins.List.Umod.blockVanish)
                    {
                        timer.Once(0.25f, () => {
                            if ((bool)(Vanish?.Call<bool>("IsInvisible", player)) == true) { player.IPlayer.Command("vanish"); }
                            DirectMessage(player, "Cannot use vanish in player mode");
                        });
                    }
                }
            }
        }

        //Block Godmode
        private void OnGodmodeToggled(string playerId, bool enabled)
        {
            if (enabled)
            {
                thirdParty.isEnabled.Godmode = true; UI.Draw.Menu(BasePlayer.FindByID(Convert.ToUInt64(playerId)));
                var player = BasePlayer.FindByID(Convert.ToUInt64(playerId));

                if (thirdParty.isLoaded.UseGodmode)
                {
                    if (HasPerm.BothPerm(player)) { return; /*Override To Master*/ }
                    if (HasPerm.LimitedPerm(player) && player.IsAdmin && _config.CORE.Modes.Limited.Enabled &&
                    (!_config.ADMIN.LIMITED.Limitations.Plugins.enableAll && !_config.ADMIN.LIMITED.Limitations.Plugins.list.Umod.useGodmode))
                    {
                        timer.Once(0.25f, () => {
                            if ((bool)(Godmode.Call<bool>("IsGod", player.UserIDString)) == true) { player.IPlayer.Command("god"); }
                            DirectMessage(player, "Insufficient permission cannot use godmode");
                        });
                    }
                }
                if (!player.IsAdmin) //Prevent Player Usage if bypassed Chat comamnds
                {
                    if (LevelFunctions.BlockPlayerPlugins(player))
                    {
                        if (_config.PLAYER.Blocking.Plugins.blockAll || _config.PLAYER.Blocking.Plugins.List.Umod.blockGodmode)
                        {
                            timer.Once(0.25f, () => {
                                if ((bool)(Godmode?.Call<bool>("IsGod", player.UserIDString)) == true) { player.IPlayer.Command("god"); }
                                DirectMessage(player, "Cannot use godmode in player mode");
                            });
                        }
                    }
                }
            }
            else { thirdParty.isEnabled.Godmode = false; UI.Draw.Menu(BasePlayer.FindByID(Convert.ToUInt64(playerId))); }
        }

        //Block Admin Radar
        void OnRadarActivated(BasePlayer player)
        {
            thirdParty.isEnabled.AdminRadar = true; UI.Draw.Menu(player);
            if (thirdParty.isLoaded.UseAdminRadar)
            {
                if (HasPerm.BothPerm(player)) { return; /*Override To Master*/ }
                if (HasPerm.LimitedPerm(player) && player.IsAdmin && _config.CORE.Modes.Limited.Enabled &&
                (!_config.ADMIN.LIMITED.Limitations.Plugins.enableAll && !_config.ADMIN.LIMITED.Limitations.Plugins.list.Umod.useAdminRadar))
                {
                    timer.Once(0.25f, () => {
                        if ((bool)(AdminRadar.Call<bool>("IsRadar", player.UserIDString)) == true) { player.IPlayer.Command("radar"); }
                        DirectMessage(player, "Insufficient permission cannot use admin radar");
                    });
                }
                if (!player.IsAdmin) //Prevent Player Usage if bypassed Chat comamnds
                {
                    if (LevelFunctions.BlockPlayerPlugins(player))
                    {
                        if (_config.PLAYER.Blocking.Plugins.blockAll || _config.PLAYER.Blocking.Plugins.List.Umod.blockAdminRadar)
                        {
                            timer.Once(0.25f, () => {
                                if ((bool)(AdminRadar.Call<bool>("IsRadar", player.UserIDString)) == true) { player.IPlayer.Command("radar"); }
                                DirectMessage(player, "Cannot use admin radar in player mode");
                            });
                        }
                    }
                }
            }
        }
        void OnRadarDeactivated(BasePlayer player) { thirdParty.isEnabled.AdminRadar = false; UI.Draw.Menu(player); }


        //Block Commands In Player Mode (First line of defense)
        private readonly string[] RadarCommand = { "radar" };
        private readonly string[] FreezeCommand = { "freeze", "unfreeze", "freezeall", "unfreezeall" };
        private readonly string[] GodModeCommand = { "god", "godmode", "gods", "godlist" };
        private readonly string[] InventoryViewerCommand = { "viewinv", "viewinventory", "inspect" };
        private readonly string[] PlayerAdministrationCommand = { "padmin" };
        private readonly string[] SpectateCommand = { "spectate" };
        private readonly string[] VanishCommand = { "vanish" };

        object OnPlayerCommand(BasePlayer player, string command, string[] args)
        {
            //Custom Hook For PlayerAdministrationCommand
            if (PlayerAdministrationCommand.Contains(command))
            {
                if (thirdParty.perms.has.umod.PlayerAdministration(player))
                {
                    thirdParty.isEnabled.PlayerAdministration = true; UI.Draw.Menu(player);
                }
                else { DirectMessage(player, $"<size=13>Third-Party Permission Missing: {thirdParty.perms.has.umod.playeradministrationPerm}</size>"); }
            }




            if (RadarCommand.Contains(command) || FreezeCommand.Contains(command) ||
             GodModeCommand.Contains(command) || InventoryViewerCommand.Contains(command) ||
             PlayerAdministrationCommand.Contains(command) || VanishCommand.Contains(command))
            {
                if ((player.IsAdmin && HasPerm.MasterPerm(player)) || (player.IsAdmin && HasPerm.BothPerm(player))) { return null; /*Execute Any*/};

                if (player.IsAdmin && HasPerm.LimitedPerm(player))
                {
                    if (_config.ADMIN.LIMITED.Limitations.Plugins.enableAll) { return null; /*Execute Any*/ }

                    if (RadarCommand.Contains(command) && !_config.ADMIN.LIMITED.Limitations.Plugins.list.Umod.useAdminRadar) { DirectMessage(player, $"Insufficient permission cannot use /{command}"); return false; /*Block*/}
                    if (FreezeCommand.Contains(command) && !_config.ADMIN.LIMITED.Limitations.Plugins.list.Umod.useFreeze) { DirectMessage(player, $"Insufficient permission cannot use /{command}"); return false; /*Block*/}
                    if (GodModeCommand.Contains(command) && !_config.ADMIN.LIMITED.Limitations.Plugins.list.Umod.useGodmode) { DirectMessage(player, $"Insufficient permission cannot use /{command}"); return false; /*Block*/}
                    if (InventoryViewerCommand.Contains(command) && !_config.ADMIN.LIMITED.Limitations.Plugins.list.Umod.useInventoryViewer) { DirectMessage(player, $"Insufficient permission cannot use /{command}"); return false; /*Block*/}
                    if (SpectateCommand.Contains(command) && !_config.ADMIN.LIMITED.Limitations.Plugins.list.Umod.useSpectate) { DirectMessage(player, $"Insufficient permission cannot use /{command}"); return false; /*Block*/}
                    if (VanishCommand.Contains(command) && !_config.ADMIN.LIMITED.Limitations.Plugins.list.Umod.useVanish) { DirectMessage(player, $"Insufficient permission cannot use /{command}"); return false; /*Block*/}
                }

                //In Player Block
                if (!player.IsAdmin && LevelFunctions.BlockPlayerPlugins(player))
                {
                    if (_config.PLAYER.Blocking.Plugins.blockAll) { DirectMessage(player, $"/{command} cannot be used in Player Mode"); return false; /*Block All*/ }

                    if (RadarCommand.Contains(command) && _config.PLAYER.Blocking.Plugins.List.Umod.blockAdminRadar) { DirectMessage(player, $"/{command} cannot be used in Player Mode"); return false; /*Block*/}
                    if (FreezeCommand.Contains(command) && _config.PLAYER.Blocking.Plugins.List.Umod.blockFreeze) { DirectMessage(player, $"/{command} cannot be used in Player Mode"); return false; /*Block*/}
                    if (GodModeCommand.Contains(command) && _config.PLAYER.Blocking.Plugins.List.Umod.blockGodmode) { DirectMessage(player, $"/{command} cannot be used in Player Mode"); return false; /*Block*/}
                    if (InventoryViewerCommand.Contains(command) && _config.PLAYER.Blocking.Plugins.List.Umod.blockInventoryViewer) { DirectMessage(player, $"/{command} cannot be used in Player Mode"); return false; /*Block*/}
                    if (PlayerAdministrationCommand.Contains(command) && _config.PLAYER.Blocking.Plugins.List.Umod.blockPlayerAdministration) { DirectMessage(player, $"/{command} cannot be used in Player Mode"); return false; /*Block*/}
                    if (SpectateCommand.Contains(command) && _config.PLAYER.Blocking.Plugins.List.Umod.blockSpectate) { DirectMessage(player, $"/{command} cannot be used in Player Mode"); return false; /*Block*/}
                    if (VanishCommand.Contains(command) && _config.PLAYER.Blocking.Plugins.List.Umod.blockVanish) { DirectMessage(player, $"/{command} cannot be used in Player Mode"); return false; /*Block*/}
                }

                return null; /*Execute Unknown*/
            }
            else { return null; /*Execute Unknown*/ }
        }
        #endregion Limitations

        #region API Methods & Hooks
        private bool Toggle(BasePlayer player, string[] args)
        {
            ToggleMode.Toggle(player, args);
            return true;
        }
        private bool SetAdmin(BasePlayer player, string[] args)
        {
            if (!player.IsAdmin) { ToggleMode.SetAdmin(player, args); return true; }
            return false;
        }
        private bool SetPlayer(BasePlayer player)
        {
            if (player.IsAdmin) { ToggleMode.SetPlayer(player); return true; }
            return false;
        }

        private BasePlayer AdminToggleOnAdmin(BasePlayer player) { return player; }
        private BasePlayer AdminToggleOnPlayer(BasePlayer player) { return player; }
        #endregion API Methods & Hooks

        #region Logging
        private void LogAction(BasePlayer player, string reason)
        {
            if (_config.CORE.Logging.enableFile && LevelFunctions.CanLog(player))
            {
                var r = string.IsNullOrEmpty(reason) ? "" : reason;
                LogToFile("Activations", $"[{DateTime.Now.ToString("yyyy/MM/dd:HH:m:ss")}] {player} Activated Admin Mode with the reason: {r}", this, false);
            }
        }

        private void PrintToDiscord(BasePlayer player, string reason)
        {
            if (_config.CORE.Logging.enableDiscord && LevelFunctions.CanLog(player))
            {
                string body = "{\n  \"content\": null,\n  \"embeds\": [\n    {\n      \"title\": \"Admin Mode Activated\",\n      \"description\": \"**" + player.displayName + "**: [" + player.UserIDString + "](https://steamcommunity.com/profiles/" + player.UserIDString + ")\\n**Reason**: " + reason + "\",\n      \"color\": " + _config.LOGGING.Discord.embedColor + "\n    }\n  ]\n}";
                webrequest.Enqueue(_config.LOGGING.Discord.webHook, body, (code, response) => { }, this, RequestMethod.POST, new Dictionary<string, string> { { "Content-Type", "application/json" } });
            }
        }
        #endregion Logging

        #region Notification Functions
        private void ServerBroadcast(BasePlayer player, string message)
        {
            if (_config.CORE.Notification.Global.enableBroadcast && LevelFunctions.CanLog(player))
            {
                Server.Broadcast($"{TextEncodeing(16, SignatureColor, Instance.Name, true)}{TextEncodeing(16, "ffffff", ":", false)} {TextEncodeing(16, "ffffff", message, false)}", _config.NOTIFICATION.icon.ToString() != "-1" ? Convert.ToUInt64(_config.NOTIFICATION.icon) : 0);
            }
        }

        private void ShowPopup(BasePlayer player, string Message, int Duration)
        {
            if (_config.CORE.Notification.Self.enablePopup)
            {
                player.SendConsoleCommand($"gametip.showgametip \"{Message}\"");
                timer.Once(Duration, () => { player.SendConsoleCommand("gametip.hidegametip"); });
            }
        }
        private static void DirectMessage(BasePlayer player, string message)
        {
            try { player.SendConsoleCommand("chat.add", 0, _config.NOTIFICATION.icon, $"{TextEncodeing(16, SignatureColor, Instance.Name, true)}{TextEncodeing(16, "ffffff", ":", false)} {TextEncodeing(16, "ffffff", message, false)}"); }
            catch { Instance.SendReply(player, message); }
        }
        private static string TextEncodeing(double TextSize, string HexColor, string text, bool bold)
        {
            var s = TextSize <= 0 ? 1 : TextSize;
            var c = HexColor.StartsWith("#") ? HexColor : $"#{HexColor}";
            if (bold) { return $"<size={s}><color={c}><b>{text}</b></color></size>"; }
            else { return $"<size={s}><color={c}>{text}</color></size>"; }
        }
        #endregion Notification Functions

        #region Oxide Hooks 
        void OnNewSave(string filename)
        {
            //Wipe StaffTime Data IF Enabled
            if (_config.LOGGING.StaffTime.resetOnWipe) { Interface.Oxide.DataFileSystem.WriteObject($"{Instance.Name}/StaffTime", new Dictionary<ulong, StaffTime>()); }
        }

        void OnServerSave()
        {
            StaffSaveFile();
        }

        void OnPlayerConnected(BasePlayer player) => UnloadPlayer(player);
        void OnPlayerDisconnected(BasePlayer player) => UnloadPlayer(player);

        object OnPlayerViolation(BasePlayer player, AntiHackType type, float amount)
        {
            if ((type == AntiHackType.InsideTerrain || type == AntiHackType.FlyHack) && HasPerm.AnyPerm(player)) { return false; }
            return null;
        }

        void OnPlayerBanned(string name, ulong id, string address, string reason)
        {
            var player = BasePlayer.allPlayerList.Where(x => x.userID == id).FirstOrDefault();
            if ((HasPerm.AnyPerm(player) || HasPerm.BothPerm(player)) && reason.ToLower().Contains("cheat"))
            {
                ServerUsers.Set(player.userID, ServerUsers.UserGroup.None, player.displayName, "AdminToggle Unban Admin");
                ServerUsers.Save();
                PrintWarning($"[WARNING][EAC] Admin tired toggle while flying\nUnbanning: {player.displayName} ({player.userID})");
                return;
            }
        }
        #endregion Oxide Hooks

        private void UnloadPlayer(BasePlayer player)
        {
            if (HasPerm.AnyPerm(player))
            {
                ToggleMode.ForceSetPlayer(player); //UnToggle Admin Mode 
                CuiHelper.DestroyUi(player, "AdminToggleButton");
                CuiHelper.DestroyUi(player, "AdminTogglePulsePanel");
                CuiHelper.DestroyUi(player, "AdminToggleMenu");
                UI.PanelStopPulsing(player);
                PlayerLocations.Remove(player.userID);
                UItimers.Remove(player.userID);
                PlayerState.Remove(player.userID);
                StaffTimeSave(player, true);
            }
        }
        void Unload()
        {
            foreach (BasePlayer player in BasePlayer.allPlayerList.Where(x => HasPerm.AnyPerm(x)))
            {
                try
                {
                    UnloadPlayer(player);
                }
                catch { }
            }

            try
            {
                thirdParty.UnSubscribe();
                StaffSaveFile();
                _staffTime.Clear();
            } catch { }
        }
    }
}