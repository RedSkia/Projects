using Harmony;
using System;
using static BaseEntity;

namespace HarmonyMods.NoReport
{
    [HarmonyPatch(typeof(BasePlayer), nameof(BasePlayer.OnPlayerReported), new Type[]
    {
        typeof(BaseEntity.RPCMessage)
    })]
    internal class BasePlayer_OnPlayerReported
    {
        [HarmonyPrefix]
        private static bool Prefix(RPCMessage msg)
        {
            return false;
        }
    }

    [HarmonyPatch(typeof(BasePlayer), nameof(BasePlayer.OnFeedbackReport), new Type[]
    {
            typeof(BaseEntity.RPCMessage)
    })]
    internal class BasePlayer_OnFeedbackReport
    {
        [HarmonyPrefix]
        private static bool Prefix(RPCMessage msg)
        {
            return false;
        }
    }
}