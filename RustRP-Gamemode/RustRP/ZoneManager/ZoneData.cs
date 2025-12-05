using System;
using System.Collections.Generic;
using UnityEngine;

namespace CoreRP.ZoneManager
{
    internal static class ZoneData
    {
        internal readonly static ZoneDataLoader Instance = new ZoneDataLoader();
    }

    internal sealed class ZoneDataLoader : Contracts.BaseData<Dictionary<string, ZoneObject>>
    {
        internal override string FileName => "ZoneData";
        internal override Dictionary<string, ZoneObject> LoadData(string args = "")
        {
            Script.Instance.ScriptCheck();
            base.LoadData(args);
            foreach (var zone in CachedData)
            {
                /*Init New ZoneRect*/
                CachedData[zone.Key].Rect = new ZoneRect(zone.Value.Rect.Points, zone.Value.Rect.Height, zone.Value.Rect.Rotation);
            }
            KillColliders();
            InitColliders();
            return null;
        }
        internal override void SaveData(string args = "", object data = null) => base.SaveData(args, data);

        internal ZoneObject GetZone(string zoneName)
        {
            Script.Instance.ScriptCheck();
            if (String.IsNullOrEmpty(zoneName)) { return null; }
            ZoneObject zone; CachedData.TryGetValue(zoneName.ToLower(), out zone);
            return zone;
        }
        internal Dictionary<string, ZoneObject> GetZones()
        {
            Script.Instance.ScriptCheck();
            return new Dictionary<string, ZoneObject>(CachedData);
        }
        internal bool AddZone(string zoneName, ZoneObject zone)
        {
            Script.Instance.ScriptCheck();
            if (zone == null || String.IsNullOrEmpty(zoneName)) { return false; }
            if(CachedData.ContainsKey(zoneName.ToLower())) /*Update*/
            {
                CachedData[zoneName].Dispose();
                CachedData[zoneName] = zone;
                CachedData[zoneName].InitCollider();
            }
            else /*Add new*/
            {
                CachedData.Add(zoneName.ToLower(), zone);
                CachedData[zoneName].InitCollider();
            }
            return true;
        }
        internal bool DeleteZone(string zoneName)
        {
            Script.Instance.ScriptCheck();
            if (!CachedData.ContainsKey(zoneName.ToLower())) { return false; }
            CachedData[zoneName.ToLower()].Dispose();
            return CachedData.Remove(zoneName.ToLower());
        }

        internal void InitColliders()
        {
            Script.Instance.ScriptCheck();
            foreach (var zone in CachedData.Values)
            {
                zone?.InitCollider();
            }
        }
        internal void KillColliders()
        {
            var ZoneColliderBehaviours = Resources.FindObjectsOfTypeAll<ZoneColliderBehaviour>();
            foreach (var zoneCollider in ZoneColliderBehaviours)
            {
                zoneCollider?.Dispose();
            }
        }
    }
}