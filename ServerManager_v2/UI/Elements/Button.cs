using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UI.Elements
{
    public class Button : System.Windows.Forms.Button
    {
        public Button()
        {
            BackColor = UI.Helpers.Color.MainElement;
            ForeColor = UI.Helpers.Color.Text;

            ImageAlign = ContentAlignment.MiddleRight;
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            FlatAppearance.MouseOverBackColor = UI.Helpers.Color.Hover;
            FlatAppearance.MouseDownBackColor = UI.Helpers.Color.Click;
            BackgroundImageLayout = ImageLayout.Zoom;
            Margin = new Padding(0, 0, 0, 0);
            Size = new Size(215, 50);
            Cursor = Cursors.Hand;
            TextAlign = ContentAlignment.MiddleLeft;
        }
    }
}
