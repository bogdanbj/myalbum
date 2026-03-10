using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MyAlbum.Models.Xml
{
    public class XmlBorder : XmlElement
    {

        [XmlAttribute("style")]
        public string Style { get; set; }
        [XmlAttribute("border-type")]
        public string BorderType { get; set; }
        [XmlAttribute("border-width")]
        public string BorderWidth { get; set; }
        [XmlAttribute("margin")]
        public string Margin { get; set; }
        [XmlAttribute("padding")]
        public string Padding { get; set; }
        //private string _padding;
        //[XmlAttribute("color")]
        //public string Color { get; set; }
        //[XmlAttribute("bgcolor")]
        //public string BgColor { get; set; }
    }
}
