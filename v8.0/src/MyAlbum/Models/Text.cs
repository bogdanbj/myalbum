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
        private bool? _wrap;
        private bool? _justify;
        protected readonly record struct TextLine(string Text, bool Justify);
        protected TextLine[] lines = Array.Empty<TextLine>();
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
        public bool Wrap
        {
            get => _wrap ?? Style.Wrap ?? false;
            set => _wrap = value;
        }
        public bool Justify
        {
            get => _justify ?? Style.Justify ?? false;
            set => _justify = value;
        }
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
            _wrap = XmlParser.ParseBool(xElem.Attribute("wrap")?.Value);
            _justify = XmlParser.ParseBool(xElem.Attribute("justify")?.Value);

            Value = xElem.Value;
        }
        internal override void Calculate(XGraphics gfx, Canvas parentCanvas)
        {
            // base.Calculate initializes the X, Y, W, H and the Canvas. 
            // It is important if Text has explicit width or height.
            base.Calculate(gfx, parentCanvas);
            
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
        //protected virtual string[] Split(XGraphics gfx)
        //{
        //    if (string.IsNullOrEmpty(Value))
        //        return new string[] { "" };

        //    return Value.Split(sep, StringSplitOptions.None);
        //}
        protected TextLine[] Split(XGraphics gfx)
        {
            if (string.IsNullOrEmpty(Value))
                return [new TextLine("", false)];

            if (!Wrap)
                return Value.Split(sep, StringSplitOptions.None)
                            .Select(l => new TextLine(l, false))
                            .ToArray();

            var result = new List<TextLine>();

            // Explicit line breaks
            string[] longLines = Value.Split(sep, StringSplitOptions.None);
            
            // Try to wrap if needed
            foreach (string longLine in longLines)
            {
                // If the line fits, it's a segment-end line -> don't justify.
                if (gfx.MeasureString(longLine, Font).Width <= Canvas.W)
                {
                    result.Add(new TextLine(longLine, false));
                    continue;
                }

                // If the line is longer than Width, wrap it.
                string shortLine = string.Empty;
                foreach (string word in longLine.Split(' '))
                {
                    string candidate = shortLine.Length == 0 ? word : $"{shortLine} {word}";

                    if (gfx.MeasureString(candidate, Font).Width <= Canvas.W)
                    {
                        // Fits on the current line.
                        shortLine = candidate;
                    }
                    else
                    {
                        result.Add(new TextLine(shortLine, Justify && shortLine.Contains(' ')));
                        shortLine = word;
                    }
                }
                result.Add(new TextLine(shortLine, false));
            }
            return result.ToArray();
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
                w = Math.Max(w, gfx.MeasureString(lines[i].Text, this.Font).Width);
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
                            if (lines[i].Justify)
                            {
                                DrawJustifiedLine(gfx, lines[i].Text, RowStartPoint(i, true).Y);
                                continue;
                            }
                            //TODO: not "else"?
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
                            gfx.DrawString(lines[i].Text, Font, this.Brush, startPoint, format);
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

        private void DrawJustifiedLine(XGraphics gfx, string line, double y)
        {
            string[] words = line.Split(' ');

            double wordsWidth = 0;
            foreach (string word in words)
                wordsWidth += gfx.MeasureString(word, Font).Width;

            int gaps = words.Length - 1;
            double gapWidth = (Canvas.W.Point - wordsWidth) / gaps;

            var format = new XStringFormat
            {
                Alignment = XStringAlignment.Near,
                LineAlignment = XLineAlignment.Near
            };

            double x = 0;
            foreach (string word in words)
            {
                gfx.DrawString(word, Font, this.Brush, new XPoint(x, y), format);
                x += gfx.MeasureString(word, Font).Width + gapWidth;
            }
        }
        private XPoint RowStartPoint(int index, bool isJustified = false)
        {
            // X depends only on horizontal alignment.
            double x = Align switch
            {
                Alignment.Left => 0,
                Alignment.Center => this.W.Point / 2,
                Alignment.Right => this.W.Point,
                _ => 0
            };

            // Y depends only on vertical alignment (and the line index).
            double y = VAlign switch
            {
                VerticalAlignment.Top => this.MarginTop.Point + index * this.Font.Height,
                VerticalAlignment.Center => this.MarginTop.Point - this.H.Point / 2 + (index + 0.5) * this.Font.Height,
                VerticalAlignment.Bottom => this.MarginTop.Point - this.H.Point + (index + 1) * this.Font.Height,
                _ => this.MarginTop.Point + index * this.Font.Height
            };

            // Justified lines always start at the content's left edge.
            if (isJustified)
                x = 0;

            return new XPoint(x, y);
        }
    }
}
