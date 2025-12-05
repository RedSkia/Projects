using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

namespace LIB.RustRcon
{
    public class Get
    {
        public class RCON
        {

            /// <returns>Response from <paramref name="client"/></returns>
            private static async Task<object> Response(ClientWebSocket client, int limit, string message)
            {
                byte[] buffer = new byte[limit];
                await Client.Send(client, message, Client.TypeIdentifiers.Generic);
                await client.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                return Client.GetReply(buffer);
            }

            public static async Task<JsonStructures.RCON.ServerInfo> ServerInfo(ClientWebSocket client, int limit)
            {
                var get = (string)await Response(client, limit, "serverinfo");
                var msg = JsonConvert.DeserializeObject<Client.RconRequest>(get);
                var result = JsonConvert.DeserializeObject<JsonStructures.RCON.ServerInfo>(msg.Message);
                return result;
            }

            public static async Task<JsonStructures.RCON.Player.Connected> Player(ClientWebSocket client, int limit, ulong id)
            {
                var get = (string)await Response(client, limit, "playerlist");
                var msg = JsonConvert.DeserializeObject<Client.RconRequest>(get);
                var arry = JsonConvert.DeserializeObject<JsonStructures.RCON.Player.Connected[]>(msg.Message);
                var result = Array.Find(arry, x => x.SteamID.Equals(Convert.ToString(id)));
                return result;
            }

            public static async Task<JsonStructures.RCON.Player.Connected[]> PlayerAll(ClientWebSocket client, int limit)
            {
                var get = (string)await Response(client, limit, "playerlist");
                var msg = JsonConvert.DeserializeObject<Client.RconRequest>(get);
                var result = JsonConvert.DeserializeObject<JsonStructures.RCON.Player.Connected[]>(msg.Message);
                return result;
            }
        }

        public class STEAM
        {
            private static Dictionary<ulong, JsonStructures.STEAM.Bans.Player> BansDict = new Dictionary<ulong, JsonStructures.STEAM.Bans.Player>();

            private static DateTimeOffset LastUpdateTime { get; set; }

            /// <summary>
            /// Required <paramref name="Seconds"/> Before Updateing <see cref="BansDict"/>
            /// </summary>
            /// <param name="Seconds"></param>
            private static void UpdateBansDict(int Seconds)
            {
                if(DateTimeOffset.UtcNow >= LastUpdateTime.AddSeconds(Seconds))
                {
                    BansDict.Clear();
                    LastUpdateTime = DateTimeOffset.UtcNow;
                }
            }

            /// <summary>
            /// Adds New <paramref name="SteamIds"/> To <see cref="BansDict"/> IF Keys Not Exist
            /// </summary>
            /// <returns><paramref name="SteamIds"/></returns>
            public static async Task<JsonStructures.STEAM.Bans.Player[]> PlayerBan(string APIKey, ulong[] SteamIds)
            {
                //Update if X Time Passed
                UpdateBansDict(300);

                var StringIds = new string[SteamIds.Length];
                for (int i = 0; i < SteamIds.Length; i++) StringIds[i] = SteamIds[i].ToString();

                var newKeys = StringIds.Where(x => !BansDict.ContainsKey(Convert.ToUInt64(x))).ToArray();
                if(newKeys.Count() > 0)
                {
                    var response = await PlayerBansAll(APIKey, StringIds);
                    /*Cache New Ids IF NOT EXISTS*/
                    for (int i = 0; i < response.list.Length; i++)
                    {
                        var id = Convert.ToUInt64(response?.list[i].SteamId);
                        var target = response.list[i];
                        if (BansDict.ContainsKey(id)) continue;
                        BansDict.Add(id, new JsonStructures.STEAM.Bans.Player
                        {
                            NumberOfVACBans = target.NumberOfVACBans,
                            VACBanned = target.VACBanned,
                            CommunityBanned = target.CommunityBanned,
                            DaysSinceLastBan = target.DaysSinceLastBan,
                            EconomyBan = target.EconomyBan,
                            NumberOfGameBans = target.NumberOfGameBans,
                            SteamId = target.SteamId,
                        });
                    }
                }
                return BansDict.Where(x => SteamIds.Contains(x.Key)).Select(x => x.Value).ToArray();
            }

   
            /// <returns>API data from steam by <paramref name="SteamIds"/></returns>
            private static async Task<JsonStructures.STEAM.Bans> PlayerBansAll(string APIKey, string[] SteamIds)
            {
                using(HttpClient client = new HttpClient())
                {
                    var URL = $"https://api.steampowered.com/ISteamUser/GetPlayerBans/v1/?key={APIKey}&steamids={String.Join(",", SteamIds)}";
                    HttpResponseMessage response = await client.GetAsync(URL);
                    response.EnsureSuccessStatusCode();
                    string jsonString = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<JsonStructures.STEAM.Bans>(jsonString);
                };
            }
        }
	}
}