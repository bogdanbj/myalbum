using PdfSharpCore;
using PdfSharpCore.Drawing;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyAlbum.Models
{
    internal class PageBanner : Image
    {
        internal void Calculate(XGraphics gfx, Canvas parentCanvas, PageOrientation orientation)
        {
            base.Calculate(gfx, parentCanvas);

            // If Landscape, shift attributes 90 degrees counterclockwise
            if (orientation == PageOrientation.Landscape)
            {
                X = parentCanvas.Y + MarginLeft;
                Y = parentCanvas.X + MarginTop;
                W = parentCanvas.H - (MarginLeft + MarginRight);
                this.Canvas = new Canvas
                {
                    X = X + PaddingLeft,
                    Y = Y + PaddingTop,
                    W = W - (PaddingLeft + PaddingRight),
                    H = H - (PaddingTop + PaddingBottom)
                };
                //// border types
                //FrameType t = TypeTop;
                //TypeTop = TypeLeft;
                //TypeLeft = TypeBottom;
                //TypeBottom = TypeRight;
                //TypeRight = t;

                //// margins
                //XUnit m = MarginTop;
                //MarginTop = MarginLeft;
                //MarginLeft = MarginBottom;
                //MarginBottom = MarginRight;
                //MarginRight = m;

                //// paddings
                //XUnit p = PaddingTop;
                //PaddingTop = PaddingLeft;
                //PaddingLeft = PaddingBottom;
                //PaddingBottom = PaddingRight;
                //PaddingRight = p;
            }

            Calculate(gfx, parentCanvas);
        }
    }
}
