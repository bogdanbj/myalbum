using MyAlbum.Models.Xml;
using MyAlbum.Models.Xml.Styles;
using MyAlbum.Services;
using PdfSharpCore.Drawing;

namespace MyAlbum.Models.Layout
{
    internal class Text : BaseElement
    {
        #region Fields
        private string[][] arr;
        string[] sep = { "\\n" };
        private XFont _font;
        #endregion


        public string FontName { get; set; }
        public double FontSize { get; set; }
        public XFontStyle FontStyle { get; set; }
        public bool Justify { get; set; }
        public string Value { get; set; }
        public XFont Font
        {
            get
            {
                if (_font == null)
                   _font = new XFont(FontName, FontSize, FontStyle);
                return _font;
            }
            private set
            {
                _font = value;
            }
        }
        public XBrush Brush
        {
            get
            {
                return new XSolidBrush(Color);
            }
        }


        public Text()
        {
            W = XUnit.Zero;
            H = XUnit.Zero;
        }

        internal void FromXml(XmlText xmlText, AlbumStyles styles)
        {
            // Find the appropriate text style
            TextStyle? style = StyleFactory.FindStyle<TextStyle>(xmlText.Style, styles.TextStyles);
            if (style == null)
            {
                throw new InvalidOperationException(
                    $"Text style '{xmlText.Style ?? "(default)"}' not found. " +
                    $"Ensure a matching TextStyle exists in the album styles or that a default TextStyle is defined.");
            }
            base.FromXml(xmlText, style);

            // Apply the style
            try
            {
                Color = ParseColor(xmlText.Color ?? style.Color ?? $"{Color.R},{Color.G},{Color.B}");
                BgColor = ParseColor(xmlText.BgColor ?? style.BgColor ?? $"{BgColor.R},{BgColor.G},{BgColor.B}");
                Align = ParseAlignment(xmlText.Align ?? style.Align ?? "left");
                VAlign = ParseVerticalAlignment(xmlText.VAlign ?? style.VAlign ?? "top");

                FontName = xmlText.FontName ?? style.FontName ?? "Verdana";
                FontSize = xmlText.FontSize != 0 ? xmlText.FontSize : style.FontSize != 0 ? style.FontSize : 12;
                FontStyle = ParseFontStyle(xmlText.FontStyle ?? style.FontStyle ?? "Regular");
                Font = new XFont(FontName, FontSize, FontStyle);
                Justify = bool.TryParse(xmlText.Justify ?? style.Justify, out var result) ? result : false;
                //Justify = bool.Parse(xmlText.Justify ?? style.Justify ?? "false");
                //Justify = !string.IsNullOrEmpty(xmlText.Justify)
                //            ? bool.Parse(xmlText.Justify)
                //            : !string.IsNullOrEmpty(style.Justify)
                //                ? bool.Parse(style.Justify)
                //                : false;
            }
            catch
            {
                throw;
            }
            WidthPercent = 100;
            if (xmlText.Width != null)
            {
                try
                {
                    string width = xmlText.Width;
                    if (width.Contains('%'))
                    {
                        WidthPercent = double.Parse(width.TrimEnd(new char[] { '%', ' ' }));
                    }
                    else
                    {
                        W = ParseXUnit(width);
                    }
                }
                catch (Exception)
                { }
            }

            Value = xmlText.Value;
        }
        internal override void CalculateSize(XGraphics gfx, XUnit w, XUnit h)
        {
            // h is not used for text, as height is determined by the number of lines and font size
            // if W is zero, then inherit the parent's canvas width
            if (W == XUnit.Zero) 
            { 
                W = w * WidthPercent / 100; 
            }

            
            if (!string.IsNullOrEmpty(Value))
            {
                arr = SplitText(gfx);

                for (int i = 0; i < arr.Length; i++)
                {
                    H += XUnit.FromPoint(this.Font.Height);
                }
            }
            if (H > XUnit.Zero)
            {
                H += MarginTop + MarginBottom;
            }

            TopAlign = XUnit.Zero;
            MiddleAlign = H / 2;
            BottomAlign = H;
        }
        internal override void CalculateInnerPositions()
        {
            //text does not have inner elements
            return;
        }
        internal override void Draw(XGraphics gfx)
        {
            try
            {
                // TEST : fill canvas
                //XBrush textBgBrush = new XSolidBrush(TextBgColor);
                //gfx.DrawRectangle(textBgBrush, X, Y, W, H);

                XPoint startPoint;

                VAlign = VerticalAlignment.Top;


                //DrawBackground();
                //DrawBox();
                //DrawCross(new XPoint(x, y), XColors.CadetBlue);

                if (!string.IsNullOrEmpty(this.Value))
                {
                    for (int i = 0; i < arr.Length; i++)
                    {
                        XStringFormat format = new XStringFormat();

                        //if ((this.Justify) && (i < arr.Length - 1))
                        if ((this.Justify) && (arr[i][0] != "LAST"))
                        {
                            startPoint = RowStartPoint(i, true);
                            string[] words = arr[i][1].Split();
                            double wordsWidth = 0;
                            double spaceWidth = 0;
                            double space;
                            for (int j = 0; j < words.Length; j++)
                            {
                                wordsWidth += gfx.MeasureString(words[j], Font).Width;
                            }
                            spaceWidth = this.W - wordsWidth;
                            space = spaceWidth / (words.Length - 1);
                            for (int j = 0; j < words.Length; j++)
                            {
                                format.Alignment = XStringAlignment.Near;
                                gfx.DrawString(words[j], Font, Brush, (XPoint)startPoint, format);
                                startPoint.X += gfx.MeasureString(words[j], Font).Width + space;
                            }

                        }
                        else
                        {
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
                            gfx.DrawString(arr[i][1], Font, this.Brush, startPoint, format);
                        }

                    }
                }
            }
            catch { 
                throw; 
            }
        }
        private XFontStyle ParseFontStyle(string fontStyle)
        {
            if (!string.IsNullOrWhiteSpace(fontStyle) && Enum.TryParse(fontStyle, true, out XFontStyle result))
            {
                return result;
            }
            return default;
        }
        private string[][] SplitText(XGraphics gfx)
        {
            try
            {
                //needsProcess = false;
                string[][] result = { };

                //string[] arr = Value.Split(sep, StringSplitOptions.RemoveEmptyEntries);
                string[] arr = Value.Split(sep, StringSplitOptions.None);
                string[] words;
                string line;
                for (int i = 0; i < arr.Length; i++)
                {
                    if (result.Length > 0)
                    {
                        result[result.Length - 1][0] = "LAST";
                    }
                    //words = arr[i].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    words = arr[i].Split(new char[] { ' ' }, StringSplitOptions.None);
                    line = words[0];
                    for (int j = 1; j < words.Length; j++)
                    {
                        if (gfx.MeasureString(line + " " + words[j], Font).Width < this.W)
                        {
                            line += (" " + words[j]);
                        }
                        else
                        {
                            Array.Resize(ref result, result.Length + 1);
                            //result.SetValue(line, result.Length - 1);
                            result.SetValue(new String[2] { "MID", line }, result.Length - 1);
                            line = words[j];
                        }
                    }
                    Array.Resize(ref result, result.Length + 1);
                    result.SetValue(new String[2] { "MID", line }, result.Length - 1);
                }

                result[result.Length - 1][0] = "LAST";
                this.arr = result;

                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }
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
