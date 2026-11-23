using MyAlbum.Utilities;
using PdfSharp.Drawing;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace MyAlbum.Models
{
    internal class Rect : BaseElement<RectStyle>
    {
        #region Fields
        protected XUnit? _lineWidth;
        private string? _xRaw;
        private string? _yRaw;
        #endregion

        #region Style properties
        public XUnit LineWidth
        {
            get => _lineWidth ?? Style?.LineWidth ?? XUnit.Zero;
            set => _lineWidth = value;
        }
        #endregion
        
        #region Constructors
        public Rect()
        {
            Style = Styles.Rect.GetStyle("default") ?? new RectStyle();
        }
        #endregion

        #region Override methods
        internal override void ParseXml(XElement xElem)
        {
            Style = Styles.Rect.GetStyle(xElem.Attribute("style")?.Value);
            base.ParseXml(xElem);

            _xRaw = xElem.Attribute("x")?.Value;
            _yRaw = xElem.Attribute("y")?.Value;

            _lineWidth = XmlParser.ParseXUnit(xElem.Attribute("line-width")?.Value);
        }

        internal override void Calculate(XGraphics gfx, Canvas parentCanvas)
        {
            base.Calculate(gfx, parentCanvas);

            X = CalculateMeasure(_xRaw, parentCanvas.W);
            Y = CalculateMeasure(_yRaw, parentCanvas.H);
        }

        internal override void Draw(XGraphics gfx)
        {
            base.Draw(gfx);

            var pen = new XPen(Color, LineWidth.Point);
            gfx.DrawRectangle(pen, 0, 0, W.Point, H.Point);

        }
        #endregion
    }
}
