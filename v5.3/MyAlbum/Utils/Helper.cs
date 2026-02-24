using MyAlbum.Models.Layout;
using PdfSharpCore.Drawing;

namespace MyAlbum.Utils
{
    internal static class Helper
    {
        internal static void Fill(XGraphics gfx, BaseElement elem)
        {
            Fill(gfx, elem, elem.BgColor);
        }
        internal static void Fill(XGraphics gfx, BaseElement elem, XColor bgColor)
        {
            gfx.DrawRectangle(new XSolidBrush(bgColor), elem.X, elem.Y, elem.W, elem.H);
        }
        internal static void FillCanvas(XGraphics gfx, BaseElement elem)
        {
            XColor color = elem switch
            {
                Page => XColors.Bisque,
                //Row => XColors.MistyRose,
                Row => XColors.PaleGoldenrod,
                Text => XColors.Aqua,
                Column => XColors.Brown,
                Image => XColors.Plum,
                Stamp => XColors.LightSkyBlue,
                _ => XColors.Transparent
            };
            /*
                public XColor PageCanvasColor => XColors.Bisque;
                public XColor EmptyRowColor => XColors.MistyRose;
                public XColor TextBgColor => XColors.Aqua;
                public XColor ColumnBgColor => XColors.Brown;
                public XColor ImageBgColor => XColors.Plum;
                public XColor StampBgColor => XColors.LightSkyBlue;
                public XColor StampBorderColor => XColors.DeepSkyBlue;
            */
            gfx.DrawRectangle(new XSolidBrush(color), elem.Canvas.X, elem.Canvas.Y, elem.Canvas.W, elem.Canvas.H);
        }
        internal static void Fill(XGraphics gfx, XUnit x, XUnit y, XUnit w, XUnit h, XColor bgColor)
        {
            gfx.DrawRectangle(new XSolidBrush(bgColor), x, y, w, h);
        }

        internal static void MarkCorners(XGraphics gfx, BaseElement elem, bool fill)
        {
            MarkCorners(gfx, elem.X, elem.Y, elem.W, elem.H, fill);
        }
        internal static void MarkCorners(XGraphics gfx, Rect rect, bool fill)
        {
            MarkCorners(gfx, rect.X, rect.Y, rect.W, rect.H, fill);
        }
        internal static void MarkCorners(XGraphics gfx, XUnit x, XUnit y, XUnit w, XUnit h, bool fill)
        {
            XUnit radius = XUnit.FromMillimeter(1);
            if (fill)
            {
                gfx.DrawEllipse(new XSolidBrush(XColors.Green), x - radius, y - radius, radius * 2, radius * 2);
                gfx.DrawEllipse(new XSolidBrush(XColors.Red), x + w - radius, y + h - radius, radius * 2, radius * 2);
            }
            else
            {
                gfx.DrawEllipse(new XPen(XColors.Green, XUnit.FromMillimeter(0.5)), x - radius, y - radius, radius * 2, radius * 2);
                gfx.DrawEllipse(new XPen(XColors.Red, XUnit.FromMillimeter(0.5)), x + w - radius, y + h - radius, radius * 2, radius * 2);
            }
        }
        internal static void DrawCorner(XGraphics gfx, XUnit x, XUnit y, XColor color)
        {
            XPen pen = new XPen(color, XUnit.FromMillimeter(0.1));
            gfx.DrawLine(pen, x - XUnit.FromMillimeter(10), y, x + XUnit.FromMillimeter(10), y);
            gfx.DrawLine(pen, x, y - XUnit.FromMillimeter(10), x, y + XUnit.FromMillimeter(10));
        }

        internal static void WriteMe(XGraphics gfx, BaseElement elem)
        {
            string text = elem switch
            {
                Page => "PAGE",
                Row => "ROW",
                Column => "COLUMN",
                Text => "TEXT",
                Image => "IMAGE",
                Stamp => "STAMP",
                _ => "Unknown"
            };
            Write(gfx, elem, text);
        }
        internal static void Write(XGraphics gfx, BaseElement elem, string text)
        {
            Write(gfx, elem.X, elem.Y, elem.W, elem.H, text, XColors.Black);
        }
        internal static void Write(XGraphics gfx, Rect rect, string text)
        {
            Write (gfx, rect.X, rect.Y, rect.W, rect.H, text, XColors.Black);
        }
        internal static void Write(XGraphics gfx, XUnit x, XUnit y, XUnit w, XUnit h, string text, XColor color)
        {

            // Write page dimensions on the page
            XFont font = new XFont("Arial", 12, XFontStyle.Regular);
            XBrush textBrush = new XSolidBrush(color);

            XStringFormat stringFormat = new XStringFormat
            {
                Alignment = XStringAlignment.Center,
                LineAlignment = XLineAlignment.Center
            };
            // Calculate text position (center of page)
            x += w / 2;
            y += h / 2;

            gfx.DrawString(text, font, textBrush, x, y, stringFormat);
        }
        internal static void WriteMySize(XGraphics gfx, BaseElement elem)
        {
            string text = $"{elem.W.Millimeter:F2}mm x {elem.H.Millimeter:F2}mm";
            Write(gfx, elem, text);
        }
        internal static void WriteMySize(XGraphics gfx, Rect rect)
        {
            string text = $"{rect.W.Millimeter:F2}mm x {rect.H.Millimeter:F2}mm";
            Write(gfx, rect, text);
        }
    }
}
