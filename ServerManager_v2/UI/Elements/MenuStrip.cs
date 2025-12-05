using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UI.Elements
{
    public class MenuStrip : System.Windows.Forms.MenuStrip
    {
        public MenuStrip()
        {
            this.Renderer = new MyMenuRenderer();
            BackColor = UI.Helpers.Color.Background;
        }
    }

    internal class MyMenuRenderer : ToolStripRenderer
    {
        protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
        {
            base.OnRenderMenuItemBackground(e);

            if (e.Item.Enabled)
            {
                if (e.Item.IsOnDropDown == false && e.Item.Selected)
                {
                    var rect = new Rectangle(0, 0, e.Item.Width - 1, e.Item.Height - 1);
                    var rect2 = new Rectangle(0, 0, e.Item.Width - 1, e.Item.Height - 1);
                    //Hover
                    e.Graphics.FillRectangle(new SolidBrush(UI.Helpers.Color.Hover), rect);
                    //Outline Hover
                    e.Graphics.DrawRectangle(new Pen(new SolidBrush(UI.Helpers.Color.MainElement)), rect2);
                    e.Item.ForeColor = UI.Helpers.Color.Text;
                }
                else
                {
                    e.Item.ForeColor = UI.Helpers.Color.Text;
                }

                if (e.Item.IsOnDropDown && e.Item.Selected)
                {
                    var rect = new Rectangle(0, 0, e.Item.Width - 1, e.Item.Height - 1);
                    //Item Hover
                    e.Graphics.FillRectangle(new SolidBrush(UI.Helpers.Color.Hover), rect);
                    //Item Hover Outline
                    e.Graphics.DrawRectangle(new Pen(new SolidBrush(UI.Helpers.Color.MainElement)), rect);
                    e.Item.ForeColor = UI.Helpers.Color.Text;
                }
                if ((e.Item as ToolStripMenuItem).DropDown.Visible && e.Item.IsOnDropDown == false)
                {
                    var rect = new Rectangle(0, 0, e.Item.Width - 1, e.Item.Height - 1);
                    var rect2 = new Rectangle(0, 0, e.Item.Width - 1, e.Item.Height - 1);
                    //Nav Item Selected
                    e.Graphics.FillRectangle(new SolidBrush(UI.Helpers.Color.Click), rect);
                    //Nav Item Selected Outline
                    e.Graphics.DrawRectangle(new Pen(new SolidBrush(UI.Helpers.Color.MainElement)), rect2);
                    e.Item.ForeColor = UI.Helpers.Color.Text;
                }
            }
        }

        protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
        {
            base.OnRenderSeparator(e);
            var DarkLine = new SolidBrush(UI.Helpers.Color.Line);
            var rect = new Rectangle(30, 3, e.Item.Width - 30, 1);
            e.Graphics.FillRectangle(DarkLine, rect);
        }


        protected override void OnRenderItemCheck(ToolStripItemImageRenderEventArgs e)
        {
            base.OnRenderItemCheck(e);

            if (e.Item.Selected)
            {
                var rect = new Rectangle(4, 2, 18, 18);
                var rect2 = new Rectangle(5, 3, 16, 16);
                //Hover Outline
                SolidBrush b = new SolidBrush(UI.Helpers.Color.Selected);
                //Hover Inner
                SolidBrush b2 = new SolidBrush(UI.Helpers.Color.Selected);

                e.Graphics.FillRectangle(b, rect);
                e.Graphics.FillRectangle(b2, rect2);
                e.Graphics.DrawImage(e.Image, new Point(5, 3));
            }
            else
            {
                var rect = new Rectangle(4, 2, 18, 18);
                var rect2 = new Rectangle(5, 3, 16, 16);
                //Hover
                SolidBrush b = new SolidBrush(UI.Helpers.Color.Selected);
                //Item Color
                SolidBrush b2 = new SolidBrush(UI.Helpers.Color.Selected);

                e.Graphics.FillRectangle(b, rect);
                e.Graphics.FillRectangle(b2, rect2);
                e.Graphics.DrawImage(e.Image, new Point(5, 3));
            }
        }

        protected override void OnRenderImageMargin(ToolStripRenderEventArgs e)
        {
            base.OnRenderImageMargin(e);

            var rect = new Rectangle(0, 0, e.ToolStrip.Width, e.ToolStrip.Height);
            //Sub Item
            e.Graphics.FillRectangle(new SolidBrush(UI.Helpers.Color.MainElement), rect);

            //Image
            var DarkLine = new SolidBrush(UI.Helpers.Color.MainElement);
            var rect3 = new Rectangle(0, 0, 26, e.AffectedBounds.Height);
            e.Graphics.FillRectangle(DarkLine, rect3);

            //Image Line Splitter
            e.Graphics.DrawLine(new Pen(new SolidBrush(UI.Helpers.Color.MainElement)), 28, 0, 28, e.AffectedBounds.Height);

            var rect2 = new Rectangle(0, 0, e.ToolStrip.Width - 1, e.ToolStrip.Height - 1);
            //Sub Item Outline
            e.Graphics.DrawRectangle(new Pen(new SolidBrush(UI.Helpers.Color.Dark)), rect2);
        }
    }
}
