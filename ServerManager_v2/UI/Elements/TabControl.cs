

using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace UI.Elements
{
    public class TabControl : System.Windows.Forms.TabControl
    {
        #region Make Buttons FILL
        private const int TCM_ADJUSTRECT = 0x1328;

        protected override void WndProc(ref Message m)
        {
            //Hide the tab headers at run-time
            if (m.Msg == TCM_ADJUSTRECT)
            {

                RECT rect = (RECT)(m.GetLParam(typeof(RECT)));
                rect.Left = this.Left - this.Margin.Left;
                rect.Right = this.Right + this.Margin.Right;

                rect.Top = this.Top - this.Margin.Top;
                rect.Bottom = this.Bottom + this.Margin.Bottom;
                Marshal.StructureToPtr(rect, m.LParam, true);
                //m.Result = (IntPtr)1;
                //return;
            }
            //else
            // call the base class implementation
            base.WndProc(ref m);
        }

        private struct RECT
        {
            public int Left, Top, Right, Bottom;
        }
        #endregion Make Buttons FILL

        public TabControl()
        {
            DrawMode = TabDrawMode.OwnerDrawFixed;
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.UserPaint | ControlStyles.SupportsTransparentBackColor, true);
            DoubleBuffered = true;
            SizeMode = TabSizeMode.Fixed;
            ItemSize = new System.Drawing.Size(120, 35);
            this.DrawMode = TabDrawMode.OwnerDrawFixed;
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            Bitmap B = new Bitmap(Width, Height);
            Graphics G = Graphics.FromImage(B);

            //Background Color
            G.Clear(UI.Helpers.Color.Background);
            Pen LinePen = new Pen(UI.Helpers.Color.Selected, 1);
            for (int i = 0; i <= TabCount - 1; i++)
            {
                Rectangle TabRectangle = GetTabRect(i);

                if (i == SelectedIndex)
                {
                    //Tab is selected
                    G.FillRectangle(new SolidBrush(UI.Helpers.Color.SubElement), TabRectangle);

                    G.DrawLine(LinePen, TabRectangle.Location.X, TabRectangle.Height, TabRectangle.Location.X + TabRectangle.Width, TabRectangle.Height); 
                }
                else
                {
                    //Tab is not selected
                    G.FillRectangle(new SolidBrush(UI.Helpers.Color.MainElement), TabRectangle);
                }

                StringFormat sf = new StringFormat();

                sf.LineAlignment = StringAlignment.Center;
                sf.Alignment = StringAlignment.Center;


                G.DrawString(TabPages[i].Text, new Font(this.Font, FontStyle.Regular), new SolidBrush(UI.Helpers.Color.Text), TabRectangle, sf);

                TabPages[i].BackColor = UI.Helpers.Color.Dark;
            }
            e.Graphics.DrawImage(B, 0, 0);
            G.Dispose();
            B.Dispose();
            base.OnPaint(e);
        }
    }
}