using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Xml.Linq;
using PdfSharp.Drawing;

namespace MyAlbum3
{
    class Text : BaseElement
    {
        #region Fields
        private string[][] arr;
        string[] sep = { "\\n" };
        private XFont _font;
        //private XBrush _brush;
        #endregion
        
        #region Properties
        public string FontName { get; set; }
        public double FontSize { get; set; }
        public XFontStyle FontStyle { get; set; }
        public bool Justify { get; set; }
        public string Value { get; set; }
        public XFont Font
        {
            get
            {
                return new XFont(FontName, FontSize, FontStyle);
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
        #endregion

        #region Constructors
        public Text()
        {
            BoxColor = XColors.Orange;
        }
        public Text(XElement xml) : this()
        {
            Xml = xml;
        }
        #endregion

        #region Public Methods
        public override void Parse()
        {
            if (Parent != null) { Inherit(); }
            Text style = FindStyle();
            ApplyStyle(style);
            ParseAttributes();
        }
        public override void Calculate()
        {
            h = XUnit.Zero;
            if (!string.IsNullOrEmpty(this.Value))
            {
                h = this.MarginTop + this.MarginBottom;

                if (w == XUnit.Zero)
                {
                    w = GetWidth(this.Parent) * this.WidthPercent / 100;
                }
                arr = Split();

                for (int i = 0; i < arr.Length; i++)
                {
                    h += XUnit.FromPoint(this.Font.Height);
                }
            }
            TopAlign = XUnit.Zero;
            MiddleAlign = h / 2;
            BottomAlign = h;
        }
        public override void Draw()
        {
            XPoint startPoint;

            VAlign = Styles.VerticalAlignments.Top;


            DrawBackground();
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
                        spaceWidth = this.w.Point - wordsWidth;
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
                            case Styles.Alignments.Left:
                                format.Alignment = XStringAlignment.Near;
                                break;
                            case Styles.Alignments.Center:
                                format.Alignment = XStringAlignment.Center;
                                break;
                            case Styles.Alignments.Right:
                                format.Alignment = XStringAlignment.Far;
                                break;
                        }
                        gfx.DrawString(arr[i][1], Font, this.Brush, startPoint, format);
                    }

                }
            }
        }
        #endregion

        #region Private Methods
        private void Inherit()
        {
            if (Parent != null)
            {
                this.VAlign = Parent.VAlign;
                this.Color = Parent.Color;
            }

        }
        private Text FindStyle()
        {
            Text style = null;

            // use specific style
            if (Xml.Attribute("style") != null)
            {
                style = Styles.TextStyles.Where(t => t.Style == Xml.Attribute("style").Value).FirstOrDefault();

            }

            // use default
            if (style == null)
            {
                style = Styles.TextStyles.Where(t => t.IsDefault == true).FirstOrDefault();
            }

            return style;

        }
        public void ApplyStyle(Text styleText)
        {
            if (styleText != null)
            {
                Style = styleText.Style;
                FontName = styleText.FontName;
                FontSize = styleText.FontSize;
                FontStyle = styleText.FontStyle;
                Justify = styleText.Justify;
                Align = styleText.Align;
                if (styleText.VAlign != Styles.VerticalAlignments.NotSet)
                {
                    VAlign = styleText.VAlign;
                }
                MarginLeft = styleText.MarginLeft;
                MarginRight = styleText.MarginRight;
                MarginTop = styleText.MarginTop;
                MarginBottom = styleText.MarginBottom;
                if (!styleText.Color.IsEmpty)
                {
                    Color = styleText.Color;
                }
                //Value = styleText.Value;
                w = styleText.w;
            }

        }
        private void ParseAttributes()
        {
            // base element attribute
            ParseBaseAttribute();

            // justify attribute
            ParseJustifyAttribute();

            // font_name attribute
            ParseFontNameAttribute();

            // font_size attribute
            ParseFontSizeAttribute();

            // font_style attribute
            ParseFontStyleAttribute();

            // value
            ParseValueAttribute();
        }
        private void ParseJustifyAttribute()
        {
            if (Xml.Attribute("justify") != null)
            {
                if (Xml.Attribute("justify").Value.ToLower() == "true")
                {
                    this.Justify = true;
                }
            }
        }
        private void ParseFontNameAttribute()
        {
            if (Xml.Attribute("font_name") != null)
            {
                this.FontName = Xml.Attribute("font_name").Value;
            }
        }
        private void ParseFontSizeAttribute()
        {
            if (Xml.Attribute("font_size") != null)
            {
                try
                {
                    this.FontSize = int.Parse(Xml.Attribute("font_size").Value);
                }
                catch (Exception)
                {
                    this.FontSize = 8;
                }
            }
        }
        private void ParseFontStyleAttribute()
        {
            if (Xml.Attribute("font_style") != null)
            {
                string fs = Xml.Attribute("font_style").Value.ToLower();
                switch (fs)
                {
                    case "bold":
                        this.FontStyle = XFontStyle.Bold;
                        break;
                    case "bolditalic":
                        this.FontStyle = XFontStyle.BoldItalic;
                        break;
                    case "italic":
                        this.FontStyle = XFontStyle.Italic;
                        break;
                    case "regular":
                        this.FontStyle = XFontStyle.Regular;
                        break;
                    case "strikeout":
                        this.FontStyle = XFontStyle.Strikeout;
                        break;
                    case "underline":
                        this.FontStyle = XFontStyle.Underline;
                        break;
                    default:
                        this.FontStyle = XFontStyle.Regular;
                        break;
                }

            }
        }
        private void ParseValueAttribute()
        {
            if (Xml.Attribute("value") != null) { this.Value = Xml.Attribute("value").Value; }
            if (!string.IsNullOrEmpty(Xml.Value)) 
            {
                //System.Xml.XmlReader xReader = Xml.CreateReader();
                //xReader.MoveToContent();
                //xReader.ReadInnerXml();
                //this.Value = xReader.ReadInnerXml(); 
                this.Value = Xml.Value; 
            }
        }
        private string[][] Split()
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
                    if (gfx.MeasureString(line + " " + words[j], Font).Width < this.w)
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
            

            //if (CanGrow)
            //{
            //    this.H = XUnit.FromPoint(this.TextHeight);
            //}
            //else
            //{
            //    while (this.TextHeight >= this.H)
            //    {
            //        Array.Resize(ref result, result.Length - 1);
            //        this.arr = result;
            //    }
            //}

            return result;

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        private XPoint RowStartPoint(int index, bool isJustified = false)
        {
            XPoint point = new XPoint();
            switch (VAlign)
            {
                case Styles.VerticalAlignments.Top:
                    switch (Align)
                    {
                        case Styles.Alignments.Left:
                            point = new XPoint(this.x,
                                               this.y + this.MarginTop + index * this.Font.Height);
                            break;
                        case Styles.Alignments.Center:
                            point = new XPoint(this.x + this.w / 2,
                                               this.y + this.MarginTop + index * this.Font.Height);
                            break;
                        case Styles.Alignments.Right:
                            point = new XPoint(this.x + this.w,
                                               this.y + this.MarginTop + index * this.Font.Height);
                            break;
                    }
                    break;
                case Styles.VerticalAlignments.Center:
                    switch (Align)
                    {
                        case Styles.Alignments.Left:
                            point = new XPoint(this.x,
                                               this.y + this.MarginTop - this.h / 2 + (index + 0.5) * this.Font.Height);
                            break;
                        case Styles.Alignments.Center:
                            point = new XPoint(this.x + this.w / 2,
                                               this.y + this.MarginTop - this.h / 2 + (index + 0.5) * this.Font.Height);
                            break;
                        case Styles.Alignments.Right:
                            point = new XPoint(this.x + this.w,
                                               this.y + this.MarginTop - this.h / 2 + (index + 0.5) * this.Font.Height);
                            break;
                    }
                    break;
                case Styles.VerticalAlignments.Bottom:
                    switch (Align)
                    {
                        case Styles.Alignments.Left:
                            point = new XPoint(this.x,
                                               this.y + this.MarginTop - this.h + (index + 1) * this.Font.Height);
                            break;
                        case Styles.Alignments.Center:
                            point = new XPoint(this.x + this.w / 2,
                                               this.y + this.MarginTop - this.h + (index + 1) * this.Font.Height);
                            break;
                        case Styles.Alignments.Right:
                            point = new XPoint(this.x + this.w,
                                               this.y + this.MarginTop - this.h + (index + 1) * this.Font.Height);
                            break;
                    }
                    break;
            }
            if (isJustified) { point.X = this.x; }

            //DrawCross(point, XColors.Brown);
            return point;
        }
        #endregion
    }
}
