using Oxide.Core;
using Oxide.Core.Plugins;
using ProtoBuf;
using System;
using System.Linq;
using UnityEngine;

namespace Oxide.Plugins
{

    [Info("XVehicle", "InfinityNet/Xray", "1.0.0")]
    [Description("Copyright Infinitynet.dk All Rights Reserved")]
    public class XVehicle : RustPlugin
    {
        [PluginReference] private readonly Plugin ServerRewards;
        private static XVehicle Instance { get; set; }

        private const int MiniCopterPrice = 1000;
        private const int TransportCopterPrice = 2000;
        private const int RHIBPrice = 700;
        private const int BoatPrice = 300;
        #region Init
        private void Loaded()
        {
            Instance = this;
            cmd.AddChatCommand("buy", this, "CommandHandler");
        }
        #endregion Init

        private void CommandHandler(BasePlayer player, string command, string[] args)
        {
            try
            {
                switch (args[0])
                {
                    case "mini":
                    case "heli":
                    case "copter":
                    case "miniheli":
                    case "minicopter":
                        Buy.spawn(player, "assets/content/vehicles/minicopter/minicopter.entity.prefab", "Minicopter", MiniCopterPrice, new Minicopter());
                        break;

                    case "transport":
                    case "scrapheli":
                    case "scraptransport":
                    case "scraptransporthelicopter":
                        Buy.spawn(player, "assets/content/vehicles/scrap heli carrier/scraptransporthelicopter.prefab", "Scrap Transport Helicopter", TransportCopterPrice, new ScrapTransportHelicopter());
                        break;

                    case "rhib":
                    case "fastboat":
                    case "speedboat":
                        Buy.spawn(player, "assets/content/vehicles/boats/rhib/rhib.prefab", "RHIB Boat", RHIBPrice, new RHIB());
                        break;

                    case "boat":
                    case "slowboat":
                    case "regularboat":
                        Buy.spawn(player, "assets/content/vehicles/boats/rowboat/rowboat.prefab", "Regular Boat", BoatPrice, new MotorRowboat());
                        break;
                }
            } catch { Instance.DirectMessage(player, $"Missing Arg"); }
        }


        private class Buy
        {
            public static void spawn(BasePlayer player, string prefab, string Item, int Price, object type)
            {
                if (player.isInAir || player.InSafeZone() || player.IsWounded() || player.IsBuildingBlocked() || player.isMounted || player.IsFlying)
                { Instance.DirectMessage(player, $"Purchase Failed"); return; }

                var coins = Convert.ToInt32(Instance.ServerRewards?.Call("CheckPoints", player.userID));

                if (coins < Price)
                {
                    Instance.DirectMessage(player, $"Not enough coins to purchase a \n{TextEncodeing(16, "4be1fa", Item, true)}\n" +
                    $"Acquire { TextEncodeing(16, "4be1fa", $"{Price - coins}", true)} Coin's more to purchase a item"); return;
                }


                #region Spawn Code
                Vector3 forward = player.GetNetworkRotation() * Vector3.forward;
                forward.y = 0;
                bool isBig = (prefab.Contains("scraptransporthelicopter") || prefab.Contains("rhib")) ? true : false;
                float distance = isBig ? 10f : 3f;
                var pos = player.transform.position + forward.normalized * distance + Vector3.up * 2f;

       
                var vehicle = GameManager.server.CreateEntity(
                    prefab,
                    pos,
                    Quaternion.Euler(0, player.GetNetworkRotation().eulerAngles.y - 180, 0));

                vehicle.OwnerID = player.userID;
                vehicle.Spawn();

           
                if(type is Minicopter)
                {
                    /*
                    var i = vehicle as Minicopter;
                    var fuel = i.GetFuelSystem().GetFuelContainer();
                    fuel.inventory.AddItem(fuel.allowedItem, 1000);
                    */
                }
                if (type is ScrapTransportHelicopter)
                {
                    /*
                    var i = vehicle as ScrapTransportHelicopter;
                    var fuel = i.GetFuelSystem().GetFuelContainer();
                    fuel.inventory.AddItem(fuel.allowedItem, 2000);
                    */
                }
                if(type is RHIB)
                {
                    /*
                    var i = vehicle as RHIB;
                    var fuel = i.GetFuelSystem().GetFuelContainer();
                    fuel.inventory.AddItem(fuel.allowedItem, 250);
                    */

                }
                if(type is MotorRowboat)
                {
                    /*
                    var i = vehicle as MotorRowboat;
                    var fuel = i.GetFuelSystem().GetFuelContainer();
                    fuel.inventory.AddItem(fuel.allowedItem, 100);
                    */
                }
           
          
                Instance.ServerRewards?.Call("TakePoints", player.userID, Price);

                Instance.DirectMessage(player, $"Successfully purchased a {TextEncodeing(16, "4be1fa", Item, true)} Enjoy");
                #endregion Spawn Code
            }
        }

        #region Helpers
        private void DirectMessage(BasePlayer player, string message)
        {
            try { player.SendConsoleCommand("chat.add", 0, 76561198389709969, $"{TextEncodeing(16, "4be1fa", Instance.Name, true)}{TextEncodeing(16, "ffffff", ":", false)} {TextEncodeing(16, "ffffff", message, false)}"); }
            catch { Instance.SendReply(player, message); }
        }
        private static string TextEncodeing(double TextSize, string HexColor, string text, bool bold)
        {
            var s = TextSize <= 0 ? 1 : TextSize;
            var c = HexColor.StartsWith("#") ? HexColor : $"#{HexColor}";
            if (bold) { return $"<size={s}><color={c}><b>{text}</b></color></size>"; }
            else { return $"<size={s}><color={c}>{text}</color></size>"; }
        }
        #endregion Helpers


        #region Rust On Spawn Ensure Good Fuel
        void OnEntitySpawned(Minicopter entity)
        {
            if (entity == null) return;

            timer.Once(1f, () =>
            {
                /*
                var fuel = entity.GetFuelSystem().GetFuelContainer();
                var fuelCount = fuel.inventory.itemList.Sum(x => x.amount);
                fuel.inventory.AddItem(fuel.allowedItem, 1000 - fuelCount);
                */
            });
        }
        void OnEntitySpawned(ScrapTransportHelicopter entity)
        {
            if (entity == null) return;

            timer.Once(1f, () =>
            {
                /*
                var fuel = entity.GetFuelSystem().GetFuelContainer();
                var fuelCount = fuel.inventory.itemList.Sum(x => x.amount);
                fuel.inventory.AddItem(fuel.allowedItem, 2000 - fuelCount);
                */
            });
        }
        void OnEntitySpawned(RHIB entity)
        {
            if (entity == null) return;

            timer.Once(1f, () =>
            {
                /*
                var fuel = entity.GetFuelSystem().GetFuelContainer();
                var fuelCount = fuel.inventory.itemList.Sum(x => x.amount);
                fuel.inventory.AddItem(fuel.allowedItem, 250 - fuelCount);
                */
            });
        }
        void OnEntitySpawned(MotorRowboat entity)
        {
            if (entity == null) return;

            timer.Once(1f, () =>
            {
                /*
                var fuel = entity.GetFuelSystem().GetFuelContainer();
                var fuelCount = fuel.inventory.itemList.Sum(x => x.amount);
                fuel.inventory.AddItem(fuel.allowedItem, 100 - fuelCount);
                */
            });
        }
        #endregion Rust On Spawn Ensure Good Fuel


    }
}