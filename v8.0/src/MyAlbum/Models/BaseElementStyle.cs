using MyAlbum.Utilities;
using PdfSharp.Drawing;
using System.Xml.Linq;

namespace MyAlbum.Models
{
    internal class BaseElementStyle
    {
        #region Properties
        public bool IsDefalut { get; set; } = false;
        public XColor? Color { get; set; }
        public XColor? BgColor { get; set; }
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
        public bool? Rotate { get; set; }
        #endregion

        #region Methods
        internal virtual void ParseXml(XElement element)
        {
            IsDefalut = false;
            Color = XmlParser.ParseColor(element.Attribute("color")?.Value);
            BgColor = XmlParser.ParseColor(element.Attribute("bgcolor")?.Value);
            (MarginTop, MarginRight, MarginBottom, MarginLeft) = XmlParser.ParseMargin(element.Attribute("margin")?.Value);
            (PaddingTop, PaddingRight, PaddingBottom, PaddingLeft) = XmlParser.ParseMargin(element.Attribute("padding")?.Value);
            Height = element.Attribute("height")?.Value;
            Width = element.Attribute("width")?.Value;
            Rotate = XmlParser.ParseBool(element.Attribute("rotate")?.Value);
            Name = element.Attribute("style")?.Value;
        }
        #endregion
    }
}
