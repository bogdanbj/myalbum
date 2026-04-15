using MyAlbum.Utilities;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Xml.Linq;
using PdfSharpCore.Drawing;

namespace MyAlbum.Models
{
    internal class RowStyle : BaseElementStyle
    {
        #region Properties
        public Alignment? Align { get; set; }
        public VerticalAlignment? VAlign { get; set; }
        public SpacingMode? SpacingMode { get; set; }
        public XUnit? Space { get; set; }
        public bool? Rotate { get; set; }
        #endregion

        internal override void ParseXml(XElement xRow)
        {
            base.ParseXml(xRow);

            Align = XmlParser.ParseAlignment(xRow.Attribute("align")?.Value);
            VAlign = XmlParser.ParseVerticalAlignment(xRow.Attribute("valign")?.Value);
            SpacingMode = XmlParser.ParseSpacingMode(xRow.Attribute("spacing-mode")?.Value);
            Space = XmlParser.ParseXUnit(xRow.Attribute("space")?.Value);
            Rotate = XmlParser.ParseBool(xRow.Attribute("rotate")?.Value);
        }
    }
}
