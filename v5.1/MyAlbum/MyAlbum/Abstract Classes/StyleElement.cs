using PdfSharp.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MyAlbum
{
    internal class StyleElement : BaseElement
    {
        #region properties
        public bool IsDefault { get; set; }
        public string? StyleName { get; set; }
        public XBrush Brush { get; set; }
        public XColor Color { get; set; }
        public XUnitPt MarginLeft { get; set; }
        public XUnitPt MarginRight { get; set; }
        public XUnitPt MarginTop { get; set; }
        public XUnitPt MarginBottom { get; set; }
        public XUnitPt PaddingLeft { get; set; }
        public XUnitPt PaddingRight { get; set; }
        public XUnitPt PaddingTop { get; set; }
        public XUnitPt PaddingBottom { get; set; }
        #endregion

        #region constructors
        protected StyleElement() 
        {
            Brush = XBrushes.White;
            Color = XColors.Black;
        }
        #endregion

        #region private methods
        internal void ParseBaseAttributes()
        {
            // default flag
            ParseDefaultAttribute();
            
            // style name
            ParseStyleNameAttribute();

            // margin
            ParseMarginAttribute();

            // padding
            ParsePaddingAttribute();

            // color
            ParseColorAttribute();

            // background color 
            ParseBackgroundColorAttribute();
        }
        private void ParseDefaultAttribute()
        {
            this.IsDefault = false;
            
            if (xml.Attribute("default") != null)
            {
                if (xml.Attribute("default").Value.ToLower() == "true")
                {
                    this.IsDefault = true;
                }
            }
        }
        private void ParseStyleNameAttribute()
        {
            if (xml.Attribute("style") != null)
            {
                this.StyleName = xml.Attribute("style")?.Value.ToLower() ?? throw new InvalidOperationException("The 'style' attribute of a style is missing or empty.");
            }
        }
        internal void ParseMarginAttribute()
        {
            if (xml.Attribute("margin") != null)
            {
                try
                {
                    string[] arr = (xml.Attribute("margin")?.Value ?? "0").Split(',');

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
                    MarginLeft = MarginRight = MarginTop = MarginBottom = XUnitPt.Zero;
                }
            }
        }
        internal void ParsePaddingAttribute()
        {
            if (xml.Attribute("padding") != null)
            {
                try
                {
                    string[] arr = (xml.Attribute("padding")?.Value ?? "0").Split(',');

                    switch (arr.Length)
                    {
                        case 1:
                            PaddingLeft = PaddingRight = PaddingTop = PaddingBottom = XUnitPt.FromMillimeter(int.Parse(arr[0]));
                            break;
                        case 2:
                            PaddingTop = PaddingBottom = XUnitPt.FromMillimeter(int.Parse(arr[0]));
                            PaddingLeft = PaddingRight = XUnitPt.FromMillimeter(int.Parse(arr[1]));
                            break;
                        case 4:
                            PaddingTop = XUnitPt.FromMillimeter(int.Parse(arr[0]));
                            PaddingLeft = XUnitPt.FromMillimeter(int.Parse(arr[1]));
                            PaddingBottom = XUnitPt.FromMillimeter(int.Parse(arr[2]));
                            PaddingRight = XUnitPt.FromMillimeter(int.Parse(arr[3]));
                            break;
                        default:
                            PaddingLeft = PaddingRight = PaddingTop = PaddingBottom = XUnitPt.Zero;
                            break;
                    }
                }
                catch (Exception)
                {
                    PaddingLeft = PaddingRight = PaddingTop = PaddingBottom = XUnitPt.Zero;
                }
            }
        }
        internal void ParseColorAttribute()
        {
            if (xml.Attribute("color") != null)
            {
                string[] rgb = (xml.Attribute("color")?.Value ?? "0,0,0").Split(',');
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
        internal void ParseBackgroundColorAttribute()
        {
            if (xml.Attribute("bgcolor") != null)
            {
                string[] rgb = (xml.Attribute("bgcolor")?.Value ?? "255,255,255").Split(',');
                try
                {
                    Brush = new XSolidBrush(XColor.FromArgb(int.Parse(rgb[0]), int.Parse(rgb[1]), int.Parse(rgb[2])));
                }
                catch (Exception)
                {
                    Brush = XBrushes.White;
                }
            }
        }
        #endregion
    }
}
