using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Xml.Linq;
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;

namespace MyAlbum
{
    class Page : Container
    {
        #region Properties
        private PdfPage _pdfPage;
        private Image _banner;
        private Border _frame;
        private List<Row> _rows;
        private XUnitPt yPos, yPosRotate;
        private PageOrientation _orientation;
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
            set 
            { 
                _pdfPage = value; 
                this.Height = _pdfPage.Height.Point;
                this.Width = _pdfPage.Width.Point;
            }
        }
        public XUnitPt Width { get; set; }
        public XUnitPt Height { get; set; }
        public string Title { get; set; }
        public string No { get; set; }
        //public string Orientation { get; set; }

        public PageOrientation Orientation
        {
            get { return _orientation; }
            set 
            { 
                _orientation = value; 
                this.PdfPage.Orientation = value;
            }
        }

        //public PageOrientation Orientation { get; set; }
        //public string Size { get; set; }
        public PageSize Size { get; set; }
        public new static XUnitPt VSpace { get; set; }
        //public XUnitPt VSpace { get; set; }
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
            //this.PdfPage = new PdfPage();
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
            //gfx = XGraphics.FromPdfPage(_pdfPage);
            //gfx = XGraphics.FromPdfPage(this.PdfPage);
            XSolidBrush brush;

            #region page orientation
            this.PdfPage.Orientation = this.Orientation;
            this.PdfPage.Size = this.Size;
            gfx = XGraphics.FromPdfPage(this.PdfPage);
            //switch (this.Orientation.ToLower())
            //{
            //    case "portrait":
            //        this.Orientation = PdfSharp.PageOrientation.Portrait;
            //        break;
            //    case "landscape":
            //        this.Orientation = PdfSharp.PageOrientation.Landscape;
            //        break;
            //    default:
            //        this.Orientation = PdfSharp.PageOrientation.Portrait;
            //        break;
            //}
            #endregion

            #region page size
            //switch (this.Size.ToLower())
            //{
            //    case "letter":
            //        this.Size = PdfSharp.PageSize.Letter;
            //        break;
            //    case "a4":
            //        this.Size = PdfSharp.PageSize.A4;
            //        break;
            //    default:
            //        this.Size = PdfSharp.PageSize.Letter;
            //        break;
            //}
            #endregion

            #region page canvas
            if (this.Orientation == PdfSharp.PageOrientation.Portrait)
            {
                x = MarginLeft;
                y = MarginTop;
                w = this.Width - MarginLeft - MarginRight;
                h = this.Height - MarginTop - MarginBottom;
            }
            else
            {
                x = MarginBottom;
                y = MarginLeft;
                w = this.Height - MarginTop - MarginBottom;
                h = this.Width - MarginLeft - MarginRight;
            }
            //brush = new XSolidBrush(XColors.AliceBlue);
            //gfx.DrawRectangle(brush, x, y, w, h);
            #endregion


            #region calculate banner
            if (!String.IsNullOrEmpty(Banner.FileName))
            {
                Banner.gfx = gfx;
                Banner.Load();

                if (this.Orientation == PdfSharp.PageOrientation.Portrait)
                {
                    Banner.w = w;
                    Banner.h = XUnitPt.FromPoint(Banner.XImg.Size.Height) * Banner.w / XUnitPt.FromPoint(Banner.XImg.Size.Width);
                    Banner.x = x + w / 2;
                    Banner.y = y;

                    y += Banner.h;
                    h -= Banner.h;
                }
                else
                {

                    Banner.w = h;
                    Banner.h = XUnitPt.FromPoint(Banner.XImg.Size.Height) * Banner.w / XUnitPt.FromPoint(Banner.XImg.Size.Width);
                    Banner.x = (this.Width - this.Height) / 2 + MarginTop + Banner.w / 2;
                    Banner.y = - (this.Width - this.Height) / 2 + MarginRight;

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
                    //Frame.x = (this.Width - this.Height) / 2 + MarginTop + Frame.MarginLeft;
                    //Frame.y = -(this.Width - this.Height) / 2 + MarginRight + Banner.h + Frame.MarginTop;
                    Frame.x = (w - h) / 2 + MarginTop + Frame.MarginLeft;
                    Frame.x = x + Frame.MarginLeft;
                    //Frame.x -= 200;
                    Frame.y = -(w - h) / 2 + MarginRight + Banner.h + Frame.MarginTop;
                    Frame.y = y + Frame.MarginTop;
                    //Frame.y = 0;
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
                if (this.Orientation == PdfSharp.PageOrientation.Portrait)
                {
                    Banner.Draw();
                }
                else
                {
                    gfx.TranslateTransform(this.Width / 2, this.Height / 2);
                    gfx.RotateTransform(90);
                    gfx.TranslateTransform(-this.Width / 2, -this.Height / 2);
                    Banner.Draw();
                    gfx.TranslateTransform(this.Width / 2, this.Height / 2);
                    gfx.RotateTransform(-90);
                    gfx.TranslateTransform(-this.Width / 2, -this.Height / 2);
                }
            }
            #endregion

            #region draw frame
            if (Frame != null) 
            { 
                if (this.Orientation == PdfSharp.PageOrientation.Portrait)
                {
                    Frame.Draw();
                }
                else
                {
                    //XSolidBrush brush = new XSolidBrush(XColors.AliceBlue);
                    //gfx.DrawRectangle(brush, x, y, w, h);

                    //gfx.TranslateTransform(this.Width / 2, this.Height / 2);
                    //gfx.RotateTransform(90);
                    //gfx.TranslateTransform(-this.Width / 2, -this.Height / 2);

                    XSolidBrush brush = new XSolidBrush(XColors.AliceBlue);
                    gfx.DrawRectangle(brush, x, y, w, h);
                    //gfx.TranslateTransform(this.w / 2, this.h / 2);
                    //gfx.RotateTransform(90);
                    //gfx.TranslateTransform(-this.w / 2, -this.h / 2);

                    Frame.Draw();

                    gfx.TranslateTransform(this.Width / 2, this.Height / 2);
                    gfx.RotateTransform(-90);
                    gfx.TranslateTransform(-this.Width / 2, -this.Height / 2);

                    //gfx.TranslateTransform(this.w / 2, this.h / 2);
                    //gfx.RotateTransform(-90);
                    //gfx.TranslateTransform(-this.w / 2, -this.h / 2);
                }
            }
            #endregion

            #region draw rows
            yPos = y;
            yPosRotate = y - (this.Width - this.Height) / 2;
            yPosRotate -= Frame.MarginLeft + Frame.WidthLeft + Frame.PaddingLeft;
            yPosRotate += Frame.MarginTop + Frame.WidthTop + Frame.PaddingTop + VSpace;

            //foreach (Row row in Rows) 
            //{
            //    row.Parent = this;
            //    if (row.Rotate.ToLower() == "true")
            //    {
            //        gfx.TranslateTransform(this.Width / 2, this.Height / 2);
            //        gfx.RotateTransform(90);
            //        gfx.TranslateTransform(-this.Width / 2, -this.Height / 2);
            //        row.y = yPosRotate;
            //        row.Draw();
            //        yPosRotate += row.h + VSpace;
            //        this.w = this.w - row.h;
            //        gfx.TranslateTransform(this.Width / 2, this.Height / 2);
            //        gfx.RotateTransform(-90);
            //        gfx.TranslateTransform(-this.Width / 2, -this.Height / 2);
            //    }
            //    else 
            //    {
            //        row.y = yPos;
            //        //brush = new XSolidBrush(XColors.LightYellow);
            //        //gfx.DrawRectangle(brush, row.x, row.y, row.w, row.h);
            //        row.Draw();
            //        yPos += row.h + VSpace;
            //    }
            //}
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
                switch (Xml.Attribute("orientation").Value.ToLower())
                {
                    case "portrait":
                        this.Orientation = PdfSharp.PageOrientation.Portrait;
                        break;
                    case "landscape":
                        this.Orientation = PdfSharp.PageOrientation.Landscape;
                        break;
                    default:
                        this.Orientation = PdfSharp.PageOrientation.Portrait;
                        break;
                }
            }
            //else
            //    this.Orientation = PdfSharp.PageOrientation.Portrait;
        }
        private void ParseSizeAttribute()
        {
            if (Xml.Attribute("size") != null)
            {
                switch (Xml.Attribute("size").Value.ToLower())
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
            else
                this.Size = PdfSharp.PageSize.Letter;
        }
        private void ParseVSpaceAttribute()
        {
            if (Xml.Attribute("vspace") != null)
            {
                try
                {
                    VSpace = XUnitPt.FromMillimeter(double.Parse(Xml.Attribute("vspace").Value));
                }
                catch (Exception)
                {
                    VSpace = XUnitPt.Zero;
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
