using PdfSharp;
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
        public static BorderStyle Default
        {
            get
            {
                var defaultBorderStyle = DefaultStyleElement() as BorderStyle;
                if (defaultBorderStyle != null)
                {
                    defaultBorderStyle.LineWidth1 = XUnitPt.Zero;
                    defaultBorderStyle.LineWidth2 = XUnitPt.Zero;
                    defaultBorderStyle.Offset = XUnitPt.Zero;
                    defaultBorderStyle.TypeLeft = Styles.BorderTypes.None;
                    defaultBorderStyle.TypeRight = Styles.BorderTypes.None;
                    defaultBorderStyle.TypeTop = Styles.BorderTypes.None;
                    defaultBorderStyle.TypeBottom = Styles.BorderTypes.None;
                }
                return defaultBorderStyle;
            }
        }
        public XUnitPt LineWidth1 { get; set; }
        public XUnitPt LineWidth2 { get; set; }
        public XUnitPt Offset { get; set; }
        public Styles.BorderTypes TypeLeft { get; set; }
        public Styles.BorderTypes TypeRight { get; set; }
        public Styles.BorderTypes TypeTop { get; set; }
        public Styles.BorderTypes TypeBottom { get; set; }
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
            ParseBaseStyleAttributes();

            // border type
            Styles.BorderTypes[] types = ParseBorderTypeAttribute(xml);
            TypeTop = types[0];
            TypeRight = types[1];
            TypeBottom = types[2];
            TypeLeft = types[3];

            // lines and offset width
            XUnitPt[] widths = ParseBorderLineWidthAttribute(xml);
            LineWidth1 = widths[0];
            Offset = widths[1];
            LineWidth2 = widths[2];

        }
        #endregion region

    }
}
