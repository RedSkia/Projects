using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UI.Elements
{
    public class DataGridView : System.Windows.Forms.DataGridView
    {
        public DataGridView()
        {
            BackgroundColor = UI.Helpers.Color.Hover;

            BorderStyle = System.Windows.Forms.BorderStyle.None;
            EnableHeadersVisualStyles = false;

            //Left box
            RowHeadersVisible = false;

            //Remove Cell Grid
            CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.RaisedVertical;

            //Remove Header Grid
            ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;

            //Header Colors
            ColumnHeadersDefaultCellStyle.BackColor = UI.Helpers.Color.MainElement;
            ColumnHeadersDefaultCellStyle.ForeColor = UI.Helpers.Color.Text;

            //Cell font Colors
            DefaultCellStyle.ForeColor = UI.Helpers.Color.Text;

            //Grid Color
            GridColor = UI.Helpers.Color.Hover;

            //Makes Cell Grid Color Same As rid Color
            CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleVertical;

            //Alternative Colors 
            RowsDefaultCellStyle.BackColor = UI.Helpers.Color.Dark;
            AlternatingRowsDefaultCellStyle.BackColor = UI.Helpers.Color.Light;

            //Selection Colors
            DefaultCellStyle.SelectionBackColor = UI.Helpers.Color.SubElement;
            DefaultCellStyle.SelectionForeColor = UI.Helpers.Color.Text;

            //Rows Height
            RowTemplate.Height = 30;

            //Padding
            ColumnHeadersDefaultCellStyle.Padding = new System.Windows.Forms.Padding(5, 5, 5, 5);
        }
    }
}
