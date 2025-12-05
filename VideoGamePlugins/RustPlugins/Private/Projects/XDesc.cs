namespace Oxide.Plugins
{
    [Info("XDesc", "Infinitynet.dk/Xray", "1.0.0")]
    [Description("Copyright Infinitynet.dk All Rights Reserved")]
    public class XDesc : RustPlugin
    {
        public const string Description = "InfinityNet.dk offers a unique PvE experience with 100 highly customized plugins\n\nINFORMATION\n● Hostile timer disabled\n● Player- and storage looting disabled (unless in the same team)\n● Player- and building damage disabled\n● Turrets targeting players disabled\n● Wipe cycle: monthly map wipes (Blue prints NEVER WIPES)\nUse /help in the chat for a full list of commands and helpful information\n\nSOCIALS\n● Website: infinitynet.dk\n● Discord: discord.infinitynet.dk\n● Steam: steam.infinitynet.dk\n\nFEATURES\n● Blueprints, coins and /backpack are persistent through server wipes\n● Custom developed plugins\n● Improved spawn items given upon respawn\n● Improved Bradley with better loot and more challenging to beat\n● Cargo planes may crash at a random location with GREAT loot\n● Crafting time heavily reduced\n● Custom event with NPCs roaming around highly valuable treasures\n● Many, many Quality Of Life improvements\n● Minimalistic information GUI\n● Custom purge event 48 hours before server wipe\n● Custom zombies roaming the map\n\nRULES\n● Share the loot at public events if multiple players (be fair to fellow players)\n● No exploiting of plugins or server features\n● No raiding, killing, shooting, looting, griefing, monument camping\n● No monument/road controlled/interruption bases, multi-TC bases, overtaking bases, trap/harmful bases, mass grid claiming bases";

        private void OnServerInitialized()
        {
            ConVar.Server.description = Description;
        }
    }
}