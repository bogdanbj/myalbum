//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

using System.Xml.Serialization;

namespace MyAlbum.Models.Xml
{
    public class XmlSpace : XmlElement
    {
        [XmlAttribute("height")]
        public double Height { get; set; }

        [XmlAttribute("width")]
        public double Width { get; set; }
    }
}
