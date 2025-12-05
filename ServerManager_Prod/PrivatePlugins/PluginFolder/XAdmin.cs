using Network;
using Oxide.Core.Plugins;
using Oxide.Game.Rust.Cui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Oxide.Plugins
{
    [Info("XAdmin", "Infinitynet.dk", "1.0.0")]
    [Description("Copyright Infinitynet.dk All Rights Reserved")]
    public class XAdmin : RustPlugin
    {
        //Requires Vanish
        [PluginReference]
        Plugin Vanish;




        #region Inventory SET GET
        static Dictionary<ulong, PlayerInventory> cachedInventories = new Dictionary<ulong, PlayerInventory>();

        private class PlayerInventory
        {
            public List<CustomItem> Items = new List<CustomItem>();

            internal static void CacheInventory(ulong id)
            {
                if (!cachedInventories.ContainsKey(id)) cachedInventories.Add(id, new PlayerInventory());
            }
        }

        private class CustomItem
        {
            public Item item;

            public ItemContainer container;

            public int position;
            public float condition;
            public float maxCondition;
            public int quantity;
            public ulong skin;

            public int blueprint;


            //Bullet Ammo Specfic
            public int ammo_quantity;
            public ItemDefinition ammotype;
            public List<string> mods = new List<string>();
        }


        CustomItem ProcessItem(Item item, ItemContainer container)
        {
            CustomItem cachedItem = new CustomItem();
            cachedItem.container = container;
            cachedItem.item = item;
            cachedItem.position = item.position;


            cachedItem.condition = item.condition;
            cachedItem.maxCondition = item.maxCondition;

            cachedItem.quantity = item.amount;
            cachedItem.skin = item.skin;
            cachedItem.blueprint = item.blueprintTarget;


            //Bullet/Arrow/Nail Ammo Specfic
            if (item.GetHeldEntity() is BaseProjectile)
            {
                cachedItem.ammotype = (item.GetHeldEntity() as BaseProjectile).primaryMagazine.ammoType;
                cachedItem.ammo_quantity = (item.GetHeldEntity() as BaseProjectile).primaryMagazine.contents;
            }

            //Lowgrade FlameThrower Specfic
            if (item.GetHeldEntity() is FlameThrower)
            {
                cachedItem.ammo_quantity = (item.GetHeldEntity() as FlameThrower).ammo;
            }

            //Weapon Mods SET
            try
            {
                foreach (var mod in item.contents.itemList)
                {
                    cachedItem.mods.Add(mod.info.shortname);
                }
            }
            catch { }


            return cachedItem;
        }


        //Main Functions
        private void SaveInventory(BasePlayer player)
        {
            ClearCachedInventory(player);

            PlayerInventory.CacheInventory(player.userID);

            //Main
            foreach (Item item in player.inventory.containerMain.itemList)
            {
                var i = ProcessItem(item, player.inventory.containerMain);
                cachedInventories[player.userID].Items.Add(i);
            }

            //Belt
            foreach (Item item in player.inventory.containerBelt.itemList)
            {
                var i = ProcessItem(item, player.inventory.containerBelt);
                cachedInventories[player.userID].Items.Add(i);
            }
            //Wear
            foreach (Item item in player.inventory.containerWear.itemList)
            {
                var i = ProcessItem(item, player.inventory.containerWear);
                cachedInventories[player.userID].Items.Add(i);
            }

            ClearInventory(player);
        }

        private void LoadInventory(BasePlayer player)
        {
            ClearInventory(player);

            PlayerInventory.CacheInventory(player.userID);


            foreach (var cachedItem in cachedInventories[player.userID].Items)
            {

                Item newItem = ItemManager.CreateByName(cachedItem.item.info.shortname, cachedItem.quantity, cachedItem.skin);
                newItem.condition = cachedItem.condition;
                newItem.maxCondition = cachedItem.maxCondition;
                newItem.blueprintTarget = cachedItem.blueprint;

                //Bullet/Arrow/Nail Ammo Specfic
                if (newItem.GetHeldEntity() is BaseProjectile)
                {
                    (newItem.GetHeldEntity() as BaseProjectile).primaryMagazine.ammoType = cachedItem.ammotype;
                    (newItem.GetHeldEntity() as BaseProjectile).primaryMagazine.contents = cachedItem.ammo_quantity;
                }


                //Lowgrade FlameThrower Specfic
                if (newItem.GetHeldEntity() is FlameThrower)
                {
                    (newItem.GetHeldEntity() as FlameThrower).ammo = cachedItem.ammo_quantity;
                }

                //Give Item
                player.inventory.GiveItem(newItem, cachedItem.container);
                newItem.MoveToContainer(cachedItem.container, cachedItem.position);


                //Weapon Mods GET
                try
                {
                    foreach (var mod in cachedItem.mods)
                    {
                        var item = ItemManager.CreateByName(mod, 1);
                        item.MoveToContainer(newItem.contents);
                    }
                }
                catch { }
            }

            ClearCachedInventory(player);
        }




        //Extra Functions
        private void ClearInventory(BasePlayer player) { foreach (Item i in player.inventory.AllItems()) i.Remove(); }
        private void ClearCachedInventory(BasePlayer player)
        {
            if (cachedInventories.ContainsKey(player.userID))
            {
                cachedInventories.Remove(player.userID);
            }
        }
        #endregion






        #region Play Sound
        private void SendEffect(BasePlayer player, string prefab)
        {
            var effect = new Effect(prefab, player, 0, Vector3.zero, Vector3.forward);
            EffectNetwork.Send(effect, player.net.connection);

            effect.Clear();
        }
        #endregion


     

        #region MenuUI

        private void MenuUI_TryReDrawUI(BasePlayer player)
        {
            if (AdminMode[player.userID].AdminMenuIsOpen)
            {
                MenuUI_DestoryUI(player);
                MenuUI_DrawUI(player);
            }
        }

        private void MenuUI_DestoryUI(BasePlayer player)
        {
            CuiHelper.DestroyUi(player, "AdminPanelUI_Menu");
            CuiHelper.DestroyUi(player, "AdminPanelUI_Button");

        }


        private void MenuUI_DrawUI(BasePlayer player)
        {
            const float ButtonSize = 246;

            CuiElementContainer elements = new CuiElementContainer();

            #region Main Panel

            string MainPanel = elements.Add(new CuiPanel
            {
                Image = { Color = "0.1 0.1 0.1 0.9" },
                RectTransform =
                {
                    AnchorMin = "0 0",
                    AnchorMax = "0.335 0.107"

                },
                CursorEnabled = false
            }, "Overlay", "AdminPanelUI_Menu");


            //Line
            elements.Add(new CuiElement
            {
                Parent = MainPanel,
                Components =
                {
                    new CuiImageComponent
                    {
                        Color = "0.2 0.4 0.6 1"
                    },

                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0 0",
                        AnchorMax = "0.9975 0"
                    }
                }
            });
            #endregion



            //Button1
            #region Button1 Toggle Admin Powers/Cheat
            elements.Add(new CuiElement
            {
                Name = "Button1",
                Parent = MainPanel,
                Components =
                {
                    new CuiButtonComponent
                    {
                        Color = "0.5 0 0 1",
                        Command = "chat.say /admin",
                    },

                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0.008 0.53",
                        AnchorMax = $"{ButtonSize/1000f} 0.93",
                    }

                }

            });
            //Text
            elements.Add(new CuiElement
            {
                Name = "Button1Text",
                Parent = "Button1",
                Components =
                {
                    new CuiTextComponent
                    {
                        Text = "Enable AdminPowers",
                        Color = "1 1 1 1",
                        Align = TextAnchor.MiddleCenter,
                        FontSize = 14,
                    },
                }
            });
            #endregion

            //Button2
            #region Button2
            elements.Add(new CuiElement
            {
                Name = "Button2",
                Parent = MainPanel,
                Components =
                {
                    new CuiButtonComponent
                    {
                        Color = "0.5 0 0 1",
                        Command = "chat.say /admin",
                    },

                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0.2555 0.53",
                        AnchorMax = $"{0.2550+ButtonSize/1000f-0.0075} 0.93",
                    }

                }

            });
            //Text
            elements.Add(new CuiElement
            {
                Name = "Button2Text",
                Parent = "Button2",
                Components =
                {
                    new CuiTextComponent
                    {
                        Text = "Button2",
                        Color = "1 1 1 1",
                        Align = TextAnchor.MiddleCenter,
                        FontSize = 14,
                    },
                }
            });
            #endregion

            //Button3
            #region Button3
            elements.Add(new CuiElement
            {
                Name = "Button3",
                Parent = MainPanel,
                Components =
                {
                    new CuiButtonComponent
                    {
                        Color = "0.5 0 0 1",
                        Command = "chat.say /admin",
                    },

                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0.503 0.53",
                        AnchorMax = $"{0.503+ButtonSize/1000f-0.0075} 0.93",
                    }

                }

            });
            //Text
            elements.Add(new CuiElement
            {
                Name = "Button3Text",
                Parent = "Button3",
                Components =
                {
                    new CuiTextComponent
                    {
                        Text = "Button3",
                        Color = "1 1 1 1",
                        Align = TextAnchor.MiddleCenter,
                        FontSize = 14,
                    },
                }
            });
            #endregion

            //Button4
            #region Button4
            elements.Add(new CuiElement
            {
                Name = "Button4",
                Parent = MainPanel,
                Components =
                {
                    new CuiButtonComponent
                    {
                        Color = "0.5 0 0 1",
                        Command = "chat.say /admin",
                    },

                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0.751 0.53",
                        AnchorMax = $"{0.751+ButtonSize/1000f-0.0075} 0.93",
                    }

                }

            });
            //Text
            elements.Add(new CuiElement
            {
                Name = "Button4Text",
                Parent = "Button4",
                Components =
                {
                    new CuiTextComponent
                    {
                        Text = "Button4",
                        Color = "1 1 1 1",
                        Align = TextAnchor.MiddleCenter,
                        FontSize = 14,
                    },
                }
            });
            #endregion

            //Button5
            #region Button5
            elements.Add(new CuiElement
            {
                Name = "Button5",
                Parent = MainPanel,
                Components =
                {
                    new CuiButtonComponent
                    {
                        Color = "0.5 0 0 1",
                        Command = "chat.say /admin",
                    },

                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0.008 0.0675",
                        AnchorMax = $"{ButtonSize/1000f} 0.46",
                    }

                }

            });
            //Text
            elements.Add(new CuiElement
            {
                Name = "Button5Text",
                Parent = "Button5",
                Components =
                {
                    new CuiTextComponent
                    {
                        Text = "Button5",
                        Color = "1 1 1 1",
                        Align = TextAnchor.MiddleCenter,
                        FontSize = 14,
                    },
                }
            });
            #endregion












            #region Button
            elements.Add(new CuiElement
            {
                Name = "Button6",
                Parent = MainPanel,
                Components =
                {
                    new CuiButtonComponent
                    {
                        Color = "0.5 0 0 1",
                        Command = "chat.say /admin",
                    },

                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0.2555 0.0675",
                        AnchorMax = $"{0.2555+ButtonSize/1000f-0.0075} 0.46",
                    }
                }
            });
            //Text
            elements.Add(new CuiElement
            {
                Name = "Button6Text",
                Parent = "Button6",
                Components =
                {
                    new CuiTextComponent
                    {
                        Text = "Button6",
                        Color = "1 1 1 1",
                        Align = TextAnchor.MiddleCenter,
                        FontSize = 14,
                    },
                }
            });
            #endregion

            #region Button
            elements.Add(new CuiElement
            {
                Name = "Button7",
                Parent = MainPanel,
                Components =
                {
                    new CuiButtonComponent
                    {
                        Color = "0.5 0 0 1",
                        Command = "chat.say /admin",
                    },

                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0.503 0.0675",
                        AnchorMax = $"{0.503+ButtonSize/1000f-0.0075} 0.46",
                    }
                }
            });
            //Text
            elements.Add(new CuiElement
            {
                Name = "Button7Text",
                Parent = "Button7",
                Components =
                {
                    new CuiTextComponent
                    {
                        Text = "Button7",
                        Color = "1 1 1 1",
                        Align = TextAnchor.MiddleCenter,
                        FontSize = 14,
                    },
                }
            });
            #endregion

            #region Button
            elements.Add(new CuiElement
            {
                Name = "Button8",
                Parent = MainPanel,
                Components =
                {
                    new CuiButtonComponent
                    {
                        Color = "0.5 0 0 1",
                        Command = "chat.say /admin",
                    },

                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0.751 0.0675",
                        AnchorMax = $"{0.751+ButtonSize/1000f-0.0075} 0.46",
                    }
                }
            });
            //Text
            elements.Add(new CuiElement
            {
                Name = "Button8Text",
                Parent = "Button8",
                Components =
                {
                    new CuiTextComponent
                    {
                        Text = "Button8",
                        Color = "1 1 1 1",
                        Align = TextAnchor.MiddleCenter,
                        FontSize = 14,
                    },
                }
            });
            #endregion





            #region OLD
            /*

            #region Main Container

            string MainPanel = elements.Add(new CuiPanel
            {
                Image = { Color = "0.1 0.1 0.1 0.9" },
                RectTransform =
                {
                    AnchorMin = "0 0",
                    AnchorMax = "0.3405 0.1066"

                },
                CursorEnabled = false
            }, "Overlay", "AdminPanelUI_Menu");


            elements.Add(new CuiElement
            {
                Parent = MainPanel,
                Components =
                {
                    new CuiImageComponent { Color = "0.2 0.4 0.6 1"},
                    new CuiRectTransformComponent { AnchorMin = "0 0", AnchorMax = "1 0.02", OffsetMax = "0 0.1"},
                }
            });
            #endregion

            #region Buttons

            //Exit Admin Mode Button
            elements.Add(new CuiButton
            {
                Button = { Command = "chat.say /admin", Color = "0.5 0 0 1" },
                RectTransform = { OffsetMin = "5 7", OffsetMax = "10 18", AnchorMin = "0 0", AnchorMax = "0.215 0.25" },
                Text = { Text = "Exit Admin Mode", Align = TextAnchor.MiddleCenter }

            }, MainPanel, "AdminPanelUI_Button");




            //Admin Cheat Button
            if (!AdminMode[player.userID].AdminCheatsEnabled)
            {
                //Enable Admin Cheats Button
                elements.Add(new CuiButton
                {
                    Button = { Command = "chat.say /admin", Color = "0 0.5 0 1" },
                    RectTransform = { OffsetMin = "5 3", OffsetMax = "10 14", AnchorMin = "0 0.5", AnchorMax = "0.215 0.75" },
                    Text = { Text = "Enable Powers", Align = TextAnchor.MiddleCenter }

                }, MainPanel, "AdminPanelUI_Button");
            }
            else
            {
                //Enable Admin Cheats Button
                elements.Add(new CuiButton
                {
                    Button = { Command = "chat.say /admin", Color = "0.5 0 0 1" },
                    RectTransform = { OffsetMin = "5 3", OffsetMax = "10 14", AnchorMin = "0 0.5", AnchorMax = "0.215 0.75" },
                    Text = { Text = "Disable Powers", Align = TextAnchor.MiddleCenter }

                }, MainPanel, "AdminPanelUI_Button");
            }








            //Vanish Button (Third Party)
            if (AdminMode[player.userID].Vanish_PlayerVisible)
            {
                //Enable Admin Cheats Button
                elements.Add(new CuiButton
                {
                    Button = { Command = "chat.say /vanish", Color = "0 0.5 0 1" },
                    RectTransform = { OffsetMin = "140.5 3", OffsetMax = "145.5 14", AnchorMin = "0 0.5", AnchorMax = "0.215 0.75" },
                    Text = { Text = "Enable Invisibility", Align = TextAnchor.MiddleCenter }

                }, MainPanel, "AdminPanelUI_Button");
            }
            else
            {
                //Enable Admin Cheats Button
                elements.Add(new CuiButton
                {
                    Button = { Command = "chat.say /vanish", Color = "0.5 0 0 1" },
                    RectTransform = { OffsetMin = "140.5 3", OffsetMax = "145.5 14", AnchorMin = "0 0.5", AnchorMax = "0.215 0.75" },
                    Text = { Text = "Disable Invisibility", Align = TextAnchor.MiddleCenter }

                }, MainPanel, "AdminPanelUI_Button");
            }






            #endregion

            */
            #endregion

            CuiHelper.AddUi(player, elements);

        }
        #endregion



        #region Admin Mode Toggle
        static Dictionary<ulong, AdminModeData> AdminMode = new Dictionary<ulong, AdminModeData>();

        private class AdminModeData
        {
            public bool Vanish_PlayerVisible = false;
            public bool AdminMenuIsOpen = false;


            public bool AdminModeEnabled = false;
            public bool AdminCheatsEnabled = false;
            internal static void CacheAdminMode(ulong id)
            {
                if (!AdminMode.ContainsKey(id)) AdminMode.Add(id, new AdminModeData());
            }
        }

        private void ToggleAdminMode(BasePlayer player)
        {
            AdminModeData.CacheAdminMode(player.userID);

            if (AdminMode[player.userID].AdminModeEnabled)
            {
                SaveInventory(player);

                SetAdminOutfit(player);

                LockContainer(player.inventory.containerWear);

                AdminMode[player.userID].AdminModeEnabled = false;

                MenuUI_DrawUI(player);

                AdminMode[player.userID].AdminMenuIsOpen = true;
            }
            else
            {

                UnLockContainer(player.inventory.containerWear);


                LoadInventory(player);

                AdminMode[player.userID].AdminModeEnabled = true;

                MenuUI_DestoryUI(player);

                AdminMode[player.userID].AdminMenuIsOpen = false;
            }
        }
        #endregion





        [ChatCommand("admin")]
        private void ToggleAdminModeCOMMAND(BasePlayer player)
        {
            SendEffect(player, "assets/prefabs/misc/easter/painted eggs/effects/eggpickup.prefab");


            ToggleAdminMode(player);

        }












        #region Admin Outfit
        private void LockContainer(ItemContainer container) => container.SetLocked(true);
        private void UnLockContainer(ItemContainer container) => container.SetLocked(false);

        private void SetAdminOutfit(BasePlayer player)
        {
            Item hoodie_Item = ItemManager.CreateByName("hoodie", 1, 2552347289);
            Item pants_Item = ItemManager.CreateByName("pants", 1, 2552349192);
            Item shoes_Item = ItemManager.CreateByName("shoes.boots", 1, 2552351142);
            Item gloves_HoodieItem = ItemManager.CreateByName("burlap.gloves", 1, 2552352502);
            Item mask_HoodieItem = ItemManager.CreateByName("mask.bandana", 1, 2552417976);
            Item hat_HoodieItem = ItemManager.CreateByName("riot.helmet", 1, 2552419407);

            player.inventory.GiveItem(hat_HoodieItem);
            player.inventory.GiveItem(mask_HoodieItem);
            player.inventory.GiveItem(hoodie_Item);
            player.inventory.GiveItem(pants_Item);
            player.inventory.GiveItem(shoes_Item);
            player.inventory.GiveItem(gloves_HoodieItem);



            hoodie_Item.MoveToContainer(player.inventory.containerWear);
            pants_Item.MoveToContainer(player.inventory.containerWear);
            shoes_Item.MoveToContainer(player.inventory.containerWear);
            gloves_HoodieItem.MoveToContainer(player.inventory.containerWear);
            mask_HoodieItem.MoveToContainer(player.inventory.containerWear);
            hat_HoodieItem.MoveToContainer(player.inventory.containerWear);
        }
        #endregion





















        private void TeleportPlayer(BasePlayer player, Vector3 Location) => player.Teleport(Location);












        #region Third Party Plugins Hooks

        //Vanish
        void OnVanishReappear(BasePlayer player)
        {
            if (Vanish)
            {

                AdminModeData.CacheAdminMode(player.userID);
                AdminMode[player.userID].Vanish_PlayerVisible = true;
                MenuUI_TryReDrawUI(player);
            }

        }
        void OnVanishDisappear(BasePlayer player)
        {
            if (Vanish)
            {

                AdminModeData.CacheAdminMode(player.userID);
                AdminMode[player.userID].Vanish_PlayerVisible = false;
                MenuUI_TryReDrawUI(player);
            }
        }

        #endregion


    }
}