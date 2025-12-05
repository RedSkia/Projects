using Oxide.Core;
using Oxide.Game.Rust.Cui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Oxide.Plugins
{
    [Info("XJail", "Infinitynet.dk", "1.0.0")]
    [Description("Copyright Infinitynet.dk All Rights Reserved")]
    public class XJail : RustPlugin
    {

        #region OnStartup
        private void Init()
        {

            foreach (BasePlayer player in BasePlayer.activePlayerList)
            {
                CuiHelper.DestroyUi(player, "XJail_MainUI");
            }
        }
        #endregion




        #region Data Management
        static Dictionary<ulong, PlayerData> cachedPlayerData = new Dictionary<ulong, PlayerData>();


        private class PlayerData
        {
            public Timer teleportTimer;
            public Vector3 playerPos = new Vector3();
            public float playerPosZ_Cached = 0;

            public List<BaseEntity> EntityBox = new List<BaseEntity>();

            internal static void LoadCachedData(ulong id)
            {
                if (!cachedPlayerData.ContainsKey(id)) cachedPlayerData.Add(id, new PlayerData());
            }
        }
        #endregion




        #region UI
        private void DestoryJailUI(BasePlayer player) => CuiHelper.DestroyUi(player, "XJail_MainUI");


        [ChatCommand("jailui")]
        private void DrawJailUI(BasePlayer player)
        {
            CuiElementContainer elements = new CuiElementContainer();
            elements.Add(new CuiPanel
            {
                Image = { Color = "0 0 0 1" },
                RectTransform =
                {
                    AnchorMin = "0 0",
                    AnchorMax = "1 1"
                },
                CursorEnabled = true
            }, "Overall", "XJail_MainUI"); //Main Panel


            elements.Add(new CuiElement
            {
                Parent = "XJail_MainUI",
                Components =
                {
                    new CuiRawImageComponent
                    {
                        Url = "http://hypergamer/pics/bars.png",
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0 0",
                        AnchorMax = "1 1",
                        OffsetMax = "0 0"
                    }
                }
            }); //Bars

            elements.Add(new CuiElement
            {
                Name = "XJail_SubPanel",
                Parent = "XJail_MainUI",
                Components =
                {
                    new CuiImageComponent {Color = "0 0 0 0.95"},
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
                Name = "XJail_DisplayPanel",
                Parent = "XJail_SubPanel",
                Components =
                {
                    new CuiImageComponent {Color = "0.5 0.5 0.5 0"},
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0.3 0.2",
                        AnchorMax = "0.7 0.8",
                        OffsetMax = "0 0"
                    }
                }
            });




            elements.Add(new CuiElement
            {
                Name = "XJail_DisplayPanel_Text",
                Parent = "XJail_DisplayPanel",
                Components =
                {
                    new CuiTextComponent
                    {
                        Text = "You have been <color=#c83232>Suspended</color>",
                        Align = TextAnchor.MiddleCenter,
                        Color = "1 1 1 1",
                        FontSize = 35,
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0 0.9",
                        AnchorMax = "1 1",
                        OffsetMax = "0 0"
                    }
                }
            });
            elements.Add(new CuiElement
            {
                Name = "XJail_DisplayPanel_Text2",
                Parent = "XJail_DisplayPanel",
                Components =
                {
                    new CuiTextComponent
                    {
                        Text = "for the following reason",
                        Align = TextAnchor.MiddleCenter,
                        Color = "1 1 1 1",
                        FontSize = 15,
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0 0.835",
                        AnchorMax = "1 0.935",
                        OffsetMax = "0 0"
                    }
                }
            });



            elements.Add(new CuiElement
            {
                Parent = "XJail_DisplayPanel",
                Components =
                {
                    new CuiTextComponent
                    {
                        Text = $"{PlayerJail.GetReason(player.userID)}",
                        Align = TextAnchor.MiddleCenter,
                        Color = "1 1 1 1",
                        FontSize = 15,
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0 0.165",
                        AnchorMax = "1 0.835",
                        OffsetMax = "0 0"
                    }
                }
            });



            elements.Add(new CuiElement
            {
                Parent = "XJail_DisplayPanel",
                Components =
                {
                    new CuiTextComponent
                    {
                        Text = $"<color=#a5a5a5>Report ID:<</color>{PlayerJail.GetToken(player.userID)}<color=#a5a5a5>></color>",
                        Align = TextAnchor.MiddleCenter,
                        Color = "1 1 1 1",
                        FontSize = 18,
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0 0",
                        AnchorMax = "1 0.265",
                        OffsetMax = "0 0"
                    }
                }
            });

            #region Button
            elements.Add(new CuiElement
            {
                Name = "XJail_DisplayPanel_Button",
                Parent = "XJail_DisplayPanel",
                Components =
                {
                    new CuiButtonComponent
                    {
                        Color = "0.59 0.2 0.2 1",
                        Command = "xjail.kick.player"
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0.35 0",
                        AnchorMax = "0.65 0.1",
                        OffsetMax = "0 0"
                    }
                }
            });
            elements.Add(new CuiElement
            {
                Parent = "XJail_DisplayPanel_Button",
                Components =
                {
                    new CuiTextComponent
                    {
                        Text = "I Understand",
                        Align = TextAnchor.MiddleCenter,
                        Color = "1 1 1 1",
                        FontSize = 35,

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


            CuiHelper.AddUi(player, elements);
        }
        #endregion


        [ConsoleCommand("xjail.kick.player")]
        private void TestMute(ConsoleSystem.Arg arg) => arg.Player().Kick("");




        private void SpawnEntity(ulong id, string EntityType, Vector3 position, Quaternion rotation)
        {
            BaseEntity entity = null;
            switch (EntityType)
            {
                case "floor":
                    entity = GameManager.server.CreateEntity("assets/prefabs/building core/floor/floor.prefab", position, rotation, true);
                    break;

                case "wall":
                    entity = GameManager.server.CreateEntity("assets/prefabs/building core/wall/wall.prefab", position, rotation, true);
                    break;
            }
            BuildingBlock BuildingEntity = entity as BuildingBlock;


            BuildingEntity.grounded = true;
            BuildingEntity.Spawn();
            BuildingEntity.SetGrade(BuildingGrade.Enum.TopTier);
            BuildingEntity.SetHealthToMax();
            BuildingEntity.blockDefinition = PrefabAttribute.server.Find<Construction>(BuildingEntity.prefabID);
            cachedPlayerData[id].EntityBox.Add(BuildingEntity);
            //BuildingEntity.AdminKill();

        }


        private void JailPlayer(BasePlayer player)
        {

            PlayerData.LoadCachedData(player.userID); //Load Data

            DrawJailUI(player);

            StartFreezeTimer(player);

            cachedPlayerData[player.userID].EntityBox.Clear();

            //Bottom Floor
            SpawnEntity(player.userID, "floor", player.transform.position, player.transform.rotation);

            //Front Wall
            SpawnEntity(player.userID, "wall", player.transform.position += new Vector3(0, 0, 1.4f), new Quaternion(x: 0, y: -1, z: 0, w: 1));

            //Back Wall
            SpawnEntity(player.userID, "wall", player.transform.position += new Vector3(0, 0, -2.8f), new Quaternion(x: 0, y: 1, z: 0, w: 1));

            //Left Wall
            SpawnEntity(player.userID, "wall", player.transform.position += new Vector3(-1.4f, 0, 1.4f), new Quaternion(x: 0, y: 5308416, z: 0, w: 1));


            //Right Wall
            SpawnEntity(player.userID, "wall", player.transform.position += new Vector3(2.8f, 0, 0), new Quaternion(x: 0, y: 0, z: 0, w: 1));

            //Top Floor
            SpawnEntity(player.userID, "floor", player.transform.position += new Vector3(-1.4f, 2.8f, 0), player.transform.rotation);


        }

        private void UNJailPlayer(BasePlayer player)
        {

            PlayerData.LoadCachedData(player.userID); //Load Data


            StopFreezeTimer(player);


            foreach (var i in cachedPlayerData[player.userID].EntityBox) { try { i.AdminKill(); } catch { } }



        }

        object OnEntityTakeDamage(BaseCombatEntity entity, HitInfo info)
        {
            try
            {
                ulong targetID = info.InitiatorPlayer.userID;
                if (cachedPlayerData.ContainsKey(targetID) && cachedPlayerData[targetID].EntityBox.Contains(entity as BaseEntity))
                {
                    return false;
                }
            }
            catch { }





            return null;
        }



        string GenerateJailToken()
        {
            System.Random random = new System.Random();
            int length = 4; // 1.413.720 possible tokens
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var token = new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
            return token;
        }


        public class PlayerJail
        {

            internal static string GetReason(ulong id)
            {
                SortedList<ulong, string> list = Interface.Oxide.DataFileSystem.ReadObject<SortedList<ulong, string>>("XJail/List");

                foreach (var i in list)
                {
                    if (i.Key == id)
                    {
                        string[] args = i.Value.Split('|');
                        string reason = args[0];
                        return reason.Replace(".", " "); ;
                        break;
                    }
                }
                return "NULL";
            }

            internal static string GetToken(ulong id)
            {
                SortedList<ulong, string> list = Interface.Oxide.DataFileSystem.ReadObject<SortedList<ulong, string>>("XJail/List");

                foreach (var i in list)
                {
                    if (i.Key == id)
                    {
                        string[] args = i.Value.Split('|');
                        string token = args[1];
                        return token.Replace(".", " ");
                        break;
                    }

                }
                return "NULL";
            }

            internal static void Jail(ulong id, string args)
            {
                SortedList<ulong, string> list = Interface.Oxide.DataFileSystem.ReadObject<SortedList<ulong, string>>("XJail/List");

                if (!list.ContainsKey(id))
                {
                    list.Add(id, args);
                    Interface.Oxide.DataFileSystem.WriteObject("XJail/List", list, true);
                }


            }

            internal static void UnJail(ulong id)
            {
                SortedList<ulong, string> list = Interface.Oxide.DataFileSystem.ReadObject<SortedList<ulong, string>>("XJail/List");
                if (list.ContainsKey(id))
                {
                    list.Remove(id);
                    Interface.Oxide.DataFileSystem.WriteObject("XJail/List", list, true);
                }
            }

            internal static bool Jailed(ulong id)
            {
                SortedList<ulong, string> list = Interface.Oxide.DataFileSystem.ReadObject<SortedList<ulong, string>>("XJail/List");

                if (list.ContainsKey(id))
                {
                    return true;
                }

                return false;
            }

        }

        #region API
        [ConsoleCommand("xjail")]
        private void API_Jail(ConsoleSystem.Arg arg)
        {

            if (arg.HasArgs(2))
            {
                ulong targetID = Convert.ToUInt64(arg.Args[0]);
                var reason = arg.Args[1];

                SortedList<ulong, string> list = Interface.Oxide.DataFileSystem.ReadObject<SortedList<ulong, string>>("XJail/List");
                var token = GenerateJailToken();

                List<string> tokens = new List<string>();
                for (int i = 0; i < list.Keys.Count; i++)
                {
                    var disk_token = list.Values[i].Split('|')[1];
                    tokens.Add(disk_token);
                }

                int retryLimit = 10;
            Retry:
                if (tokens.Contains(token))
                {
                    retryLimit--;
                    if (retryLimit > 0) //Check
                    {
                        token = GenerateJailToken();
                        goto Retry;
                    }
                    else //NO Tokens Available after x tries
                    {
                        PrintWarning("Token Gen Failed after X Tries");
                    }
                }
                else if (!tokens.Contains(token)) //Final
                {
                    PlayerJail.Jail(targetID, $"{reason}|{token}");

                    try
                    {
                        foreach (BasePlayer player in BasePlayer.allPlayerList)
                        {
                            if (targetID == player.userID)
                            {
                                JailPlayer(player);
                                break;
                            }
                        }
                    }
                    catch { }
                }
            }
        }

        [ConsoleCommand("xunjail")]
        private void API_UNJail(ConsoleSystem.Arg arg)
        {
            if (arg.HasArgs(1))
            {
                ulong targetID = Convert.ToUInt64(arg.Args[0]);

                PlayerJail.UnJail(targetID);


                try
                {
                    foreach (BasePlayer player in BasePlayer.allPlayerList)
                    {
                        if (targetID == player.userID)
                        {
                            DestoryJailUI(player);
                            UNJailPlayer(player);
                            break;
                        }
                    }
                }
                catch { }

            }
        }
        #endregion

        #region Jail Teleport
        private void StartFreezeTimer(BasePlayer player)
        {
            PlayerData.LoadCachedData(player.userID); //Load Data


            cachedPlayerData[player.userID].playerPos = player.transform.position;
            //Pre Teleport UP in air
            cachedPlayerData[player.userID].playerPosZ_Cached = cachedPlayerData[player.userID].playerPos.y;
            cachedPlayerData[player.userID].playerPos.y = 995;
            player.Teleport(cachedPlayerData[player.userID].playerPos);


            cachedPlayerData[player.userID].teleportTimer = timer.Every(1f, () =>
            {
                player.Teleport(cachedPlayerData[player.userID].playerPos);
            });
        }

        private void StopFreezeTimer(BasePlayer player)
        {
            PlayerData.LoadCachedData(player.userID); //Load Data

            cachedPlayerData[player.userID].playerPos = player.transform.position;
            //Pre Teleport Down to ground
            cachedPlayerData[player.userID].playerPos.y = cachedPlayerData[player.userID].playerPosZ_Cached;
            player.Teleport(cachedPlayerData[player.userID].playerPos);

            cachedPlayerData[player.userID].teleportTimer.Destroy();
        }
        #endregion
    }
}
