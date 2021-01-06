using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Xml.Linq;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;

namespace MyAlbum
{
    abstract class BaseElement: IDrawable
    {
        #region Properties
        public XElement Xml { get; set; }

        public XGraphics gfx { get; set; }
        public XUnit x { get; set; }
        public XUnit y { get; set; }
        public XUnit w { get; set; }
        public XUnit h { get; set; }
        public double WidthPercent { get; set; }
        public bool IsDefault { get; set; }
        public string Style { get; set; }
        public XUnit MarginLeft { get; set; }
        public XUnit MarginRight { get; set; }
        public XUnit MarginTop { get; set; }
        public XUnit MarginBottom { get; set; }
        public XColor Color { get; set; }
        public XColor BoxColor { get; set; }
        public Styles.Alignments Align { get; set; }
        public Styles.VerticalAlignments VAlign { get; set; }
        public bool Absolute { get; set; }
        public XColor BackgroundColor { get; set; }
        


        public BaseElement Parent { get; set; }

        public XUnit TopAlign { get; set; }
        public XUnit MiddleAlign { get; set; }
        public XUnit BottomAlign { get; set; }
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
            this.x = XUnit.Zero;
            if (Xml.Attribute("x") != null)
            {
                try
                {
                    x = XUnit.FromMillimeter(double.Parse(Xml.Attribute("x").Value));
                }
                catch (Exception)
                { }
            }
        }
        public void ParseYAttribute()
        {
            // Y attribute
            this.y = XUnit.Zero;
            if (Xml.Attribute("y") != null)
            {
                try
                {
                    y = XUnit.FromMillimeter(double.Parse(Xml.Attribute("y").Value));
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
                        w = XUnit.FromMillimeter(double.Parse(width));
                    }
                }
                catch (Exception)
                { }
            }
        }
        public void ParseHeightAttribute()
        {
            //height attribute
            this.h = XUnit.Zero;
            if (Xml.Attribute("height") != null)
            {
                try
                {
                    this.h = XUnit.FromMillimeter(double.Parse(Xml.Attribute("height").Value));
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
                            MarginLeft = MarginRight = MarginTop = MarginBottom = XUnit.FromMillimeter(double.Parse(arr[0]));
                            break;
                        case 2:
                            MarginTop = MarginBottom = XUnit.FromMillimeter(double.Parse(arr[0]));
                            MarginLeft = MarginRight = XUnit.FromMillimeter(double.Parse(arr[1]));
                            break;
                        case 4:
                            MarginTop = XUnit.FromMillimeter(double.Parse(arr[0]));
                            MarginRight = XUnit.FromMillimeter(double.Parse(arr[1]));
                            MarginBottom = XUnit.FromMillimeter(double.Parse(arr[2]));
                            MarginLeft = XUnit.FromMillimeter(double.Parse(arr[3]));
                            break;
                        default:
                            MarginLeft = MarginRight = MarginTop = MarginBottom = XUnit.Zero;
                            break;
                    }
                }
                catch (Exception)
                {
                    this.MarginLeft = this.MarginRight = this.MarginTop = MarginBottom = XUnit.Zero;
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

        public XUnit GetHeight(BaseElement obj)
        {
            if (obj.h != XUnit.Zero)
            {
                return obj.h;
            }
            else
            {
                return GetHeight(obj.Parent) - (obj.MarginTop + obj.MarginBottom);
            }
        }

        public XUnit GetWidth(BaseElement obj)
        {
            if (obj.w != XUnit.Zero)
            {
                return obj.w;
            }
            else
            {
                return GetWidth(obj.Parent) - (obj.MarginLeft + obj.MarginRight);
            }
        }

        public PdfPage GetPage()
        {
            BaseElement elem = this;

            while (elem.GetType() != typeof(Page))
	        {
	            elem = elem.Parent;
	        }

            return ((Page)elem).PdfPage;
        }

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
            XUnit d = XUnit.FromMillimeter(7);
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
