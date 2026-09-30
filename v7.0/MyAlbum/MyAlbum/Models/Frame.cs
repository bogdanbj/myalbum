using PdfSharpCore.Drawing;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Xml.Linq;
using MyAlbum.Utilities; 

namespace MyAlbum.Models
{
    internal class Frame : BaseElement<FrameStyle>
    {
        #region Fields
        protected FrameType? _typeTop;
        protected FrameType? _typeRight;
        protected FrameType? _typeBottom;
        protected FrameType? _typeLeft;
        protected XUnit? _lineWidth1;
        protected XUnit? _lineWidth2;
        protected XUnit? _offset;
        #endregion

        #region Style properties
        public FrameType TypeTop 
        { 
            get => _typeTop ?? Style.TypeTop ?? FrameType.None; 
            set => _typeTop = value; 
        }
        public FrameType TypeRight 
        { 
            get => _typeRight ?? Style.TypeRight ?? FrameType.None; 
            set => _typeRight = value; 
        }
        public FrameType TypeBottom 
        { 
            get => _typeBottom ?? Style.TypeBottom ?? FrameType.None; 
            set => _typeBottom = value; 
        }
        public FrameType TypeLeft 
        { 
            get => _typeLeft ?? Style.TypeLeft ?? FrameType.None; 
            set => _typeLeft = value; 
        }
        public XUnit LineWidth1 
        { 
            get => _lineWidth1 ?? Style.LineWidth1 ?? XUnit.Zero; 
            set => _lineWidth1 = value; 
        }
        public XUnit LineWidth2 
        { 
            get => _lineWidth2 ?? Style.LineWidth2 ?? XUnit.Zero;
            set => _lineWidth2 = value; 
        }
        public XUnit Offset 
        { 
            get => _offset ?? Style.Offset ?? XUnit.Zero;
            set => _offset = value;
        }
        #endregion

        #region Other properties
        public XUnit WidthTop { get; set; }
        public XUnit WidthRight { get; set; }
        public XUnit WidthBottom { get; set; }
        public XUnit WidthLeft { get; set; }
        #endregion

        #region Constructors
        public Frame()
        {
            Style = Styles.Frame.GetStyle("default") ?? new FrameStyle();
        }
        #endregion

        #region Override methods
        internal override void ParseXml(XElement xElem)
        {
            Style = Styles.Frame.GetStyle(xElem.Attribute("style")?.Value);
            base.ParseXml(xElem);

            (_typeTop, _typeRight, _typeBottom, _typeLeft) = XmlParser.ParseFrameType(xElem.Attribute("frame-type")?.Value);
            (_lineWidth1, _offset, _lineWidth2) = XmlParser.ParseFrameWidth(xElem.Attribute("frame-width")?.Value);
        }
        #endregion
    }
}
