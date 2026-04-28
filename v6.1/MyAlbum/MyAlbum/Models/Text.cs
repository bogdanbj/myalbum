using MyAlbum.Utilities;
using SixLabors.Fonts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using PdfSharpCore.Drawing;

namespace MyAlbum.Models
{
    internal class Text : BaseElement<TextStyle>
    {
        #region Fields
        protected Alignment? _align;
        protected string? _fontName;
        protected double? _fontSize;
        protected XFontStyle? _fontStyle;
        private XFont _font;
        private string[] lines;
        string[] sep = { "\\n" };
        #endregion

        #region Style properties
        public Alignment Align
        {
            get => _align ?? Style.Align ?? Alignment.Center;
            set => _align = value;
        }
        public VerticalAlignment VAlign { get; set; } = VerticalAlignment.Top;
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
        public XFontStyle FontStyle 
        { 
            get => _fontStyle ?? Style.FontStyle ?? XFontStyle.Regular;
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
            // Text does not wrap. Only Paragraph does. For the text element, the user might break the line with \n.
            if (!string.IsNullOrEmpty(Value))
            {
                lines = Value.Split(sep, StringSplitOptions.None);

                for (int i = 0; i < lines.Length; i++)
                {
                    H += XUnit.FromPoint(this.Font.Height);
                    W = Math.Max(W, gfx.MeasureString(lines[i], this.Font).Width);
                }
                H += MarginTop + MarginBottom;
                W += MarginLeft + MarginRight;
            }
            //if (H > XUnit.Zero)
            //{
            //    H += MarginTop + MarginBottom;
            //}

            //TopAlign = XUnit.Zero;
            //MiddleAlign = H / 2;
            //BottomAlign = H;
        }
        internal override void Draw(XGraphics gfx)
        {
            base.Draw(gfx);
            try
            {
                // TEST : fill canvas
                //XBrush textBgBrush = new XSolidBrush(TextBgColor);
                //gfx.DrawRectangle(textBgBrush, X, Y, W, H);

                XPoint startPoint;

                //VAlign = VerticalAlignment.Top;


                //DrawBackground();
                //DrawBox();
                //DrawCross(new XPoint(x, y), XColors.CadetBlue);

                if (!string.IsNullOrEmpty(this.Value))
                {
                    for (int i = 0; i < lines.Length; i++)
                    {
                        XStringFormat format = new XStringFormat();

                        ////if ((this.Justify) && (i < arr.Length - 1))
                        //if ((this.Justify) && (arr[i][0] != "LAST"))
                        //{
                        //    startPoint = RowStartPoint(i, true);
                        //    string[] words = arr[i][1].Split();
                        //    double wordsWidth = 0;
                        //    double spaceWidth = 0;
                        //    double space;
                        //    for (int j = 0; j < words.Length; j++)
                        //    {
                        //        wordsWidth += gfx.MeasureString(words[j], Font).Width;
                        //    }
                        //    spaceWidth = this.W - wordsWidth;
                        //    space = spaceWidth / (words.Length - 1);
                        //    for (int j = 0; j < words.Length; j++)
                        //    {
                        //        format.Alignment = XStringAlignment.Near;
                        //        gfx.DrawString(words[j], Font, Brush, (XPoint)startPoint, format);
                        //        startPoint.X += gfx.MeasureString(words[j], Font).Width + space;
                        //    }

                        //}
                        //else
                        //{
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
                        //}

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
                            point = new XPoint(this.X,
                                               this.Y + this.MarginTop + index * this.Font.Height);
                            break;
                        case Alignment.Center:
                            point = new XPoint(this.X + this.W / 2,
                                               this.Y + this.MarginTop + index * this.Font.Height);
                            break;
                        case Alignment.Right:
                            point = new XPoint(this.X + this.W,
                                               this.Y + this.MarginTop + index * this.Font.Height);
                            break;
                    }
                    break;
                case VerticalAlignment.Center:
                    switch (Align)
                    {
                        case Alignment.Left:
                            point = new XPoint(this.X,
                                               this.Y + this.MarginTop - this.H / 2 + (index + 0.5) * this.Font.Height);
                            break;
                        case Alignment.Center:
                            point = new XPoint(this.X + this.W / 2,
                                               this.Y + this.MarginTop - this.H / 2 + (index + 0.5) * this.Font.Height);
                            break;
                        case Alignment.Right:
                            point = new XPoint(this.X + this.W,
                                               this.Y + this.MarginTop - this.H / 2 + (index + 0.5) * this.Font.Height);
                            break;
                    }
                    break;
                case VerticalAlignment.Bottom:
                    switch (Align)
                    {
                        case Alignment.Left:
                            point = new XPoint(this.X,
                                               this.Y + this.MarginTop - this.H + (index + 1) * this.Font.Height);
                            break;
                        case Alignment.Center:
                            point = new XPoint(this.X + this.W / 2,
                                               this.Y + this.MarginTop - this.H + (index + 1) * this.Font.Height);
                            break;
                        case Alignment.Right:
                            point = new XPoint(this.X + this.W,
                                               this.Y + this.MarginTop - this.H + (index + 1) * this.Font.Height);
                            break;
                    }
                    break;
            }
            if (isJustified) { point.X = this.X; }

            //DrawCross(point, XColors.Brown);
            return point;
        }
    }
}
