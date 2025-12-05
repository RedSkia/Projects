using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LIB.Helpers
{
    /// <summary>
    /// <list type="">Loads Bitmap Images into <see cref="ImageDictionary"/></list>
    /// <list type="">Using <see cref="ProcessImages(BitmapConverter.ImageData[])"/></list>
    /// <list type="">Using <see cref="GetImage(string)"/></list>
    /// </summary>
    public class BitmapConverter
    {
        private static readonly Dictionary<string, Bitmap> ImageDictionary = new Dictionary<string, Bitmap>();
        public class ImageDB
        {
            public static KeyValuePair<string, Bitmap>[] Get() => ImageDictionary.ToArray();
       
            public static void RemoveKey(string key)
            {
                if(ImageDictionary.ContainsKey(key)) { ImageDictionary.Remove(key); }
            }
        }


        public static async void LoadImages()
        {
            try
            {
                var tempDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(JsonConvert.SerializeObject(LIB.Settings.CORE.JsonManager.LoadFile(LIB.Settings.CORE.FileRef.Images)));
                if (tempDictionary != null)
                {
                    LIB.Data.Pool.Processing = true;
                    foreach (var key in tempDictionary.Keys)
                    {
                        if (!ImageDictionary.ContainsKey(key))
                        {
                            await Task.Run(() =>
                            {
                                BitmapConverter.ProcessRawImage(key, new Bitmap(Base64Converter.FromBase64(tempDictionary[key])));
                            });
                        }
                    }
                    LIB.Data.Pool.Processing = false;
                }
            }
            catch { SaveImages(); /*Failed To Load Images Override Save*/ }
        }
        
        public static async void SaveImages()
        {
            var tempDictionary = new Dictionary<string, string>();

            LIB.Data.Pool.Processing = true;

            foreach (var i in ImageDB.Get().ToArray())
            {
                tempDictionary.Add(i.Key, await Base64Converter.FromBitmap(ImageDictionary[i.Key]));
            }
            LIB.Settings.CORE.JsonManager.SaveFile(LIB.Settings.CORE.FileRef.Images, tempDictionary);

            LIB.Data.Pool.Processing = false;
        }


        public class Base64Converter
        {
            /// <summary>
            /// Converts <paramref name="bitmap"/> To Base64
            /// </summary>
            /// <returns>Base64 String</returns>
            public static async Task<string> FromBitmap(Bitmap bitmap)
            {
                return Convert.ToBase64String((byte[])new ImageConverter().ConvertTo(bitmap, typeof(byte[])));
            }

            /// <summary>
            /// Converts <paramref name="base64"/> To Bitmap
            /// </summary>
            /// <returns><see cref="Bitmap"/></returns>
            public static Bitmap FromBase64(string base64)
            {
                byte[] bytes = Convert.FromBase64String(base64);
                using (MemoryStream ms = new MemoryStream(bytes))
                {
                    return new Bitmap(Image.FromStream(ms));
                }
            }
        }


        public static Color InvertColor(Color color) => Color.FromArgb(255 - color.R, 255 - color.G, 255 - color.B);


        public class ImageData
        {
            public string name { get; set; }
            public Bitmap bitmap { get; set; }
            public Color color { get; set; }
        }



        /// <summary>
        /// <list type="table">Remove and updates a <see cref="ImageData.bitmap"/> from <see cref="ImageDictionary"/></list>
        /// <list type="table">Avoid using if not needed</list>
        /// </summary>
        /// <param name="data"></param>
        public static async void UpdateImage(ImageData data)
        {
            ImageDictionary.Remove(data.name);
            if(!ImageDictionary.ContainsKey(data.name)) 
            {
                await LIB.Helpers.BitmapConverter.ProcessImage(new LIB.Helpers.BitmapConverter.ImageData
                {
                    bitmap = new Bitmap(data.bitmap),
                    color = data.color,
                    name = data.name,
                });
            }
        }

        /// <summary>
        /// Attempts fetch from <see cref="ImageDictionary"/>
        /// </summary>
        /// <param name="key"></param>
        /// <returns>
        /// <list type="table"><paramref name="key"/> From <see cref="ImageDictionary"/> If Success</list>
        /// <list type="table">If Fail retruns <see cref="Properties.Resources.Placeholder"/></list>
        /// </returns>
        public static Bitmap GetImage(string key)
        {
            if (ImageDictionary.ContainsKey(key)) { return ImageDictionary[key]; }
            else if (ImageDictionary.ContainsKey("placeholder")) { return ImageDictionary["placeholder"]; }
            else { return Properties.Resources.Placeholder; }
        }


        /// <summary>
        /// Processes a single <paramref name="bitmap"/> Image without re-coloring
        /// </summary>
        /// <param name="name"></param>
        /// <param name="bitmap"></param>
        public static void ProcessRawImage(string name, Bitmap bitmap)
        {
            if (!ImageDictionary.ContainsKey(name)) 
            {
                try { ImageDictionary.Add(name, new Bitmap(bitmap)); }
                catch { }
            }
        }

        /// <summary>
        /// Processes single <paramref name="image"/> Using <see cref="ProcessImages(ImageData[])"/>
        /// </summary>
        public static async Task ProcessImage(ImageData image)
        {
            await Task.Delay(1);
            if (!ImageDictionary.ContainsKey(image.name))
            {
                await ProcessImages(new ImageData[] { image });
            }
        }
 

        /// <summary>
        /// Processes <paramref name="images"/> To new <see cref="ImageData.color"/> And caches image to <see cref="ImageDictionary"/>
        /// </summary>
        public static async Task ProcessImages(ImageData[] images)
        {
            LIB.Data.Pool.Processing = true;

            //Process New Images
            foreach (var i in images)
            {
                await Task.Delay(1);
                var bitmap = new Bitmap(i?.bitmap);
                if (!ImageDictionary.ContainsKey(i.name))
                {
                    await Task.Run(() =>
                    {
                        for (int y = 0; y < bitmap.Height; y++)
                        {
                            for (int x = 0; x < bitmap.Width; x++)
                            {
                                bitmap.SetPixel(x, y, Color.FromArgb(bitmap.GetPixel(x, y).A, i.color.R, i.color.G, i.color.B));
                            }
                        }
                    });
                    try { ImageDictionary.Add(i.name, bitmap); }
                    catch { }
                }
            }
            LIB.Data.Pool.Processing = false;
        }
    }
}