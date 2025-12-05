using Newtonsoft.Json;
using Oxide.Core;
using Oxide.Core.Libraries.Covalence;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

using Oxide.Core.Configuration;
using ConVar;
using Network;

namespace Oxide.Plugins
{
    [Info("XLogs", "Infinitynet.dk/Xray", "1.0.0")]
    [Description("Copyright Infinitynet.dk All Rights Reserved")]
    public class XLogs : RustPlugin
    {
        private const int UpdateDelaySeconds = 60;

        Timer tempTimer;
        #region Init
        void OnServerInitialized(bool initial)
        {
            if(initial) { tempTimer = timer.Repeat(UpdateDelaySeconds, 0, () => { UpdateServerData(); }); }
        }
        void Unload()
        {
            try{ tempTimer.Destroy();}
            catch { }
        }
        #endregion Init

        private void UpdateServerData()
        {
            var dataObj = new ServerData
            {
                HostName = Admin.ServerInfo().Hostname,
                Description = ConVar.Server.description,
                Framerate = Admin.ServerInfo().Framerate,
                FpsLimit = ConVar.FPS.limit,
                MapSeed = ConVar.Server.seed,
                MapSize = ConVar.Server.worldsize,
                MapType = Admin.ServerInfo().Map,
                Tags = ConVar.Server.tags,
                GamePort = ConVar.Server.port,
                Queued = Admin.ServerInfo().Queued,
                Slots = Admin.ServerInfo().MaxPlayers,
                Players = Admin.ServerInfo().Players,
                EntCount = Admin.ServerInfo().EntityCount,
                Joining = Admin.ServerInfo().Joining,
                Uptime = Admin.ServerInfo().Uptime,
                Memory = Admin.ServerInfo().Memory,
                Collections = Admin.ServerInfo().Collections,
                NetworkIn = Admin.ServerInfo().NetworkIn,
                NetworkOut = Admin.ServerInfo().NetworkOut,
                Restarting = Admin.ServerInfo().Restarting,
                SaveCreatedTime = Admin.ServerInfo().SaveCreatedTime,
                Protocol = Rust.Protocol.printable,
                GameTime = Admin.ServerInfo().GameTime,
            };
            Interface.Oxide.DataFileSystem.WriteObject($"{Name.ToUpper()}/Data", dataObj); 
        }

        private class ServerData
        {
            public string HostName { get; set; }
            public string Description { get; set; }
            public float Players { get; set; }
            public float Joining { get; set; }
            public float Queued { get; set; }
            public float Slots { get; set; }
            public float Framerate { get; set; }
            public float FpsLimit { get; set; }
            public float GamePort { get; set; }
            public float NetworkIn { get; set; }
            public float NetworkOut { get; set; }
            public string SaveCreatedTime { get; set; }
            public string Protocol { get; set; }
            public float Collections { get; set; }
            public float Uptime { get; set; }
            public float Memory { get; set; }
            public float EntCount { get; set; }
            public bool Restarting { get; set; }
            public string GameTime { get; set; }
            public string Tags { get; set; }
            public string MapType { get; set; }
            public float MapSeed { get; set; }
            public float MapSize { get; set; }
        }    
    }
}