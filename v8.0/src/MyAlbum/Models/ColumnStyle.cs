using MyAlbum.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using PdfSharp.Drawing;

namespace MyAlbum.Models
{
    internal class ColumnStyle : BaseElementStyle
    {
        #region Properties
        public Alignment? Align { get; set; }
        public XUnit? VSpace { get; set; }
        #endregion

        internal override void ParseXml(XElement xColumn)
        {
            base.ParseXml(xColumn);

            Align = XmlParser.ParseAlignment(xColumn.Attribute("align")?.Value);
            VSpace = XmlParser.ParseXUnit(xColumn.Attribute("vspace")?.Value);
        }
    }
}
