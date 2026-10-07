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
        private XFont _font;
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
            

            //PageNumber = int.Parse(pageElement.Attribute("no")?.Value ?? "0");
        }
        internal override void Calculate(XGraphics gfx, Canvas parentCanvas)
        {
            base.Calculate(gfx, parentCanvas);

            // Text does not wrap. Only Paragraph does. For the text element, the user might break the line with \n.
            if (!string.IsNullOrEmpty(Value))
            {
                lines = Split(gfx);
                H = CalculateHeight(gfx);
                W = CalculateWidth(gfx);
            }
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
            XUnit h = XUnit.Zero;
            for (int i = 0; i < lines.Length; i++)
            {
                h += XUnit.FromPoint(this.Font.Height);
            }
            h += MarginTop + MarginBottom;
            return h;
        }

        protected virtual XUnit CalculateWidth(XGraphics gfx)
        {
            XUnit w = XUnit.Zero;
            for (int i = 0; i < lines.Length; i++)
            {
                w = Math.Max(w, gfx.MeasureString(lines[i], this.Font).Width);
            }
            w += MarginLeft + MarginRight;
            return w;
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
        //private string[][] SplitText(XGraphics gfx)
        //{
        //    try
        //    {
        //        //needsProcess = false;
        //        string[][] result = { };

        //        //string[] arr = Value.Split(sep, StringSplitOptions.RemoveEmptyEntries);
        //        string[] arr = Value.Split(sep, StringSplitOptions.None);
        //        //string[] words;
        //        string line;
        //        for (int i = 0; i < arr.Length; i++)
        //        {
        //            if (result.Length > 0)
        //            {
        //                result[result.Length - 1][0] = "LAST";
        //            }
        //            //words = arr[i].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        //            words = arr[i].Split(new char[] { ' ' }, StringSplitOptions.None);
        //            line = words[0];
        //            for (int j = 1; j < words.Length; j++)
        //            {
        //                if (gfx.MeasureString(line + " " + words[j], Font).Width < this.W)
        //                {
        //                    line += (" " + words[j]);
        //                }
        //                else
        //                {
        //                    Array.Resize(ref result, result.Length + 1);
        //                    //result.SetValue(line, result.Length - 1);
        //                    result.SetValue(new String[2] { "MID", line }, result.Length - 1);
        //                    line = words[j];
        //                }
        //            }
        //            Array.Resize(ref result, result.Length + 1);
        //            result.SetValue(new String[2] { "MID", line }, result.Length - 1);
        //        }

        //        result[result.Length - 1][0] = "LAST";
        //        this.arr = result;

        //        return result;
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}
        private XPoint RowStartPoint(int index, bool isJustified = false)
        {
            XPoint point = new XPoint();
            switch (VAlign)
            {
                case VerticalAlignment.Top:
                    switch (Align)
                    {
                        case Alignment.Left:
                            //point = new XPoint(this.X,
                            //                   this.Y + this.MarginTop + index * this.Font.Height);
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
            if (isJustified) { point.X = this.X.Point; }

            //DrawCross(point, XColors.Brown);
            return point;
        }
    }
}
