using MyAlbum.Models.Xml;
using MyAlbum.Models.Xml.Styles;
using MyAlbum.Services;
using MyAlbum.Utils;
using PdfSharpCore;
using PdfSharpCore.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace MyAlbum.Models.Layout
{
    internal class Border : BaseElement
    {
        public BorderType TypeTop { get; set; }
        public BorderType TypeRight { get; set; }
        public BorderType TypeBottom { get; set; }
        public BorderType TypeLeft { get; set; }
        public XUnit LineWidth1 { get; set; }
        public XUnit LineWidth2 { get; set; }
        public XUnit Offset { get; set; }
        public XUnit PaddingTop { get; set; }
        public XUnit PaddingRight { get; set; }
        public XUnit PaddingBottom { get; set; }
        public XUnit PaddingLeft { get; set; }
        public XUnit WidthTop { get; set; }
        public XUnit WidthRight { get; set; }
        public XUnit WidthBottom { get; set; }
        public XUnit WidthLeft { get; set; }
        public PageOrientation Orientation { get; set; }

        internal void FromXml(XmlBorder xmlBorder, AlbumStyles styles)
        {
            // Find the appropriate page style
            BorderStyle? style = StyleFactory.FindStyle<BorderStyle>(xmlBorder.Style ?? "", styles.BorderStyles);
            if (style == null)
            {
                throw new InvalidOperationException($"Border style '{xmlBorder.Style ?? "(default)"}' not found. ");
            }
            base.FromXml(xmlBorder, style);

            // Apply the style
            (TypeTop, TypeRight, TypeBottom, TypeLeft) = ParseBorderType(xmlBorder.BorderType ?? style.BorderType);
            (LineWidth1, Offset, LineWidth2) = ParseBorderWidth(xmlBorder.BorderWidth ?? style.BorderWidth);
            //(MarginTop, MarginRight, MarginBottom, MarginLeft) = ParseMargin(xmlBorder.Margin ?? style.Margin);
            (PaddingTop, PaddingRight, PaddingBottom, PaddingLeft) = ParsePadding(xmlBorder.Padding ?? style.Padding);
            Color = ParseColor(xmlBorder.Color ?? style.Color ?? $"{Color.R},{Color.G},{Color.B}");
            BgColor = ParseColor(xmlBorder.BgColor ?? style.BgColor ?? $"{BgColor.R},{BgColor.G},{BgColor.B}");

        }
        internal void CalculateBorderWidths()
        {
            WidthTop = CalculateLineWidth(TypeTop);
            WidthRight = CalculateLineWidth(TypeRight);
            WidthBottom = CalculateLineWidth(TypeBottom);
            WidthLeft = CalculateLineWidth(TypeLeft);
        }
        private XUnit CalculateLineWidth(BorderType type) => type switch
        {
            BorderType.None => XUnit.Zero,
            BorderType.Single => LineWidth1,
            BorderType.Double => LineWidth1 + Offset + LineWidth2,
            _ => XUnit.Zero
        };
        internal override void Calculate(XGraphics gfx, XUnit x, XUnit y, XUnit w, XUnit h)
        {
            // If Landscape, shift attributes 90 degrees counterclockwise
            if (Orientation == PageOrientation.Landscape)
            {
                // border types
                BorderType t = TypeTop;
                TypeTop = TypeLeft;
                TypeLeft = TypeBottom;
                TypeBottom = TypeRight;
                TypeRight = t;

                // margins
                XUnit m = MarginTop;
                MarginTop = MarginLeft;
                MarginLeft = MarginBottom;
                MarginBottom = MarginRight;
                MarginRight = m;

                // paddings
                XUnit p = PaddingTop;
                PaddingTop = PaddingLeft;
                PaddingLeft = PaddingBottom;
                PaddingBottom = PaddingRight;
                PaddingRight = p;
            }

            // Adjust the border X, Y, W, H with the exterior line width
            X = x + MarginLeft + LineWidth1 / 2;
            Y = y + MarginTop + LineWidth1 / 2;
            W = w - (MarginLeft + MarginRight + LineWidth1);
            H = h - (MarginTop + MarginBottom + LineWidth1);

            CalculateBorderWidths();

            this.Canvas.X = x + MarginLeft+ WidthLeft + PaddingLeft;
            this.Canvas.Y = y + MarginTop + WidthTop + PaddingTop;
            this.Canvas.W = w - MarginLeft - WidthLeft - PaddingLeft - MarginRight - WidthRight - PaddingRight;
            this.Canvas.H = h - MarginTop - WidthTop - PaddingTop - MarginBottom - WidthBottom - PaddingBottom;
            //Helper.DrawCorner(gfx, Canvas.X, Canvas.Y, XColors.Green);
        }
        internal override void Draw(XGraphics gfx)
        {
            XPen pen;
            XUnit x1, x2, y1, y2;

            #region single border
            pen = new XPen(this.Color, LineWidth1)
            {
                LineCap = XLineCap.Round,
                LineJoin = XLineJoin.Bevel
                //pen.DashStyle = XDashStyle.Dash;
            };
            if (TypeTop == BorderType.Single || TypeTop == BorderType.Double) 
            {
                x1 = X; 
                x2 = x1 + W;
                y1 = Y;
                y2 = Y;
                gfx.DrawLine(pen, x1, y1, x2, y2);
            }
            if (TypeRight == BorderType.Single || TypeRight == BorderType.Double) 
            {
                x1 = X + W;
                x2 = X + W;
                y1 = Y; 
                y2 = Y + H;
                gfx.DrawLine(pen, x1, y1, x2, y2);
            }
            if (TypeBottom == BorderType.Single || TypeBottom == BorderType.Double) 
            {
                x1 = X;
                x2 = X + W;
                y1 = Y + H;
                y2 = Y + H;
                gfx.DrawLine(pen, x1, y1, x2, y2);
            }
            if (TypeLeft == BorderType.Single || TypeLeft == BorderType.Double) 
            {
                x1 = X;
                x2 = X;
                y1 = Y;
                y2 = Y + H;
                gfx.DrawLine(pen, x1, y1, x2, y2);
            }
            #endregion

            #region double border
            XUnit space = LineWidth1 / 2 + Offset + LineWidth2 / 2;
            pen = new XPen(this.Color, LineWidth2)
            {
                LineCap = XLineCap.Round,
                LineJoin = XLineJoin.Bevel
            };
            if (TypeTop == BorderType.Double) 
            {
                x1 = X + ((TypeLeft == BorderType.Double) ? space : 0);
                x2 = X + W - ((TypeRight != BorderType.Double) ? space : 0);
                y1 = Y + space;
                y2 = Y + space;
                gfx.DrawLine(pen, x1, y1, x2, y2);
            }
            if (TypeRight == BorderType.Double) 
            {
                x1 = X + W - space;
                x2 = X + W - space;
                y1 = Y + ((TypeTop == BorderType.Double) ? space : 0);
                y2 = Y + H - ((TypeBottom == BorderType.Double) ? space : 0);
                gfx.DrawLine(pen, x1, y1, x2, y2);
            }
            if (TypeBottom == BorderType.Double) 
            {
                x1 = X + ((TypeLeft == BorderType.Double) ? space : 0);
                x2 = X + W - ((TypeRight == BorderType.Double) ? space : 0);
                y1 = Y + H - space;
                y2 = Y + H - space;
                gfx.DrawLine(pen, x1, y1, x2, y2);
            }
            if (TypeLeft == BorderType.Double) 
            {
                x1 = X + space;
                x2 = X + space;
                y1 = Y + ((TypeTop == BorderType.Double) ? space : 0);
                y2 = Y + H - ((TypeBottom == BorderType.Double) ? space : 0);
                gfx.DrawLine(pen, x1, y1, x2, y2);
            }
            #endregion
        }

        private (BorderType typeTop, BorderType typeRight, BorderType typeBottom, BorderType typeLeft) ParseBorderType(string borderType)
        {
            BorderType typeTop = BorderType.None;
            BorderType typeRight = BorderType.None;
            BorderType typeBottom = BorderType.None;
            BorderType typeLeft = BorderType.None;

            if (!string.IsNullOrWhiteSpace(borderType))
            {

                string[] arr = borderType.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                BorderType result;

                switch (arr.Length)
                {
                    case 1:
                        typeTop = typeRight = typeBottom = typeLeft = Enum.TryParse(arr[0], true, out result) ? result : BorderType.None;
                        break;
                    case 2:
                        typeTop = typeBottom = Enum.TryParse(arr[0], true, out result) ? result : BorderType.None;
                        typeLeft = typeRight = Enum.TryParse(arr[1], true, out result) ? result : BorderType.None;
                        break;
                    case 4:
                        typeTop = Enum.TryParse(arr[0], true, out result) ? result : BorderType.None;
                        typeRight = Enum.TryParse(arr[1], true, out result) ? result : BorderType.None;
                        typeBottom = Enum.TryParse(arr[2], true, out result) ? result : BorderType.None;
                        typeLeft = Enum.TryParse(arr[3], true, out result) ? result : BorderType.None;
                        break;
                }


            }
            return (typeTop, typeRight, typeBottom, typeLeft);
        }
        private (XUnit lineWidth1, XUnit offset, XUnit lineWidth2) ParseBorderWidth(string borderWidth)
        {
            // Default values
            XUnit lineWidth1 = 0;
            XUnit offset = 0;
            XUnit lineWidth2 = 0;

            if (!string.IsNullOrWhiteSpace(borderWidth))
            {
                string[] arr = borderWidth.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                
                switch (arr.Length)
                {
                    case 1:
                        lineWidth1 = XUnit.FromMillimeter(double.Parse(arr[0]));
                        offset = lineWidth2 = 0;
                        break;
                    case 2:
                        lineWidth1 = lineWidth2 = XUnit.FromMillimeter(double.Parse(arr[0]));
                        offset = XUnit.FromMillimeter(double.Parse(arr[1]));
                        break;
                    case 3:
                        lineWidth1 = XUnit.FromMillimeter(double.Parse(arr[0]));
                        offset = XUnit.FromMillimeter(double.Parse(arr[1]));
                        lineWidth2 = XUnit.FromMillimeter(double.Parse(arr[2]));
                        break;
                }

            }

            return (lineWidth1, offset, lineWidth2);
        }
    }
}
