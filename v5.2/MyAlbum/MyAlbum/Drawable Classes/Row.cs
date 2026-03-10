using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

using System.Xml.Linq;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using static MyAlbum.Styles;

namespace MyAlbum
{
    internal class Row : DrawableElement
    {
        #region fields
        private RowStyle _style;
        private DrawableElement _canvas;
        private List<DrawableElement> _elements;
        #endregion

        #region properties
        public RowStyle Style
        {
            get 
            {
                if (_style == null)
                    _style = new RowStyle();
                return _style; 
            }
            set { _style = value; }
        }
        public DrawableElement Canvas
        {
            get
            {
                if (_canvas == null) { _canvas = new DrawableElement(); }
                return _canvas;
            }
        }
        public Styles.SpacingModes Spacing { get; set; }
        public XUnitPt Space { get; set; }
        public string Rotate { get; set; }
        public List<DrawableElement> Elements
        {
            get
            {
                if (_elements == null) { _elements = new List<DrawableElement>(); }
                return _elements;
            }
            set { _elements = value; }
        }
        #endregion

        #region constructors
        public Row()
        {
            BoxColor = XColors.Aqua;
        }
        public Row(XElement xml) : this()
        {
            this.xml = xml;
        }
        #endregion

        #region public methods
        public override void Parse()
        {
            ParseAttributes();
            ParseComponents();
        }
        public override void Calculate()
        {
            Canvas.x = this.x + Style.PaddingLeft;
            Canvas.y = this.y + Style.PaddingTop;
            Canvas.w = this.w - (Style.PaddingRight + Style.PaddingLeft);
            Canvas.h = this.h - (Style.PaddingTop + Style.PaddingBottom);
            //h = 50;
            //w = 250;
            
            if (w == XUnitPt.Zero)
            {
                if (Rotate.ToLower() == "true")
                {
                    //w = GetHeight(this.Parent) - (MarginTop + MarginBottom);
                }
                else
                {
                    //w = ((Page)Parent).Canvas.w;// - (MarginLeft + MarginRight);
                }
            }


            //if (Elements.Count==0)
            //{
            //    this.TopAlign = XUnitPt.Zero;
            //    this.MiddleAlign = this.h / 2;
            //    this.BottomAlign = this.h;

            //}
            foreach (BaseElement element in Elements)
            {
                this.h = Math.Max(this.h + 10, this.Style.Height);
            //    element.gfx = gfx;

            //    element.Calculate();

            //    if (element.Absolute == false)
            //    {
            //        this.TopAlign = Math.Max(this.TopAlign, element.TopAlign);
            //        this.MiddleAlign = Math.Max(this.MiddleAlign, element.MiddleAlign);
            //        this.BottomAlign = Math.Max(this.BottomAlign, element.BottomAlign);

            //        switch (VAlign)
            //        {
            //            case Styles.VerticalAlignments.Top:
            //                this.h = Math.Max(this.h, this.TopAlign + element.h - element.TopAlign);
            //                break;
            //            case Styles.VerticalAlignments.Center:
            //                this.h = Math.Max(this.h, this.MiddleAlign + element.h - element.MiddleAlign);
            //                break;
            //            case Styles.VerticalAlignments.Bottom:
            //                this.h = Math.Max(this.h, this.BottomAlign + element.h - element.BottomAlign);
            //                break;
            //            default:
            //                break;
            //        }
            //    }
            }

            //this.h += this.MarginTop + this.MarginBottom;
            Canvas.h = this.h - (Style.PaddingTop + Style.PaddingBottom);

        }
        public override void Draw()
        {
            Canvas.gfx = gfx;
            //x = 0;
            //y = 0;
            //w = 200;
            //h = 50;
            if (Style.Rotate == true)
            {
                gfx.TranslateTransform(w / 2, h / 2);
                gfx.RotateTransform(90);
                gfx.TranslateTransform(-w / 2, -h / 2);
            }
            if (this.Elements.Count == 0) 
            {
                DrawBox(BoxColor, new XPoint(x, y), new XSize(w, h));
            }
            else
            {  
                Canvas.DrawBackground(BoxColor);
                string text = xml.ToString();

                XPdfFontOptions options = new XPdfFontOptions(PdfFontEncoding.Unicode, PdfFontEmbedding.EmbedCompleteFontFile);
                XFont font = new XFont("Stymie Becker Light", 10, XFontStyleEx.Regular, options);
                gfx.DrawString($"{this.xml}",
                            font,
                            XBrushes.Black,
                            //new XRect(Canvas.x, Canvas.y, Canvas.w, Canvas.h),
                            new XRect(Canvas.x, y, Canvas.w, Canvas.h),
                            XStringFormats.TopLeft);

            }
            ////XSolidBrush brush;

            //#region horizontal alignment
            //if (Rotate.ToLower() == "true")
            //{
            //    x = Parent.x + (Parent.w - Parent.h) / 2 + this.MarginLeft;
            //    w = Parent.h - (this.MarginLeft + this.MarginRight);

            //}
            //else
            //{
            //    x = Parent.x + this.MarginLeft;
            //    w = Parent.w - (this.MarginLeft + this.MarginRight);
            //}

            ////brush = new XSolidBrush(XColors.MediumSlateBlue);
            ////gfx.DrawRectangle(brush, x, y, w, h);

            //XUnitPt xPos, yPos;
            //XUnitPt width = XUnitPt.Zero;
            //int elementCount = 0;
            //switch (Spacing)
            //{
            //    case Styles.SpacingModes.FS:
            //        foreach (BaseElement element in Elements)
            //        {
            //            if (element.Absolute == false)
            //            {
            //                width += element.w;
            //                elementCount++;
            //            }
            //        }

            //        width += (elementCount - 1) * Space;

            //        xPos = this.x + (w - width) / 2;

            //        foreach (BaseElement element in Elements)
            //        {
            //            if (element.Absolute == false)
            //            {
            //                element.x = xPos;
            //                xPos += element.w + Space;
            //            }
            //        }
            //        break;
            //    case Styles.SpacingModes.ES:
            //        foreach (BaseElement element in Elements)
            //        {
            //            if (element.Absolute == false)
            //            {
            //                width += element.w;
            //                elementCount++;
            //            }
            //        }

            //        Space = (w - width) / (elementCount + 1);

            //        xPos = x + Space;

            //        foreach (BaseElement element in Elements)
            //        {
            //            if (element.Absolute== false)
            //            {
            //                element.x = xPos;
            //                xPos += element.w + Space;
            //            }
            //        }
            //        break;
            //    case Styles.SpacingModes.JS:
            //        if (Elements.Count == 1)
            //        {
            //            if (Elements[0].Absolute == false)
            //            {
            //                Elements[0].x = x + (w - Elements[0].w) / 2;
            //            }
            //        }
            //        else
            //        {
            //            foreach (BaseElement element in Elements)
            //            {
            //                if (element.Absolute == false)
            //                {
            //                    width += element.w;
            //                    elementCount++;
            //                }
            //            }

            //            Space = (w - width) / (elementCount - 1);

            //            xPos = x + Space;

            //            foreach (BaseElement element in Elements)
            //            {
            //                if (element.Absolute == false)
            //                {
            //                    element.x = xPos;
            //                    xPos += element.w + Space;
            //                }
            //            }
            //        }
            //        break;
            //    default:
            //        break;
            //}
            //#endregion
            //DrawBackground();
            ////DrawBox();
            //yPos = this.y + this.MarginTop;
            //foreach (BaseElement element in Elements)
            //{
            //    if (element.Absolute == false)
            //    {
            //        switch (this.VAlign)
            //        {
            //            case Styles.VerticalAlignments.Top:
            //                element.y = yPos + this.TopAlign - element.TopAlign;
            //                break;
            //            case Styles.VerticalAlignments.Center:
            //                element.y = yPos + this.MiddleAlign - element.MiddleAlign;
            //                break;
            //            case Styles.VerticalAlignments.Bottom:
            //                element.y = yPos + this.BottomAlign - element.BottomAlign;
            //                break;
            //        }
            //    }
            //    else
            //    {
            //        if (this.Rotate.ToLower() == "true")
            //        {
            //            //PdfSharp.Pdf.PdfPage page = element.GetPage();
            //            Page page = element.GetPage();
            //            element.x += (page.Width - page.Height) / 2;
            //            element.y += (page.Height - page.Width) / 2;
            //        }
            //    }
            //    element.Draw();
            //}

        }
        #endregion

        #region private methods
        //private void Inherit()
        //{
        //    this.Color = Parent.Color;
        //    //this.VSpace = Parent.VSpace;
        //}
        //private void ApplyStyle()
        //{
        //    Row styleRow = null;

        //    // style
        //    if (Xml.Attribute("style") != null)
        //    {
        //        styleRow = Styles.RowStyles.Where(x => x.Style == Xml.Attribute("style").Value).FirstOrDefault();
        //    }

        //    // use default
        //    if (styleRow == null)
        //    {
        //        styleRow = Styles.RowStyles.Where(x => x.IsDefault == true).FirstOrDefault();
        //    }

        //    // copy style properties
        //    if (styleRow != null)
        //    {
        //        Style = styleRow.Style;
        //        Align = styleRow.Align;
        //        VAlign = styleRow.VAlign;
        //        Spacing = styleRow.Spacing;
        //        Space = styleRow.Space;
        //        MarginLeft = styleRow.MarginLeft;
        //        MarginRight = styleRow.MarginRight;
        //        MarginTop = styleRow.MarginTop;
        //        MarginBottom = styleRow.MarginBottom;
        //        if (!styleRow.BackgroundColor.IsEmpty)    // because is inherited from parent
        //        {
        //            BackgroundColor = styleRow.BackgroundColor;
        //        }
        //        if (!styleRow.Color.IsEmpty)    // because is inherited from parent
        //        {
        //            Color = styleRow.Color;
        //        }
        //        h = styleRow.h;

        //        // copy components
        //        Elements.Clear();
        //        foreach (BaseElement element in styleRow.Elements)
        //        {
        //            Elements.Add(element);
        //        }
        //    }
        //}
        private void ParseAttributes()
        {
            // inherit from parent
            if (Parent != null)
            {
                Style.Color = Parent.Style.Color;
                Style.Brush = Parent.Style.Brush;
            }

            StyleName = ParseStyleNameAttribute(xml);
            if (!string.IsNullOrEmpty(StyleName))
            {
                Style = Styles.RowStyles.FirstOrDefault(s => s.StyleName == StyleName);
            }

            // spacing-mode attribute
            if (xml.Attribute("spacing-mode") != null)
            {
                Style.SpacingMode = ParseSpacingModeAttribute(xml);
            }

            // space attribute
            if (xml.Attribute("space") != null)
            {
                Style.Space = ParseSpaceAttribute(xml);
            }

            // rotate attribute
            if (xml.Attribute("rotate") != null)
            {
                Style.Rotate = ParseRotateAttribute(xml);
            }

            // width attribute
            if (xml.Attribute("width") != null)
            {
                Style.Width = ParseWidthAttribute(xml);
            }

            // height attribute
            if (xml.Attribute("height") != null)
            {
                Style.Height = ParseHeightAttribute(xml);
            }
        }
        //private void ParseSpacingModeAttribute()
        //{
        //    if (Xml.Attribute("spacing-mode") != null)
        //    {
        //        switch (Xml.Attribute("spacing-mode").Value.ToLower())
        //        {
        //            case "fs":
        //                this.Spacing = Styles.SpacingModes.FS;
        //                break;
        //            case "es":
        //                this.Spacing = Styles.SpacingModes.ES;
        //                break;
        //            case "js":
        //                this.Spacing = Styles.SpacingModes.JS;
        //                break;
        //            default:
        //                this.Spacing = Styles.SpacingModes.ES;
        //                break;
        //        }
        //    }
        //}
        //private void ParseSpaceAttribute()
        //{
        //    if (Xml.Attribute("space") != null)
        //    {
        //        try
        //        {
        //            this.Space = XUnitPt.FromMillimeter(double.Parse(Xml.Attribute("space").Value));
        //        }
        //        catch (Exception)
        //        {
        //            this.Space = XUnitPt.Zero;
        //        }
        //    }
        //}
        //private void ParseRotateAttribute()
        //{
        //    if (Xml.Attribute("rotate") != null)
        //    {
        //        this.Rotate = Xml.Attribute("rotate").Value;
        //    }
        //    else
        //    {
        //        this.Rotate = "FALSE";
        //    }
        //}
        private void ParseComponents()
        {
            //todo: Row.ParseComponents();
            IEnumerable<XElement> xElements = xml.Elements();
            foreach (XElement xElem in xElements)
            {
                DrawableElement? element = null;
                switch (xElem.Name.LocalName)
                {
                    case "column":
                        //element = new Column(xElem);
                        element = new DrawableElement();
                        break;
                    case "image":
                        //element = new Image(xElem);
                        element = new DrawableElement();
                        break;
                    case "stamp":
                        //element = new Stamp(xElem);
                        element = new DrawableElement();
                        break;
                    case "text":
                        //element = new Text(xElem);
                        element = new DrawableElement();
                        break;
                }
                if (element != null)
                {
                    element.Style.Color = this.Style.Color;
                    element.Parent = this;
                    //element.Parse();
                    this.Elements.Add(element);
                }
            }
        }
        #endregion

    }
}
