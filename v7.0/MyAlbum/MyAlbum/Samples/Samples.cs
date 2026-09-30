using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyAlbum.Samples
{
    abstract class Samples
    {
        XGraphicsState? state;
        internal double borderWidth = 0;
        XColor shadowColor = XColors.Aqua;
        internal XColor backColor = XColors.WhiteSmoke;
        XColor backColor2 = XColors.LightGray;
        internal XPen borderPen = XPens.DarkOrange;
        
        public abstract void DrawPage(PdfPage page);
        
        public void DrawTitle(PdfPage page, XGraphics gfx, string title)
        {
            XRect rect = new XRect(new XPoint(), gfx.PageSize);
            rect.Inflate(-10, -15);

            // draw title
            XFont font = new XFont("Verdana", 14, XFontStyle.Bold);
            gfx.DrawString(title, font, XBrushes.MidnightBlue, rect, XStringFormats.TopCenter);
        }

        public void DrawFooter(PdfPage page, XGraphics gfx, int pageNumber)
        {
            XRect rect = new XRect(new XPoint(), gfx.PageSize);
            rect.Inflate(-10, -15);

            // draw footer
            rect.Offset(0, 5);
            XFont font = new XFont("Verdana", 8, XFontStyle.Italic);
            XStringFormat format = new XStringFormat();
            format.Alignment = XStringAlignment.Center;
            format.LineAlignment = XLineAlignment.Far;
            //gfx.DrawString("Created with " + PdfSharpCore.ProductVersionInfo.Producer, font, XBrushes.DarkOrchid, rect, format);

            // draw page number
            font = new XFont("Verdana", 8);
            format.Alignment = XStringAlignment.Center;
            gfx.DrawString(pageNumber.ToString(), font, XBrushes.DarkOrchid, rect, format);
        }
        
        public void BeginBox(XGraphics gfx, int number, string title)
        {
            const int dEllipse = 15;
            XRect rect = new XRect(0, 20, 300, 200);
            if (number % 2 == 0)
                rect.X = 300 - 5;
            rect.Y = 40 + ((number - 1) / 2) * (200 - 5);
            rect.Inflate(-10, -10);
            XRect rect2 = rect;
            rect2.Offset(this.borderWidth, this.borderWidth);
            gfx.DrawRoundedRectangle(
                new XSolidBrush(this.shadowColor),
                rect2,
                new XSize(dEllipse + 8, dEllipse + 8)
            );
            XLinearGradientBrush brush = new XLinearGradientBrush(
                rect, this.backColor, this.backColor2, XLinearGradientMode.Vertical);
            gfx.DrawRoundedRectangle(this.borderPen, brush, rect, new XSize(dEllipse, dEllipse));
            rect.Inflate(-5, -5);

            XFont font = new XFont("Verdana", 12, XFontStyle.Regular);
            gfx.DrawString(title, font, XBrushes.Navy, rect, XStringFormats.TopCenter);

            rect.Inflate(-10, -5);
            rect.Y += 20;
            rect.Height -= 20;

            this.state = gfx.Save();
            gfx.TranslateTransform(rect.X, rect.Y);
        }

        public void EndBox(XGraphics gfx)
        {
            gfx.Restore(this.state);
        }

    }
}
