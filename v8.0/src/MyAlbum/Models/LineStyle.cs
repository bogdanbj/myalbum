using MyAlbum.Utilities;
using PdfSharp.Drawing;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace MyAlbum.Models
{
    internal class LineStyle : BaseElementStyle
    {
        #region Properties
        public XUnit? LineWidth { get; set; }
        #endregion


        #region Methods
        internal override void ParseXml(XElement xLine)
        {
            base.ParseXml(xLine);

            LineWidth = XmlParser.ParseXUnit(xLine.Attribute("line-width")?.Value);
        }
        #endregion
    }
    
}