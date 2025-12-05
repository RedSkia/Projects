
using ConVar;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Oxide.Core;
using Oxide.Core.Configuration;
using Oxide.Core.Libraries;
using Oxide.Core.Libraries.Covalence;
using Oxide.Core.Plugins;
using Oxide.Game.Rust.Cui;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;

#region Functionality Information
#endregion Functionality Information

namespace Oxide.Plugins
{

    [Info("XLib", "InfinityNet/Xray", "1.0.0")]
    [Description("Copyright Infinitynet.dk All Rights Reserved")]
    public class OLD_XLib : RustPlugin
    {

        #region Setup Configuration
        private static Configuration config { get; set; }
        private class Configuration
        {
            public Security security { get; set; }
            public class Security
            {
                public Groups groups { get; set; }
                public class Groups
                {
                    public string[] Executives =
                    {
                        "CEO",
                        "Executive"
                    };

                    public string[] Internal =
                    {
                        "Developer",
                        "Designer",
                        "Support"
                    };
                    public string[] External =
                    {
                        "HeadAdministrator",
                        "Administrator",
                        "SeniorModerator",
                        "Moderator"
                    };
                }

                public Ranks ranks { get; set; }
                public class Ranks
                {
                    public string CEO = "CEO";
                    public string Executive = "Executive";

                    public string Developer = "Developer";
                    public string Designer = "Designer";
                    public string Support = "Support";

                    public string HeadAdministrator = "HeadAdministrator";
                    public string Administrator = "Administrator";
                    public string SeniorModerator = "SeniorModerator";
                    public string Moderator = "Moderator";
                }
            }
        }



        #endregion Setup Configuration


        #region Core

        private static OLD_XLib Instance;
        private const bool DebugMode = false;



        public static readonly string[] PrivatePlugins = {
            "XTemplate"
        };


        private void SqlReference()
        {
            const string host = "localhost";

            SqlConnection connection = new SqlConnection("Server=localhost;Database=world;User Id=root;Password=2002;");
            connection.Open();
            PrintWarning("worked!");
        }

        private void Loaded()
        {
            Instance = this;

            timer.Once(1f, () =>
            {
                PrintWarning($"-------------------------{Name} {Version}-------------------------");
                ReferencePlugins();
                ReferenceWebRequests();
                timer.Once(0.2f, () =>
                {
                    PrintWarning("------------------------------------------------------------");
                });
            });



        }

        private void ReferenceWebRequests()
        {
            OxideSecurity.Caller.SetStaffData();

        }

        private void ReferencePlugins()
        {
            List<string> ServerPluginsList = new List<string>();
            foreach (var plugin in plugins.GetAll())
            {
                ServerPluginsList.Add(plugin.Name);
                if (PrivatePlugins.Contains(plugin.Name))
                {
                    Puts($"PLUGIN | LOADED | {plugin.Name}.cs | Version ({plugin.Version}) | By {plugin.Author.Replace("InfinityNet/", "")}");
                }

            }


            foreach (var PluginName in PrivatePlugins)
            {
                if (!ServerPluginsList.Contains(PluginName))
                {
                    PrintError($"PLUGIN | MISSING | {PluginName}.cs");
                }
            }




            //else { PrintError($"{PluginName}.cs MISSING - Contact The Server Administrator"); }


        }
        #endregion





        [ChatCommand("mount")]
        void testCommand8(BasePlayer player) => UI.LockPlayer.Mount(player);
        [ChatCommand("dis")]
        void testCommand9(BasePlayer player) => UI.LockPlayer.Dismount(player);







        [ChatCommand("d")]
        void testCommand2(BasePlayer player)
        {
            CuiHelper.DestroyUi(player, "Test");

        }


        private void ShowPopup(BasePlayer player, string Message, int Duration)
        {
            player.SendConsoleCommand($"gametip.showgametip \"{Message}\"");
            timer.Once(Duration, () => { player.SendConsoleCommand("gametip.hidegametip"); });

        }


        public class OxideSecurity
        {
            public static OxideSecurity Caller = new OxideSecurity();
            public bool BelongsToGroup(BasePlayer player, string group) => player.IPlayer.BelongsToGroup(group);
            public bool HasPermission(BasePlayer player, string permission) => player.IPlayer.HasPermission(permission);


            #region Staff Web Request 
            public class WebCall
            {
                public static Dictionary<ulong, StaffData> webData = new Dictionary<ulong, StaffData>();
                public class StaffData
                {
                    public string Rank { get; set; }
                    public ulong SteamID { get; set; }
                    public string Name { get; set; }
                    public string PhotoUrl { get; set; }
                }
            }
            public void SetStaffData()
            {
                OxideSecurity.WebCall.webData.Clear();
                string requestPath = "http://localhost/websitev2/api/StaffData.json";
                Instance.webrequest.Enqueue(requestPath, null, (code, response) =>
                {
                    if (code != 200 || response == null) { Instance.PrintError($"WEB | MISSING | {requestPath.ToLower().Replace("http://", "").Replace("https://", "")}"); return; }
                    Instance.Puts($"WEB | LOADED | {requestPath.ToLower().Replace("http://", "").Replace("https://", "")}");

                    JObject json = JObject.Parse(response);

                    foreach (var team in JsonConvert.DeserializeObject<Dictionary<string, object>>(json["StaffList"].ToString()))
                    {
                        foreach (var person in JsonConvert.DeserializeObject<Dictionary<string, object>>(json["StaffList"][team.Key].ToString()))
                        {
                            var rank = (string)json["StaffList"][team.Key][person.Key]["rank"];
                            var key = (ulong)json["StaffList"][team.Key][person.Key]["steamid"];
                            var name = (string)json["StaffList"][team.Key][person.Key]["name"];
                            var photo = (string)json["StaffList"][team.Key][person.Key]["photo"];

                            if (!OxideSecurity.WebCall.webData.ContainsKey(key))
                            {
                                OxideSecurity.WebCall.webData.Add(key, new OxideSecurity.WebCall.StaffData { Rank = rank, SteamID = key, Name = name, PhotoUrl = photo });
                            }
                        }
                    }
                }, Instance, RequestMethod.GET, null, 1f);
            }


            //SetStaffData() - Must be SET before calling this
            public bool IsStaff(ulong SteamID)
            {
                return OxideSecurity.WebCall.webData.ContainsKey(SteamID) ? true : false;
            }




            #endregion Staff Web Request 
        }



        [ChatCommand("web")]
        void WebTestCommand(BasePlayer player)
        {
            if (OxideSecurity.Caller.IsStaff(player.userID))
            {
                SendReply(player, OxideSecurity.WebCall.webData[player.userID].Rank);
                SendReply(player, OxideSecurity.WebCall.webData[player.userID].SteamID.ToString());
                SendReply(player, OxideSecurity.WebCall.webData[player.userID].Name);
                SendReply(player, OxideSecurity.WebCall.webData[player.userID].PhotoUrl);
            }
        }

        public class AdminTools
        {


            public static AdminTools Caller = new AdminTools();



            public void SetAdminOutfit(BasePlayer player)
            {


                Item head = ItemManager.CreateByName("burlap.headwrap", 1, 2691239227); player.inventory.GiveItem(head); head.MoveToContainer(player.inventory.containerWear, 0);
                Item hoodie = ItemManager.CreateByName("hoodie", 1, 2691236552); player.inventory.GiveItem(hoodie); hoodie.MoveToContainer(player.inventory.containerWear, 1);
                Item pants = ItemManager.CreateByName("pants", 1, 2691240646); player.inventory.GiveItem(pants); pants.MoveToContainer(player.inventory.containerWear, 2);
                Item boots = ItemManager.CreateByName("shoes.boots", 1, 2691244153); player.inventory.GiveItem(boots); boots.MoveToContainer(player.inventory.containerWear, 3);
                Item gloves = ItemManager.CreateByName("burlap.gloves", 1, 2691242718); player.inventory.GiveItem(gloves); gloves.MoveToContainer(player.inventory.containerWear, 4);
            }

            public void ToggleAdminMode(BasePlayer player)
            {

            }

            public void DisableRecoil(BasePlayer player)
            {
                var item = ItemManager.CreateByName("rifle.lr300", 1, 2692113448);
                var mag = item.GetHeldEntity() is BaseProjectile ? (item.GetHeldEntity() as BaseProjectile).primaryMagazine : null;
                mag.capacity = 100000;
                mag.contents = 100000;
                player.GiveItem(item);
            }
        }

        [ChatCommand("boom")]
        void testCommand50(BasePlayer player)
        {
            AdminTools.Caller.SetAdminOutfit(player);
        }


        void OnWeaponFired(BaseProjectile projectile, BasePlayer player, ItemModProjectile mod, ProtoBuf.ProjectileShoot projectiles)
        {




            var item = player.GetActiveItem();
            item.condition = 100;
            projectile.damageScale = 10;
            projectile.aimSwaySpeed = 10000;



        }






        public class DataFormat
        {
            public static string FormatHours(int Hours) { return Hours == -1 ? "Permanent" : Hours < 24 ? "hour(s)" : Hours >= 24 && Hours < 24 * 7 ? "day(s)" : Hours >= 24 * 7 && Hours < 30 * 24 ? "week(s)" : Hours >= 30 * 24 ? "month(s)" : null; }
        }

        [ChatCommand("r")]
        void RemoveimgCommand(BasePlayer player)
        {
            CuiHelper.DestroyUi(player, "MainImageContainer");
        }



        [ConsoleCommand("InputFieldButtom")]
        private void OnInputFieldButtonClose(ConsoleSystem.Arg arg) //Input Field Close Delete
        {
            if (arg == null) { return; }
            var player = arg.Player();
            SendReply(player, "CLICK");
            UI.DestroyUI($"InputFieldPanel{arg.Args[0]}", player);
        }

        [ConsoleCommand("OnInputFieldSubmit")]
        private void OnInputFieldSubmit(ConsoleSystem.Arg arg) //Draw Label Overlay on Input Field
        {

            if (arg != null && arg.HasArgs(2))
            {
                var player = arg.Player();
                var indexKey = $"InputField{arg.Args[0]}";

                List<string> textInput = new List<string>();
                for (int i = 0; i < arg.Args.Count(); i++)
                {
                    if (i >= 1)
                    {
                        if (!textInput.Contains(arg.Args[i])) { textInput.Add(arg.Args[i]); }
                    }
                }

                CuiElementContainer container = new CuiElementContainer();
                double buttonWidth = 0.15;
                UI.CreatePanel(container, indexKey, $"InputFieldPanel{arg.Args[0]}", UI.colors.Gray30, 1f, 0f, new UI.CuiPoint(1, 1, 0, 0));
                UI.CreateButton(container, $"InputFieldPanel{arg.Args[0]}", $"InputFieldButtom {arg.Args[0]}", UI.colors.Red, 1f, 0f, false, "X", "ffffff", 20, TextAnchor.MiddleCenter, new UI.CuiPoint(buttonWidth, 1, 0.85, 0));
                UI.CreateLabel(container, $"InputFieldPanel{arg.Args[0]}", string.Join(" ", textInput), TextAnchor.MiddleLeft, "ffffff", 14, new UI.CuiPoint(Math.Abs(buttonWidth - 1) - 0.01, 1, 0.01, 0));
                UI.DrawUI(container, player);
            }
        }

        [ChatCommand("i")]
        void testCommand100(BasePlayer player)
        {
            try { UI.DestroyUI("Test", player); UI.DestroyUI("InputField1", player); } catch { }
            CuiElementContainer container = UI.CreateElementContainer("Test", 0, UI.colors.Gray20, 1f, 0f, true, new UI.CuiPoint(0.2, 0.2, 0, 0));


            UI.CreateInputField(container, "Test", 1, UI.colors.Gray30, 1f, TextAnchor.MiddleCenter, -1, "ffffff", 15, false, "OnInputFieldSubmit 1", new UI.CuiPoint(1, 0.2, 0, 0));
            UI.CreateInputField(container, "Test", 2, UI.colors.Gray30, 1f, TextAnchor.MiddleCenter, -1, "ffffff", 15, false, "OnInputFieldSubmit 2", new UI.CuiPoint(1, 0.2, 0, 0.5));

            UI.DrawUI(container, player);

        }





        [ChatCommand("testUI")]
        private void TestUICommand(BasePlayer player)
        {
            CuiElementContainer elements = new CuiElementContainer();
            elements.Add(new CuiPanel
            {
                Image = { Color = "0.2 0.4 0.6 1" },
                RectTransform = {
                    AnchorMin = "0 0",
                    AnchorMax = "0 0",
                    OffsetMin = "100 0",
                    OffsetMax = "100 0"
                },
                CursorEnabled = false
            }, "Overlay", "Test");





            CuiHelper.AddUi(player, elements);
        }


        [ChatCommand("die2")]
        private void DieUICommand2(BasePlayer player)
        {
            UI.DestroyUI("Test", player);
        }

        #region Image Upload
        void OnPlayerLootEnd(PlayerLoot inventory) //When Looting stops Try DestroyUI & Close Containers
        {
            var player = (BasePlayer)inventory.gameObject.ToBaseEntity();
            try { UploadImage.Caller.DestroyUI(player); } catch { }
        }

        #region Commands
        [ConsoleCommand("xlib.upload.submit.button")]
        private void OnImageUploadSubmit(ConsoleSystem.Arg arg)
        {
            var player = arg.Player();

            ImageContainers[player.userID][0].inventory.SetLocked(true);

            if (UploadImage.Caller.TryUploadPicture(player) <= 0)
            {
                UploadImage.Caller.UploadFailed(player);

                SoundDictionary.PlayError(player);
            }
            else if (UploadImage.Caller.TryUploadPicture(player) is uint)
            {
                SoundDictionary.PlaySound(player, SoundDictionary.sounds.Submit);
                SoundDictionary.PlaySound(player, SoundDictionary.sounds.Star);

                var ImageID = UploadImage.Caller.TryUploadPicture(player); //Image ID used for Png field in CUI Image
                if (DebugMode) { PlayerManagement.Chat.DirectMessage(player, $"{ImageID}"); }
                UploadImage.Caller.UploadSuccess(player);
            }
        }

        [ConsoleCommand("xlib.upload.cancel.button")]
        private void OnImageUploadCancel(ConsoleSystem.Arg arg)
        {
            var player = arg.Player();
            SoundDictionary.PlaySound(player, SoundDictionary.sounds.Beeb);
            UploadImage.Caller.RemoveContainer(player);
            UploadImage.Caller.DestroyUI(player);
        }
        [ConsoleCommand("xlib.upload")]
        private void OnImageUpload(ConsoleSystem.Arg arg)
        {
            if (arg == null) { return; }
            var player = arg.Player();
            UploadImage.Caller.RemoveContainer(player);
            UploadImage.Caller.DestroyUI(player);
            UploadImage.Caller.GiveCamera(player);
        }

        [ChatCommand("die")]
        private void DieCommand(BasePlayer player) => UploadImage.Caller.DestroyUI(player);

        [ChatCommand("upload")]
        private void ItemContainerCommand(BasePlayer player) => player.SendConsoleCommand("xlib.upload");
        #endregion Commands

        public static Dictionary<ulong, List<StorageContainer>> ImageContainers = new Dictionary<ulong, List<StorageContainer>>();
        public static List<ulong> ActiveImageMessageUI = new List<ulong>();
        public class UploadImage
        {
            public static UploadImage Caller = new UploadImage();

            private const string prefab = "assets/prefabs/deployable/large wood storage/box.wooden.large.prefab";

            Timer CameraTimer = null;
            public void GiveCamera(BasePlayer player)
            {
                try { CameraTimer.Destroy(); } catch { }
                List<Item> Items = new List<Item>();
                player.inventory.GetAllItems(Items);
                if (Items.FirstOrDefault(x => x.info.shortname == "photo") == null) //Not Has Picture
                {
                    var CameraItem = ItemManager.CreateByName("tool.instant_camera", 1);
                    player.inventory.GiveItem(CameraItem);

                    CameraTimer = Instance.timer.Every(1f, () =>
                    {
                        if (Items.FirstOrDefault(x => x.info.shortname == "photo") != null) //Has Picture
                        {
                            try { /*player.inventory.FindItemID("tool.instant_camera").Remove();*/ } catch { }
                            UploadImage.Caller.SpawnContainer(player);
                            CameraTimer.Destroy();
                        }
                    });
                }
                else if (Items.FirstOrDefault(x => x.info.shortname == "photo") != null) //Has Picture
                {
                    try { CameraTimer.Destroy(); } catch { }

                    Instance.timer.Once(0.1f, () =>
                    {
                        try { /*player.inventory.FindItemID("tool.instant_camera").Remove();*/ } catch { }
                        UploadImage.Caller.SpawnContainer(player);
                    });
                }
            }

            public void SpawnContainer(BasePlayer player)
            {
                player.EndLooting();

                StorageContainer container = GameManager.server.CreateEntity(prefab, new Vector3(Vector3.zero.x, Vector3.zero.y - 16, Vector3.zero.z)) as StorageContainer;

                container.enableSaving = false;
                container.syncPosition = false;
                container.Spawn();
                container.inventory.capacity = 1;
                container.OwnerID = player.userID;

                if (ImageContainers.ContainsKey(player.userID))
                {
                    ImageContainers[player.userID].Add(container);
                }
                else
                {
                    ImageContainers.Add(player.userID, new List<StorageContainer>());
                    ImageContainers[player.userID].Add(container);
                }

                Instance.timer.Once(0.25f, () =>
                {
                    player.inventory.loot.SendImmediate();
                    container.PlayerOpenLoot(player, container.panelName, false);
                    UploadImage.Caller.DrawUI(player); //Draw UI
                });
            }
            public void RemoveContainer(BasePlayer player) //Remove all Image Containers
            {
                try
                {
                    foreach (var imgContainer in ImageContainers[player.userID])
                    {
                        if (imgContainer.IsAlive()) { imgContainer.Kill(); }
                    }
                    ImageContainers.Remove(player.userID);
                }
                catch { }
            }

            public void DrawUI(BasePlayer player)
            {
                //CuiElementContainer container = UI.CreateElementContainer($"{Instance.Name}_ImageUpload", 0, UI.colors.Empty, 1, 0, false, new UI.CuiPoint(0.09, 0.0805, 0.612, 0.1525));
                CuiElementContainer container = UI.CreateElementContainer($"{Instance.Name}_ImageUpload", 1, UI.colors.Green, 0.5f, 0f, false, new UI.CuiPoint(1, 1, 0, 0));
                UI.CreatePanel(container, $"{Instance.Name}_ImageUpload", $"{Instance.Name}_ImageUpload_ButtonPanel", UI.colors.Red, 1f, 0f, new UI.CuiPoint(0.09, 0.0805, 0.1, 0));
                UI.DrawUI(container, player);
                /*
                UI.CreateButton(container, $"{Instance.Name}_ImageUpload", "xlib.upload.submit.button", UI.colors.Green, 0.5f, 0f, false, "Upload", "ffffff", 14, TextAnchor.MiddleCenter, new UI.CuiPoint(0.5, 1, 0.5, 0));
                UI.CreateButton(container, $"{Instance.Name}_ImageUpload", "xlib.upload.cancel.button", UI.colors.Red, 0.5f, 0f, false, "Cancel", "ffffff", 14, TextAnchor.MiddleCenter, new UI.CuiPoint(0.5, 1, 0, 0));

                CuiElementContainer container2 = UI.CreateElementContainer($"{Instance.Name}_ImageUploadText", 0, "000000", 0f, 0f, false, new UI.CuiPoint(0.09, 0.029, 0.615, 0.2415));
                UI.CreateLabel(container2, $"{Instance.Name}_ImageUploadText", "Upload Picture", TextAnchor.MiddleLeft, "ffffff", 14, new UI.CuiPoint(1, 1, 0, 0));
                UI.DrawUI(container, player);
                UI.DrawUI(container2, player);
                */

            }
            public void DestroyUI(BasePlayer player)
            {
                UI.DestroyUI($"{Instance.Name}_ImageUpload", player);
                UI.DestroyUI($"{Instance.Name}_ImageUploadText", player);
                UI.DestroyUI($"{Instance.Name}_UploadFailed", player);
                UI.DestroyUI($"{Instance.Name}_UploadSuccess", player);
            }

            public void UploadFailed(BasePlayer player)
            {
                if (!ActiveImageMessageUI.Contains(player.userID))
                {
                    CuiElementContainer container = UI.CreateElementContainer($"{Instance.Name}_UploadFailed", 0, UI.colors.Gray20, 0.15f, 0f, false, new UI.CuiPoint(0.09, 0.029, 0.612, 0.1225));
                    UI.CreateLabel(container, $"{Instance.Name}_UploadFailed", "Upload Failed", TextAnchor.MiddleCenter, UI.colors.Red, 14, new UI.CuiPoint(1, 1, 0, 0));
                    UI.DrawUI(container, player);
                    ActiveImageMessageUI.Add(player.userID);
                }
                Instance.timer.Once(5f, () =>
                {
                    ActiveImageMessageUI.Remove(player.userID);
                    CuiHelper.DestroyUi(player, $"{Instance.Name}_UploadFailed");
                    ImageContainers[player.userID][0].inventory.SetLocked(false);
                });

            }
            public void UploadSuccess(BasePlayer player)
            {
                if (!ActiveImageMessageUI.Contains(player.userID))
                {
                    CuiElementContainer container = UI.CreateElementContainer($"{Instance.Name}_UploadSuccess", 0, UI.colors.RustUI, 0.15f, 0f, false, new UI.CuiPoint(0.09, 0.029, 0.612, 0.1225));
                    UI.CreateLabel(container, $"{Instance.Name}_UploadSuccess", "Upload Success", TextAnchor.MiddleCenter, UI.colors.Green, 14, new UI.CuiPoint(1, 1, 0, 0));
                    UI.DrawUI(container, player);
                    ActiveImageMessageUI.Add(player.userID);
                }
                Instance.timer.Once(5f, () =>
                {
                    ActiveImageMessageUI.Remove(player.userID);
                    CuiHelper.DestroyUi(player, $"{Instance.Name}_UploadSuccess");
                    ImageContainers[player.userID][0].inventory.SetLocked(false);
                    UploadImage.Caller.RemoveContainer(player);

                });
            }

            public uint TryUploadPicture(BasePlayer player)
            {
                if (ImageContainers.ContainsKey(player.userID))
                {
                    var img = ImageContainers[player.userID][0].inventory.itemList.First<Item>();

                    try
                    {
                        var photoEnt = BaseNetworkable.serverEntities.OfType<PhotoEntity>().FirstOrDefault(x => x.net.ID == img.instanceData.subEntity);
                        var photoBytes = FileStorage.server.Get(photoEnt.ImageCrc, FileStorage.Type.jpg, photoEnt.net.ID);

                        var ImageID = photoEnt.ImageCrc;
                        var ImageEntID = photoEnt.net.ID;

                        //æImageManagement.AddImage(player.userID, photoBytes, ImageID, ImageEntID, FileStorage.Type.jpg);

                        return ImageID; //Image ID used for Png field in CUI Image

                    }
                    catch { }

                }
                return 0;
            }
        }

        public class ImageManagement
        {
            public static void AddImage(ulong UploaderID, byte[] ImageBytes, uint ImageID, uint EntID, FileStorage.Type ImageType)
            {
                var FileDB = Interface.Oxide.DataFileSystem.ReadObject<Dictionary<uint, ImageManagement.ImageData>>("XLib/Images");
                if (!FileDB.ContainsKey(ImageID))
                {
                    FileDB.Add(ImageID, new ImageData
                    {
                        UploaderID = UploaderID,
                        ImageID = ImageID,
                        EntID = EntID,
                        ImageType = ImageType
                    });
                }

                //FileStorage.server.Store(ImageBytes, ImageType, ImageID);
                Interface.Oxide.DataFileSystem.WriteObject("XLib/Images", FileDB, false);
            }

            public static void RemoveImage(uint ImageID, FileStorage.Type ImageType, uint EntID)
            {
                //try { FileStorage.server.Remove(ImageID, ImageType, EntID); } catch { }
            }
            public class ImageData
            {
                public ulong UploaderID { get; set; }
                public uint ImageID { get; set; }
                public uint EntID { get; set; }
                public FileStorage.Type ImageType { get; set; }
            }
        }

        #endregion Image Upload



        public class EffectDictionary
        {
            public static Dictionary<string, string> DefaultEffects = new Dictionary<string, string>()
            {
                {"Submit", "assets/prefabs/deployable/dropbox/effects/submit_items.prefab"},
                {"Click", "assets/bundled/prefabs/fx/notice/item.select.fx.prefab"},
                {"Denied", "assets/prefabs/weapons/toolgun/effects/repairerror.prefab"},
                {"Beeb", "assets/prefabs/locks/keypad/effects/lock.code.unlock.prefab"},
                {"Star", "assets/prefabs/misc/easter/painted eggs/effects/eggpickup.prefab"},
                {"CloseBBQ", "assets/prefabs/deployable/bbq/effects/barbeque-deploy.prefab"},
                {"HeavyDoor", "assets/content/structures/bunker_hatch/sound/bunker-hatch-close-start.prefab"},
            };

            public static void PlayEffect(BasePlayer player, string prefab)
            {
                var effect = new Effect(prefab, player, 0, Vector3.zero, Vector3.forward);
                EffectNetwork.Send(effect, player.net.connection);
                effect.Clear(true);
            }
        }

        public class SoundDictionary
        {

            public static DefaultSounds sounds = new DefaultSounds();
            public class DefaultSounds
            {
                public string Submit = "assets/prefabs/deployable/dropbox/effects/submit_items.prefab";
                public string Click = "assets/bundled/prefabs/fx/notice/item.select.fx.prefab";
                public string Denied = "assets/prefabs/weapons/toolgun/effects/repairerror.prefab";
                public string Beeb = "assets/prefabs/locks/keypad/effects/lock.code.unlock.prefab";
                public string Star = "assets/prefabs/misc/easter/painted eggs/effects/eggpickup.prefab";
                public string CloseBBQ = "assets/prefabs/deployable/bbq/effects/barbeque-deploy.prefab";
                public string HeavyDoor = "assets/content/structures/bunker_hatch/sound/bunker-hatch-close-start.prefab";
            }

            public static void PlayError(BasePlayer player)
            {
                SoundDictionary.PlaySound(player, SoundDictionary.sounds.Denied);
                Instance.timer.Once(0.4f, () => { SoundDictionary.PlaySound(player, SoundDictionary.sounds.Denied); });
                Instance.timer.Once(0.8f, () => { SoundDictionary.PlaySound(player, SoundDictionary.sounds.Denied); });
            }

            public static void PlaySound(BasePlayer player, string prefab)
            {
                var effect = new Effect(prefab, player, 0, Vector3.zero, Vector3.forward);
                EffectNetwork.Send(effect, player.net.connection);
                effect.Clear(true);
            }
        }











        [ConsoleCommand("xxban")]
        void testCommand45(ConsoleSystem.Arg arg)
        {

            /*
            if (arg.HasArgs(10))
            {
                var i = arg.Args;
                PlayerManagement.Punishments.BanPlayerConsole(arg, i[0], i[1], i[2], i[3], i[4], i[5], i[6], i[7], i[8], i[9]);
                var bans = Interface.Oxide.DataFileSystem.ReadObject<Dictionary<string, FileManagement.BanLog.Punishments.BanData>>("XLib/Punishments");

                var player = arg.Player() as BasePlayer;
                PlayerManagement.Chat.DirectMessage(player, JsonConvert.SerializeObject(bans));
            }
            else
            {
                var player = arg.Player() as BasePlayer;
                PlayerManagement.Chat.DirectMessage(player, PlayerManagement.Chat.ErrorLevel.SendError(player, "Missing Args"));
            }
            */

        }


        #region Punishments Prevention Hooks

        private bool CanClientLogin(Network.Connection connection)
        {
            var RecordDB = Interface.Oxide.DataFileSystem.ReadObject<Dictionary<ulong, PunishmentsManagement.Execute.Data>>("XLib/Punishments");
            if (RecordDB.ContainsKey(connection.userid))
            {

            }

            //Credit: UIChamp
            timer.Once(1f, () => Network.Net.sv.Kick(connection, "Banned 10d", false));
            return true;
        }


        #endregion Punishments Prevention Hooks
        [ConsoleCommand("xbanned")]
        void XBanGetCommand(ConsoleSystem.Arg arg)
        {
            PrintWarning(PunishmentsManagement.Get.IsBanned(Convert.ToUInt64(arg.Args[0])).ToString());
        }

        [ConsoleCommand("xbanrevoke")]
        void OnBanRevoke(ConsoleSystem.Arg arg)
        {
            var SteamID = Convert.ToUInt64(arg.Args[0]);
            var BanKey = arg.Args[1];
            PunishmentsManagement.Revoke.Ban(SteamID, BanKey);
        }
        [ConsoleCommand("xbanexecute")]
        void OnBanExecute(ConsoleSystem.Arg arg)
        {
            var ExecuterName = arg.Args[0];
            var ExecuterID = arg.Args[1];
            var TargetName = arg.Args[2];
            var TargetID = arg.Args[3];
            var BanTitle = arg.Args[4];
            var BanReason = arg.Args[5];
            var BanDurationInput = arg.Args[6];
            var BanEvidence = arg.Args[7];
            var BanAppealable = arg.Args[8];
            var BanDetails = arg.Args[9];
            var BanAuthorName = arg.Args[10];
            var BanAuthorID = arg.Args[11];


            DateTimeOffset BanDuration = DateTimeOffset.UtcNow;
            long BanDurationUnix = 0;
            if (BanDurationInput == "-1") { BanDurationUnix = BanDuration.AddYears(1000).ToUnixTimeSeconds(); }//Life Time Permanent
            else
            {
                string BanFormat = BanDurationInput.ToLower().Contains("h") ? "h" : BanDurationInput.ToLower().Contains("d") ? "d" : BanDurationInput.ToLower().Contains("w") ? "w" : BanDurationInput.ToLower().Contains("m") ? "m" : null;
                var BanDurationRegex = Regex.Match(BanDurationInput, @"\d+");
                if (BanFormat == "h") { BanDurationUnix = BanDuration.AddHours(Convert.ToDouble(BanDurationRegex.Value)).ToUnixTimeSeconds(); }
                if (BanFormat == "d") { BanDurationUnix = BanDuration.AddDays(Convert.ToDouble(BanDurationRegex.Value)).ToUnixTimeSeconds(); }
                if (BanFormat == "w") { BanDurationUnix = BanDuration.AddDays(Convert.ToDouble(BanDurationRegex.Value) * 7).ToUnixTimeSeconds(); }
                if (BanFormat == "m") { BanDurationUnix = BanDuration.AddMonths(Convert.ToInt32(BanDurationRegex.Value)).ToUnixTimeSeconds(); }
            }


            PunishmentsManagement.Execute.Ban(new PunishmentsManagement.Execute.Data.BanData
            {
                ExecuterName = ExecuterName,
                ExecuterID = Convert.ToUInt64(ExecuterID),
                TargetName = TargetName,
                TargetID = Convert.ToUInt64(TargetID),
                BanTitle = BanTitle,
                BanReason = BanReason,
                BanDuration = BanDurationUnix,
                BanEvidence = BanEvidence,
                BanDetails = BanDetails,
                BanAppealable = BanAppealable,
                BanAuthorName = BanAuthorName,
                BanAuthorID = Convert.ToUInt64(BanAuthorID)
            });
        }
        public class PunishmentsManagement
        {


            public class Get
            {
                public static bool IsBanned(ulong SteamID)
                {
                    var RecordDB = Interface.Oxide.DataFileSystem.ReadObject<Dictionary<ulong, PunishmentsManagement.Execute.Data>>("XLib/Punishments");

                    if (RecordDB.ContainsKey(SteamID))
                    {
                        foreach (var ban in RecordDB[SteamID].Bans)
                        {
                            if (ban.Value.BanDuration > DateTimeOffset.UtcNow.ToUnixTimeSeconds()) //Ban Active
                            {
                                return true;
                            }
                            else if (DateTimeOffset.UtcNow.ToUnixTimeSeconds() >= ban.Value.BanDuration) //Ban Expired
                            {
                                return false;
                            }
                        }
                    }
                    return false;
                }



                public static int TotalBans(ulong SteamID)
                {
                    var RecordDB = Interface.Oxide.DataFileSystem.ReadObject<Dictionary<ulong, PunishmentsManagement.Execute.Data>>("XLib/Punishments");
                    if (RecordDB.ContainsKey(SteamID))
                    {
                        return RecordDB[SteamID].TotalBans;
                    }
                    return 0;
                }
            }
            public class Execute
            {
                private static void TryCreateRecord(ulong TargetID)
                {
                    var RecordDB = Interface.Oxide.DataFileSystem.ReadObject<Dictionary<ulong, PunishmentsManagement.Execute.Data>>("XLib/Punishments");
                    var Bans = new Dictionary<string, PunishmentsManagement.Execute.Data.BanData>();
                    var Kicks = new Dictionary<string, PunishmentsManagement.Execute.Data.KickData>();
                    var Mutes = new Dictionary<string, PunishmentsManagement.Execute.Data.MuteData>();
                    if (!RecordDB.ContainsKey(TargetID))
                    {
                        RecordDB.Add(TargetID, new PunishmentsManagement.Execute.Data
                        {
                            Bans = Bans,
                            Kicks = Kicks,
                            Mutes = Mutes,
                            TotalBans = 0,
                            TotalKicks = 0,
                            TotalMutes = 0,
                        });
                        Interface.Oxide.DataFileSystem.WriteObject("XLib/Punishments", RecordDB, false);
                    }
                }

                public static void Ban(PunishmentsManagement.Execute.Data.BanData BanData)
                {
                    TryCreateRecord(BanData.TargetID);
                    var RecordDB = Interface.Oxide.DataFileSystem.ReadObject<Dictionary<ulong, PunishmentsManagement.Execute.Data>>("XLib/Punishments");


                    RecordDB[BanData.TargetID].Bans.Add($"ban{PunishmentsManagement.Get.TotalBans(BanData.TargetID) + 1}", new Data.BanData
                    {
                        ExecuterName = BanData.ExecuterName,
                        ExecuterID = BanData.ExecuterID,
                        TargetName = BanData.TargetName,
                        TargetID = BanData.TargetID,
                        BanTitle = BanData.BanTitle,
                        BanReason = BanData.BanReason,
                        BanDuration = BanData.BanDuration,
                        BanEvidence = BanData.BanEvidence,
                        BanDetails = BanData.BanDetails,
                        BanAppealable = BanData.BanAppealable,
                        BanAuthorName = BanData.BanAuthorName,
                        BanAuthorID = BanData.BanAuthorID,
                    });
                    RecordDB[BanData.TargetID].TotalBans = RecordDB[BanData.TargetID].Bans.Count;
                    Interface.Oxide.DataFileSystem.WriteObject("XLib/Punishments", RecordDB, false);


                }

                public class Data
                {
                    public Dictionary<string, BanData> Bans { get; set; }
                    public Dictionary<string, KickData> Kicks { get; set; }
                    public Dictionary<string, MuteData> Mutes { get; set; }

                    public int TotalBans { get; set; }
                    public int TotalKicks { get; set; }
                    public int TotalMutes { get; set; }
                    public int TotalRepots { get; set; }

                    public class BanData
                    {
                        public string ExecuterName { get; set; }
                        public ulong ExecuterID { get; set; }
                        public string TargetName { get; set; }
                        public ulong TargetID { get; set; }
                        public string BanTitle { get; set; }
                        public string BanReason { get; set; }
                        public long BanDuration { get; set; }
                        public string BanEvidence { get; set; }
                        public string BanDetails { get; set; }
                        public string BanAppealable { get; set; }
                        public string BanAuthorName { get; set; }
                        public ulong BanAuthorID { get; set; }
                    }

                    public class KickData
                    {
                        public string ExecuterName { get; set; }
                        public ulong ExecuterID { get; set; }
                        public string TargetName { get; set; }
                        public ulong TargetID { get; set; }
                        public string KickTitle { get; set; }
                        public string KickReason { get; set; }
                        public string KickEvidence { get; set; }
                        public string KickDetails { get; set; }
                        public string KickAuthorName { get; set; }
                        public ulong KickAuthorID { get; set; }
                    }

                    public class MuteData
                    {
                        public string ExecuterName { get; set; }
                        public long ExecuterID { get; set; }
                        public string TargetName { get; set; }
                        public long TargetID { get; set; }
                        public string MuteTitle { get; set; }
                        public string MuteReason { get; set; }
                        public DateTimeOffset MuteDuration { get; set; }
                        public string MuteEvidence { get; set; }
                        public string MuteAppealable { get; set; }
                        public string MuteDetails { get; set; }
                        public string MuteAuthorName { get; set; }
                        public ulong MuteAuthorID { get; set; }
                    }

                    public class ReportData
                    {
                        public string TargetName { get; set; }
                        public long TargetID { get; set; }
                        public string ReportTitle { get; set; }
                        public string ReportReason { get; set; }
                        public string ReportDetails { get; set; }
                        public string ReportAuthorName { get; set; }
                        public ulong ReportAuthorID { get; set; }
                    }
                }
            }
            public class Revoke
            {
                public static void Ban(ulong SteamID, string BanKey)
                {
                    var RecordDB = Interface.Oxide.DataFileSystem.ReadObject<Dictionary<ulong, PunishmentsManagement.Execute.Data>>("XLib/Punishments");
                    if (RecordDB.ContainsKey(SteamID) && RecordDB[SteamID].Bans.ContainsKey(BanKey))
                    {
                        RecordDB[SteamID].Bans.Remove(BanKey);
                        RecordDB[SteamID].TotalBans -= 1;
                    }
                    Interface.Oxide.DataFileSystem.WriteObject("XLib/Punishments", RecordDB, false);
                }
            }
        }

        public class PlayerManagement
        {


            public class Location
            {
                private void TeleportPlayer(BasePlayer player, Vector3 Location) => player.Teleport(Location);

                private void SaveLocation(BasePlayer player, Vector3 Location)
                {
                    Dictionary<ulong, Vector3> LocationData = new Dictionary<ulong, Vector3>();
                    if (!LocationData.ContainsKey(player.userID)) { LocationData.Add(player.userID, Location); }
                }
            }

            public class Chat
            {
                public class ErrorLevel
                {
                    public static string SendSuccess(BasePlayer player, string Message) { return TextEncodeing(16, "00fa00", "[SUCCESS] ") + TextEncodeing(16, "ffffff", Message); }
                    public static string SendWarning(BasePlayer player, string Message) { return TextEncodeing(16, "fac800", "[WARNING] ") + TextEncodeing(16, "ffffff", Message); }
                    public static string SendError(BasePlayer player, string Message) { return TextEncodeing(16, "fa0000", "[ERROR] ") + TextEncodeing(16, "ffffff", Message); }

                }

                public static string TextEncodeing(double TextSize, string HexColor, string PlainText) => $"<size={TextSize}><color=#{HexColor}>{PlainText}</color></size>";

                public static void DirectMessage(BasePlayer player, string message) => player.SendConsoleCommand("chat.add", 0, "76561198389709969", message);
            }

            public class Inventory
            {
                public void GiveItem(BasePlayer player, string ShortName, int Quantity, ulong SkinID, ItemContainer container, int position)
                {
                    Item item = ItemManager.CreateByName(ShortName, Quantity, SkinID);
                    player.inventory.GiveItem(item);
                    if (container != null && position >= 0) { item.MoveToContainer(container, position); }
                    else if (container != null && position < 0) { item.MoveToContainer(container); }
                }

                public class Items
                {
                    public static Items Caller = new Items();

                    private Dictionary<ulong, SavedInventories> cachedInventories = new Dictionary<ulong, SavedInventories>();
                    private class SavedInventories
                    {
                        public List<SavedItem> Items;
                    }
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
                        foreach (Item item in player.inventory.containerMain.itemList) { if (item != null) { PlayerItems.Add(ProcessItem(item, player.inventory.containerMain)); } }
                        foreach (Item item in player.inventory.containerBelt.itemList) { if (item != null) { PlayerItems.Add(ProcessItem(item, player.inventory.containerBelt)); } }
                        foreach (Item item in player.inventory.containerWear.itemList) { if (item != null) { PlayerItems.Add(ProcessItem(item, player.inventory.containerWear)); } }
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
                        if (!cachedInventories.ContainsKey(player.userID)) { Chat.DirectMessage(player, "Inventory Lost"); }
                        ClearInventory(player);
                        foreach (SavedItem savedItem in cachedInventories[player.userID].Items)
                        {
                            var i = CreateItem(savedItem);
                            GiveItem(player, i, savedItem.container);
                            i.MoveToContainer(player.inventory.containerMain, savedItem.ItemPosition);
                        }
                        cachedInventories.Remove(player.userID);
                    }
                    private void GiveItem(BasePlayer player, Item item, ItemContainer container)
                    {
                        if (item != null) { player.inventory.GiveItem(item, container); }
                    }
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

                    public void ClearInventory(BasePlayer player) => player.inventory.Strip();

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
        }





        private object OnPlayerViolation(BasePlayer player, AntiHackType type) //Fixes Players Undermap When Mouted
        {
            if (UI.LockPlayer.IsMounted(player)) return false;
            return null;
        }



        public class UI
        {

            public class CuiPoint
            {


                public double Width, Height, X, Y;

                public CuiPoint(double Width, double Height, double X, double Y)
                {
                    //this.Width = Width + X > 1.0 ? 1.0 : Width + X;
                    //this.Height = Height + Y > 1.0 ? 1.0 : Height + Y;
                    this.Width = Width;
                    this.Height = Height;
                    this.X = X > 1.0 ? 1.0 : X;
                    this.Y = Y > 1.0 ? 1.0 : Y;
                }
                public string GetXY() => $"{X} {Y}";
                public string GetWH() => $"{Width} {Height}";
            }



            public class AspectRatio
            {


                public static double Scale(double oldValue, double oldMin, double oldMax, double newMin, double newMax) //Credit: UIChamp
                {

                    double num = oldMax - oldMin;
                    double num2 = newMax - newMin;
                    return (oldValue - oldMin) * num2 / num + newMin;
                }
            }

            #region Functions
            public static DefaultColors colors = new DefaultColors();
            public class DefaultColors
            {
                public string Empty = "000000";
                public string Green = "32c832";
                public string Red = "c83232";
                public string Gray20 = "202020";
                public string Gray30 = "303030";
                public string Gray40 = "404040";
                public string Gray50 = "505050";
                public string RustUI = "fff7eb";//0.15 alpha
            }

            public static string HexColor(string hexColor, float alpha)
            {
                if (hexColor.StartsWith("#")) { hexColor = hexColor.TrimStart('#'); }
                int red = int.Parse(hexColor.Substring(0, 2), NumberStyles.AllowHexSpecifier);
                int green = int.Parse(hexColor.Substring(2, 2), NumberStyles.AllowHexSpecifier);
                int blue = int.Parse(hexColor.Substring(4, 2), NumberStyles.AllowHexSpecifier);
                return $"{(double)red / 255} {(double)green / 255} {(double)blue / 255} {alpha}";
            }


            public static void DestroyUI(string Element, BasePlayer player) => CuiHelper.DestroyUi(player, Element);
            public static void DrawUI(CuiElementContainer container, BasePlayer player) => CuiHelper.AddUi(player, container);


            public class LockPlayer
            {
                private static LockPlayer Caller = new LockPlayer();
                public static void Mount(BasePlayer player) => Caller.MountPlayer(player);
                public static void Dismount(BasePlayer player) => Caller.DismountPlayer(player);
                public static bool IsMounted(BasePlayer player) => Caller.Mounted(player);


                private Dictionary<ulong, PlayerData> PlayerDict = new Dictionary<ulong, PlayerData>();
                private class PlayerData
                {
                    public Vector3 PlayerPos;
                    public Quaternion PlayerRot;
                    public List<BaseEntity> entities = new List<BaseEntity>();
                    public bool IsMounted;
                }

                private bool Mounted(BasePlayer player)
                {
                    if (PlayerDict.ContainsKey(player.userID))
                        if (PlayerDict[player.userID].IsMounted)
                            return true;
                    return false;
                }


                private void MountPlayer(BasePlayer player)
                {
                    string prefab = "assets/prefabs/deployable/chair/chair.deployed.prefab";

                    if (!PlayerDict.ContainsKey(player.userID))
                    {
                        PlayerDict.Add(player.userID, new PlayerData());
                    }

                    PlayerDict[player.userID].PlayerPos = player.transform.position;
                    PlayerDict[player.userID].PlayerRot = player.transform.rotation;

                    Vector3 PlayerPos = PlayerDict[player.userID].PlayerPos;
                    Quaternion PlayerRot = PlayerDict[player.userID].PlayerRot;

                    var MountOjb = GameManager.server.CreateEntity(prefab, new Vector3(PlayerPos.x, 0, PlayerPos.z), PlayerRot);
                    MountOjb.Spawn();
                    PlayerDict[player.userID].entities.Add(MountOjb);
                    player.SetMounted((BaseMountable)MountOjb);
                    PlayerDict[player.userID].IsMounted = true;

                }

                private void DismountPlayer(BasePlayer player)
                {
                    foreach (var ent in PlayerDict[player.userID].entities)
                    {
                        try { ent.Kill(); }
                        catch { }
                    }

                    player.Teleport(PlayerDict[player.userID].PlayerPos);
                    PlayerDict.Remove(player.userID);
                    player.DismountObject();
                    player.EnsureDismounted();
                }
            }



            #endregion


            #region Elements

            public static CuiElementContainer CreateElementContainer(string panelName, int zIndex, string HexColor, float HexAlpha, float fadeDuration, bool useCursor, CuiPoint point)
            {
                string Layer;
                switch (zIndex) //Layer switch
                {
                    case 0:
                        Layer = "Overall";
                        break;
                    case 1:
                        Layer = "Overlay";
                        break;
                    case 2:
                        Layer = "Hud";
                        break;
                    case 3:
                        Layer = "Hud.Menu";
                        break;
                    case 4:
                        Layer = "Under";
                        break;
                    default:
                        Layer = "Overall";
                        break;
                }

                CuiElementContainer container = new CuiElementContainer()
                {
                    {
                        new CuiPanel
                        {
                            Image = { Color = HexColor == null ? null : UI.HexColor(HexColor, HexAlpha),FadeIn = fadeDuration,},
                            RectTransform ={ AnchorMin = point.GetXY(), AnchorMax = point.GetXY(), OffsetMin = "0 0", OffsetMax = point.GetWH() },
                            CursorEnabled = useCursor,
                            FadeOut = fadeDuration,
                        }, new CuiElement().Parent = Layer, panelName
                    }
                };
                return container;
            }



            public static void CreateButton(CuiElementContainer container, string ParentItem, string BtnCommand, string HexColor, float HexAlpha, float fadeDuration, bool useCursor, string text, string HexTextHexColor, int TextSize, TextAnchor TextAlign, CuiPoint point)
            {
                container.Add(new CuiButton
                {
                    Button = { Color = HexColor == null ? null : UI.HexColor(HexColor, HexAlpha), Command = BtnCommand, FadeIn = fadeDuration },
                    RectTransform = { AnchorMin = point.GetXY(), AnchorMax = point.GetXY(), OffsetMin = "0 0", OffsetMax = point.GetWH() },
                    Text = { Text = text, Color = UI.HexColor(HexTextHexColor, 1), FontSize = TextSize, Align = TextAlign },
                }, ParentItem);
            }

            public static void CreatePanel(CuiElementContainer container, string ParentItem, string ElementName, string HexColor, float HexAlpha, float fadeDuration, CuiPoint point)
            {
                container.Add(new CuiElement
                {
                    Name = ElementName,
                    Parent = ParentItem,
                    Components =
                    {
                        new CuiImageComponent{ Color = HexColor == null ? null : UI.HexColor(HexColor, HexAlpha), FadeIn = fadeDuration },
                        new CuiRectTransformComponent{ AnchorMin = point.GetXY(), AnchorMax = point.GetXY(), OffsetMin = "0 0", OffsetMax = point.GetWH()}
                    },
                    FadeOut = fadeDuration,
                });
            }

            public static void CreateLabel(CuiElementContainer container, string ParentItem, string text, TextAnchor TextAlign, string TextHexColor, int TextSize, CuiPoint point)
            {
                container.Add(new CuiLabel
                {
                    Text = { Text = text, Align = TextAlign, Color = TextHexColor == null ? null : UI.HexColor(TextHexColor, 1), FontSize = TextSize },
                    RectTransform = { AnchorMin = point.GetXY(), AnchorMax = point.GetXY(), OffsetMin = "0 0", OffsetMax = point.GetWH() },
                }, ParentItem);
            }

            public static void CreateImage(CuiElementContainer container, string ParentItem, string ElementName, string ImageURL, CuiPoint point)
            {
                container.Add(new CuiElement
                {
                    Name = ElementName,
                    Parent = ParentItem,
                    Components =
                    {
                        new CuiRawImageComponent { Url = ImageURL },
                        new CuiRectTransformComponent { AnchorMin = point.GetXY(), AnchorMax = point.GetXY(), OffsetMin = "0 0", OffsetMax = point.GetWH() }
                    }
                });
            }

            public static void CreateInputField(CuiElementContainer container, string ParentItem, int ElementIndex, string HexColor, float HexAlpha, TextAnchor TextAlign, int CharLimit, string TextHexColor, int TextSize, bool Password, string InputComamnd, CuiPoint point)
            {
                container.Add(new CuiElement
                {
                    Name = $"InputField{ElementIndex}",
                    Parent = ParentItem,
                    Components =
                    {
                        new CuiImageComponent { Color = HexColor == null ? null : UI.HexColor(HexColor, HexAlpha) },
                        new CuiRectTransformComponent { AnchorMin = point.GetXY(), AnchorMax = point.GetXY(), OffsetMin = "0 0", OffsetMax = point.GetWH() }
                    }
                });

                container.Add(new CuiElement
                {
                    Parent = $"InputField{ElementIndex}",
                    Components =
                    {
                        new CuiInputFieldComponent { Color = TextHexColor == null ? null : UI.HexColor(TextHexColor, 1), Align = TextAlign, FontSize = TextSize, CharsLimit = CharLimit <= -1 ? -1 : CharLimit, Command = InputComamnd, IsPassword = Password },
                        new CuiRectTransformComponent { AnchorMin = "0 0", AnchorMax = "1 1", OffsetMax = "0 0" },
                    }
                });
            }
            #endregion Elements
        }

    }
}