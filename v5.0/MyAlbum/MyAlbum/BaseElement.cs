using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Xml.Linq;
using PdfSharp.Drawing;
using PdfSharp.Pdf;

namespace MyAlbum
{
    abstract class BaseElement: IDrawable
    {
        #region Properties
        public XElement Xml { get; set; }

        public XGraphics gfx { get; set; }
        public XUnitPt x { get; set; }
        public XUnitPt y { get; set; }
        public XUnitPt w { get; set; }
        public XUnitPt h { get; set; }
        //public double x { get; set; }
        //public double y { get; set; }
        //public double w { get; set; }
        //public double h { get; set; }
        //public XUnitPt x { get; set; }
        //public XUnitPt y { get; set; }
        //public XUnitPt w { get; set; }
        //public XUnitPt h { get; set; }
        public double WidthPercent { get; set; }
        public bool IsDefault { get; set; }
        public string Style { get; set; }
        public XUnitPt MarginLeft { get; set; }
        public XUnitPt MarginRight { get; set; }
        public XUnitPt MarginTop { get; set; }
        public XUnitPt MarginBottom { get; set; }
        public XColor Color { get; set; }
        public XColor BoxColor { get; set; }
        public Styles.Alignments Align { get; set; }
        public Styles.VerticalAlignments VAlign { get; set; }
        public bool Absolute { get; set; }
        public XColor BackgroundColor { get; set; }
        


        public BaseElement Parent { get; set; }

        public XUnitPt TopAlign { get; set; }
        public XUnitPt MiddleAlign { get; set; }
        public XUnitPt BottomAlign { get; set; }
        #endregion

        #region Public Abstract Methods
        public abstract void Parse();
        public abstract void Calculate();
        public abstract void Draw();
        #endregion

        #region Public Methods
        public void ParseBaseAttribute()
        {
            // style attribute
            ParseStyleAttribute();

            // default attribute
            ParseDefaultAttribute();

            // x attribute
            ParseXAttribute();

            // y attribute
            ParseYAttribute();
            
            // width attribute
            ParseWidthAttribute();

            // height attribute
            ParseHeightAttribute();

            // align attribute
            ParseAlignAttribute();

            // valign attribute
            ParseVAlignAttribute();

            // margin attribute
            ParseMarginAttribute();

            // color attribute
            ParseColorAttribute();

            // absolute attribute
            ParseAbsoluteAttribute();

            // background color attribute
            ParseBackgroundColorAttribute();
        }
        public void ParseStyleAttribute()
        {
            if (Xml.Attribute("style") != null)
            {
                this.Style = Xml.Attribute("style").Value.ToLower();
            }
        }
        public void ParseDefaultAttribute()
        {
            if (Xml.Attribute("default") != null)
            {
                if (Xml.Attribute("default").Value.ToLower() == "true")
                {
                    this.IsDefault = true;
                }
            }
        }
        public void ParseXAttribute()
        {
            // X attribute
            this.x = XUnitPt.Zero;
            if (Xml.Attribute("x") != null)
            {
                try
                {
                    x = XUnitPt.FromMillimeter(double.Parse(Xml.Attribute("x").Value));
                }
                catch (Exception)
                { }
            }
        }
        public void ParseYAttribute()
        {
            // Y attribute
            this.y = XUnitPt.Zero;
            if (Xml.Attribute("y") != null)
            {
                try
                {
                    y = XUnitPt.FromMillimeter(double.Parse(Xml.Attribute("y").Value));
                }
                catch (Exception)
                { }
            }
        }
        public void ParseWidthAttribute()
        {
            // width attribute
            WidthPercent = 100;
            if (Xml.Attribute("width") != null)
            {
                try
                {
                    string width = Xml.Attribute("width").Value;
                    if (width.Contains('%'))
                    {
                        WidthPercent = double.Parse(width.TrimEnd(new char[] { '%', ' ' }));
                    }
                    else
                    {
                        w = XUnitPt.FromMillimeter(double.Parse(width));
                    }
                }
                catch (Exception)
                { }
            }
        }
        public void ParseHeightAttribute()
        {
            //height attribute
            this.h = XUnitPt.Zero;
            if (Xml.Attribute("height") != null)
            {
                try
                {
                    this.h = XUnitPt.FromMillimeter(double.Parse(Xml.Attribute("height").Value));
                }
                catch (Exception)
                {
                }
            }
        }
        public void ParseAlignAttribute()
        {
            if (Xml.Attribute("align") != null)
            {
                switch (Xml.Attribute("align").Value.ToLower())
                {
                    case "left":
                        this.Align = Styles.Alignments.Left;
                        break;
                    case "center":
                        this.Align = Styles.Alignments.Center;
                        break;
                    case "right":
                        this.Align = Styles.Alignments.Right;
                        break;
                    default:
                        this.Align = Styles.Alignments.Left;
                        break;
                }
            }
        }
        public void ParseVAlignAttribute()
        {
            if (Xml.Attribute("valign") != null)
            {
                switch (Xml.Attribute("valign").Value.ToLower())
                {
                    case "top":
                        this.VAlign = Styles.VerticalAlignments.Top;
                        break;
                    case "center":
                        this.VAlign = Styles.VerticalAlignments.Center;
                        break;
                    case "bottom":
                        this.VAlign = Styles.VerticalAlignments.Bottom;
                        break;
                    default:
                        this.VAlign = Styles.VerticalAlignments.Top;
                        break;
                }
            }
        }
        public void ParseMarginAttribute()
        {
            if (Xml.Attribute("margin") != null)
            {
                try
                {
                    string[] arr = Xml.Attribute("margin").Value.Split(',');

                    switch (arr.Length)
                    {
                        case 1:
                            MarginLeft = MarginRight = MarginTop = MarginBottom = XUnitPt.FromMillimeter(double.Parse(arr[0]));
                            break;
                        case 2:
                            MarginTop = MarginBottom = XUnitPt.FromMillimeter(double.Parse(arr[0]));
                            MarginLeft = MarginRight = XUnitPt.FromMillimeter(double.Parse(arr[1]));
                            break;
                        case 4:
                            MarginTop = XUnitPt.FromMillimeter(double.Parse(arr[0]));
                            MarginRight = XUnitPt.FromMillimeter(double.Parse(arr[1]));
                            MarginBottom = XUnitPt.FromMillimeter(double.Parse(arr[2]));
                            MarginLeft = XUnitPt.FromMillimeter(double.Parse(arr[3]));
                            break;
                        default:
                            MarginLeft = MarginRight = MarginTop = MarginBottom = XUnitPt.Zero;
                            break;
                    }
                }
                catch (Exception)
                {
                    this.MarginLeft = this.MarginRight = this.MarginTop = MarginBottom = XUnitPt.Zero;
                }
            }
        }
        public void ParseColorAttribute()
        {
            if (Xml.Attribute("color") != null)
            {
                string[] rgb = Xml.Attribute("color").Value.Split(',');
                try
                {
                    this.Color = XColor.FromArgb(int.Parse(rgb[0]), int.Parse(rgb[1]), int.Parse(rgb[2]));
                }
                catch (Exception)
                {
                    this.Color = XColors.Black;
                }
            }

        }
        public void ParseAbsoluteAttribute()
        {
            if (Xml.Attribute("absolute") != null)
            {
                if (Xml.Attribute("absolute").Value.ToLower() == "true")
                {
                    this.Absolute = true;
                }
            }
        }
        public void ParseBackgroundColorAttribute()
        {
            this.BackgroundColor = XColors.Transparent;
            if (Xml.Attribute("bgcolor") != null)
            {
                string[] rgb = Xml.Attribute("bgcolor").Value.Split(',');
                try
                {
                    this.BackgroundColor = XColor.FromArgb(int.Parse(rgb[0]), int.Parse(rgb[1]), int.Parse(rgb[2]));
                }
                catch (Exception)
                {
                    this.BackgroundColor = XColors.LightPink;
                }
            }

        }

        public XUnitPt GetHeight(BaseElement obj)
        {
            if (obj.h != XUnitPt.Zero)
            {
                return obj.h;
            }
            else
            {
                return GetHeight(obj.Parent) - (obj.MarginTop + obj.MarginBottom);
            }
        }

        public XUnitPt GetWidth(BaseElement obj)
        {
            if (obj.w != XUnitPt.Zero)
            {
                return obj.w;
            }
            else
            {
                return GetWidth(obj.Parent) - (obj.MarginLeft + obj.MarginRight);
            }
        }

        public Page GetPage()
        {
            BaseElement elem = this;

            while (elem.GetType() != typeof(Page))
            {
                elem = elem.Parent;
            }

            return (Page)elem;
        }
        //public PdfPage GetPage()
        //{
        //    BaseElement elem = this;

        //    while (elem.GetType() != typeof(Page))
        //    {
        //        elem = elem.Parent;
        //    }

        //    return ((Page)elem).PdfPage;
        //}

        #endregion

        #region Helper Methods
        public void DrawBox()
        {
            if (BoxColor.IsEmpty)
            {
                DrawBox(Color);
            }
            else
            {
                DrawBox(BoxColor);
            }
        }
        public void DrawBox(XColor color)
        {
            if (h >= 0)
                DrawBox(color, new XPoint(x, y), new XSize(w, h));
            else
                DrawBox(color, new XPoint(x, y+h), new XSize(w, -h));
        }
        public void DrawBox(XPoint point, XSize size)
        {
            if (BoxColor.IsEmpty)
            {
                DrawBox(Color, point, size);
            }
            else
            {
                DrawBox(BoxColor, point, size);
            }
        }
        public void DrawBox(XColor color, XPoint point, XSize size)
        {
            gfx.DrawRectangle(new XPen(color, 0.1), point.X, point.Y, size.Width, size.Height);
        }
        public void DrawCross(XPoint point)
        {
            DrawCross(point, Color);
        }
        public void DrawCross(XPoint point, XColor color)
        {
            XUnitPt d = XUnitPt.FromMillimeter(7);
            gfx.DrawLine(new XPen(color, 0.1), point.X - d, point.Y, point.X + d, point.Y);
            gfx.DrawLine(new XPen(color, 0.1), point.X, point.Y - d, point.X, point.Y + d);
        }
        public void DrawBackground()
        {
            XBrush brush;
            if (!BackgroundColor.IsEmpty)
            {
                brush = new XSolidBrush(BackgroundColor);
                if (h >= 0)
                    gfx.DrawRectangle(brush, x, y, w, h);
                else
                    gfx.DrawRectangle(brush, x, y + h, w, -h);
            }
        }
        #endregion

    }
}
