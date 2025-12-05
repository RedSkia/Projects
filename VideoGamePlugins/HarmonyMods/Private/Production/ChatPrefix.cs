using System;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Harmony;
using Network;
using static ConsoleSystem;

namespace HarmonyMods.ChatPrefix
{
    internal static class CustomBehaviour
    {
        internal static void OnMessage(string strCommand, ref object[] args)
        {
            if (strCommand.ToLower() != "chat.add") { return; }
            
            try
            {
                int chatType = 2; int.TryParse(args[0].ToString(), out chatType);
                ulong chatId = 0; ulong.TryParse(args[1].ToString(), out chatId);
                if (chatId == 0) /*Is Server*/
                {
                    StringBuilder sb = new StringBuilder(args[2].ToString());
                    sb.Replace("SERVER", "");
                    sb.Replace("<color=#eee></color>", "");
                    string output = sb.ToString().TrimStart();
                    args = new object[] { chatType, 76561198389709969, $"<size=16>[<color=#4bafe1>SERVER</color>]</size> {output}" };
                }
            } catch { }
        }
    }



    [HarmonyPatch(typeof(ConsoleNetwork), nameof(ConsoleNetwork.SendClientCommand), new Type[]
    {
        typeof(Connection),
        typeof(string),
        typeof(object[])
    })]
    internal class ConsoleNetwork_SendClientCommand
    {
        [HarmonyPrefix]
        private static void Prefix(Connection cn, string strCommand, ref object[] args)
        {
            CustomBehaviour.OnMessage(strCommand, ref args);
        }
    }

    [HarmonyPatch(typeof(ConsoleNetwork), nameof(ConsoleNetwork.BroadcastToAllClients), new Type[]
    {
        typeof(string),
        typeof(object[])
    })]
    internal class ConsoleNetwork_BroadcastToAllClients
    {
        [HarmonyPrefix]
        private static void Prefix(string strCommand, ref object[] args)
        {
            CustomBehaviour.OnMessage(strCommand, ref args);
        }
    }
}