using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using MyAlbum.Models.Xml.Styles;

namespace MyAlbum.Models.Xml
{
    [XmlRoot("MyAlbum")]
    public class XmlAlbum
    {
        [XmlAttribute("ver")]
        public string Version { get; set; }

        //[XmlArray("styles")]
        //[XmlArrayItem("border", Type = typeof(BorderStyle))]
        //public List<BorderStyle> BorderStyles { get; set; }

        //[XmlArray("styles")]
        //[XmlArrayItem("text", Type = typeof(TextStyle))]
        //public List<TextStyle> TextStyles { get; set; }

        //[XmlArray("styles")]
        //[XmlArrayItem("image", Type = typeof(ImageStyle))]
        //public List<ImageStyle> ImageStyles { get; set; }



        [XmlElement("styles")]
        public AlbumStyles Styles { get; set; }

        [XmlElement("page")]
        public List<XmlPage> Pages { get; set; }
    }
    public class AlbumStyles
    {
        [XmlElement("border")]
        public List<BorderStyle> BorderStyles { get; set; }

        [XmlElement("text")]
        public List<TextStyle> TextStyles { get; set; }

        [XmlElement("image")]
        public List<ImageStyle> ImageStyles { get; set; }

        [XmlElement("stamp")]
        public List<StampStyle> StampStyles { get; set; }

        [XmlElement("row")]
        public List<RowStyle> RowStyles { get; set; }

        [XmlElement("column")]
        public List<ColumnStyle> ColumnStyles { get; set; }

        [XmlElement("page")]
        public List<PageStyle> PageStyles { get; set; }
    }
}
