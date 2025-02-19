using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PdfSharp;
using PdfSharp.Drawing;


namespace HelloWorld
{
    internal class Frame : BaseElement
    {
        #region constructors
        public Frame() { }
        public Frame(XGraphics gfx)
        {
            this.gfx = gfx;
        }
        #endregion

        #region public methods
        public void Draw()
        {
            XPen pen = new XPen(XColors.CadetBlue);
            gfx.DrawRectangle(pen, x, y, w, h);
        }
        #endregion
    }
}
