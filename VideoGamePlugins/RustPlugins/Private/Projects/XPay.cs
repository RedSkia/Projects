using Oxide.Core;
using Oxide.Core.Plugins;
using System;

namespace Oxide.Plugins
{

    [Info("XPay", "InfinityNet/Xray", "1.0.0")]
    [Description("Copyright Infinitynet.dk All Rights Reserved")]
    public class XPay : RustPlugin
    {
        [PluginReference] private readonly Plugin ServerRewards;
        private static XPay Instance { get; set; }
        private const string VIPGroup = "VIP";
        private const string SignatureColor = "faaf19";
        private const string DiscordSupportURL = "discord.gg/PuRJjhcgtG";
        private readonly string[] StaffGroups = new string[]
        {
            "administrator",
            "external",
            "internal"
        };
        #region Init
        private void Loaded()
        {
            Instance = this;

            cmd.AddChatCommand("buyvip", this, "PayCoins");
            cmd.AddChatCommand("vipbuy", this, "PayCoins");
            cmd.AddChatCommand("vip", this, "VipCheck");
        }
        #endregion Init
        #region Configuration
        private const int CoinAmount = 10000;
        private const string CoinPrices = "(Coins)";
        private const string CoinCurrency = "COIN";

        private static readonly string[] AprovedCashPrices = { "8.00", "7.30", "6.70", "6.00", "5.30", "4.60", "4.00", "3.20", "2.50" };
        private static readonly string AprovedCurrency = "EUR";
        private static readonly string AprovedStatus = "COMPLETED";

        #endregion Configuration

        private void PayCoins(BasePlayer player) => VIP.BuyWithCoins(player);
        private void VipCheck(BasePlayer player) => VIP.IsVIP(player, true);

        void OnNewSave(string filename) => VIP.WIPE();

        private class VIPData
        {
            public string status { get; set; }
            public string currency { get; set; }
            public string price { get; set; }
            public string timeStamp { get; set; }
            public PayerPersonalData personalData { get; set; }

            public class PayerPersonalData
            {
                public string first_name { get; set; }
                public string last_name { get; set; }
                public string email { get; set; }
                public string country_code { get; set; }
            }
        }
        private class VIP
        {
            public static void WIPE()
            {
                var i = Interface.Oxide.DataFileSystem.GetFiles($"{Instance.Name}");
                foreach (var file in i) { Interface.Oxide.DataFileSystem.WriteObject(file.Replace(".json",""), $"Data Wiped: {DateTime.Now.ToString("yyyy/MM/dd:HH:mm:ss")}", true); }
            }

            public static void BuyWithCoins(BasePlayer player)
            {
                if(!VIP.IsVIP(player, true))
                {
                    var coins = Convert.ToInt32(Instance.ServerRewards?.Call("CheckPoints", player.userID));
                    if (coins < CoinAmount)
                    {
                        Instance.DirectMessage(player, $"Not enough coins to purchase {TextEncodeing(16, SignatureColor, "VIP PASS", true)}\n" +
                            $"Acquire {TextEncodeing(16, "4be1fa", $"{CoinAmount - coins}", true)} Coin's  more \n" +
                            $"Or purchase {TextEncodeing(16, SignatureColor, "VIP PASS", true)} At {TextEncodeing(16, "4be1fa", $"Infinitynet.dk", true)}");
                        return;
                    }
                    else if (coins >= CoinAmount)
                    {
                        Interface.Oxide.DataFileSystem.WriteObject($"{Instance.Name}/{player.userID}", new VIPData
                        {
                            currency = "COIN",
                            price = $"{CoinAmount} {CoinPrices}",
                            status = "COMPLETED",
                            timeStamp = DateTime.Now.ToString("yyyy/MM/dd:HH:mm:ss"),
                            personalData = new VIPData.PayerPersonalData
                            {
                                first_name = player?.displayName,
                                last_name = player?._lastSetName,
                                country_code = player.IPlayer.Language.ToString(),
                                email = "NULL",
                            },
                        });
                        Instance.permission.AddUserGroup(player.UserIDString, VIPGroup);
                        Instance.ServerRewards?.Call("TakePoints", player.userID, CoinAmount);
                        Instance.DirectMessage(player, $"Successfully purchased {TextEncodeing(16, SignatureColor, "VIP PASS", true)} Enjoy");
                    }
                } 
            }

            public static bool IsVIP(BasePlayer player, bool ShowMessage)
            {
                //Staff?
                foreach (var group in Instance.StaffGroups)
                {
                    if (Instance.permission.UserHasGroup(player.UserIDString, group))
                    {
                        if (ShowMessage)
                        {
                            if (!Instance.permission.UserHasGroup(player.UserIDString, VIPGroup))
                            {
                                Instance.DirectMessage(player, $"{TextEncodeing(16, SignatureColor, "VIP PASS", true)} Features {TextEncodeing(16, "4be1fa", $"Activated", true)}\n" +
                                $"Staff Rank Detected");
                            }
                            else
                            {
                                Instance.DirectMessage(player, $"{TextEncodeing(16, SignatureColor, "VIP PASS", true)} Features already {TextEncodeing(16, "4be1fa", $"Activated", true)}\n" +
                                $"SteamID: {TextEncodeing(16, "4be1fa", player.UserIDString, true)} Staff Rank Detected");
                            }
                        }
                        Instance.permission.AddUserGroup(player.UserIDString, VIPGroup);
                        return true;
                    }
                }

                if (Interface.Oxide.DataFileSystem.GetFile($"{Instance.Name}/{player.userID}").Exists())
                {
                    var Data = Interface.Oxide.DataFileSystem.ReadObject<VIPData>($"{Instance.Name}/{player.userID}");
    
                    if (Data.status == AprovedStatus && (AprovedCurrency == Data.currency || CoinCurrency == Data.currency) && (AprovedCashPrices.Contains(Data.price) || $"{CoinAmount} {CoinPrices}" == Data.price))
                    {
                        if (ShowMessage)
                        {
                            if (!Instance.permission.UserHasGroup(player.UserIDString, VIPGroup))
                            {
                                Instance.DirectMessage(player, $"{TextEncodeing(16, SignatureColor, "VIP PASS", true)} Features {TextEncodeing(16, "4be1fa", $"Activated", true)}\n" +
                                $"Thank you for supporting the server");
                            }
                            else
                            {
                                Instance.DirectMessage(player, $"{TextEncodeing(16, SignatureColor, "VIP PASS", true)} Features already {TextEncodeing(16, "4be1fa", $"Activated", true)}\n" +
                                $"SteamID: {TextEncodeing(16, "4be1fa", player.UserIDString, true)}");
                            }
                        }
                        Instance.permission.AddUserGroup(player.UserIDString, VIPGroup);
                        return true;
                    }
                    else //NO VIP
                    {
                        Instance.permission.RemoveUserGroup(player.UserIDString, VIPGroup);
                        if (ShowMessage)
                        {
                            Instance.DirectMessage(player, $"{TextEncodeing(16, SignatureColor, "VIP PASS", true)} Not detected\n" +
                                $"Steamid: ({TextEncodeing(16, "4be1fa", player.UserIDString, true)})\n" +
                                $"If you didn't receive {TextEncodeing(16, SignatureColor, "VIP PASS", true)} after purchase\n" +
                                $"Visit our Discord support: {TextEncodeing(16, "4be1fa", DiscordSupportURL, true)}");
                        }
                    }
                }
                else //NO VIP
                {
                    Instance.permission.RemoveUserGroup(player.UserIDString, VIPGroup);
                    if (ShowMessage)
                    {
                        Instance.DirectMessage(player, $"{TextEncodeing(16, SignatureColor, "VIP PASS", true)} Not detected\n" +
                            $"Steamid: ({TextEncodeing(16, "4be1fa", player.UserIDString, true)})\n" +
                            $"If you didn't receive {TextEncodeing(16, SignatureColor, "VIP PASS", true)} after purchase\n" +
                            $"Visit our Discord support: {TextEncodeing(16, "4be1fa", DiscordSupportURL, true)}");
                    }
                }
                return false;
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
    }
}