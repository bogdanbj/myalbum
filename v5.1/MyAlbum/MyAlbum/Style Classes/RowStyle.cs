using PdfSharp.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MyAlbum
{
    internal class RowStyle : StyleElement
    {
        #region Properties
        public Styles.SpacingModes Spacing { get; set; }
        public XUnitPt Space { get; set; }
        public string Rotate { get; set; }
        #endregion

        #region constructors
        public RowStyle()
        {
            Rotate = "false";
        }
        public RowStyle(XElement xml) : this()
        {
            this.xml = xml;
        }
        #endregion

        #region public methods
        public override void Parse()
        {
            // common style attributes
            ParseBaseAttributes();

            // spacing-mode attribute
            ParseSpacingModeAttribute();

            // space attribute
            ParseSpaceAttribute();

            // space attribute
            ParseRotateAttribute();
        }
        #endregion

        #region private methods
        private void ParseSpacingModeAttribute()
        {
            if (xml.Attribute("spacing-mode") != null)
            {
                switch (xml.Attribute("spacing-mode")!.Value.ToLower())
                {
                    case "fs":
                        this.Spacing = Styles.SpacingModes.FS;
                        break;
                    case "es":
                        this.Spacing = Styles.SpacingModes.ES;
                        break;
                    case "js":
                        this.Spacing = Styles.SpacingModes.JS;
                        break;
                    default:
                        this.Spacing = Styles.SpacingModes.ES;
                        break;
                }
            }
        }
        private void ParseSpaceAttribute()
        {
            if (xml.Attribute("space") != null)
            {
                try
                {
                    this.Space = XUnitPt.FromMillimeter(double.Parse(xml.Attribute("space")!.Value));
                }
                catch (Exception)
                {
                    this.Space = XUnitPt.Zero;
                }
            }
        }
        private void ParseRotateAttribute()
        {
            if (xml.Attribute("rotate") != null)
            {
                this.Rotate = xml.Attribute("rotate")!.Value;
            }
            else
            {
                this.Rotate = "false";
            }
        }

        #endregion
    }

}
