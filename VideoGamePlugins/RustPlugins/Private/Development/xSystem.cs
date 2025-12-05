
using Oxide.Core;
using Oxide.Game.Rust.Cui;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using UnityEngine;
using xSystem.Extensions;

namespace xSystem.Extensions
{
    public static class ExtensionMethods
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns><see cref="String"/> first <see cref="Char"/> of <paramref name="value"/></returns>
        public static string FirstToUpper(this string value) => (!String.IsNullOrEmpty(value) && value.Length > 1) ? char.ToUpper(value[0]) + value.Substring(1).ToLower() : value.ToUpper();
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="index"></param>
        /// <returns>Value in array at <paramref name="index"/> as <see cref="String"/></returns>
        public static string Index<T>(this T[] value, int index = 0) => (value != null && value.Length > index) ? (value[index] as String) : String.Empty;

        /// <summary>
        /// Supports: <see cref="object"/>, <see cref="IDictionary"/>, <see cref="IEnumerable"/>, <see cref="Type.IsValueType"/>, <see cref="String"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns><see cref="bool"/> based on the <typeparamref name="T"/> <paramref name="value"/></returns>
        public static bool IsEmpty<T>(this T value)
        {
            if (value == null) { return true; }
            if (value is IDictionary) { return (value as IDictionary).Count == 0; }
            if (value is IEnumerable) { return !(value as IEnumerable).GetEnumerator().MoveNext(); }
            if (typeof(T).IsValueType) { return value.Equals(default(T)); }
            if (typeof(T) == typeof(String)) { return String.IsNullOrWhiteSpace(value as String); }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="types"></param>
        /// <returns><see cref="bool"/> based on if <typeparamref name="T"/> is any <see cref="Type"/> in <paramref name="types"/></returns>
        public static bool IsType<T>(this T obj, params Type[] types) where T : class
        {
            bool isType = false;
            for (int i = 0; i < types.Length; i++)
            {
                if (types[i].IsAssignableFrom(obj.GetType())) { isType = true; break; }
            }
            return isType;
        }
    }
}

namespace Oxide.Plugins
{
    [Info("xSystem", "InfinityNet/Xray", "2.0.0")]
    [Description("Core system of Infinitynet servers")]
    internal class xSystem : RustPlugin
    {
        #region Configuration
        private static bool ConsoleConfigurationCommander(ConsoleSystem.Arg console)
        {
            Helper.Commands.Console.TryRun(() =>
            {
                var selector1 = console.Args.Index(0);
                var selector2 = console.Args.Index(1);
                var selector3 = console.Args.Index(2);
                var selector4 = console.Args.Index(3);
                var selector5 = console.Args.Index(4);
                var selector6 = console.Args.Index(5);
                var selector7 = console.Args.Index(6);
                var selector8 = console.Args.Index(7);
                var selector9 = console.Args.Index(8);

                bool isCached = (selector3 != "raw" && selector3 != "-r" && selector3 != "r");
                switch (selector1)
                {
                    case "set":
                    case "-s":
                    case "s": 
                    switch (selector3)
                    {
                        case "state":
                        case "-s":
                        case "s":
                            Configuration.ConfigHelper.Loader.SetState(selector2, Configuration.ConfigHelper.Loader.String2Bool(selector4));
                        break;

                        case "build":
                        case "-b":
                        case "b":
                            Configuration.ConfigHelper.Loader.SetBuild(selector2, Configuration.ConfigHelper.Loader.String2Build(selector4));
                        break;
                    }
                    break;

                    case "get":
                    case "-g":
                    case "g": 
                    switch (selector2)
                    {
                        case "all":
                        case "-a":
                        case "a":
                            Instance?.PrintWarning("List of configurations");
                            var totalConfigs = Configuration.ConfigHelper.GetConfigs(isCached).ToArray();
                            foreach (var cfg in totalConfigs.OrderByDescending(c => c.State))
                            {
                                Configuration.ConfigHelper.PrintCfg(cfg.Title, isCached);
                            }
                        break;

                        default:
                            Configuration.ConfigHelper.PrintCfg(selector2, isCached);
                        break;
                    }
                    break;
                }
            }, new Helper.Commands.Settings { Token = Configuration.Settings.Tokens.ConsoleCommandToken, Server = true, }, console);
            return false;
        }
        private static class Configuration
        {
            internal static class Settings
            {
                [Configuration("6.5.0", true, BuildType.ConstantProduction)]
                internal sealed class Groups
                {
                    #region Groups
                    internal const string Administrator = "staff_administrator";
                    internal const string Moderator = "staff_moderator";
                    internal const string Helper = "staff_helper";

                    internal const string Emerald = "vip_emerald";
                    internal const string Diamond = "vip_diamond";
                    internal const string Platinum = "vip_platinum";
                    internal const string Gold = "vip_gold";
                    internal const string Silver = "vip_silver";
                    internal const string Bronze = "vip_bronze";

                    internal const string Playtime = "playtime_vip";

                    internal const string Verified = "link_verified";
                    internal const string Steam = "link_steam";
                    internal const string Discord = "link_discord";

                    internal const string Default = "default";
                    #endregion Groups

                    internal sealed class GroupSettings
                    {
                        internal ulong[] AllowedIds;
                        internal string[] AllowedIPs;
                        internal string[] RequiredGroups;
                        internal int RequiredAuth;
                    }
                    internal static readonly ReadOnlyDictionary<string, GroupSettings> groups = new ReadOnlyDictionary<string, GroupSettings>(new Dictionary<string, GroupSettings>()
                    {
                        { Administrator, new GroupSettings {
                            AllowedIds = new ulong[] { 76561198066857380 },
                            RequiredAuth = 2,
                        }},
                        { Moderator, new GroupSettings {
                            AllowedIds = new ulong[] { 76561198023076226 }
                        }},
                        { Helper, new GroupSettings {
                            AllowedIds = new ulong[] { 76561198196806881 },
                        }},


                        { Verified, new GroupSettings {
                            RequiredGroups = new string[] { Steam, Discord },
                        }},
                    });
                }

                [Configuration("6.5.0", true, BuildType.ConstantProduction)]
                internal static class Tokens
                {
                    internal const string ConsoleCommandToken = "test1234";
                    internal const string ChatCommandToken = "test1234";
                }
            }
            #region Core
            [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
            internal sealed class ConfigurationAttribute : Attribute
            {
                internal SettingsData Data { get; }

                internal ConfigurationAttribute(string version, bool enabled, BuildType build = BuildType.Development, [CallerMemberName] string caller = null)
                {
                    if(String.IsNullOrEmpty(caller)) { return; }
                    this.Data = new SettingsData(caller, enabled, version, build);
                }
            }

            internal enum BuildType
            {
                /// <summary>
                /// Same as <see cref="BuildType.Production"/> (Expect this cannot be changed)
                /// </summary>
                ConstantProduction = 3,
                /// <summary>
                /// Same as <see cref="BuildType.Development"/> (Expect this cannot be changed)
                /// </summary>
                ConstantDevelopment = 2,
                /// <summary>
                /// Enables all features (Only use for LIVE builds)
                /// </summary>
                Production = 1,
                /// <summary>
                /// Disables all features associated with <see cref="BuildType.Production"/> (This is a safe option for non-live builds)
                /// </summary>
                Development = 0,
            }
            internal struct Version
            {
                internal ushort Major { get; }
                internal ushort Minor { get; }
                internal ushort Patch { get; }

                private Version(ushort major, ushort minor, ushort patch)
                {
                    this.Major = major;
                    this.Minor = minor;
                    this.Patch = patch;
                }

                internal static Version Default => new Version(1, 0, 0);
                public static implicit operator string(Version v) => $"{v.Major}.{v.Minor}.{v.Patch}";

                internal static Version FromString(string version)
                {
                    if (String.IsNullOrEmpty(version)) { return Default; }

                    var Version = version.Split('.');
                    if (Version.Length < typeof(Version).GetProperties(BindingFlags.GetProperty | BindingFlags.NonPublic | BindingFlags.Instance).Length) { return Default; }

                    ushort major = Default.Major;
                    ushort minor = Default.Minor;
                    ushort patch = Default.Patch;
                    if (!ushort.TryParse(Version[0], out major) ||
                        !ushort.TryParse(Version[1], out minor) ||
                        !ushort.TryParse(Version[2], out patch)) { return Default; }
                    return new Version(major, minor, patch);
                }
            }
            internal sealed class SettingsData
            {
                internal string Title { get; private set; }
                public bool State { get; private set; }
                internal string Version { get; private set; }
                public BuildType Build { get; private set; }
                public SettingsData(string title, bool state = false, string version = "1.0.0", BuildType build = BuildType.Development)
                {
                    this.Title = title;
                    this.State = state;
                    this.Version = Configuration.Version.FromString(version);
                    this.Build = build;
                }
            }
            #endregion Core
            internal static class ConfigHelper
            {
                private static Type[] configs = typeof(Settings)?.GetNestedTypes(BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) ?? null;

                internal static ConfigLoader Loader = new ConfigLoader();
                internal sealed class ConfigLoader : Helper.DataFiles<Dictionary<string, SettingsData>>
                {
                    private Dictionary<string, SettingsData> ConfigData = new Dictionary<string, SettingsData>();
                    private void Validate()
                    {
                        var configs = GetConfigs();
                        if (ConfigData == null || ConfigData.Count < 1) /*Check ConfigData*/
                        {
                            Helper.Logger.Log($"Data validation failed", "Configs");
                            Reset(ref ConfigData, "Configs");
                            foreach (var config in configs)
                            {
                                if (ConfigData.ContainsKey(config.Title)) { continue; }
                                ConfigData.Add(config.Title, config);
                            }
                            Helper.Logger.Log($"Data validation reset", "Configs");
                        }

                        foreach (var config in ConfigData.ToArray()) /*Check Invalid Cached Configs*/
                        {
                            bool isValid = configs.Any(cfg => cfg.Title == config.Key);
                            if (isValid) { continue; }
                            ConfigData.Remove(config.Key);
                        }

                        foreach (var config in configs) /*Add Missing Configs*/
                        {
                            if (ConfigData.ContainsKey(config.Title)) { continue; }
                            ConfigData.Add(config.Title, config);
                        }

                        foreach (var config in configs) /*Check Invalid BuildType*/
                        {
                            SettingsData cfg;
                            if (!ConfigData.TryGetValue(config.Title, out cfg)) { continue; }
                            bool success = Enum.IsDefined(typeof(BuildType), cfg.Build);
                            if (!success) { SetBuild(config.Title); }
                        }
                        Helper.Logger.Log($"Data validation success", "Configs");
                    }

                    internal void SetState(string configName, bool state = false)
                    {
                        var cachedCfw = ConfigHelper.GetConfig(configName);
                        var rawCfg = ConfigHelper.GetConfig(configName, false);

                        if (cachedCfw == null || rawCfg == null || !ConfigData.ContainsKey(configName.FirstToUpper())) { Helper.Logger.Log("Config not found", "Configuration"); return; }
                        if ((int)rawCfg.Build >= 2) { Helper.Logger.Log("Cannot change a constant config", "Configuration"); return; }

                        ConfigData[configName.FirstToUpper()] = new SettingsData(rawCfg.Title, state, rawCfg.Version, cachedCfw.Build);
                        Helper.Logger.Log($"[{rawCfg.Title}] state changed [{rawCfg.State} -> {state}]", "Configuration", Helper.Logger.Type.Important);
                    }
                    internal void SetBuild(string configName, BuildType build = BuildType.Development)
                    {
                        var cachedCfw = ConfigHelper.GetConfig(configName);
                        var rawCfg = ConfigHelper.GetConfig(configName, false);

                        if (cachedCfw == null || rawCfg == null || !ConfigData.ContainsKey(configName.FirstToUpper())) { Helper.Logger.Log("Config not found", "Configuration"); return; }
                        if ((int)rawCfg.Build >= 2) { Helper.Logger.Log("Cannot change a constant config", "Configuration"); return; }

                        ConfigData[configName.FirstToUpper()] = new SettingsData(rawCfg.Title, cachedCfw.State, rawCfg.Version, build);
                        Helper.Logger.Log($"[{rawCfg.Title}] build changed [{rawCfg.Build} -> {build}]", "Configuration", Helper.Logger.Type.Important, true);
                    }



                    internal bool String2Bool(string input)
                    {
                        switch (input.ToLower())
                        {
                            case "enabled":
                            case "on":
                                return true;
                        }
                        return false;
                    }
                    internal BuildType String2Build(string input)
                    {
                        switch (input.ToLower())
                        {
                            case "production":
                            case "prod":
                            case "p":
                                return BuildType.Production;
                        }
                        return BuildType.Development;
                    }

                    internal ValueTuple<bool, BuildType> GetCachedConfig(string configName)
                    {
                        var Default = new ValueTuple<bool, BuildType>(false, BuildType.Development);
                        if (String.IsNullOrEmpty(configName)) { return Default; }
                        SettingsData cfg; if (!ConfigData.TryGetValue(configName.FirstToUpper(), out cfg)) { return Default; }
                        return new ValueTuple<bool, BuildType>(cfg.State, cfg.Build);
                    }

                    internal void Load()
                    {
                        ConfigData = Load("settings", "Configs");
                        Validate();
                        Save(true);
                    }
                    internal void Save(bool silent = false)
                    {
                        Save(ConfigData, "settings", "Configs", silent);
                    }
                }
                internal static void PrintCfg(string configName, bool isCached = true)
                {
                    var cfg = Configuration.ConfigHelper.GetConfig(configName, isCached);
                    if (cfg == null) { Helper.Logger.Log($"Config \'{configName.FirstToUpper()}\' not found", "Configuration"); return; }
                    string state = cfg.State ? "Loaded" : "Disabled";
                    Instance?.Puts($"[{cfg.Title}] v{(string)cfg.Version} {cfg.Build} {state}");
                }



                internal static SettingsData GetConfig(string configName, bool cached = true)
                {
                    if (configs == null || configs.Length < 1) { return null; }
                    if (String.IsNullOrEmpty(configName)) { return null; }

                    var config = configs.FirstOrDefault(cfg => cfg.Name.FirstToUpper() == configName.FirstToUpper());
                    if (config == null) { return null; }

                    var defaultConfig = config?.GetCustomAttribute<ConfigurationAttribute>()?.Data ?? null;
                    if (defaultConfig == null) { return null; }

                    var cachedConfig = Loader.GetCachedConfig(configName);
                    return cached ? new SettingsData(defaultConfig.Title, cachedConfig.Item1, defaultConfig.Version, cachedConfig.Item2) : defaultConfig;
                }
                internal static SettingsData GetConfig(Type configType, bool cached = true)
                {
                    if (configs == null || configs.Length < 1) { return null; }
                    if (configType == null) { return null; }

                    var config = configs.FirstOrDefault(cfg => cfg == configType);
                    if (config == null) { return null; }

                    var defaultConfig = config?.GetCustomAttribute<ConfigurationAttribute>()?.Data ?? null;
                    if (defaultConfig == null) { return null; }

                    var cachedConfig = Loader.GetCachedConfig(defaultConfig.Title);
                    return cached ? new SettingsData(defaultConfig.Title, cachedConfig.Item1, defaultConfig.Version, cachedConfig.Item2) : defaultConfig;
                }
                internal static IEnumerable<SettingsData> GetConfigs(bool cached = true)
                {
                    if (configs == null || configs.Length < 1) { yield return null; }
                    foreach (Type cfg in configs)
                    {
                        var config = GetConfig(cfg, cached);
                        if (config == null) { continue; }
                        yield return config;
                    }
                }
            }
        }
        #endregion Configuration
        #region Init
        internal static xSystem Instance { get; private set; } = new xSystem();
        private const string Prefix = "xsystem";
        private static class InitCore
        {
            internal static class Commands
            {
                private static HashSet<CommandData> commands = new HashSet<CommandData>();
                internal struct CommandData
                {
                    internal string Command;
                    internal CommandType Type;
                    internal string[] Args;
                }
                internal enum CommandType
                {
                    Console,
                    Chat
                }
                internal static void AddCachedCommand(CommandData command) => commands.Add(command);
                internal static void RemoveCachedCommand(string command)
                {
                    var targetCmd = commands.FirstOrDefault(cmd => cmd.Command == command);
                    if (targetCmd.Equals(default(CommandData))) { return; }
                    commands?.Remove(targetCmd);
                }
                internal static void RegisterCommands()
                {
                    Helper.Commands.Console.Register("cfg", ConsoleConfigurationCommander,
                        "set CONFIG state <VALUE:on|enabled> (No value -> False)",
                        "set CONFIG build <VALUE:production|prod|p> (No value -> Development)",
                        "get <CONFIG|all> raw"
                    );
                }
                internal static void RevokeCommands()
                {
                    foreach (var cmd in commands)
                    {
                        try
                        {
                            switch (cmd.Type)
                            {
                                //case CommandType.Console: Helper.Commands.Console.Revoke(cmd.Command); break;
                                case CommandType.Chat: Helper.Commands.Chat.Revoke(cmd.Command); break;
                            }
                        }
                        catch { continue; }
                    }
                }
                internal static CommandData[] List => commands.ToArray();
            }
        }

        [ConsoleCommand(Prefix)]
        private void ConsoleCoreCommander(ConsoleSystem.Arg console)
        {
            Helper.Commands.Console.TryRun(() =>
            {
                var selector1 = console.Args.Index(0);
                var selector2 = console.Args.Index(1);
                var selector3 = console.Args.Index(2);
                var selector4 = console.Args.Index(3);
                var selector5 = console.Args.Index(4);
                var selector6 = console.Args.Index(5);
                var selector7 = console.Args.Index(6);
                var selector8 = console.Args.Index(7);
                var selector9 = console.Args.Index(8);

                Instance?.PrintWarning("Console comamnd not found. Here is a list of registered commands.");
                foreach (var cmd in InitCore.Commands.List)
                {
                    Instance?.PrintWarning($"\n==={cmd.Command.Replace(Prefix, "").Replace(".", "").ToUpper()}===");
                    foreach (var arg in cmd.Args)
                    {
                        Instance?.Puts($"{cmd.Command} {arg}");
                    }
                }

            }, new Helper.Commands.Settings { Token = Configuration.Settings.Tokens.ConsoleCommandToken, Server = true, }, console);
        }

        private void Loaded()
        {
            Instance = this;
            #region Init
            InitCore.Commands.RegisterCommands();
            #endregion Init
            #region Configs
            Configuration.ConfigHelper.Loader.Load();

            var totalConfigs = Configuration.ConfigHelper.GetConfigs(true).ToArray();
            var loadedConfigs = totalConfigs.Where(cfg => cfg.State).ToArray();
            Instance?.PrintWarning($"v{this.Version.ToString()} Instantiated {loadedConfigs.Length}/{totalConfigs.Length} Configurations");
            foreach (var cfg in totalConfigs.OrderByDescending(c => c.State))
            {
                Configuration.ConfigHelper.PrintCfg(cfg.Title);
            }
            #endregion Configs
        }
        private void Unload()
        {
            #region Init
            InitCore.Commands.RevokeCommands();
            #endregion Init
            #region Configs
            Configuration.ConfigHelper.Loader.Save();
            #endregion Configs
        }
        #endregion Init

        #region Localization
        internal static class Msg
        {
            #region Core
            internal const char H = '%'; /*Highlight Color*/
            internal const char B = '@'; /*Bold Text*/
            internal const char I = '~'; /*Italic Text*/
            internal const char S = '^'; /*Size Text*/
            private const string Pattern = @"(?<=\{0}).*?(?=\{0})";
            internal static string DoStyle(string text, FormatSettings settings = null)
            {
                if (!String.IsNullOrEmpty(settings?.Color))
                {
                    string color = settings.Color.StartsWith("#") ? settings.Color.Replace("#", "") : settings.Color;
                    text = Regex.Replace(text, String.Format(Pattern, H), match => $"<color=#{color}>{match.Value}</color>");
                }
                if (settings?.Bold ?? false)
                {
                    text = Regex.Replace(text, String.Format(Pattern, B), match => $"<b>{match.Value}</b>");
                }
                if (settings?.Italic ?? false)
                {
                    text = Regex.Replace(text, String.Format(Pattern, I), match => $"<i>{match.Value}</i>");
                }
                if ((int)(settings?.Size ?? 0) > 0)
                {
                    text = Regex.Replace(text, String.Format(Pattern, S), match => $"<size={settings.Size}>{match.Value}</size>");
                }

                return text.
                    Replace($"{H}", "").
                    Replace($"{B}", "").
                    Replace($"{I}", "").
                    Replace($"{S}", "");
            }
            /// <summary>
            /// <list type="table">Color %</list>
            /// <list type="table">Bold @</list>
            /// <list type="table">Italic ~</list>
            /// <list type="table">Size ^</list>
            /// </summary>
            internal sealed class FormatSettings
            {
                /// <summary>
                /// Symbol %
                /// </summary>
                internal string Color;
                /// <summary>
                /// Symbol @
                /// </summary>
                internal bool Bold;
                /// <summary>
                /// Symbol ~
                /// </summary>
                internal bool Italic;
                /// <summary>
                /// Symbol ^
                /// </summary>
                internal int Size;
            }
            #endregion Core

            internal static class Genetic
            {
                internal static string UnkownCommand(string command) => DoStyle(command, new FormatSettings { Size = 14, });
                internal static string NotFound(string obj, string value = null, FormatSettings settings = null)
                {
                    if (String.IsNullOrEmpty(value)) { return $"{DoStyle($"{obj.FirstToUpper()} not found", settings)}"; }
                    return $"{DoStyle($"{obj.FirstToUpper()} {H}\'{value}\'{H} not found", settings)}";
                }
                internal static string Toggle(string obj, bool enabled, FormatSettings settings = null)
                {
                    if (enabled) { return $"{DoStyle($"{obj.FirstToUpper()} {H}activated{H}!", settings)}"; }
                    return $"{DoStyle($"{obj.FirstToUpper()} {H}disabled{H}!", settings)}";
                }
            }
        }
        #endregion Localization



        [ChatCommand("ui")]
        private void TestUI1(BasePlayer player)
        {
            CuiElementContainer Container = new CuiElementContainer();
            Helper.UI.Elements.Control control = new Helper.UI.Elements.Control()
                .AddPanel(Helper.UI.Core.Anchor.Center, new Helper.UI.Core.Settings
                {
                    Color = new Helper.UI.Elements.CuiColor("000000")
                }, new Helper.UI.Core.Offset(100, 100), new Helper.UI.Core.Fade());
            Helper.UI.Draw(player, control.UI);
        }
        [ChatCommand("no")]
        private void TestUI2(BasePlayer player)
        {
            Helper.UI.Destroy(player);
        }

        #region Utility
        private static class Helper
        {
            internal static class UI
            {
                internal static class Core
                {
                    internal struct Anchor
                    {
                        internal readonly float xMin, yMin, xMax, yMax;
                        internal Anchor(float xMin, float yMin, float xMax, float yMax)
                        {
                            this.xMin = xMin;
                            this.yMin = yMin;
                            this.xMax = xMax;
                            this.yMax = yMax;
                        }

                        internal string Min => $"{this.xMin} {this.yMin}";
                        internal string Max => $"{this.xMax} {this.yMax}";


                        internal static Anchor Center => new Anchor(0.5f, 0.5f, 0.5f, 0.5f);
                        internal static Anchor TopCenter => new Anchor(0.5f, 1f, 0.5f, 1f);
                        internal static Anchor TopLeft => new Anchor(0f, 1f, 0f, 1f);
                        internal static Anchor TopRight => new Anchor(1f, 1f, 1f, 1f);
                        internal static Anchor BottomCenter => new Anchor(0.5f, 0f, 0.5f, 0f);
                        internal static Anchor BottomLeft => new Anchor(0f, 0f, 0f, 0f);
                        internal static Anchor BottomRight => new Anchor(1f, 0f, 1f, 0f);
                        internal static Anchor LeftCenter => new Anchor(0f, 0.5f, 0f, 0.5f);
                        internal static Anchor RightCenter => new Anchor(1f, 0.5f, 1f, 0.5f);
                    }
                    internal struct Offset
                    {
                        internal readonly int Width, Height, X, Y;
                        internal Offset(int width, int height, int x = 0, int y = 0)
                        {
                            this.Width = width;
                            this.Height = height;
                            this.X = x;
                            this.Y = y;
                        }

                        internal string Min => $"{-((this.Width / 2) + this.X)} {-((this.Height / 2) + this.Y)}";
                        internal string Max => $"{((this.Width / 2) - this.X)} {((this.Height / 2) - this.Y)}";

                        internal static Offset Default => new Offset(700, 500);
                    }
                    internal enum Layers
                    {
                        Overall = 4,
                        Overlay = 3,
                        Hud = 2,
                        HudMenu = 1,
                        Under = 0
                    }
                    internal enum Controls
                    {
                        None,
                        Mouse,
                        Keyboard,
                        Both
                    }
                    internal struct Fade
                    {
                        internal readonly float Delay;
                        internal readonly FadeType Type;
                        internal Fade(float delay, FadeType type = FadeType.Both)
                        {
                            this.Delay = delay;
                            this.Type = type;
                        }
                        internal enum FadeType
                        {
                            None,
                            In,
                            Out,
                            Both,
                        }
                    }
                    internal struct Settings
                    {
                        internal Elements.CuiColor Color;
                        internal string Material;
                        internal string Sprite;
                        internal UnityEngine.UI.Image.Type SpriteType;
                    }
                }

                internal static void Destroy(BasePlayer player, params string[] elements)
                {
                    if (!PlayerEx.IsPlayer(player)) { return; }

                    if (elements != null && elements.Length > 0)
                    {
                        foreach (var element in elements)
                        {
                            CuiHelper.DestroyUi(player, element);
                        }
                    }
                    else
                    {
                        foreach (var field in typeof(Elements.Names).GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static))
                        {
                            CuiHelper.DestroyUi(player, field.GetValue(field).ToString());
                        }
                    }
                }
                internal static void Draw(BasePlayer player, string Json)
                {
                    Destroy(player);
                    CuiHelper.AddUi(player, Json);
                }

                internal static class Elements
                {
                    internal class Names
                    {
                        /// <summary>
                        /// Used as the main Element
                        /// </summary>
                        internal const string RootContainer = Prefix + ".RootContainer";
                        internal const string Panel = Prefix + ".Panel";
                    }

                    internal struct CuiColor
                    {
                        internal readonly float Alpha;
                        internal readonly string HexColor;
                        internal CuiColor(string HexColor, float Alpha = 1)
                        {
                            this.Alpha = Alpha;
                            this.HexColor = HexColor;
                        }
                        public static implicit operator string(CuiColor Color)
                        {
                            string HexColor = Color.HexColor.StartsWith("#") ? Color.HexColor : $"#{Color.HexColor}";
                            System.Drawing.Color RGBColor = ColorTranslator.FromHtml(HexColor);
                            string CuiColor = $"{(RGBColor.R / (float)byte.MaxValue)} {(RGBColor.G / (float)byte.MaxValue)} {(RGBColor.B / (float)byte.MaxValue)} {Mathf.Clamp(Color.Alpha, 0, 1)}";
                            return CuiColor;
                        }
                    }


                    internal sealed class Control
                    {
                        private CuiElementContainer Container = new CuiElementContainer();

                        internal string UI => Container.ToJson();

                        internal Control AddPanel(Core.Anchor anchor, Core.Settings settings, Core.Offset offset = default(Core.Offset), Core.Fade fade = default(Core.Fade), Core.Controls control = Core.Controls.None)
                        {
                            var element = new CuiPanel()
                            {
                                RectTransform =
                                {
                                    AnchorMin = anchor.Min,
                                    AnchorMax = anchor.Max,
                                    OffsetMin = offset.Min,
                                    OffsetMax = offset.Max,
                                },
                                Image =
                                {
                                    Color = settings.Color,
                                    Material = settings.Material,
                                    Sprite = settings.Sprite,
                                    ImageType = settings.SpriteType,
                                }

                            };
                            switch (control)
                            {
                                case Core.Controls.Mouse:
                                    {
                                        element.CursorEnabled = true;
                                    }
                                    break;
                                case Core.Controls.Keyboard:
                                    {
                                        element.KeyboardEnabled = true;
                                    }
                                    break;
                                case Core.Controls.Both:
                                    {
                                        element.CursorEnabled = true;
                                        element.KeyboardEnabled = true;
                                    }
                                    break;
                            }
                            switch (fade.Type)
                            {
                                case Core.Fade.FadeType.In:
                                    {
                                        element.Image.FadeIn = fade.Delay;
                                    }
                                    break;
                                case Core.Fade.FadeType.Out:
                                    {
                                        element.FadeOut = fade.Delay;
                                    }
                                    break;
                                case Core.Fade.FadeType.Both:
                                    {
                                        element.Image.FadeIn = fade.Delay;
                                        element.FadeOut = fade.Delay;
                                    }
                                    break;
                            }
                            return this;
                        }
                    }





                }

                internal static class Sprites
                {

                }

                internal static class Materials
                {

                }
            }


            internal static class PlayerEx
            {
                internal static bool HasPerm(ulong id, string permission) => Instance?.permission?.UserHasPermission($"{id}", permission) ?? false;

                internal static bool HasGroups(BasePlayer player, params string[] groups)
                {
                    if (!IsPlayer(player) || groups == null || groups.Length < 1) { return false; }
                    var hasGroups = groups.All(group => HasGroup(player, group));
                    return hasGroups;
                }
                internal static bool HasGroup(BasePlayer player, string group)
                {
                    if (!IsPlayer(player)) { return false; }
                    Configuration.Settings.Groups.GroupSettings GroupData;
                    if (!Configuration.Settings.Groups.groups.TryGetValue(group, out GroupData)) { return false; }

                    var id = player.userID;
                    var ip = player.Connection.IPAddressWithoutPort();
                    var groups = Instance?.permission?.GetUserGroups(player.UserIDString) ?? new string[0];
                    var auth = player.Connection.authLevel;

                    if (GroupData?.AllowedIds?.Contains(id) == false ||
                        GroupData?.AllowedIPs?.Contains(ip) == false ||
                        !groups.All(x => GroupData?.RequiredGroups?.Contains(x) == true) ||
                        auth < GroupData?.RequiredAuth)
                    {
                        return false;
                    }

                    return Instance?.permission?.UserHasGroup(player.UserIDString, group) ?? false;
                }

                internal enum GametipType
                {
                    Info = '0',
                    Warning = '1'
                }
                internal static void ShowGametip(BasePlayer player, string Message, GametipType type = GametipType.Info) => player.SendConsoleCommand($"gametip.showtoast {(char)type} \"{Message}\"");

                internal struct CustomItem
                {
                    internal string shortname { get; private set; }
                    private uint _quantity;
                    internal uint quantity
                    {
                        get { return _quantity; }
                        private set { _quantity = (uint)Mathf.Clamp(value, uint.MinValue + 1, uint.MaxValue - 1); }
                    }
                    internal ulong skinid { get; private set; }
                    internal List<CustomItem> mods { get; private set; }
                }
                internal static bool GiveItem(BasePlayer player, CustomItem item, ItemContainer container = null)
                {
                    var mainItem = ItemManager.CreateByName(item.shortname, (int)item.quantity, item.skinid);
                    if (mainItem == null) { return false; }
                    if (item.mods != null && item.mods.Count > 0)
                    {
                        item.mods.ForEach((mod) =>
                        {
                            var modItem = ItemManager.CreateByName(mod.shortname, (int)mod.quantity, mod.skinid);
                            if (modItem != null)
                            {
                                modItem.MoveToContainer(mainItem.contents);
                                modItem.MarkDirty();
                            }
                        });
                    }
                    mainItem.MarkDirty();
                    player.inventory.GiveItem(mainItem, (container ?? null));
                    player.inventory.ServerUpdate(0);
                    return true;
                }

                internal static bool IsPlayer(BasePlayer player, bool RequireConnected = false) => player != null && (player.userID.IsSteamId() || player.UserIDString.IsSteamId()) && (RequireConnected && player.IsConnected);
                internal static bool IsPlayer(ulong id) => id.IsSteamId();
                internal static bool IsPlayer(string id) => id.IsSteamId();

                internal static bool IsTeam(BasePlayer player, ulong target)
                {
                    if (!IsPlayer(player, true) || !IsPlayer(target)) { return false; }
                    if (player.userID == target) { return true; }
                    if (player.Team == null || player.Team.members.Count <= 1) { return false; }
                    return player.Team.members.Contains(target);
                }

                internal static BasePlayer ToPlayer(string name) => BasePlayer.FindAwakeOrSleeping(name) ?? null;
                internal static BasePlayer ToPlayer(ulong id) => BasePlayer.FindAwakeOrSleeping(id.ToString()) ?? null;
            }
            internal static class Logger
            {
                internal enum Type
                {
                    Info,
                    Warning,
                    Error,
                    Important,
                }
                internal static void Log(string message, string prefix, Type logType = Type.Info, bool disk = true)
                {
                    string log = $"[{logType.ToString().ToUpper()}] [{prefix}] {message}";
                    if (logType == Type.Info) { Instance?.Puts(log); } else { Instance?.PrintWarning(log); }
                    if (disk) { Instance?.LogToFile($"_{logType}", $"{DateTime.Now.ToString("yyyy:MM:dd  HH:mm:ss")}  {log}", (Instance ?? null), false); }
                }
            }
            internal abstract class DataFiles<T> where T : class
            {
                private static T Default => Activator.CreateInstance<T>();

                internal static void Reset(ref T data, string logName, bool silent = false)
                {
                    try
                    {
                        //var list = Facepunch.Pool.Free<T>();
                        data = Default;
                        if (!silent) { Logger.Log($"Data reset success", logName, Logger.Type.Info); }
                    }
                    catch (Exception ex)
                    {
                        if (!silent) { Logger.Log($"Data reset failed ({ex.Message})", logName, Logger.Type.Error); }
                    }
                }
                internal static T Load(string fileName, string logName, bool silent = false)
                {
                    T data = Default;
                    try
                    {
                        data = Interface.Oxide.DataFileSystem.ReadObject<T>($"{Prefix}/{fileName}");
                        if (data == null)
                        {
                            if (!silent) { Logger.Log($"Data load empty", logName, Logger.Type.Warning); }
                            return Default;
                        }

                        if (!silent) { Logger.Log($"Data load success", logName, Logger.Type.Info); }
                        return data;
                    }
                    catch (Exception ex)
                    {
                        if (!silent) { Logger.Log($"Data load failed ({ex})", logName, Logger.Type.Error); }
                        return Default;
                    }
                }
                internal static void Save(T data, string fileName, string logName, bool silent = false)
                {
                    try
                    {
                        if (data.IsEmpty()) { Logger.Log($"Data save aborted", logName, Logger.Type.Info); return; }
                        Interface.Oxide.DataFileSystem.WriteObject<T>($"{Prefix}/{fileName}", data);
                        if (!silent) { Logger.Log($"Data save success", logName, Logger.Type.Info); }
                    }
                    catch (Exception ex)
                    {
                        if (!silent) { Logger.Log($"Data save failed ({ex.Message})", logName, Logger.Type.Error); }
                    }
                }
            }
            internal static class Commands
            {
                internal struct Settings
                {
                    internal string Token;
                    internal bool Server;
                    internal string[] IP;
                    internal ulong[] ID;
                    internal string[] Group;
                    internal string[] Perm;
                    internal uint Auth;
                }

                private static bool ValidateSettings<T>(string Token, Settings settings, string prefix, T sender, Action callback) where T : class
                {
                    if (settings.Equals(default(Settings))) { Logger.Log("Settings Invalid", prefix, Logger.Type.Warning); return false; }
                    if (settings.Token != Token) { Logger.Log("Token Invalid", prefix, Logger.Type.Warning); return false; }
                    if (sender == null) { Logger.Log("Sender Invalid", prefix, Logger.Type.Warning); return false; }
                    if (callback == null) { Logger.Log("Callback Invalid", prefix, Logger.Type.Warning); return false; }
                    return true;
                }
                private static bool UserPassed(BasePlayer player, Settings settings)
                {
                    if (player == null) { return true; }
                    if (settings.Equals(default(Settings))) { return false; }

                    var ip = player?.Connection?.IPAddressWithoutPort() ?? null;
                    var id = player?.userID ?? 0;
                    var groups = Instance?.permission?.GetUserGroups($"{id}") ?? null;
                    var perm = Instance?.permission?.GetUserPermissions($"{id}") ?? null;
                    var auth = player?.Connection?.authLevel ?? 0;

                    if (settings.IP?.Contains(ip) == false ||
                        settings.ID?.Contains(id) == false ||
                        !groups.All(x => settings.Group?.Contains(x) == true) ||
                        !perm.All(x => settings.Perm?.Contains(x) == true) ||
                        auth < settings.Auth)
                    {
                        return false;
                    }
                    return true;
                }

                internal static class Console
                {
                    internal static void TryRun(Action callback, Settings settings, ConsoleSystem.Arg console)
                    {
                        if (!ValidateSettings(Configuration.Settings.Tokens.ConsoleCommandToken, settings, "ConsoleCommand", (console ?? null), callback)) { return; }
                        if (settings.Server && !((console?.IsRcon ?? false) && (console?.IsServerside ?? false))) { return; }
                        if (!UserPassed(console?.Player() ?? null, settings)) { return; }

                        callback.Invoke();
                    }
                    internal static void Register(string command, Func<ConsoleSystem.Arg, bool> method, params string[] args)
                    {
                        try
                        {
                            Instance?.cmd?.AddConsoleCommand($"{Prefix}.{command}", (Instance ?? null), method);
                            InitCore.Commands.AddCachedCommand(new InitCore.Commands.CommandData
                            {
                                Command = $"{Prefix}.{command}",
                                Type = InitCore.Commands.CommandType.Console,
                                Args = args
                            });
                        }
                        catch { }
                    }
                    internal static void Revoke(string command)
                    {
                        try
                        {
                            Instance?.cmd?.RemoveConsoleCommand(command, (Instance ?? null));
                            InitCore.Commands.RemoveCachedCommand(command);
                        }
                        catch { }
                    }
                }
                internal static class Chat
                {
                    internal static void TryRun(Action callback, Settings settings, BasePlayer player)
                    {
                        if (!ValidateSettings(Configuration.Settings.Tokens.ChatCommandToken, settings, "ChatCommand", (player ?? null), callback)) { return; }
                        if (!UserPassed((player ?? null), settings)) { return; }

                        callback.Invoke();
                    }
                    internal static void Register(string command, Action<BasePlayer, string, string[]> method)
                    {
                        try
                        {
                            Instance?.cmd?.AddChatCommand(command, (Instance ?? null), method);
                            InitCore.Commands.AddCachedCommand(new InitCore.Commands.CommandData
                            {
                                Command = command,
                                Type = InitCore.Commands.CommandType.Chat,
                            });
                        }
                        catch { }
                    }
                    internal static void Revoke(string command)
                    {
                        try
                        {
                            Instance?.cmd?.RemoveChatCommand(command, (Instance ?? null));
                            InitCore.Commands.RemoveCachedCommand(command);
                        }
                        catch { }
                    }
                }
            }
        }
        #endregion Utility
    }
}