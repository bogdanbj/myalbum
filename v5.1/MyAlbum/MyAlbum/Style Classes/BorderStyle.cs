using PdfSharp.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MyAlbum
{
    internal class BorderStyle : StyleElement
    {
        #region Properties
        public XUnitPt LineWidth1 { get; set; }
        public XUnitPt LineWidth2 { get; set; }
        public XUnitPt Offset { get; set; }
        public string? TypeLeft { get; set; }
        public string? TypeRight { get; set; }
        public string? TypeTop { get; set; }
        public string? TypeBottom { get; set; }
        //public XUnitPt WidthLeft { get; set; }
        //public XUnitPt WidthRight { get; set; }
        //public XUnitPt WidthTop { get; set; }
        //public XUnitPt WidthBottom { get; set; }
        #endregion

        #region constructors
        public BorderStyle()
        {
            this.Brush = XBrushes.White;
        }
        public BorderStyle(XElement xml) : this()
        {
            this.xml = xml;
        }
        #endregion

        #region public methods
        public override void Parse()
        {
            // common style attributes
            ParseBaseAttributes();

            // border type
            ParseBorderTypeAttribute();

            // lines and offset width
            ParseWidthAttribute();
        }
        #endregion region

        #region private methods
        private void ParseBorderTypeAttribute()
        {
            if (xml.Attribute("border_type") != null)
            {
                try
                {
                    string[] arr = (xml.Attribute("border_type")?.Value.ToLower() ?? "none").Split(',');

                    switch (arr.Length)
                    {
                        case 1:
                            TypeLeft = TypeRight = TypeTop = TypeBottom = arr[0].Trim();
                            break;
                        case 2:
                            TypeTop = TypeBottom = arr[0].Trim();
                            TypeLeft = TypeRight = arr[1].Trim();
                            break;
                        case 4:
                            TypeTop = arr[0].Trim();
                            TypeLeft = arr[1].Trim();
                            TypeBottom = arr[2].Trim();
                            TypeRight = arr[3].Trim();
                            break;
                        default:
                            TypeLeft = TypeRight = TypeTop = TypeBottom = "none";
                            break;
                    }
                }
                catch (Exception)
                {
                    TypeLeft = TypeRight = TypeTop = TypeBottom = "none";
                }
            }
        }
        private void ParseWidthAttribute()
        {
            if (xml.Attribute("width") != null)
            {
                try
                {
                    string[] arr = (xml.Attribute("width")?.Value ?? "0").Split(',');

                    switch (arr.Length)
                    {
                        case 1:
                            LineWidth1 = LineWidth2 = Offset = XUnitPt.FromMillimeter(double.Parse(arr[0]));
                            break;
                        case 2:
                            LineWidth1 = LineWidth2 = XUnitPt.FromMillimeter(double.Parse(arr[0]));
                            Offset = XUnitPt.FromMillimeter(double.Parse(arr[1]));
                            break;
                        case 3:
                            LineWidth1 = XUnitPt.FromMillimeter(double.Parse(arr[0]));
                            Offset = XUnitPt.FromMillimeter(double.Parse(arr[1]));
                            LineWidth2 = XUnitPt.FromMillimeter(double.Parse(arr[2]));
                            break;
                        default:
                            LineWidth1 = LineWidth2 = Offset = XUnitPt.Zero;
                            break;
                    }
                }
                catch (Exception)
                {
                    LineWidth1 = LineWidth2 = Offset = XUnitPt.Zero;
                }
            }
        }

        #endregion
    }
}
