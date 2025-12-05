using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;

namespace CoreRP
{
    internal static class ImageManager
    {
        internal readonly static BaseImageManager Instance = new BaseImageManager();
    }

    internal sealed class BaseImageManager : Contracts.BaseData<Dictionary<string, uint>>
    {
        private NetworkableId CEID => CommunityEntity.ServerInstance.net.ID;
        internal override string FileName => "ImageData";
        internal override Dictionary<string, uint> LoadData(string args = "")
        {
            base.LoadData(args);
            ProcessImages();
            return null;
        }
        internal override void SaveData(string args = "", object data = null) => base.SaveData(args, data);

        internal string GetImage(string image)
        {
            uint id = 0;
            CachedData.TryGetValue(image, out id);
            return id.ToString();
        }
        private async Task<uint> DownloadImage(string url)
        {
            using (WebClient client = new WebClient())
            {
                byte[] imageData = await client.DownloadDataTaskAsync(url);
                if(imageData == null) { return 0; }
                return FileStorage.server.Store(imageData, FileStorage.Type.png, CEID);
            }
        }
        private async void ProcessImages()
        {
            var images = typeof(Settings.Images).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
            foreach (var image in images)
            {
                string name = image.Name;
                string url = image.GetValue(null) as String;
                if(CachedData.ContainsKey(name)) { continue; }
                uint id = await DownloadImage(url);
                CachedData.Add(name, id);
                FileLogger.LogMessage($"Process Image: {name} ({id}) \"{url}\"", diskLog: false);
            }
        }
    }
}