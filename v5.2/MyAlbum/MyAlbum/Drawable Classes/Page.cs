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
        //private Border? _frame;
        //private List<Row>? _rows;
        DrawableElement _canvas;
        #endregion

        #region properties
        public PageStyle Style { get; set; }
        //private PageOrientation _orientation;
        //public Image Banner
        //{
        //    get
        //    {
        //        if (_banner == null) { _banner = new Image(); }
        //        return _banner;
        //    }
        //    set { _banner = value; }
        //}
        //public Border Frame
        //{
        //    get
        //    {
        //        if (_frame == null) { _frame = new Border(); }
        //        return _frame;
        //    }
        //    set { _frame = value; }
        //}
        public DrawableElement Canvas
        {
            get
            {
                if (_canvas == null) { _canvas = new DrawableElement(); }
                return _canvas;
            }
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
        public string? No { get; set; }
        //public PageOrientation Orientation
        //{
        //    get 
        //    { 
        //        _pdfPage.Orientation = _orientation;
        //        w = _pdfPage.Width;
        //        h = _pdfPage.Height;
        //        return _orientation; 
        //    }
        //    set { _orientation = value; }
        //}
        public PageSize Size { get; set; }
        public XUnitPt VSpace { get; set; }
        #endregion

        #region constructors
        public Page() : base() 
        {
            _pdfPage = new();
            Parent = null;

            //this.Style = PageStyle.Default;

            x = XUnitPt.Zero;
            y = XUnitPt.Zero;
            w = _pdfPage.Width;
            h = _pdfPage.Height;

            Canvas.x = XUnitPt.Zero;
            Canvas.y = XUnitPt.Zero;
            Canvas.w = _pdfPage.Width;
            Canvas.h = _pdfPage.Height;

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
        public override void Parse()
        {
            ParseAttributes();
            ParseComponents();
        }
        public override void Calculate()
        {
            _pdfPage.Orientation = Style.Orientation;
            w = _pdfPage.Width;
            h = _pdfPage.Height;

            Canvas.x = Style.MarginLeft;
            Canvas.y = Style.MarginTop;
            Canvas.w = w - (Style.MarginLeft + Style.MarginRight);
            Canvas.h = h - (Style.MarginTop + Style.MarginBottom);

            
            //#region calculate frame
            //if (Frame != null)
            //{
            //    //Frame.gfx = gfx;

            //    if (this.Orientation == PdfSharp.PageOrientation.Portrait)
            //    {
            //        Frame.x = x + Frame.MarginLeft;
            //        Frame.y = y + Frame.MarginTop;
            //        Frame.w = w - (Frame.MarginLeft + Frame.MarginRight);
            //        Frame.h = h - (Frame.MarginTop + Frame.MarginBottom);
            //        Frame.Calculate();

            //        //Canvas.x += Frame.MarginLeft + Frame.WidthLeft + Frame.PaddingLeft;
            //        //Canvas.y += Frame.MarginTop + Frame.WidthTop + Frame.PaddingTop + VSpace;
            //        //Canvas.w -= Frame.MarginLeft + Frame.WidthLeft + Frame.PaddingLeft + Frame.PaddingRight + Frame.WidthRight + Frame.MarginRight;
            //        //Canvas.h -= Frame.MarginTop + Frame.WidthTop + Frame.PaddingTop + Frame.PaddingBottom + Frame.WidthBottom + Frame.MarginBottom + 2 * VSpace;
            //        Canvas.x = Frame.x + Frame.WidthLeft + Frame.PaddingLeft;
            //        Canvas.y = Frame.y + Frame.WidthTop + Frame.PaddingTop + VSpace;
            //        Canvas.w = Frame.w - (Frame.WidthLeft + Frame.PaddingLeft + Frame.PaddingRight + Frame.WidthRight);
            //        Canvas.h = Frame.h - (Frame.WidthTop + Frame.PaddingTop + Frame.PaddingBottom + Frame.WidthBottom + 2 * VSpace);
            //    }
            //    else
            //    {
            //        Frame.x = x + Frame.MarginBottom;
            //        Frame.y = y + Frame.MarginLeft;
            //        Frame.w = w - (Frame.MarginBottom + Frame.MarginTop);
            //        Frame.h = h - (Frame.MarginLeft + Frame.MarginRight);
            //        Frame.Calculate();

            //        Canvas.x = Frame.x + Frame.WidthBottom + Frame.PaddingLeft;
            //        Canvas.y = Frame.y + Frame.WidthLeft + Frame.PaddingTop + VSpace;
            //        Canvas.w = Frame.w - (Frame.WidthBottom + Frame.PaddingLeft + Frame.PaddingRight + Frame.WidthTop);
            //        Canvas.h = Frame.h - (Frame.WidthLeft + Frame.PaddingTop + Frame.PaddingBottom + Frame.WidthRight + 2 * VSpace);
            //    }
            //}
            //#endregion


        }
        public override void Draw()
        {

            //#region draw frame
            //if (Frame != null)
            //{
            //     Frame.gfx = gfx;
            //    if (Orientation == PageOrientation.Landscape)
            //    {
            //        //Frame.gfx.TranslateTransform(w / 2, h / 2);
            //        //Frame.gfx.RotateTransform(90);
            //        //Frame.gfx.TranslateTransform(-w / 2, -h / 2);

            //    }
            //    Frame.Draw();
            //}
            //#endregion

            Canvas.gfx = gfx;
            Canvas.DrawBackground(XColors.WhiteSmoke);
            PrintAttributes();
        }

        #endregion

        #region private methods
        private void ParseAttributes()
        {
            StyleName = ParseStyleNameAttribute(xml) ?? "";

            Style = Styles.PageStyles.FirstOrDefault(s => s.StyleName == StyleName);

            // orientation
            if (xml.Attribute("orientation") != null)
            {
                Style.Orientation = ParsePageOrientationAttribute(xml);
            }

            // size
            if (xml.Attribute("size") != null)
            {
                Style.Size = ParsePageSizeAttribute(xml);
            }

            //VSpace Attribute
            if (xml.Attribute("vspace") != null)
            {
                Style.VSpace = ParseVSpaceAttribute(xml);
            }

            // number
            No = ParseStringAttribute(xml, "no");

        }
        private void ParseComponents()
        {
        }
        #endregion

        #region helper methods
        private void PrintAttributes()
        {
            string text = xml.ToString();

            XPdfFontOptions options = new XPdfFontOptions(PdfFontEncoding.Unicode, PdfFontEmbedding.EmbedCompleteFontFile);
            XFont font = new XFont("Stymie Becker Light", 20, XFontStyleEx.Regular, options);

            XUnitPt y = Canvas.y;
            foreach (XAttribute attribute in xml.Attributes())
            {
                gfx.DrawString($"{attribute.Name}: {attribute.Value}",
                            font,
                            XBrushes.Black,
                            //new XRect(Canvas.x, Canvas.y, Canvas.w, Canvas.h),
                            new XRect(Canvas.x, y, Canvas.w, Canvas.h),
                            XStringFormats.TopLeft);
                y += 20;
            }
            gfx.DrawString($"StyleName: {this.StyleName}",
                        font,
                        XBrushes.Black,
                        new XRect(Canvas.x, y, Canvas.w, Canvas.h),
                        XStringFormats.TopLeft);
            y += 20;
            gfx.DrawString($"Style.StyleName: {Style.StyleName}",
                        font,
                        XBrushes.Black,
                        new XRect(Canvas.x, y, Canvas.w, Canvas.h),
                        XStringFormats.TopLeft);
            y += 20;
            gfx.DrawString($"Orientation: {Style.Orientation}",
                        font,
                        XBrushes.Black,
                        new XRect(Canvas.x, y, Canvas.w, Canvas.h),
                        XStringFormats.TopLeft);
            y += 20;
            gfx.DrawString($"Size: {Style.Size}",
                        font,
                        XBrushes.Black,
                        new XRect(Canvas.x, y, Canvas.w, Canvas.h),
                        XStringFormats.TopLeft);
            y += 20;
            gfx.DrawString($"Width: {PdfPage.Width}, Height: {PdfPage.Height}",
                        font,
                        XBrushes.Black,
                        new XRect(Canvas.x, y, Canvas.w, Canvas.h),
                        XStringFormats.TopLeft);
        }
        #endregion
    }
}
