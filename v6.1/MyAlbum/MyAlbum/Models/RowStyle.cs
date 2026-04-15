using MyAlbum.Utilities;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Xml.Linq;

namespace MyAlbum.Models
{
    internal class RowStyle : BaseElementStyle
    {
        #region Properties
        public string? Align { get; set; }
        public string? VAlign { get; set; }
        public string? SpacingMode { get; set; }
        public string? Space { get; set; }
        public bool? Rotate { get; set; }
        #endregion

        internal override void ParseXml(XElement xRow)
        {
            base.ParseXml(xRow);

            Align = xRow.Attribute("align")?.Value;
            VAlign = xRow.Attribute("valign")?.Value;
            SpacingMode = xRow.Attribute("spacing-mode")?.Value;
            Space = xRow.Attribute("space")?.Value;
            Rotate = XmlParser.ParseBool(xRow.Attribute("rotate")?.Value);
        }
    }
}
