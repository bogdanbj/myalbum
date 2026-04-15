using PdfSharpCore;
using PdfSharpCore.Drawing;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using MyAlbum.Utilities;

namespace MyAlbum.Models
{
    internal class PageStyle : BaseElementStyle
    {
        public PageOrientation? Orientation { get; set; }
        public PageSize? Size { get; set; }
        //public string? Padding { get; set; }
        //public string? VSpace { get; set; }
        public List<XElement> ChildElements { get; set; } = new List<XElement>();

        internal override void ParseXml(XElement xPage)
        {
            base.ParseXml(xPage);
            Orientation = XmlParser.ParseOrientation(xPage.Attribute("orientation")?.Value);
            Size = XmlParser.ParsePageSize(xPage.Attribute("size")?.Value);
            //VSpace = elem.Attribute("vspace")?.Value ?? null;

            ChildElements = xPage.Elements().ToList();
        }
    }
}
