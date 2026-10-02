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
        protected XUnit? _lineWidth1 = XUnit.Zero;
        protected XUnit? _lineWidth2 = XUnit.Zero;
        protected XUnit? _offset = XUnit.Zero;
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
            WidthTop = CalculateFrameWidth(TypeTop, LineWidth1, Offset, LineWidth2);
            WidthRight = CalculateFrameWidth(TypeRight, LineWidth1, Offset, LineWidth2);
            WidthBottom = CalculateFrameWidth(TypeBottom, LineWidth1, Offset, LineWidth2);
            WidthLeft = CalculateFrameWidth(TypeLeft, LineWidth1, Offset, LineWidth2);
        }
        private XUnit CalculateFrameWidth(FrameType type, XUnit width1, XUnit offset, XUnit width2)
        {
            return type switch
            {
                FrameType.None => XUnit.Zero,
                FrameType.Single => width1,
                FrameType.Double => width1 + offset + width2,
                _ => throw new ArgumentOutOfRangeException(nameof(type), $"Unsupported frame type: {type}"),
            };
        }
        internal override void Draw(XGraphics gfx)
        {
            base.Draw(gfx);
            gfx.DrawRectangle(new XSolidBrush(BgColor), X, Y, W, H);

            XUnit X1 = XUnit.Zero, 
                  Y1 = XUnit.Zero, 
                  X2 = XUnit.Zero, 
                  Y2 = XUnit.Zero;
            XPen pen = new XPen(Color) { LineCap = XLineCap.Round };
            
            // Draw TOP frame
            if (TypeTop == FrameType.Single || TypeTop == FrameType.Double)
            {
                // Draw first line
                X1 = X + LineWidth1 / 2.0;
                Y1 = Y + LineWidth1 / 2.0;
                X2 = X + W - LineWidth1 / 2.0;
                Y2 = Y + LineWidth1 / 2.0;
                pen.Width = LineWidth1.Point;
                gfx.DrawLine(pen, X1, Y1, X2, Y2);
            }
            if (TypeTop == FrameType.Double)
            {
                // Draw second line
                X1 = X + (TypeLeft == FrameType.Double ? LineWidth1 + Offset : XUnit.Zero) + LineWidth2 / 2.0;
                Y1 = Y + LineWidth1 + Offset + LineWidth2 / 2.0;
                X2 = X + W - (TypeRight == FrameType.Double ? LineWidth1 + Offset : XUnit.Zero) - LineWidth2 / 2.0;
                Y2 = Y + LineWidth1 + Offset + LineWidth2 / 2.0;
                pen.Width = LineWidth2.Point;
                gfx.DrawLine(pen, X1, Y1, X2, Y2);
            }

            // Draw RIGHT frame
            if (TypeRight == FrameType.Single || TypeRight == FrameType.Double)
            {
                // Draw first line
                X1 = X + W - LineWidth1 / 2.0;
                Y1 = Y + LineWidth1 / 2.0;
                X2 = X + W - LineWidth1 / 2.0;
                Y2 = Y + H - LineWidth1 / 2.0;
                pen.Width = LineWidth1.Point;
                gfx.DrawLine(pen, X1, Y1, X2, Y2);
            }
            if (TypeRight == FrameType.Double)
            {
                // Draw second line
                X1 = X + W - LineWidth1 - Offset - LineWidth2 / 2.0;
                Y1 = Y + (TypeTop == FrameType.Double ? LineWidth1 + Offset : XUnit.Zero) + LineWidth2 / 2.0;
                X2 = X + W - LineWidth1 - Offset - LineWidth2 / 2.0;
                Y2 = Y + H - (TypeBottom == FrameType.Double ? LineWidth1 + Offset : XUnit.Zero) - LineWidth2 / 2.0;
                pen.Width = LineWidth2.Point;
                gfx.DrawLine(pen, X1, Y1, X2, Y2);
            }

            // Draw BOTTOM frame
            if (TypeBottom == FrameType.Single || TypeBottom == FrameType.Double)
            {
                // Draw first line
                X1 = X + LineWidth1 / 2.0;
                Y1 = Y + H - LineWidth1 / 2.0;
                X2 = X + W - LineWidth1 / 2.0;
                Y2 = Y + H - LineWidth1 / 2.0;
                pen.Width = LineWidth1.Point;
                gfx.DrawLine(pen, X1, Y1, X2, Y2);
            }
            if (TypeBottom == FrameType.Double)
            {
                // Draw second line
                X1 = X + (TypeLeft == FrameType.Double ? LineWidth1 + Offset : XUnit.Zero) + LineWidth2 / 2.0;
                Y1 = Y + H - LineWidth1 - Offset - LineWidth2 / 2.0;
                X2 = X + W - (TypeRight == FrameType.Double ? LineWidth1 + Offset : XUnit.Zero) - LineWidth2 / 2.0;
                Y2 = Y + H - LineWidth1 - Offset - LineWidth2 / 2.0;
                pen.Width = LineWidth2.Point;
                gfx.DrawLine(pen, X1, Y1, X2, Y2);
            }

            // Draw LEFT frame
            if (TypeLeft == FrameType.Single || TypeLeft == FrameType.Double)
            {
                // Draw first line
                X1 = X + LineWidth1 / 2.0;
                Y1 = Y + LineWidth1 / 2.0;
                X2 = X + LineWidth1 / 2.0;
                Y2 = Y + H - LineWidth1 / 2.0;
                pen.Width = LineWidth1.Point;
                gfx.DrawLine(pen, X1, Y1, X2, Y2);
            }
            if (TypeLeft == FrameType.Double)
            {
                // Draw second line
                X1 = X + LineWidth1 + Offset + LineWidth2 / 2.0;
                Y1 = Y + (TypeTop == FrameType.Double ? LineWidth1 + Offset : XUnit.Zero) + LineWidth2 / 2.0;
                X2 = X + LineWidth1 + Offset + LineWidth2 / 2.0;
                Y2 = Y + H - (TypeBottom == FrameType.Double ? LineWidth1 + Offset : XUnit.Zero) - LineWidth2 / 2.0;
                pen.Width = LineWidth2.Point;
                gfx.DrawLine(pen, X1, Y1, X2, Y2);
            }




            //var topLeft = new XPoint(X, Y);
            //var topRight = new XPoint(X + W, Y);
            //var bottomRight = new XPoint(X + W, Y + H);
            //var bottomLeft = new XPoint(X, Y + H);

            //DrawSide(gfx, TypeTop, topLeft, topRight, new XVector(0, 1));
            //DrawSide(gfx, TypeRight, topRight, bottomRight, new XVector(-1, 0));
            //DrawSide(gfx, TypeBottom, bottomLeft, bottomRight, new XVector(0, -1));
            //DrawSide(gfx, TypeLeft, topLeft, bottomLeft, new XVector(1, 0));
        }

        /// <summary>
        /// Draws a single frame side along the edge defined by <paramref name="start"/> and
        /// <paramref name="end"/>. <paramref name="inward"/> is a unit vector pointing toward
        /// the inside of the box; lines are inset along it so strokes stay within the box.
        /// None draws nothing, Single draws one line (LineWidth1), Double draws an outer line
        /// (LineWidth1), a gap (Offset) and an inner line (LineWidth2).
        /// </summary>
        private void DrawSide(XGraphics gfx, FrameType type, XPoint start, XPoint end, XVector inward)
        {
            if (type == FrameType.None)
                return;

            DrawInsetLine(gfx, start, end, inward, LineWidth1, XUnit.Zero);

            if (type == FrameType.Double)
            {
                DrawInsetLine(gfx, start, end, inward, LineWidth2, LineWidth1 + Offset);
            }
        }

        /// <summary>
        /// Draws a line parallel to the edge (<paramref name="start"/> → <paramref name="end"/>),
        /// inset inward by <paramref name="inset"/> plus half of <paramref name="lineWidth"/> so
        /// the stroke sits fully inside the box.
        /// </summary>
        private void DrawInsetLine(XGraphics gfx, XPoint start, XPoint end, XVector inward, XUnit lineWidth, XUnit inset)
        {
            if (lineWidth <= XUnit.Zero)
                return;

            double offset = inset.Point + lineWidth.Point / 2.0;
            var shift = new XVector(inward.X * offset, inward.Y * offset);

            var pen = new XPen(Color, lineWidth.Point);
            gfx.DrawLine(pen, start + shift, end + shift);
        }
        #endregion
    }
}
