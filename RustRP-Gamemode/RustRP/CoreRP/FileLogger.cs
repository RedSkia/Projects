using System;
using System.Runtime.CompilerServices;

namespace CoreRP
{
    internal sealed class OxideLogger : Oxide.Plugins.CSharpPlugin
    {
        internal void Log(string file, string message) => LogToFile($"_{file}", message, Oxide.Plugins.RustRP.Instance, false, false);
    }
    internal static class FileLogger
    {
        private static OxideLogger Logger = new OxideLogger();
        internal enum LogCatagory
        {
            ServerShutdown,
            ServerOnline,
            ServerCommand,
            ServerMessage,
            ServerPermission,

            PlayerSpawn,
            PlayerMessage,
            PlayerCommand,
            PlayerRevive,
            PlayerReportF7,
            PlayerLooted,
            PlayerKill,
            PlayerDamage,
            PlayerKicked,
            PlayerLogin,
            PlayerConnect,
            PlayerDisconnect,

            PlayerTeamCreate,
            PlayerTeamInvite,
            PlayerTeamInviteAccept,
            PlayerTeamDisband,
            PlayerTeamInviteReject,
            PlayerTeamLeaderPromote,
            PlayerTeamLeave,
            PlayerTeamKick,

            PlayerResourceGathered,
       
            PlayerPruahceVendingMachine,



            ItemDrop,
            ItemPickup,

            WeaponFired,
            WeaponHit,

            VicheleUse,

            ShopWindowTrade,


            ToolCupAuth,
            ToolCupDeAuth,


            BuildingBuild,
            BuildingDestory,



            
        }
        internal enum LogType
        {
            Info,
            Warning,
            Error,
            Important,
        }

        internal static void LogMessage(string message, LogType logType = LogType.Info, bool diskLog = true, string prefix = null, [CallerFilePath] string filePath = null, [CallerMemberName] string member = null, [CallerLineNumber] int line = 0)
        {
            try
            {
                string name = !String.IsNullOrEmpty(prefix) ? prefix : System.IO.Path.GetFileNameWithoutExtension(filePath);
                string logMessage = $"[{name}]    [{logType}]    {member}:{line}    {message}";
#if CONSOLE_LOGGING
                Debug.LogError(logMessage); 
#endif
                if (diskLog)
                {
                    Logger?.Log($"{logType}", $"{DateTime.Now.ToString("yyyy:MM:dd    HH:mm:ss")}    {logMessage}");
                }
            } catch { }
        }
    }
}