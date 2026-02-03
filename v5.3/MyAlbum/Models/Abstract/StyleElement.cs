using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MyAlbum.Models.Abstract
{
    public class StyleElement : BaseElement
    {
        [XmlAttribute("style")]
        public string Name { get; set; }
        [XmlAttribute("default")]
        public bool IsDefault { get; set; }
        
        public StyleElement()
        {
            Name = string.Empty;
            IsDefault = false;
        }
    }
}
