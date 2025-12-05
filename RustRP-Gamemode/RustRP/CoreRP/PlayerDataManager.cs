using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace CoreRP
{
    internal static class PlayerData
    {
        internal readonly static PlayerDataManager Instance = new PlayerDataManager();
    }
    internal sealed class PlayerDataManager : Contracts.BaseData<Dictionary<ulong, Dictionary<string, object>>>
    {
        internal override string FileName => "PlayerData";
        internal override Dictionary<ulong, Dictionary<string, object>> LoadData(string args = "")
        {
            ulong PlayerId = 0; ulong.TryParse(args, out PlayerId);
            CachedData[PlayerId] = base.LoadData(args)[PlayerId];
            return null;
        }
        internal override void SaveData(string args = "", object data = null)
        {
            ulong PlayerId = 0; ulong.TryParse(args, out PlayerId);
            base.SaveData(args, GetKey(PlayerId));
        }

        private void CreateKey(ulong PlayerId)
        {
            if (!CachedData.ContainsKey(PlayerId)) { CachedData.Add(PlayerId, new Dictionary<string, object>()); }
        }
        private string GetKey<T>() where T : class => typeof(T)?.Name?.ToLower() ?? "";
        private Dictionary<string, object> GetKey(ulong PlayerId) => CachedData.ContainsKey(PlayerId) ? CachedData[PlayerId] : null;
        private object GetKey<T>(ulong PlayerId) where T : class => CachedData.ContainsKey(PlayerId) && CachedData[PlayerId].ContainsKey(GetKey<T>()) ? CachedData[PlayerId][GetKey<T>()] : null;

        internal void CreateData<T>(ulong PlayerId, T Data) where T : class
        {
            CreateKey(PlayerId);
            string key = GetKey<T>();
            if (!CachedData[PlayerId].ContainsKey(key)) { CachedData[PlayerId].Add(key, Data); return; }
            CachedData[PlayerId][key] = Data;
        }
        internal void RemoveData<T>(ulong PlayerId) where T : class
        {
            CreateKey(PlayerId);
            string key = GetKey<T>();
            if (CachedData[PlayerId].ContainsKey(key)) { CachedData[PlayerId].Remove(key); }
        }
        internal T GetData<T>(ulong PlayerId) where T : class
        {
            CreateKey(PlayerId);
            var data = GetKey<T>(PlayerId) as JObject;
            return data?.ToObject<T>() ?? Activator.CreateInstance<T>();
        }
    }
}