using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MyAlbum.Models.Xml
{
    public class XmlColumn : XmlElement
    {
        //[XmlAttribute("style")]
        //public string Style { get; set; }
        [XmlAttribute("width")]
        public string Width { get; set; }
        [XmlAttribute("align")]
        public string Align { get; set; }
        [XmlAttribute("space")]
        public string Space { get; set; }
        [XmlElement("row", typeof(XmlRow))]
        [XmlElement("text", typeof(XmlText))]
        [XmlElement("stamp", typeof(XmlStamp))]
        [XmlElement("image", typeof(XmlImage))]
        //[XmlElement("border", typeof(XmlBorder))]
        public List<XmlElement> Elements { get; set; }
    }
}
