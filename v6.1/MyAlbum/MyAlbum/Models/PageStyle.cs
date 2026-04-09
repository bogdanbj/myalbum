using PdfSharpCore;
using PdfSharpCore.Drawing;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using MyAlbum.Utilities;

namespace MyAlbum.Models
{
    internal class PageStyle: BaseElementStyle
    {
        #region Fields
        //protected PageOrientation? _orientation;
        //protected PageSize? _size;
        //margins
        //vspace
        #endregion

        public PageOrientation? Orientation { get; set; }
        public PageSize? Size { get; set; }
        //public string? Margin { get; set; }
        //public string? VSpace { get; set; }

        internal new void ParseXml(XElement elem)
        {
            base.ParseXml(elem); 
            Orientation = XmlParser.ParseOrientation(elem.Attribute("orientation")?.Value);
            Size = XmlParser.ParsePageSize(elem.Attribute("size")?.Value);
            //Margin = elem.Attribute("margin")?.Value ?? null;
            //VSpace = elem.Attribute("vspace")?.Value ?? null;
        }
    }
}
