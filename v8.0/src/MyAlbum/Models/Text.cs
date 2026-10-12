using MyAlbum.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using PdfSharp.Drawing;
using System.Configuration;

namespace MyAlbum.Models
{
    internal class Text : BaseElement<TextStyle>
    {
        #region Fields
        protected Alignment? _align;
        protected string? _fontName;
        protected double? _fontSize;
        protected XFontStyleEx? _fontStyle;
        private XFont? _font;
        protected string[] lines = Array.Empty<string>();
        protected string[] sep = { "\\n" };
        #endregion

        #region Style properties
        public Alignment Align
        {
            get => _align ?? Style.Align ?? Alignment.Center;
            set => _align = value;
        }
        public VerticalAlignment VAlign { get; } = VerticalAlignment.Top;
        public string FontName 
        { 
            get => _fontName ?? Style.FontName ?? "Verdana"; 
            set => _fontName = value;
        }
        public double FontSize
        {
            get => _fontSize ?? Style.FontSize ?? 12; 
            set => _fontSize = value;
        }
        public XFontStyleEx FontStyle 
        { 
            get => _fontStyle ?? Style.FontStyle ?? XFontStyleEx.Regular;
            set => _fontStyle = value;
        }
        public XFont Font
        {
            get 
            { 
                if (_font == null)
                {
                    _font = new XFont(FontName, FontSize, FontStyle);
                }
                return _font; 
            }
            set => _font = value; 
        }
        public XBrush Brush
        {
            get
            {
                return new XSolidBrush(Color);
            }
        }
        public string? Value { get; set; }
        #endregion

        #region Constructors
        public Text()
        {
            Style = Styles.Text.GetStyle("default") ?? new TextStyle();
        }
        #endregion

        #region Override methods
        internal override void ParseXml(XElement xElem)
        {
            base.ParseXml(xElem);
            Style = Styles.Text.GetStyle(xElem.Attribute("style")?.Value);

            _align = XmlParser.ParseAlignment(xElem.Attribute("align")?.Value);
            _fontName = xElem.Attribute("font-name")?.Value;
            _fontSize = XmlParser.ParseDouble(xElem.Attribute("font-size")?.Value);
            _fontStyle = XmlParser.ParseFontStyle(xElem.Attribute("font-style")?.Value);
            Font = new XFont(FontName, FontSize, FontStyle);

            Value = xElem.Value;
        }
        internal override void Calculate(XGraphics gfx, Canvas parentCanvas)
        {
            // base.Calculate initializes the X, Y, W, H and the Canvas. 
            // It is important if Text has explicit width or height.
            base.Calculate(gfx, parentCanvas);
            
            // Text does not wrap. Only Paragraph does.
            // For the text element, the user could break the line with "\n".
            if (!string.IsNullOrEmpty(Value))
            {
                lines = Split(gfx);
            }

            // Calculate the actual text size and get the larger between explicit and calculated.
            Canvas.W = XUnit.FromPoint( Math.Max(Canvas.W.Point, CalculateWidth(gfx).Point) ); 
            Canvas.H = XUnit.FromPoint( Math.Max(Canvas.H.Point, CalculateHeight(gfx).Point) );

            // Reset the total W and H.
            W = Canvas.W + (MarginLeft + MarginRight);
            H = Canvas.H + (MarginTop + MarginBottom);
        }

        /* 
         * XGraphics gfx parameter is not used here. this is intentional 
         * The Split method is designed to be overridden in derived classes, 
         * where the gfx parameter will be used for word wrapping
         */
        protected virtual string[] Split(XGraphics gfx)
        {
            if (string.IsNullOrEmpty(Value))
                return new string[] { "" };

            return Value.Split(sep, StringSplitOptions.None);
        }
        protected virtual XUnit CalculateHeight(XGraphics gfx)
        {
            double h = 0;
            for (int i = 0; i < lines.Length; i++)
            {
                h += this.Font.Height;
            }
            return XUnit.FromPoint(h);
        }
        protected virtual XUnit CalculateWidth(XGraphics gfx)
        {
            double w = 0;
            for (int i = 0; i < lines.Length; i++)
            {
                w = Math.Max(w, gfx.MeasureString(lines[i], this.Font).Width);
            }
            return XUnit.FromPoint(w);
        }
        internal override void Draw(XGraphics gfx)
        {
            base.Draw(gfx);
            try
            {
                XPoint startPoint;

                if (!string.IsNullOrEmpty(this.Value))
                {
                    if (lines != null)
                    {
                        for (int i = 0; i < lines.Length; i++)
                        {
                            XStringFormat format = new XStringFormat();
                            format.LineAlignment = XLineAlignment.Near;
                            startPoint = RowStartPoint(i, false);
                            switch (Align)
                            {
                                case Alignment.Left:
                                    format.Alignment = XStringAlignment.Near;
                                    break;
                                case Alignment.Center:
                                    format.Alignment = XStringAlignment.Center;
                                    break;
                                case Alignment.Right:
                                    format.Alignment = XStringAlignment.Far;
                                    break;
                            }
                            gfx.DrawString(lines[i], Font, this.Brush, startPoint, format);
                        }
                    }
                }
            }
            catch
            {
                throw;
            }
        }
        #endregion

        private XPoint RowStartPoint(int index, bool isJustified = false)
        {
            XPoint point = new XPoint();
            switch (VAlign)
            {
                case VerticalAlignment.Top:
                    switch (Align)
                    {
                        case Alignment.Left:
                            point = new XPoint(0,
                                               this.MarginTop.Point + index * this.Font.Height);
                            break;
                        case Alignment.Center:
                            point = new XPoint(this.W.Point / 2,
                                               this.MarginTop.Point + index * this.Font.Height);
                            break;
                        case Alignment.Right:
                            point = new XPoint(this.W.Point ,
                                               this.MarginTop.Point + index * this.Font.Height);
                            break;
                    }
                    break;
                case VerticalAlignment.Center:
                    switch (Align)
                    {
                        case Alignment.Left:
                            point = new XPoint(0,
                                               this.MarginTop.Point - this.H.Point / 2 + (index + 0.5) * this.Font.Height);
                            break;
                        case Alignment.Center:
                            point = new XPoint(this.W.Point / 2,
                                               this.MarginTop.Point - this.H.Point / 2 + (index + 0.5) * this.Font.Height);
                            break;
                        case Alignment.Right:
                            point = new XPoint(this.W.Point,
                                               this.MarginTop.Point - this.H.Point / 2 + (index + 0.5) * this.Font.Height);
                            break;
                    }
                    break;
                case VerticalAlignment.Bottom:
                    switch (Align)
                    {
                        case Alignment.Left:
                            point = new XPoint(0,
                                               this.MarginTop.Point - this.H.Point + (index + 1) * this.Font.Height);
                            break;
                        case Alignment.Center:
                            point = new XPoint(this.W.Point / 2,
                                               this.MarginTop.Point - this.H.Point + (index + 1) * this.Font.Height);
                            break;
                        case Alignment.Right:
                            point = new XPoint(this.W.Point,
                                               this.MarginTop.Point - this.H.Point + (index + 1) * this.Font.Height);
                            break;
                    }
                    break;
            }
            return point;
        }
    }
}
