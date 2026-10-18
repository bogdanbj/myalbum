using PdfSharp;
using PdfSharp.Drawing;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Xml.Linq;

namespace MyAlbum.Models
{
    internal class PageBorder : Frame
    {
        #region Override methods
        internal override void ParseXml(XElement xElem)
        {
            base.ParseXml(xElem);
            Width = "100%";
            Height= "100%";
            Alignment = FrameAlignment.Outside;
        }
        internal override void Calculate(XGraphics gfx, Canvas parentCanvas)
        {
            base.Calculate(gfx, parentCanvas);

            if (Rotate)
            {
                W = parentCanvas.H;
                H = parentCanvas.W;
                Canvas.X = WidthBottom + MarginBottom;
                Canvas.Y = WidthLeft + MarginLeft;
                Canvas.W = H - (WidthTop + MarginTop + MarginBottom + WidthBottom);
                Canvas.H = W - (WidthLeft + MarginLeft + MarginRight + WidthRight);
            }
            else
            {
                // This is taken care of in Frame.
            }
        }
        internal override void Draw(XGraphics gfx)
        {
            base.Draw(gfx);
        }
        #endregion
    }
}
