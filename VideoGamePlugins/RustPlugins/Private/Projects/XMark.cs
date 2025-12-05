using Oxide.Core;
using Oxide.Core.Libraries;
using Oxide.Core.Plugins;
using Oxide.Game.Rust.Cui;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Oxide.Plugins
{
    [Info("XMark", "InfinityNet/Xray", "1.0.0")]
    [Description("Copyright Infinitynet.dk All Rights Reserved")]
    public class XMark : RustPlugin
    {
        /*
         * https://red-route.org/code/image-resizing-calculator
         * https://gist.github.com/lopspower/03fb1cc0ac9f32ef38f4
         */

        #region Settings
        const bool DEBUG = false;
        const string BoxColor = "0 0 0 0";
        const int BoxWidth = 200;
        const int BoxHeight = 40;
        const int BoxMargin = 5;
        const float LogoWidth = 0.20f;
        const float BoxContentOpacity = 0.5f;
        #endregion Settings

        private class UI
        {
            public static void DrawUI(BasePlayer player)
            {
                DestroyUI(player);
                CuiElementContainer container = new CuiElementContainer();

                container.Add(new CuiPanel
                {
                    Image = { Color = $"0.5 0 0 {(DEBUG ? 1 : 0)}" },
                    RectTransform =
                    {
                        AnchorMin = "0 1",
                        AnchorMax = "0 1",
                        OffsetMin = $"{BoxMargin} {-(BoxHeight+BoxMargin)}",
                        OffsetMax = $"{(BoxWidth+BoxMargin)} {-BoxMargin}"
                    },
                    CursorEnabled = false,
                }, "Hud", "XWatermark");


                #region Logo
                container.Add(new CuiPanel
                {
                    Image = { Color = $"0 0.5 0 {(DEBUG ? 1 : 0)}" },
                    RectTransform =
                    {
                        OffsetMax = "0 0",
                        OffsetMin = "0 0",
                        AnchorMin = "0 0",
                        AnchorMax = $"{LogoWidth} 1",
                    },
                    CursorEnabled = false,
                }, "XWatermark", "XWatermarkLogo");
                container.Add(new CuiElement
                {
                    Parent = "XWatermarkLogo",
                    Components =
                    {
                        new CuiRawImageComponent
                        {
                            Color = $"1 1 1 {BoxContentOpacity}",
                            Url = "https://infinitynet.dk/host/logo.png",
                        },
                        new CuiRectTransformComponent
                        {
                            AnchorMin = "0 0",
                            AnchorMax = "1 1",
                            OffsetMax = "0 0"
                        },
                    },
                });
                #endregion Logo

                #region Text
                container.Add(new CuiPanel
                {
                    Image = { Color = $"0 0.5 0.5 {(DEBUG ? 1 : 0)}" },
                    RectTransform =
                    {
                        OffsetMax = "0 0",
                        OffsetMin = "0 0",
                        AnchorMin = $"{LogoWidth} 0",
                        AnchorMax = "1 1",
                    },
                    CursorEnabled = false,
                }, "XWatermark", "XWatermarkTopText");
                container.Add(new CuiLabel
                {
                    Text =
                    {
                        Align = TextAnchor.MiddleCenter,
                        Color = $"1 1 1 {BoxContentOpacity}",
                        FontSize = 24,
                        Text = "INFINITYNET.DK",
                    },
                    RectTransform =
                    {
                        AnchorMin = "0 0",
                        AnchorMax = "1 1"
                    }
                }, "XWatermarkTopText");
                #endregion Text
                CuiHelper.AddUi(player, container);
            }
            public static void DestroyUI(BasePlayer player) => CuiHelper.DestroyUi(player, "XWatermark");
        }


        void Loaded()
        {
            foreach (var player in BasePlayer.activePlayerList) { UI.DrawUI(player); }
        }
        void OnPlayerConnected(BasePlayer player) => UI.DrawUI(player);
        void OnPlayerDisconnected(BasePlayer player, string reason) => UI.DestroyUI(player);
    }
}