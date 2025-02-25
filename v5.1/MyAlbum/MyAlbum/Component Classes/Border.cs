using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Xml.Linq;
using PdfSharp.Drawing;

namespace MyAlbum
{
    class Border : BorderStyle, IDrawable
    {
        #region properties
        public XGraphics? gfx { get; set; }
        public StyleElement? Parent { get; set; }
        public XUnitPt x { get; set; }
        public XUnitPt y { get; set; }
        public XUnitPt h { get; set; }
        public XUnitPt w { get; set; }
        #endregion

        #region Constructors
        public Border()
        {}
        public Border(XElement xml)
        {
            this.xml = xml;
        }
        public Border(XElement xml, StyleElement parent) : this(xml)
        {
            Parent = parent;
        }
        #endregion

        #region Public Methods
        public override void Parse()
        {
            // inherit from parent
            if (Parent != null) { Inherit(); }
            
            // find and apply the style
            ParseStyleNameAttribute();
            ApplyStyleAttributes();

            // apply other explicit attributes
            //type
            ParseBorderTypeAttribute();

            //width
            ParseWidthAttribute();

            //margin
            ParseMarginAttribute();
            
            //padding
            ParsePaddingAttribute();

            //color
            ParseColorAttribute();

            //bgcolor
            ParseBackgroundColorAttribute();
        }
        public void Calculate()
        {
        //    switch (TypeLeft)
        //    {
        //        case "single":
        //            WidthLeft = LineWidth1 / 2;
        //            break;
        //        case "double":
        //            WidthLeft = LineWidth1 / 2 + Offset + LineWidth2;
        //            break;
        //        case "none":
        //            WidthLeft = XUnitPt.Zero;
        //            break;
        //        default:
        //            WidthLeft = XUnitPt.Zero;
        //            break;
        //    }
        //    switch (TypeRight)
        //    {
        //        case "single":
        //            WidthRight = LineWidth1 / 2;
        //            break;
        //        case "double":
        //            WidthRight = LineWidth1 / 2 + Offset + LineWidth2;
        //            break;
        //        case "none":
        //            WidthRight = XUnitPt.Zero;
        //            break;
        //        default:
        //            WidthRight = XUnitPt.Zero;
        //            break;
        //    }
        //    switch (TypeTop)
        //    {
        //        case "single":
        //            WidthTop = LineWidth1 / 2;
        //            break;
        //        case "double":
        //            WidthTop = LineWidth1 / 2 + Offset + LineWidth2;
        //            break;
        //        case "none":
        //            WidthTop = XUnitPt.Zero;
        //            break;
        //        default:
        //            WidthTop = XUnitPt.Zero;
        //            break;
        //    }
        //    switch (TypeBottom)
        //    {
        //        case "single":
        //            WidthBottom = LineWidth1 / 2;
        //            break;
        //        case "double":
        //            WidthBottom = LineWidth1 / 2 + Offset + LineWidth2;
        //            break;
        //        case "none":
        //            WidthBottom = XUnitPt.Zero;
        //            break;
        //        default:
        //            WidthBottom = XUnitPt.Zero;
        //            break;
        //    }
        //    //if (TypeLeft == "single") 
        //    //{ 
        //    //    Width = Height = LineWidth1 / 2; }
        //    //if (BorderType == "double") { Width = Height = LineWidth1 / 2 + Offset + LineWidth2; }

            //    //if (BorderType == "white_ace_page") { Width = Height = XUnitPt.FromMillimeter(5); }

        }
        
        public void Draw()
        {
            //    XPen pen;

            //    #region single border
            //    pen = new XPen(this.Color, LineWidth1);
            //    pen.LineCap = XLineCap.Round;
            //    pen.LineJoin = XLineJoin.Bevel;
            //    if (TypeLeft == "single" || TypeLeft == "double") { gfx.DrawLine(pen, x, y, x, y + h); }
            //    //if (TypeTop == "single" || TypeTop == "double") { gfx.DrawLine(pen, x, y, x + w, y); }
            //    //if (TypeRight == "single" || TypeRight == "double") { gfx.DrawLine(pen, x + w, y, x + w, y + h); }
            //    //if (TypeBottom == "single" || TypeBottom == "double") { gfx.DrawLine(pen, x, y + h, x + w, y + h); }
            //    #endregion

            //    #region double border
            //    XUnitPt space = Offset + LineWidth1;
            //    pen = new XPen(this.Color, LineWidth2);
            //    pen.LineCap = XLineCap.Round;
            //    pen.LineJoin = XLineJoin.Bevel;
            //    if (TypeLeft == "double") { gfx.DrawLine(pen, x + space, y + ((TypeTop != "none") ? space : 0), x + space, y + h - ((TypeBottom != "none") ? space : 0)); }
            //    //if (TypeTop == "double") { gfx.DrawLine(pen, x + ((TypeLeft != "none") ? space : 0), y + space, x + w - ((TypeRight != "none") ? space : 0), y + space); }
            //    //if (TypeRight == "double") { gfx.DrawLine(pen, x + w - space, y + ((TypeTop != "none") ? space : 0), x + w - space, y + h - ((TypeBottom != "none") ? space : 0)); }
            //    //if (TypeBottom == "double") { gfx.DrawLine(pen, x + ((TypeLeft != "none") ? space : 0), y + h - space, x + w - ((TypeRight != "none") ? space : 0), y + h - space); }
            //    #endregion

            //    #region white ace borders - commented out
            //    /*
            //    XPoint[] points;
            //    XImage img;

            //    #region white ace page border
            //    if (BorderType == "white_ace_page")
            //    {
            //        XUnit adj = XUnitPt.FromMillimeter(0.2);
            //        pen = new XPen(this.Color, XUnitPt.FromMillimeter(0.25));

            //        #region top border
            //        // exterior rectangle
            //        gfx.DrawRectangle(pen,
            //            x + Width + Page.VSpace,
            //            y,
            //            w - 2 * Width - 2 * Page.VSpace,
            //            Height);

            //        // center line
            //        gfx.DrawLine(pen,
            //            x + Width + Page.VSpace,
            //            y + Height / 2,
            //            x + w - Width - Page.VSpace,
            //            y + Height / 2);

            //        // center rectangle
            //        gfx.DrawRectangle(pen,
            //            XBrushes.White,
            //            x + w / 2 - XUnitPt.FromMillimeter(20),
            //            y,
            //            XUnitPt.FromMillimeter(40),
            //            Height);

            //        // center text
            //        gfx.DrawString("PARLIAMENT BUILDINGS, OTTAWA",
            //            new XFont("Century Gothic", 6, XFontStyleEx.Regular),
            //            new XSolidBrush(this.Color),
            //            x + w / 2,
            //            y + Height / 2,
            //            XStringFormats.Center);

            //        // center rectangle - left end
            //        points = new XPoint[3];
            //        points[0].X = x + w / 2 - XUnitPt.FromMillimeter(20);
            //        points[0].Y = y + adj;
            //        points[1].X = x + w / 2 - XUnitPt.FromMillimeter(20);
            //        points[1].Y = y + Height - adj;
            //        points[2].X = x + w / 2 - XUnitPt.FromMillimeter(26);
            //        points[2].Y = y + Height / 2;
            //        gfx.DrawPolygon(pen, new XSolidBrush(this.Color), points, XFillMode.Winding);

            //        // center rectangle - right end
            //        points = new XPoint[3];
            //        points[0].X = x + w / 2 + XUnitPt.FromMillimeter(20);
            //        points[0].Y = y + adj;
            //        points[1].X = x + w / 2 + XUnitPt.FromMillimeter(20);
            //        points[1].Y = y + Height - adj;
            //        points[2].X = x + w / 2 + XUnitPt.FromMillimeter(26);
            //        points[2].Y = y + Height / 2;
            //        gfx.DrawPolygon(pen, new XSolidBrush(this.Color), points, XFillMode.Winding);
            //        #endregion

            //        #region left border
            //        // exterior rectangle
            //        gfx.DrawRectangle(pen,
            //            x,
            //            y + Height + Page.VSpace,
            //            Width,
            //            h - 2 * Height - 2 * Page.VSpace);

            //        // center line
            //        gfx.DrawLine(pen,
            //            x + Width / 2,
            //            y + Height + Page.VSpace,
            //            x + Width / 2,
            //            y + h - Height - Page.VSpace);

            //        // top diamond
            //        points = new XPoint[4];
            //        points[0].X = x + Width / 2;
            //        points[0].Y = y + Height + Page.VSpace + adj;
            //        points[1].X = x + adj;
            //        points[1].Y = y + Height + Page.VSpace + Width / Math.Sqrt(2) + adj;
            //        points[2].X = x + Width / 2;
            //        points[2].Y = y + Height + Page.VSpace + 2 * Width / Math.Sqrt(2) + adj;
            //        points[3].X = x + Width - adj;
            //        points[3].Y = y + Height + Page.VSpace + Width / Math.Sqrt(2) + adj;
            //        gfx.DrawPolygon(pen, new XSolidBrush(this.Color), points, XFillMode.Winding);

            //        // middle-top diamond
            //        points = new XPoint[4];
            //        points[0].X = x + Width / 2;
            //        points[0].Y = y + h / 2;
            //        points[1].X = x + adj;
            //        points[1].Y = y + h / 2 - Width / Math.Sqrt(2);
            //        points[2].X = x + Width / 2;
            //        points[2].Y = y + h / 2 - 2 * Width / Math.Sqrt(2);
            //        points[3].X = x + Width - adj;
            //        points[3].Y = y + h / 2 - Width / Math.Sqrt(2);
            //        gfx.DrawPolygon(pen, new XSolidBrush(XColor.FromArgb(239, 65, 73)), points, XFillMode.Winding);

            //        // middle-bottom diamond
            //        points = new XPoint[4];
            //        points[0].X = x + Width / 2;
            //        points[0].Y = y + h / 2;
            //        points[1].X = x + adj;
            //        points[1].Y = y + h / 2 + Width / Math.Sqrt(2);
            //        points[2].X = x + Width / 2;
            //        points[2].Y = y + h / 2 + 2 * Width / Math.Sqrt(2);
            //        points[3].X = x + Width - adj;
            //        points[3].Y = y + h / 2 + Width / Math.Sqrt(2);
            //        gfx.DrawPolygon(pen, new XSolidBrush(XColor.FromArgb(239, 65, 73)), points, XFillMode.Winding);

            //        // bottop diamond
            //        points = new XPoint[4];
            //        points[0].X = x + Width / 2;
            //        points[0].Y = y + h - Height - Page.VSpace - adj;
            //        points[1].X = x + adj;
            //        points[1].Y = y + h - Height - Page.VSpace - Width / Math.Sqrt(2) - adj;
            //        points[2].X = x + Width / 2;
            //        points[2].Y = y + h - Height - Page.VSpace - 2 * Width / Math.Sqrt(2) - adj;
            //        points[3].X = x + Width - adj;
            //        points[3].Y = y + h - Height - Page.VSpace - Width / Math.Sqrt(2) - adj;
            //        gfx.DrawPolygon(pen, new XSolidBrush(this.Color), points, XFillMode.Winding);

            //        #endregion

            //        #region right border
            //        // exterior rectangle
            //        gfx.DrawRectangle(pen,
            //            x + w - Width,
            //            y + Height + Page.VSpace,
            //            Width,
            //            h - 2 * Height - 2 * Page.VSpace);

            //        // center line
            //        gfx.DrawLine(pen,
            //            x + w - Width / 2,
            //            y + Height + Page.VSpace,
            //            x + w - Width / 2,
            //            y + h - Height - Page.VSpace);

            //        // top diamond
            //        points = new XPoint[4];
            //        points[0].X = x + w - Width / 2;
            //        points[0].Y = y + Height + Page.VSpace + adj;
            //        points[1].X = x + w - Width + adj;
            //        points[1].Y = y + Height + Page.VSpace + Width / Math.Sqrt(2) + adj;
            //        points[2].X = x + w - Width / 2;
            //        points[2].Y = y + Height + Page.VSpace + 2 * Width / Math.Sqrt(2) + adj;
            //        points[3].X = x + w - adj;
            //        points[3].Y = y + Height + Page.VSpace + Width / Math.Sqrt(2) + adj;
            //        gfx.DrawPolygon(pen, new XSolidBrush(this.Color), points, XFillMode.Winding);

            //        // middle-top diamond
            //        points = new XPoint[4];
            //        points[0].X = x + w - Width / 2;
            //        points[0].Y = y + h / 2;
            //        points[1].X = x + w - Width + adj;
            //        points[1].Y = y + h / 2 - Width / Math.Sqrt(2);
            //        points[2].X = x + w - Width / 2;
            //        points[2].Y = y + h / 2 - 2 * Width / Math.Sqrt(2);
            //        points[3].X = x + w - adj;
            //        points[3].Y = y + h / 2 - Width / Math.Sqrt(2);
            //        gfx.DrawPolygon(pen, new XSolidBrush(XColor.FromArgb(239, 65, 73)), points, XFillMode.Winding);

            //        // middle-bottom diamond
            //        points = new XPoint[4];
            //        points[0].X = x + w - Width / 2;
            //        points[0].Y = y + h / 2;
            //        points[1].X = x + w - Width + adj;
            //        points[1].Y = y + h / 2 + Width / Math.Sqrt(2);
            //        points[2].X = x + w - Width / 2;
            //        points[2].Y = y + h / 2 + 2 * Width / Math.Sqrt(2);
            //        points[3].X = x + w - adj;
            //        points[3].Y = y + h / 2 + Width / Math.Sqrt(2);
            //        gfx.DrawPolygon(pen, new XSolidBrush(XColor.FromArgb(239, 65, 73)), points, XFillMode.Winding);

            //        // bottop diamond
            //        points = new XPoint[4];
            //        points[0].X = x + w - Width / 2;
            //        points[0].Y = y + h - Height - Page.VSpace - adj;
            //        points[1].X = x + w - Width + adj;
            //        points[1].Y = y + h - Height - Page.VSpace - Width / Math.Sqrt(2) - adj;
            //        points[2].X = x + w - Width / 2;
            //        points[2].Y = y + h - Height - Page.VSpace - 2 * Width / Math.Sqrt(2) - adj;
            //        points[3].X = x + w - adj;
            //        points[3].Y = y + h - Height - Page.VSpace - Width / Math.Sqrt(2) - adj;
            //        gfx.DrawPolygon(pen, new XSolidBrush(this.Color), points, XFillMode.Winding);
            //        #endregion

            //        #region bottom border
            //        // exterior rectangle
            //        gfx.DrawRectangle(pen,
            //            x + Width + Page.VSpace,
            //            y + h + Page.VSpace - Height,
            //            w - 2 * Width - 2 * Page.VSpace,
            //            Height);

            //        // center line
            //        gfx.DrawLine(pen,
            //            x + Width + Page.VSpace,
            //            y + h + Page.VSpace - Height / 2,
            //            x + w - Width - Page.VSpace,
            //            y + h + Page.VSpace - Height / 2);

            //        // coats of arms
            //        if (XImage.ExistsFile("Pictures\\coats.png"))
            //        {
            //            img = XImage.FromFile("Pictures\\coats.png");
            //            gfx.DrawImage(img,
            //                x + w / 2 - 3 * Height * img.Size.Width / img.Size.Height / 2,
            //                y + h - XUnitPt.FromMillimeter(8.5),
            //                3 * Height * img.Size.Width / img.Size.Height,
            //                3 * Height);
            //        }
            //        #endregion

            //        #region corners
            //        //top-left leaf
            //        if (XImage.ExistsFile("Pictures\\leaf_left.png"))
            //        {
            //            img = XImage.FromFile("Pictures\\leaf_left.png");
            //            gfx.DrawImage(img, x, y, Width, Height);
            //        }
            //        //top-right leaf
            //        if (XImage.ExistsFile("Pictures\\leaf_right.png"))
            //        {
            //            img = XImage.FromFile("Pictures\\leaf_right.png");
            //            gfx.DrawImage(img, x + w - Width, y, Width, Height);
            //        }
            //        //bottom-left leaf
            //        if (XImage.ExistsFile("Pictures\\leaf_left.png"))
            //        {
            //            img = XImage.FromFile("Pictures\\leaf_left.png");
            //            gfx.DrawImage(img, x, y + h - Height, Width, Height);
            //        }
            //        //bottom-right leaf
            //        if (XImage.ExistsFile("Pictures\\leaf_right.png"))
            //        {
            //            img = XImage.FromFile("Pictures\\leaf_right.png");
            //            gfx.DrawImage(img, x + w - Width, y + h - Height, Width, Height);
            //        }
            //        #endregion
            //    }
            //    #endregion

            //    #region white ace page border no banner
            //    if (BorderType == "white_ace_page_no_banner")
            //    {
            //        XUnit adj = XUnitPt.FromMillimeter(0.2);
            //        pen = new XPen(this.Color, XUnitPt.FromMillimeter(0.25));

            //        #region top border
            //        // exterior rectangle
            //        gfx.DrawRectangle(pen,
            //            x + Width + Page.VSpace,
            //            y,
            //            w - 2 * Width - 2 * Page.VSpace,
            //            Height);

            //        // center line
            //        gfx.DrawLine(pen,
            //            x + Width + Page.VSpace,
            //            y + Height / 2,
            //            x + w - Width - Page.VSpace,
            //            y + Height / 2);

            //        // center rectangle
            //        gfx.DrawRectangle(pen,
            //            XBrushes.White,
            //            x + w / 2 - XUnitPt.FromMillimeter(20),
            //            y,
            //            XUnitPt.FromMillimeter(40),
            //            Height);

            //        // center text
            //        gfx.DrawString("PARLIAMENT BUILDINGS, OTTAWA",
            //            new XFont("Century Gothic", 6, XFontStyleEx.Regular),
            //            new XSolidBrush(this.Color),
            //            x + w / 2,
            //            y + Height / 2,
            //            XStringFormats.Center);

            //        // center rectangle - left end
            //        points = new XPoint[3];
            //        points[0].X = x + w / 2 - XUnitPt.FromMillimeter(20);
            //        points[0].Y = y + adj;
            //        points[1].X = x + w / 2 - XUnitPt.FromMillimeter(20);
            //        points[1].Y = y + Height - adj;
            //        points[2].X = x + w / 2 - XUnitPt.FromMillimeter(26);
            //        points[2].Y = y + Height / 2;
            //        gfx.DrawPolygon(pen, new XSolidBrush(this.Color), points, XFillMode.Winding);

            //        // center rectangle - right end
            //        points = new XPoint[3];
            //        points[0].X = x + w / 2 + XUnitPt.FromMillimeter(20);
            //        points[0].Y = y + adj;
            //        points[1].X = x + w / 2 + XUnitPt.FromMillimeter(20);
            //        points[1].Y = y + Height - adj;
            //        points[2].X = x + w / 2 + XUnitPt.FromMillimeter(26);
            //        points[2].Y = y + Height / 2;
            //        gfx.DrawPolygon(pen, new XSolidBrush(this.Color), points, XFillMode.Winding);
            //        #endregion

            //        #region left border
            //        // exterior rectangle
            //        gfx.DrawRectangle(pen,
            //            x,
            //            y + Height + Page.VSpace,
            //            Width,
            //            h - 2 * Height - 2 * Page.VSpace);

            //        // center line
            //        gfx.DrawLine(pen,
            //            x + Width / 2,
            //            y + Height + Page.VSpace,
            //            x + Width / 2,
            //            y + h - Height - Page.VSpace);

            //        // top diamond
            //        points = new XPoint[4];
            //        points[0].X = x + Width / 2;
            //        points[0].Y = y + Height + Page.VSpace + adj;
            //        points[1].X = x + adj;
            //        points[1].Y = y + Height + Page.VSpace + Width / Math.Sqrt(2) + adj;
            //        points[2].X = x + Width / 2;
            //        points[2].Y = y + Height + Page.VSpace + 2 * Width / Math.Sqrt(2) + adj;
            //        points[3].X = x + Width - adj;
            //        points[3].Y = y + Height + Page.VSpace + Width / Math.Sqrt(2) + adj;
            //        gfx.DrawPolygon(pen, new XSolidBrush(this.Color), points, XFillMode.Winding);

            //        // middle-top diamond
            //        points = new XPoint[4];
            //        points[0].X = x + Width / 2;
            //        points[0].Y = y + h / 2;
            //        points[1].X = x + adj;
            //        points[1].Y = y + h / 2 - Width / Math.Sqrt(2);
            //        points[2].X = x + Width / 2;
            //        points[2].Y = y + h / 2 - 2 * Width / Math.Sqrt(2);
            //        points[3].X = x + Width - adj;
            //        points[3].Y = y + h / 2 - Width / Math.Sqrt(2);
            //        gfx.DrawPolygon(pen, new XSolidBrush(XColor.FromArgb(239, 65, 73)), points, XFillMode.Winding);

            //        // middle-bottom diamond
            //        points = new XPoint[4];
            //        points[0].X = x + Width / 2;
            //        points[0].Y = y + h / 2;
            //        points[1].X = x + adj;
            //        points[1].Y = y + h / 2 + Width / Math.Sqrt(2);
            //        points[2].X = x + Width / 2;
            //        points[2].Y = y + h / 2 + 2 * Width / Math.Sqrt(2);
            //        points[3].X = x + Width - adj;
            //        points[3].Y = y + h / 2 + Width / Math.Sqrt(2);
            //        gfx.DrawPolygon(pen, new XSolidBrush(XColor.FromArgb(239, 65, 73)), points, XFillMode.Winding);

            //        // bottop diamond
            //        points = new XPoint[4];
            //        points[0].X = x + Width / 2;
            //        points[0].Y = y + h - Height - Page.VSpace - adj;
            //        points[1].X = x + adj;
            //        points[1].Y = y + h - Height - Page.VSpace - Width / Math.Sqrt(2) - adj;
            //        points[2].X = x + Width / 2;
            //        points[2].Y = y + h - Height - Page.VSpace - 2 * Width / Math.Sqrt(2) - adj;
            //        points[3].X = x + Width - adj;
            //        points[3].Y = y + h - Height - Page.VSpace - Width / Math.Sqrt(2) - adj;
            //        gfx.DrawPolygon(pen, new XSolidBrush(this.Color), points, XFillMode.Winding);

            //        #endregion

            //        #region right border
            //        // exterior rectangle
            //        gfx.DrawRectangle(pen,
            //            x + w - Width,
            //            y + Height + Page.VSpace,
            //            Width,
            //            h - 2 * Height - 2 * Page.VSpace);

            //        // center line
            //        gfx.DrawLine(pen,
            //            x + w - Width / 2,
            //            y + Height + Page.VSpace,
            //            x + w - Width / 2,
            //            y + h - Height - Page.VSpace);

            //        // top diamond
            //        points = new XPoint[4];
            //        points[0].X = x + w - Width / 2;
            //        points[0].Y = y + Height + Page.VSpace + adj;
            //        points[1].X = x + w - Width + adj;
            //        points[1].Y = y + Height + Page.VSpace + Width / Math.Sqrt(2) + adj;
            //        points[2].X = x + w - Width / 2;
            //        points[2].Y = y + Height + Page.VSpace + 2 * Width / Math.Sqrt(2) + adj;
            //        points[3].X = x + w - adj;
            //        points[3].Y = y + Height + Page.VSpace + Width / Math.Sqrt(2) + adj;
            //        gfx.DrawPolygon(pen, new XSolidBrush(this.Color), points, XFillMode.Winding);

            //        // middle-top diamond
            //        points = new XPoint[4];
            //        points[0].X = x + w - Width / 2;
            //        points[0].Y = y + h / 2;
            //        points[1].X = x + w - Width + adj;
            //        points[1].Y = y + h / 2 - Width / Math.Sqrt(2);
            //        points[2].X = x + w - Width / 2;
            //        points[2].Y = y + h / 2 - 2 * Width / Math.Sqrt(2);
            //        points[3].X = x + w - adj;
            //        points[3].Y = y + h / 2 - Width / Math.Sqrt(2);
            //        gfx.DrawPolygon(pen, new XSolidBrush(XColor.FromArgb(239, 65, 73)), points, XFillMode.Winding);

            //        // middle-bottom diamond
            //        points = new XPoint[4];
            //        points[0].X = x + w - Width / 2;
            //        points[0].Y = y + h / 2;
            //        points[1].X = x + w - Width + adj;
            //        points[1].Y = y + h / 2 + Width / Math.Sqrt(2);
            //        points[2].X = x + w - Width / 2;
            //        points[2].Y = y + h / 2 + 2 * Width / Math.Sqrt(2);
            //        points[3].X = x + w - adj;
            //        points[3].Y = y + h / 2 + Width / Math.Sqrt(2);
            //        gfx.DrawPolygon(pen, new XSolidBrush(XColor.FromArgb(239, 65, 73)), points, XFillMode.Winding);

            //        // bottop diamond
            //        points = new XPoint[4];
            //        points[0].X = x + w - Width / 2;
            //        points[0].Y = y + h - Height - Page.VSpace - adj;
            //        points[1].X = x + w - Width + adj;
            //        points[1].Y = y + h - Height - Page.VSpace - Width / Math.Sqrt(2) - adj;
            //        points[2].X = x + w - Width / 2;
            //        points[2].Y = y + h - Height - Page.VSpace - 2 * Width / Math.Sqrt(2) - adj;
            //        points[3].X = x + w - adj;
            //        points[3].Y = y + h - Height - Page.VSpace - Width / Math.Sqrt(2) - adj;
            //        gfx.DrawPolygon(pen, new XSolidBrush(this.Color), points, XFillMode.Winding);
            //        #endregion

            //        #region bottom border
            //        // exterior rectangle
            //        gfx.DrawRectangle(pen,
            //            x + Width + Page.VSpace,
            //            y + h + Page.VSpace - Height,
            //            w - 2 * Width - 2 * Page.VSpace,
            //            Height);

            //        // center line
            //        gfx.DrawLine(pen,
            //            x + Width + Page.VSpace,
            //            y + h + Page.VSpace - Height / 2,
            //            x + w - Width - Page.VSpace,
            //            y + h + Page.VSpace - Height / 2);

            //        // coats of arms
            //        if (XImage.ExistsFile("Pictures\\coats.png"))
            //        {
            //            img = XImage.FromFile("Pictures\\coats.png");
            //            gfx.DrawImage(img,
            //                x + w / 2 - 3 * Height * img.Size.Width / img.Size.Height / 2,
            //                y + h - XUnitPt.FromMillimeter(8.5),
            //                3 * Height * img.Size.Width / img.Size.Height,
            //                3 * Height);
            //        }
            //        #endregion

            //        #region corners
            //        //top-left leaf
            //        if (XImage.ExistsFile("Pictures\\leaf_left.png"))
            //        {
            //            img = XImage.FromFile("Pictures\\leaf_left.png");
            //            gfx.DrawImage(img, x, y, Width, Height);
            //        }
            //        //top-right leaf
            //        if (XImage.ExistsFile("Pictures\\leaf_right.png"))
            //        {
            //            img = XImage.FromFile("Pictures\\leaf_right.png");
            //            gfx.DrawImage(img, x + w - Width, y, Width, Height);
            //        }
            //        //bottom-left leaf
            //        if (XImage.ExistsFile("Pictures\\leaf_left.png"))
            //        {
            //            img = XImage.FromFile("Pictures\\leaf_left.png");
            //            gfx.DrawImage(img, x, y + h - Height, Width, Height);
            //        }
            //        //bottom-right leaf
            //        if (XImage.ExistsFile("Pictures\\leaf_right.png"))
            //        {
            //            img = XImage.FromFile("Pictures\\leaf_right.png");
            //            gfx.DrawImage(img, x + w - Width, y + h - Height, Width, Height);
            //        }
            //        #endregion
            //    }
            //    #endregion

            //    #region white ace stamp border
            //    if (BorderType == "white_ace_stamp")
            //    {
            //        #region borders
            //        pen = new XPen(this.Color, LineWidth1);
            //        gfx.DrawRectangle(pen, x, y, w, h);

            //        pen = new XPen(this.Color, LineWidth2);
            //        gfx.DrawRectangle(pen, x + Offset, y + Offset, w - 2 * Offset, h - 2 * Offset);

            //        XColor shadow = XColor.FromArgb(Color.R + (256 - Color.R) / 2, Color.G + (256 - Color.G) / 2, Color.B + (256 - Color.B) / 2);
            //        pen = new XPen(shadow, LineWidth1 / 2);
            //        gfx.DrawRectangle(pen, x + Offset / 2, y + Offset / 2, w - Offset, h - Offset);
            //        #endregion

            //        Gizmo1 gizmo = new Gizmo1();
            //        gizmo.gfx = this.gfx;
            //        gizmo.Color = Color;
            //        gizmo.LineWidth = LineWidth1;

            //        #region horizontal gizmos
            //        if (w >= h)
            //        {
            //            // first gizmo on the top border
            //            gizmo.x = x + (w / 4) - 1.1 * Offset;
            //            gizmo.y = y;
            //            gizmo.w = 2.2 * Offset;
            //            gizmo.h = Offset;
            //            gizmo.Draw();

            //            // second gizmo on the top border
            //            gizmo.x = x + w - (w / 4) - 1.1 * Offset;
            //            gizmo.y = y;
            //            gizmo.w = 2.2 * Offset;
            //            gizmo.h = Offset;
            //            gizmo.Draw();

            //            // first gizmo on the bottom border
            //            gizmo.x = x + (w / 4) - 1.1 * Offset;
            //            gizmo.y = y + h - Offset;
            //            gizmo.w = 2.2 * Offset;
            //            gizmo.h = Offset;
            //            gizmo.Draw(reverse: true);

            //            // second gizmo on the bottom border
            //            gizmo.x = x + w - (w / 4) - 1.1 * Offset;
            //            gizmo.y = y + h - Offset;
            //            gizmo.w = 2.2 * Offset;
            //            gizmo.h = Offset;
            //            gizmo.Draw(reverse: true);
            //        }
            //        else
            //        {
            //            //// one gizmo on the top border
            //            gizmo.x = x + (w / 2) - 1.1 * Offset;
            //            gizmo.y = y;
            //            gizmo.w = 2.2 * Offset;
            //            gizmo.h = Offset;
            //            gizmo.Draw();

            //            //// one gizmo on the bottom border
            //            gizmo.x = x + w - (w / 2) - 1.1 * Offset;
            //            gizmo.y = y + h - Offset;
            //            gizmo.w = 2.2 * Offset;
            //            gizmo.h = Offset;
            //            gizmo.Draw(reverse: true);
            //        }
            //        #endregion

            //        #region vertical gizmos
            //        if (w > h)
            //        {
            //            // one gizmo on the left border
            //            gizmo.x = x;
            //            gizmo.y = y + (h / 2) - 1.1 * Offset;
            //            gizmo.w = Offset;
            //            gizmo.h = 2.2 * Offset;
            //            gizmo.Draw();

            //            // one gizmo on the right border
            //            gizmo.x = x + w - Offset;
            //            gizmo.y = y + (h / 2) - 1.1 * Offset;
            //            gizmo.w = Offset;
            //            gizmo.h = 2.2 * Offset;
            //            gizmo.Draw(reverse: true);
            //        }
            //        else 
            //        {
            //            // first gizmo on the left border
            //            gizmo.x = x + (w / 4) - 1.1 * Offset;
            //            gizmo.x = x;
            //            gizmo.y = y;
            //            gizmo.y = y + (h / 4) - 1.1 * Offset;
            //            gizmo.w = Offset;
            //            gizmo.h = 2.2 * Offset;
            //            gizmo.Draw();

            //            // second gizmo on the left border
            //            gizmo.x = x + w - (w / 4) - 1.1 * Offset;
            //            gizmo.x = x;
            //            gizmo.y = y;
            //            gizmo.y = y + h - (h / 4) - 1.1 * Offset;
            //            gizmo.w = Offset;
            //            gizmo.h = 2.2 * Offset;
            //            gizmo.Draw();

            //            // first gizmo on the right border
            //            gizmo.x = x + (w / 4) - 1.1 * Offset;
            //            gizmo.x = x + w - Offset;
            //            gizmo.y = y + h - Offset;
            //            gizmo.y = y + (h / 4) - 1.1 * Offset;
            //            gizmo.w = Offset;
            //            gizmo.h = 2.2 * Offset;
            //            gizmo.Draw(reverse: true);

            //            // second gizmo on the right border
            //            gizmo.x = x + w - (w / 4) - 1.1 * Offset;
            //            gizmo.x = x + w - Offset;
            //            gizmo.y = y + h - Offset;
            //            gizmo.y = y + h - (h / 4) - 1.1 * Offset;
            //            gizmo.w = Offset;
            //            gizmo.h = 2.2 * Offset;
            //            gizmo.Draw(reverse: true);
            //        }
            //        //if (h > w)
            //        //{
            //        //    // first gizmo on the left border
            //        //    x = XPos - _borderWidth1 / 2;
            //        //    y = YPos + (FrameHeight / 4) - _borderSpace;
            //        //    w = _borderWidth1 + _borderSpace + _borderWidth2;
            //        //    h = 2 * (_borderWidth1 + _borderSpace + _borderWidth2);
            //        //    DrawGizmo1(x, y, w, h);

            //        //    // second gizmo on the left border
            //        //    x = XPos - _borderWidth1 / 2;
            //        //    y = YPos + 3 * (FrameHeight / 4) - _borderSpace;
            //        //    w = _borderWidth1 + _borderSpace + _borderWidth2;
            //        //    h = 2 * (_borderWidth1 + _borderSpace + _borderWidth2);
            //        //    DrawGizmo1(x, y, w, h);

            //        //    // first gizmo on the right border
            //        //    x = XPos + FrameWidth - (_borderWidth1 + _borderSpace) - _borderWidth1 / 2;
            //        //    y = YPos + (FrameHeight / 4) - _borderSpace;
            //        //    //y = YPos + FrameHeight - (_borderWidth1 + _borderSpace) - _borderWidth1 / 2;
            //        //    w = _borderWidth1 + _borderSpace + _borderWidth2;
            //        //    h = 2 * (_borderWidth1 + _borderSpace + _borderWidth2);
            //        //    DrawGizmo1(x, y, w, h);

            //        //    // second gizmo on the right border
            //        //    x = XPos + FrameWidth - (_borderWidth1 + _borderSpace) - _borderWidth1 / 2;
            //        //    y = YPos + 3 * (FrameHeight / 4) - _borderSpace;
            //        //    //y = YPos + FrameHeight - (_borderWidth1 + _borderSpace) - _borderWidth1 / 2;
            //        //    w = _borderWidth1 + _borderSpace + _borderWidth2;
            //        //    h = 2 * (_borderWidth1 + _borderSpace + _borderWidth2);
            //        //    DrawGizmo1(x, y, w, h);
            //        //}
            //        //else
            //        //{
            //        //    // one gizmo on the left border
            //        //    x = XPos - _borderWidth1 / 2;
            //        //    y = YPos + (FrameHeight / 2) - _borderSpace;
            //        //    w = _borderWidth1 + _borderSpace + _borderWidth2;
            //        //    h = 2 * (_borderWidth1 + _borderSpace + _borderWidth2);
            //        //    DrawGizmo1(x, y, w, h);

            //        //    // one gizmo on the right border
            //        //    x = XPos + FrameWidth - (_borderWidth1 + _borderSpace) - _borderWidth1 / 2;
            //        //    y = YPos + (FrameHeight / 2) - _borderSpace;
            //        //    w = _borderWidth1 + _borderSpace + _borderWidth2;
            //        //    h = 2 * (_borderWidth1 + _borderSpace + _borderWidth2);
            //        //    DrawGizmo1(x, y, w, h);
            //        //}
            //        #endregion

            //    }
            //    #endregion
            //    */
            //    #endregion
        }
        #endregion

        #region Private Methods
        private void Inherit()
        {
            this.Brush = Parent!.Brush;
            this.Color = Parent!.Color;
        }
        
        // override the ParseStyleNameAttribute because this time
        // it is okay for the attribute "style" to be missing
        private void ParseStyleNameAttribute()
        {
            //Console.WriteLine(xml);
            this.StyleName = xml.Attribute("style")?.Value ?? "";
        }

        private void ApplyStyleAttributes()
        {
            // find style
            BorderStyle? style;
            style = Styles.BorderStyles.Find(e => e.StyleName == StyleName);
            // if not found look for default style
            if (style == null)
            {
                style = Styles.BorderStyles.Find(e => e.IsDefault == true);
            }

            // apply style
            if (style != null)
            {
                // type
                TypeLeft = style.TypeLeft;
                TypeTop = style.TypeTop;
                TypeRight = style.TypeRight;
                TypeBottom = style.TypeBottom;
                
                // widths
                LineWidth1 = style.LineWidth1;
                LineWidth2 = style.LineWidth2;
                Offset = style.Offset;

                // margin
                MarginLeft = style.MarginLeft;
                MarginRight = style.MarginRight;
                MarginTop = style.MarginTop;
                MarginBottom = style.MarginBottom;

                // padding
                PaddingLeft = style.PaddingLeft;
                PaddingRight = style.PaddingRight;
                PaddingTop = style.PaddingTop;
                PaddingBottom = style.PaddingBottom;
                
                // brush
                Brush = style.Brush;

                // color
                Color = style.Color;
            }
        }
        #endregion
    }
}
