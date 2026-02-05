using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MyAlbum.Models.Xml
{
    public class XmlColumn : XmlBaseElement
    {
        //[XmlAttribute("style")]
        //public string Style { get; set; }
        [XmlAttribute("width")]
        public double Width { get; set; }
        [XmlAttribute("align")]
        public string Align { get; set; }
        [XmlAttribute("space")]
        public string Space { get; set; }
        [XmlElement("text")]
        public List<XmlText> Texts { get; set; }
        [XmlElement("stamp")]
        public List<XmlStamp> Stamps { get; set; }
        [XmlElement("row")]
        public List<XmlRow> Rows { get; set; }
    }
}
