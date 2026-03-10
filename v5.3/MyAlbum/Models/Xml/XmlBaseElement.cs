using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MyAlbum.Models.Xml
{
    public abstract class XmlBaseElement
    {
        [XmlAttribute("style")]
        public string Style { get; set; }
    }
}
