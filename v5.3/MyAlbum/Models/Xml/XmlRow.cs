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
        //[XmlAttribute("style")]
        //public string Style { get; set; }
        [XmlAttribute("height")]
        public double Height { get; set; }
        //[XmlAttribute("align")]
        //public string Align { get; set; }
        //[XmlAttribute("valign")]
        //public string VAlign { get; set; }
        [XmlAttribute("spacing-mode")]
        public string SpacingMode { get; set; }
        [XmlAttribute("space")]
        public string Space { get; set; }
        [XmlAttribute("bgcolor")]
        public string BgColor { get; set; }
        [XmlAttribute("rotate")]
        public bool Rotate { get; set; }

        //[XmlElement("row", typeof(XmlRow))]
        [XmlElement("column", typeof(XmlColumn))]
        [XmlElement("text", typeof(XmlText))]
        [XmlElement("stamp", typeof(XmlStamp))]
        [XmlElement("image", typeof(XmlImage))]
        //[XmlElement("border", typeof(XmlBorder))]
        public List<XmlElement> Elements { get; set; }





        //[XmlElement("column")]
        //public List<XmlColumn> Columns { get; set; }
        //[XmlElement("text")]
        //public List<XmlText> Texts { get; set; }
        //[XmlElement("stamp")]
        //public List<XmlStamp> Stamps { get; set; }
        //[XmlElement("image")]
        //public List<XmlImage> Images { get; set; }
        //[XmlElement("border")]
        //public List<XmlBorder> Borders { get; set; }
    }
}
