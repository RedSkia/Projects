using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IRSM.ClassFunctions
{
    class CustomButton : Button
    {

        public CustomButton()
        {
            BackColor = Color.FromArgb(10, 20, 30);
            ForeColor = Color.White;
            ImageAlign = ContentAlignment.MiddleRight;
            FlatStyle = FlatStyle.Flat;
            BackColor = Color.Transparent;
            FlatAppearance.BorderSize = 0;
            FlatAppearance.MouseOverBackColor = Color.FromArgb(50, 60, 70);
            FlatAppearance.MouseDownBackColor = Color.FromArgb(60, 70, 80);
            BackgroundImageLayout = ImageLayout.Zoom;
            Margin = new Padding(0, 0, 0, 0);


        }

    }
}
