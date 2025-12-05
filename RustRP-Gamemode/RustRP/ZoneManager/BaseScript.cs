using Oxide.Core;
using Oxide.Game.Rust.Libraries;

namespace CoreRP.ZoneManager
{
    internal static class Script
    {
        internal readonly static BaseScript Instance = new BaseScript();
    }
    internal sealed class BaseScript : Contracts.BaseScript
    {
        private readonly ZoneCommander CC = new ZoneCommander(); 

        internal override void ScriptCheck() => base.ScriptCheck();
        internal override void Load(string Namespace = "")
        {
            base.Load(this.GetType().Namespace);
            Interface.Oxide.GetLibrary<Command>()?.AddChatCommand(CC.Command, Oxide.Plugins.RustRP.Instance, CC.OnCommand);
            ZoneData.Instance.LoadData();
        }
        internal override void Unload(string Namespace = "")
        {
            base.Unload(this.GetType().Namespace);
            Interface.Oxide.GetLibrary<Command>()?.RemoveChatCommand(CC.Command, Oxide.Plugins.RustRP.Instance);
            ZoneCreator.DisableTool();
            ZoneData.Instance.KillColliders();
            ZoneData.Instance.SaveData();
        }
    }
}