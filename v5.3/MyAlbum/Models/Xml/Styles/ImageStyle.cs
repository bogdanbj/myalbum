using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MyAlbum.Models.Xml.Styles
{
    public class ImageStyle : XmlImage
    {
        [XmlAttribute("default")]
        public bool IsDefault { get; set; }
    }
}
