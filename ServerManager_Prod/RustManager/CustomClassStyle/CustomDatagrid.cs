using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RustManager.CustomClass
{
    class CustomDatagrid : DataGridView
    {

        public CustomDatagrid()
        {
            //Data Grid Settings
            ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.False;
      
            //Background Color
            BackgroundColor = Color.FromArgb(20, 30, 40);

            //Left box
            RowHeadersVisible = false;

            BorderStyle = BorderStyle.None;
            EnableHeadersVisualStyles = false;

            //Remove Cell Grid
            CellBorderStyle = DataGridViewCellBorderStyle.RaisedVertical;

            //Remove Header Grid
            ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;

            //Header Colors
            ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(20, 30, 40);
            ColumnHeadersDefaultCellStyle.ForeColor = Color.White;

            //Cell Colors
            DefaultCellStyle.ForeColor = Color.White;

            //Grid Color
            GridColor = Color.White;

            //Alternative Colors 
            RowsDefaultCellStyle.BackColor = Color.FromArgb(30, 40, 50);
            AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(50, 60, 70);

            //Selection Colors
            DefaultCellStyle.SelectionBackColor = Color.FromArgb(10, 20, 30);
            DefaultCellStyle.SelectionForeColor = Color.FromArgb(170, 255, 255);

   
            //Rows Height
            RowTemplate.Height = 30;


            //Padding
            ColumnHeadersDefaultCellStyle.Padding = new Padding(5, 5, 5, 5);
        }
    }
}
