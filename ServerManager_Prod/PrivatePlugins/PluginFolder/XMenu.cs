using Oxide.Core;
using Oxide.Game.Rust.Cui;
using Oxide.Plugins;
using System;
using System.Collections.Generic;
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



        private void Init()
        {
            foreach (BasePlayer player in BasePlayer.activePlayerList)
            {
                PlayerMenu_DestoryUI(player);
            }
        }




        #region PlayerMenu UI



        bool MenuOpen = false;
        private void PlayerMenu_DestoryUI(BasePlayer player)
        {
            CuiHelper.DestroyUi(player, "PlayerMenu_MainUI");
            MenuOpen = false;
        }


        [ChatCommand("set")]
        private void PlayerMenu_DrawUI(BasePlayer player)
        {
            PlayerMenu_DestoryUI(player);
            CuiElementContainer elements = new CuiElementContainer();

            MenuOpen = true;


            #region MainPanel
            string MainPanel = elements.Add(new CuiPanel
            {
                Image = { Color = "0.1 0.1 0.1 0.9" },
                RectTransform =
                {
                    AnchorMin = "0.35 0.15",
                    AnchorMax = "0.65 0.85"
                },
                CursorEnabled = true
            }, "Hud", "PlayerMenu_MainUI");
            #endregion


            #region Top Panel
            //Panel
            elements.Add(new CuiElement
            {
                Name = "TopPanel",
                Parent = MainPanel,
                Components =
                {
                    new CuiImageComponent
                    {
                        Color = "0.2 0.4 0.6 1",
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0 0.925",
                        AnchorMax = "1 1",
                        OffsetMax = "0 0"
                    }
                }
            });

            //Panel Text
            elements.Add(new CuiElement
            {
                Name = "TopPanelText",
                Parent = "TopPanel",
                Components =
                {
                    new CuiTextComponent
                    {
                        Text = "Player Administration Menu",
                        Align = TextAnchor.MiddleLeft,
                        Color = "1 1 1 1",
                        FontSize = 20
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0.02 0",
                        AnchorMax = "1 1",
                    }

                }
            });

            //Close Button
            elements.Add(new CuiElement
            {
                Name = "PlayerMenu_ButtonClose",
                Parent = "TopPanel",
                Components =
                {
                    new CuiButtonComponent
                    {
                        Close = "PlayerMenu_MainUI",
                        Color = "0.5 0 0 1",
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0.9 0",
                        AnchorMax = "1 1",
                        OffsetMax = "0 0"
                    }
                }
            });

            //Close Button Text
            elements.Add(new CuiElement
            {
                Name = "PlayerMenu_ButtonClose_Text",
                Parent = "PlayerMenu_ButtonClose",
                Components =
                {
                    new CuiTextComponent
                    {
                        Text = "X",
                        Align = TextAnchor.MiddleCenter,
                        Color = "1 1 1 1",
                        FontSize = 20
                    }
                }
            });

            #endregion



            #region Border Lines Styleing

            //Bottom Border
            elements.Add(new CuiElement
            {
                Parent = MainPanel,
                Components =
                {
                    new CuiImageComponent
                    {
                        Color = "0.2 0.4 0.6 1",
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0 0",
                        AnchorMax = "1 0.005",
                        OffsetMax = "0 0"
                    }
                }
            });

            //Left Border
            elements.Add(new CuiElement
            {
                Parent = MainPanel,
                Components =
                {
                    new CuiImageComponent
                    {
                        Color = "0.2 0.4 0.6 1",
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0 0",
                        AnchorMax = "0.005 0.925",
                        OffsetMax = "0 0"
                    }
                }
            });

            //Right Border
            elements.Add(new CuiElement
            {
                Parent = MainPanel,
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
            });
            #endregion


            #region Main Buttons Panel
            elements.Add(new CuiElement
            {
                Name = "PlayerMenu_PlayerPanel",
                Parent = MainPanel,
                Components =
                {
                    new CuiImageComponent
                    {
                        Color = "0.1 0.1 0.1 1"
                    },

                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0.005 0.065", //065
                        AnchorMax = "0.995 0.865", //865
                        OffsetMax = "0 0"
                    }
                }
            });
            #endregion



            #region Bottom Panel
            //Panel
            elements.Add(new CuiElement
            {
                Name = "BottomPanel",
                Parent = MainPanel,
                Components =
                {
                    new CuiImageComponent
                    {
                        Color = "0.1 0.2 0.3 1",
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0.005 0.005",
                        AnchorMax = "0.995 0.065",
                        OffsetMax = "0 0"
                    }
                }
            });

            //Button Online Players
            elements.Add(new CuiElement
            {
                Name = "ButtonSelector_Online",
                Parent = "BottomPanel",
                Components =
                {
                    new CuiButtonComponent
                    {
                        Color = "0.2 0.3 0.4 1"
                    },

                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0.25 0.1",
                        AnchorMax = "0.4975 0.9",
                        OffsetMax = "0 0",
                    }
                }
            });
            //Button Online Players Text
            elements.Add(new CuiElement
            {
                Parent = "ButtonSelector_Online",
                Components =
                {
                    new CuiTextComponent
                    {
                        Text = "Online Players",
                        Color = "1 1 1 1",
                        Align = TextAnchor.MiddleCenter,
                        FontSize = 14
                    }
                }
            });



            //Button All Players
            elements.Add(new CuiElement
            {
                Name = "ButtonSelector_ALL",
                Parent = "BottomPanel",
                Components =
                {
                    new CuiButtonComponent
                    {
                        Color = "0.2 0.3 0.4 1"
                    },

                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0.5025 0.1",
                        AnchorMax = "0.75 0.9",
                        OffsetMax = "0 0",
                    }
                }
            });
            //Button Online Players Text
            elements.Add(new CuiElement
            {
                Parent = "ButtonSelector_ALL",
                Components =
                {
                    new CuiTextComponent
                    {
                        Text = "All Players",
                        Color = "1 1 1 1",
                        Align = TextAnchor.MiddleCenter,
                        FontSize = 14
                    }
                }
            });


            //Previous & Next Page Buttons

            //Previous Button
            elements.Add(new CuiElement
            {
                Name = "ButtonPrevious",
                Parent = "BottomPanel",
                Components =
                {
                    new CuiButtonComponent
                    {
                        Color = "0.2 0.3 0.4 1"
                    },

                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0.185 0.1",
                        AnchorMax = "0.235 0.9",
                        OffsetMax = "0 0",
                    }
                }
            });
            //Previous Button Text
            elements.Add(new CuiElement
            {
                Parent = "ButtonPrevious",
                Components =
                {
                    new CuiTextComponent
                    {
                        Text = "<",
                        Color = "1 1 1 1",
                        Align = TextAnchor.MiddleCenter,
                        FontSize = 20
                    }
                }
            });


            //Next Button
            elements.Add(new CuiElement
            {
                Name = "ButtonNext",
                Parent = "BottomPanel",
                Components =
                {
                    new CuiButtonComponent
                    {
                        Color = "0.2 0.3 0.4 1"
                    },

                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0.765 0.1",
                        AnchorMax = "0.815 0.9",
                        OffsetMax = "0 0",
                    }
                }
            });
            //Next Button Text
            elements.Add(new CuiElement
            {
                Parent = "ButtonNext",
                Components =
                {
                    new CuiTextComponent
                    {
                        Text = ">",
                        Color = "1 1 1 1",
                        Align = TextAnchor.MiddleCenter,
                        FontSize = 20
                    }
                }
            });

            #endregion

            #region Search UI

            //Search Panel TOP
            elements.Add(new CuiElement
            {
                Name = "PlayerMenu_SearchPanel",
                Parent = MainPanel,
                Components =
                {
                    new CuiImageComponent
                    {
                          Color = "0.1 0.2 0.3 1"
                    },

                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0.005 0.865",
                        AnchorMax = "0.995 0.925",
                        OffsetMax = "0 0"
                    },
                }
            });
            //Search Panel TOP TEXT
            elements.Add(new CuiElement
            {
                Parent = "PlayerMenu_SearchPanel",
                Components =
                {
                    new CuiTextComponent
                    {
                        Text = "Search Steam ID",
                        Color = "1 1 1 1",
                        Align = TextAnchor.MiddleLeft,
                        FontSize = 15
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0.02 0",
                        AnchorMax = "1 1",
                    }
                }
            });

            //Search Panel TOP INPUT Panel
            elements.Add(new CuiElement
            {
                Name = "PlayerMenu_SearchPanel_InputPanel",
                Parent = "PlayerMenu_SearchPanel",
                Components =
                {
                    new CuiImageComponent
                    {
                        Color = "0.1 0.1 0.1 1"
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0.3 0.2",
                        AnchorMax = "0.995 0.8",
                        OffsetMax = "0 0",
                    }

                }
            });

            //Search Panel TOP INPUT Field
            elements.Add(new CuiElement
            {
                Parent = "PlayerMenu_SearchPanel_InputPanel",
                Components =
                {
                    new CuiInputFieldComponent
                    {
                        Color = "1 1 1 1",
                        Align = TextAnchor.MiddleLeft,
                        Command = "xmenu.search.list"
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0.005 0",
                        AnchorMax = "0.995 1",
                        OffsetMax = "0 0",
                    }
                }
            });

            #endregion


            double ButtonSizeWidth = 0.332;


            double testvalue1 = 0.0;
            double testvalue2 = ButtonSizeWidth;


            double MarginGap = 0.005;

            //Height
            double testvalue3 = 0.94525;
            double testvalue4 = 0.995;

            double Gap = testvalue4 - testvalue3;


            int Index = 0;

       
            foreach (KeyValuePair<ulong, string> keyPlayer in PlayerLists.GetDiskList())
            {
                if (Index >= 3)
                {
                    Index = 0;

                    testvalue1 = 0.0;
                    testvalue2 = ButtonSizeWidth;



                    testvalue3 -= Gap;
                    testvalue4 -= Gap;




                }

                string playerName = null;

                if (keyPlayer.Value.Length > 5)
                {
                    playerName = keyPlayer.Value.Substring(0, 5) + "...";
                }
                else
                {
                    playerName = keyPlayer.Value;
                }

                elements.Add(new CuiElement
                {
                    Name = "PlayerMenu_PlayerPanel_Button",
                    Parent = "PlayerMenu_PlayerPanel",
                    Components =
                    {

                        new CuiButtonComponent
                        {
                            Color = "0.2 0.24 0.27 1",
                        },

                        new CuiRectTransformComponent
                        {
                            AnchorMin = $"{testvalue1+MarginGap} {testvalue3+MarginGap}", //95
                            AnchorMax = $"{testvalue2} {testvalue4}", //99

                            OffsetMax = "0 0"
                        }
                    }
                });

                elements.Add(new CuiElement
                {
                    Parent = "PlayerMenu_PlayerPanel_Button",
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

                testvalue1 += ButtonSizeWidth;
                testvalue2 += ButtonSizeWidth;



                Index++;
            }
            

            #endregion





            CuiHelper.AddUi(player, elements);

        }


        #region Search List
        [ConsoleCommand("xmenu.search.list")]
        private void ConsoleSystemGetArg(ConsoleSystem.Arg arg)
        {
            //Search
            foreach (var i in SearchFunction.searchList(PlayerLists.GetDiskList(), arg.Args[0]))
            {
                PrintWarning("Found" + i);
            }
        }
        public class SearchFunction
        {
            public static SortedList<ulong, string> searchList(SortedList<ulong, string> ListToSearch, string searchValue)
            {
                if (ListToSearch != null && searchValue != null)
                {
                    SortedList<ulong, string> ResultList = new SortedList<ulong, string>();
                    foreach (var i in ListToSearch)
                    {
                        if (i.ToString().Contains(searchValue))
                        {
                            ResultList.Add(Convert.ToUInt64(i.Key), i.Value);
                        }
                    }
                    return ResultList;
                }
                return null; // IF LIST OR VALUE NULL
            }
        }
        #endregion

        #region Get Player Lists
        public class PlayerLists
        {
            public static SortedList<ulong, string> GetDiskList()
            {
                SortedList<ulong, string> DiskPlayerList = Interface.Oxide.DataFileSystem.ReadObject<SortedList<ulong, string>>("XMenu/PlayerList");
                return DiskPlayerList; 
            }

            public static void UpdateDiskList(BasePlayer player)
            {
                SortedList<ulong, string> UpdatedDiskPlayerList = new SortedList<ulong, string>();

                //Add New Player To List
                if(!UpdatedDiskPlayerList.ContainsKey(player.userID))
                {
                    UpdatedDiskPlayerList.Add(player.userID, player.displayName.ToLower());
                    Interface.Oxide.DataFileSystem.WriteObject("XMenu/PlayerList", UpdatedDiskPlayerList, true);
                }

                //Update Name IF changed
                foreach (KeyValuePair<ulong, string> keyPlayer in UpdatedDiskPlayerList)
                {
                    if (UpdatedDiskPlayerList.ContainsKey(player.userID) && keyPlayer.Value != player.displayName)
                    {
                        UpdatedDiskPlayerList.Remove(player.userID);
                        UpdatedDiskPlayerList.Add(player.userID, player.displayName.ToLower());
                        Interface.Oxide.DataFileSystem.WriteObject(($"XMenu/PlayerList"), UpdatedDiskPlayerList, true);
                    }
                }
            }
        }
        #endregion





 

        private void OnPlayerConnected(BasePlayer player)
        {
            PlayerLists.UpdateDiskList(player);
        }



    }
}
