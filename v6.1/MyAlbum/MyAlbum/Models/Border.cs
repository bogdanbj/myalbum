using PdfSharpCore.Drawing;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace MyAlbum.Models
{
    internal class Border : BaseElement
    {
        public Border()
        {
            Color = XColors.Black;
            BgColor = XColors.Bisque;
        }

        internal new void ParseXml(XElement xBorder)
        {
            base.ParseXml(xBorder);

        }
        internal /*override*/ void Calculate(XGraphics gfx, Canvas parentCanvas)
        {
            // for now... until I have the real calculation
            this.Canvas = parentCanvas;
        }
        internal override void Draw(XGraphics gfx)
        {
            //BgColor = XColors.Bisque; 
            base.Draw(gfx);
        }
    }
}
