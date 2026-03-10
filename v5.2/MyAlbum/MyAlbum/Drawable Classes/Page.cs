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
using static System.Net.Mime.MediaTypeNames;

namespace MyAlbum
{
    internal class Page : DrawableElement
    {
        #region fields
        private PdfPage _pdfPage;
        //private Image? _banner;
        private Border? _frame;
        private List<Row>? _rows;
        DrawableElement _canvas;
        #endregion

        #region properties
        public new PageStyle Style { get; set; }
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
        public DrawableElement Canvas
        {
            get
            {
                if (_canvas == null) { _canvas = new DrawableElement(); }
                return _canvas;
            }
        }
        public List<Row> Rows
        {
            get
            {
                if (_rows == null) { _rows = new List<Row>(); }
                return _rows;
            }
            set { _rows = value; }
        }

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
        public PageSize Size { get; set; }
        //public XUnitPt VSpace { get; set; }
        #endregion

        #region constructors
        public Page() //: base() 
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
            _pdfPage.Size = Style.Size;
            w = _pdfPage.Width;
            h = _pdfPage.Height;

            Canvas.x = Style.MarginLeft;
            Canvas.y = Style.MarginTop;
            Canvas.w = w - (Style.MarginLeft + Style.MarginRight);
            Canvas.h = h - (Style.MarginTop + Style.MarginBottom);


            #region calculate frame
            if (Frame != null)
            {
                //Frame.gfx = gfx;

                if (Style.Orientation == PdfSharp.PageOrientation.Portrait)
                {
                    Frame.x = Canvas.x + Frame.Style.MarginLeft;
                    Frame.y = Canvas.y + Frame.Style.MarginTop;
                    Frame.w = Canvas.w - (Frame.Style.MarginLeft + Frame.Style.MarginRight);
                    Frame.h = Canvas.h - (Frame.Style.MarginTop + Frame.Style.MarginBottom);
                    Frame.Calculate();

                    //Canvas.x += Frame.MarginLeft + Frame.WidthLeft + Frame.PaddingLeft;
                    //Canvas.y += Frame.MarginTop + Frame.WidthTop + Frame.PaddingTop + VSpace;
                    //Canvas.w -= Frame.MarginLeft + Frame.WidthLeft + Frame.PaddingLeft + Frame.PaddingRight + Frame.WidthRight + Frame.MarginRight;
                    //Canvas.h -= Frame.MarginTop + Frame.WidthTop + Frame.PaddingTop + Frame.PaddingBottom + Frame.WidthBottom + Frame.MarginBottom + 2 * VSpace;
                    Canvas.x = Frame.x + Frame.WidthLeft + Frame.Style.PaddingLeft;
                    Canvas.y = Frame.y + Frame.WidthTop + Frame.Style.PaddingTop + Style.VSpace;
                    Canvas.w = Frame.w - (Frame.WidthLeft + Frame.Style.PaddingLeft + Frame.Style.PaddingRight + Frame.WidthRight);
                    Canvas.h = Frame.h - (Frame.WidthTop + Frame.Style.PaddingTop + Frame.Style.PaddingBottom + Frame.WidthBottom + 2 * Style.VSpace);
                }
                else
                {
                    Frame.x = Canvas.x + Frame.Style.MarginBottom;
                    Frame.y = Canvas.y + Frame.Style.MarginLeft;
                    Frame.w = Canvas.w - (Frame.Style.MarginBottom + Frame.Style.MarginTop);
                    Frame.h = Canvas.h - (Frame.Style.MarginLeft + Frame.Style.MarginRight);
                    Frame.Calculate();

                    Canvas.x = Frame.x + Frame.WidthBottom + Frame.Style.PaddingLeft;
                    Canvas.y = Frame.y + Frame.WidthLeft + Frame.Style.PaddingTop + Style.VSpace;
                    Canvas.w = Frame.w - (Frame.WidthBottom + Frame.Style.PaddingLeft + Frame.Style.PaddingRight + Frame.WidthTop);
                    Canvas.h = Frame.h - (Frame.WidthLeft + Frame.Style.PaddingTop + Frame.Style.PaddingBottom + Frame.WidthRight + 2 * Style.VSpace);
                }
            }
            #endregion


            #region calculate rows
            XUnitPt yPos = Canvas.y;

            foreach (Row row in Rows)
            {
                row.x = Canvas.x + row.Style.MarginLeft;
                row.y = yPos;
                row.w = Canvas.w;
                row.h = row.Style.Height;
                row.Calculate();
                yPos += row.h + Style.VSpace;
            }
            #endregion

        }
        public override void Draw()
        {

            #region draw frame
            if (Frame != null)
            {
                Frame.gfx = gfx;
                //if (Style.Orientation == PageOrientation.Landscape)
                //{
                //    //Frame.gfx.TranslateTransform(w / 2, h / 2);
                //    //Frame.gfx.RotateTransform(90);
                //    //Frame.gfx.TranslateTransform(-w / 2, -h / 2);

                //}
                Frame.Draw();
            }
            #endregion

            Canvas.gfx = gfx;
            Canvas.DrawBackground(XColors.WhiteSmoke);
            //yPos = Canvas.y;

            #region draw rows
            foreach (Row row in Rows)
            {
                row.gfx = gfx;
                //row.y = yPos;
                //row.x = Canvas.x;
                //row.w = Canvas.w;
                //row.h = 50;
                //if (row.Style.Rotate == true)
                //{
                //    row.gfx.TranslateTransform(w / 2, h / 2);
                //    row.gfx.RotateTransform(90);
                //    row.gfx.TranslateTransform(-w / 2, -h / 2);
                //}
                row.Draw();
                //if (row.Style.Rotate == true)
                //{
                //    row.gfx.TranslateTransform(w / 2, h / 2);
                //    row.gfx.RotateTransform(-90);
                //    row.gfx.TranslateTransform(-w / 2, -h / 2);
                //}
                //yPos += row.h + Style.VSpace;
            }
            #endregion

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
            // add Style components
            foreach (DrawableElement elem in Style.Components)
            {
                switch (elem.GetType().Name)
                {
                    //case nameof(Image):
                    //    break;
                    case nameof(Border):
                        Frame = elem as Border;
                        Frame.Parent = this;
                        Frame.Parse();
                        break;
                    case nameof(Row):
                        Row newRow = elem as Row;
                        newRow.Parent = this;
                        newRow.Parse();
                        Rows.Add(newRow);
                        break;
                    default:
                        break;
                }
            }
            //Frame = Style.Frame;
            //Frame.Parent = this;
            //Frame.Parse();


            ////todo: Page.ParseComponents()
            IEnumerable<XElement> elements = xml.Elements();
            foreach (XElement xElem in elements)
            {
                switch (xElem.Name.LocalName)
                {
                    //case "banner":
                    //    Banner = new Image(xElem);
                    //    Banner.Parent = this;
                    //    Banner.Parse();
                    //    break;
                    case "border":
                        Frame = new Border(xElem);
                        Frame.Parent = this;
                        Frame.Parse();
                        break;
                    case "row":
                        Row newRow = new Row(xElem);
                        newRow.Parent = this;
                        newRow.Parse();
                        Rows.Add(newRow);
                        break;
                    default:
                        break;
                }
                //Console.WriteLine(xElem.ToString());
            }
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
