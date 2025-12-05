using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace Networking
{
    public interface IReadOnlyServer
    {
        public IPEndPoint EndPoint { get; }
        public IReadOnlyDictionary<byte, TcpClient> Clients { get; }
    }
    public interface ITcpServer : IReadOnlyServer
    {
        public void Start();
        public void Stop();
        public bool Post(string content, byte clientId = 0);
    }
}