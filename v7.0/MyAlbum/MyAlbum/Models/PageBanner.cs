using MyAlbum.Utilities;
using PdfSharpCore;
using PdfSharpCore.Drawing;
using SixLabors.ImageSharp.Processing;
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
            _absolute = false;
            Width = "100%";
        }
        internal override void Calculate(XGraphics gfx, Canvas parentCanvas)
        {
            base.Calculate(gfx, parentCanvas);

            if (Rotate)
            {
                X = parentCanvas.Y + MarginLeft;
                Y = parentCanvas.X + MarginTop;
                W = parentCanvas.H - (MarginLeft + MarginRight);

                // W now spans the page height, so  if H is not specified, recompute from the aspect ratio.
                if (string.IsNullOrEmpty(Height))
                {
                    double aspectRatio = (XImg != null && XImg.PixelHeight != 0)
                        ? (double)XImg.PixelWidth / XImg.PixelHeight
                        : 1.0;
                    H = XUnit.FromPoint(W.Point / aspectRatio);
                }
                this.Canvas = new Canvas
                {
                    X = X + PaddingLeft,
                    Y = Y + PaddingTop,
                    W = W - (PaddingLeft + PaddingRight),
                    H = H - (PaddingTop + PaddingBottom)
                };
            }
        }
        #endregion
    }
}
