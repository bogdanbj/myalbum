using MyAlbum.Utilities;
using PdfSharpCore.Drawing;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace MyAlbum.Models
{
    internal class TextStyle : BaseElementStyle
    {
        public Alignment? Align { get; set; }
        public string? FontName { get; set; }
        public double? FontSize { get; set; }
        public XFontStyle? FontStyle { get; set; }
        internal override void ParseXml(XElement xText)
        {
            base.ParseXml(xText);

            Align = XmlParser.ParseAlignment(xText.Attribute("align")?.Value);
            FontName = xText.Attribute("font-name")?.Value;
            FontSize = XmlParser.ParseDouble(xText.Attribute("font-size")?.Value);
            FontStyle = XmlParser.ParseFontStyle(xText.Attribute("font-style")?.Value);
        }
    }
}
