using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MyAlbum.Models.Xml
{
    public class XmlBorder : XmlBaseElement
    {
        //[XmlAttribute("style")]
        //public string Style { get; set; }
        [XmlAttribute("border-type")]
        public string BorderType { get; set; }
        [XmlAttribute("width")]
        public string Width { get; set; }
        [XmlAttribute("margin")]
        public string Margin { get; set; }
        [XmlAttribute("padding")]
        public string Padding { get; set; }
        [XmlAttribute("color")]
        public string Color { get; set; }
        [XmlAttribute("background-color")]
        public string BackgroundColor { get; set; }
    }
}
