using System.Collections.Generic;
using UnityEngine;

namespace CoreRP.ZoneManager
{
    internal sealed class ZoneObject
    {
        internal HashSet<ulong> Players { get; set; } = new HashSet<ulong>(); /*Players inside zone*/
        internal ZoneColliderBehaviour Collider { get; private set; }
        public string Prefix { get; set; }
        public bool IsOwnable { get; set; }
        public ulong OwnerId { get; set; }
        public ZoneFlagsAllowed Flags { get; set; }
        public ZoneRect Rect { get; set; }

        public ZoneObject(string zonePrefix, ZoneRect zoneRect, ZoneFlagsAllowed zoneFlags = ZoneFlagsAllowed.None, bool isOwnable = false)
        {
            this.Prefix = zonePrefix;
            this.Flags = zoneFlags;
            this.Rect = zoneRect;
            this.IsOwnable = isOwnable;
        }
        internal bool SetOwner(ulong ownerid)
        {
            Script.Instance.ScriptCheck();
            if (!IsOwnable) { return false; }
            this.OwnerId = ownerid;
            FileLogger.LogMessage($"Set Owner: ({ownerid})", diskLog: false);
            return true;
        }
        internal void InitCollider()
        {
            Script.Instance.ScriptCheck();
            if(Collider != null) { return; }
            this.Collider = new GameObject("ZoneManager_ZoneColliderBehaviour").AddComponent<ZoneColliderBehaviour>();
            Collider.Init(this);
        }
        internal void Dispose()
        {
            Collider?.Dispose();
            Collider = null;
            Players?.Clear();
        }
    }
}