using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MyAlbum.Models.Xml
{
    public class XmlText : XmlElement
    {
        //[XmlAttribute("style")]
        //public string Style { get; set; }
        //[XmlAttribute("align")]
        //public string Align { get; set; }
        [XmlAttribute("justify")]
        public string Justify { get; set; }
        [XmlAttribute("font_name")]
        public string FontName { get; set; }
        [XmlAttribute("font_size")]
        public double FontSize { get; set; }
        [XmlAttribute("font_style")]
        public string FontStyle { get; set; }
        [XmlAttribute("width")]
        public string Width { get; set; }
        //[XmlAttribute("margin")]
        //public string Margin { get; set; }
        [XmlText]
        public string? Value { get; set; }
    }
}
