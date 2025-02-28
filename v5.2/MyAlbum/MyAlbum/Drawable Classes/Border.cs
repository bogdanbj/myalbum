using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

using System.Xml.Linq;
using PdfSharp.Drawing;

//using MyAlbum.Styles;

namespace MyAlbum
{
    class Border : DrawableElement
    {
        #region properties
        public new BorderStyle Style { get; set; }
        //public XUnitPt LineWidth1 { get; set; }
        //public XUnitPt LineWidth2 { get; set; }
        //public XUnitPt Offset { get; set; }
        //public string? TypeLeft { get; set; }
        //public string? TypeRight { get; set; }
        //public string? TypeTop { get; set; }
        //public string? TypeBottom { get; set; }
        public XUnitPt WidthLeft { get; set; }
        public XUnitPt WidthRight { get; set; }
        public XUnitPt WidthTop { get; set; }
        public XUnitPt WidthBottom { get; set; }
        #endregion

        #region Constructors
        public Border()
        {}
        public Border(XElement xml)
        {
            this.xml = xml;
        }
        public Border(XElement xml, DrawableElement parent) : this(xml)
        {
            Parent = parent;
        }
        #endregion

        #region Public Methods
        public override void Parse()
        {
            // inherit from parent
            if (Parent != null)
            {
                Style.Color = Parent.Style.Color;
                Style.Brush = Parent.Style.Brush;
            }

            StyleName = ParseStyleNameAttribute(xml);
            if (!string.IsNullOrEmpty(StyleName))
            {
                Style = Styles.BorderStyles.FirstOrDefault(s => s.StyleName == StyleName);
            }

            // apply other explicit attributes
            // margins
            if (xml.Attribute("margin") != null)
            {
                XUnitPt[] arr = ParseMarginAttribute(this.xml);
                Style.MarginTop = arr[0];
                Style.MarginRight = arr[1];
                Style.MarginBottom = arr[2];
                Style.MarginLeft = arr[3];
            }

            // padding
            if (xml.Attribute("padding") != null)
            {
                XUnitPt[] arr = ParsePaddingAttribute(this.xml);
                Style.PaddingTop = arr[0];
                Style.PaddingRight = arr[1];
                Style.PaddingBottom = arr[2];
                Style.PaddingLeft = arr[3];
            }

            // color
            if (xml.Attribute("color") != null)
            {
                Style.Color = ParseColorAttribute(this.xml);
            }

            // background color 
            if (xml.Attribute("bgcolor") != null)
            {
                Style.Brush = ParseBackgroundColorAttribute(this.xml);
            }

            //align
            // not relevant for Border

            //valign
            // not relevant for Border

            // border type
            Styles.BorderTypes[] types = ParseBorderTypeAttribute(xml);
            Style.TypeTop = types[0];
            Style.TypeRight = types[1];
            Style.TypeBottom = types[2];
            Style.TypeLeft = types[3];

            // lines and offset width
            XUnitPt[] widths = ParseBorderLineWidthAttribute(xml);
            Style.LineWidth1 = widths[0];
            Style.Offset = widths[1];
            Style.LineWidth2 = widths[2];
        }
        public override void Calculate()
        {
            switch (Style.TypeLeft)
            {
                case Styles.BorderTypes.Single:
                    WidthLeft = Style.LineWidth1 / 2;
                    break;
                case Styles.BorderTypes.Double:
                    WidthLeft = Style.LineWidth1 / 2 + Style.Offset + Style.LineWidth2;
                    break;
                default:
                    WidthLeft = XUnitPt.Zero;
                    break;
            }
            switch (Style.TypeRight)
            {
                case Styles.BorderTypes.Single:
                    WidthRight = Style.LineWidth1 / 2;
                    break;
                case Styles.BorderTypes.Double:
                    WidthRight = Style.LineWidth1 / 2 + Style.Offset + Style.LineWidth2;
                    break;
                default:
                    WidthRight = XUnitPt.Zero;
                    break;
            }
            switch (Style.TypeTop)
            {
                case Styles.BorderTypes.Single:
                    WidthTop = Style.LineWidth1 / 2;
                    break;
                case Styles.BorderTypes.Double:
                    WidthTop = Style.LineWidth1 / 2 + Style.Offset + Style.LineWidth2;
                    break;
                default:
                    WidthTop = XUnitPt.Zero;
                    break;
            }
            switch (Style.TypeBottom)
            {
                case Styles.BorderTypes.Single:
                    WidthBottom = Style.LineWidth1 / 2;
                    break;
                case Styles.BorderTypes.Double:
                    WidthBottom = Style.LineWidth1 / 2 + Style.Offset + Style.LineWidth2;
                    break;
                default:
                    WidthBottom = XUnitPt.Zero;
                    break;
            }
        }
        public override void Draw()
        {
            XPen pen;

            #region single border
            pen = new XPen(Style.Color, Style.LineWidth1);
            pen.LineCap = XLineCap.Round;
            pen.LineJoin = XLineJoin.Bevel;
            //pen.Color = XColors.Red;
            if (Style.TypeLeft == Styles.BorderTypes.Single || Style.TypeLeft == Styles.BorderTypes.Double) { gfx.DrawLine(pen, x, y, x, y + h); }
            //pen.Color = XColors.Yellow;
            if (Style.TypeTop == Styles.BorderTypes.Single || Style.TypeTop == Styles.BorderTypes.Double) { gfx.DrawLine(pen, x, y, x + w, y); }
            //pen.Color = XColors.Blue;
            if (Style.TypeRight == Styles.BorderTypes.Single || Style.TypeRight == Styles.BorderTypes.Double) { gfx.DrawLine(pen, x + w, y, x + w, y + h); }
            //pen.Color = XColors.Green;
            if (Style.TypeBottom == Styles.BorderTypes.Single || Style.TypeBottom == Styles.BorderTypes.Double) { gfx.DrawLine(pen, x, y + h, x + w, y + h); }
            #endregion

            #region double border
            XUnitPt space = Style.Offset + Style.LineWidth1;
            pen = new XPen(Style.Color, Style.LineWidth2);
            pen.LineCap = XLineCap.Round;
            pen.LineJoin = XLineJoin.Bevel;
            if (Style.TypeLeft == Styles.BorderTypes.Double) { gfx.DrawLine(pen, x + space, y + ((Style.TypeTop != Styles.BorderTypes.None) ? space : 0), x + space, y + h - ((Style.TypeBottom != Styles.BorderTypes.None) ? space : 0)); }
            if (Style.TypeTop == Styles.BorderTypes.Double) { gfx.DrawLine(pen, x + ((Style.TypeLeft != Styles.BorderTypes.None) ? space : 0), y + space, x + w - ((Style.TypeRight != Styles.BorderTypes.None) ? space : 0), y + space); }
            if (Style.TypeRight == Styles.BorderTypes.Double) { gfx.DrawLine(pen, x + w - space, y + ((Style.TypeTop != Styles.BorderTypes.None) ? space : 0), x + w - space, y + h - ((Style.TypeBottom != Styles.BorderTypes.None) ? space : 0)); }
            if (Style.TypeBottom == Styles.BorderTypes.Double) { gfx.DrawLine(pen, x + ((Style.TypeLeft != Styles.BorderTypes.None) ? space : 0), y + h - space, x + w - ((Style.TypeRight != Styles.BorderTypes.None) ? space : 0), y + h - space); }
            #endregion

            #region white ace borders - commented out
            /*
            XPoint[] points;
            XImage img;

            #region white ace page border
            if (BorderType == "white_ace_page")
            {
                XUnit adj = XUnitPt.FromMillimeter(0.2);
                pen = new XPen(this.Color, XUnitPt.FromMillimeter(0.25));

                #region top border
                // exterior rectangle
                gfx.DrawRectangle(pen,
                    x + Width + Page.VSpace,
                    y,
                    w - 2 * Width - 2 * Page.VSpace,
                    Height);

                // center line
                gfx.DrawLine(pen,
                    x + Width + Page.VSpace,
                    y + Height / 2,
                    x + w - Width - Page.VSpace,
                    y + Height / 2);

                // center rectangle
                gfx.DrawRectangle(pen,
                    XBrushes.White,
                    x + w / 2 - XUnitPt.FromMillimeter(20),
                    y,
                    XUnitPt.FromMillimeter(40),
                    Height);

                // center text
                gfx.DrawString("PARLIAMENT BUILDINGS, OTTAWA",
                    new XFont("Century Gothic", 6, XFontStyleEx.Regular),
                    new XSolidBrush(this.Color),
                    x + w / 2,
                    y + Height / 2,
                    XStringFormats.Center);

                // center rectangle - left end
                points = new XPoint[3];
                points[0].X = x + w / 2 - XUnitPt.FromMillimeter(20);
                points[0].Y = y + adj;
                points[1].X = x + w / 2 - XUnitPt.FromMillimeter(20);
                points[1].Y = y + Height - adj;
                points[2].X = x + w / 2 - XUnitPt.FromMillimeter(26);
                points[2].Y = y + Height / 2;
                gfx.DrawPolygon(pen, new XSolidBrush(this.Color), points, XFillMode.Winding);

                // center rectangle - right end
                points = new XPoint[3];
                points[0].X = x + w / 2 + XUnitPt.FromMillimeter(20);
                points[0].Y = y + adj;
                points[1].X = x + w / 2 + XUnitPt.FromMillimeter(20);
                points[1].Y = y + Height - adj;
                points[2].X = x + w / 2 + XUnitPt.FromMillimeter(26);
                points[2].Y = y + Height / 2;
                gfx.DrawPolygon(pen, new XSolidBrush(this.Color), points, XFillMode.Winding);
                #endregion

                #region left border
                // exterior rectangle
                gfx.DrawRectangle(pen,
                    x,
                    y + Height + Page.VSpace,
                    Width,
                    h - 2 * Height - 2 * Page.VSpace);

                // center line
                gfx.DrawLine(pen,
                    x + Width / 2,
                    y + Height + Page.VSpace,
                    x + Width / 2,
                    y + h - Height - Page.VSpace);

                // top diamond
                points = new XPoint[4];
                points[0].X = x + Width / 2;
                points[0].Y = y + Height + Page.VSpace + adj;
                points[1].X = x + adj;
                points[1].Y = y + Height + Page.VSpace + Width / Math.Sqrt(2) + adj;
                points[2].X = x + Width / 2;
                points[2].Y = y + Height + Page.VSpace + 2 * Width / Math.Sqrt(2) + adj;
                points[3].X = x + Width - adj;
                points[3].Y = y + Height + Page.VSpace + Width / Math.Sqrt(2) + adj;
                gfx.DrawPolygon(pen, new XSolidBrush(this.Color), points, XFillMode.Winding);

                // middle-top diamond
                points = new XPoint[4];
                points[0].X = x + Width / 2;
                points[0].Y = y + h / 2;
                points[1].X = x + adj;
                points[1].Y = y + h / 2 - Width / Math.Sqrt(2);
                points[2].X = x + Width / 2;
                points[2].Y = y + h / 2 - 2 * Width / Math.Sqrt(2);
                points[3].X = x + Width - adj;
                points[3].Y = y + h / 2 - Width / Math.Sqrt(2);
                gfx.DrawPolygon(pen, new XSolidBrush(XColor.FromArgb(239, 65, 73)), points, XFillMode.Winding);

                // middle-bottom diamond
                points = new XPoint[4];
                points[0].X = x + Width / 2;
                points[0].Y = y + h / 2;
                points[1].X = x + adj;
                points[1].Y = y + h / 2 + Width / Math.Sqrt(2);
                points[2].X = x + Width / 2;
                points[2].Y = y + h / 2 + 2 * Width / Math.Sqrt(2);
                points[3].X = x + Width - adj;
                points[3].Y = y + h / 2 + Width / Math.Sqrt(2);
                gfx.DrawPolygon(pen, new XSolidBrush(XColor.FromArgb(239, 65, 73)), points, XFillMode.Winding);

                // bottop diamond
                points = new XPoint[4];
                points[0].X = x + Width / 2;
                points[0].Y = y + h - Height - Page.VSpace - adj;
                points[1].X = x + adj;
                points[1].Y = y + h - Height - Page.VSpace - Width / Math.Sqrt(2) - adj;
                points[2].X = x + Width / 2;
                points[2].Y = y + h - Height - Page.VSpace - 2 * Width / Math.Sqrt(2) - adj;
                points[3].X = x + Width - adj;
                points[3].Y = y + h - Height - Page.VSpace - Width / Math.Sqrt(2) - adj;
                gfx.DrawPolygon(pen, new XSolidBrush(this.Color), points, XFillMode.Winding);

                #endregion

                #region right border
                // exterior rectangle
                gfx.DrawRectangle(pen,
                    x + w - Width,
                    y + Height + Page.VSpace,
                    Width,
                    h - 2 * Height - 2 * Page.VSpace);

                // center line
                gfx.DrawLine(pen,
                    x + w - Width / 2,
                    y + Height + Page.VSpace,
                    x + w - Width / 2,
                    y + h - Height - Page.VSpace);

                // top diamond
                points = new XPoint[4];
                points[0].X = x + w - Width / 2;
                points[0].Y = y + Height + Page.VSpace + adj;
                points[1].X = x + w - Width + adj;
                points[1].Y = y + Height + Page.VSpace + Width / Math.Sqrt(2) + adj;
                points[2].X = x + w - Width / 2;
                points[2].Y = y + Height + Page.VSpace + 2 * Width / Math.Sqrt(2) + adj;
                points[3].X = x + w - adj;
                points[3].Y = y + Height + Page.VSpace + Width / Math.Sqrt(2) + adj;
                gfx.DrawPolygon(pen, new XSolidBrush(this.Color), points, XFillMode.Winding);

                // middle-top diamond
                points = new XPoint[4];
                points[0].X = x + w - Width / 2;
                points[0].Y = y + h / 2;
                points[1].X = x + w - Width + adj;
                points[1].Y = y + h / 2 - Width / Math.Sqrt(2);
                points[2].X = x + w - Width / 2;
                points[2].Y = y + h / 2 - 2 * Width / Math.Sqrt(2);
                points[3].X = x + w - adj;
                points[3].Y = y + h / 2 - Width / Math.Sqrt(2);
                gfx.DrawPolygon(pen, new XSolidBrush(XColor.FromArgb(239, 65, 73)), points, XFillMode.Winding);

                // middle-bottom diamond
                points = new XPoint[4];
                points[0].X = x + w - Width / 2;
                points[0].Y = y + h / 2;
                points[1].X = x + w - Width + adj;
                points[1].Y = y + h / 2 + Width / Math.Sqrt(2);
                points[2].X = x + w - Width / 2;
                points[2].Y = y + h / 2 + 2 * Width / Math.Sqrt(2);
                points[3].X = x + w - adj;
                points[3].Y = y + h / 2 + Width / Math.Sqrt(2);
                gfx.DrawPolygon(pen, new XSolidBrush(XColor.FromArgb(239, 65, 73)), points, XFillMode.Winding);

                // bottop diamond
                points = new XPoint[4];
                points[0].X = x + w - Width / 2;
                points[0].Y = y + h - Height - Page.VSpace - adj;
                points[1].X = x + w - Width + adj;
                points[1].Y = y + h - Height - Page.VSpace - Width / Math.Sqrt(2) - adj;
                points[2].X = x + w - Width / 2;
                points[2].Y = y + h - Height - Page.VSpace - 2 * Width / Math.Sqrt(2) - adj;
                points[3].X = x + w - adj;
                points[3].Y = y + h - Height - Page.VSpace - Width / Math.Sqrt(2) - adj;
                gfx.DrawPolygon(pen, new XSolidBrush(this.Color), points, XFillMode.Winding);
                #endregion

                #region bottom border
                // exterior rectangle
                gfx.DrawRectangle(pen,
                    x + Width + Page.VSpace,
                    y + h + Page.VSpace - Height,
                    w - 2 * Width - 2 * Page.VSpace,
                    Height);

                // center line
                gfx.DrawLine(pen,
                    x + Width + Page.VSpace,
                    y + h + Page.VSpace - Height / 2,
                    x + w - Width - Page.VSpace,
                    y + h + Page.VSpace - Height / 2);

                // coats of arms
                if (XImage.ExistsFile("Pictures\\coats.png"))
                {
                    img = XImage.FromFile("Pictures\\coats.png");
                    gfx.DrawImage(img,
                        x + w / 2 - 3 * Height * img.Size.Width / img.Size.Height / 2,
                        y + h - XUnitPt.FromMillimeter(8.5),
                        3 * Height * img.Size.Width / img.Size.Height,
                        3 * Height);
                }
                #endregion

                #region corners
                //top-left leaf
                if (XImage.ExistsFile("Pictures\\leaf_left.png"))
                {
                    img = XImage.FromFile("Pictures\\leaf_left.png");
                    gfx.DrawImage(img, x, y, Width, Height);
                }
                //top-right leaf
                if (XImage.ExistsFile("Pictures\\leaf_right.png"))
                {
                    img = XImage.FromFile("Pictures\\leaf_right.png");
                    gfx.DrawImage(img, x + w - Width, y, Width, Height);
                }
                //bottom-left leaf
                if (XImage.ExistsFile("Pictures\\leaf_left.png"))
                {
                    img = XImage.FromFile("Pictures\\leaf_left.png");
                    gfx.DrawImage(img, x, y + h - Height, Width, Height);
                }
                //bottom-right leaf
                if (XImage.ExistsFile("Pictures\\leaf_right.png"))
                {
                    img = XImage.FromFile("Pictures\\leaf_right.png");
                    gfx.DrawImage(img, x + w - Width, y + h - Height, Width, Height);
                }
                #endregion
            }
            #endregion

            #region white ace page border no banner
            if (BorderType == "white_ace_page_no_banner")
            {
                XUnit adj = XUnitPt.FromMillimeter(0.2);
                pen = new XPen(this.Color, XUnitPt.FromMillimeter(0.25));

                #region top border
                // exterior rectangle
                gfx.DrawRectangle(pen,
                    x + Width + Page.VSpace,
                    y,
                    w - 2 * Width - 2 * Page.VSpace,
                    Height);

                // center line
                gfx.DrawLine(pen,
                    x + Width + Page.VSpace,
                    y + Height / 2,
                    x + w - Width - Page.VSpace,
                    y + Height / 2);

                // center rectangle
                gfx.DrawRectangle(pen,
                    XBrushes.White,
                    x + w / 2 - XUnitPt.FromMillimeter(20),
                    y,
                    XUnitPt.FromMillimeter(40),
                    Height);

                // center text
                gfx.DrawString("PARLIAMENT BUILDINGS, OTTAWA",
                    new XFont("Century Gothic", 6, XFontStyleEx.Regular),
                    new XSolidBrush(this.Color),
                    x + w / 2,
                    y + Height / 2,
                    XStringFormats.Center);

                // center rectangle - left end
                points = new XPoint[3];
                points[0].X = x + w / 2 - XUnitPt.FromMillimeter(20);
                points[0].Y = y + adj;
                points[1].X = x + w / 2 - XUnitPt.FromMillimeter(20);
                points[1].Y = y + Height - adj;
                points[2].X = x + w / 2 - XUnitPt.FromMillimeter(26);
                points[2].Y = y + Height / 2;
                gfx.DrawPolygon(pen, new XSolidBrush(this.Color), points, XFillMode.Winding);

                // center rectangle - right end
                points = new XPoint[3];
                points[0].X = x + w / 2 + XUnitPt.FromMillimeter(20);
                points[0].Y = y + adj;
                points[1].X = x + w / 2 + XUnitPt.FromMillimeter(20);
                points[1].Y = y + Height - adj;
                points[2].X = x + w / 2 + XUnitPt.FromMillimeter(26);
                points[2].Y = y + Height / 2;
                gfx.DrawPolygon(pen, new XSolidBrush(this.Color), points, XFillMode.Winding);
                #endregion

                #region left border
                // exterior rectangle
                gfx.DrawRectangle(pen,
                    x,
                    y + Height + Page.VSpace,
                    Width,
                    h - 2 * Height - 2 * Page.VSpace);

                // center line
                gfx.DrawLine(pen,
                    x + Width / 2,
                    y + Height + Page.VSpace,
                    x + Width / 2,
                    y + h - Height - Page.VSpace);

                // top diamond
                points = new XPoint[4];
                points[0].X = x + Width / 2;
                points[0].Y = y + Height + Page.VSpace + adj;
                points[1].X = x + adj;
                points[1].Y = y + Height + Page.VSpace + Width / Math.Sqrt(2) + adj;
                points[2].X = x + Width / 2;
                points[2].Y = y + Height + Page.VSpace + 2 * Width / Math.Sqrt(2) + adj;
                points[3].X = x + Width - adj;
                points[3].Y = y + Height + Page.VSpace + Width / Math.Sqrt(2) + adj;
                gfx.DrawPolygon(pen, new XSolidBrush(this.Color), points, XFillMode.Winding);

                // middle-top diamond
                points = new XPoint[4];
                points[0].X = x + Width / 2;
                points[0].Y = y + h / 2;
                points[1].X = x + adj;
                points[1].Y = y + h / 2 - Width / Math.Sqrt(2);
                points[2].X = x + Width / 2;
                points[2].Y = y + h / 2 - 2 * Width / Math.Sqrt(2);
                points[3].X = x + Width - adj;
                points[3].Y = y + h / 2 - Width / Math.Sqrt(2);
                gfx.DrawPolygon(pen, new XSolidBrush(XColor.FromArgb(239, 65, 73)), points, XFillMode.Winding);

                // middle-bottom diamond
                points = new XPoint[4];
                points[0].X = x + Width / 2;
                points[0].Y = y + h / 2;
                points[1].X = x + adj;
                points[1].Y = y + h / 2 + Width / Math.Sqrt(2);
                points[2].X = x + Width / 2;
                points[2].Y = y + h / 2 + 2 * Width / Math.Sqrt(2);
                points[3].X = x + Width - adj;
                points[3].Y = y + h / 2 + Width / Math.Sqrt(2);
                gfx.DrawPolygon(pen, new XSolidBrush(XColor.FromArgb(239, 65, 73)), points, XFillMode.Winding);

                // bottop diamond
                points = new XPoint[4];
                points[0].X = x + Width / 2;
                points[0].Y = y + h - Height - Page.VSpace - adj;
                points[1].X = x + adj;
                points[1].Y = y + h - Height - Page.VSpace - Width / Math.Sqrt(2) - adj;
                points[2].X = x + Width / 2;
                points[2].Y = y + h - Height - Page.VSpace - 2 * Width / Math.Sqrt(2) - adj;
                points[3].X = x + Width - adj;
                points[3].Y = y + h - Height - Page.VSpace - Width / Math.Sqrt(2) - adj;
                gfx.DrawPolygon(pen, new XSolidBrush(this.Color), points, XFillMode.Winding);

                #endregion

                #region right border
                // exterior rectangle
                gfx.DrawRectangle(pen,
                    x + w - Width,
                    y + Height + Page.VSpace,
                    Width,
                    h - 2 * Height - 2 * Page.VSpace);

                // center line
                gfx.DrawLine(pen,
                    x + w - Width / 2,
                    y + Height + Page.VSpace,
                    x + w - Width / 2,
                    y + h - Height - Page.VSpace);

                // top diamond
                points = new XPoint[4];
                points[0].X = x + w - Width / 2;
                points[0].Y = y + Height + Page.VSpace + adj;
                points[1].X = x + w - Width + adj;
                points[1].Y = y + Height + Page.VSpace + Width / Math.Sqrt(2) + adj;
                points[2].X = x + w - Width / 2;
                points[2].Y = y + Height + Page.VSpace + 2 * Width / Math.Sqrt(2) + adj;
                points[3].X = x + w - adj;
                points[3].Y = y + Height + Page.VSpace + Width / Math.Sqrt(2) + adj;
                gfx.DrawPolygon(pen, new XSolidBrush(this.Color), points, XFillMode.Winding);

                // middle-top diamond
                points = new XPoint[4];
                points[0].X = x + w - Width / 2;
                points[0].Y = y + h / 2;
                points[1].X = x + w - Width + adj;
                points[1].Y = y + h / 2 - Width / Math.Sqrt(2);
                points[2].X = x + w - Width / 2;
                points[2].Y = y + h / 2 - 2 * Width / Math.Sqrt(2);
                points[3].X = x + w - adj;
                points[3].Y = y + h / 2 - Width / Math.Sqrt(2);
                gfx.DrawPolygon(pen, new XSolidBrush(XColor.FromArgb(239, 65, 73)), points, XFillMode.Winding);

                // middle-bottom diamond
                points = new XPoint[4];
                points[0].X = x + w - Width / 2;
                points[0].Y = y + h / 2;
                points[1].X = x + w - Width + adj;
                points[1].Y = y + h / 2 + Width / Math.Sqrt(2);
                points[2].X = x + w - Width / 2;
                points[2].Y = y + h / 2 + 2 * Width / Math.Sqrt(2);
                points[3].X = x + w - adj;
                points[3].Y = y + h / 2 + Width / Math.Sqrt(2);
                gfx.DrawPolygon(pen, new XSolidBrush(XColor.FromArgb(239, 65, 73)), points, XFillMode.Winding);

                // bottop diamond
                points = new XPoint[4];
                points[0].X = x + w - Width / 2;
                points[0].Y = y + h - Height - Page.VSpace - adj;
                points[1].X = x + w - Width + adj;
                points[1].Y = y + h - Height - Page.VSpace - Width / Math.Sqrt(2) - adj;
                points[2].X = x + w - Width / 2;
                points[2].Y = y + h - Height - Page.VSpace - 2 * Width / Math.Sqrt(2) - adj;
                points[3].X = x + w - adj;
                points[3].Y = y + h - Height - Page.VSpace - Width / Math.Sqrt(2) - adj;
                gfx.DrawPolygon(pen, new XSolidBrush(this.Color), points, XFillMode.Winding);
                #endregion

                #region bottom border
                // exterior rectangle
                gfx.DrawRectangle(pen,
                    x + Width + Page.VSpace,
                    y + h + Page.VSpace - Height,
                    w - 2 * Width - 2 * Page.VSpace,
                    Height);

                // center line
                gfx.DrawLine(pen,
                    x + Width + Page.VSpace,
                    y + h + Page.VSpace - Height / 2,
                    x + w - Width - Page.VSpace,
                    y + h + Page.VSpace - Height / 2);

                // coats of arms
                if (XImage.ExistsFile("Pictures\\coats.png"))
                {
                    img = XImage.FromFile("Pictures\\coats.png");
                    gfx.DrawImage(img,
                        x + w / 2 - 3 * Height * img.Size.Width / img.Size.Height / 2,
                        y + h - XUnitPt.FromMillimeter(8.5),
                        3 * Height * img.Size.Width / img.Size.Height,
                        3 * Height);
                }
                #endregion

                #region corners
                //top-left leaf
                if (XImage.ExistsFile("Pictures\\leaf_left.png"))
                {
                    img = XImage.FromFile("Pictures\\leaf_left.png");
                    gfx.DrawImage(img, x, y, Width, Height);
                }
                //top-right leaf
                if (XImage.ExistsFile("Pictures\\leaf_right.png"))
                {
                    img = XImage.FromFile("Pictures\\leaf_right.png");
                    gfx.DrawImage(img, x + w - Width, y, Width, Height);
                }
                //bottom-left leaf
                if (XImage.ExistsFile("Pictures\\leaf_left.png"))
                {
                    img = XImage.FromFile("Pictures\\leaf_left.png");
                    gfx.DrawImage(img, x, y + h - Height, Width, Height);
                }
                //bottom-right leaf
                if (XImage.ExistsFile("Pictures\\leaf_right.png"))
                {
                    img = XImage.FromFile("Pictures\\leaf_right.png");
                    gfx.DrawImage(img, x + w - Width, y + h - Height, Width, Height);
                }
                #endregion
            }
            #endregion

            #region white ace stamp border
            if (BorderType == "white_ace_stamp")
            {
                #region borders
                pen = new XPen(this.Color, LineWidth1);
                gfx.DrawRectangle(pen, x, y, w, h);

                pen = new XPen(this.Color, LineWidth2);
                gfx.DrawRectangle(pen, x + Offset, y + Offset, w - 2 * Offset, h - 2 * Offset);

                XColor shadow = XColor.FromArgb(Color.R + (256 - Color.R) / 2, Color.G + (256 - Color.G) / 2, Color.B + (256 - Color.B) / 2);
                pen = new XPen(shadow, LineWidth1 / 2);
                gfx.DrawRectangle(pen, x + Offset / 2, y + Offset / 2, w - Offset, h - Offset);
                #endregion

                Gizmo1 gizmo = new Gizmo1();
                gizmo.gfx = this.gfx;
                gizmo.Color = Color;
                gizmo.LineWidth = LineWidth1;

                #region horizontal gizmos
                if (w >= h)
                {
                    // first gizmo on the top border
                    gizmo.x = x + (w / 4) - 1.1 * Offset;
                    gizmo.y = y;
                    gizmo.w = 2.2 * Offset;
                    gizmo.h = Offset;
                    gizmo.Draw();

                    // second gizmo on the top border
                    gizmo.x = x + w - (w / 4) - 1.1 * Offset;
                    gizmo.y = y;
                    gizmo.w = 2.2 * Offset;
                    gizmo.h = Offset;
                    gizmo.Draw();

                    // first gizmo on the bottom border
                    gizmo.x = x + (w / 4) - 1.1 * Offset;
                    gizmo.y = y + h - Offset;
                    gizmo.w = 2.2 * Offset;
                    gizmo.h = Offset;
                    gizmo.Draw(reverse: true);

                    // second gizmo on the bottom border
                    gizmo.x = x + w - (w / 4) - 1.1 * Offset;
                    gizmo.y = y + h - Offset;
                    gizmo.w = 2.2 * Offset;
                    gizmo.h = Offset;
                    gizmo.Draw(reverse: true);
                }
                else
                {
                    //// one gizmo on the top border
                    gizmo.x = x + (w / 2) - 1.1 * Offset;
                    gizmo.y = y;
                    gizmo.w = 2.2 * Offset;
                    gizmo.h = Offset;
                    gizmo.Draw();

                    //// one gizmo on the bottom border
                    gizmo.x = x + w - (w / 2) - 1.1 * Offset;
                    gizmo.y = y + h - Offset;
                    gizmo.w = 2.2 * Offset;
                    gizmo.h = Offset;
                    gizmo.Draw(reverse: true);
                }
                #endregion

                #region vertical gizmos
                if (w > h)
                {
                    // one gizmo on the left border
                    gizmo.x = x;
                    gizmo.y = y + (h / 2) - 1.1 * Offset;
                    gizmo.w = Offset;
                    gizmo.h = 2.2 * Offset;
                    gizmo.Draw();

                    // one gizmo on the right border
                    gizmo.x = x + w - Offset;
                    gizmo.y = y + (h / 2) - 1.1 * Offset;
                    gizmo.w = Offset;
                    gizmo.h = 2.2 * Offset;
                    gizmo.Draw(reverse: true);
                }
                else 
                {
                    // first gizmo on the left border
                    gizmo.x = x + (w / 4) - 1.1 * Offset;
                    gizmo.x = x;
                    gizmo.y = y;
                    gizmo.y = y + (h / 4) - 1.1 * Offset;
                    gizmo.w = Offset;
                    gizmo.h = 2.2 * Offset;
                    gizmo.Draw();

                    // second gizmo on the left border
                    gizmo.x = x + w - (w / 4) - 1.1 * Offset;
                    gizmo.x = x;
                    gizmo.y = y;
                    gizmo.y = y + h - (h / 4) - 1.1 * Offset;
                    gizmo.w = Offset;
                    gizmo.h = 2.2 * Offset;
                    gizmo.Draw();

                    // first gizmo on the right border
                    gizmo.x = x + (w / 4) - 1.1 * Offset;
                    gizmo.x = x + w - Offset;
                    gizmo.y = y + h - Offset;
                    gizmo.y = y + (h / 4) - 1.1 * Offset;
                    gizmo.w = Offset;
                    gizmo.h = 2.2 * Offset;
                    gizmo.Draw(reverse: true);

                    // second gizmo on the right border
                    gizmo.x = x + w - (w / 4) - 1.1 * Offset;
                    gizmo.x = x + w - Offset;
                    gizmo.y = y + h - Offset;
                    gizmo.y = y + h - (h / 4) - 1.1 * Offset;
                    gizmo.w = Offset;
                    gizmo.h = 2.2 * Offset;
                    gizmo.Draw(reverse: true);
                }
                //if (h > w)
                //{
                //    // first gizmo on the left border
                //    x = XPos - _borderWidth1 / 2;
                //    y = YPos + (FrameHeight / 4) - _borderSpace;
                //    w = _borderWidth1 + _borderSpace + _borderWidth2;
                //    h = 2 * (_borderWidth1 + _borderSpace + _borderWidth2);
                //    DrawGizmo1(x, y, w, h);

                //    // second gizmo on the left border
                //    x = XPos - _borderWidth1 / 2;
                //    y = YPos + 3 * (FrameHeight / 4) - _borderSpace;
                //    w = _borderWidth1 + _borderSpace + _borderWidth2;
                //    h = 2 * (_borderWidth1 + _borderSpace + _borderWidth2);
                //    DrawGizmo1(x, y, w, h);

                //    // first gizmo on the right border
                //    x = XPos + FrameWidth - (_borderWidth1 + _borderSpace) - _borderWidth1 / 2;
                //    y = YPos + (FrameHeight / 4) - _borderSpace;
                //    //y = YPos + FrameHeight - (_borderWidth1 + _borderSpace) - _borderWidth1 / 2;
                //    w = _borderWidth1 + _borderSpace + _borderWidth2;
                //    h = 2 * (_borderWidth1 + _borderSpace + _borderWidth2);
                //    DrawGizmo1(x, y, w, h);

                //    // second gizmo on the right border
                //    x = XPos + FrameWidth - (_borderWidth1 + _borderSpace) - _borderWidth1 / 2;
                //    y = YPos + 3 * (FrameHeight / 4) - _borderSpace;
                //    //y = YPos + FrameHeight - (_borderWidth1 + _borderSpace) - _borderWidth1 / 2;
                //    w = _borderWidth1 + _borderSpace + _borderWidth2;
                //    h = 2 * (_borderWidth1 + _borderSpace + _borderWidth2);
                //    DrawGizmo1(x, y, w, h);
                //}
                //else
                //{
                //    // one gizmo on the left border
                //    x = XPos - _borderWidth1 / 2;
                //    y = YPos + (FrameHeight / 2) - _borderSpace;
                //    w = _borderWidth1 + _borderSpace + _borderWidth2;
                //    h = 2 * (_borderWidth1 + _borderSpace + _borderWidth2);
                //    DrawGizmo1(x, y, w, h);

                //    // one gizmo on the right border
                //    x = XPos + FrameWidth - (_borderWidth1 + _borderSpace) - _borderWidth1 / 2;
                //    y = YPos + (FrameHeight / 2) - _borderSpace;
                //    w = _borderWidth1 + _borderSpace + _borderWidth2;
                //    h = 2 * (_borderWidth1 + _borderSpace + _borderWidth2);
                //    DrawGizmo1(x, y, w, h);
                //}
                #endregion

            }
            #endregion
            */
            #endregion
        }
        #endregion


    }
}
