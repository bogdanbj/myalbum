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
        private List<Row>? _rows;
<<<<<<< Updated upstream
        DrawableElement _canvas;
=======
>>>>>>> Stashed changes
        #endregion

        #region properties
        private PageOrientation _orientation;
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
<<<<<<< Updated upstream
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

=======
        //public List<Row> Rows
        //{
        //    get
        //    {
        //        if (_rows == null) { _rows = new List<Row>(); }
        //        return _rows;
        //    }
        //    set { _rows = value; }
        //}
        
>>>>>>> Stashed changes
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
        public string No { get; set; }
        public PageOrientation Orientation
        {
            get 
            { 
                _pdfPage.Orientation = _orientation;
                w = _pdfPage.Width;
                h = _pdfPage.Height;
                return _orientation; 
            }
            set { _orientation = value; }
        }
        public PageSize Size { get; set; }
        public XUnitPt VSpace { get; set; }
        #endregion

        #region constructors
        public Page() : base() 
        {
            _pdfPage = new();
            Parent = null;

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
            #region calculate banner
            //TODO
            #endregion

            #region calculate frame
            if (Frame != null)
            {
                //Frame.gfx = gfx;

                if (this.Orientation == PdfSharp.PageOrientation.Portrait)
                {
                    Frame.x = x + MarginLeft;
                    Frame.y = y + MarginTop;
                    Frame.w = w - (MarginLeft + MarginRight);
                    Frame.h = h - (MarginTop + MarginBottom);
                    Frame.Calculate();

                    Canvas.x = Frame.x + Frame.WidthLeft + Frame.PaddingLeft;
                    Canvas.y = Frame.y + Frame.WidthTop + Frame.PaddingTop + VSpace;
                    Canvas.w = Frame.w - (Frame.WidthLeft + Frame.PaddingLeft + Frame.PaddingRight + Frame.WidthRight);
                    Canvas.h = Frame.h - (Frame.WidthTop + Frame.PaddingTop + Frame.PaddingBottom + Frame.WidthBottom + 2 * VSpace);
                }
                else
                {
                    Frame.x = x + MarginBottom;
                    Frame.y = y + MarginLeft;
                    Frame.w = w - (MarginBottom + MarginTop);
                    Frame.h = h - (MarginLeft + MarginRight);
                    Frame.Calculate();

                    Canvas.x = Frame.x + Frame.WidthBottom + Frame.PaddingLeft;
                    Canvas.y = Frame.y + Frame.WidthLeft + Frame.PaddingTop + VSpace;
                    Canvas.w = Frame.w - (Frame.WidthBottom + Frame.PaddingLeft + Frame.PaddingRight + Frame.WidthTop);
                    Canvas.h = Frame.h - (Frame.WidthLeft + Frame.PaddingTop + Frame.PaddingBottom + Frame.WidthRight + 2 * VSpace);
                }
            }
            #endregion

            #region calculate rows
            //TODO
            #endregion


        }
        public override void Draw()
        {

            #region draw frame
            if (Frame != null)
            {
                 Frame.gfx = gfx;
                if (Orientation == PageOrientation.Landscape)
                {
                    //Frame.gfx.TranslateTransform(w / 2, h / 2);
                    //Frame.gfx.RotateTransform(90);
                    //Frame.gfx.TranslateTransform(-w / 2, -h / 2);

                }
                Frame.Draw();
            }
            #endregion

            Canvas.gfx = gfx;
            Canvas.DrawBackground(XColors.WhiteSmoke);
            PrintAttributes();
        }

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
            gfx.DrawString($"StyleName: {StyleName}",
                        font,
                        XBrushes.Black,
                        new XRect(Canvas.x, y, Canvas.w, Canvas.h),
                        XStringFormats.TopLeft);
            y += 20;
            gfx.DrawString($"Orientation: {Orientation}",
                        font,
                        XBrushes.Black,
                        new XRect(Canvas.x, y, Canvas.w, Canvas.h),
                        XStringFormats.TopLeft);
            y += 20;
            gfx.DrawString($"Size: {Size}",
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

            // number
            ParseNumberAttribute();

            //VSpace Attribute
            ParseVSpaceAttribute();
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
                this.MarginTop = style.MarginTop;
                this.MarginRight = style.MarginRight;
                this.MarginBottom = style.MarginBottom;
                this.MarginLeft = style.MarginLeft;

                this.Orientation = style.Orientation;
                this.Size = style.Size;
                this.VSpace = style.VSpace;
                
                if (style.Frame != null)
                {
                    this.Frame.StyleName = style.Frame.StyleName;
                    this.Frame.Parent = this;
                    this.Frame.ApplyStyleAttributes();
                    //Console.WriteLine("has border");
                }

                
            }
        }

        private void ParseComponents()
        {
        }
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
        private void ParseNumberAttribute()
        {
            if (xml.Attribute("no") != null)
            {
                this.No = xml.Attribute("no")!.Value;
            }
        }
        private new void ParseVSpaceAttribute()
        {
            if (xml.Attribute("vspace") != null)
            {
                try
                {
                    VSpace = XUnitPt.FromMillimeter(double.Parse(xml.Attribute("vspace")!.Value));
                }
                catch (Exception)
                {
                    VSpace = XUnitPt.Zero;
                }
            }
        }
        #endregion



    }
}
