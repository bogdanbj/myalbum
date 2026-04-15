using MyAlbum.Utilities;
using PdfSharpCore.Drawing;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Xml.Linq;

namespace MyAlbum.Models
{
    internal class FrameStyle : BaseElementStyle
    {
        #region Properties
        public FrameType? TypeTop { get; set; }
        public FrameType? TypeRight { get; set; }
        public FrameType? TypeBottom { get; set; }
        public FrameType? TypeLeft { get; set; }
        public XUnit? LineWidth1 { get; set; }
        public XUnit? LineWidth2 { get; set; }
        public XUnit? Offset { get; set; }
        #endregion

        #region Methods
        internal override void ParseXml(XElement xFrame)
        {
            base.ParseXml(xFrame);

            (TypeTop, TypeRight, TypeBottom, TypeLeft) = XmlParser.ParseFrameType(xFrame.Attribute("frame-type")?.Value);
            (LineWidth1, Offset, LineWidth2) = XmlParser.ParseFrameWidth(xFrame.Attribute("frame-width")?.Value);

            //FrameType = xFrame.Attribute("frame-type")?.Value;
            //FrameWidth = xFrame.Attribute("frame-width")?.Value;
        }
        #endregion
    }
}
