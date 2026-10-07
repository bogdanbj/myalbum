using MyAlbum.Utilities;
using PdfSharp.Drawing;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace MyAlbum.Models
{
    internal class ImageStyle : BaseElementStyle
    {
        public bool? Absolute { get; set; }
        //public bool? Rotate { get; set; }
        internal override void ParseXml(XElement xImage)
        {
            base.ParseXml(xImage);

            Absolute = XmlParser.ParseBool(xImage.Attribute("absolute")?.Value);
            //Rotate = XmlParser.ParseBool(xImage.Attribute("rotate")?.Value);
        }
    }
}
