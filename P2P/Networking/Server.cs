using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Networking
{
    public sealed class Server(ushort port, byte maxClients = 2, ushort tickInterval = 1000, AuthenticationToken token = default) : TcpServer(port, maxClients, tickInterval, token)
    {
        public override bool Post(string content, byte clientId = 0)
        {
            return base.Post(content, clientId);
        }
    }
}
