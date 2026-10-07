using MyAlbum.Utilities;
using PdfSharp.Drawing;
using System.Xml.Linq;

namespace MyAlbum.Models
{
    internal abstract class BaseElement
    {
        #region fields
        protected XColor? _color;
        protected XColor? _bgColor;
        protected XColor? _parentColor;
        protected XColor? _parentBgColor;
        protected XUnit? _marginTop;
        protected XUnit? _marginRight;
        protected XUnit? _marginBottom;
        protected XUnit? _marginLeft;
        protected XUnit? _paddingTop;
        protected XUnit? _paddingRight;
        protected XUnit? _paddingBottom;
        protected XUnit? _paddingLeft;
        protected string? _height;
        protected string? _width;
        protected bool? _rotate;

        #endregion

        #region Core Properties
        public string? StyleName { get; set; }
        public XUnit X { get; set; }
        public XUnit Y { get; set; }
        public XUnit W { get; set; }
        public XUnit H { get; set; }
        public Canvas Canvas { get; set; }
        #endregion

        #region Properties accepting Styles
        public BaseElementStyle Style { get; set; }
        public XColor Color
        {
            get => _color ?? Style.Color ?? _parentColor ?? XColors.Black;
            set => _color = value;
        }
        public XColor BgColor
        {
            get => _bgColor ?? Style.BgColor ?? _parentBgColor ?? XColors.Transparent;
            set => _bgColor = value;
        }
        public XUnit MarginTop
        {
            get => _marginTop ?? Style.MarginTop ?? XUnit.Zero;
            set => _marginTop = value;
        }
        public XUnit MarginRight
        {
            get => _marginRight ?? Style.MarginRight ?? XUnit.Zero;
            set => _marginRight = value;
        }
        public XUnit MarginBottom
        {
            get => _marginBottom ?? Style.MarginBottom ?? XUnit.Zero;
            set => _marginBottom = value;
        }
        public XUnit MarginLeft
        {
            get => _marginLeft ?? Style.MarginLeft ?? XUnit.Zero;
            set => _marginLeft = value;
        }
        public XUnit PaddingTop
        {
            get => _paddingTop ?? Style.PaddingTop ?? XUnit.Zero;
            set => _paddingTop = value;
        }
        public XUnit PaddingRight
        {
            get => _paddingRight ?? Style.PaddingRight ?? XUnit.Zero;
            set => _paddingRight = value;
        }
        public XUnit PaddingBottom
        {
            get => _paddingBottom ?? Style.PaddingBottom ?? XUnit.Zero;
            set => _paddingBottom = value;
        }
        public XUnit PaddingLeft
        {
            get => _paddingLeft ?? Style.PaddingLeft ?? XUnit.Zero;
            set => _paddingLeft = value;
        }
        public string Height
        {
            get => _height ?? Style.Height ?? "";
            set => _height = value;
        }
        public string Width
        {
            get => _width ?? Style.Width ?? "";
            set => _width = value;
        }
        public bool Rotate
        {
            get => _rotate ?? Style.Rotate ?? false;
            set => _rotate = value;
        }
        #endregion

        #region Additional Properties
        public int PageNo { get; set; }
        public XUnit TopAlign { get; set; }
        public XUnit MiddleAlign { get; set; }
        public XUnit BottomAlign { get; set; }
        #endregion

        #region Constructors
        public BaseElement()
        {
            Style = new BaseElementStyle();
            Canvas = new Canvas();
        }
        #endregion

        internal virtual void ParseXml(XElement element)
        {
            _color = XmlParser.ParseColor(element.Attribute("color")?.Value);
            _bgColor = XmlParser.ParseColor(element.Attribute("bgcolor")?.Value);
            (_marginTop, _marginRight, _marginBottom, _marginLeft) = XmlParser.ParseMargin(element.Attribute("margin")?.Value);
            (_paddingTop, _paddingRight, _paddingBottom, _paddingLeft) = XmlParser.ParsePadding(element.Attribute("padding")?.Value);
            _height = element.Attribute("height")?.Value;
            _width = element.Attribute("width")?.Value;
            _rotate = XmlParser.ParseBool(element.Attribute("rotate")?.Value);
        }
        internal virtual void Inherit(BaseElement parent)
        {
            PageNo = parent.PageNo;
            _parentColor = parent.Color;
            _parentBgColor = parent.BgColor;
        }
        internal virtual void Calculate(XGraphics gfx, Canvas parentCanvas)
        {
            X = Y = XUnit.Zero;

            // Width
            if (!string.IsNullOrEmpty(Width))
            {
                if (Width.Contains('%'))
                {
                    double widthPercent;
                    try
                    {
                        widthPercent = double.Parse(Width.TrimEnd(new char[] { '%', ' ' }));
                    }
                    catch
                    {
                        Console.WriteLine($"Invalid width percentage: {Width}. Defaulting to 100%.");
                        widthPercent = 100;
                    }
                    W = parentCanvas.W * widthPercent / 100 - (MarginLeft + MarginRight);
                }
                else
                    W = XmlParser.ParseXUnit(Width) ?? XUnit.Zero;
            }
            else
            {
                W = parentCanvas.W - (MarginLeft + MarginRight);
            }

            // Height
            if (!string.IsNullOrEmpty(Height))
            {
                H = XmlParser.ParseXUnit(Height) ?? XUnit.Zero;
            }
            else
            {
                H = XUnit.Zero;
            }

            this.Canvas = new Canvas
            {
                X = X + PaddingLeft,
                Y = Y + PaddingTop,
                W = W - (PaddingLeft + PaddingRight),
                H = H - (PaddingTop + PaddingBottom)
            };
        }
        internal virtual void Draw(XGraphics gfx)
        {
            LogDraw();
            gfx.DrawRectangle(new XPen(Color, 0.5), new XSolidBrush(BgColor), 0, 0, W.Point, H.Point);
        }
        internal virtual void LogDraw()
        {
            Console.WriteLine($"Drawing {this.GetType().Name} at ({X.Millimeter:F1}, {Y.Millimeter:F1}) with width {W.Millimeter:F1} and height {H.Millimeter:F1}.");
        }

        protected BaseElement? CreateElement(string elementName)
        {
            return elementName switch
            {
                //"banner" => new Image(),
                "frame" => new Frame(),
                "row" => new Row(),
                "column" => new Column(),
                "image" => new Image(),
                "stamp" => new Stamp(),
                "space" => new Space(),
                "text" => new Text(),
                "paragraph" => new Paragraph(),
                "p" => new Paragraph(),
                _ => null
            };
        }

        // Draws a child in its own local coordinate space.
        // The child renders itself at (0,0); we translate (and optionally rotate) here.
        protected void DrawElement(XGraphics gfx, BaseElement elem, bool rotate = false)
        {
            XGraphicsState state = gfx.Save();
            gfx.TranslateTransform(elem.X.Point, elem.Y.Point);
            if (rotate)
                gfx.RotateTransform(90); 
            elem.Draw(gfx);
            gfx.Restore(state);
        }
    }

    internal abstract class BaseElement<TStyle> : BaseElement
        where TStyle : BaseElementStyle
    {
        public new TStyle? Style
        {
            get => (TStyle?)base.Style;
            set => base.Style = value;
        }
    }
}
