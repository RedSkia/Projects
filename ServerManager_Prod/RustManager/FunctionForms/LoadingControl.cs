using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RustManager.FunctionForms
{
    public partial class LoadingControl : UserControl
    {
        public LoadingControl(string TextToDisplay)
        {
            InitializeComponent();
            label1.Text = TextToDisplay;
        }

 
    }
}
