using MyAlbum.Utilities;
using PdfSharpCore.Drawing;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace MyAlbum.Models
{
    internal class BaseElementStyle
    {
        #region Properties
        public bool IsDefalut { get; set; } = false;
        public XColor? Color { get; set; }
        public XColor? BgColor { get; set; }
        //public double? Rotate { get; set; }
        //public string? Margin { get; set; }
        //public string? Padding { get; set; }
        public XUnit? MarginTop { get; set; }
        public XUnit? MarginRight { get; set; }
        public XUnit? MarginBottom { get; set; }
        public XUnit? MarginLeft { get; set; }
        public XUnit? PaddingTop { get; set; }
        public XUnit? PaddingRight { get; set; }
        public XUnit? PaddingBottom { get; set; }
        public XUnit? PaddingLeft { get; set; }
        public string? Height { get; set; }
        public string? Width { get; set; }
        public string? Name { get; set; }
        #endregion Region

        #region Methods
        internal virtual void ParseXml(XElement element)
        {
            IsDefalut = false;
            Color = XmlParser.ParseColor(element.Attribute("color")?.Value);
            BgColor = XmlParser.ParseColor(element.Attribute("bgcolor")?.Value);
            //Margin = element.Attribute("margin")?.Value;
            //Padding = element.Attribute("padding")?.Value;
            (MarginTop, MarginRight, MarginBottom, MarginLeft) = XmlParser.ParseMargin(element.Attribute("margin")?.Value);
            (PaddingTop, PaddingRight, PaddingBottom, PaddingLeft) = XmlParser.ParseMargin(element.Attribute("padding")?.Value);
            Height = element.Attribute("height")?.Value;
            Width = element.Attribute("width")?.Value;
            Name = element.Attribute("style")?.Value;
        }
        #endregion
    }
}
