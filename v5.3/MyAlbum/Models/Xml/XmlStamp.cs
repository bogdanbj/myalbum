using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MyAlbum.Models.Xml
{
    public class XmlStamp : XmlBaseElement
    {
        //[XmlAttribute("style")]
        //public string Style { get; set; }
        [XmlAttribute("vspace")]
        public double VSpace { get; set; }
        [XmlAttribute("width")]
        public double Width { get; set; }
        [XmlAttribute("height")]
        public double Height { get; set; }
        [XmlAttribute("title")]
        public string Title { get; set; }
        [XmlAttribute("image")]
        public string Image { get; set; }
        [XmlAttribute("i1")]
        public string I1 { get; set; }
        [XmlAttribute("i2")]
        public string I2 { get; set; }
        [XmlAttribute("i3")]
        public string I3 { get; set; }
        [XmlAttribute("f1")]
        public string F1 { get; set; }
        [XmlAttribute("f2")]
        public string F2 { get; set; }
        [XmlAttribute("f3")]
        public string F3 { get; set; }
    }
}
