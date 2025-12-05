using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UI.Helpers
{
    public class IconHandler
    {
        public static async Task UpdateCustomImages(System.Drawing.Color color)
        {
            MessageBox.Show("Update Icons Custom");
            //If Color Changed Update
            if (LIB.Helpers.BitmapConverter.ImageDB.Get().Where(x => x.Key.EndsWith("C")).Count() <= 0) 
            {
                MessageBox.Show("Update Icons 1111");
                await Update(color);
            }
            else
            {
                var bitmap = LIB.Helpers.BitmapConverter.ImageDB.Get().Where(x => x.Key.EndsWith("C")).FirstOrDefault().Value;
                System.Drawing.Color pixel = new System.Drawing.Color();
                for (int y = 0; y < bitmap.Height; y++)
                {
                    for (int x = 0; x < bitmap.Width; x++)
                    { 
                        pixel = bitmap.GetPixel(x, y);
                        if (pixel.R > 0 && pixel.G > 0 && pixel.B > 0) { break; }
                    }
                    if (pixel.R > 0 && pixel.G > 0 && pixel.B > 0) { break; }
                }
        
                const int Buffer = 10;
                if (!Color.IsBetween(color.R, pixel.R - Buffer, pixel.R + Buffer) &&
                    !Color.IsBetween(color.G, pixel.G - Buffer, pixel.G + Buffer) &&
                    !Color.IsBetween(color.B, pixel.B - Buffer, pixel.B + Buffer))
                {
                    MessageBox.Show("Update Icons 2222");
                    await Update(color);
                }

            }
        }

        private static async Task Update(System.Drawing.Color color)
        {
            //Removes Old Custom Icons C from ImageDictionary
            foreach (var i in LIB.Helpers.BitmapConverter.ImageDB.Get().Where(x => x.Key.EndsWith("C")))
            {
                LIB.Helpers.BitmapConverter.ImageDB.RemoveKey(i.Key);
            }

            //Process Custom Images
            foreach (var i in LIB.Helpers.BitmapConverter.ImageDB.Get().Where(x => x.Key.EndsWith("B")))
            {
                await LIB.Helpers.BitmapConverter.ProcessImage(new LIB.Helpers.BitmapConverter.ImageData
                {
                    bitmap = i.Value,
                    color = color,
                    name = i.Key.Replace("B", "C")
                });
            }
    
            LIB.Helpers.BitmapConverter.SaveImages();
        }
    }
}
