using PdfSharpCore;
using PdfSharpCore.Drawing;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Xml.Linq;

namespace MyAlbum.Models
{
    internal class PageBorder : Frame
    {

        internal new void ParseXml(XElement xBorder)
        {
            base.ParseXml(xBorder);
        }
        internal void Calculate(XGraphics gfx, Canvas parentCanvas)
        {
            // Adjust the border X, Y, W, H with the exterior line width
            X = parentCanvas.X + MarginLeft + LineWidth1 / 2;
            Y = parentCanvas.Y + MarginTop + LineWidth1 / 2;
            W = parentCanvas.W - (MarginLeft + MarginRight + LineWidth1);
            H = parentCanvas.H - (MarginTop + MarginBottom + LineWidth1);

            CalculateBorderWidths();


            this.Canvas.X = parentCanvas.X + MarginLeft + WidthLeft + PaddingLeft;
            this.Canvas.Y = parentCanvas.Y + MarginTop + WidthTop + PaddingTop;
            this.Canvas.W = parentCanvas.W - MarginLeft - WidthLeft - PaddingLeft - MarginRight - WidthRight - PaddingRight;
            this.Canvas.H = parentCanvas.H - MarginTop - WidthTop - PaddingTop - MarginBottom - WidthBottom - PaddingBottom;
            //Helper.DrawCorner(gfx, Canvas.X, Canvas.Y, XColors.Green);
            //Console.WriteLine($"Calculated {this.GetType().Name}: X=({X.Millimeter:F1}, Y={Y.Millimeter:F1}, W={W.Millimeter:F1}, H={H.Millimeter:F1}.");
        }
        internal void Calculate(XGraphics gfx, Canvas parentCanvas, PageOrientation orientation)
        {
            // If Landscape, shift attributes 90 degrees counterclockwise
            if (orientation == PageOrientation.Landscape)
            {
                // border types
                FrameType t = TypeTop;
                TypeTop = TypeLeft;
                TypeLeft = TypeBottom;
                TypeBottom = TypeRight;
                TypeRight = t;

                // margins
                XUnit m = MarginTop;
                MarginTop = MarginLeft;
                MarginLeft = MarginBottom;
                MarginBottom = MarginRight;
                MarginRight = m;

                // paddings
                XUnit p = PaddingTop;
                PaddingTop = PaddingLeft;
                PaddingLeft = PaddingBottom;
                PaddingBottom = PaddingRight;
                PaddingRight = p;
            }

            Calculate (gfx, parentCanvas);
            //Helper.DrawCorner(gfx, Canvas.X, Canvas.Y, XColors.Green);
        }
        internal override void Draw(XGraphics gfx)
        {
            //BgColor = XColors.Bisque; 
            //Console.WriteLine($"Drawing {this.GetType().Name}: X={X.Millimeter:F1}, Y={Y.Millimeter:F1}, W={W.Millimeter:F1}, H={H.Millimeter:F1}, BgColor={BgColor.ToString()}");
            //Console.WriteLine($"  BgColor ARGB: A={BgColor.A}, R={BgColor.R}, G={BgColor.G}, B={BgColor.B}, IsEmpty={BgColor.IsEmpty}");
            base.Draw(gfx);
        }


        internal void CalculateBorderWidths()
        {
            WidthTop = CalculateLineWidth(TypeTop);
            WidthRight = CalculateLineWidth(TypeRight);
            WidthBottom = CalculateLineWidth(TypeBottom);
            WidthLeft = CalculateLineWidth(TypeLeft);
        }
        private XUnit CalculateLineWidth(FrameType type) => type switch
        {
            FrameType.None => XUnit.Zero,
            FrameType.Single => LineWidth1,
            FrameType.Double => LineWidth1 + Offset + LineWidth2,
            _ => XUnit.Zero
        };

    }
}
