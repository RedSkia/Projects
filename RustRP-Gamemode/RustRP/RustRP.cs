using CoreRP;
using Oxide.Core;
using UnityEngine;

namespace Oxide.Plugins
{
    [Info("RustRP", "InfinityNet/Xray", "1.0.34")]
    [Description("Infinitynet Roleplay System")]
    internal class RustRP : RustPlugin
    {
        internal static RustRP Instance { get; private set; } = new RustRP();
   

        #region Init
        private void Loaded()
        {
            Instance = this;
            CoreRP.ZoneManager.Script.Instance.Load();
            CoreRP.ImageManager.Instance.LoadData();
        }
        private void Unload()
        {
            CoreRP.ZoneManager.Script.Instance.Unload();
            CoreRP.ImageManager.Instance.SaveData();
        }
        #endregion Init


        #region Console Interface
        [ConsoleCommand("rp")]
        private void ConsoleCommandInterface(ConsoleSystem.Arg console)
        {
            if (!console.IsRcon || !console.IsServerside) { return; }
            var argSelector = console.Args.ElementAt(0).ToLower();
            var argValue = console.Args.ElementAt(1).ToLower();

            switch (argSelector)
            {
                case "l":
                case "load": {
                    switch (argValue)
                    {
                        case "z":
                        case "zm":
                        case "zones":
                        case "zonemanager":
                        CoreRP.ZoneManager.Script.Instance.Load(); break;
                    }
                } break;
                case "u":
                case "unload": { 
                    switch (argValue)
                    {
                        case "z":
                        case "zm":
                        case "zones":
                        case "zonemanager":
                        CoreRP.ZoneManager.Script.Instance.Unload(); break;
                    }
                } break;
            }
        }
        #endregion Console Interface

        [ChatCommand("ui")]
        private void OnDrawTestUI(BasePlayer player)
        {
            UI.Destroy(player, "testuibox");
            UI.Draw(player,
                UI.Controls.Container("testuibox", "0.5 0.5 0.5 0.5", "Red", "-100 100 -100 100"),
                UI.Controls.Image("testuibox", Settings.Images.Logo, "0 1 0 1", "Green")
            );
        }
        [ChatCommand("noui")]
        private void OnKillTestUI(BasePlayer player)
        {
            UI.Destroy(player, "testuibox");
        }

        [ChatCommand("img")]
        private void OnAddImageTest(BasePlayer player)
        {
            //ImageManager.StoreImageFromUrl("https://www.rustedit.io/images/splash.png");
            UI.Destroy(player, "testuibox");
        }

        #region Game Hooks
        private object CanBuild(Planner planner, Construction prefab, Construction.Target target)
        {
            BasePlayer player = planner?.GetOwnerPlayer();
            if (!player.IsPlayer(true)) { return null; }
            #region ZoneManager 
            if(CoreRP.ZoneManager.Script.Instance.IsEnabled)
            {
                var zone = CoreRP.ZoneManager.ZoneHelper.GetActiveZone(player);
                if (zone != null)
                {
                    bool isAdmin = new CoreRP.Security.Player(player, Settings.Groups.Administrator);
                    if (isAdmin) { return null; }


                    if (!FlagManager<CoreRP.ZoneManager.ZoneFlagsAllowed>.Has(zone.Flags, CoreRP.ZoneManager.ZoneFlagsAllowed.Building))
                    {
                        player.SendMsg("Zone", $"You cannot build in zone: {zone.Prefix}");
                        return false;
                    }
                    if (player.IsTeam(target.entity?.OwnerID ?? 0)) { return null; }
                }
            }
            #endregion ZoneManager
            return null;
        }
        private object CanDeployItem(BasePlayer player, Deployer deployer, NetworkableId entityId)
        {
            var entity = BaseNetworkable.serverEntities.Find(entityId) as BaseEntity;
            if (!player.IsPlayer(true)) { return null; }
            #region ZoneManager 
            if (CoreRP.ZoneManager.Script.Instance.IsEnabled)
            {
                var zone = CoreRP.ZoneManager.ZoneHelper.GetActiveZone(player);
                if (zone != null)
                {
                    bool isAdmin = new CoreRP.Security.Player(player, Settings.Groups.Administrator);
                    if (isAdmin) { return null; }
                    if (!FlagManager<CoreRP.ZoneManager.ZoneFlagsAllowed>.Has(zone.Flags, CoreRP.ZoneManager.ZoneFlagsAllowed.Building))
                    {
                        player.SendMsg("Zone", $"You cannot build in zone: {zone.Prefix}");
                        return false;
                    }
                    if (player.IsTeam(entity?.OwnerID ?? 0)) { return null; }
                }
            }
            #endregion ZoneManager
            return null;
        }

        private object CanLootEntity(BasePlayer player, BaseEntity entity)
        {
            if (!player.IsPlayer(true)) { return null; }
            #region ZoneManager
            if (CoreRP.ZoneManager.Script.Instance.IsEnabled)
            {
                var zone = CoreRP.ZoneManager.ZoneHelper.GetActiveZone(player);
                if (zone != null)
                {
                    bool isAdmin = new CoreRP.Security.Player(player, Settings.Groups.Administrator);
                    if (isAdmin) { return null; }
                    if (!FlagManager<CoreRP.ZoneManager.ZoneFlagsAllowed>.Has(zone.Flags, CoreRP.ZoneManager.ZoneFlagsAllowed.Interact))
                    {
                        player.SendMsg("Zone", $"You cannot interact in zone: {zone.Prefix}");
                        return false;
                    }
                    if (player.IsTeam(entity?.OwnerID ?? 0)) { return null; }
                }
            }
            #endregion ZoneManager
            return null;
        }
        private bool CanLootPlayer(BasePlayer target, BasePlayer looter)
        {
            if (!looter.IsPlayer(true)) { return true; }
            #region ZoneManager
            if (CoreRP.ZoneManager.Script.Instance.IsEnabled)
            {
                var zone = CoreRP.ZoneManager.ZoneHelper.GetActiveZone(looter);
                if (zone != null)
                {
                    bool isAdmin = new CoreRP.Security.Player(looter, Settings.Groups.Administrator);
                    if (isAdmin) { return true; }
                    if (!FlagManager<CoreRP.ZoneManager.ZoneFlagsAllowed>.Has(zone.Flags, CoreRP.ZoneManager.ZoneFlagsAllowed.Interact))
                    {
                        looter.SendMsg("Zone", $"You cannot interact in zone: {zone.Prefix}");
                        return false;
                    }
                    if (looter.IsTeam(target?.userID ?? 0)) { return true; }
                }
            }
            #endregion ZoneManager
            return true;
        }
        private object OnEntityTakeDamage(BaseEntity entity, HitInfo hit)
        {
            BasePlayer player = hit?.InitiatorPlayer;
            if (!player.IsPlayer(true)) { return null; }
            #region ZoneManager
            if (CoreRP.ZoneManager.Script.Instance.IsEnabled)
            {
                var zone = CoreRP.ZoneManager.ZoneHelper.GetActiveZone(player);
                if (zone != null)
                {
                    bool isAdmin = new CoreRP.Security.Player(player, Settings.Groups.Administrator);
                    if (isAdmin) { return true; }
                    if (!FlagManager<CoreRP.ZoneManager.ZoneFlagsAllowed>.Has(zone.Flags, CoreRP.ZoneManager.ZoneFlagsAllowed.Damage))
                    {
                        player.SendMsg("Zone", $"You cannot damage objects in zone: {zone.Prefix}");
                        return false;
                    }
                    if (player.IsTeam(entity?.OwnerID ?? 0)) { return true; }
                }
            }
            #endregion ZoneManager
            return null;
        }
        private object CanMountEntity(BasePlayer player, BaseMountable entity)
        {
            if (!player.IsPlayer(true)) { return null; }
            #region ZoneManager
            if (CoreRP.ZoneManager.Script.Instance.IsEnabled)
            {
                var zone = CoreRP.ZoneManager.ZoneHelper.GetActiveZone(player);
                if (zone != null)
                {
                    bool isAdmin = new CoreRP.Security.Player(player, Settings.Groups.Administrator);
                    if (isAdmin) { return true; }
                    if (!FlagManager<CoreRP.ZoneManager.ZoneFlagsAllowed>.Has(zone.Flags, CoreRP.ZoneManager.ZoneFlagsAllowed.Vehicles))
                    {
                        player.SendMsg("Zone", $"You cannot use vehicles in zone: {zone.Prefix}");
                        return false;
                    }
                    if (player.IsTeam(entity?.OwnerID ?? 0)) { return true; }
                }
            }
            #endregion ZoneManager
            return null;
        }
        #endregion Game Hooks
    }
}