using MyAlbum.Utilities;
using PdfSharp;
using PdfSharp.Drawing;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace MyAlbum.Models
{
    internal class PageBanner : Image
    {
        #region Override methods
        internal override void ParseXml(XElement xElem)
        {
            base.ParseXml(xElem);
            Absolute = false;
            Width = "100%";
        }
        internal override void Calculate(XGraphics gfx, Canvas parentCanvas)
        {
            base.Calculate(gfx, parentCanvas);

            if (Rotate)
            {
                W = parentCanvas.H;
                Canvas.W = W - (MarginLeft + MarginRight);

                // W changed - recompute H (if not explicitly set)
                if (string.IsNullOrEmpty(Height))
                {
                    Canvas.H = XUnit.FromPoint(Canvas.W.Point / AspectRatio);
                    H = Canvas.H + (MarginTop + MarginBottom);
                }
            }
        }
        #endregion
    }
}
