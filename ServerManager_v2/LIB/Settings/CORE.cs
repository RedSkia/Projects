using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIB.Settings
{
    public class CORE
    {
        private static readonly string ProgramData = $"{Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)}";
        private static readonly string BrandingFolder = "InfinityNet.dk";
        private static readonly string RootFolder = "IRSM";

        public class FileRef
        {
            public static readonly string Images = $"{ProgramData}/{BrandingFolder}/{RootFolder}/Images.json";
            public static readonly string Color = $"{ProgramData}/{BrandingFolder}/{RootFolder}/Colors.json";
            public static readonly string Server = $"{ProgramData}/{BrandingFolder}/{RootFolder}/Servers.json";
        }


        public class JsonManager
        {
            public static void SaveFile(string path, object jsonObj)
            {
                CreatFile(path); /*Creates If Not Exist*/

                File.WriteAllText(path, JsonConvert.SerializeObject(jsonObj, Formatting.Indented));
            }

            public static object LoadFile(string path)
            {
                if(File.Exists(path))
                {
                    return JsonConvert.DeserializeObject(File.ReadAllText(path));
                }
                return null;
            }
        }

        public static bool CreateDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path)); //Create Directory IF Not Exists

            }
            return true; //Success
        }
        public static bool CreatFile(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path)); //Create Directory IF Not Exists
            }
            if (!File.Exists(path))
            {
                File.Create($"{Path.GetDirectoryName(path)}/{Path.GetFileName(path)}").Close(); //Create File
            }
            return true; //Success
        }
    }
}