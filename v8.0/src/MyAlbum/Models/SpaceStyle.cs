using MyAlbum.Utilities;
using PdfSharp.Drawing;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace MyAlbum.Models
{
    internal class SpaceStyle : BaseElementStyle
    {
        #region Properties
        public bool? Rotate { get; set; }
        #endregion

        // Space is a generic spacer. It relies entirely on the inherited
        // BaseElementStyle properties (width, height, margin, padding, color,
        // bgcolor) plus the optional rotate flag.
        internal override void ParseXml(XElement xSpace)
        {
            base.ParseXml(xSpace);

            Rotate = XmlParser.ParseBool(xSpace.Attribute("rotate")?.Value);
        }
    }
}
