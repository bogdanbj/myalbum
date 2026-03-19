using MyAlbum.Models.Xml;
using MyAlbum.Models.Xml.Styles;
using PdfSharpCore.Drawing;

//using System.Xml;



namespace MyAlbum.Models.Layout
{
    internal abstract class BaseElement
    {
        public string? Name { get; set; }
        public XUnit X { get; set; }
        public XUnit Y { get; set; }
        public XUnit W{ get; set; }
        public XUnit H { get; set; }
        //public XUnit PaddingTop { get; set; }
        //public XUnit PaddingRight { get; set; }
        //public XUnit PaddingBottom { get; set; }
        //public XUnit PaddingLeft { get; set; }
        public XUnit MarginTop { get; set; }
        public XUnit MarginRight { get; set; }
        public XUnit MarginBottom { get; set; }
        public XUnit MarginLeft { get; set; }
        public Rect Canvas { get; set; }
        public XColor Color { get; set; } = XColors.Black;
        public XColor BgColor { get; set; } = XColors.White;
        public XUnit TopAlign { get; set; }
        public XUnit MiddleAlign { get; set; }
        public XUnit BottomAlign { get; set; }
        public double WidthPercent { get; set; }
        public Alignment Align{ get; set; }
        public VerticalAlignment VAlign { get; set; }
        public  XUnit VSpace { get; set; }
        public bool Rotate { get; set; }
        public int PageNumber { get; set; }

        protected BaseElement()
        {
            Color = XColors.Black;
            BgColor = XColors.Magenta;//.Fuchsia;
            Canvas = new Rect();
        }
        internal void Inherit(BaseElement parent)
        {
            Color = parent.Color;
            BgColor = parent.BgColor;
            Align = parent.Align;
            VAlign = parent.VAlign;
            VSpace = parent.VSpace;
            PageNumber = parent.PageNumber;
        }

        internal virtual void FromXml(XmlElement xmlElement, XmlElement style)
        {
            Name = xmlElement.Name;
            (MarginTop, MarginRight, MarginBottom, MarginLeft) = ParseMargin(xmlElement.Margin ?? style.Margin);
        }
        internal virtual void Calculate(XGraphics gfx, XUnit x, XUnit y, XUnit w, XUnit h)
        {
            CalculateSize(gfx, w, h);
            X = MarginLeft;// + PaddingLeft;
            Y = MarginTop;// + PaddingTop;
            CalculateInnerPositions();
        }
        internal virtual void CalculateSize(XGraphics gfx, XUnit w, XUnit h)
        {
            W = W - MarginLeft - MarginRight;// - PaddingLeft - PaddingRight;
            H = H - MarginTop - MarginBottom;// - PaddingTop - PaddingBottom;
        }
        //internal virtual void CalculatePositions(XGraphics gfx, XUnit x, XUnit y, XUnit w, XUnit h)
        internal virtual void CalculateInnerPositions()
        {
            // this is to calculate the inner elements positions, now that the X and Y of this element are known
            throw new NotImplementedException();
            //X = MarginLeft + PaddingLeft;
            //Y = MarginTop + PaddingTop;
        }
        internal virtual void Draw(XGraphics gfx)
        {
            gfx.DrawRectangle(new XPen(Color), new XSolidBrush(BgColor), X, Y, W, H);
        }

        internal (XUnit marginTop, XUnit marginRight, XUnit marginBottom, XUnit marginLeft) ParseMargin(string margin)
        {
            XUnit marginTop = XUnit.Zero;
            XUnit marginRight = XUnit.Zero;
            XUnit marginBottom = XUnit.Zero;
            XUnit marginLeft = XUnit.Zero;

            if (!string.IsNullOrWhiteSpace(margin))
            {

                string[] arr = margin.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                switch (arr.Length)
                {
                    case 1:
                        marginTop = marginRight = marginBottom = marginLeft = ParseXUnit(arr[0]);
                        break;
                    case 2:
                        marginTop = marginBottom = ParseXUnit(arr[0]);
                        marginLeft = marginRight = ParseXUnit(arr[1]);
                        break;
                    case 4:
                        marginTop = ParseXUnit(arr[0]);
                        marginRight = ParseXUnit(arr[1]);
                        marginBottom = ParseXUnit(arr[2]);
                        marginLeft = ParseXUnit(arr[3]);
                        break;
                }
            }
            return (marginTop, marginRight, marginBottom, marginLeft);
        }
        internal (XUnit paddingTop, XUnit paddingRight, XUnit paddingBottom, XUnit paddingLeft) ParsePadding(string padding)
        {
            XUnit paddingTop = XUnit.Zero;
            XUnit paddingRight = XUnit.Zero;
            XUnit paddingBottom = XUnit.Zero;
            XUnit paddingLeft = XUnit.Zero;

            if (!string.IsNullOrWhiteSpace(padding))
            {

                string[] arr = padding.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                switch (arr.Length)
                {
                    case 1:
                        paddingTop = paddingRight = paddingBottom = paddingLeft = ParseXUnit(arr[0]);
                        break;
                    case 2:
                        paddingTop = paddingBottom = ParseXUnit(arr[0]);
                        paddingLeft = paddingRight = ParseXUnit(arr[1]);
                        break;
                    case 4:
                        paddingTop = ParseXUnit(arr[0]);
                        paddingRight = ParseXUnit(arr[1]);
                        paddingBottom = ParseXUnit(arr[2]);
                        paddingLeft = ParseXUnit(arr[3]);
                        break;
                }
            }
            return (paddingTop, paddingRight, paddingBottom, paddingLeft);

        }
        internal XColor ParseColor(string value)
        {
            XColor color = XColors.Transparent;

            if (!string.IsNullOrWhiteSpace(value))
            {

                string[] arr = value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                switch (arr.Length)
                {
                    case 1:                          
                        try
                        {
                            PdfSharpCore.Drawing.XKnownColor knownColor = (PdfSharpCore.Drawing.XKnownColor)Enum.Parse(typeof(PdfSharpCore.Drawing.XKnownColor), arr[0], true);
                            color = XColor.FromKnownColor(knownColor);
                        }
                        catch {}
                        break;
                    case 3:
                        try
                        {
                            byte r = byte.Parse(arr[0]);
                            byte g = byte.Parse(arr[1]);
                            byte b = byte.Parse(arr[2]);
                            color = XColor.FromArgb(r, g, b);
                        }
                        catch {}
                        break;
                }
            }
            return color;
        }
        internal XUnit ParseXUnit(string value)
        {
            return XUnit.FromMillimeter(double.Parse(value));
        }
        internal Alignment ParseAlignment(string value)
        {
            return Enum.TryParse(value, true, out Alignment align) ? align : Alignment.Left;
        }
        internal VerticalAlignment ParseVerticalAlignment(string value)
        {
            return Enum.TryParse(value, true, out VerticalAlignment align) ? align : VerticalAlignment.Top;
        }
        internal SpacingMode ParseSpacingMode(string value)
        {
            return Enum.TryParse(value, true, out SpacingMode spacingMode) ? spacingMode : SpacingMode.FS;
        }

        #region helper and debug functions
        internal virtual XColor GetDebugColor()
        {
            return BgColor; // default fallback
        }
        internal void DrawBackground(XGraphics gfx)
        {
            if (Name?.Contains("test", StringComparison.OrdinalIgnoreCase)==true) // XColor.A == 0 means fully transparent (not set)
            {
                if (BgColor != XColors.Transparent)
                {
                    XBrush brush = new XSolidBrush(BgColor);
                    gfx.DrawRectangle(brush, X, Y, W, H);
                }
            }
        }

        #endregion

    }
    public class Rect
    {
        public Rect()
        { }
        public Rect(XUnit x, XUnit y, XUnit w, XUnit h)
        {
            X = x; Y = y; W = w; H = h;        
        }
        public XUnit X { get; set; }
        public XUnit Y { get; set; }
        public XUnit W { get; set; }
        public XUnit H { get; set; }
    }
}
