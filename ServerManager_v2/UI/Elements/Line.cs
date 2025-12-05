using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UI.Elements
{
    public class Line : Panel
    {
        public Line()
        {
            BackColor = UI.Helpers.Color.MainElement;
            Padding = new Padding(0);
            Margin = new Padding(0);
            Dock = DockStyle.Fill;
        }
    }
}
