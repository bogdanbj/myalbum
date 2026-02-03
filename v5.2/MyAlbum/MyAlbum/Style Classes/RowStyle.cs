using PdfSharp.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MyAlbum
{
    internal class RowStyle : StyleElement
    {
        #region Properties
        public static RowStyle Default
        { 
            get
            {
                var defaultRowStyle = DefaultStyleElement() as RowStyle;
                if (defaultRowStyle != null)
                {
                    defaultRowStyle.SpacingMode = Styles.SpacingModes.ES;
                    defaultRowStyle.Space = XUnitPt.Zero;
                    defaultRowStyle.Rotate = false;
                }
                return defaultRowStyle;
            }
        }
        public Styles.SpacingModes SpacingMode { get; set; }
        public XUnitPt Space { get; set; }
        public bool Rotate { get; set; }
        #endregion

        #region constructors
        public RowStyle()
        { }
        public RowStyle(XElement xml) : this()
        {
            this.xml = xml;
        }
        #endregion

        #region public methods
        public override void Parse()
        {
            // common style attributes
            ParseBaseStyleAttributes();

            // spacing-mode attribute
            SpacingMode = ParseSpacingModeAttribute(xml);

            // space attribute
            Space = ParseSpaceAttribute(xml);

            // rotate attribute
            Rotate = ParseRotateAttribute(xml);

            // width attribute
            Width = ParseWidthAttribute(xml);

            // height attribute
            Height = ParseHeightAttribute(xml);
        }
        #endregion
    }
}
