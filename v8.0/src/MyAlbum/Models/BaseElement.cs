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
        protected string? _rem;

        #endregion

        #region Core Properties
        public string? StyleName { get; set; }
        public XUnit X { get; set; }
        public XUnit Y { get; set; }
        public XUnit W { get; set; }
        public XUnit H { get; set; }
        public Canvas Canvas { get; set; }
        #endregion

        //#region Hooks: IsOutboundSize → YES
        //// Overridable hooks: subclasses can disable padding/margin entirely.
        //protected virtual bool IsOutboundSize => true;
        //#endregion

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
        //public XUnit PaddingTop
        //{
        //    get => HasPadding ? (_paddingTop ?? Style.PaddingTop ?? XUnit.Zero) : XUnit.Zero;
        //    set => _paddingTop = value;
        //}
        //public XUnit PaddingRight
        //{
        //    get => HasPadding ? (_paddingRight ?? Style.PaddingRight ?? XUnit.Zero) : XUnit.Zero;
        //    set => _paddingRight = value;
        //}
        //public XUnit PaddingBottom
        //{
        //    get => HasPadding ? (_paddingBottom ?? Style.PaddingBottom ?? XUnit.Zero) : XUnit.Zero;
        //    set => _paddingBottom = value;
        //}
        //public XUnit PaddingLeft
        //{
        //    get => HasPadding ? (_paddingLeft ?? Style.PaddingLeft ?? XUnit.Zero) : XUnit.Zero;
        //    set => _paddingLeft = value;
        //}
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
        public string Indent { get; set; }
        public string Rem
        {
            get => _rem ?? "";
            set => _rem = value;
        }
        internal static bool IsDebug { get; set; }
        #endregion

        #region Constructors
        public BaseElement()
        {
            Style = new BaseElementStyle();
            Canvas = new Canvas();
        }
        #endregion

        #region Virtual methods
        internal virtual void ParseXml(XElement element)
        {
            _color = XmlParser.ParseColor(element.Attribute("color")?.Value);
            _bgColor = XmlParser.ParseColor(element.Attribute("bgcolor")?.Value);
            (_marginTop, _marginRight, _marginBottom, _marginLeft) = XmlParser.ParseMargin(element.Attribute("margin")?.Value);
            //(_paddingTop, _paddingRight, _paddingBottom, _paddingLeft) = XmlParser.ParsePadding(element.Attribute("padding")?.Value);
            _height = element.Attribute("height")?.Value;
            _width = element.Attribute("width")?.Value;
            _rotate = XmlParser.ParseBool(element.Attribute("rotate")?.Value);
            _rem = element.Attribute("rem")?.Value;
        }
        internal virtual void Inherit(BaseElement parent)
        {
            PageNo = parent.PageNo;
            _parentColor = parent.Color;
            _parentBgColor = parent.BgColor;
            Indent = parent.Indent + "  ";
        }
        internal virtual void Calculate(XGraphics gfx, Canvas parentCanvas)
        {
            // BaseElement assumes that width and height is total w/h, including margins.
            // This fits Page and Border behavior. The rest will need to override.

            X = Y = XUnit.Zero;

            W = CalculateWidth(parentCanvas.W);

            H = CalculateHeight(parentCanvas.H);

            // Canvas
            Canvas.X = MarginLeft;
            Canvas.Y = MarginTop;
            Canvas.W = W - (MarginLeft + MarginRight);
            Canvas.H = H - (MarginTop + MarginBottom);
        }
        internal virtual XUnit CalculateWidth(XUnit parentWidth)
        {
            if (string.IsNullOrEmpty(Width))
                return XUnit.Zero;

            // Percentage width (e.g. "50%") → relative to parent width
            if (Width.Contains('%'))
            {
                if (!double.TryParse(Width.TrimEnd('%', ' '), out double widthPercent))
                {
                    Console.WriteLine($"Invalid width percentage: {Width}. Defaulting to 100%.");
                    widthPercent = 100;
                }
                return parentWidth * widthPercent / 100;
            } 

            // Return width
            return XmlParser.ParseXUnit(Width) ?? XUnit.Zero;
        }
        internal virtual XUnit CalculateHeight(XUnit parentHeight)
        {
            //return string.IsNullOrEmpty(Height)
            //    ? XUnit.Zero
            //    : XmlParser.ParseXUnit(Height) ?? XUnit.Zero;


            if (string.IsNullOrEmpty(Height))
                return XUnit.Zero;

            // Percentage height (e.g. "50%") → relative to parent height
            if (Height.Contains('%'))
            {
                if (!double.TryParse(Height.TrimEnd('%', ' '), out double heightPercent))
                {
                    Console.WriteLine($"Invalid height percentage: {Height}. Defaulting to 100%.");
                    heightPercent = 100;
                }
                return parentHeight * heightPercent / 100;
            }

            // Return height
            return XmlParser.ParseXUnit(Height) ?? XUnit.Zero;
        }
        internal virtual void Draw(XGraphics gfx)
        {
            if (IsDebug)
            {
                LogDraw();
                gfx.DrawRectangle(new XPen(Color, 0.5), new XSolidBrush(BgColor), 0, 0, W.Point, H.Point);
            }
        }
        internal virtual void LogDraw()
        {
            Console.WriteLine($"{Indent}Drawing {this.GetType().Name} at ({X.Millimeter:F1}, {Y.Millimeter:F1}) with width {W.Millimeter:F1} and height {H.Millimeter:F1}.");
        }
        #endregion

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
