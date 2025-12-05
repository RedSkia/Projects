using Newtonsoft.Json;
using Oxide.Core;
using Oxide.Game.Rust.Cui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Runtime.CompilerServices;
using UnityEngine;
using System.Reflection;
using ProtoBuf;
using CompanionServer;
using System.Collections;
// Reference: XLIB

namespace Oxide.Plugins
{
    [Info("ConfigEditor", "InfinityNet/Xray", "1.0.0")]
    [Description("In Progress")]
    public class ConfigEditorTEST : RustPlugin
    {
        private static ConfigEditorTEST Instance { get; set; }

        private const string constPluginName = "configeditor";

        private void Loaded()
        {
            Instance = this;
        }

        #region Core
        private static Dictionary<ulong, NavigationData> Navigation = new Dictionary<ulong, NavigationData>();
        private class NavigationData
        {
            public string Plugin { get; set; }
            //public XLIB.FILES.FileType FileType { get; set; } = XLIB.FILES.FileType.OxideConfig;
            public string[] Tokens { get; set; }
        }
        #endregion Core



        [ChatCommand("d")]
        private void DrawUI(BasePlayer player) => UI.DrawContainer(player);

        [ChatCommand("k")]
        private void KILLUI(BasePlayer player) => CuiHelper.DestroyUi(player, $"{Instance.Name}.Container");

        #region Helpers

        private Core.Plugins.Plugin[] GetPlugins()
        {
            string[] BlockedPlugins = { "RustCore", "UnityCore", Instance.Name };
            return plugins.GetAll().Where(x => !BlockedPlugins.Contains(x.Name)).ToArray();
        }
        #endregion Helpers


        [ConsoleCommand(constPluginName + ".navigation")]
        private void NavigationHandle(ConsoleSystem.Arg arg)
        {
            /*
            var player = arg?.Player();
            if (player == null) { return; }
            if (!Navigation.ContainsKey(player.userID)) { Navigation.Add(player.userID, new NavigationData()); }

            string plugin = arg.HasArgs(1) ? arg.Args[0] : null;
            string type = arg.HasArgs(2) ? arg.Args[1] : null;
            XLIB.FILES.FileType fileType = (type != null && (type.ToLower() == "data" || type.ToLower() == "d")) ? XLIB.FILES.FileType.OxideData : XLIB.FILES.FileType.OxideConfig;


            if (!XLIB.FILES.FileExists(fileType, plugin)) { PrintWarning("NO PLUGIN"); return; }
            string[] tokens = (type != null) ? arg.Args.Skip(2).ToArray() : arg.Args.Skip(1).ToArray();
            Navigation[player.userID].Plugin = plugin;
            Navigation[player.userID].FileType = fileType;
            Navigation[player.userID].Tokens = tokens;
            */
        }

        [ConsoleCommand(constPluginName + ".close")]
        private void CloseUIHandle(ConsoleSystem.Arg arg)
        {
            if (arg.Player() == null) { return; }
            CuiHelper.DestroyUi(arg.Player(), $"{Instance.Name}.Container");
        }

        [ConsoleCommand(constPluginName + ".pagination")]
        private void PaginationHandle(ConsoleSystem.Arg arg)
        {
            if (arg.Player() == null) { return; }
            var player = arg.Player();
            if (!UI.Pagination.ContainsKey(player.userID)) { return; }
            if (!arg.HasArgs(1)) { return; }
            var selector = arg.Args[0];
            int dump;
            int specficPage = (arg.HasArgs(2) && int.TryParse(arg.Args[1], out dump)) ? int.Parse(arg.Args[1]) : -1;
            switch (selector)
            {
                case "set":
                    if (!(specficPage > UI.Pagination[player.userID].TotalPages) &&
                        !(specficPage < 1)) { UI.Pagination[player.userID].Page = specficPage; }
                    break;

                case "next": if (!(UI.Pagination[player.userID].Page >= UI.Pagination[player.userID].TotalPages)) { UI.Pagination[player.userID].Page += 1; } break;
                case "prev": if (!(UI.Pagination[player.userID].Page <= 1)) { UI.Pagination[player.userID].Page -= 1; } break;
            }
            UI.DrawContainer(player);
        }


        #region UI
        private class UI
        {
            const double Gap = 0.0125;
            public static void DrawContainer(BasePlayer player)
            {
                CuiHelper.DestroyUi(player, $"{Instance.Name}.Container");
                const double GapDiff = 0.5;

         

                /*Root Container Outer*/
                var Container = XLIB.UI.Create.Container(
                    new XLIB.UI.UIPoint.Anchor
                    {
                        Min_Width = Gap,
                        Min_Height = Gap * 1.5,
                        Max_Width = (1.0 - Gap),
                        Max_Height = (1.0 - Gap * 1.5)
                    },
                    new XLIB.UI.UIPoint.Offset { },
                    new XLIB.UI.UIColor { color = "#454545" },
                    new XLIB.UI.UIProperties
                    {
                        EnableCursor = false,
                        EnableKeyboard = false,
                    },
                    XLIB.UI.UILayer.Overlay,
                    $"{Instance.Name}.Container");

                /*Root Panel*/
                XLIB.UI.Create.Panel(ref Container,
                new XLIB.UI.UIPoint.Anchor
                {
                    Min_Width = (Gap * GapDiff),
                    Min_Height = (Gap * GapDiff) * 1.5,
                    Max_Width = 1.0 - (Gap * GapDiff),
                    Max_Height = 1.0 - (Gap * GapDiff) * 1.5
                },
                new XLIB.UI.UIColor { color = "#753f3f" },
                new XLIB.UI.UIProperties { },
                $"{Instance.Name}.Container", $"{Instance.Name}.Panel");

                #region TopNav
                const double TopNavHeight = 0.075;
                XLIB.UI.Create.Panel(ref Container,
                new XLIB.UI.UIPoint.Anchor
                {
                    Min_Height = (1.0 - TopNavHeight),
                    Max_Width = 1.0,
                    Max_Height = 1.0
                },
                new XLIB.UI.UIColor { color = "#4f5282" },
                new XLIB.UI.UIProperties { },
                $"{Instance.Name}.Panel", $"{Instance.Name}.TopNav");



                const double LabelWidth = 0.95;
                /*Title Label*/
                XLIB.UI.Create.Label(ref Container,
                new XLIB.UI.UIPoint.Anchor
                {
                    Bottom = 0.1,
                    Min_Width = 1.0 - LabelWidth,
                    Max_Height = 1.0,
                    Max_Width = LabelWidth
                },
                new XLIB.UI.UIProperties { },
                $"{Instance.Name}.TopNav", $"{Instance.Name}.TopNav.Label",
                new XLIB.UI.UIColor { },
                TextAnchor.MiddleCenter,
                $"{Instance.Name} - The Universal Data Editor", 24);

                /*Credit Label*/
                XLIB.UI.Create.Label(ref Container,
                new XLIB.UI.UIPoint.Anchor
                {
                    Top = 0.6,
                    Min_Width = 1.0 - LabelWidth,
                    Max_Height = 1.0,
                    Max_Width = LabelWidth
                },
                new XLIB.UI.UIProperties { },
                $"{Instance.Name}.TopNav", $"{Instance.Name}.TopNav.Label",
                new XLIB.UI.UIColor { },
                TextAnchor.MiddleCenter,
                $"Made By Xray", 10);

                const double BtnGap = 0.1;
                XLIB.UI.Create.Button(ref Container,
                new XLIB.UI.UIPoint.Anchor
                {
                    Min_Height = BtnGap,
                    Max_Height = 1.0 - BtnGap,
                    Max_Width = 1.0 - BtnGap / 20,
                    Min_Width = LabelWidth + BtnGap / 20
                },
                new XLIB.UI.UIColor { color = "#cf19bc" },
                new XLIB.UI.UIProperties { },
                $"{Instance.Name}.TopNav", $"{Instance.Name}.TopNav.Button", $"{constPluginName}.close",
                new XLIB.UI.UIColor { },
                TextAnchor.MiddleCenter,
                "X", 30);
                #endregion TopNav

                /*Splitter Line*/
                XLIB.UI.Create.Panel(ref Container,
                new XLIB.UI.UIPoint.Anchor
                {
                    Max_Width = 1.0,
                    Min_Height = 1.0 - TopNavHeight - Gap,
                    Max_Height = 1.0 - TopNavHeight
                },
                new XLIB.UI.UIColor { color = "#32a0a8" },
                new XLIB.UI.UIProperties { },
                $"{Instance.Name}.Panel", $"{Instance.Name}.SplitterLine");



                /*Root Tables Panel*/
                XLIB.UI.Create.Panel(ref Container,
                new XLIB.UI.UIPoint.Anchor
                {
                    Max_Width = 1.0,
                    Min_Height = TopNavHeight + Gap,
                    Max_Height = 1.0 - TopNavHeight - Gap
                },
                new XLIB.UI.UIColor { color = "#4b8049" },
                new XLIB.UI.UIProperties { },
                $"{Instance.Name}.Panel", $"{Instance.Name}.TablesContainer");



                /*Splitter Line*/
                XLIB.UI.Create.Panel(ref Container,
                new XLIB.UI.UIPoint.Anchor
                {
                    Max_Width = 1.0,
                    Min_Height = TopNavHeight,
                    Max_Height = TopNavHeight + Gap
                },
                new XLIB.UI.UIColor { color = "#32a0a8" },
                new XLIB.UI.UIProperties { },
                $"{Instance.Name}.Panel", $"{Instance.Name}.SplitterLine");


                #region BottomNav
                /*BottomNav Panel*/
                XLIB.UI.Create.Panel(ref Container,
                new XLIB.UI.UIPoint.Anchor
                {
                    Max_Width = 1.0,
                    Max_Height = TopNavHeight
                },
                new XLIB.UI.UIColor { color = "#a787c9" },
                new XLIB.UI.UIProperties { },
                $"{Instance.Name}.Panel", $"{Instance.Name}.BottomNav");


                #endregion BottomNav

                string plug = "admintoggle";
                string[] tokens = { "MODES","0", "" };
                //DrawTable(ref Container, player, plug, tokens);
                DrawPlugins(ref Container, player);
               // DrawPopupOverlay(ref Container, player, "ayy", MessageType.Error);
                CuiHelper.AddUi(player, Container);
            }

            #region Overlay Popup
            private static Dictionary<ulong, bool> Overlay = new Dictionary<ulong, bool>();
            public enum MessageType
            {
                Success,
                Infomation,
                Warning,
                Error
            }
            public static void DrawPopupOverlay(ref CuiElementContainer Container, BasePlayer player, string Message, MessageType msgType = MessageType.Infomation, float time = 2.0f)
            {
                if(!Overlay.ContainsKey(player.userID)) { Overlay.Add(player.userID, false); }
                bool IsActive = Overlay[player.userID];
                if(!IsActive)
                {
                    Overlay[player.userID] = true;
                    const int MessageSize = 32;
                    string TypeMessage = "";
                    switch (msgType)
                    {
                        case MessageType.Success: TypeMessage = $"{XLIB.DATA.String.TextEncodeing(MessageSize, "#00ff00", msgType.ToString(), true)}"; break;
                        case MessageType.Infomation: TypeMessage = $"{XLIB.DATA.String.TextEncodeing(MessageSize, "#00ffff", msgType.ToString(), true)}"; break;
                        case MessageType.Warning: TypeMessage = $"{XLIB.DATA.String.TextEncodeing(MessageSize, "#ffff00", msgType.ToString(), true)}"; break;
                        case MessageType.Error: TypeMessage = $"{XLIB.DATA.String.TextEncodeing(MessageSize, "#ff0000", msgType.ToString(), true)}"; break;
                    }
                    XLIB.UI.Create.Panel(ref Container,
                        new XLIB.UI.UIPoint.Anchor { Max_Height = 1.0, Max_Width = 1.0 },
                        new XLIB.UI.UIColor { color = "#000000", alpha = 0.95 },
                        new XLIB.UI.UIProperties { Material = "assets/content/ui/uibackgroundblur.mat" },
                        $"{Instance.Name}.Container", $"{Instance.Name}.Container.Overlay");

                    XLIB.UI.Create.Label(ref Container,
                        new XLIB.UI.UIPoint.Anchor { Max_Height = 1.0, Max_Width = 1.0 },
                        new XLIB.UI.UIProperties { },
                        $"{Instance.Name}.Container.Overlay", $"{Instance.Name}.Container.Overlay.Label",
                        new XLIB.UI.UIColor { },
                        TextAnchor.MiddleCenter,
                        $"[{TypeMessage}]\n{Message}", MessageSize);

                    /*Credit: UIChamp*/
                    ServerMgr.Instance.Invoke(() =>
                    {
                        CuiHelper.DestroyUi(player, $"{Instance.Name}.Container.Overlay");
                        Overlay[player.userID] = false;
                    }, time);

                }
            }
            #endregion Overlay Popup

            #region Pagination

            public static Dictionary<ulong, PaginationData> Pagination = new Dictionary<ulong, PaginationData>();
            public class PaginationData
            {
                public int Page { get; set; } = 1; /*Must 1*/
                public int TotalPages { get; set; } = 0;
                public string[] Array { get; set; }
            }
            private static void DrawPagination(ref CuiElementContainer Container, ulong steamID, string[] Array, int Page, int PageBuffer, int PageLimit)
            {
                if (!Pagination.ContainsKey(steamID)) { Pagination.Add(steamID, new PaginationData()); }
                Pagination[steamID].Array = Array;
                #region Static UI

                const double PaginationWidth = 0.7;
                const double BtnWidth = 0.075;
                XLIB.UI.Create.Panel(ref Container,
                new XLIB.UI.UIPoint.Anchor
                {
                    Min_Width = 1.0 - PaginationWidth,
                    Max_Width = PaginationWidth,
                    Min_Height = Gap * 10,
                    Max_Height = 1.0 - Gap * 10
                },
                new XLIB.UI.UIColor { color = "#27484a" },
                new XLIB.UI.UIProperties { },
                $"{Instance.Name}.BottomNav", $"{Instance.Name}.BottomNav.Pagination");

                XLIB.UI.Create.Panel(ref Container,
                new XLIB.UI.UIPoint.Anchor
                {
                    Min_Width = BtnWidth,
                    Max_Width = 1.0 - BtnWidth,
                    Max_Height = 1.0
                },
                new XLIB.UI.UIColor { color = "#c24e61" },
                new XLIB.UI.UIProperties { },
                $"{Instance.Name}.BottomNav.Pagination", $"{Instance.Name}.BottomNav.Pagination.Panel");

                #endregion Static UI


                double PageBtnWidth = 1.0 / PageBuffer;
                int index = 0;
                int totalPages = 0;
                foreach (var page in ALGORITHMS.Pagination.Create(Array, Page, PageBuffer, PageLimit, out totalPages))
                {
                    double min = PageBtnWidth * index;
                    double max = PageBtnWidth + PageBtnWidth * index;

                    double safeMax = max > 1.0 ? 1.0 : max;
                    double safeMin = min < 0.0 ? 0.0 : min;

                    XLIB.UI.Create.Button(ref Container,
                    new XLIB.UI.UIPoint.Anchor
                    {
                        Min_Width = safeMin,
                        Max_Width = safeMax,
                        Max_Height = 1.0
                    },
                    new XLIB.UI.UIColor { color = page == Page ? "#03ecfc" : "#78311e" },
                    new XLIB.UI.UIProperties { },
                    $"{Instance.Name}.BottomNav.Pagination.Panel", $"{Instance.Name}.BottomNav.Pagination.Page.{page}", $"{constPluginName}.pagination set {page}",
                    new XLIB.UI.UIColor { },
                    TextAnchor.MiddleCenter,
                    $"{page}", 20);

                    index++;
                }
                Pagination[steamID].TotalPages = totalPages;

                #region Semi Dynamic Buttons << >> 
                XLIB.UI.Create.Button(ref Container,
                new XLIB.UI.UIPoint.Anchor
                {
                    Max_Width = BtnWidth,
                    Max_Height = 1.0

                },
                new XLIB.UI.UIColor { color = (!(UI.Pagination[steamID].Page <= 1)) ? "#2ab863" : "#454545" },
                new XLIB.UI.UIProperties { },
                $"{Instance.Name}.BottomNav.Pagination", $"{Instance.Name}.BottomNav.Pagination.Button.Prev", $"{constPluginName}.pagination prev",
                new XLIB.UI.UIColor { },
                TextAnchor.MiddleCenter,
                "<<", 20);

                XLIB.UI.Create.Button(ref Container,
                new XLIB.UI.UIPoint.Anchor
                {
                    Min_Width = 1.0 - BtnWidth,
                    Max_Width = 1.0,
                    Max_Height = 1.0

                },
                new XLIB.UI.UIColor { color = (!(UI.Pagination[steamID].Page >= UI.Pagination[steamID].TotalPages)) ? "#2ab863" : "#454545" },
                new XLIB.UI.UIProperties { },
                $"{Instance.Name}.BottomNav.Pagination", $"{Instance.Name}.BottomNav.Pagination.Button.Next", $"{constPluginName}.pagination next",
                new XLIB.UI.UIColor { },
                TextAnchor.MiddleCenter,
                ">>", 20);
                #endregion Semi Dynamic Buttons << >> 
            }
            #endregion Pagination

            public static void DrawPlugins(ref CuiElementContainer Container, BasePlayer player)
            {
                if (!Pagination.ContainsKey(player.userID)) { Pagination.Add(player.userID, new PaginationData()); }

                const int RowLimit = 5;
                const int ColumnLimit = 25;
                const double Gap = 0.0125;

                double BtnWidth = 1.0 / RowLimit - (Gap / (RowLimit * 2));
                double BtnHeight = 1.0 / RowLimit - (Gap / RowLimit);

                int page = Pagination[player.userID].Page-1;

                int index = 0;
                int Row = 0;
                int pos = 0;

                var array = Instance.GetPlugins().Select(x => x.Name).ToArray();
                foreach (var plugin in array.Skip(page * ColumnLimit).Take(ColumnLimit))
                {
                    if (index > ColumnLimit - 1) { /*Hide*/ continue; }

                    if (pos == RowLimit) { pos = 0; Row++; }

                    double minW = BtnWidth * pos + Gap / 2;
                    double maxW = BtnWidth + BtnWidth * pos;
                    double maxH = 1.0 - BtnHeight * Row - Gap;
                    double minH = 1.0 - BtnHeight - BtnHeight * Row;

                    XLIB.UI.Create.Button(ref Container,
                    new XLIB.UI.UIPoint.Anchor
                    {
                        Min_Width = minW,
                        Max_Width = maxW,
                        Min_Height = minH,
                        Max_Height = maxH
                    },
                    new XLIB.UI.UIColor { color = "#8f4242" },
                    new XLIB.UI.UIProperties { },
                    $"{Instance.Name}.TablesContainer", $"{Instance.Name}.TablesContainer.Plugins", $"{constPluginName}.navigation {plugin.ToLower()}",
                    new XLIB.UI.UIColor { },
                    TextAnchor.MiddleCenter,
                    $"{plugin}", 20);

                    pos++; index++;
                }
                DrawPagination(ref Container, player.userID, array, Pagination[player.userID].Page, 9, ColumnLimit);
            }

            public static void DrawTable(ref CuiElementContainer Container, BasePlayer player,string plugin, string[] tokens)
            {
                /*
                var x = XLIB.JSON.UnknownObject.GetValue(tokens, $@"E:\Skrivebord\Dev Server\LiveServer\Oxide\config\admintoggle.json");
                if(x == null) { Instance.PrintWarning("BADDDD"); return; }
                foreach (var item in x)
                {
                    Instance.PrintWarning(item.ToString());
                }
                */
            }
        }
        #endregion UI

        public class ALGORITHMS
        {
            public class Pagination
            {
                public static int[] Create(object[] Items, int Page, int PageBuffer, int PageLimit, out int totalPages)
                {
                    const int StartPoint = 1;
                    decimal FloatingTotal = (decimal)Items.Length / (decimal)PageLimit;
                    bool IsFloating = (FloatingTotal - Math.Floor(FloatingTotal)) != 0;
                    int TotalPages = (Items.Length / PageLimit);

                    if (IsFloating) { TotalPages++; }

                    /*Out*/
                    totalPages = TotalPages;

                    int[] array = Enumerable.Range(StartPoint, TotalPages).ToArray();
                    int Skip1 = Page > (PageBuffer / 2) ? Page - (PageBuffer / 2) - StartPoint : 0;
                    int Skip2 = (Page + (PageBuffer / 2)) > TotalPages ? Page + (PageBuffer / 2) - TotalPages : 0;

                    return array.Skip(Skip1 - Skip2).Take(PageBuffer).ToArray();
                }           
            }
        }
    }
}