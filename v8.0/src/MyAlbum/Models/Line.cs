using System;
using System.Collections.Generic;
using System.Text;
using PdfSharp.Drawing;
using MyAlbum.Utilities;
using System.Xml.Linq;

namespace MyAlbum.Models
{
    internal class Line : BaseElement<LineStyle>
    {
        #region Fields
        protected XUnit? _lineWidth;
        private string? _x1Raw;
        private string? _y1Raw;
        private string? _x2Raw;
        private string? _y2Raw;

        #endregion

        #region Core properties
        XUnit X1 { get; set; }
        XUnit Y1 { get; set; }
        XUnit X2 { get; set; }
        XUnit Y2 { get; set; }

        #endregion

        #region Style properties
        public XUnit LineWidth
        {
            get => _lineWidth ?? Style?.LineWidth ?? XUnit.Zero;
            set => _lineWidth = value;
        }
        #endregion

        #region Constructors
        public Line()
        {
            Style = Styles.Line.GetStyle("default") ?? new LineStyle();
        }
        #endregion

        #region Override methods
        internal override void ParseXml(XElement xElem)
        {
            Style = Styles.Line.GetStyle(xElem.Attribute("style")?.Value);
            base.ParseXml(xElem);

            _x1Raw = xElem.Attribute("x1")?.Value;
            _y1Raw = xElem.Attribute("y1")?.Value;
            _x2Raw = xElem.Attribute("x2")?.Value;
            _y2Raw = xElem.Attribute("y2")?.Value;

            _lineWidth = XmlParser.ParseXUnit(xElem.Attribute("width")?.Value);
        }

        internal override void Calculate(XGraphics gfx, Canvas parentCanvas)
        {
            base.Calculate(gfx, parentCanvas);

            X1 = CalculateMeasure(_x1Raw, parentCanvas.W);
            Y1 = CalculateMeasure(_y1Raw, parentCanvas.H);
            X2 = CalculateMeasure(_x2Raw, parentCanvas.W);
            Y2 = CalculateMeasure(_y2Raw, parentCanvas.H);
        }

        internal override void Draw(XGraphics gfx)
        {
            base.Draw(gfx);

            var pen = new XPen(Color, LineWidth.Point) { LineCap = XLineCap.Square };
            gfx.DrawLine(pen, X1.Point, Y1.Point, X2.Point, Y2.Point);
            
        }
        #endregion

    }
}
