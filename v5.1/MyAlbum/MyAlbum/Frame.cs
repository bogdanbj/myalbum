using PdfSharp.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyAlbum
{
    internal class Frame : BaseElement
    {
        #region constructors
        private Frame() { }
        public Frame(XGraphics gfx)
        {
            this.Gfx = gfx;
        }
        #endregion

        #region public methods
        public override void Draw()
        {
            XPen pen = new XPen(XColors.CadetBlue);
            Gfx.DrawRectangle(pen, X, Y, W, H);
        }

        public override void Parse()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
