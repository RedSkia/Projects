using Newtonsoft.Json;
using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LIB.RustRcon
{
    public class Client
    {
        /// <summary>
        /// Disconnects <paramref name="webSocket"/>
        /// </summary>
        public static async Task Disconnect(ClientWebSocket webSocket)
        {
            await Send(webSocket, "Disconnected", TypeIdentifiers.Generic);
            await webSocket?.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
            webSocket?.Dispose();
        }

        /// <returns>Rcon connection <see cref="ClientWebSocket"/></returns>
        public static async Task<ClientWebSocket> Connect(string Host, int Port, string Pass)
        {
            try
            {
                var client = new ClientWebSocket();
                await client?.ConnectAsync(new Uri($"ws://{Host}:{Port}/{Pass}"), CancellationToken.None);
                await Send(client, "Connected", TypeIdentifiers.Generic);
                return client;
            }
            catch (Exception ex) { return null; }
        }

        /// <summary>
        /// <list type="table">Sends a <see cref="TypeIdentifiers.Generic"/> Command</list>
        /// <list type="table">OR</list>
        /// <list type="table">Sends a <see cref="TypeIdentifiers.Chat"/> Message</list>
        /// </summary>
        public static async Task Send(ClientWebSocket webSocket, string message, TypeIdentifiers type)
        {
            RconRequest request = new RconRequest();
            request.Message = type == TypeIdentifiers.Chat ? $"say {message}" : message;
            request.Identifier = (int?)type;
            request.Type = type;
            await webSocket.SendAsync(new ArraySegment<byte>(new UTF8Encoding().GetBytes(JsonConvert.SerializeObject(request))), WebSocketMessageType.Text, true, CancellationToken.None);
        }

        /// <returns>All Messages From Connection</returns>
        public static async Task<string> Receive(ClientWebSocket webSocket, int limit)
        {
            byte[] buffer = new byte[limit];
            if (webSocket?.State == WebSocketState.Open)
            {
                var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Close) { Disconnect(webSocket).Wait(); };
                return GetReply(buffer);
            }
            return null;
        }

        internal static string GetReply(byte[] buffer) => new UTF8Encoding()?.GetString(buffer);

        public enum TypeIdentifiers
        {
            Chat = -1,
            Generic = 0,
        }
        internal class RconRequest
        {
            public int? Identifier { get; set; }
            public TypeIdentifiers Type { get; set; }
            public string Message { get; set; }
        }
    }
}