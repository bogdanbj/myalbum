using PdfSharp.Drawing;
using System;
using System.Collections.Generic;
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
        protected XUnit? _lineWidth1;// = XUnit.Zero;
        protected XUnit? _lineWidth2;// = XUnit.Zero;
        protected XUnit? _offset;// = XUnit.Zero;
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
        internal override void Calculate(XGraphics gfx, Canvas parentCanvas)
        {
            base.Calculate(gfx, parentCanvas);
            CalculateThickness();

            Canvas.X += WidthLeft;
            Canvas.Y += WidthTop;
            Canvas.W -= WidthLeft + WidthRight;
            Canvas.H -= WidthTop + WidthBottom;
        }
        internal void CalculateThickness()
        {
            WidthTop    = CalculateSideThickness(TypeTop);
            WidthRight  = CalculateSideThickness(TypeRight);
            WidthBottom = CalculateSideThickness(TypeBottom);
            WidthLeft   = CalculateSideThickness(TypeLeft);
        }
        private XUnit CalculateSideThickness(FrameType type) => type switch
        {
            FrameType.None => XUnit.Zero,
            FrameType.Single => LineWidth1,
            FrameType.Double => LineWidth1 + Offset + LineWidth2,
            _ => XUnit.Zero
        };
        internal override void Draw(XGraphics gfx)
        {
            base.Draw(gfx);
            //gfx.DrawRectangle(new XSolidBrush(BgColor), 0, 0, W.Point, H.Point);

            XUnit X1 = XUnit.Zero, 
                  Y1 = XUnit.Zero, 
                  X2 = XUnit.Zero, 
                  Y2 = XUnit.Zero;
            XPen pen = new XPen(Color) { LineCap = XLineCap.Round };
            
            // Draw TOP frame
            if (TypeTop == FrameType.Single || TypeTop == FrameType.Double)
            {
                // Draw first line
                X1 = LineWidth1 / 2.0;
                Y1 = LineWidth1 / 2.0;
                X2 = W - LineWidth1 / 2.0;
                Y2 = LineWidth1 / 2.0;
                pen.Width = LineWidth1.Point;
                gfx.DrawLine(pen, X1.Point, Y1.Point, X2.Point, Y2.Point);
            }
            if (TypeTop == FrameType.Double)
            {
                // Draw second line
                X1 = (TypeLeft == FrameType.Double ? LineWidth1 + Offset : XUnit.Zero) + LineWidth2 / 2.0;
                Y1 = LineWidth1 + Offset + LineWidth2 / 2.0;
                X2 = W - (TypeRight == FrameType.Double ? LineWidth1 + Offset : XUnit.Zero) - LineWidth2 / 2.0;
                Y2 = LineWidth1 + Offset + LineWidth2 / 2.0;
                pen.Width = LineWidth2.Point;
                gfx.DrawLine(pen, X1.Point, Y1.Point, X2.Point, Y2.Point);
            }

            // Draw RIGHT frame
            if (TypeRight == FrameType.Single || TypeRight == FrameType.Double)
            {
                // Draw first line
                X1 = W - LineWidth1 / 2.0;
                Y1 = LineWidth1 / 2.0;
                X2 = W - LineWidth1 / 2.0;
                Y2 = H - LineWidth1 / 2.0;
                pen.Width = LineWidth1.Point;
                gfx.DrawLine(pen, X1.Point, Y1.Point, X2.Point, Y2.Point);
            }
            if (TypeRight == FrameType.Double)
            {
                // Draw second line
                X1 = W - LineWidth1 - Offset - LineWidth2 / 2.0;
                Y1 = (TypeTop == FrameType.Double ? LineWidth1 + Offset : XUnit.Zero) + LineWidth2 / 2.0;
                X2 = W - LineWidth1 - Offset - LineWidth2 / 2.0;
                Y2 = H - (TypeBottom == FrameType.Double ? LineWidth1 + Offset : XUnit.Zero) - LineWidth2 / 2.0;
                pen.Width = LineWidth2.Point;
                gfx.DrawLine(pen, X1.Point, Y1.Point, X2.Point, Y2.Point);
            }

            // Draw BOTTOM frame
            if (TypeBottom == FrameType.Single || TypeBottom == FrameType.Double)
            {
                // Draw first line
                X1 = LineWidth1 / 2.0;
                Y1 = H - LineWidth1 / 2.0;
                X2 = W - LineWidth1 / 2.0;
                Y2 = H - LineWidth1 / 2.0;
                pen.Width = LineWidth1.Point;
                gfx.DrawLine(pen, X1.Point, Y1.Point, X2.Point, Y2.Point);
            }
            if (TypeBottom == FrameType.Double)
            {
                // Draw second line
                X1 = (TypeLeft == FrameType.Double ? LineWidth1 + Offset : XUnit.Zero) + LineWidth2 / 2.0;
                Y1 = H - LineWidth1 - Offset - LineWidth2 / 2.0;
                X2 = W - (TypeRight == FrameType.Double ? LineWidth1 + Offset : XUnit.Zero) - LineWidth2 / 2.0;
                Y2 = H - LineWidth1 - Offset - LineWidth2 / 2.0;
                pen.Width = LineWidth2.Point;
                gfx.DrawLine(pen, X1.Point, Y1.Point, X2.Point, Y2.Point);
            }

            // Draw LEFT frame
            if (TypeLeft == FrameType.Single || TypeLeft == FrameType.Double)
            {
                // Draw first line
                X1 = LineWidth1 / 2.0;
                Y1 = LineWidth1 / 2.0;
                X2 = LineWidth1 / 2.0;
                Y2 = H - LineWidth1 / 2.0;
                pen.Width = LineWidth1.Point;
                gfx.DrawLine(pen, X1.Point, Y1.Point, X2.Point, Y2.Point);
            }
            if (TypeLeft == FrameType.Double)
            {
                // Draw second line
                X1 = LineWidth1 + Offset + LineWidth2 / 2.0;
                Y1 = (TypeTop == FrameType.Double ? LineWidth1 + Offset : XUnit.Zero) + LineWidth2 / 2.0;
                X2 = LineWidth1 + Offset + LineWidth2 / 2.0;
                Y2 = H - (TypeBottom == FrameType.Double ? LineWidth1 + Offset : XUnit.Zero) - LineWidth2 / 2.0;
                pen.Width = LineWidth2.Point;
                gfx.DrawLine(pen, X1.Point, Y1.Point, X2.Point, Y2.Point);
            }

        }
        #endregion
    }
}
