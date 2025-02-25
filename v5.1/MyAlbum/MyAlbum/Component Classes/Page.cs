using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MyAlbum
{
    internal class Page : DrawableElement
    {
        #region fields
        private PdfPage _pdfPage;
        //private Image? _banner;
        private Border? _frame;
        //private List<Row>? _rows;
        #endregion

        #region properties
        public PdfPage PdfPage
        {
            get
            {
                if (_pdfPage == null) { _pdfPage = new PdfPage(); }
                return _pdfPage;
            }
            set
            {
                _pdfPage = value;
                gfx = XGraphics.FromPdfPage(_pdfPage);
            }
        }
        public PageOrientation Orientation { get; set; }
        public PageSize Size { get; set; }


        //public Image Banner
        //{
        //    get
        //    {
        //        if (_banner == null) { _banner = new Image(); }
        //        return _banner;
        //    }
        //    set { _banner = value; }
        //}
        public Border Frame
        {
            get
            {
                if (_frame == null) { _frame = new Border(); }
                return _frame;
            }
            set { _frame = value; }
        }
        //public List<Row> Rows
        //{
        //    get
        //    {
        //        if (_rows == null) { _rows = new List<Row>(); }
        //        return _rows;
        //    }
        //    set { _rows = value; }
        //}
        #endregion

        #region constructors
        public Page() : base() 
        {
            _pdfPage = new();
            Parent = null;
        }
        public Page(XElement xml) :this() 
        {
            this.xml = xml;
        }
        public Page(XElement xml, PdfPage pdfPage) : this(xml)
        {
            this.PdfPage = pdfPage;
        }
        #endregion

        #region public methods
        public void Calculate()
        {
            gfx = XGraphics.FromPdfPage(_pdfPage);

            #region calculate frame
            if (Frame != null)
            {
                Frame.gfx = gfx;

                if (this.Orientation == PdfSharp.PageOrientation.Portrait)
                {
                    Frame.x = x + Frame.MarginLeft;
                    Frame.y = y + Frame.MarginTop;
                    Frame.w = w - (Frame.MarginLeft + Frame.MarginRight);
                    Frame.h = h - (Frame.MarginTop + Frame.MarginBottom);
                    Frame.Calculate();

                    x += Frame.MarginLeft + Frame.WidthLeft + Frame.PaddingLeft;
                    y += Frame.MarginTop + Frame.WidthTop + Frame.PaddingTop + VSpace;
                    w -= Frame.MarginLeft + Frame.WidthLeft + Frame.PaddingLeft + Frame.PaddingRight + Frame.WidthRight + Frame.MarginRight;
                    h -= Frame.MarginTop + Frame.WidthTop + Frame.PaddingTop + Frame.PaddingBottom + Frame.WidthBottom + Frame.MarginBottom + 2 * VSpace;
                }
                else
                {
                    Frame.x = (w - h) / 2 + MarginTop + Frame.MarginLeft;
                    Frame.x = x + Frame.MarginLeft;
                    Frame.y = -(w - h) / 2 + MarginRight + Banner.h + Frame.MarginTop;
                    Frame.y = y + Frame.MarginTop;
                    Frame.w = h - (Frame.MarginLeft + Frame.MarginRight);
                    Frame.h = w - (Frame.MarginTop + Frame.MarginBottom);


                    brush = new XSolidBrush(XColors.LightGreen);
                    gfx.DrawRectangle(brush, Frame.x, Frame.y, Frame.w, Frame.h);

                    Frame.Calculate();

                    x += Frame.MarginTop + Frame.WidthTop + Frame.PaddingTop + VSpace;
                    y += Frame.MarginLeft + Frame.WidthLeft + Frame.PaddingLeft;
                    w -= Frame.MarginTop + Frame.WidthTop + Frame.PaddingTop + Frame.PaddingBottom + Frame.WidthBottom + Frame.MarginBottom + 2 * VSpace;// ;
                    h -= Frame.MarginLeft + Frame.WidthLeft + Frame.PaddingLeft + Frame.PaddingRight + Frame.WidthRight + Frame.MarginRight;
                }
            }
            #endregion


        }

        public void Draw()
        {
            this._pdfPage.Orientation = this.Orientation;

            #region draw frame
            if (Frame != null)
            { 
            }
            #endregion



            PrintAttributes();
        }

        private void PrintAttributes()
        {
            string text = xml.ToString();

            XPdfFontOptions options = new XPdfFontOptions(PdfFontEncoding.Unicode, PdfFontEmbedding.EmbedCompleteFontFile);
            XFont font = new XFont("Stymie Becker Light", 20, XFontStyleEx.Regular, options);

            int y = 0;
            foreach (XAttribute attribute in xml.Attributes())
            {
                gfx.DrawString($"{attribute.Name}: {attribute.Value}",
                            font,
                            XBrushes.Black,
                            // new XRect(Canvas.x, Canvas.y, Canvas.w, Canvas.h),
                            new XRect(0, y, 500, 800),
                            XStringFormats.TopLeft);
                y += 20;
            }
            gfx.DrawString($"StyleName: {StyleName}",
                        font,
                        XBrushes.Black,
                        new XRect(0, y, 500, 800),
                        XStringFormats.TopLeft);
            y += 20;
            gfx.DrawString($"Orientation: {Orientation}",
                        font,
                        XBrushes.Black,
                        new XRect(0, y, 500, 800),
                        XStringFormats.TopLeft);
            y += 20;
            gfx.DrawString($"Size: {Size}",
                        font,
                        XBrushes.Black,
                        new XRect(0, y, 500, 800),
                        XStringFormats.TopLeft);
            y += 20;
            gfx.DrawString($"Width: {PdfPage.Width}, Height: {PdfPage.Height}",
                        font,
                        XBrushes.Black,
                        new XRect(0, y, 500, 800),
                        XStringFormats.TopLeft);
        }

        public override void Parse()
        {
            ParseAttributes();
            ParseComponents();
        }
        #endregion

        #region private methods
        private void ParseAttributes()
        {
            // get and apply the style
            ParseStyleNameAttribute();
            ApplyStyleAttributes();

            // orientation
            ParseOrientationAttribute();

            // size
            ParseSizeAttribute();
        }

        private void ApplyStyleAttributes()
        {
            // find style
            PageStyle? style;
            style = Styles.PageStyles.Find(e => e.StyleName == StyleName);
            // if not found look for default style
            if (style == null)
            {
                style = Styles.PageStyles.Find(e => e.IsDefault == true);
            }

            // apply style
            if (style != null)
            {
                this.Orientation = style.Orientation;
                this.Size = style.Size;
            }
        }

        private void ParseComponents()
        {
        }
        #endregion
        
        // override the ParseStyleNameAttribute because this time
        // it is okay for the attribute "style" to be missing
        private void ParseStyleNameAttribute()
        {
            //Console.WriteLine(xml);
            this.StyleName = xml.Attribute("style")?.Value ?? "";
        }
        private void ParseOrientationAttribute()
        {
            if (xml.Attribute("orientation") != null)
            {
                switch (xml.Attribute("orientation").Value.ToLower())
                {
                    case "portrait":
                        this.Orientation = PdfSharp.PageOrientation.Portrait;
                        break;
                    case "landscape":
                        this.Orientation = PdfSharp.PageOrientation.Landscape;
                        break;
                    default:
                        break;
                }
            }
            //else
            //    this.Orientation = PdfSharp.PageOrientation.Portrait;
        }
        private void ParseSizeAttribute()
        {
            if (xml.Attribute("size") != null)
            {
                switch (xml.Attribute("size").Value.ToLower())
                {
                    case "letter":
                        this.Size = PdfSharp.PageSize.Letter;
                        break;
                    case "a4":
                        this.Size = PdfSharp.PageSize.A4;
                        break;
                    default:
                        this.Size = PdfSharp.PageSize.Letter;
                        break;
                }
            }
        }



    }
}
