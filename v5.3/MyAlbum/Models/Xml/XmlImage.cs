using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MyAlbum.Models.Xml
{
    public class XmlImage : XmlElement
    {
        //[XmlAttribute("style")]
        //public string Style { get; set; }
        [XmlAttribute("color")]
        public string Color { get; set; }
        [XmlAttribute("absolute")]
        public bool Absolute { get; set; }
        //[XmlAttribute("align")]
        //public string Align { get; set; }
        //[XmlAttribute("valign")]
        //public string Valign { get; set; }
        [XmlAttribute("file_name")]
        public string FileName { get; set; }
        [XmlAttribute("x")]
        public double X { get; set; }
        [XmlAttribute("y")]
        public double Y { get; set; }
        [XmlAttribute("height")]
        public double Height { get; set; }
        [XmlAttribute("width")]
        public double Width { get; set; }
        [XmlAttribute("stretched")]
        public bool Stretched { get; set; }
        [XmlAttribute("margin")]
        public string Margin { get; set; }
    }
}
