using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using MyAlbum.Models.Abstract;

namespace MyAlbum.Models.Xml.Styles
{
    public class PageStyle : XmlPage
    {
        [XmlAttribute("default")]
        public bool IsDefault { get; set; }
    }
}
