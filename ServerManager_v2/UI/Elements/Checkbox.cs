using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UI.Elements
{
    public class Checkbox : System.Windows.Forms.CheckBox
    {
        public Color TextColor { get; set; } = Color.Gold;
        public float TextHeight { get; set; } = 0;
        public float TextSize { get; set; } = 20;

        public Color TextTransparnetColor { get; set; } = Color.Transparent;

        public Color CheckBoxColor { get; set; } = Color.Black;

        public Checkbox()
        {
            MinimumSize = new Size(this.Width + this.Height, this.Height);
            Cursor = Cursors.Hand;
        }
        
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            using (Font f = new Font(this.Font.FontFamily, 24, this.Font.Style, GraphicsUnit.Pixel))
            using (var b1 = new SolidBrush(CheckBoxColor))
            using (var b2 = new SolidBrush(TextColor))
            {
                var g = e.Graphics;
                Font = new Font(this.Font.FontFamily, 0.1f);
                Rectangle checkBoxRect = new Rectangle(0, 0, this.Height, this.Height);
                g.FillRectangle(b1, checkBoxRect);
                ForeColor = TextTransparnetColor;
                PointF textPoint = new PointF(this.Height, TextHeight);
                e.Graphics.DrawString(this.Text, f, b2, textPoint);
       

                if (this.Checked)
                {
                    switch (UI.Helpers.Color.imageColor)
                    {
                        case UI.Helpers.Color.ImageColor.black:
                            g.DrawImage(LIB.Helpers.BitmapConverter.GetImage("checkmarkB"), checkBoxRect);

                            break;
                        case UI.Helpers.Color.ImageColor.white:
                            g.DrawImage(LIB.Helpers.BitmapConverter.GetImage("checkmarkW"), checkBoxRect);
                            break;

                        case UI.Helpers.Color.ImageColor.custom:
                            g.DrawImage(LIB.Helpers.BitmapConverter.GetImage("checkmarkC"), checkBoxRect);
                            break;
                    }
                }
            }
            
        }
    }
}
