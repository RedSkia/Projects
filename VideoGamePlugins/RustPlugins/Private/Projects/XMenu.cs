using ConVar;
using Oxide.Core;
using Oxide.Game.Rust.Cui;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Oxide.Plugins
{
    [Info("XMenu", "Infinitynet.dk", "1.0.0")]
    [Description("Copyright Infinitynet.dk All Rights Reserved")]
    public class XMenu : RustPlugin
    {

        #region Data Management
        static Dictionary<ulong, PlayerData> cachedPlayerData = new Dictionary<ulong, PlayerData>();


        private class PlayerData
        {
            public Timer FreezeTimer;
            public Vector3 playerPos = new Vector3();


            public Timer PopupMessageTimer;

            public bool MenuMainOpen = false;
            public bool MenuOptionsOpen = false;
            public bool MenuBannedOpen = false;
            public bool MenuCreateBanOpen = false;
            public bool MenuCreateReportOpen = false;


            public int TotalTimesBanned = 0;

            public int SearchOverRide = 0;

            public SortedList<ulong, string> ActiveList = new SortedList<ulong, string>();
            public string ActiveListName = "";

            public int CurrentPage = 0;
            public int CurrentPageItemsCount = 0;

            public ulong SelectedTargetID = 0;
            public string SelectedTargetName = "";
            public string SelectedBannedTargetPlayerInfo = "";
            public List<string> SelectedBannedTargetPlayerInfo_Args = new List<string>();
            public List<string> SelectedBannedTargetPlayerInfoInput_Args = new List<string>() { "NULL", "NULL", "NULL", "NULL", "NULL", "NULL", "NULL" };


            public List<string> SelectedReportTargetPlayerInfoInput_Args = new List<string>() { "NULL", "NULL", "NULL", "NULL" };


            internal static void LoadCachedData(ulong id)
            {
                if (!cachedPlayerData.ContainsKey(id)) cachedPlayerData.Add(id, new PlayerData());
            }
        }
        #endregion


        #region OnStartup
        private void Init()
        {


            foreach (BasePlayer player in BasePlayer.activePlayerList)
            {
                CuiHelper.DestroyUi(player, "XMenu_MainContainer");
                CuiHelper.DestroyUi(player, "XMenu_SelectedPlayer_OptionsMenu");
                CuiHelper.DestroyUi(player, "XMenu_BannedPlayer_Menu");
                CuiHelper.DestroyUi(player, "XMenu_BanPlayer_Form");
                CuiHelper.DestroyUi(player, "XMenu_ReportPlayer_Form");

                CuiHelper.DestroyUi(player, "XMenu_BlurElement");

                cachedPlayerData.Remove(player.userID);
            }
        }
        #endregion



        #region UI

        private void DrawPopupMessage(BasePlayer player, string ParrentElement, string TextToDisplay, bool ReloadMenu, int FontSize)
        {
            PlayerData.LoadCachedData(player.userID); //Load Data

            CuiElementContainer elements = new CuiElementContainer();

            elements.Add(new CuiElement
            {
                Name = "XMenu_PopupMessage",
                Parent = ParrentElement,
                Components =
                {
                    new CuiImageComponent
                    {
                        Color = "0.1 0.1 0.1 0.925",
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0.005 0.005",
                        AnchorMax = "0.995 0.925",
                        OffsetMax = "0 0"
                    }
                }
            });
            elements.Add(new CuiElement
            {
                Parent = "XMenu_PopupMessage",
                Components =
                {
                    new CuiTextComponent
                    {
                        Text = TextToDisplay,
                        Align = TextAnchor.MiddleCenter,
                        Color = "1 1 1 1",
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





            cachedPlayerData[player.userID].PopupMessageTimer = timer.Once(1f, () =>
            {
                CuiHelper.DestroyUi(player, "XMenu_PopupMessage");

                if (ReloadMenu)
                {
                    #region Pages To Close
                    player.SendConsoleCommand("xmenu.close.menu.main");
                    player.SendConsoleCommand("xmenu.close.menu.selected.options");
                    player.SendConsoleCommand("xmenu.close.menu.selected.banned");
                    player.SendConsoleCommand("xmenu.close.menu.selected.createban");
                    #endregion
                    cachedPlayerData[player.userID].PopupMessageTimer = timer.Once(0.1f, () =>
                    {
                        if (cachedPlayerData[player.userID].ActiveListName == "Online")
                        {
                            cachedPlayerData[player.userID].ActiveList = PlayerLists.GetOnlineList();
                            cachedPlayerData[player.userID].ActiveListName = "Online";
                            DrawMenu(player, cachedPlayerData[player.userID].ActiveList, false, cachedPlayerData[player.userID].CurrentPage);
                            DrawMenu(player, cachedPlayerData[player.userID].ActiveList, true, cachedPlayerData[player.userID].CurrentPage);
                        }
                        if (cachedPlayerData[player.userID].ActiveListName == "Disk")
                        {
                            cachedPlayerData[player.userID].ActiveList = PlayerLists.GetDiskList();
                            cachedPlayerData[player.userID].ActiveListName = "Disk";
                            DrawMenu(player, cachedPlayerData[player.userID].ActiveList, false, cachedPlayerData[player.userID].CurrentPage);
                            DrawMenu(player, cachedPlayerData[player.userID].ActiveList, true, cachedPlayerData[player.userID].CurrentPage);
                        }
                        if (cachedPlayerData[player.userID].ActiveListName == "Banned")
                        {
                            cachedPlayerData[player.userID].ActiveList = PlayerLists.GetBannedList();
                            cachedPlayerData[player.userID].ActiveListName = "Banned";
                            DrawMenu(player, cachedPlayerData[player.userID].ActiveList, false, cachedPlayerData[player.userID].CurrentPage);
                            DrawMenu(player, cachedPlayerData[player.userID].ActiveList, true, cachedPlayerData[player.userID].CurrentPage);
                        }


                    });
                }

            });



        }

        private void DrawBlurElement(BasePlayer player)
        {
            CuiElementContainer elements = new CuiElementContainer();

            elements.Add(new CuiPanel
            {
                Image = { Color = "0 0 0 0.925" },
                RectTransform =
                {
                    AnchorMin = "0 0.",
                    AnchorMax = "2 2"
                },
                CursorEnabled = true
            }, "Overlay", "XMenu_BlurElement");
            CuiHelper.AddUi(player, elements);
        }

        private void DrawMenu(BasePlayer player, SortedList<ulong, string> ListToDisplay, bool ReDrawDynamic, int Page)
        {


            CuiElementContainer elements = new CuiElementContainer();


            if (!ReDrawDynamic)
            {
                CuiHelper.DestroyUi(player, "XMenu_MainContainer");
            }
            else
            {
                CuiHelper.DestroyUi(player, "XMenu_MainContainer_DisplayPanel");
            }





            if (!ReDrawDynamic)
            {
                #region Static UI
                elements.Add(new CuiPanel
                {
                    RectTransform =
                    {
                        AnchorMin = "0.35 0.15",
                        AnchorMax = "0.65 0.85"
                    },
                    CursorEnabled = true
                }, "Overall", "XMenu_MainContainer"); //Main Panel




                #region Top Panel
                elements.Add(new CuiElement
                {
                    Name = "XMenu_MainContainer_TopPanel",
                    Parent = "XMenu_MainContainer",
                    Components =
                {
                    new CuiImageComponent {Color = "0.2 0.4 0.6 1"},
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0 0.925",
                        AnchorMax = "1 1",
                        OffsetMax = "0 0"
                    }
                }
                }); //Top Panel


                elements.Add(new CuiElement
                {
                    Name = "XMenu_MainContainer_TopPanel_CloseButton",
                    Parent = "XMenu_MainContainer_TopPanel",
                    Components =
                {
                    new CuiButtonComponent
                    {
                        Color = "0.5 0 0 1",
                        Command = "xmenu.menu.destory",
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0.915 0.05",
                        AnchorMax = "0.995 0.95",
                        OffsetMax = "0 0"
                    }
                }
                }); //Top Panel Close Button
                elements.Add(new CuiElement
                {
                    Parent = "XMenu_MainContainer_TopPanel_CloseButton",
                    Components =
                {
                    new CuiTextComponent
                    {
                        Text = "X",
                        Align = UnityEngine.TextAnchor.MiddleCenter,
                        Color = "1 1 1 1",
                        FontSize = 20,
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMax = "1 1",
                        OffsetMax = "0 0"
                    }
                }
                }); //Top Panel Close Button Text

                elements.Add(new CuiElement
                {
                    Name = "XMenu_MainContainer_TopPanel_Text",
                    Parent = "XMenu_MainContainer_TopPanel",
                    Components =
                    {
                        new CuiTextComponent
                        {
                            Text = "Player Administration Menu",
                            Color = "1 1 1 1",
                            Align = UnityEngine.TextAnchor.MiddleLeft,
                            FontSize = 20,
                        },
                        new CuiRectTransformComponent
                        {
                            AnchorMin = "0.01 0",
                            AnchorMax = "0.9 1",
                            OffsetMax = "0 0"
                        }
                    }
                }); //Top Panel Text Display
                #endregion

                #region Sub Top Search Panel
                elements.Add(new CuiElement
                {
                    Name = "XMenu_MainContainer_Top_SearchPanel",
                    Parent = "XMenu_MainContainer",
                    Components =
                    {
                        new CuiImageComponent { Color = "0.1 0.2 0.3 1"},
                        new CuiRectTransformComponent
                        {
                            AnchorMin = "0.005 0.865",
                            AnchorMax = "0.995 0.925",
                            OffsetMax = "0 0"
                        }
                    }
                }); //Top Search Container Panel

                elements.Add(new CuiElement
                {
                    Name = "XMenu_MainContainer_Top_SearchPanel_InputFieldBox",
                    Parent = "XMenu_MainContainer_Top_SearchPanel",

                    Components =
                    {
                        new CuiImageComponent { Color = "0.1 0.1 0.1 1"},
                        new CuiRectTransformComponent
                        {
                            AnchorMin = "0.1 0.1",
                            AnchorMax = "0.995 0.9",
                            OffsetMax = "0 0"
                        }
                    }

                }); //Top Search Container Panel InputFieldBox

                elements.Add(new CuiElement
                {
                    Name = "XMenu_MainContainer_Top_SearchPanel_InputField",
                    Parent = "XMenu_MainContainer_Top_SearchPanel_InputFieldBox",
                    Components =
                    {
                        new CuiInputFieldComponent
                        {
                            Color = "1 1 1 1",
                            Align = UnityEngine.TextAnchor.MiddleLeft,
                            FontSize = 14,
                            CharsLimit = 64,
                            Command = "xmenu.search.list"
                        },
                        new CuiRectTransformComponent
                        {
                            AnchorMin = "0.005 0",
                            AnchorMax = "1 1",
                            OffsetMax = "0 0"
                        }
                    }
                }); //Top Search Container Search InputField 

                elements.Add(new CuiElement
                {
                    Name = "XMenu_MainContainer_Top_SearchPanel_SearchText",
                    Parent = "XMenu_MainContainer_Top_SearchPanel",

                    Components =
                {
                    new CuiTextComponent
                    {
                        Text = "Search",
                        Align = UnityEngine.TextAnchor.MiddleLeft,
                        Color = "1 1 1 1",
                        FontSize = 14,
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0.0065 0.1",
                        AnchorMax = "0.0935 0.9",
                        OffsetMax = "0 0"
                    }

                }

                }); //Top Search Container Search Text
                #endregion

                #region Border Lines
                elements.Add(new CuiElement
                {
                    Parent = "XMenu_MainContainer",
                    Components =
                {
                    new CuiImageComponent{Color = "0.2 0.4 0.6 1"},
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0 0",
                        AnchorMax = "1 0.005",
                        OffsetMax = "0 0",
                    }
                }

                }); //Bottom Border

                elements.Add(new CuiElement
                {
                    Parent = "XMenu_MainContainer",
                    Components =
                {
                    new CuiImageComponent{Color = "0.2 0.4 0.6 1"},
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0 0",
                        AnchorMax = "0.005 0.925",
                        OffsetMax = "0 0",
                    }
                }

                }); //Left Border

                elements.Add(new CuiElement
                {
                    Parent = "XMenu_MainContainer",
                    Components =
                {
                    new CuiImageComponent
                    {
                        Color = "0.2 0.4 0.6 1",
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0.995 0",
                        AnchorMax = "1 0.925",
                        OffsetMax = "0 0"
                    }
                }
                }); //Right Border
                #endregion


                #endregion
            }

            if (ReDrawDynamic)
            {
                #region Dynamic UI

                #region Display Panel
                elements.Add(new CuiElement
                {
                    Name = "XMenu_MainContainer_DisplayPanel",
                    Parent = "XMenu_MainContainer",
                    Components =
                    {
                        new CuiImageComponent { Color = "0.1 0.1 0.1 1"},
                        new CuiRectTransformComponent
                        {
                            AnchorMin = "0.005 0.065",
                            AnchorMax = "0.995 0.865",
                            OffsetMax = "0 0"
                        }
                    }
                }); //Main Display Panel Container
                #endregion

                #region Dynamic List
                double ButtonSizeWidth = 0.332;

                double MarginGap = 0.005;


                double DataValue1 = 0.0;
                double DataValue2 = ButtonSizeWidth;
                //Height
                double HeightValue1 = 0.94525;
                double HeightValue2 = 0.995;

                double Gap = HeightValue2 - HeightValue1;


                int PosIndex = 0;

                int IndexCounter = 0;
                int PageLimit = 60;



                foreach (var keyPlayer in ListToDisplay.Skip(PageLimit * Page))
                {

                    IndexCounter++;

                    if (IndexCounter <= PageLimit)
                    {
                        if (PosIndex >= 3)
                        {
                            PosIndex = 0;

                            DataValue1 = 0.0;
                            DataValue2 = ButtonSizeWidth;


                            HeightValue1 -= Gap;
                            HeightValue2 -= Gap;
                        }

                        string playerName = null;

                        if (keyPlayer.Value.Length > 7)
                        {
                            playerName = keyPlayer.Value.Substring(0, 7) + "...";
                        }
                        else
                        {
                            playerName = keyPlayer.Value;
                        }

                        if (cachedPlayerData[player.userID].ActiveListName == "Banned")
                        {
                            var i = keyPlayer.Value;
                            var FiltedPlayerName = i.Split('|');


                            if (FiltedPlayerName[0].Length > 7)
                            {
                                playerName = FiltedPlayerName[0].Substring(0, 7) + "...";
                            }
                            else
                            {
                                playerName = FiltedPlayerName[0];
                            }
                        }

                        if (PlayerBanned(keyPlayer.Key))
                        {

                            //Button
                            elements.Add(new CuiElement
                            {
                                Name = "XMenu_MainContainer_DisplayContainer_Button",
                                Parent = "XMenu_MainContainer_DisplayPanel",
                                Components =
                                {
                                    new CuiButtonComponent
                                    {
                                        Color = "0.5 0.1 0.1 1",
                                        Command = $"xmenu.player.onclick {keyPlayer.Key} {keyPlayer.Value}"
                                    },

                                    new CuiRectTransformComponent
                                    {
                                        AnchorMin = $"{DataValue1+MarginGap} {HeightValue1+MarginGap}",
                                        AnchorMax = $"{DataValue2} {HeightValue2}",

                                        OffsetMax = "0 0"
                                    }
                                }
                            });


                            //Button Text
                            elements.Add(new CuiElement
                            {
                                Parent = "XMenu_MainContainer_DisplayContainer_Button",
                                Components =
                                {
                                    new CuiTextComponent
                                    {
                                        Color = "1 1 1 1",
                                        Text = $"{playerName} {keyPlayer.Key}",
                                        Align = TextAnchor.MiddleCenter,
                                        FontSize = 12
                                    },
                                    new CuiRectTransformComponent
                                    {
                                        AnchorMin = "0 0",
                                        AnchorMax = "1 1",
                                    }
                                }
                            });
                        } // Change Color On Button IF Player Banned
                        else if (PlayerReported(keyPlayer.Key)) // Change Color On Button IF Player Reported
                        {
                            //Button
                            elements.Add(new CuiElement
                            {
                                Name = "XMenu_MainContainer_DisplayContainer_Button",
                                Parent = "XMenu_MainContainer_DisplayPanel",
                                Components =
                                {
                                    new CuiButtonComponent
                                    {
                                        Color = "0.75 0.5 0 1",
                                        Command = $"xmenu.player.onclick {keyPlayer.Key} {keyPlayer.Value}"
                                    },

                                    new CuiRectTransformComponent
                                    {
                                        AnchorMin = $"{DataValue1+MarginGap} {HeightValue1+MarginGap}",
                                        AnchorMax = $"{DataValue2} {HeightValue2}",

                                        OffsetMax = "0 0"
                                    }
                                }
                            });


                            //Button Text
                            elements.Add(new CuiElement
                            {
                                Parent = "XMenu_MainContainer_DisplayContainer_Button",
                                Components =
                                {
                                    new CuiTextComponent
                                    {
                                        Color = "1 1 1 1",
                                        Text = $"{playerName} {keyPlayer.Key}",
                                        Align = TextAnchor.MiddleCenter,
                                        FontSize = 12
                                    },
                                    new CuiRectTransformComponent
                                    {
                                        AnchorMin = "0 0",
                                        AnchorMax = "1 1",
                                    }
                                }
                            });
                        }
                        else
                        {

                            //Button
                            elements.Add(new CuiElement
                            {
                                Name = "XMenu_MainContainer_DisplayContainer_Button",
                                Parent = "XMenu_MainContainer_DisplayPanel",
                                Components =
                            {
                                new CuiButtonComponent
                                {
                                    Color = "0.2 0.24 0.28 1",
                                    Command = $"xmenu.player.onclick {keyPlayer.Key} {keyPlayer.Value}"
                                },

                                new CuiRectTransformComponent
                                {
                                    AnchorMin = $"{DataValue1+MarginGap} {HeightValue1+MarginGap}",
                                    AnchorMax = $"{DataValue2} {HeightValue2}",

                                    OffsetMax = "0 0"
                                }
                            }
                            });


                            //Button Text
                            elements.Add(new CuiElement
                            {
                                Parent = "XMenu_MainContainer_DisplayContainer_Button",
                                Components =
                            {
                                new CuiTextComponent
                                {
                                    Color = "1 1 1 1",
                                    Text = $"{playerName} {keyPlayer.Key}",
                                    Align = TextAnchor.MiddleCenter,
                                    FontSize = 12
                                },
                                new CuiRectTransformComponent
                                {
                                    AnchorMin = "0 0",
                                    AnchorMax = "1 1",
                                }
                            }
                            });
                        }




                        DataValue1 += ButtonSizeWidth;
                        DataValue2 += ButtonSizeWidth;

                        PosIndex++;
                    }
                }


                cachedPlayerData[player.userID].CurrentPageItemsCount = IndexCounter;
                #endregion


                #region Bottom Menu Panel
                elements.Add(new CuiElement
                {
                    Name = "XMenu_MainContainer_Bottom_MenuPanel",
                    Parent = "XMenu_MainContainer",
                    Components =
                {
                    new CuiImageComponent{Color = "0.1 0.2 0.3 1"},
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0.005 0.005",
                        AnchorMax = "0.995 0.065",
                        OffsetMax = "0 0"
                    }
                }
                }); //Main Bottom Container



                #region Buttons
                const double SelectorButton_Width = 0.25f;
                const double SelectorButton_Gap = 0.005f;
                const double SelectorButton_StartGap = 0.115f;


                if (cachedPlayerData[player.userID].ActiveListName == "Online" || cachedPlayerData[player.userID].SearchOverRide == 1)
                {
                    elements.Add(new CuiElement
                    {
                        Name = "XMenu_MainContainer_Bottom_SelectorButton_Online",
                        Parent = "XMenu_MainContainer_Bottom_MenuPanel",

                        Components =
                    {
                        new CuiButtonComponent
                        {
                            Color = "0.2 0.4 0.6 1",
                            Command = "xmenu.setlist.online"
                        },
                        new CuiRectTransformComponent
                        {
                            AnchorMin = $"{SelectorButton_StartGap+SelectorButton_Gap} 0.1",
                            AnchorMax = $"{SelectorButton_StartGap+SelectorButton_Gap+SelectorButton_Width} 0.9",
                            OffsetMax = "0 0"
                        }
                    }
                    }); //Main Bottom Selector Online List
                }
                else
                {
                    elements.Add(new CuiElement
                    {
                        Name = "XMenu_MainContainer_Bottom_SelectorButton_Online",
                        Parent = "XMenu_MainContainer_Bottom_MenuPanel",

                        Components =
                        {
                            new CuiButtonComponent
                            {
                                Color = "0.2 0.3 0.4 1",
                                Command = "xmenu.setlist.online"
                            },
                            new CuiRectTransformComponent
                            {
                                AnchorMin = $"{SelectorButton_StartGap+SelectorButton_Gap} 0.1",
                                AnchorMax = $"{SelectorButton_StartGap+SelectorButton_Gap+SelectorButton_Width} 0.9",
                                OffsetMax = "0 0"
                            }
                        }
                    }); //Main Bottom Selector Online List
                }
                elements.Add(new CuiElement
                {
                    Parent = "XMenu_MainContainer_Bottom_SelectorButton_Online",
                    Components =
                    {
                        new CuiTextComponent
                        {
                            Text = "Online Players",
                            Align = UnityEngine.TextAnchor.MiddleCenter,
                            Color = "1 1 1 1",
                            FontSize = 14
                        },
                        new CuiRectTransformComponent
                        {
                            AnchorMin = "0 0",
                            AnchorMax = "1 1",
                            OffsetMax = "0 0"
                        }
                    }
                }); //Main Bottom Selector Online List TEXT


                if (cachedPlayerData[player.userID].ActiveListName == "Disk" || cachedPlayerData[player.userID].SearchOverRide == 2)
                {
                    elements.Add(new CuiElement
                    {
                        Name = "XMenu_MainContainer_Bottom_SelectorButton_Disk",
                        Parent = "XMenu_MainContainer_Bottom_MenuPanel",

                        Components =
                        {
                            new CuiButtonComponent
                            {
                                Color = "0.2 0.4 0.6 1",
                                Command = "xmenu.setlist.disk"
                            },
                            new CuiRectTransformComponent
                            {
                                AnchorMin = $"{SelectorButton_StartGap+SelectorButton_Gap*2+SelectorButton_Width*1} 0.1",
                                AnchorMax = $"{SelectorButton_StartGap+SelectorButton_Gap*2+SelectorButton_Width*2} 0.9",
                                OffsetMax = "0 0"
                            }
                        }
                    }); //Main Bottom Selector Disk List
                }
                else
                {
                    elements.Add(new CuiElement
                    {
                        Name = "XMenu_MainContainer_Bottom_SelectorButton_Disk",
                        Parent = "XMenu_MainContainer_Bottom_MenuPanel",

                        Components =
                        {
                            new CuiButtonComponent
                            {
                                Color = "0.2 0.3 0.4 1",
                                Command = "xmenu.setlist.disk"
                            },
                            new CuiRectTransformComponent
                            {
                                AnchorMin = $"{SelectorButton_StartGap+SelectorButton_Gap*2+SelectorButton_Width*1} 0.1",
                                AnchorMax = $"{SelectorButton_StartGap+SelectorButton_Gap*2+SelectorButton_Width*2} 0.9",
                                OffsetMax = "0 0"
                            }
                        }
                    }); //Main Bottom Selector Disk List
                }
                elements.Add(new CuiElement
                {
                    Parent = "XMenu_MainContainer_Bottom_SelectorButton_Disk",
                    Components =
                    {
                        new CuiTextComponent
                        {
                            Text = "All Players",
                            Align = UnityEngine.TextAnchor.MiddleCenter,
                            Color = "1 1 1 1",
                            FontSize = 14
                        },
                        new CuiRectTransformComponent
                        {
                            AnchorMin = "0 0",
                            AnchorMax = "1 1",
                            OffsetMax = "0 0"
                        }
                    }
                }); //Main Bottom Selector Disk List TEXT


                if (cachedPlayerData[player.userID].ActiveListName == "Banned" || cachedPlayerData[player.userID].SearchOverRide == 3)
                {
                    elements.Add(new CuiElement
                    {
                        Name = "XMenu_MainContainer_Bottom_SelectorButton_Banned",
                        Parent = "XMenu_MainContainer_Bottom_MenuPanel",

                        Components =
                        {
                            new CuiButtonComponent
                            {
                                Color = "0.2 0.4 0.6 1",
                                Command = "xmenu.setlist.banned"
                            },
                            new CuiRectTransformComponent
                            {
                                AnchorMin = $"{SelectorButton_StartGap+SelectorButton_Gap*3+SelectorButton_Width*2} 0.1",
                                AnchorMax = $"{SelectorButton_StartGap+SelectorButton_Gap*3+SelectorButton_Width*3} 0.9",
                                OffsetMax = "0 0"
                            }
                        }
                    }); //Main Bottom Selector Banned List
                }
                else
                {
                    elements.Add(new CuiElement
                    {
                        Name = "XMenu_MainContainer_Bottom_SelectorButton_Banned",
                        Parent = "XMenu_MainContainer_Bottom_MenuPanel",

                        Components =
                        {
                            new CuiButtonComponent
                            {
                                Color = "0.2 0.3 0.4 1",
                                Command = "xmenu.setlist.banned"
                            },
                            new CuiRectTransformComponent
                            {
                                AnchorMin = $"{SelectorButton_StartGap+SelectorButton_Gap*3+SelectorButton_Width*2} 0.1",
                                AnchorMax = $"{SelectorButton_StartGap+SelectorButton_Gap*3+SelectorButton_Width*3} 0.9",
                                OffsetMax = "0 0"
                            }
                        }
                    }); //Main Bottom Selector Banned List
                }
                elements.Add(new CuiElement
                {
                    Parent = "XMenu_MainContainer_Bottom_SelectorButton_Banned",
                    Components =
                    {
                        new CuiTextComponent
                        {
                            Text = "Banned Players",
                            Align = UnityEngine.TextAnchor.MiddleCenter,
                            Color = "1 1 1 1",
                            FontSize = 14
                        },
                        new CuiRectTransformComponent
                        {
                            AnchorMin = "0 0",
                            AnchorMax = "1 1",
                            OffsetMax = "0 0"
                        }
                    }
                }); //Main Bottom Selector Banned List TEXT





                if (cachedPlayerData[player.userID].CurrentPage >= 1)
                {
                    elements.Add(new CuiElement
                    {
                        Name = "XMenu_MainContainer_Bottom_SelectorButton_Prev",
                        Parent = "XMenu_MainContainer_Bottom_MenuPanel",
                        Components =
                        {
                            new CuiButtonComponent
                            {
                                Color = "0.1 0.15 0.2 1",
                                Command = "xmenu.prevpage"
                            },
                            new CuiRectTransformComponent
                            {
                                AnchorMin = "0.0695 0.1",
                                AnchorMax = $"{SelectorButton_StartGap} 0.9",
                                OffsetMax = "0 0"
                            }
                        }
                    }); //Main Bottom Selector Prev Button
                    elements.Add(new CuiElement
                    {
                        Parent = "XMenu_MainContainer_Bottom_SelectorButton_Prev",
                        Components =
                        {
                            new CuiTextComponent
                            {
                                Text = "<",
                                Align = UnityEngine.TextAnchor.MiddleCenter,
                                Color = "1 1 1 1",
                                FontSize = 20
                            },
                            new CuiRectTransformComponent
                            {
                                AnchorMin = "0 0",
                                AnchorMax = "1 1",
                                OffsetMax = "0 0"
                            }
                        }
                    }); // Main Bottom Selector Prev Button TEXT
                } //Selector Prev Button

                if (cachedPlayerData[player.userID].CurrentPageItemsCount > 60)
                {
                    elements.Add(new CuiElement
                    {
                        Name = "XMenu_MainContainer_Bottom_SelectorButton_Next",
                        Parent = "XMenu_MainContainer_Bottom_MenuPanel",
                        Components =
                        {
                            new CuiButtonComponent
                            {
                                Color = "0.1 0.15 0.2 1",
                                Command = "xmenu.nextpage"
                            },
                            new CuiRectTransformComponent
                            {
                                AnchorMin = $"{SelectorButton_StartGap+SelectorButton_Gap*4.25+SelectorButton_Width*3} 0.1",
                                AnchorMax = $"{SelectorButton_StartGap+SelectorButton_Gap*4.25+SelectorButton_Width*3+0.0475} 0.9",
                                OffsetMax = "0 0"
                            }
                        }
                    }); //Main Bottom Selector Next Button
                    elements.Add(new CuiElement
                    {
                        Parent = "XMenu_MainContainer_Bottom_SelectorButton_Next",
                        Components =
                        {
                            new CuiTextComponent
                            {
                                Text = ">",
                                Align = UnityEngine.TextAnchor.MiddleCenter,
                                Color = "1 1 1 1",
                                FontSize = 20
                            },
                            new CuiRectTransformComponent
                            {
                                AnchorMin = "0 0",
                                AnchorMax = "1 1",
                                OffsetMax = "0 0"
                            }
                        }
                    }); // Main Bottom Selector Next Button TEXT
                } //Selector Next Button


                #endregion















                const int MaxPages = 999;
                string PageIndicatorText = "";
                if (cachedPlayerData[player.userID].CurrentPage + 1 > Math.Ceiling(Convert.ToDecimal(cachedPlayerData[player.userID].ActiveList.Count) / 60)) { PageIndicatorText = "1\n1"; }
                else if (Math.Ceiling(Convert.ToDecimal(cachedPlayerData[player.userID].ActiveList.Count) / 60) >= MaxPages) { PageIndicatorText = $"{cachedPlayerData[player.userID].CurrentPage + 1}\n{MaxPages}+"; }
                else if (cachedPlayerData[player.userID].CurrentPage >= MaxPages) { PageIndicatorText = $"{MaxPages}+\n{MaxPages}+"; }
                else { PageIndicatorText = $"{cachedPlayerData[player.userID].CurrentPage + 1}\n{Math.Ceiling(Convert.ToDecimal(cachedPlayerData[player.userID].ActiveList.Count) / 60)}"; }
                elements.Add(new CuiElement
                {
                    Name = "XMenu_MainContainer_Bottom_PageIndicator",
                    Parent = "XMenu_MainContainer_Bottom_MenuPanel",
                    Components =
                    {
                        new CuiTextComponent
                        {
                            Text = $"{PageIndicatorText}",
                            Align = UnityEngine.TextAnchor.MiddleCenter,
                            Color = "1 1 1 1",
                            FontSize = 12
                        },
                        new CuiRectTransformComponent
                        {
                            AnchorMin = "0.935 0",
                            AnchorMax = "1 1",
                            OffsetMax = "0 0"
                        }
                    }
                }); //Page Indicator
                elements.Add(new CuiElement
                {
                    Parent = "XMenu_MainContainer_Bottom_PageIndicator",
                    Components =
                    {
                        new CuiImageComponent {Color = "1 1 1 1"},
                        new CuiRectTransformComponent
                        {
                            AnchorMin = "0.1 0.5",
                            AnchorMax = "0.9 0.51",
                            OffsetMax = "0 0"
                        }
                    }
                }); //Page Indicator Line


                #endregion



                #endregion
            }

            CuiHelper.DestroyUi(player, "XMenu_BlurElement");
            DrawBlurElement(player);

            CuiHelper.AddUi(player, elements);



            cachedPlayerData[player.userID].MenuMainOpen = true;

            ToggleFreezeTimer(player);
        } //Main UI


        private void DrawPlayerOptionsMenu(BasePlayer player)
        {
            PlayerData.LoadCachedData(player.userID); //Try Load Data

            CuiElementContainer elements = new CuiElementContainer();

            elements.Add(new CuiPanel
            {
                RectTransform =
                {
                    AnchorMin = "0.35 0.15",
                    AnchorMax = "0.65 0.85"
                },
                CursorEnabled = true
            }, "Overlay", "XMenu_SelectedPlayer_OptionsMenu"); //Main Panel

            #region Top Panel
            elements.Add(new CuiElement
            {
                Name = "XMenu_SelectedPlayer_OptionsMenu_TopPanel",
                Parent = "XMenu_SelectedPlayer_OptionsMenu",
                Components =
                {
                    new CuiImageComponent {Color = "0.2 0.4 0.6 1"},
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0 0.925",
                        AnchorMax = "1 1",
                        OffsetMax = "0 0"
                    }
                }
            }); //Top Panel
            elements.Add(new CuiElement
            {
                Name = "XMenu_SelectedPlayer_OptionsMenu_TopPanel_CloseButton",
                Parent = "XMenu_SelectedPlayer_OptionsMenu_TopPanel",
                Components =
                {
                    new CuiButtonComponent
                    {
                        Color = "0.5 0 0 1",
                        Command = "xmenu.menu.destory",
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0.915 0.05",
                        AnchorMax = "0.995 0.95",
                        OffsetMax = "0 0"
                    }
                }
            }); //Top Panel Close Button
            elements.Add(new CuiElement
            {
                Parent = "XMenu_SelectedPlayer_OptionsMenu_TopPanel_CloseButton",
                Components =
                {
                    new CuiTextComponent
                    {
                        Text = "X",
                        Align = UnityEngine.TextAnchor.MiddleCenter,
                        Color = "1 1 1 1",
                        FontSize = 20,
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMax = "1 1",
                        OffsetMax = "0 0"
                    }
                }
            }); //Top Panel Close Button Text

            elements.Add(new CuiElement
            {
                Name = "XMenu_SelectedPlayer_OptionsMenu_TopPanel_Text",
                Parent = "XMenu_SelectedPlayer_OptionsMenu_TopPanel",
                Components =
                {
                    new CuiTextComponent
                    {
                        Text = "Player Administration Menu",
                        Color = "1 1 1 1",
                        Align = UnityEngine.TextAnchor.MiddleLeft,
                        FontSize = 20,
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0.01 0",
                        AnchorMax = "0.9 1",
                        OffsetMax = "0 0"
                    }
                }
            }); //Top Panel Text Display
            #endregion

            #region Sub Top Search Panel
            elements.Add(new CuiElement
            {
                Name = "XMenu_SelectedPlayer_OptionsMenu_Sub_TopPanel",
                Parent = "XMenu_SelectedPlayer_OptionsMenu",
                Components =
                {
                    new CuiImageComponent { Color = "0.1 0.2 0.3 1"},
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0.005 0.865",
                        AnchorMax = "0.995 0.925",
                        OffsetMax = "0 0"
                    }
                }
            }); //Top Search Container Panel

            elements.Add(new CuiElement
            {
                Name = "jdklsajfgklsa",
                Parent = "XMenu_SelectedPlayer_OptionsMenu_Sub_TopPanel",
                Components =
                {
                    new CuiTextComponent
                    {
                        Text = $"{cachedPlayerData[player.userID].SelectedTargetName} ({cachedPlayerData[player.userID].SelectedTargetID})",
                        Align = TextAnchor.MiddleCenter,
                        Color = "1 1 1 1",
                        FontSize = 16
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0 0",
                        AnchorMax = "1 1",
                        OffsetMax = "0 0"
                    }
                }
            });
            #endregion

            #region Border Lines
            elements.Add(new CuiElement
            {
                Parent = "XMenu_SelectedPlayer_OptionsMenu",
                Components =
                {
                    new CuiImageComponent{Color = "0.2 0.4 0.6 1"},
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0 0",
                        AnchorMax = "1 0.005",
                        OffsetMax = "0 0",
                    }
                }

            }); //Bottom Border

            elements.Add(new CuiElement
            {
                Parent = "XMenu_SelectedPlayer_OptionsMenu",
                Components =
                {
                    new CuiImageComponent{Color = "0.2 0.4 0.6 1"},
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0 0",
                        AnchorMax = "0.005 0.925",
                        OffsetMax = "0 0",
                    }
                }

            }); //Left Border

            elements.Add(new CuiElement
            {
                Parent = "XMenu_SelectedPlayer_OptionsMenu",
                Components =
                {
                    new CuiImageComponent
                    {
                        Color = "0.2 0.4 0.6 1",
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0.995 0",
                        AnchorMax = "1 0.925",
                        OffsetMax = "0 0"
                    }
                }
            }); //Right Border
            #endregion

            #region Bottom Menu Panel
            elements.Add(new CuiElement
            {
                Name = "XMenu_SelectedPlayer_Bottom_MenuPanel",
                Parent = "XMenu_SelectedPlayer_OptionsMenu",
                Components =
                {
                    new CuiImageComponent{Color = "0.1 0.2 0.3 1"},
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0.005 0.005",
                        AnchorMax = "0.995 0.065",
                        OffsetMax = "0 0"
                    }
                }
            }); //Main Bottom Container


            elements.Add(new CuiElement
            {
                Name = "XMenu_SelectedPlayer_Bottom_Button_Goback",
                Parent = "XMenu_SelectedPlayer_Bottom_MenuPanel",
                Components =
                {
                    new CuiButtonComponent
                    {
                        Color = "0.2 0.3 0.4 1",
                        Command = "xmenu.menu.goback",
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0.35 0.1",
                        AnchorMax = "0.65 0.9",
                        OffsetMax = "0 0"
                    }
                }
            }); //Go back Button
            elements.Add(new CuiElement
            {
                Parent = "XMenu_SelectedPlayer_Bottom_Button_Goback",
                Components =
                {
                    new CuiTextComponent
                    {
                        Text = "Go Back",
                        Align = TextAnchor.MiddleCenter,
                        Color = "1 1 1 1",
                        FontSize = 14,
                    }
                }
            }); //Go back Button Text


            #endregion

            #region Display Panel
            elements.Add(new CuiElement
            {
                Name = "XMenu_SelectedPlayer_OptionsMenu_DisplayPanel",
                Parent = "XMenu_SelectedPlayer_OptionsMenu",
                Components =
                    {
                        new CuiImageComponent { Color = "0.1 0.1 0.1 1"},
                        new CuiRectTransformComponent
                        {
                            AnchorMin = "0.005 0.065",
                            AnchorMax = "0.995 0.865",
                            OffsetMax = "0 0"
                        }
                    }
            }); //Main Display Panel Container


            #region Reason To Action Input Field

            elements.Add(new CuiElement
            {
                Name = "XMenu_SelectedPlayer_OptionsMenu_DisplayPanel_ReasonToAction",
                Parent = "XMenu_SelectedPlayer_OptionsMenu_DisplayPanel",
                Components =
                {
                    new CuiImageComponent{ Color = "0.1 0.1 0.1 1"},
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0 0.89",
                        AnchorMax = "1 0.995",
                        OffsetMax = "0 0"
                    }
                }
            }); //Actions Reason Box
            elements.Add(new CuiElement
            {
                Parent = "XMenu_SelectedPlayer_OptionsMenu_DisplayPanel_ReasonToAction",
                Components =
                {
                    new CuiTextComponent
                    {
                        Text = "Reason To Action (Required)",
                        Align = TextAnchor.MiddleCenter,
                        Color = "1 1 1 1",
                        FontSize = 12
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0 0.55",
                        AnchorMax = "1 1",
                        OffsetMax = "0 0"
                    }
                }
            });//Actions Reason Box Text



            elements.Add(new CuiElement
            {
                Name = "XMenu_SelectedPlayer_OptionsMenu_DisplayPanel_ReasonToAction_InputBox",
                Parent = "XMenu_SelectedPlayer_OptionsMenu_DisplayPanel_ReasonToAction",
                Components =
                {
                    new CuiImageComponent { Color = "0.2 0.24 0.28 1"},
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0.005 0.05",
                        AnchorMax = "0.995 0.55",
                        OffsetMax = "0 0"
                    }
                }
            });//Actions Reason Box InputBox
            elements.Add(new CuiElement
            {
                Parent = "XMenu_SelectedPlayer_OptionsMenu_DisplayPanel_ReasonToAction_InputBox",
                Components =
                {
                    new CuiInputFieldComponent
                    {
                        Color = "1 1 1 1",
                        Align = TextAnchor.MiddleLeft,
                        FontSize = 14,
                        Command = "",
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0.005 0",
                        AnchorMax = "1 1",
                        OffsetMax = "0 0"
                    }
                }
            });

            #endregion

            elements.Add(new CuiElement
            {
                Parent = "XMenu_SelectedPlayer_OptionsMenu_DisplayPanel",
                Components =
                {
                    new CuiImageComponent {Color = "0.2 0.4 0.6 1"},
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0 0.9925",
                        AnchorMax = "1 1",
                        OffsetMax = "0 0"
                    }
                }
            }); //Splitter Line
            #region Button Options

            elements.Add(new CuiElement
            {
                Name = "XMenu_SelectedPlayer_OptionsMenu_Button_TeleportToPlayer",
                Parent = "XMenu_SelectedPlayer_OptionsMenu_DisplayPanel",
                Components =
                {
                    new CuiButtonComponent
                    {
                        Color = "0.2 0.24 0.28 1"
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0.005 0.810",
                        AnchorMax = "0.4925 0.885",
                        OffsetMax = "0 0"
                    }
                }

            }); //Button Teleport To Player
            elements.Add(new CuiElement
            {
                Parent = "XMenu_SelectedPlayer_OptionsMenu_Button_TeleportToPlayer",
                Components =
                {
                    new CuiTextComponent
                    {
                        Text = "Teleport To Player",
                        Align = TextAnchor.MiddleCenter,
                        Color = "1 1 1 1",
                        FontSize = 14,
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0 0",
                        AnchorMax = "1 1",
                        OffsetMax = "0 0"
                    }
                }
            }); //Button Teleport To Player Text


            elements.Add(new CuiElement
            {
                Name = "XMenu_SelectedPlayer_OptionsMenu_Button_JailPlayer",
                Parent = "XMenu_SelectedPlayer_OptionsMenu_DisplayPanel",
                Components =
                {
                    new CuiButtonComponent
                    {
                        Color = "0.2 0.24 0.28 1"
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0.4975 0.810",
                        AnchorMax = "0.995 0.885",
                        OffsetMax = "0 0"
                    }
                }

            }); //Button Jail Player
            elements.Add(new CuiElement
            {
                Parent = "XMenu_SelectedPlayer_OptionsMenu_Button_JailPlayer",
                Components =
                {
                    new CuiTextComponent
                    {
                        Text = "Jail Player",
                        Align = TextAnchor.MiddleCenter,
                        Color = "1 1 1 1",
                        FontSize = 14,
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0 0",
                        AnchorMax = "1 1",
                        OffsetMax = "0 0"
                    }
                }
            }); //Button Jail Player Text


            elements.Add(new CuiElement
            {
                Name = "XMenu_SelectedPlayer_OptionsMenu_Button_MutePlayer",
                Parent = "XMenu_SelectedPlayer_OptionsMenu_DisplayPanel",
                Components =
                {
                    new CuiButtonComponent
                    {
                        Color = "0.2 0.24 0.28 1"
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0.005 0.73",
                        AnchorMax = "0.4925 0.805",
                        OffsetMax = "0 0"
                    }
                }

            }); //Button Mute Player
            elements.Add(new CuiElement
            {
                Parent = "XMenu_SelectedPlayer_OptionsMenu_Button_MutePlayer",
                Components =
                {
                    new CuiTextComponent
                    {
                        Text = "Mute Player",
                        Align = TextAnchor.MiddleCenter,
                        Color = "1 1 1 1",
                        FontSize = 14,
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0 0",
                        AnchorMax = "1 1",
                        OffsetMax = "0 0"
                    }

                }
            }); //Button Mute Player Text

            //Report Button
            if (PlayerReported(Convert.ToUInt64(cachedPlayerData[player.userID].SelectedTargetID)))
            {
                elements.Add(new CuiElement
                {
                    Name = "XMenu_SelectedPlayer_OptionsMenu_Button_ReportPlayer",
                    Parent = "XMenu_SelectedPlayer_OptionsMenu_DisplayPanel",
                    Components =
                    {
                        new CuiButtonComponent
                        {
                            Color = "0.15 0.15 0.15 1",
                            Command = "xmenu.open.menu.selected.createreport",
                        },
                        new CuiRectTransformComponent
                        {
                            AnchorMin = "0.4975 0.73",
                            AnchorMax = "0.995 0.805",
                            OffsetMax = "0 0"
                        }
                    }

                }); //Button Make Report
                elements.Add(new CuiElement
                {
                    Parent = "XMenu_SelectedPlayer_OptionsMenu_Button_ReportPlayer",
                    Components =
                {
                    new CuiTextComponent
                    {
                        Text = "Make Report",
                        Align = TextAnchor.MiddleCenter,
                        Color = "1 1 1 1",
                        FontSize = 14,
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0 0",
                        AnchorMax = "1 1",
                        OffsetMax = "0 0"
                    }
                }
                }); //Button Mute Player Text
            }
            else
            {
                elements.Add(new CuiElement
                {
                    Name = "XMenu_SelectedPlayer_OptionsMenu_Button_ReportPlayer",
                    Parent = "XMenu_SelectedPlayer_OptionsMenu_DisplayPanel",
                    Components =
                    {
                        new CuiButtonComponent
                        {
                            Color = "0.2 0.24 0.28 1",
                            Command = "xmenu.open.menu.selected.createreport",
                        },
                        new CuiRectTransformComponent
                        {
                            AnchorMin = "0.4975 0.73",
                            AnchorMax = "0.995 0.805",
                            OffsetMax = "0 0"
                        }
                    }

                }); //Button Make Report
                elements.Add(new CuiElement
                {
                    Parent = "XMenu_SelectedPlayer_OptionsMenu_Button_ReportPlayer",
                    Components =
                {
                    new CuiTextComponent
                    {
                        Text = "Make Report",
                        Align = TextAnchor.MiddleCenter,
                        Color = "1 1 1 1",
                        FontSize = 14,
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0 0",
                        AnchorMax = "1 1",
                        OffsetMax = "0 0"
                    }
                }
                }); //Button Mute Player Text
            }


            //Ban Button
            if (PlayerBanned(Convert.ToUInt64(cachedPlayerData[player.userID].SelectedTargetID)))
            {

                elements.Add(new CuiElement
                {
                    Name = "XMenu_SelectedPlayer_OptionsMenu_Button_BanPlayer",
                    Parent = "XMenu_SelectedPlayer_OptionsMenu_DisplayPanel",
                    Components =
                {
                    new CuiButtonComponent
                    {
                        Color = "0.15 0.15 0.15 1",
                        Command = "xmenu.open.menu.selected.createban",
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0.005 0.005",
                        AnchorMax = "0.995 0.095",
                        OffsetMax = "0 0"
                    }
                }

                }); //Button Ban Player
                elements.Add(new CuiElement
                {
                    Parent = "XMenu_SelectedPlayer_OptionsMenu_Button_BanPlayer",
                    Components =
                {
                    new CuiTextComponent
                    {
                        Text = "Create Ban",
                        Align = TextAnchor.MiddleCenter,
                        Color = "1 1 1 1",
                        FontSize = 14,
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0 0",
                        AnchorMax = "1 1",
                        OffsetMax = "0 0"
                    }
                }
                }); //Button Ban Player Text
            }
            else
            {
                elements.Add(new CuiElement
                {
                    Name = "XMenu_SelectedPlayer_OptionsMenu_Button_BanPlayer",
                    Parent = "XMenu_SelectedPlayer_OptionsMenu_DisplayPanel",
                    Components =
                {
                    new CuiButtonComponent
                    {
                        Color = "0.2 0.24 0.28 1",
                        Command = "xmenu.open.menu.selected.createban",
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0.005 0.005",
                        AnchorMax = "0.995 0.095",
                        OffsetMax = "0 0"
                    }
                }

                }); //Button Ban Player
                elements.Add(new CuiElement
                {
                    Parent = "XMenu_SelectedPlayer_OptionsMenu_Button_BanPlayer",
                    Components =
                {
                    new CuiTextComponent
                    {
                        Text = "Create Ban",
                        Align = TextAnchor.MiddleCenter,
                        Color = "1 1 1 1",
                        FontSize = 14,
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0 0",
                        AnchorMax = "1 1",
                        OffsetMax = "0 0"
                    }
                }
                }); //Button Ban Player Text
            }

            #endregion


            #endregion



            CuiHelper.AddUi(player, elements);


        }


        private void DrawPlayerBannedMenu(BasePlayer player)
        {
            CuiElementContainer elements = new CuiElementContainer();

            elements.Add(new CuiPanel
            {
                RectTransform =
                {
                    AnchorMin = "0.35 0.15",
                    AnchorMax = "0.65 0.85"
                },
                CursorEnabled = true
            }, "Overlay", "XMenu_BannedPlayer_Menu"); //Main Panel



            string Ban_Title = string.Empty;
            string Ban_Reason = string.Empty;
            string Ban_Duration = string.Empty;
            string Ban_Evidence = string.Empty;
            string Ban_Appealable = string.Empty;
            string Ban_Expire = string.Empty;
            string Ban_Details = string.Empty;
            string Ban_Author = string.Empty;


            var list = cachedPlayerData[player.userID].SelectedBannedTargetPlayerInfo_Args;
            if (!String.IsNullOrEmpty(list[0]) && !String.IsNullOrEmpty(list[1]) && !String.IsNullOrEmpty(list[2]) && !String.IsNullOrEmpty(list[3]) && !String.IsNullOrEmpty(list[4]) && !String.IsNullOrEmpty(list[5]) && !String.IsNullOrEmpty(list[6]))
            {
                Ban_Title = $"{list[1]}";
                Ban_Reason = $"{list[2]}";
                Ban_Duration = $"{list[3]}";
                Ban_Evidence = $"{list[4]}";
                Ban_Appealable = $"{list[5]}";
                Ban_Details = $"{list[6]}";
                Ban_Author = $"{list[7]}";
                Ban_Expire = $"{list[8]}";


                #region Ban Checker

                var ExpireDatetime = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(Ban_Expire));

                string Ban_Duration_Format = "";

                if (Ban_Duration.All(char.IsDigit))
                {

                    int valueToCheck = Convert.ToInt32(Ban_Duration);

                    if (valueToCheck < 24)
                    {
                        Ban_Duration_Format = "hour(s)";
                    }
                    else if (valueToCheck >= 24 && valueToCheck <= 168)
                    {
                        double value = Convert.ToDouble(Ban_Duration) / 24;
                        Ban_Duration = value.ToString("0.0");

                        Ban_Duration_Format = "day(s)";
                    }
                    else if (valueToCheck >= 168)
                    {
                        double value = Convert.ToDouble(Ban_Duration) / 168;
                        Ban_Duration = value.ToString("0.0");
                        Ban_Duration_Format = "week(s)";
                    }

                }



                if (Convert.ToInt32(Ban_Expire) <= 0 || Ban_Expire == "-1") //Permanent
                {
                    Ban_Duration = "Permanent";
                    Ban_Expire = "Never";
                }
                else //Timed
                {

                    Ban_Duration = $"{Ban_Duration} {Ban_Duration_Format}";
                    Ban_Expire = $"{ExpireDatetime.ToString("HH:mm-dd/MM/yyyy")}";
                }

                #endregion

                #region Details Checker
                if (list[1].Length >= 43) { list[1] = $"{list[1].Substring(0, 43)}..."; }
                if (list[2].Length >= 42) { list[2] = $"{list[2].Substring(0, 42)}..."; }
                if (list[3].Length >= 41) { list[3] = $"{list[3].Substring(0, 41)}..."; }
                if (list[4].Length >= 41) { list[4] = $"{list[4].Substring(0, 41)}..."; }
                if (list[5].Length >= 40) { list[5] = $"{list[5].Substring(0, 40)}..."; }
                if (list[6].Length >= 38) { list[6] = $"{list[6].Substring(0, 38)}..."; }
                if (list[7].Length >= 38) { list[7] = $"{list[7].Substring(0, 38)}..."; }
                #endregion



                Ban_Title = $"{list[1]}";
                Ban_Reason = $"{list[2]}";
                Ban_Duration = $"{list[3]}";
                Ban_Evidence = $"{list[4]}";
                Ban_Appealable = $"{list[5]}";
                Ban_Details = $"{list[6]}";
                Ban_Author = $"{list[7]}";
                Ban_Expire = $"{list[8]}";


            }


            //FIX



            #region Top Panel
            elements.Add(new CuiElement
            {
                Name = "XMenu_SelectedPlayer_BannedMenu_TopPanel",
                Parent = "XMenu_BannedPlayer_Menu",
                Components =
                    {
                        new CuiImageComponent {Color = "0.2 0.4 0.6 1"},
                        new CuiRectTransformComponent
                        {
                            AnchorMin = "0 0.925",
                            AnchorMax = "1 1",
                            OffsetMax = "0 0"
                        }
                    }
            }); //Top Panel
            elements.Add(new CuiElement
            {
                Name = "XMenu_SelectedPlayer_BannedMenu_TopPanel_CloseButton",
                Parent = "XMenu_SelectedPlayer_BannedMenu_TopPanel",
                Components =
                    {
                        new CuiButtonComponent
                        {
                            Color = "0.5 0 0 1",
                            Command = "xmenu.menu.destory",
                        },
                        new CuiRectTransformComponent
                        {
                            AnchorMin = "0.915 0.05",
                            AnchorMax = "0.995 0.95",
                            OffsetMax = "0 0"
                        }
                    }
            }); //Top Panel Close Button
            elements.Add(new CuiElement
            {
                Parent = "XMenu_SelectedPlayer_BannedMenu_TopPanel_CloseButton",
                Components =
                    {
                        new CuiTextComponent
                        {
                            Text = "X",
                            Align = UnityEngine.TextAnchor.MiddleCenter,
                            Color = "1 1 1 1",
                            FontSize = 20,
                        },
                        new CuiRectTransformComponent
                        {
                            AnchorMax = "1 1",
                            OffsetMax = "0 0"
                        }
                    }
            }); //Top Panel Close Button Text

            elements.Add(new CuiElement
            {
                Name = "XMenu_SelectedPlayer_BannedMenu_TopPanel_Text",
                Parent = "XMenu_SelectedPlayer_BannedMenu_TopPanel",
                Components =
                    {
                        new CuiTextComponent
                        {
                            Text = "Player Administration Menu",
                            Color = "1 1 1 1",
                            Align = UnityEngine.TextAnchor.MiddleLeft,
                            FontSize = 20,
                        },
                        new CuiRectTransformComponent
                        {
                            AnchorMin = "0.01 0",
                            AnchorMax = "0.9 1",
                            OffsetMax = "0 0"
                        }
                    }
            }); //Top Panel Text Display
            #endregion

            #region Sub Top Panel
            elements.Add(new CuiElement
            {
                Name = "XMenu_SelectedPlayer_BannedPlayer_Sub_TopPanel",
                Parent = "XMenu_BannedPlayer_Menu",
                Components =
                    {
                        new CuiImageComponent { Color = "0.1 0.2 0.3 1"},
                        new CuiRectTransformComponent
                        {
                            AnchorMin = "0.005 0.865",
                            AnchorMax = "0.995 0.925",
                            OffsetMax = "0 0"
                        }
                    }
            }); //Top Search Container Panel

            elements.Add(new CuiElement
            {
                Parent = "XMenu_SelectedPlayer_BannedPlayer_Sub_TopPanel",
                Components =
                {
                    new CuiTextComponent
                    {
                        Text = $"{cachedPlayerData[player.userID].SelectedTargetName} ({cachedPlayerData[player.userID].SelectedTargetID})",
                        Align = TextAnchor.MiddleCenter,
                        Color = "1 1 1 1",
                        FontSize = 16
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0 0",
                        AnchorMax = "1 1",
                        OffsetMax = "0 0"
                    }
                }
            });
            #endregion

            #region Border Lines
            elements.Add(new CuiElement
            {
                Parent = "XMenu_BannedPlayer_Menu",
                Components =
                {
                    new CuiImageComponent{Color = "0.2 0.4 0.6 1"},
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0 0",
                        AnchorMax = "1 0.005",
                        OffsetMax = "0 0",
                    }
                }

            }); //Bottom Border

            elements.Add(new CuiElement
            {
                Parent = "XMenu_BannedPlayer_Menu",
                Components =
                {
                    new CuiImageComponent{Color = "0.2 0.4 0.6 1"},
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0 0",
                        AnchorMax = "0.005 0.925",
                        OffsetMax = "0 0",
                    }
                }

            }); //Left Border

            elements.Add(new CuiElement
            {
                Parent = "XMenu_BannedPlayer_Menu",
                Components =
                {
                    new CuiImageComponent
                    {
                        Color = "0.2 0.4 0.6 1",
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0.995 0",
                        AnchorMax = "1 0.925",
                        OffsetMax = "0 0"
                    }
                }
            }); //Right Border
            #endregion

            #region Display Panel
            elements.Add(new CuiElement
            {
                Name = "XMenu_BannedPlayer_Menu_DisplayPanel",
                Parent = "XMenu_BannedPlayer_Menu",
                Components =
                {
                    new CuiImageComponent { Color = "0.5 0.1 0.1 1"},
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0.005 0.065",
                        AnchorMax = "0.995 0.865",
                        OffsetMax = "0 0"
                    }
                }
            }); //Main Display Panel Container


            elements.Add(new CuiElement
            {
                Parent = "XMenu_BannedPlayer_Menu_DisplayPanel",
                Components =
                {
                    new CuiImageComponent {Color = "0.2 0.4 0.6 1"},
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0 0.9925",
                        AnchorMax = "1 1",
                        OffsetMax = "0 0"
                    }
                }
            }); //Splitter Line

            #region Display Info
            elements.Add(new CuiElement
            {
                Name = "XMenu_BannedPlayer_Menu_DisplayPanel_Title_Container",
                Parent = "XMenu_BannedPlayer_Menu_DisplayPanel",
                Components =
                {
                    new CuiImageComponent { Color = "0.1 0.1 0.1 1"},
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0 0.915", //0.075 Height + Gap
                        AnchorMax = "1 0.995",
                        OffsetMax = "0 0"
                    }
                }
            }); //Title Container Box
            elements.Add(new CuiElement
            {
                Name = "XMenu_BannedPlayer_Menu_DisplayPanel_Title_Container_Box",
                Parent = "XMenu_BannedPlayer_Menu_DisplayPanel_Title_Container",
                Components =
                {
                    new CuiTextComponent
                    {
                        Text = $"{TextEncodeing(14,"ffffff", "Title:")} {TextEncodeing(14,"a5a5a5",Ban_Title.ToLower())}",
                        Align = TextAnchor.MiddleLeft,
                        Color = "1 1 1 1",
                        FontSize = 14,
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0.005 0.2",
                        AnchorMax = "0.995 0.8",
                        OffsetMax = "0 0"
                    }
                }
            }); //Title Container Box Text


            elements.Add(new CuiElement
            {
                Parent = "XMenu_BannedPlayer_Menu_DisplayPanel",
                Components =
                {
                    new CuiImageComponent {Color = "0.2 0.4 0.6 1"},
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0 0.91",
                        AnchorMax = "1 0.915",
                        OffsetMax = "0 0"
                    }
                }
            }); //Splitter Line


            elements.Add(new CuiElement
            {
                Name = "XMenu_BannedPlayer_Menu_DisplayPanel_Reason_Container",
                Parent = "XMenu_BannedPlayer_Menu_DisplayPanel",
                Components =
                {
                    new CuiImageComponent { Color = "0.1 0.1 0.1 1"},
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0 0.83", //0.075 Height + Gap
                        AnchorMax = "1 0.91",
                        OffsetMax = "0 0"
                    }
                }
            }); //Reason Container Box
            elements.Add(new CuiElement
            {
                Name = "XMenu_BannedPlayer_Menu_DisplayPanel_Reason_Container_Box",
                Parent = "XMenu_BannedPlayer_Menu_DisplayPanel_Reason_Container",
                Components =
                {
                    new CuiTextComponent
                    {
                        Text = $"{TextEncodeing(14,"ffffff", "Reason:")} {TextEncodeing(14,"a5a5a5",Ban_Reason.ToLower())}",
                        Align = TextAnchor.MiddleLeft,
                        Color = "1 1 1 1",
                        FontSize = 14
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0.005 0.2",
                        AnchorMax = "0.995 0.8",
                        OffsetMax = "0 0"
                    }
                }
            }); //Reason Container Box Text


            elements.Add(new CuiElement
            {
                Parent = "XMenu_BannedPlayer_Menu_DisplayPanel",
                Components =
                {
                    new CuiImageComponent {Color = "0.2 0.4 0.6 1"},
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0 0.825",
                        AnchorMax = "1 0.83",
                        OffsetMax = "0 0"
                    }
                }
            }); //Splitter Line


            elements.Add(new CuiElement
            {
                Name = "XMenu_BannedPlayer_Menu_DisplayPanel_Duration_Container",
                Parent = "XMenu_BannedPlayer_Menu_DisplayPanel",
                Components =
                {
                    new CuiImageComponent { Color = "0.1 0.1 0.1 1"},
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0 0.7445", //0.075 Height + Gap
                        AnchorMax = "1 0.825",
                        OffsetMax = "0 0"
                    }
                }
            }); //Duration Container Box
            elements.Add(new CuiElement
            {
                Name = "XMenu_BannedPlayer_Menu_DisplayPanel_Duration_Container_Box",
                Parent = "XMenu_BannedPlayer_Menu_DisplayPanel_Duration_Container",
                Components =
                {
                    new CuiTextComponent
                    {
                        Text = $"{TextEncodeing(14,"ffffff", "Duration:")} {TextEncodeing(14,"a5a5a5",Ban_Duration.ToLower())}",
                        Align = TextAnchor.MiddleLeft,
                        Color = "1 1 1 1",
                        FontSize = 14
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0.005 0.2",
                        AnchorMax = "0.995 0.8",
                        OffsetMax = "0 0"
                    }
                }
            }); //Duration Container Box Text


            elements.Add(new CuiElement
            {
                Parent = "XMenu_BannedPlayer_Menu_DisplayPanel",
                Components =
                {
                    new CuiImageComponent {Color = "0.2 0.4 0.6 1"},
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0 0.739",
                        AnchorMax = "1 0.744",
                        OffsetMax = "0 0"
                    }
                }
            }); //Splitter Line


            elements.Add(new CuiElement
            {
                Name = "XMenu_BannedPlayer_Menu_DisplayPanel_Evidence_Container",
                Parent = "XMenu_BannedPlayer_Menu_DisplayPanel",
                Components =
                {
                    new CuiImageComponent { Color = "0.1 0.1 0.1 1"},
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0 0.6585", //0.075 Height + Gap
                        AnchorMax = "1 0.7395",
                        OffsetMax = "0 0"
                    }
                }
            }); //Evidence Container Box
            elements.Add(new CuiElement
            {
                Name = "XMenu_BannedPlayer_Menu_DisplayPanel_Evidence_Container_Box",
                Parent = "XMenu_BannedPlayer_Menu_DisplayPanel_Evidence_Container",
                Components =
                {
                    new CuiTextComponent
                    {
                        Text = $"{TextEncodeing(14,"ffffff", "Evidence:")} {TextEncodeing(14,"a5a5a5",Ban_Evidence.ToLower())}",
                        Align = TextAnchor.MiddleLeft,
                        Color = "1 1 1 1",
                        FontSize = 14
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0.005 0.2",
                        AnchorMax = "0.995 0.8",
                        OffsetMax = "0 0"
                    }
                }
            }); //Evidence Container Box Text


            elements.Add(new CuiElement
            {
                Parent = "XMenu_BannedPlayer_Menu_DisplayPanel",
                Components =
                {
                    new CuiImageComponent {Color = "0.2 0.4 0.6 1"},
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0 0.6533",
                        AnchorMax = "1 0.6585",
                        OffsetMax = "0 0"
                    }
                }
            }); //Splitter Line


            elements.Add(new CuiElement
            {
                Name = "XMenu_BannedPlayer_Menu_DisplayPanel_Appealable_Container",
                Parent = "XMenu_BannedPlayer_Menu_DisplayPanel",
                Components =
                {
                    new CuiImageComponent { Color = "0.1 0.1 0.1 1"},
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0 0.573", //0.075 Height + Gap
                        AnchorMax = "1 0.654",
                        OffsetMax = "0 0"
                    }
                }
            }); //Appealable Date Container Box
            elements.Add(new CuiElement
            {
                Name = "XMenu_BannedPlayer_Menu_DisplayPanel_Appealable_Container_Box",
                Parent = "XMenu_BannedPlayer_Menu_DisplayPanel_Appealable_Container",
                Components =
                {
                    new CuiTextComponent
                    {
                        Text = $"{TextEncodeing(14,"ffffff", "Appealable:")} {TextEncodeing(14,"a5a5a5",Ban_Appealable.ToLower())}",
                        Align = TextAnchor.MiddleLeft,
                        Color = "1 1 1 1",
                        FontSize = 14
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0.005 0.2",
                        AnchorMax = "0.995 0.8",
                        OffsetMax = "0 0"
                    }
                }
            }); //Appealable Date Container Box Text


            elements.Add(new CuiElement
            {
                Parent = "XMenu_BannedPlayer_Menu_DisplayPanel",
                Components =
                {
                    new CuiImageComponent {Color = "0.2 0.4 0.6 1"},
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0 0.569",
                        AnchorMax = "1 0.574",
                        OffsetMax = "0 0"
                    }
                }
            }); //Splitter Line


            elements.Add(new CuiElement
            {
                Name = "XMenu_BannedPlayer_Menu_DisplayPanel_BanExpires_Container",
                Parent = "XMenu_BannedPlayer_Menu_DisplayPanel",
                Components =
                {
                    new CuiImageComponent { Color = "0.1 0.1 0.1 1"},
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0 0.4875", //0.075 Height + Gap
                        AnchorMax = "1 0.568",
                        OffsetMax = "0 0"
                    }
                }
            }); //Expires Container Box
            elements.Add(new CuiElement
            {
                Name = "XMenu_BannedPlayer_Menu_DisplayPanel_BanExpires_Container_Box",
                Parent = "XMenu_BannedPlayer_Menu_DisplayPanel_BanExpires_Container",
                Components =
                {
                    new CuiTextComponent
                    {
                        Text = $"{TextEncodeing(14,"ffffff", "Expires at:")} {TextEncodeing(14,"a5a5a5",Ban_Expire.ToLower())}",
                        Align = TextAnchor.MiddleLeft,
                        Color = "1 1 1 1",
                        FontSize = 14
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0.005 0.2",
                        AnchorMax = "0.995 0.8",
                        OffsetMax = "0 0"
                    }
                }
            }); //Expires Container Box Text


            elements.Add(new CuiElement
            {
                Parent = "XMenu_BannedPlayer_Menu_DisplayPanel",
                Components =
                {
                    new CuiImageComponent {Color = "0.2 0.4 0.6 1"},
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0 0.4825",
                        AnchorMax = "1 0.4875",
                        OffsetMax = "0 0"
                    }
                }
            }); //Splitter Line


            elements.Add(new CuiElement
            {
                Name = "XMenu_BannedPlayer_Menu_DisplayPanel_ReportDetails_Container",
                Parent = "XMenu_BannedPlayer_Menu_DisplayPanel",
                Components =
                {
                    new CuiImageComponent { Color = "0.1 0.1 0.1 1"},
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0 0.4025", //0.075 Height + Gap
                        AnchorMax = "1 0.4825",
                        OffsetMax = "0 0"
                    }
                }
            }); //Report Details Container Box
            elements.Add(new CuiElement
            {
                Name = "XMenu_BannedPlayer_Menu_DisplayPanel_ReportDetails_Container_Box",
                Parent = "XMenu_BannedPlayer_Menu_DisplayPanel_ReportDetails_Container",
                Components =
                {
                    new CuiTextComponent
                    {
                        Text = $"{TextEncodeing(14,"ffffff", "Report Details:")} {TextEncodeing(14,"a5a5a5",Ban_Details.ToLower())}",
                        Align = TextAnchor.MiddleLeft,
                        Color = "1 1 1 1",
                        FontSize = 14
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0.005 0.2",
                        AnchorMax = "0.995 0.8",
                        OffsetMax = "0 0"
                    }
                }
            }); //Report Details Container Box Text


            elements.Add(new CuiElement
            {
                Parent = "XMenu_BannedPlayer_Menu_DisplayPanel",
                Components =
                {
                    new CuiImageComponent {Color = "0.2 0.4 0.6 1"},
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0 0.3975",
                        AnchorMax = "1 0.4025",
                        OffsetMax = "0 0"
                    }
                }
            }); //Splitter Line


            elements.Add(new CuiElement
            {
                Name = "XMenu_BannedPlayer_Menu_DisplayPanel_ReportAuthor_Container",
                Parent = "XMenu_BannedPlayer_Menu_DisplayPanel",
                Components =
                {
                    new CuiImageComponent { Color = "0.1 0.1 0.1 1"},
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0 0.3175", //0.075 Height + Gap
                        AnchorMax = "1 0.3975",
                        OffsetMax = "0 0"
                    }
                }
            }); //Report Author Container Box
            elements.Add(new CuiElement
            {
                Name = "XMenu_BannedPlayer_Menu_DisplayPanel_ReportAuthor_Container_Box",
                Parent = "XMenu_BannedPlayer_Menu_DisplayPanel_ReportAuthor_Container",
                Components =
                {
                    new CuiTextComponent
                    {
                        Text = $"{TextEncodeing(14,"ffffff", "Report Author:")} {TextEncodeing(14,"a5a5a5",Ban_Author.ToLower())}",
                        Align = TextAnchor.MiddleLeft,
                        Color = "1 1 1 1",
                        FontSize = 14
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0.005 0.2",
                        AnchorMax = "0.995 0.8",
                        OffsetMax = "0 0"
                    }
                }
            }); //Report Author Container Box Text


            elements.Add(new CuiElement
            {
                Parent = "XMenu_BannedPlayer_Menu_DisplayPanel",
                Components =
                {
                    new CuiImageComponent {Color = "0.2 0.4 0.6 1"},
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0 0.3125",
                        AnchorMax = "1 0.3175",
                        OffsetMax = "0 0"
                    }
                }
            }); //Splitter Line

            #endregion




            #endregion





            elements.Add(new CuiElement
            {
                Name = "XMenu_BannedPlayer_Menu_DisplayPanel_TotalBans_Container",
                Parent = "XMenu_BannedPlayer_Menu_DisplayPanel",
                Components =
                {
                    new CuiImageComponent { Color = "0.1 0.1 0.1 1"},
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0.005 0.1",
                        AnchorMax = "0.995 0.15",
                        OffsetMax = "0 0"
                    }
                }
            }); //Total Bans Counter
            elements.Add(new CuiElement
            {
                Parent = "XMenu_BannedPlayer_Menu_DisplayPanel_TotalBans_Container",
                Components =
                {
                    new CuiTextComponent
                    {
                        Text = $"Total Bans {PlayerLists.GetTotalBans(cachedPlayerData[player.userID].SelectedTargetID)}",
                        Align = TextAnchor.MiddleCenter,
                        Color = "1 1 1 1",
                        FontSize = 14,
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0 0",
                        AnchorMax = "1 1",
                        OffsetMax = "0 0"
                    }
                }
            }); //Total Bans Counter Text

            #region UNBan Button
            elements.Add(new CuiElement
            {
                Name = "XMenu_BannedPlayer_Menu_DisplayPanel_UNBanButton",
                Parent = "XMenu_BannedPlayer_Menu_DisplayPanel",
                Components =
                {
                    new CuiButtonComponent
                    {
                        Color = "0.2 0.24 0.28 1",
                        Command = $"xmenu.unban.submit {cachedPlayerData[player.userID].SelectedTargetID}",
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0.005 0.005",
                        AnchorMax = "0.995 0.095",
                        OffsetMax = "0 0"
                    }
                }
            });
            elements.Add(new CuiElement
            {
                Parent = "XMenu_BannedPlayer_Menu_DisplayPanel_UNBanButton",
                Components =
                {
                    new CuiTextComponent
                    {
                        Text = "Unban Ban",
                        Align = TextAnchor.MiddleCenter,
                        Color = "1 1 1 1",
                        FontSize = 14
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0 0",
                        AnchorMax = "1 1",
                        OffsetMax = "0 0"
                    }
                }
            });

            #endregion


            #region Bottom Menu Panel
            elements.Add(new CuiElement
            {
                Name = "XMenu_BannedPlayer_Bottom_MenuPanel",
                Parent = "XMenu_BannedPlayer_Menu",
                Components =
                {
                    new CuiImageComponent{Color = "0.1 0.2 0.3 1"},
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0.005 0.005",
                        AnchorMax = "0.995 0.065",
                        OffsetMax = "0 0"
                    }
                }
            }); //Main Bottom Container


            elements.Add(new CuiElement
            {
                Name = "XMenu_BannedPlayer_Bottom_Button_Goback",
                Parent = "XMenu_BannedPlayer_Bottom_MenuPanel",
                Components =
                {
                    new CuiButtonComponent
                    {
                        Color = "0.2 0.3 0.4 1",
                        Command = "xmenu.menu.goback",
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0.35 0.1",
                        AnchorMax = "0.65 0.9",
                        OffsetMax = "0 0"
                    }
                }
            }); //Go back Button
            elements.Add(new CuiElement
            {
                Parent = "XMenu_BannedPlayer_Bottom_Button_Goback",
                Components =
                {
                    new CuiTextComponent
                    {
                        Text = "Go Back",
                        Align = TextAnchor.MiddleCenter,
                        Color = "1 1 1 1",
                        FontSize = 14,
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0 0",
                        AnchorMax = "1 1",
                        OffsetMax = "0 0"
                    }
                }
            }); //Go back Button Text


            #endregion

            CuiHelper.AddUi(player, elements);

        }

        private void DrawBanMenu(BasePlayer player)
        {
            CuiElementContainer elements = new CuiElementContainer();

            elements.Add(new CuiPanel
            {
                RectTransform =
                {
                    AnchorMin = "0.35 0.15",
                    AnchorMax = "0.65 0.85"
                },
                CursorEnabled = true
            }, "Overlay", "XMenu_BanPlayer_Form"); //Main Panel

            #region Top Panel
            elements.Add(new CuiElement
            {
                Name = "XMenu_SelectedPlayer_BanForm_TopPanel",
                Parent = "XMenu_BanPlayer_Form",
                Components =
                {
                    new CuiImageComponent {Color = "0.2 0.4 0.6 1"},
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0 0.925",
                        AnchorMax = "1 1",
                        OffsetMax = "0 0"
                    }
                }
            }); //Top Panel
            elements.Add(new CuiElement
            {
                Name = "XMenu_SelectedPlayer_BanForm_TopPanel_CloseButton",
                Parent = "XMenu_SelectedPlayer_BanForm_TopPanel",
                Components =
                {
                    new CuiButtonComponent
                    {
                        Color = "0.5 0 0 1",
                        Command = "xmenu.menu.destory",
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0.915 0.05",
                        AnchorMax = "0.995 0.95",
                        OffsetMax = "0 0"
                    }
                }
            }); //Top Panel Close Button
            elements.Add(new CuiElement
            {
                Parent = "XMenu_SelectedPlayer_BanForm_TopPanel_CloseButton",
                Components =
                {
                    new CuiTextComponent
                    {
                        Text = "X",
                        Align = UnityEngine.TextAnchor.MiddleCenter,
                        Color = "1 1 1 1",
                        FontSize = 20,
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMax = "1 1",
                        OffsetMax = "0 0"
                    }
                }
            }); //Top Panel Close Button Text

            elements.Add(new CuiElement
            {
                Name = "XMenu_SelectedPlayer_BanForm_TopPanel_Text",
                Parent = "XMenu_SelectedPlayer_BanForm_TopPanel",
                Components =
                {
                    new CuiTextComponent
                    {
                        Text = "Player Administration Menu",
                        Color = "1 1 1 1",
                        Align = UnityEngine.TextAnchor.MiddleLeft,
                        FontSize = 20,
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0.01 0",
                        AnchorMax = "0.9 1",
                        OffsetMax = "0 0"
                    }
                }
            }); //Top Panel Text Display
            #endregion

            #region Sub Top Panel
            elements.Add(new CuiElement
            {
                Name = "XMenu_SelectedPlayer_BanForm_Sub_TopPanel",
                Parent = "XMenu_BanPlayer_Form",
                Components =
                    {
                        new CuiImageComponent { Color = "0.1 0.2 0.3 1"},
                        new CuiRectTransformComponent
                        {
                            AnchorMin = "0.005 0.865",
                            AnchorMax = "0.995 0.925",
                            OffsetMax = "0 0"
                        }
                    }
            }); //Top Search Container Panel

            elements.Add(new CuiElement
            {
                Parent = "XMenu_SelectedPlayer_BanForm_Sub_TopPanel",
                Components =
                {
                    new CuiTextComponent
                    {
                        Text = $"{cachedPlayerData[player.userID].SelectedTargetName} ({cachedPlayerData[player.userID].SelectedTargetID})",
                        Align = TextAnchor.MiddleCenter,
                        Color = "1 1 1 1",
                        FontSize = 16
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0 0",
                        AnchorMax = "1 1",
                        OffsetMax = "0 0"
                    }
                }
            });
            #endregion

            #region Border Lines
            elements.Add(new CuiElement
            {
                Parent = "XMenu_BanPlayer_Form",
                Components =
                {
                    new CuiImageComponent{Color = "0.2 0.4 0.6 1"},
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0 0",
                        AnchorMax = "1 0.005",
                        OffsetMax = "0 0",
                    }
                }

            }); //Bottom Border

            elements.Add(new CuiElement
            {
                Parent = "XMenu_BanPlayer_Form",
                Components =
                {
                    new CuiImageComponent{Color = "0.2 0.4 0.6 1"},
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0 0",
                        AnchorMax = "0.005 0.925",
                        OffsetMax = "0 0",
                    }
                }

            }); //Left Border

            elements.Add(new CuiElement
            {
                Parent = "XMenu_BanPlayer_Form",
                Components =
                {
                    new CuiImageComponent
                    {
                        Color = "0.2 0.4 0.6 1",
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0.995 0",
                        AnchorMax = "1 0.925",
                        OffsetMax = "0 0"
                    }
                }
            }); //Right Border
            #endregion

            elements.Add(new CuiElement
            {
                Name = "XMenu_BanForm_Menu_DisplayPanel",
                Parent = "XMenu_BanPlayer_Form",
                Components =
                {
                    new CuiImageComponent { Color = "0.1 0.1 0.1 1"},
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0.005 0.065",
                        AnchorMax = "0.995 0.865",
                        OffsetMax = "0 0"
                    }
                }
            }); //Main Display Panel Container


            elements.Add(new CuiElement
            {
                Parent = "XMenu_BanForm_Menu_DisplayPanel",
                Components =
                {
                    new CuiImageComponent {Color = "0.2 0.4 0.6 1"},
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0 0.9925",
                        AnchorMax = "1 1",
                        OffsetMax = "0 0"
                    }
                }
            }); //Splitter Line



            #region Input Fields
            elements.Add(new CuiElement
            {
                Name = "XMenu_BanForm_Menu_DisplayPanel_Title_Container",
                Parent = "XMenu_BanForm_Menu_DisplayPanel",
                Components =
                {
                    new CuiImageComponent { Color = "0.1 0.1 0.1 1"},
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0 0.915", //0.075 Height + Gap
                        AnchorMax = "1 0.995",
                        OffsetMax = "0 0"
                    }
                }
            }); //Title Container Box
            elements.Add(new CuiElement
            {
                Name = "XMenu_BanForm_Menu_DisplayPanel_Title_Container_Box",
                Parent = "XMenu_BanForm_Menu_DisplayPanel_Title_Container",
                Components =
                {
                    new CuiTextComponent
                    {
                        Text = "Title",
                        Align = TextAnchor.MiddleLeft,
                        Color = "1 1 1 1",
                        FontSize = 14
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0.005 0.2",
                        AnchorMax = "0.995 0.8",
                        OffsetMax = "0 0"
                    }
                }
            }); //Title Container Box Text

            elements.Add(new CuiElement
            {
                Name = "XMenu_BanForm_Menu_DisplayPanel_Title_Container_InputBox",
                Parent = "XMenu_BanForm_Menu_DisplayPanel_Title_Container",
                Components =
                {
                    new CuiImageComponent { Color = "0.2 0.24 0.28 1"},
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0.2 0.2",
                        AnchorMax = "0.99 0.8",
                        OffsetMax = "0 0"
                    }
                }
            }); //Title Container Input Box
            elements.Add(new CuiElement
            {
                Parent = "XMenu_BanForm_Menu_DisplayPanel_Title_Container_InputBox",
                Components =
                {
                    new CuiInputFieldComponent
                    {
                        Align = TextAnchor.MiddleLeft,
                        Color = "1 1 1 1",
                        FontSize = 14,
                        Command = "xmenu.set.ban.details titleinput",
                        CharsLimit = 64,
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0.005 0",
                        AnchorMax = "0.995 1",
                        OffsetMax = "0 0"
                    }
                }
            }); ; //Title Container Input Field


            elements.Add(new CuiElement
            {
                Parent = "XMenu_BanForm_Menu_DisplayPanel",
                Components =
                {
                    new CuiImageComponent {Color = "0.2 0.4 0.6 1"},
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0 0.91",
                        AnchorMax = "1 0.915",
                        OffsetMax = "0 0"
                    }
                }
            }); //Splitter Line


            elements.Add(new CuiElement
            {
                Name = "XMenu_BanForm_Menu_DisplayPanel_Reason_Container",
                Parent = "XMenu_BanForm_Menu_DisplayPanel",
                Components =
                {
                    new CuiImageComponent { Color = "0.1 0.1 0.1 1"},
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0 0.83", //0.075 Height + Gap
                        AnchorMax = "1 0.91",
                        OffsetMax = "0 0"
                    }
                }
            }); //Reason Container Box
            elements.Add(new CuiElement
            {
                Name = "XMenu_BanForm_Menu_DisplayPanel_Reason_Container_Box",
                Parent = "XMenu_BanForm_Menu_DisplayPanel_Reason_Container",
                Components =
                {
                    new CuiTextComponent
                    {
                        Text = "Reason",
                        Align = TextAnchor.MiddleLeft,
                        Color = "1 1 1 1",
                        FontSize = 14
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0.005 0.2",
                        AnchorMax = "0.995 0.8",
                        OffsetMax = "0 0"
                    }
                }
            }); //Reason Container Box Text

            elements.Add(new CuiElement
            {
                Name = "XMenu_BanForm_Menu_DisplayPanel_Reason_Container_InputBox",
                Parent = "XMenu_BanForm_Menu_DisplayPanel_Reason_Container",
                Components =
                {
                    new CuiImageComponent { Color = "0.2 0.24 0.28 1"},
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0.2 0.2",
                        AnchorMax = "0.99 0.8",
                        OffsetMax = "0 0"
                    }
                }
            }); //Reason Container Input Box
            elements.Add(new CuiElement
            {
                Parent = "XMenu_BanForm_Menu_DisplayPanel_Reason_Container_InputBox",
                Components =
                {
                    new CuiInputFieldComponent
                    {
                        Align = TextAnchor.MiddleLeft,
                        Color = "1 1 1 1",
                        FontSize = 14,
                        Command = "xmenu.set.ban.details reasoninput",
                        CharsLimit = 64,
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0.005 0",
                        AnchorMax = "0.995 1",
                        OffsetMax = "0 0"
                    }
                }
            }); //Reason Container Input Field



            elements.Add(new CuiElement
            {
                Parent = "XMenu_BanForm_Menu_DisplayPanel",
                Components =
                {
                    new CuiImageComponent {Color = "0.2 0.4 0.6 1"},
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0 0.825",
                        AnchorMax = "1 0.83",
                        OffsetMax = "0 0"
                    }
                }
            }); //Splitter Line


            elements.Add(new CuiElement
            {
                Name = "XMenu_BanForm_Menu_DisplayPanel_Duration_Container",
                Parent = "XMenu_BanForm_Menu_DisplayPanel",
                Components =
                {
                    new CuiImageComponent { Color = "0.1 0.1 0.1 1"},
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0 0.7445", //0.075 Height + Gap
                        AnchorMax = "1 0.825",
                        OffsetMax = "0 0"
                    }
                }
            }); //Duration Container Box
            elements.Add(new CuiElement
            {
                Name = "XMenu_BanForm_Menu_DisplayPanel_Duration_Container_Box",
                Parent = "XMenu_BanForm_Menu_DisplayPanel_Duration_Container",
                Components =
                {
                    new CuiTextComponent
                    {
                        Text = "Duration (Hours)",
                        Align = TextAnchor.MiddleLeft,
                        Color = "1 1 1 1",
                        FontSize = 14
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0.005 0.2",
                        AnchorMax = "0.995 0.8",
                        OffsetMax = "0 0"
                    }
                }
            }); //Duration Container Box Text

            elements.Add(new CuiElement
            {
                Name = "XMenu_BanForm_Menu_DisplayPanel_Duration_Container_InputBox",
                Parent = "XMenu_BanForm_Menu_DisplayPanel_Duration_Container",
                Components =
                {
                    new CuiImageComponent { Color = "0.2 0.24 0.28 1"},
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0.2 0.2",
                        AnchorMax = "0.99 0.8",
                        OffsetMax = "0 0"
                    }
                }
            }); //Duration Container Input Box
            elements.Add(new CuiElement
            {
                Parent = "XMenu_BanForm_Menu_DisplayPanel_Duration_Container_InputBox",
                Components =
                {
                    new CuiInputFieldComponent
                    {
                        Align = TextAnchor.MiddleLeft,
                        Color = "1 1 1 1",
                        FontSize = 14,
                        Command = "xmenu.set.ban.details durationinput",
                        CharsLimit = 64,
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0.005 0",
                        AnchorMax = "0.995 1",
                        OffsetMax = "0 0"
                    }
                }
            }); //Duration Container Input Field


            elements.Add(new CuiElement
            {
                Parent = "XMenu_BanForm_Menu_DisplayPanel",
                Components =
                {
                    new CuiImageComponent {Color = "0.2 0.4 0.6 1"},
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0 0.739",
                        AnchorMax = "1 0.744",
                        OffsetMax = "0 0"
                    }
                }
            }); //Splitter Line


            elements.Add(new CuiElement
            {
                Name = "XMenu_BanForm_Menu_DisplayPanel_Evidence_Container",
                Parent = "XMenu_BanForm_Menu_DisplayPanel",
                Components =
                {
                    new CuiImageComponent { Color = "0.1 0.1 0.1 1"},
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0 0.6585", //0.075 Height + Gap
                        AnchorMax = "1 0.7395",
                        OffsetMax = "0 0"
                    }
                }
            }); //Evidence Container Box
            elements.Add(new CuiElement
            {
                Name = "XMenu_BanForm_Menu_DisplayPanel_Evidence_Container_Box",
                Parent = "XMenu_BanForm_Menu_DisplayPanel_Evidence_Container",
                Components =
                {
                    new CuiTextComponent
                    {
                        Text = "Evidence",
                        Align = TextAnchor.MiddleLeft,
                        Color = "1 1 1 1",
                        FontSize = 14
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0.005 0.2",
                        AnchorMax = "0.995 0.8",
                        OffsetMax = "0 0"
                    }
                }
            }); //Evidence Container Box Text

            elements.Add(new CuiElement
            {
                Name = "XMenu_BanForm_Menu_DisplayPanel_Evidence_Container_InputBox",
                Parent = "XMenu_BanForm_Menu_DisplayPanel_Evidence_Container",
                Components =
                {
                    new CuiImageComponent { Color = "0.2 0.24 0.28 1"},
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0.2 0.2",
                        AnchorMax = "0.99 0.8",
                        OffsetMax = "0 0"
                    }
                }
            }); //Evidence Container Input Box
            elements.Add(new CuiElement
            {
                Parent = "XMenu_BanForm_Menu_DisplayPanel_Evidence_Container_InputBox",
                Components =
                {
                    new CuiInputFieldComponent
                    {
                        Align = TextAnchor.MiddleLeft,
                        Color = "1 1 1 1",
                        FontSize = 14,
                        Command = "xmenu.set.ban.details evidenceinput",
                        CharsLimit = 64,
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0.005 0",
                        AnchorMax = "0.995 1",
                        OffsetMax = "0 0"
                    }
                }
            }); //Evidence Container Input Field


            elements.Add(new CuiElement
            {
                Parent = "XMenu_BanForm_Menu_DisplayPanel",
                Components =
                {
                    new CuiImageComponent {Color = "0.2 0.4 0.6 1"},
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0 0.6533",
                        AnchorMax = "1 0.6585",
                        OffsetMax = "0 0"
                    }
                }
            }); //Splitter Line


            elements.Add(new CuiElement
            {
                Name = "XMenu_BanForm_Menu_DisplayPanel_BanAppealable_Container",
                Parent = "XMenu_BanForm_Menu_DisplayPanel",
                Components =
                {
                    new CuiImageComponent { Color = "0.1 0.1 0.1 1"},
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0 0.573", //0.075 Height + Gap
                        AnchorMax = "1 0.654",
                        OffsetMax = "0 0"
                    }
                }
            }); //Ban Appealable Container Box
            elements.Add(new CuiElement
            {
                Name = "XMenu_BanForm_Menu_DisplayPanel_BanAppealable_Container_Box",
                Parent = "XMenu_BanForm_Menu_DisplayPanel_BanAppealable_Container",
                Components =
                {
                    new CuiTextComponent
                    {
                        Text = "Appealable",
                        Align = TextAnchor.MiddleLeft,
                        Color = "1 1 1 1",
                        FontSize = 14
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0.005 0.2",
                        AnchorMax = "0.995 0.8",
                        OffsetMax = "0 0"
                    }
                }
            }); //Ban Appealable Container Box Text

            elements.Add(new CuiElement
            {
                Name = "XMenu_BanForm_Menu_DisplayPanel_BanAppealable_Container_InputBox",
                Parent = "XMenu_BanForm_Menu_DisplayPanel_BanAppealable_Container",
                Components =
                {
                    new CuiImageComponent { Color = "0.2 0.24 0.28 1"},
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0.2 0.2",
                        AnchorMax = "0.99 0.8",
                        OffsetMax = "0 0"
                    }
                }
            }); //Ban Appealable Container Input Box
            elements.Add(new CuiElement
            {
                Parent = "XMenu_BanForm_Menu_DisplayPanel_BanAppealable_Container_InputBox",
                Components =
                {
                    new CuiInputFieldComponent
                    {
                        Align = TextAnchor.MiddleLeft,
                        Color = "1 1 1 1",
                        FontSize = 14,
                        Command = "xmenu.set.ban.details appealinput",
                        CharsLimit = 64,
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0.005 0",
                        AnchorMax = "0.995 1",
                        OffsetMax = "0 0"
                    }
                }
            }); //Ban Appealable Container Input Field


            elements.Add(new CuiElement
            {
                Parent = "XMenu_BanForm_Menu_DisplayPanel",
                Components =
                {
                    new CuiImageComponent {Color = "0.2 0.4 0.6 1"},
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0 0.569",
                        AnchorMax = "1 0.574",
                        OffsetMax = "0 0"
                    }
                }
            }); //Splitter Line


            elements.Add(new CuiElement
            {
                Name = "XMenu_BanForm_Menu_DisplayPanel_ReportDetails_Container",
                Parent = "XMenu_BanForm_Menu_DisplayPanel",
                Components =
                {
                    new CuiImageComponent { Color = "0.1 0.1 0.1 1"},
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0 0.4875", //0.075 Height + Gap
                        AnchorMax = "1 0.568",
                        OffsetMax = "0 0"
                    }
                }
            }); //Report Details Container Box
            elements.Add(new CuiElement
            {
                Name = "XMenu_BanForm_Menu_DisplayPanel_ReportDetails_Container_Box",
                Parent = "XMenu_BanForm_Menu_DisplayPanel_ReportDetails_Container",
                Components =
                {
                    new CuiTextComponent
                    {
                        Text = "Report Details",
                        Align = TextAnchor.MiddleLeft,
                        Color = "1 1 1 1",
                        FontSize = 14
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0.005 0.2",
                        AnchorMax = "0.995 0.8",
                        OffsetMax = "0 0"
                    }
                }
            }); //Report Details Container Box Text

            elements.Add(new CuiElement
            {
                Name = "XMenu_BanForm_Menu_DisplayPanel_ReportDetails_Container_InputBox",
                Parent = "XMenu_BanForm_Menu_DisplayPanel_ReportDetails_Container",
                Components =
                {
                    new CuiImageComponent { Color = "0.2 0.24 0.28 1"},
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0.2 0.2",
                        AnchorMax = "0.99 0.8",
                        OffsetMax = "0 0"
                    }
                }
            }); //Report Details Container Input Box
            elements.Add(new CuiElement
            {
                Parent = "XMenu_BanForm_Menu_DisplayPanel_ReportDetails_Container_InputBox",
                Components =
                {
                    new CuiInputFieldComponent
                    {
                        Align = TextAnchor.MiddleLeft,
                        Color = "1 1 1 1",
                        FontSize = 14,
                        Command = "xmenu.set.ban.details detailsinput",
                        CharsLimit = 64,
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0.005 0",
                        AnchorMax = "0.995 1",
                        OffsetMax = "0 0"
                    }
                }
            }); //Report Details Container Input Field


            elements.Add(new CuiElement
            {
                Parent = "XMenu_BanForm_Menu_DisplayPanel",
                Components =
                {
                    new CuiImageComponent {Color = "0.2 0.4 0.6 1"},
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0 0.4825",
                        AnchorMax = "1 0.4875",
                        OffsetMax = "0 0"
                    }
                }
            }); //Splitter Line

            elements.Add(new CuiElement
            {
                Name = "XMenu_BanForm_Menu_DisplayPanel_ReportAuthor_Container",
                Parent = "XMenu_BanForm_Menu_DisplayPanel",
                Components =
                {
                    new CuiImageComponent { Color = "0.1 0.1 0.1 1"},
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0 0.4025", //0.075 Height + Gap
                        AnchorMax = "1 0.483",
                        OffsetMax = "0 0"
                    }
                }
            }); //Report Author Container Box
            elements.Add(new CuiElement
            {
                Name = "XMenu_BanForm_Menu_DisplayPanel_ReportAuthor_Container_Box",
                Parent = "XMenu_BanForm_Menu_DisplayPanel_ReportAuthor_Container",
                Components =
                {
                    new CuiTextComponent
                    {
                        Text = "Report Author",
                        Align = TextAnchor.MiddleLeft,
                        Color = "1 1 1 1",
                        FontSize = 14
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0.005 0.2",
                        AnchorMax = "0.995 0.8",
                        OffsetMax = "0 0"
                    }
                }
            }); //Report Author Container Box Text

            elements.Add(new CuiElement
            {
                Name = "XMenu_BanForm_Menu_DisplayPanel_ReportAuthor_Container_InputBox",
                Parent = "XMenu_BanForm_Menu_DisplayPanel_ReportAuthor_Container",
                Components =
                {
                    new CuiImageComponent { Color = "0.2 0.24 0.28 1"},
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0.2 0.2",
                        AnchorMax = "0.99 0.8",
                        OffsetMax = "0 0"
                    }
                }
            }); //Report Author Container Input Box
            elements.Add(new CuiElement
            {
                Parent = "XMenu_BanForm_Menu_DisplayPanel_ReportAuthor_Container_InputBox",
                Components =
                {
                    new CuiInputFieldComponent
                    {
                        Align = TextAnchor.MiddleLeft,
                        Color = "1 1 1 1",
                        FontSize = 14,
                        Command = "xmenu.set.ban.details authorinput",
                        CharsLimit = 64,
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0.005 0",
                        AnchorMax = "0.995 1",
                        OffsetMax = "0 0"
                    }
                }
            }); //Report Author Container Input Field

            elements.Add(new CuiElement
            {
                Parent = "XMenu_BanForm_Menu_DisplayPanel",
                Components =
                {
                    new CuiImageComponent {Color = "0.2 0.4 0.6 1"},
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0 0.398",
                        AnchorMax = "1 0.4025",
                        OffsetMax = "0 0"
                    }
                }
            }); //Splitter Line

            #endregion


            #region Ban Button
            elements.Add(new CuiElement
            {
                Name = "XMenu_BanForm_Menu_DisplayPanel_BanButton",
                Parent = "XMenu_BanForm_Menu_DisplayPanel",
                Components =
                {
                    new CuiButtonComponent
                    {
                        Color = "0.2 0.24 0.28 1",
                        Command = $"xmenu.ban.submit {cachedPlayerData[player.userID].SelectedTargetID} {cachedPlayerData[player.userID].SelectedTargetName}",
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0.005 0.005",
                        AnchorMax = "0.995 0.095",
                        OffsetMax = "0 0"
                    }
                }
            });
            elements.Add(new CuiElement
            {
                Parent = "XMenu_BanForm_Menu_DisplayPanel_BanButton",
                Components =
                {
                    new CuiTextComponent
                    {
                        Text = "Submit Ban",
                        Align = TextAnchor.MiddleCenter,
                        Color = "1 1 1 1",
                        FontSize = 14
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0 0",
                        AnchorMax = "1 1",
                        OffsetMax = "0 0"
                    }
                }
            });

            #endregion

            elements.Add(new CuiElement
            {
                Name = "XMenu_BanForm_Menu_DisplayPanel_TotalBans_Container",
                Parent = "XMenu_BanForm_Menu_DisplayPanel",
                Components =
                {
                    new CuiImageComponent { Color = "0.1 0.1 0.1 1"},
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0.005 0.1",
                        AnchorMax = "0.995 0.15",
                        OffsetMax = "0 0"
                    }
                }
            });
            elements.Add(new CuiElement
            {
                Parent = "XMenu_BanForm_Menu_DisplayPanel_TotalBans_Container",
                Components =
                {
                    new CuiTextComponent
                    {
                        Text = $"Total Bans {PlayerLists.GetTotalBans(cachedPlayerData[player.userID].SelectedTargetID)}",
                        Align = TextAnchor.MiddleCenter,
                        Color = "1 1 1 1",
                        FontSize = 14,
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0 0",
                        AnchorMax = "1 1",
                        OffsetMax = "0 0"
                    }
                }
            });

            #region Bottom Menu Panel
            elements.Add(new CuiElement
            {
                Name = "XMenu_BanForm_Bottom_MenuPanel",
                Parent = "XMenu_BanPlayer_Form",
                Components =
                {
                    new CuiImageComponent{Color = "0.1 0.2 0.3 1"},
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0.005 0.005",
                        AnchorMax = "0.995 0.065",
                        OffsetMax = "0 0"
                    }
                }
            }); //Main Bottom Container


            elements.Add(new CuiElement
            {
                Name = "XMenu_BanForm_Bottom_Button_Goback",
                Parent = "XMenu_BanForm_Bottom_MenuPanel",
                Components =
                {
                    new CuiButtonComponent
                    {
                        Color = "0.2 0.3 0.4 1",
                        Command = "xmenu.menu.goback",
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0.35 0.1",
                        AnchorMax = "0.65 0.9",
                        OffsetMax = "0 0"
                    }
                }
            }); //Go back Button
            elements.Add(new CuiElement
            {
                Parent = "XMenu_BanForm_Bottom_Button_Goback",
                Components =
                {
                    new CuiTextComponent
                    {
                        Text = "Go Back",
                        Align = TextAnchor.MiddleCenter,
                        Color = "1 1 1 1",
                        FontSize = 14,
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0 0",
                        AnchorMax = "1 1",
                        OffsetMax = "0 0"
                    }
                }
            }); //Go back Button Text


            #endregion

            CuiHelper.AddUi(player, elements);
        }


        private void DrawReportMenu(BasePlayer player)
        {
            CuiElementContainer elements = new CuiElementContainer();


            elements.Add(new CuiPanel
            {
                RectTransform =
                {
                    AnchorMin = "0.35 0.15",
                    AnchorMax = "0.65 0.85"
                },
                CursorEnabled = true
            }, "Overlay", "XMenu_ReportPlayer_Form"); //Main Panel

            #region Top Panel
            elements.Add(new CuiElement
            {
                Name = "XMenu_SelectedPlayer_ReportForm_TopPanel",
                Parent = "XMenu_ReportPlayer_Form",
                Components =
                {
                    new CuiImageComponent {Color = "0.2 0.4 0.6 1"},
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0 0.925",
                        AnchorMax = "1 1",
                        OffsetMax = "0 0"
                    }
                }
            }); //Top Panel
            elements.Add(new CuiElement
            {
                Name = "XMenu_SelectedPlayer_ReportForm_TopPanel_CloseButton",
                Parent = "XMenu_SelectedPlayer_ReportForm_TopPanel",
                Components =
                {
                    new CuiButtonComponent
                    {
                        Color = "0.5 0 0 1",
                        Command = "xmenu.menu.destory",
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0.915 0.05",
                        AnchorMax = "0.995 0.95",
                        OffsetMax = "0 0"
                    }
                }
            }); //Top Panel Close Button
            elements.Add(new CuiElement
            {
                Parent = "XMenu_SelectedPlayer_ReportForm_TopPanel_CloseButton",
                Components =
                {
                    new CuiTextComponent
                    {
                        Text = "X",
                        Align = UnityEngine.TextAnchor.MiddleCenter,
                        Color = "1 1 1 1",
                        FontSize = 20,
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMax = "1 1",
                        OffsetMax = "0 0"
                    }
                }
            }); //Top Panel Close Button Text

            elements.Add(new CuiElement
            {
                Name = "XMenu_SelectedPlayer_ReportForm_TopPanel_Text",
                Parent = "XMenu_SelectedPlayer_ReportForm_TopPanel",
                Components =
                {
                    new CuiTextComponent
                    {
                        Text = "Player Administration Menu",
                        Color = "1 1 1 1",
                        Align = UnityEngine.TextAnchor.MiddleLeft,
                        FontSize = 20,
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0.01 0",
                        AnchorMax = "0.9 1",
                        OffsetMax = "0 0"
                    }
                }
            }); //Top Panel Text Display
            #endregion

            #region Sub Top Panel
            elements.Add(new CuiElement
            {
                Name = "XMenu_SelectedPlayer_ReportForm_Sub_TopPanel",
                Parent = "XMenu_ReportPlayer_Form",
                Components =
                    {
                        new CuiImageComponent { Color = "0.1 0.2 0.3 1"},
                        new CuiRectTransformComponent
                        {
                            AnchorMin = "0.005 0.865",
                            AnchorMax = "0.995 0.925",
                            OffsetMax = "0 0"
                        }
                    }
            }); //Top Search Container Panel

            elements.Add(new CuiElement
            {
                Parent = "XMenu_SelectedPlayer_ReportForm_Sub_TopPanel",
                Components =
                {
                    new CuiTextComponent
                    {
                        Text = $"{cachedPlayerData[player.userID].SelectedTargetName} ({cachedPlayerData[player.userID].SelectedTargetID})",
                        Align = TextAnchor.MiddleCenter,
                        Color = "1 1 1 1",
                        FontSize = 16
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0 0",
                        AnchorMax = "1 1",
                        OffsetMax = "0 0"
                    }
                }
            });
            #endregion

            #region Border Lines
            elements.Add(new CuiElement
            {
                Parent = "XMenu_ReportPlayer_Form",
                Components =
                {
                    new CuiImageComponent{Color = "0.2 0.4 0.6 1"},
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0 0",
                        AnchorMax = "1 0.005",
                        OffsetMax = "0 0",
                    }
                }

            }); //Bottom Border

            elements.Add(new CuiElement
            {
                Parent = "XMenu_ReportPlayer_Form",
                Components =
                {
                    new CuiImageComponent{Color = "0.2 0.4 0.6 1"},
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0 0",
                        AnchorMax = "0.005 0.925",
                        OffsetMax = "0 0",
                    }
                }

            }); //Left Border

            elements.Add(new CuiElement
            {
                Parent = "XMenu_ReportPlayer_Form",
                Components =
                {
                    new CuiImageComponent
                    {
                        Color = "0.2 0.4 0.6 1",
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0.995 0",
                        AnchorMax = "1 0.925",
                        OffsetMax = "0 0"
                    }
                }
            }); //Right Border
            #endregion

            elements.Add(new CuiElement
            {
                Name = "XMenu_ReportForm_Menu_DisplayPanel",
                Parent = "XMenu_ReportPlayer_Form",
                Components =
                {
                    new CuiImageComponent { Color = "0.1 0.1 0.1 1"},
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0.005 0.065",
                        AnchorMax = "0.995 0.865",
                        OffsetMax = "0 0"
                    }
                }
            }); //Main Display Panel Container

            elements.Add(new CuiElement
            {
                Parent = "XMenu_ReportForm_Menu_DisplayPanel",
                Components =
                {
                    new CuiImageComponent {Color = "0.2 0.4 0.6 1"},
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0 0.9925",
                        AnchorMax = "1 1",
                        OffsetMax = "0 0"
                    }
                }
            }); //Splitter Line


            #region Input Fields
            elements.Add(new CuiElement
            {
                Name = "XMenu_ReportForm_Menu_DisplayPanel_Title_Container",
                Parent = "XMenu_ReportForm_Menu_DisplayPanel",
                Components =
                {
                    new CuiImageComponent { Color = "0.1 0.1 0.1 1"},
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0 0.915", //0.075 Height + Gap
                        AnchorMax = "1 0.995",
                        OffsetMax = "0 0"
                    }
                }
            }); //Title Container Box
            elements.Add(new CuiElement
            {
                Name = "XMenu_ReportForm_Menu_DisplayPanel_Title_Container_Box",
                Parent = "XMenu_ReportForm_Menu_DisplayPanel_Title_Container",
                Components =
                {
                    new CuiTextComponent
                    {
                        Text = "Report Title",
                        Align = TextAnchor.MiddleLeft,
                        Color = "1 1 1 1",
                        FontSize = 14
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0.005 0.2",
                        AnchorMax = "0.995 0.8",
                        OffsetMax = "0 0"
                    }
                }
            }); //Title Container Box Text

            elements.Add(new CuiElement
            {
                Name = "XMenu_ReportForm_Menu_DisplayPanel_Title_Container_InputBox",
                Parent = "XMenu_ReportForm_Menu_DisplayPanel_Title_Container",
                Components =
                {
                    new CuiImageComponent { Color = "0.2 0.24 0.28 1"},
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0.2 0.2",
                        AnchorMax = "0.99 0.8",
                        OffsetMax = "0 0"
                    }
                }
            }); //Title Container Input Box
            elements.Add(new CuiElement
            {
                Parent = "XMenu_ReportForm_Menu_DisplayPanel_Title_Container_InputBox",
                Components =
                {
                    new CuiInputFieldComponent
                    {
                        Align = TextAnchor.MiddleLeft,
                        Color = "1 1 1 1",
                        FontSize = 14,
                        Command = "xmenu.set.report.details titleinput",
                        CharsLimit = 64,
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0.005 0",
                        AnchorMax = "0.995 1",
                        OffsetMax = "0 0"
                    }
                }
            }); //Title Container Input Field

            elements.Add(new CuiElement
            {
                Parent = "XMenu_ReportForm_Menu_DisplayPanel",
                Components =
                {
                    new CuiImageComponent {Color = "0.2 0.4 0.6 1"},
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0 0.91",
                        AnchorMax = "1 0.915",
                        OffsetMax = "0 0"
                    }
                }
            }); //Splitter Line


            elements.Add(new CuiElement
            {
                Name = "XMenu_ReportForm_Menu_DisplayPanel_Reason_Container",
                Parent = "XMenu_ReportForm_Menu_DisplayPanel",
                Components =
                {
                    new CuiImageComponent { Color = "0.1 0.1 0.1 1"},
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0 0.83", //0.075 Height + Gap
                        AnchorMax = "1 0.91",
                        OffsetMax = "0 0"
                    }
                }
            }); //Reason Container Box
            elements.Add(new CuiElement
            {
                Name = "XMenu_ReportForm_Menu_DisplayPanel_Reason_Container_Box",
                Parent = "XMenu_ReportForm_Menu_DisplayPanel_Reason_Container",
                Components =
                {
                    new CuiTextComponent
                    {
                        Text = "Report Reason",
                        Align = TextAnchor.MiddleLeft,
                        Color = "1 1 1 1",
                        FontSize = 14
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0.005 0.2",
                        AnchorMax = "0.995 0.8",
                        OffsetMax = "0 0"
                    }
                }
            }); //Reason Container Box Text

            elements.Add(new CuiElement
            {
                Name = "XMenu_ReportForm_Menu_DisplayPanel_Reason_Container_InputBox",
                Parent = "XMenu_ReportForm_Menu_DisplayPanel_Reason_Container",
                Components =
                {
                    new CuiImageComponent { Color = "0.2 0.24 0.28 1"},
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0.2 0.2",
                        AnchorMax = "0.99 0.8",
                        OffsetMax = "0 0"
                    }
                }
            }); //Reason Container Input Box
            elements.Add(new CuiElement
            {
                Parent = "XMenu_ReportForm_Menu_DisplayPanel_Reason_Container_InputBox",
                Components =
                {
                    new CuiInputFieldComponent
                    {
                        Align = TextAnchor.MiddleLeft,
                        Color = "1 1 1 1",
                        FontSize = 14,
                        Command = "xmenu.set.report.details reasoninput",
                        CharsLimit = 64,
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0.005 0",
                        AnchorMax = "0.995 1",
                        OffsetMax = "0 0"
                    }
                }
            }); //Reason Container Input Field

            elements.Add(new CuiElement
            {
                Parent = "XMenu_ReportForm_Menu_DisplayPanel",
                Components =
                {
                    new CuiImageComponent {Color = "0.2 0.4 0.6 1"},
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0 0.825",
                        AnchorMax = "1 0.83",
                        OffsetMax = "0 0"
                    }
                }
            }); //Splitter Line


            elements.Add(new CuiElement
            {
                Name = "XMenu_ReportForm_Menu_DisplayPanel_Duration_Container",
                Parent = "XMenu_ReportForm_Menu_DisplayPanel",
                Components =
                {
                    new CuiImageComponent { Color = "0.1 0.1 0.1 1"},
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0 0.7445", //0.075 Height + Gap
                        AnchorMax = "1 0.825",
                        OffsetMax = "0 0"
                    }
                }
            }); //Evidence Container Box
            elements.Add(new CuiElement
            {
                Name = "XMenu_ReportForm_Menu_DisplayPanel_Duration_Container_Box",
                Parent = "XMenu_ReportForm_Menu_DisplayPanel_Duration_Container",
                Components =
                {
                    new CuiTextComponent
                    {
                        Text = "Report Evidence",
                        Align = TextAnchor.MiddleLeft,
                        Color = "1 1 1 1",
                        FontSize = 14
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0.005 0.2",
                        AnchorMax = "0.995 0.8",
                        OffsetMax = "0 0"
                    }
                }
            }); //Evidence Container Box Text

            elements.Add(new CuiElement
            {
                Name = "XMenu_ReportForm_Menu_DisplayPanel_Duration_Container_InputBox",
                Parent = "XMenu_ReportForm_Menu_DisplayPanel_Duration_Container",
                Components =
                {
                    new CuiImageComponent { Color = "0.2 0.24 0.28 1"},
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0.2 0.2",
                        AnchorMax = "0.99 0.8",
                        OffsetMax = "0 0"
                    }
                }
            }); //Evidence Container Input Box
            elements.Add(new CuiElement
            {
                Parent = "XMenu_ReportForm_Menu_DisplayPanel_Duration_Container_InputBox",
                Components =
                {
                    new CuiInputFieldComponent
                    {
                        Align = TextAnchor.MiddleLeft,
                        Color = "1 1 1 1",
                        FontSize = 14,
                        Command = "xmenu.set.report.details evidenceinput",
                        CharsLimit = 64,
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0.005 0",
                        AnchorMax = "0.995 1",
                        OffsetMax = "0 0"
                    }
                }
            }); //Evidence Container Input Field

            elements.Add(new CuiElement
            {
                Parent = "XMenu_ReportForm_Menu_DisplayPanel",
                Components =
                {
                    new CuiImageComponent {Color = "0.2 0.4 0.6 1"},
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0 0.739",
                        AnchorMax = "1 0.744",
                        OffsetMax = "0 0"
                    }
                }
            }); //Splitter Line


            elements.Add(new CuiElement
            {
                Name = "XMenu_ReportForm_Menu_DisplayPanel_Evidence_Container",
                Parent = "XMenu_ReportForm_Menu_DisplayPanel",
                Components =
                {
                    new CuiImageComponent { Color = "0.1 0.1 0.1 1"},
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0 0.6585", //0.075 Height + Gap
                        AnchorMax = "1 0.7395",
                        OffsetMax = "0 0"
                    }
                }
            }); //Details Container Box
            elements.Add(new CuiElement
            {
                Name = "XMenu_ReportForm_Menu_DisplayPanel_Evidence_Container_Box",
                Parent = "XMenu_ReportForm_Menu_DisplayPanel_Evidence_Container",
                Components =
                {
                    new CuiTextComponent
                    {
                        Text = "Report Details",
                        Align = TextAnchor.MiddleLeft,
                        Color = "1 1 1 1",
                        FontSize = 14
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0.005 0.2",
                        AnchorMax = "0.995 0.8",
                        OffsetMax = "0 0"
                    }
                }
            }); //Details Container Box Text

            elements.Add(new CuiElement
            {
                Name = "XMenu_ReportForm_Menu_DisplayPanel_Evidence_Container_InputBox",
                Parent = "XMenu_ReportForm_Menu_DisplayPanel_Evidence_Container",
                Components =
                {
                    new CuiImageComponent { Color = "0.2 0.24 0.28 1"},
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0.2 0.2",
                        AnchorMax = "0.99 0.8",
                        OffsetMax = "0 0"
                    }
                }
            }); //Details Container Input Box
            elements.Add(new CuiElement
            {
                Parent = "XMenu_ReportForm_Menu_DisplayPanel_Evidence_Container_InputBox",
                Components =
                {
                    new CuiInputFieldComponent
                    {
                        Align = TextAnchor.MiddleLeft,
                        Color = "1 1 1 1",
                        FontSize = 14,
                        Command = "xmenu.set.report.details detailsinput",
                        CharsLimit = 64,
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0.005 0",
                        AnchorMax = "0.995 1",
                        OffsetMax = "0 0"
                    }
                }
            }); //Details Container Input Field


            elements.Add(new CuiElement
            {
                Parent = "XMenu_ReportForm_Menu_DisplayPanel",
                Components =
                {
                    new CuiImageComponent {Color = "0.2 0.4 0.6 1"},
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0 0.6533",
                        AnchorMax = "1 0.6585",
                        OffsetMax = "0 0"
                    }
                }
            }); //Splitter Line
            #endregion


            #region Report Button
            elements.Add(new CuiElement
            {
                Name = "XMenu_ReportForm_Menu_DisplayPanel_BanButton",
                Parent = "XMenu_ReportForm_Menu_DisplayPanel",
                Components =
                {
                    new CuiButtonComponent
                    {
                        Color = "0.2 0.24 0.28 1",
                        Command = $"xmenu.report.submit {cachedPlayerData[player.userID].SelectedTargetID} {cachedPlayerData[player.userID].SelectedTargetName}",
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0.005 0.005",
                        AnchorMax = "0.995 0.095",
                        OffsetMax = "0 0"
                    }
                }
            });
            elements.Add(new CuiElement
            {
                Parent = "XMenu_ReportForm_Menu_DisplayPanel_BanButton",
                Components =
                {
                    new CuiTextComponent
                    {
                        Text = "Submit Report",
                        Align = TextAnchor.MiddleCenter,
                        Color = "1 1 1 1",
                        FontSize = 14
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0 0",
                        AnchorMax = "1 1",
                        OffsetMax = "0 0"
                    }
                }
            });

            #endregion

            #region Bottom Menu Panel
            elements.Add(new CuiElement
            {
                Name = "XMenu_ReportForm_Bottom_MenuPanel",
                Parent = "XMenu_ReportPlayer_Form",
                Components =
                {
                    new CuiImageComponent{Color = "0.1 0.2 0.3 1"},
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0.005 0.005",
                        AnchorMax = "0.995 0.065",
                        OffsetMax = "0 0"
                    }
                }
            }); //Main Bottom Container


            elements.Add(new CuiElement
            {
                Name = "XMenu_ReportForm_Bottom_Button_Goback",
                Parent = "XMenu_ReportForm_Bottom_MenuPanel",
                Components =
                {
                    new CuiButtonComponent
                    {
                        Color = "0.2 0.3 0.4 1",
                        Command = "xmenu.menu.goback",
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0.35 0.1",
                        AnchorMax = "0.65 0.9",
                        OffsetMax = "0 0"
                    }
                }
            }); //Go back Button
            elements.Add(new CuiElement
            {
                Parent = "XMenu_ReportForm_Bottom_Button_Goback",
                Components =
                {
                    new CuiTextComponent
                    {
                        Text = "Go Back",
                        Align = TextAnchor.MiddleCenter,
                        Color = "1 1 1 1",
                        FontSize = 14,
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0 0",
                        AnchorMax = "1 1",
                        OffsetMax = "0 0"
                    }
                }
            }); //Go back Button Text


            #endregion
            CuiHelper.AddUi(player, elements);
        }

        #endregion


        #region Toggle Player Movement

        private void ToggleFreezeTimer(BasePlayer player)
        {
            PlayerData.LoadCachedData(player.userID); //Load Data


            cachedPlayerData[player.userID].playerPos = player.transform.position;

            cachedPlayerData[player.userID].FreezeTimer = timer.Every(0.1f, () =>
            {
                if (cachedPlayerData[player.userID].MenuMainOpen || cachedPlayerData[player.userID].MenuOptionsOpen)
                {
                    player.Teleport(cachedPlayerData[player.userID].playerPos);
                }
                else
                {
                    cachedPlayerData[player.userID].FreezeTimer.Destroy();
                }
            });
        }
        #endregion


        #region Genral Functions
        private bool PlayerBanned(ulong id)
        {
            if (PlayerLists.GetBannedListSimple().ContainsKey(id)) { return true; }
            return false;



        }

        private bool PlayerReported(ulong id)
        {
            if (PlayerLists.GetReportedListSimple().ContainsKey(id)) { return true; }
            return false;
        }
        string TextEncodeing(double TextSize, string HexColor, string PlainText) => $"<size={TextSize}><color=#{HexColor}>{PlainText}</color></size>";
        #endregion

        #region Search Function
        [ConsoleCommand("xmenu.search.list")]
        private void ConsoleSystemGetArg(ConsoleSystem.Arg arg)
        {
            if (arg.HasArgs(1))
            {
                PlayerData.LoadCachedData(arg.Player().userID); //Load Data

                //Reset Lists

                if (cachedPlayerData[arg.Player().userID].ActiveListName == "Online")
                {
                    cachedPlayerData[arg.Player().userID].ActiveList = PlayerLists.GetOnlineList();
                    cachedPlayerData[arg.Player().userID].SearchOverRide = 1;
                }
                if (cachedPlayerData[arg.Player().userID].ActiveListName == "Disk")
                {
                    cachedPlayerData[arg.Player().userID].ActiveList = PlayerLists.GetDiskList();
                    cachedPlayerData[arg.Player().userID].SearchOverRide = 2;


                }
                if (cachedPlayerData[arg.Player().userID].ActiveListName == "Banned")
                {
                    cachedPlayerData[arg.Player().userID].ActiveList = PlayerLists.GetBannedList();
                    cachedPlayerData[arg.Player().userID].SearchOverRide = 3;
                }

                var ResultList = new SortedList<ulong, string>();

                //Search
                foreach (var i in SearchFunction.searchList(cachedPlayerData[arg.Player().userID].ActiveList, arg.Args[0], arg.Player()))
                {
                    ResultList.Add(i.Key, i.Value);
                }


                cachedPlayerData[arg.Player().userID].ActiveList = ResultList;
                DrawMenu(arg.Player(), ResultList, true, 0);




            }
            else
            {
                PlayerData.LoadCachedData(arg.Player().userID); //Load Data
                                                                //Reset Lists

                if (cachedPlayerData[arg.Player().userID].ActiveListName == "Online")
                {
                    cachedPlayerData[arg.Player().userID].ActiveList = PlayerLists.GetOnlineList();
                    cachedPlayerData[arg.Player().userID].SearchOverRide = 1;
                }
                if (cachedPlayerData[arg.Player().userID].ActiveListName == "Disk")
                {
                    cachedPlayerData[arg.Player().userID].ActiveList = PlayerLists.GetDiskList();
                    cachedPlayerData[arg.Player().userID].SearchOverRide = 2;


                }
                if (cachedPlayerData[arg.Player().userID].ActiveListName == "Banned")
                {
                    cachedPlayerData[arg.Player().userID].ActiveList = PlayerLists.GetBannedList();
                    cachedPlayerData[arg.Player().userID].SearchOverRide = 3;
                }

                DrawMenu(arg.Player(), cachedPlayerData[arg.Player().userID].ActiveList, true, 0);
            }
        }


        public class SearchFunction
        {
            public static SortedList<ulong, string> searchList(SortedList<ulong, string> ListToSearch, string searchValue, BasePlayer player)
            {
                SortedList<ulong, string> ResultList = new SortedList<ulong, string>();
                if (ListToSearch.Keys.Count >= 1 && ListToSearch.Values.Count >= 1 && searchValue != string.Empty)
                {

                    foreach (var i in ListToSearch)
                    {
                        if (i.ToString().Contains(searchValue.ToLower()))
                        {
                            ResultList.Add(Convert.ToUInt64(i.Key), i.Value);
                        }
                    }
                }
                else //Empty List
                {
                    ResultList.Clear();
                }
                return ResultList;
            }
        }
        #endregion


        #region UI Console Commands

        #region Report
        [ConsoleCommand("xmenu.set.report.details")]//Set Report Data
        private void SetReportDetailsConsoleCommand(ConsoleSystem.Arg arg)
        {
            PlayerData.LoadCachedData(arg.Player().userID); //Try Load Data

            string inputValue = string.Empty;
            var list = cachedPlayerData[arg.Player().userID].SelectedReportTargetPlayerInfoInput_Args;
            for (int i = 1; i < arg.Args.Length; i++)
            {
                if (i > 1) { inputValue += $".{arg.Args[i]}"; }
                else { inputValue += $"{arg.Args[i]}"; }
            }

            if (arg.Args[0].ToLower() == "titleinput")
            {
                int listIndex = 0;
                if (!String.IsNullOrEmpty(inputValue))
                {
                    list[listIndex] = inputValue;
                }
                else { list[listIndex] = "NULL"; }

            } //Title Input

            if (arg.Args[0].ToLower() == "reasoninput")
            {
                int listIndex = 1;
                if (!String.IsNullOrEmpty(inputValue))
                {
                    list[listIndex] = inputValue;
                }
                else { list[listIndex] = "NULL"; }

            } //Reason Input

            if (arg.Args[0].ToLower() == "evidenceinput")
            {
                int listIndex = 2;
                if (!String.IsNullOrEmpty(inputValue))
                {
                    list[listIndex] = inputValue;
                }
                else { list[listIndex] = "NULL"; }

            } //Duration Input

            if (arg.Args[0].ToLower() == "detailsinput")
            {
                int listIndex = 3;
                if (!String.IsNullOrEmpty(inputValue))
                {
                    list[listIndex] = inputValue;
                }
                else { list[listIndex] = "NULL"; }

            } //Details Input
        }

        [ConsoleCommand("xmenu.report.submit")] //Submit Report On Player
        private void SubmitReportConsoleCommand(ConsoleSystem.Arg arg)
        {
            PlayerData.LoadCachedData(arg.Player().userID); //Try Load Data
            if (!PlayerReported(Convert.ToUInt64(arg.Args[0])))
            {
                var list = cachedPlayerData[arg.Player().userID].SelectedReportTargetPlayerInfoInput_Args;
                if (!string.IsNullOrEmpty(arg.Args[0]) && !string.IsNullOrEmpty(arg.Args[1]) && !list.Contains("NULL"))
                {
                    ulong TargetID = Convert.ToUInt64(arg.Args[0]);
                    string TargetName = arg.Args[1];

                    //Input Args
                    string title = list[0];
                    string reason = list[1];
                    string evidence = list[2];
                    string details = list[3];
                    string author = arg.Player().displayName;
                    DrawPopupMessage(arg.Player(), "XMenu_ReportPlayer_Form", "Report submitted", true, 30);


                    PlayerLists.AddReportToList(TargetID, $"{TargetName.Replace(" ", ".").ToLower()}|{title.Replace(" ", ".").ToLower()}|{reason.Replace(" ", ".").ToLower()}|{evidence.Replace(" ", ".").ToLower()}|{details.Replace(" ", ".").ToLower()}|{author.Replace(" ", ".").ToLower()}");
                }
                else
                {
                    DrawPopupMessage(arg.Player(), "XMenu_ReportPlayer_Form", "Args not valid or empty", false, 30);
                }
            }
            else
            {
                DrawPopupMessage(arg.Player(), "XMenu_ReportPlayer_Form", "Player Already Reported", false, 30);
            }


        }
        #endregion

        #region Set/Ban/Unban

        [ConsoleCommand("xmenu.set.ban.details")] //Set Ban Data
        private void SetBanDetailsConsoleCommand(ConsoleSystem.Arg arg)
        {
            PlayerData.LoadCachedData(arg.Player().userID); //Try Load Data

            string inputValue = string.Empty;
            var list = cachedPlayerData[arg.Player().userID].SelectedBannedTargetPlayerInfoInput_Args;
            for (int i = 1; i < arg.Args.Length; i++)
            {
                if (i > 1) { inputValue += $".{arg.Args[i]}"; }
                else { inputValue += $"{arg.Args[i]}"; }
            }




            if (arg.Args[0].ToLower() == "titleinput")
            {
                int listIndex = 0;
                if (!String.IsNullOrEmpty(inputValue))
                {
                    list[listIndex] = inputValue;
                }
                else { list[listIndex] = "NULL"; }

            } //Title Input

            if (arg.Args[0].ToLower() == "reasoninput")
            {
                int listIndex = 1;
                if (!String.IsNullOrEmpty(inputValue))
                {
                    list[listIndex] = inputValue;
                }
                else { list[listIndex] = "NULL"; }

            } //Reason Input

            if (arg.Args[0].ToLower() == "durationinput")
            {
                int listIndex = 2;
                if (!String.IsNullOrEmpty(inputValue))
                {
                    list[listIndex] = inputValue;
                }
                else { list[listIndex] = "NULL"; }

            } //Duration Input

            if (arg.Args[0].ToLower() == "evidenceinput")
            {
                int listIndex = 3;
                if (!String.IsNullOrEmpty(inputValue))
                {
                    list[listIndex] = inputValue;
                }
                else { list[listIndex] = "NULL"; }

            } //Evidence Input

            if (arg.Args[0].ToLower() == "appealinput")
            {
                int listIndex = 4;
                if (!String.IsNullOrEmpty(inputValue))
                {
                    list[listIndex] = inputValue;
                }
                else { list[listIndex] = "NULL"; }

            } //Appeal Input

            if (arg.Args[0].ToLower() == "detailsinput")
            {
                int listIndex = 5;
                if (!String.IsNullOrEmpty(inputValue))
                {
                    list[listIndex] = inputValue;
                }
                else { list[listIndex] = "NULL"; }

            } //Details Input

            if (arg.Args[0].ToLower() == "authorinput")
            {
                int listIndex = 6;
                if (!String.IsNullOrEmpty(inputValue))
                {
                    list[listIndex] = inputValue;
                }
                else { list[listIndex] = "NULL"; }

            } //Author Input
        }



        [ConsoleCommand("xmenu.ban.submit")] //Submit Ban On Player
        private void SubmitBanConsoleCommand(ConsoleSystem.Arg arg)
        {

            PlayerData.LoadCachedData(arg.Player().userID); //Try Load Data
            var list = cachedPlayerData[arg.Player().userID].SelectedBannedTargetPlayerInfoInput_Args;

            if (!string.IsNullOrEmpty(arg.Args[0]) && !string.IsNullOrEmpty(arg.Args[1]) && !list.Contains("NULL"))
            {
                ulong TargetID = Convert.ToUInt64(arg.Args[0]);
                string TargetName = arg.Args[1];

                //Input Args
                string title = list[0];
                string reason = list[1];
                string duration = list[2];
                string evidence = list[3];
                string appealable = list[4];
                string details = list[5];
                string author = list[6];


                string duration_Text = duration;

                string TargetArgs = $"{title}|{reason}|{duration}|{evidence}|{appealable}|{details}|{author}";

                string reasonFilter = reason.Replace(".", " ");


                string Ban_Duration_Format = "";

                if (duration_Text.All(char.IsDigit) && duration_Text != "-1")
                {
                    int valueToCheck = Convert.ToInt32(duration_Text);

                    if (valueToCheck < 24)
                    {
                        Ban_Duration_Format = " hour(s)";
                    }
                    else if (valueToCheck >= 24 && valueToCheck <= 168)
                    {
                        double value = Convert.ToDouble(duration_Text) / 24;
                        duration_Text = value.ToString("0.0");

                        Ban_Duration_Format = " day(s)";
                    }
                    else if (valueToCheck >= 168)
                    {
                        double value = Convert.ToDouble(duration_Text) / 168;
                        duration_Text = value.ToString("0.0");
                        Ban_Duration_Format = " week(s)";
                    }

                }



                if (duration == "-1" && TargetID.ToString().Contains("765")) //Permanent
                {
                    arg.Player().SendConsoleCommand($"banid {TargetID} {TargetName} {TargetArgs} -1");
                    duration_Text = "Permanently";
                    PlayerLists.UpdateBannedCount(TargetID);

                    // Ban Message Broadcast
                    PrintToChat($"{TextEncodeing(16, "c83232", "Server")}{TextEncodeing(14, "ffffff", ":")} {TextEncodeing(14, "3296fa", TargetName)} {TextEncodeing(14, "ffffff", "has been banned")} {TextEncodeing(14, "3296fa", duration_Text)}{TextEncodeing(14, "ffffff", $"{Ban_Duration_Format}")} {TextEncodeing(14, "ffffff", $"for")} {TextEncodeing(14, "3296fa", reasonFilter)}");

                    DrawPopupMessage(arg.Player(), "XMenu_BanPlayer_Form", "Ban submitted", true, 30);
                }
                else if (TargetID.ToString().Contains("765")) //Timed
                {
                    arg.Player().SendConsoleCommand($"banid {TargetID} {TargetName} {TargetArgs} {duration}");
                    PlayerLists.UpdateBannedCount(TargetID);

                    // Ban Message Broadcast
                    PrintToChat($"{TextEncodeing(16, "c83232", "Server")}{TextEncodeing(14, "ffffff", ":")} {TextEncodeing(14, "3296fa", TargetName)} {TextEncodeing(14, "ffffff", "has been banned")} {TextEncodeing(14, "3296fa", duration_Text)}{TextEncodeing(14, "ffffff", $"{Ban_Duration_Format}")} {TextEncodeing(14, "ffffff", $"for")} {TextEncodeing(14, "3296fa", reasonFilter)}");

                    DrawPopupMessage(arg.Player(), "XMenu_BanPlayer_Form", "Ban submitted", true, 30);
                }
                else //Error
                {
                    DrawPopupMessage(arg.Player(), "XMenu_BanPlayer_Form", "Couldn't execute the ban\nCheck for valid steamid", false, 20);
                }

            }
            else
            {
                DrawPopupMessage(arg.Player(), "XMenu_BanPlayer_Form", "Args not valid or empty", false, 30);
            }


        }

        [ConsoleCommand("xmenu.unban.submit")] //Submit Ban On Player
        private void UnbanConsoleCommand(ConsoleSystem.Arg arg)
        {
            PlayerData.LoadCachedData(arg.Player().userID); //Try Load Data

            ulong TargetID = Convert.ToUInt64(arg.Args[0]);


            if (TargetID.IsSteamId())
            {
                arg.Player().SendConsoleCommand($"unban {TargetID}");
                DrawPopupMessage(arg.Player(), "XMenu_BannedPlayer_Menu", "Player unbanned", true, 30);
            }
            else
            {
                foreach (var i in ServerUsers.GetAll(ServerUsers.UserGroup.Banned))
                {
                    if (i.steamid == TargetID)
                    {
                        i.group = ServerUsers.UserGroup.None;
                        DrawPopupMessage(arg.Player(), "XMenu_BannedPlayer_Menu", "Player unbanned", true, 30);
                        break;
                    }
                }
            }

        }



        #endregion







        [ConsoleCommand("xmenu.menu.goback")] //Goes Back To Main UI
        private void OptionsGoBackConsoleCommand(ConsoleSystem.Arg arg)
        {
            PlayerData.LoadCachedData(arg.Player().userID); //Try Load Data

            #region Pages To Close
            arg.Player().SendConsoleCommand("xmenu.close.menu.selected.options");
            arg.Player().SendConsoleCommand("xmenu.close.menu.selected.banned");
            arg.Player().SendConsoleCommand("xmenu.close.menu.selected.createban");
            #endregion
            #region Reset CachedData
            cachedPlayerData[arg.Player().userID].TotalTimesBanned = 0;
            cachedPlayerData[arg.Player().userID].SelectedBannedTargetPlayerInfo = "";
            cachedPlayerData[arg.Player().userID].SelectedBannedTargetPlayerInfoInput_Args = new List<string>() { "NULL", "NULL", "NULL", "NULL", "NULL", "NULL", "NULL" };
            cachedPlayerData[arg.Player().userID].SelectedBannedTargetPlayerInfo_Args = new List<string>();
            cachedPlayerData[arg.Player().userID].SelectedTargetID = 0;
            cachedPlayerData[arg.Player().userID].SelectedTargetName = "";
            #endregion
            DrawMenu(arg.Player(), cachedPlayerData[arg.Player().userID].ActiveList, false, cachedPlayerData[arg.Player().userID].CurrentPage);
            DrawMenu(arg.Player(), cachedPlayerData[arg.Player().userID].ActiveList, true, cachedPlayerData[arg.Player().userID].CurrentPage);



        }






        [ConsoleCommand("xmenu.menu.destory")]
        private void DestoryMenu(ConsoleSystem.Arg arg)
        {
            cachedPlayerData.Remove(arg.Player().userID);
            arg.Player().SendConsoleCommand("xmenu.close.menu.main");
            arg.Player().SendConsoleCommand("xmenu.close.menu.selected.options");
            arg.Player().SendConsoleCommand("xmenu.close.menu.selected.banned");
            arg.Player().SendConsoleCommand("xmenu.close.menu.selected.createban");
            arg.Player().SendConsoleCommand("xmenu.close.menu.selected.createreport");


            CuiHelper.DestroyUi(arg.Player(), "XMenu_BlurElement"); //Destroy Blur

            ToggleFreezeTimer(arg.Player());

        } //Destory ALL UI





        [ConsoleCommand("xmenu.player.onclick")] //ON Button/player Click
        private void Panel_OnPlayerClick(ConsoleSystem.Arg arg)
        {


            PlayerData.LoadCachedData(arg.Player().userID);

            cachedPlayerData[arg.Player().userID].SelectedTargetID = 0;
            cachedPlayerData[arg.Player().userID].SelectedTargetName = "";


            if (arg.Args[0].All(char.IsDigit)) cachedPlayerData[arg.Player().userID].SelectedTargetID = Convert.ToUInt64(arg.Args[0]);


            cachedPlayerData[arg.Player().userID].SelectedTargetName = arg.Args[1];

            if (cachedPlayerData[arg.Player().userID].ActiveListName == "Online" || cachedPlayerData[arg.Player().userID].ActiveListName == "Disk" && !PlayerBanned(cachedPlayerData[arg.Player().userID].SelectedTargetID))
            {
                arg.Player().SendConsoleCommand("xmenu.open.menu.selected.options"); //Opens Options Menu
            }

            if (PlayerBanned(cachedPlayerData[arg.Player().userID].SelectedTargetID))
            {
                foreach (var BannedPlayer in ServerUsers.GetAll(ServerUsers.UserGroup.Banned))
                {
                    if (BannedPlayer.steamid == cachedPlayerData[arg.Player().userID].SelectedTargetID)
                    {



                        ulong returnID = BannedPlayer.steamid;
                        string returnName = BannedPlayer.username.ToLower();
                        string returnNotes = BannedPlayer.notes;
                        long returnExpiry = 0;
                        if (!BannedPlayer.IsExpired)
                        {
                            returnExpiry = BannedPlayer.expiry;
                        }

                        cachedPlayerData[arg.Player().userID].SelectedBannedTargetPlayerInfo = $"{returnName}|{returnNotes}|{returnExpiry}";
                        arg.Player().SendConsoleCommand("xmenu.open.menu.selected.banned"); //Opens Banned Menu
                        break;
                    }
                }
            }

            if (cachedPlayerData[arg.Player().userID].ActiveListName == "Banned")
            {
                cachedPlayerData[arg.Player().userID].SelectedBannedTargetPlayerInfo = arg.Args[1];
                arg.Player().SendConsoleCommand("xmenu.open.menu.selected.banned"); //Opens Banned Menu
            }


        }


        #region Set List To Display

        [ConsoleCommand("xmenu.setlist.banned")]
        private void DisplayListBanned(ConsoleSystem.Arg arg)
        {
            PlayerData.LoadCachedData(arg.Player().userID); //Load Data

            if (cachedPlayerData[arg.Player().userID].ActiveListName != "Banned" || cachedPlayerData[arg.Player().userID].SearchOverRide == 3)
            {
                cachedPlayerData[arg.Player().userID].SearchOverRide = 0;
                cachedPlayerData[arg.Player().userID].ActiveList = PlayerLists.GetBannedList();
                cachedPlayerData[arg.Player().userID].ActiveListName = "Banned";
                cachedPlayerData[arg.Player().userID].CurrentPage = 0;



                DrawMenu(arg.Player(), cachedPlayerData[arg.Player().userID].ActiveList, true, cachedPlayerData[arg.Player().userID].CurrentPage);
            }

        } //Set List to Banned

        [ConsoleCommand("xmenu.setlist.online")]
        private void DisplayListOnline(ConsoleSystem.Arg arg)
        {
            PlayerData.LoadCachedData(arg.Player().userID); //Load Data

            if (cachedPlayerData[arg.Player().userID].ActiveListName != "Online" || cachedPlayerData[arg.Player().userID].SearchOverRide == 1)
            {
                cachedPlayerData[arg.Player().userID].SearchOverRide = 0;
                cachedPlayerData[arg.Player().userID].ActiveList = PlayerLists.GetOnlineList();
                cachedPlayerData[arg.Player().userID].ActiveListName = "Online";
                cachedPlayerData[arg.Player().userID].CurrentPage = 0;
                DrawMenu(arg.Player(), cachedPlayerData[arg.Player().userID].ActiveList, true, cachedPlayerData[arg.Player().userID].CurrentPage);
            }
        } //Set List to Online

        [ConsoleCommand("xmenu.setlist.disk")]
        private void DisplayListDisk(ConsoleSystem.Arg arg)
        {
            PlayerData.LoadCachedData(arg.Player().userID); //Load Data

            if (cachedPlayerData[arg.Player().userID].ActiveListName != "Disk" || cachedPlayerData[arg.Player().userID].SearchOverRide == 2)
            {
                cachedPlayerData[arg.Player().userID].SearchOverRide = 0;
                cachedPlayerData[arg.Player().userID].ActiveList = PlayerLists.GetDiskList();
                cachedPlayerData[arg.Player().userID].ActiveListName = "Disk";
                cachedPlayerData[arg.Player().userID].CurrentPage = 0;
                DrawMenu(arg.Player(), cachedPlayerData[arg.Player().userID].ActiveList, true, cachedPlayerData[arg.Player().userID].CurrentPage);
            }

        } //Set List to Disk

        #endregion

        #region Next / Prev Page

        [ConsoleCommand("xmenu.nextpage")]
        private void DrawNextPage(ConsoleSystem.Arg arg)
        {
            PlayerData.LoadCachedData(arg.Player().userID); //Load Data

            cachedPlayerData[arg.Player().userID].CurrentPage++;

            DrawMenu(arg.Player(), cachedPlayerData[arg.Player().userID].ActiveList, true, cachedPlayerData[arg.Player().userID].CurrentPage);
        } //Goes to Next Page

        [ConsoleCommand("xmenu.prevpage")]
        private void DrawPreviousPage(ConsoleSystem.Arg arg)
        {
            PlayerData.LoadCachedData(arg.Player().userID); //Load Data

            if (cachedPlayerData[arg.Player().userID].CurrentPage != 0)
            {
                cachedPlayerData[arg.Player().userID].CurrentPage--;

                DrawMenu(arg.Player(), cachedPlayerData[arg.Player().userID].ActiveList, true, cachedPlayerData[arg.Player().userID].CurrentPage);
            }

        } //Goes to Previous Page
        #endregion

        #endregion

        #region Open/Close UI
        [ConsoleCommand("xmenu.open.menu.main")]
        private void OpenMainUI(ConsoleSystem.Arg arg)
        {
            #region Pages To Close
            arg.Player().SendConsoleCommand("xmenu.close.menu.selected.options");
            arg.Player().SendConsoleCommand("xmenu.close.menu.selected.banned");
            arg.Player().SendConsoleCommand("xmenu.close.menu.selected.createban");
            arg.Player().SendConsoleCommand("xmenu.close.menu.selected.createreport");
            #endregion

            PlayerData.LoadCachedData(arg.Player().userID); //Try Load Data

            if (!cachedPlayerData[arg.Player().userID].MenuMainOpen)
            {
                cachedPlayerData[arg.Player().userID].ActiveList = PlayerLists.GetOnlineList(); // Default List
                cachedPlayerData[arg.Player().userID].ActiveListName = "Online";

                DrawMenu(arg.Player(), cachedPlayerData[arg.Player().userID].ActiveList, false, cachedPlayerData[arg.Player().userID].CurrentPage);
                DrawMenu(arg.Player(), cachedPlayerData[arg.Player().userID].ActiveList, true, cachedPlayerData[arg.Player().userID].CurrentPage);

                cachedPlayerData[arg.Player().userID].MenuMainOpen = true;
            }
        }


        [ConsoleCommand("xmenu.open.menu.selected.options")]
        private void OpenOptionsUI(ConsoleSystem.Arg arg)
        {
            #region Pages To Close
            arg.Player().SendConsoleCommand("xmenu.close.menu.main");
            arg.Player().SendConsoleCommand("xmenu.close.menu.selected.banned");
            arg.Player().SendConsoleCommand("xmenu.close.menu.selected.createban");
            arg.Player().SendConsoleCommand("xmenu.close.menu.selected.createreport");
            #endregion

            PlayerData.LoadCachedData(arg.Player().userID); //Try Load Data

            if (!cachedPlayerData[arg.Player().userID].MenuOptionsOpen)
            {
                DrawPlayerOptionsMenu(arg.Player());

                cachedPlayerData[arg.Player().userID].MenuOptionsOpen = true;
            }
        }

        [ConsoleCommand("xmenu.open.menu.selected.banned")]
        private void OpenBannedUI(ConsoleSystem.Arg arg)
        {
            #region Pages To Close
            arg.Player().SendConsoleCommand("xmenu.close.menu.main");
            arg.Player().SendConsoleCommand("xmenu.close.menu.selected.options");
            arg.Player().SendConsoleCommand("xmenu.close.menu.selected.createban");
            arg.Player().SendConsoleCommand("xmenu.close.menu.selected.createreport");
            #endregion

            PlayerData.LoadCachedData(arg.Player().userID); //Try Load Data

            if (!cachedPlayerData[arg.Player().userID].MenuBannedOpen)
            {
                cachedPlayerData[arg.Player().userID].SelectedBannedTargetPlayerInfo_Args.Clear();



                foreach (var i in cachedPlayerData[arg.Player().userID].SelectedBannedTargetPlayerInfo.Split('|'))
                {
                    cachedPlayerData[arg.Player().userID].SelectedBannedTargetPlayerInfo_Args.Add(i.Replace(".", " "));
                }

                cachedPlayerData[arg.Player().userID].SelectedTargetName = cachedPlayerData[arg.Player().userID].SelectedBannedTargetPlayerInfo_Args[0];


                DrawPlayerBannedMenu(arg.Player());

                cachedPlayerData[arg.Player().userID].MenuBannedOpen = true;
            }
        }

        [ConsoleCommand("xmenu.open.menu.selected.createban")]
        private void OpenCreateBanUI(ConsoleSystem.Arg arg)
        {
            #region Pages To Close
            arg.Player().SendConsoleCommand("xmenu.close.menu.main");
            //arg.Player().SendConsoleCommand("xmenu.close.menu.selected.options");
            arg.Player().SendConsoleCommand("xmenu.close.menu.selected.banned");
            arg.Player().SendConsoleCommand("xmenu.close.menu.selected.createreport");
            #endregion

            PlayerData.LoadCachedData(arg.Player().userID); //Try Load Data


            if (!PlayerBanned(cachedPlayerData[arg.Player().userID].SelectedTargetID))
            {
                if (!cachedPlayerData[arg.Player().userID].MenuCreateBanOpen)
                {
                    arg.Player().SendConsoleCommand("xmenu.close.menu.selected.options");
                    DrawBanMenu(arg.Player());

                    cachedPlayerData[arg.Player().userID].MenuCreateBanOpen = true;
                }
            }
            else
            {

                DrawPopupMessage(arg.Player(), "XMenu_SelectedPlayer_OptionsMenu", "Player Already Banned", false, 30);

            }
        }


        [ConsoleCommand("xmenu.open.menu.selected.createreport")]
        private void OpenCreateReportUI(ConsoleSystem.Arg arg)
        {
            #region Pages To Close
            arg.Player().SendConsoleCommand("xmenu.close.menu.main");
            //arg.Player().SendConsoleCommand("xmenu.close.menu.selected.options");
            arg.Player().SendConsoleCommand("xmenu.close.menu.selected.banned");
            arg.Player().SendConsoleCommand("xmenu.close.menu.selected.createban");
            #endregion

            PlayerData.LoadCachedData(arg.Player().userID); //Try Load Data
            if (!PlayerReported(cachedPlayerData[arg.Player().userID].SelectedTargetID))
            {
                if (!cachedPlayerData[arg.Player().userID].MenuCreateReportOpen)
                {
                    arg.Player().SendConsoleCommand("xmenu.close.menu.selected.options");
                    DrawReportMenu(arg.Player());

                    cachedPlayerData[arg.Player().userID].MenuCreateReportOpen = true;
                }
            }
            else
            {

                DrawPopupMessage(arg.Player(), "XMenu_SelectedPlayer_OptionsMenu", "Player Already Reported", false, 30);

            }

        }


        #region Close
        [ConsoleCommand("xmenu.close.menu.main")]
        private void CloseMainUI(ConsoleSystem.Arg arg)
        {
            PlayerData.LoadCachedData(arg.Player().userID); //Try Load Data
            CuiHelper.DestroyUi(arg.Player(), "XMenu_MainContainer");
            cachedPlayerData[arg.Player().userID].MenuMainOpen = false;
        }

        [ConsoleCommand("xmenu.close.menu.selected.options")]
        private void Close_Selected_OptionsUI(ConsoleSystem.Arg arg)
        {
            PlayerData.LoadCachedData(arg.Player().userID); //Try Load Data
            CuiHelper.DestroyUi(arg.Player(), "XMenu_SelectedPlayer_OptionsMenu");
            cachedPlayerData[arg.Player().userID].MenuOptionsOpen = false;
        }

        [ConsoleCommand("xmenu.close.menu.selected.banned")]
        private void Close_Selected_BannedUI(ConsoleSystem.Arg arg)
        {
            PlayerData.LoadCachedData(arg.Player().userID); //Try Load Data
            CuiHelper.DestroyUi(arg.Player(), "XMenu_BannedPlayer_Menu");
            cachedPlayerData[arg.Player().userID].MenuBannedOpen = false;
        }

        [ConsoleCommand("xmenu.close.menu.selected.createban")]
        private void Close_Selected_CreateBanUI(ConsoleSystem.Arg arg)
        {
            PlayerData.LoadCachedData(arg.Player().userID); //Try Load Data
            CuiHelper.DestroyUi(arg.Player(), "XMenu_BanPlayer_Form");
            cachedPlayerData[arg.Player().userID].MenuCreateBanOpen = false;
        }

        [ConsoleCommand("xmenu.close.menu.selected.createreport")]
        private void Close_Selected_CreateReportUI(ConsoleSystem.Arg arg)
        {
            PlayerData.LoadCachedData(arg.Player().userID); //Try Load Data
            CuiHelper.DestroyUi(arg.Player(), "XMenu_ReportPlayer_Form");
            cachedPlayerData[arg.Player().userID].MenuCreateReportOpen = false;
        }
        #endregion
        #endregion


        #region Get Player Lists
        public class PlayerLists
        {


            public static SortedList<ulong, string> GetReportedListSimple()
            {
                SortedList<ulong, string> ReportedPlayerList = Interface.Oxide.DataFileSystem.ReadObject<SortedList<ulong, string>>("XMenu/Reports");
                foreach (var ReportedPlayer in ReportedPlayerList)
                {
                    ulong returnID = ReportedPlayer.Key;
                    string returnName = ReportedPlayer.Value.ToLower();
                }
                return ReportedPlayerList;
            }


            public static void AddReportToList(ulong id, string args)
            {
                SortedList<ulong, string> UpdatedReportPlayerList = Interface.Oxide.DataFileSystem.ReadObject<SortedList<ulong, string>>("XMenu/Reports");
                if (!UpdatedReportPlayerList.ContainsKey(id))
                {
                    UpdatedReportPlayerList.Add(id, args);
                    Interface.Oxide.DataFileSystem.WriteObject("XMenu/Reports", UpdatedReportPlayerList, true);
                }



            }


            public static SortedList<ulong, string> GetBannedListSimple()
            {
                SortedList<ulong, string> BannedPlayerList = new SortedList<ulong, string>();
                foreach (var BannedPlayer in ServerUsers.GetAll(ServerUsers.UserGroup.Banned))
                {
                    ulong returnID = BannedPlayer.steamid;
                    string returnName = BannedPlayer.username.ToLower();

                    BannedPlayerList.Add(returnID, returnName);
                }
                return BannedPlayerList;
            }

            public static SortedList<ulong, string> GetBannedList()
            {
                SortedList<ulong, string> BannedPlayerList = new SortedList<ulong, string>();
                foreach (var BannedPlayer in ServerUsers.GetAll(ServerUsers.UserGroup.Banned))
                {
                    ulong returnID = BannedPlayer.steamid;
                    string returnName = BannedPlayer.username.ToLower();
                    string returnNotes = BannedPlayer.notes;
                    long returnExpiry = 0;
                    if (!BannedPlayer.IsExpired)
                    {
                        returnExpiry = BannedPlayer.expiry;
                    }

                    BannedPlayerList.Add(returnID, $"{returnName}|{returnNotes}|{returnExpiry}");
                }
                return BannedPlayerList;
            }




            public static SortedList<ulong, string> GetOnlineList()
            {
                SortedList<ulong, string> OnlinePlayerList = new SortedList<ulong, string>();
                foreach (BasePlayer player in BasePlayer.activePlayerList)
                {
                    OnlinePlayerList.Add(player.userID, player.displayName.ToLower());
                }
                return OnlinePlayerList;
            }

            public static SortedList<ulong, string> GetDiskList()
            {
                SortedList<ulong, string> DiskPlayerList = Interface.Oxide.DataFileSystem.ReadObject<SortedList<ulong, string>>("XMenu/PlayerList");
                return DiskPlayerList;
            }

            public static int GetTotalBans(ulong id)
            {
                SortedList<ulong, int> UpdatedBannedPlayerCountList = Interface.Oxide.DataFileSystem.ReadObject<SortedList<ulong, int>>("XMenu/BanCountList");
                //Add New Ban To List
                int BanCount = 0;
                foreach (var i in UpdatedBannedPlayerCountList)
                {
                    if (i.Key == id)
                    {
                        if (i.Value >= 1)
                        {
                            BanCount = i.Value;
                        }
                        break;
                    }
                }
                return BanCount;
            }

            public static void UpdateBannedCount(ulong id)
            {
                SortedList<ulong, int> UpdatedBannedPlayerCountList = Interface.Oxide.DataFileSystem.ReadObject<SortedList<ulong, int>>("XMenu/BanCountList");
                if (!UpdatedBannedPlayerCountList.ContainsKey(id))
                {
                    UpdatedBannedPlayerCountList.Add(id, 1);
                    Interface.Oxide.DataFileSystem.WriteObject("XMenu/BanCountList", UpdatedBannedPlayerCountList, true);
                }

                //Add Ban To List
                foreach (var i in UpdatedBannedPlayerCountList)
                {
                    if (UpdatedBannedPlayerCountList.ContainsKey(id) && i.Key == id)
                    {

                        int BanCount = i.Value;
                        UpdatedBannedPlayerCountList.Remove(id);
                        UpdatedBannedPlayerCountList.Add(id, BanCount + 1);
                        break;

                    }

                }
                Interface.Oxide.DataFileSystem.WriteObject("XMenu/BanCountList", UpdatedBannedPlayerCountList, true);
            }


            public static void UpdateDiskList(BasePlayer player)
            {
                SortedList<ulong, string> UpdatedDiskPlayerList = Interface.Oxide.DataFileSystem.ReadObject<SortedList<ulong, string>>("XMenu/PlayerList");

                //Add New Player To List
                if (!UpdatedDiskPlayerList.ContainsKey(player.userID))
                {
                    UpdatedDiskPlayerList.Add(player.userID, player.displayName.ToLower());
                    Interface.Oxide.DataFileSystem.WriteObject("XMenu/PlayerList", UpdatedDiskPlayerList, true);
                }

                //Update Name IF changed
                foreach (KeyValuePair<ulong, string> keyPlayer in UpdatedDiskPlayerList)
                {
                    if (UpdatedDiskPlayerList.ContainsKey(player.userID) && keyPlayer.Value != player.displayName.ToLower())
                    {
                        UpdatedDiskPlayerList.Remove(player.userID);
                        UpdatedDiskPlayerList.Add(player.userID, player.displayName.ToLower());
                        Interface.Oxide.DataFileSystem.WriteObject(($"XMenu/PlayerList"), UpdatedDiskPlayerList, true);
                    }
                }
            }
        }
        #endregion




        #region Mute Function





        [ConsoleCommand("x")]
        private void TestMute(ConsoleSystem.Arg arg)
        {

            if (arg.HasArgs(2))
            {
                var targetID = arg.Args[0];
                var reason = arg.Args[1];
                var expire = arg.Args[2];
                arg.Player().SendConsoleCommand("xmute", targetID, reason, expire);
            }
            //string expire = DateTimeOffset.Now.AddHours(1).ToUnixTimeSeconds().ToString();

        }





        #endregion

        [ChatCommand("xmenu")] private void ChatCommand_OpenMenu(BasePlayer player) => player.SendConsoleCommand("xmenu.open.menu.main"); // Chat command Open Menu UI

    }
}

