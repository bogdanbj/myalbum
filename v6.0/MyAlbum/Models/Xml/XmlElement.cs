using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MyAlbum.Models.Xml
{
    public abstract class XmlElement
    {
        [XmlAttribute("name")]
        public string Name { get; set; }
        [XmlAttribute("style")]
        public string Style { get; set; }
        [XmlAttribute("margin")]
        public string Margin { get; set; }
        [XmlAttribute("color")]
        public string Color { get; set; }
        [XmlAttribute("bgcolor")]
        public string BgColor { get; set; }
        [XmlAttribute("align")]
        public string Align { get; set; }
        [XmlAttribute("valign")]
        public string VAlign { get; set; }

    }
}
