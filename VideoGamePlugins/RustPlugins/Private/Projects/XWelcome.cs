using static ConVar.Admin;

namespace Oxide.Plugins
{

    [Info("XWelcome", "InfinityNet/Xray", "1.0.0")]
    [Description("Copyright Infinitynet.dk All Rights Reserved")]
    public class XWelcome : RustPlugin
    {
        public static string TextEncodeing(double TextSize, string HexColor, string PlainText) => $"<size={TextSize}><color=#{HexColor}>{PlainText}</color></size>";

        private static void DirectMessage(BasePlayer player, string message) => player.SendConsoleCommand("chat.add", 0, 76561198389709969, $"{TextEncodeing(16, "4be1fa", "Infinitynet.dk")}{TextEncodeing(16, "ffffff", ":")} {TextEncodeing(16, "ffffff", message)}");

        void OnPlayerDisconnected(BasePlayer player, string reason) => player.SendConsoleCommand("chat.add", 2, 76561198389709969, $"{TextEncodeing(16, "4be1fa", $"{player.displayName} Left")}");

        void OnPlayerConnected(BasePlayer player)
        {
            player.SendConsoleCommand("chat.add", 2, 76561198389709969, $"{TextEncodeing(16, "4be1fa", $"{player.displayName}")} {TextEncodeing(16, "ffffff", $"Joined")}");

            timer.Once(10f, () =>
            {
                DirectMessage(player, $"Welcome to our server \n" +
                    $"{TextEncodeing(16, "4be1fa", $"({ConVar.Server.hostname})")}\n" +
                    $"There're currently {TextEncodeing(16, "4be1fa", $"{ServerInfo().Players}")}/{TextEncodeing(16, "4be1fa", $"{ServerInfo().MaxPlayers}")} players online\n" +
                    $"Visit our website {TextEncodeing(16, "4be1fa", $"Infinitynet.dk")}\n" +
                    $"To join our Discord & Steam\n" +
                    $"For a list of commands do {TextEncodeing(16, "4be1fa", $"/help")}");
            });
        }
    }
}