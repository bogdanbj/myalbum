using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MyAlbum.Models.Xml
{
    public class XmlStamp : XmlElement
    {
        //[XmlAttribute("style")]
        //public string Style { get; set; }
        [XmlAttribute("vspace")]
        public string VSpace { get; set; }
        [XmlAttribute("width")]
        public double Width { get; set; }
        [XmlAttribute("height")]
        public double Height { get; set; }

        #region value oriented attributes
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
        #endregion

        #region style oriented attributes
        [XmlElement("border")]
        public XmlBorder BorderStyle { get; set; }
        [XmlElement("image")]
        public XmlImage ImageStyle { get; set; }
        [XmlElement("title")]
        public XmlText TitleStyle { get; set; }
        [XmlElement("inside1")]
        public XmlText Inside1Style { get; set; }
        [XmlElement("inside2")]
        public XmlText Inside2Style { get; set; }
        [XmlElement("inside3")]
        public XmlText Inside3Style { get; set; }
        [XmlElement("footer1")]
        public XmlText Footer1Style { get; set; }
        [XmlElement("footer2")]
        public XmlText Footer2Style { get; set; }
        [XmlElement("footer3")]
        public XmlText Footer3Style { get; set; }
        #endregion
    }
}
