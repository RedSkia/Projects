using Oxide.Core;

namespace CoreRP
{
    public enum GametipType
    {
        Info = '0',
        Warning = '1'
    }
    public static class PlayerEx
    {
        public static bool IsPlayer(this BasePlayer player, bool RequireConnected = false) =>
            (player != null && (player?.userID ?? 0).IsSteamId()) ||
            (player != null && (player?.userID ?? 0).IsSteamId() && RequireConnected && (player?.Connection?.connected ?? false));
        public static bool IsTeam(this BasePlayer host, ulong member)
        {
            if (!IsPlayer(host, true) || !member.IsSteamId()) { return false; }
            if (host?.userID == member) { return true; }
            if (host?.Team == null || host?.Team?.members?.Count <= 1) { return false; }
            return host?.Team?.members?.Contains(member) ?? false;
        }
        public static void SendMsg(this BasePlayer player, string title, string message, string titleColorHex = "#faaf19") => player?.SendConsoleCommand("chat.add", 2, 0, "[<color=" + (titleColorHex.StartsWith("#") ? $"{titleColorHex}" : $"#{titleColorHex}") + $">{title}</color>]\n{message}");
        public static void ShowGametip(this BasePlayer player, string Message, GametipType type = GametipType.Info) => player?.SendConsoleCommand($"gametip.showtoast {(char)type} \"{Message}\"");
    }
}