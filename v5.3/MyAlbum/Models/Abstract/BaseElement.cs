using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MyAlbum.Models.Abstract
{
    public class BaseElement
    {
        [XmlAttribute("height")]
        public double Height { get; set; }
        [XmlAttribute("width")]
        public double Width { get; set; }
    }
}
