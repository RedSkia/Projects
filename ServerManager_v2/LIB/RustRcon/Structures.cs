using Newtonsoft.Json;
using System.Collections.Generic;

namespace LIB.RustRcon
{
    public class JsonStructures
    {
        public class RCON
        {
            public class ServerInfo
            {
                public string Hostname { get; set; }
                public int MaxPlayers { get; set; }
                public int Players { get; set; }
                public int Queued { get; set; }
                public int Joining { get; set; }
                public int EntityCount { get; set; }
                public string GameTime { get; set; }
                public int Uptime { get; set; }
                public string Map { get; set; }
                public double Framerate { get; set; }
                public int Memory { get; set; }
                public int Collections { get; set; }
                public int NetworkIn { get; set; }
                public int NetworkOut { get; set; }
                public bool Restarting { get; set; }
                public string SaveCreatedTime { get; set; }
            }
            public class Player
            {
                public class Banned
                {
                    public string SteamID { get; set; }
                    public string DisplayName { get; set; }
                    public string Reason { get; set; }
                    public string Expire { get; set; }
                }
                public class Connected
                {
                    public string SteamID { get; set; }
                    public string OwnerSteamID { get; set; }
                    public string DisplayName { get; set; }
                    public int Ping { get; set; }
                    public string Address { get; set; }
                    public int ConnectedSeconds { get; set; }
                    public double VoiationLevel { get; set; }
                    public double CurrentLevel { get; set; }
                    public double UnspentXp { get; set; }
                    public double Health { get; set; }
                }
            }
        }

        public class STEAM
        {
            public class Bans
            {
                [JsonProperty("players")]
                public Player[] list { get; set; }

                public class Player
                {
                    public string SteamId { get; set; }
                    public bool CommunityBanned { get; set; }
                    public bool VACBanned { get; set; }
                    public int NumberOfVACBans { get; set; }
                    public int DaysSinceLastBan { get; set; }
                    public int NumberOfGameBans { get; set; }
                    public string EconomyBan { get; set; }
                }
            }
        }
    }
}