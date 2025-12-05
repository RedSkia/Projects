using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LIB.RustServer
{
    public class Manage
    {
        public enum SeverType
        {
            selfLocal,
            dirtyLocal,
            rcon,
        }

        private static Dictionary<string, ServerData> serverDict = new Dictionary<string, ServerData>();

        public static KeyValuePair<string, ServerData>[] GetServers() => serverDict.ToArray();
  

        public static void RenameServer(string currentKey,string newKey)
        {

        }

        public static ServerData GetServer(string key)
        {
            if(serverDict.ContainsKey(key))
            {
                var server = serverDict[key];
                switch (server.type)
                {
                    case SeverType.rcon:
                        return new ServerData.Rcon
                        {
                            type = SeverType.rcon,
                            Host = server.rcon.Host,
                            Pass = server.rcon.Pass,
                            Port = server.rcon.Port,
                        };
                }
            }
            return null;
        }

        public static void LoadServers()
        {
            SaveServers();
            serverDict = JsonConvert.DeserializeObject<Dictionary<string, ServerData>>(JsonConvert.SerializeObject(LIB.Settings.CORE.JsonManager.LoadFile(LIB.Settings.CORE.FileRef.Server)));
        }

        public static void SaveServers()
        {
            LIB.Settings.CORE.JsonManager.SaveFile(LIB.Settings.CORE.FileRef.Server, serverDict);
        }

        public class Add
        {
            public static void SelfLocal()
            {

            }
            public static void DirtyLocal(string ServName, ServerData.DirtyLocal dirty, [Optional] ServerData.Rcon rcon)
            {
                if (serverDict.ContainsKey(ServName)) { return; }
                serverDict.Add(ServName, new ServerData.DirtyLocal
                {
                    type = SeverType.dirtyLocal,
                    Root = dirty.Root,
                    StartBat = dirty.StartBat,
                    SteamCMD = dirty.SteamCMD,
                    Oxide = dirty.Oxide,
                    rcon = new ServerData.Rcon
                    {
                        Host = dirty.rcon.Host,
                        Port = dirty.rcon.Port,
                        Pass = dirty.rcon.Pass,
                    }
                });
            }

            public static void Rcon(string ServName, ServerData.Rcon rcon)
            {
     
                if (serverDict.ContainsKey(ServName)) { return; }
                serverDict.Add(ServName, new ServerData.Rcon
                {
                    type = SeverType.rcon,
                    Host = rcon.Host,
                    Port = rcon.Port,
                    Pass = rcon.Pass,
                });
            }
        }


        public class ServerData
        {
            public SeverType type { get; set; }

            public SelfLocal selfLocal { get; set; }
            public class SelfLocal : ServerData
            {
                public Rcon rconData { get; set; }
 
            }

            public DirtyLocal dirtyLocal { get; set; }
            public class DirtyLocal : ServerData
            {
                public Rcon rconData { get; set; }
                public string Root { get; set; }
                public string StartBat { get; set; }
                public string SteamCMD { get; set; }
                public string Oxide { get; set; }
            }

            public Rcon rcon { get; set; }
            public class Rcon : ServerData
            {
                public string Host { get; set; }
                public int Port { get; set; }
                public string Pass { get; set; }
            }
        };
    }
}
