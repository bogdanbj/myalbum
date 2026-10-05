using MyAlbum.Utilities;
using PdfSharp.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace MyAlbum.Models
{
    internal class StampStyle : BaseElementStyle
    {
        #region Properties
        public XUnit? VSpace { get; set; }

        // The stamp style defines the composition of the stamp (similar to how a
        // page composition lists its predefined child elements). Each child element
        // (title / frame / image / inside1..3 / footer1..3) describes the appearance
        // of the corresponding predefined part. The actual content values come from
        // the <stamp> element attributes.
        public List<XElement> ChildElements { get; set; } = new List<XElement>();
        #endregion

        internal override void ParseXml(XElement xStamp)
        {
            base.ParseXml(xStamp);

            VSpace = XmlParser.ParseXUnit(xStamp.Attribute("vspace")?.Value);

            ChildElements = xStamp.Elements().ToList();
        }
    }
}
