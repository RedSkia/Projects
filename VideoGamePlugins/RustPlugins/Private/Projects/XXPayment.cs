using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Oxide.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Oxide.Plugins
{
    [Info("PaymentHandler", "Xray", "1.0.0")]
    class PaymentHandler : RustPlugin
    {
        #region Configuration

        private Configuration config;

        //Default Config
        public class Configuration
        {
            [JsonProperty("VIP Group Name")]
            public string VIPGroupName { get; set; } = "vip";


            [JsonProperty("Discord Support URL")]
            public string DiscordSupportURL { get; set; } = "discord.gg/u7gQwQPERX";


            [JsonProperty("What should the payment folder be named in Oxide/Data")]
            public string PaymentFolderName { get; set; } = "PAYMENTS";

            [JsonProperty("Payment Price Check")]
            public string[] PaymentPrices { get; set; } = { "8.00", "7.30", "6.70", "6.00", "5.30", "4.60", "4.00", "3.20", "2.50" };

            [JsonProperty("Payment Currency Check")]
            public string PaymentCurrency { get; set; } = "EUR";
        }

        protected override void LoadConfig()
        {
            base.LoadConfig();
            try
            {
                config = Config.ReadObject<Configuration>();
                if (config == null)
                {
                    LoadDefaultConfig();
                }
            }
            catch
            {
                LoadDefaultConfig();
            }
            SaveConfig();
        }

        protected override void LoadDefaultConfig()
        {
            string configPath = $"{Interface.Oxide.ConfigDirectory}{Path.DirectorySeparatorChar}{Name}.json";
            PrintWarning("[ERROR] Invalid Configuration Overwriting");
            config = new Configuration();
        }

        protected override void SaveConfig() => Config.WriteObject(config);

        #endregion Configuration


        string TextEncodeing(double TextSize, string HexColor, string PlainText) => $"<size={TextSize}><color=#{HexColor}>{PlainText}</color></size>";


        [ChatCommand("vip")]
        private void checkChatCommand(BasePlayer player) => CheckVIP(player, true);

        void OnPlayerConnected(BasePlayer player) => CheckVIP(player, false);

        void OnPlayerDisconnected(BasePlayer player, string reason) => CheckVIP(player, false);


        void CheckVIP(BasePlayer player, bool ShowMessage)
        {
            if (Regex.IsMatch(player.UserIDString, @"\d+"))
            {
                string SteamID = Regex.Match(player.UserIDString, @"\d+").Value;

                if (Interface.Oxide.DataFileSystem.GetFile($"{config.PaymentFolderName}/{SteamID}").Exists())
                {
                    var DataObj = Interface.Oxide.DataFileSystem.ReadObject<object>($"{config.PaymentFolderName}/{SteamID}");

                    string output = JsonConvert.SerializeObject(DataObj);
                    JObject json = JObject.Parse(output);

                    string Payment_Status = (string)json["status"];
                    string Payment_Price = (string)json["purchase_units"][0]["amount"]["value"];
                    string Payment_Currency = (string)json["purchase_units"][0]["amount"]["currency_code"];

                    if (config.PaymentPrices.Contains(Payment_Price) && config.PaymentCurrency == Payment_Currency && Payment_Status == "COMPLETED")
                    {
                        if (player.IPlayer.BelongsToGroup(config.VIPGroupName)) //Already Activated
                        {
                            if (ShowMessage)
                            {
                                SendReply(player, $"" +
                                    $"{TextEncodeing(16, "c83232", "Server")}{TextEncodeing(14, "ffffff", ":")}" +
                                    $" {TextEncodeing(14, "3296fa", "VIP PASS")} {TextEncodeing(14, "ffffff", "Activated for")} {TextEncodeing(14, "ffffff", "(")}{TextEncodeing(14, "3296fa", SteamID)}{TextEncodeing(14, "ffffff", ")")}" +
                                    $"\n{TextEncodeing(14, "ffffff", "Thank you for supporting the server")}"
                                );
                            }
                        }
                        else //First Time Activate
                        {
                            player.IPlayer.AddToGroup(config.VIPGroupName);
                            if (ShowMessage)
                            {
                                SendReply(player, $"" +
                                    $"{TextEncodeing(16, "c83232", "Server")}{TextEncodeing(14, "ffffff", ":")}" +
                                    $" {TextEncodeing(14, "3296fa", "VIP PASS")} {TextEncodeing(14, "ffffff", "Features are now")} {TextEncodeing(14, "f47fff", "Activated")} \n{TextEncodeing(14, "ffffff", "Steamid:")} {TextEncodeing(14, "ffffff", "(")}{TextEncodeing(14, "3296fa", SteamID)}{TextEncodeing(14, "ffffff", ")")}" +
                                    $"\n{TextEncodeing(14, "ffffff", "Thank you for supporting the server")}"
                                );
                            }
                        }
                    }
                    else //Payment File Error worng Price,Currency,Status
                    {
                        player.IPlayer.RemoveFromGroup(config.VIPGroupName);
                        if (ShowMessage)
                        {
                            SendReply(player, $"" +
                               $"{TextEncodeing(16, "c83232", "Server")}{TextEncodeing(14, "ffffff", ":")}" +
                               $" {TextEncodeing(14, "3296fa", "VIP PASS")} {TextEncodeing(14, "ffffff", "ERROR Activation failed")}" +
                               $"{TextEncodeing(14, "ffffff", $"\nContact our Discord support:")} {TextEncodeing(14, "3296fa", config.DiscordSupportURL)}"
                           );
                        }
                    }
                }
                else //No VIP
                {
                    player.IPlayer.RemoveFromGroup(config.VIPGroupName);
                    if (ShowMessage)
                    {
                        SendReply(player, $"" +
                             $"{TextEncodeing(16, "c83232", "Server")}{TextEncodeing(14, "ffffff", ":")}" +
                             $" {TextEncodeing(14, "3296fa", "VIP PASS")} {TextEncodeing(14, "ffffff", "Not detected")}" +
                             $"\n{TextEncodeing(14, "ffffff", "Steamid:")} {TextEncodeing(14, "ffffff", "(")}{TextEncodeing(14, "3296fa", SteamID)}{TextEncodeing(14, "ffffff", ")")}" +
                             $"{TextEncodeing(14, "ffffff", $"\nIf you didn't receive VIP PASS after purchase")}" +
                             $"{TextEncodeing(14, "ffffff", $"\nVisit our Discord support:")} {TextEncodeing(14, "3296fa", config.DiscordSupportURL)}"
                         );
                    }
                }
            }
        }
    }
}