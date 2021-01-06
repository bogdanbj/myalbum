using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Xml.Linq;
using PdfSharp.Drawing;
using PdfSharp.Pdf;

namespace MyAlbum3
{
    class Page : Container
    {
        #region Properties
        PdfPage _pdfPage;
        private Image _banner;
        private Border _frame;
        private List<Row> _rows;
        private XUnit yPos, yPosRotate;
        public PdfPage PdfPage
        {
            get 
            {
                if (_pdfPage == null)
                {
                    _pdfPage = new PdfPage();
                }
                return _pdfPage;
            }
            set { _pdfPage = value; }
        }
        public string Title { get; set; }
        public string No { get; set; }
        public string Orientation { get; set; }
        public string Size { get; set; }
        public new static XUnit VSpace { get; set; }
        //public XUnit VSpace { get; set; }
        public Image Banner
        {
            get 
            {
                if (_banner == null) { _banner = new Image(); }
                return _banner;
            }
            set { _banner = value; }
        }
        public Border Frame
        {
            get 
            {
                if (_frame == null) { _frame = new Border(); }
                return _frame;
            }
            set { _frame = value; }
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
        #endregion

        #region Constructors
        public Page()
        {
            BoxColor = XColors.Red;
        }
        public Page(XElement xml) : this()
        {
            Xml = xml;
        }
        #endregion

        #region Public Methods
        public override void Parse()
        {
            Parse(isStyle:false);
        }
        public void Parse(bool isStyle)
        {
            Page style = FindStyle();
            if (!isStyle)
                ApplyStyle(style);
            ParseAttributes();
            ParseComponents();
        }
        public override void Calculate()
        {
            gfx = XGraphics.FromPdfPage(_pdfPage);
            //XSolidBrush brush;

            #region page orientation
            switch (this.Orientation.ToLower())
            {
                case "portrait":
                    _pdfPage.Orientation = PdfSharp.PageOrientation.Portrait;
                    break;
                case "landscape":
                    _pdfPage.Orientation = PdfSharp.PageOrientation.Landscape;
                    break;
                default:
                    _pdfPage.Orientation = PdfSharp.PageOrientation.Portrait;
                    break;
            }
            #endregion

            #region page size
            switch (this.Size.ToLower())
            {
                case "letter":
                    _pdfPage.Size = PdfSharp.PageSize.Letter;
                    break;
                case "a4":
                    _pdfPage.Size = PdfSharp.PageSize.A4;
                    break;
                default:
                    _pdfPage.Size = PdfSharp.PageSize.Letter;
                    break;
            }
            #endregion

            #region page canvas
                if (_pdfPage.Orientation == PdfSharp.PageOrientation.Portrait)
                {
                    x = MarginLeft;
                    y = MarginTop;
                    w = _pdfPage.Width - MarginLeft - MarginRight;
                    h = _pdfPage.Height - MarginTop - MarginBottom;
                }
                else
                {
                    x = MarginBottom;
                    y = MarginLeft;
                    w = _pdfPage.Width - (MarginBottom + MarginTop);
                    h = _pdfPage.Height - (MarginLeft + MarginRight);
                    //x = MarginLeft;
                    //y = MarginTop;
                    //w = _pdfPage.Width - MarginLeft - MarginRight;
                    //h = _pdfPage.Height - MarginTop - MarginBottom;
                }
            #endregion
            //brush = new XSolidBrush(XColors.AliceBlue);
            //gfx.DrawRectangle(brush, x, y, w, h);

            #region calculate banner
            if (!String.IsNullOrEmpty(Banner.FileName))
            {
                Banner.gfx = gfx;
                Banner.Load();

                if (_pdfPage.Orientation == PdfSharp.PageOrientation.Portrait)
                {
                    Banner.w = w;
                    Banner.h = XUnit.FromPoint(Banner.XImg.Size.Height) * Banner.w / XUnit.FromPoint(Banner.XImg.Size.Width);
                    Banner.x = x + w / 2;
                    Banner.y = y;

                    y += Banner.h;
                    h -= Banner.h;
                }
                else
                {

                    Banner.w = h;
                    Banner.h = XUnit.FromPoint(Banner.XImg.Size.Height) * Banner.w / XUnit.FromPoint(Banner.XImg.Size.Width);
                    Banner.x = (_pdfPage.Width - _pdfPage.Height) / 2 + MarginTop + Banner.w / 2;
                    Banner.y = - (_pdfPage.Width - _pdfPage.Height) / 2 + MarginRight;

                    w -= Banner.h;
                    //h -= Banner.h;
                }
            }
            #endregion
            //brush = new XSolidBrush(XColors.LightPink);
            //gfx.DrawRectangle(brush, x, y, w, h);

            #region calculate frame
            if (Frame != null)
            {
                if (_pdfPage.Orientation == PdfSharp.PageOrientation.Portrait)
                {
                    Frame.gfx = gfx;
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
                    Frame.gfx = gfx;
                    Frame.x = (_pdfPage.Width - _pdfPage.Height) / 2 + MarginTop + Frame.MarginLeft;
                    Frame.y = -(_pdfPage.Width - _pdfPage.Height) / 2 + MarginRight + Banner.h + Frame.MarginTop;
                    Frame.w = h - (Frame.MarginLeft + Frame.MarginRight);
                    Frame.h = w - (Frame.MarginTop + Frame.MarginBottom);
                    Frame.Calculate();

                    x += Frame.MarginTop + Frame.WidthTop + Frame.PaddingTop + VSpace;
                    y += Frame.MarginLeft + Frame.WidthLeft + Frame.PaddingLeft;
                    w -= Frame.MarginTop + Frame.WidthTop + Frame.PaddingTop + Frame.PaddingBottom + Frame.WidthBottom + Frame.MarginBottom + 2 * VSpace;// ;
                    h -= Frame.MarginLeft + Frame.WidthLeft + Frame.PaddingLeft + Frame.PaddingRight + Frame.WidthRight + Frame.MarginRight;
                }
            }
            #endregion
            //brush = new XSolidBrush(XColors.LightGreen);
            //gfx.DrawRectangle(brush, x, y, w, h);

            #region calculate rows
            //yPos = y;
            foreach (Row row in Rows)
            {
                row.gfx = gfx;
                row.Parent = this;
                row.Calculate();
                //brush = new XSolidBrush(XColors.LightSkyBlue);
                //gfx.DrawRectangle(brush, x, y, w, h);

                //yPos += row.h + VSpace;
            }
            #endregion


        }
        public override void Draw()
        {
            //XSolidBrush brush;


            #region draw banner
            if (Banner != null && Banner.FileName.Length > 0) 
            {
                if (_pdfPage.Orientation == PdfSharp.PageOrientation.Portrait)
                {
                    Banner.Draw();
                }
                else
                {
                    gfx.TranslateTransform(_pdfPage.Width / 2, _pdfPage.Height / 2);
                    gfx.RotateTransform(90);
                    gfx.TranslateTransform(-_pdfPage.Width / 2, -_pdfPage.Height / 2);
                    Banner.Draw();
                    gfx.TranslateTransform(_pdfPage.Width / 2, _pdfPage.Height / 2);
                    gfx.RotateTransform(-90);
                    gfx.TranslateTransform(-_pdfPage.Width / 2, -_pdfPage.Height / 2);
                }
            }
            #endregion

            #region draw frame
            if (Frame != null) 
            { 
                if (_pdfPage.Orientation == PdfSharp.PageOrientation.Portrait)
                {
                    Frame.Draw();
                }
                else
                {
                    //XSolidBrush brush = new XSolidBrush(XColors.AliceBlue);
                    //gfx.DrawRectangle(brush, x, y, w, h);

                    gfx.TranslateTransform(_pdfPage.Width / 2, _pdfPage.Height / 2);
                    gfx.RotateTransform(90);
                    gfx.TranslateTransform(-_pdfPage.Width / 2, -_pdfPage.Height / 2);
                    Frame.Draw();
                    gfx.TranslateTransform(_pdfPage.Width / 2, _pdfPage.Height / 2);
                    gfx.RotateTransform(-90);
                    gfx.TranslateTransform(-_pdfPage.Width / 2, -_pdfPage.Height / 2);
                }
            }
            #endregion

            #region draw rows
            yPos = y;
            yPosRotate = y - (_pdfPage.Width - _pdfPage.Height) / 2;
            yPosRotate -= Frame.MarginLeft + Frame.WidthLeft + Frame.PaddingLeft;
            yPosRotate += Frame.MarginTop + Frame.WidthTop + Frame.PaddingTop + VSpace;

            foreach (Row row in Rows) 
            {
                row.Parent = this;
                if (row.Rotate.ToLower() == "true")
                {
                    gfx.TranslateTransform(_pdfPage.Width / 2, _pdfPage.Height / 2);
                    gfx.RotateTransform(90);
                    gfx.TranslateTransform(-_pdfPage.Width / 2, -_pdfPage.Height / 2);
                    row.y = yPosRotate;
                    row.Draw();
                    yPosRotate += row.h + VSpace;
                    this.w = this.w - row.h;
                    gfx.TranslateTransform(_pdfPage.Width / 2, _pdfPage.Height / 2);
                    gfx.RotateTransform(-90);
                    gfx.TranslateTransform(-_pdfPage.Width / 2, -_pdfPage.Height / 2);
                }
                else 
                {
                    row.y = yPos;
                    //brush = new XSolidBrush(XColors.LightYellow);
                    //gfx.DrawRectangle(brush, row.x, row.y, row.w, row.h);
                    row.Draw();
                    yPos += row.h + VSpace;
                }
            }
            #endregion

        }
        #endregion

        #region Private Methods
        private Page FindStyle()
        {
            Page style = null;

            // use specific style
            if (Xml.Attribute("style") != null)
            {
                style = Styles.PageStyles.Where(t => t.Style == Xml.Attribute("style").Value).FirstOrDefault();

            }

            // use default
            if (style == null)
            {
                style = Styles.PageStyles.Where(t => t.IsDefault == true).FirstOrDefault();
            }

            return style;

        }
        private void ApplyStyle(Page stylePage)
        {
            // copy style properties
            if (stylePage != null)
            {
                Style = stylePage.Style;
                Orientation = stylePage.Orientation;
                Size = stylePage.Size;
                Banner = stylePage.Banner;
                Frame = stylePage.Frame;
                MarginLeft = stylePage.MarginLeft;
                MarginRight = stylePage.MarginRight;
                MarginTop = stylePage.MarginTop;
                MarginBottom = stylePage.MarginBottom;
                Color = stylePage.Color;

                // copy components
                Rows.Clear();
                foreach (Row row in stylePage.Rows)
                {
                    Rows.Add(row);
                }
            }

        }
        private void ParseAttributes()
        {
            // base element attribute
            ParseBaseAttribute();

            // title attribute
            ParseTitleAttribute();

            // number attribute
            ParseNumberAttribute();

            // orientation attribute
            ParseOrientationAttribute();

            // size attribute
            ParseSizeAttribute();

            // vspace attribute
            ParseVSpaceAttribute();
        }
        private void ParseTitleAttribute()
        {
            if (Xml.Attribute("title") != null)
            {
                this.Title = Xml.Attribute("title").Value;
            }
        }
        private void ParseNumberAttribute()
        {
            if (Xml.Attribute("no") != null)
            {
                this.No = Xml.Attribute("no").Value;
            }
        }
        private void ParseOrientationAttribute()
        {
            if (Xml.Attribute("orientation") != null)
            {
                this.Orientation = Xml.Attribute("orientation").Value.ToLower();
            }
        }
        private void ParseSizeAttribute()
        {
            if (Xml.Attribute("size") != null)
            {
                this.Size = Xml.Attribute("size").Value.ToLower();
            }
        }
        private void ParseVSpaceAttribute()
        {
            if (Xml.Attribute("vspace") != null)
            {
                try
                {
                    VSpace = XUnit.FromMillimeter(double.Parse(Xml.Attribute("vspace").Value));
                }
                catch (Exception)
                {
                    VSpace = XUnit.Zero;
                }
            }
        }
        private void ParseComponents()
        {
            //todo: Page.ParseComponents()
            IEnumerable<XElement> elements = Xml.Elements();
            foreach (XElement xElem in elements)
            {
                switch (xElem.Name.LocalName)
                {
                    case "banner":
                        Banner = new Image(xElem);
                        Banner.Parent = this;
                        Banner.Parse();
                        break;
                    case "border":
                        Frame = new Border(xElem);
                        Frame.Parent = this;
                        Frame.Parse();
                        break;
                    case "row":
                        Row newRow = new Row(xElem);
                        newRow.Parent = this;
                        newRow.Parse();
                        this.Rows.Add(newRow);
                        break;
                    default:
                        break;
                }
                //Console.WriteLine(xElem.ToString());
            }
        }
        #endregion
    }
}
