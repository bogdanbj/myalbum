using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MyAlbum.Models.Xml
{
    public class XmlPage
    {
        [XmlAttribute("style")]
        public string Style { get; set; }
        [XmlAttribute("no")]
        public int Number { get; set; }
        [XmlAttribute("size")]
        public string Size { get; set; }
        [XmlAttribute("orientation")]
        public string Orientation { get; set; }
        [XmlAttribute("title")]
        public string Title { get; set; }
        [XmlAttribute("margin")]
        public string Margin { get; set; }
        [XmlAttribute("color")]
        public string Color { get; set; }
        [XmlAttribute("vspace")]
        public double VSpace { get; set; }
        [XmlElement("row")]
        public List<XmlRow> Rows { get; set; }
    }
}
