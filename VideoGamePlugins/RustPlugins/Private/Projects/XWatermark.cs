using Oxide.Core;
using Oxide.Game.Rust.Cui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Oxide.Plugins
{
    [Info("Watermark", "Infinitynet.dk/Xray", "1.0.0")]
    [Description("Copyright Infinitynet.dk All Rights Reserved")]
    public class XWatermark : RustPlugin
    {



        const int FontSize = 12;
        const string Discord = "discord.gg/u7gQwQPERX";
        const string Website = "Infinitynet.dk";
        const string Steam = "steamcommunity.com/groups/InfinityNet";


        static Dictionary<ulong, PlayerOptions> cachedPlayerOptions = new Dictionary<ulong, PlayerOptions>();

        private class PlayerOptions
        {
            public bool ShowWatermark = true;



            internal static void LoadData(ulong id)
            {

                if (cachedPlayerOptions.ContainsKey(id)) return;

                PlayerOptions data = Interface.Oxide.DataFileSystem.ReadObject<PlayerOptions>($"Watermark/{id}");

                if (data == null) data = new PlayerOptions();

                cachedPlayerOptions.Add(id, data);
            }

            internal void SaveData(ulong id) => Interface.Oxide.DataFileSystem.WriteObject(($"Watermark/{id}"), this, true);
        }





        void drawUI(BasePlayer player)
        {
            CuiElementContainer elements = new CuiElementContainer();

            elements.Add(new CuiPanel
            {
                Image = { Color = "0 0 0 0" },
                RectTransform =
                {
                    AnchorMin = "0 0",
                    AnchorMax = "1 0.02",
                },

                CursorEnabled = false
            }, "Under", "X_WaterMark"); //Main Panel

            elements.Add(new CuiElement
            {
                Name = "X_WaterMark_Box1",
                Parent = "X_WaterMark",
                Components =
                {
                    new CuiImageComponent
                    {
                        Color = "0 0 0 0"
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0 0",
                        AnchorMax = "0.33 1",
                        OffsetMax = "0 0"

                    }
                }
            });

            elements.Add(new CuiElement
            {
                Parent = "X_WaterMark_Box1",
                Components =
                {
                    new CuiTextComponent
                    {
                        Text = $"Discord: {Discord}",
                        Align = UnityEngine.TextAnchor.MiddleCenter,
                        FontSize = FontSize,
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0 0",
                        AnchorMax = "1 1",
                        OffsetMax = "0 0"

                    }
                }
            });



            elements.Add(new CuiElement
            {
                Name = "X_WaterMark_Box2",
                Parent = "X_WaterMark",
                Components =
                {
                    new CuiImageComponent
                    {
                        Color = "0 0 0 0"
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0.33 0",
                        AnchorMax = "0.66 1",
                        OffsetMax = "0 0"

                    }
                }
            });

            elements.Add(new CuiElement
            {
                Parent = "X_WaterMark_Box2",
                Components =
                {
                    new CuiTextComponent
                    {
                        Text = $"Website: {Website}",
                        Align = UnityEngine.TextAnchor.MiddleCenter,
                        FontSize = FontSize,
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0 0",
                        AnchorMax = "1 1",
                        OffsetMax = "0 0"

                    }
                }
            });



            elements.Add(new CuiElement
            {
                Name = "X_WaterMark_Box3",
                Parent = "X_WaterMark",
                Components =
                {
                    new CuiImageComponent
                    {
                        Color = "0 0 0 0"
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0.66 0",
                        AnchorMax = "0.99 1",
                        OffsetMax = "0 0"

                    }
                }
            });

            elements.Add(new CuiElement
            {
                Parent = "X_WaterMark_Box3",
                Components =
                {
                    new CuiTextComponent
                    {
                        Text = $"Steam: {Steam}",
                        Align = UnityEngine.TextAnchor.MiddleCenter,
                        FontSize = FontSize,
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0 0",
                        AnchorMax = "1 1",
                        OffsetMax = "0 0"

                    }
                }
            });

            CuiHelper.AddUi(player, elements);
        }

        void destoryUI(BasePlayer player)
        {
            CuiHelper.DestroyUi(player, "X_WaterMark");
        }

        [ChatCommand("watermark")]
        private void ToggleWatermark(BasePlayer player, string command, string[] args)
        {
            if (args.Count() >= 1)
            {
                PlayerOptions.LoadData(player.userID);


                if (args[0] == "true" || args[0] == "True")
                {

                    destoryUI(player);
                    drawUI(player);
                    cachedPlayerOptions[player.userID].ShowWatermark = true;

                    cachedPlayerOptions[player.userID].SaveData(player.userID);

                }
                else if (args[0] == "false" || args[0] == "False")
                {

                    destoryUI(player);

                    cachedPlayerOptions[player.userID].ShowWatermark = false;

                    cachedPlayerOptions[player.userID].SaveData(player.userID);
                }
                else
                {
                    SendReply(player, "Do /watermark (true/false) - to toggle");
                }

            }
            else
            {
                SendReply(player, "Do /watermark (true/false) - to toggle");
            }

        }



        void OnPlayerConnected(BasePlayer player)
        {
            PlayerOptions.LoadData(player.userID);

            if (cachedPlayerOptions[player.userID].ShowWatermark)
            {
                drawUI(player);
            }




        }




    }
}
