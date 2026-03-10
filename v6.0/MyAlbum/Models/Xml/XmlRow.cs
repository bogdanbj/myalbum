using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MyAlbum.Models.Xml
{
    public class XmlRow : XmlElement
    {
        [XmlAttribute("height")]
        public double Height { get; set; }

        [XmlAttribute("spacing-mode")]
        public string SpacingMode { get; set; }

        [XmlAttribute("space")]
        public string Space { get; set; }

        [XmlAttribute("bgcolor")]
        public string BgColor { get; set; }

        [XmlAttribute("rotate")]
        public bool Rotate { get; set; }

        [XmlElement("column", typeof(XmlColumn))]
        [XmlElement("text", typeof(XmlText))]
        [XmlElement("stamp", typeof(XmlStamp))]
        [XmlElement("image", typeof(XmlImage))]
        //[XmlElement("border", typeof(XmlBorder))]
        public List<XmlElement> Elements { get; set; }
    }
}
