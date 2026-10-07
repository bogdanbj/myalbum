using MyAlbum.Utilities;
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace MyAlbum.Models
{
    internal class Page : BaseElement<PageStyle>
    {
        #region Fields
        protected PageOrientation? _orientation;
        protected PageSize? _size;
        //margins
        protected XUnit? _vSpace;
        #endregion

        #region Style properties
        //public new PageStyle? Style 
        //{ 
        //    get => (PageStyle)base.Style; 
        //    set => base.Style = value; 
        //}
        public PageOrientation Orientation 
        { 
            get => _orientation ?? Style?.Orientation ?? PageOrientation.Portrait; 
            set => _orientation = value; 
        }
        public PageSize Size 
        { 
            get => _size ?? Style?.Size ?? PageSize.Letter; 
            set => _size = value; 
        }
        public XUnit VSpace 
        { 
            get => _vSpace ?? Style?.VSpace ?? XUnit.Zero;
            set => _vSpace = value; 
        }
        #endregion

        #region Other properties
        public string? Title { get; set; }
        public PageBanner? PageBanner { get; set; }
        public PageBorder? PageBorder { get; set; }
        public List<BaseElement> Elements { get; set; }
        public PdfPage pdfPage { get; set; }
        #endregion

        #region Constructors
        public Page() : base()
        {
            Style = Styles.Page.GetStyle("default") ?? new PageStyle();
            Elements = new List<BaseElement>();

            //PageBanner = new PageBanner();
            //PageBorder = new PageBorder();
        }
        #endregion

        #region Override methods
        internal override void ParseXml(XElement xElem)
        {
            Style = Styles.Page.GetStyle(xElem.Attribute("style")?.Value);
            base.ParseXml(xElem);

            Title = xElem.Attribute("title")?.Value ?? "";
            _orientation = XmlParser.ParseOrientation(xElem.Attribute("orientation")?.Value);
            _size = XmlParser.ParsePageSize(xElem.Attribute("size")?.Value);
            _vSpace = XmlParser.ParseXUnit(xElem.Attribute("vspace")?.Value);

            // Add elements from style first
            if (Style?.ChildElements != null)
            {
                foreach (var styleElement in Style.ChildElements)
                    ParseChild(styleElement);
            }

            // Then parse page-specific elements
            foreach (XElement xElement in xElem.Elements())
                ParseChild(xElement);

        }
        internal void Calculate(XGraphics gfx)
        {
            // size first, then orientation. Order is important because the orientation swaps the width and height.
            pdfPage.Size = this.Size;
            pdfPage.Orientation = this.Orientation;
            X = XUnit.Zero;
            Y = XUnit.Zero;
            W = pdfPage.Width;
            H = pdfPage.Height;

            // Adjust the Canvas area based on margins
            Canvas.X = X + PaddingLeft;
            Canvas.Y = Y + PaddingTop;
            Canvas.W = W - PaddingLeft - PaddingRight;
            Canvas.H = H - PaddingTop - PaddingBottom;


            // Calculate the banner against the full page canvas (outside the border)
            if (PageBanner != null)
            {
                PageBanner.Calculate(gfx, Canvas);

                if (PageBanner.Rotate)
                {
                    PageBanner.X = Canvas.X + Canvas.W - PageBanner.MarginTop;
                    PageBanner.Y = Canvas.Y + PageBanner.MarginLeft;
                    Canvas.W -= PageBanner.H + PageBanner.MarginTop + PageBanner.MarginBottom + VSpace;
                }
                else
                {
                    PageBanner.X = Canvas.X + PageBanner.MarginLeft;
                    PageBanner.Y = Canvas.Y + PageBanner.MarginTop;
                    Canvas.Y += PageBanner.H + PageBanner.MarginTop + PageBanner.MarginBottom + VSpace;
                    Canvas.H -= PageBanner.H + PageBanner.MarginTop + PageBanner.MarginBottom + VSpace;
                }
            }

            // Calculate the border
            if (PageBorder != null)
            {
                PageBorder.Calculate(gfx, Canvas);

                if (PageBorder.Rotate)
                {
                    PageBorder.X = Canvas.X + Canvas.W - PageBorder.MarginTop;
                    PageBorder.Y = Canvas.Y + PageBorder.MarginLeft;
                    Canvas.X = Canvas.X + PageBorder.MarginBottom + PageBorder.WidthBottom + PageBorder.PaddingBottom;
                    Canvas.Y = Canvas.Y + PageBorder.MarginLeft + PageBorder.WidthLeft + PageBorder.PaddingLeft;
                    Canvas.W = PageBorder.H - (PageBorder.WidthBottom + PageBorder.PaddingBottom + PageBorder.PaddingTop + PageBorder.WidthTop);
                    Canvas.H = PageBorder.W - (PageBorder.WidthLeft + PageBorder.PaddingLeft + PageBorder.PaddingRight + PageBorder.WidthRight);
                }
                else
                {
                    PageBorder.X = Canvas.X + PageBorder.MarginLeft;
                    PageBorder.Y = Canvas.Y + PageBorder.MarginTop;
                    Canvas.X = PageBorder.X + PageBorder.WidthLeft + PageBorder.PaddingLeft;
                    Canvas.Y = PageBorder.Y + PageBorder.WidthTop + PageBorder.PaddingTop;
                    Canvas.W = PageBorder.W - (PageBorder.WidthLeft + PageBorder.PaddingLeft + PageBorder.PaddingRight + PageBorder.WidthRight);
                    Canvas.H = PageBorder.H - (PageBorder.WidthTop + PageBorder.PaddingTop + PageBorder.PaddingBottom + PageBorder.WidthBottom);
                }
            }
            // Calculate each element and adjust canvas for next element
            foreach (var element in Elements)
            {
                element.Calculate(gfx, Canvas);

                bool rotate = element is Row { Rotate: true } or Space { Rotate: true};

                //if (element is Text)
                //{
                //    var a = element as Text;
                //}

                if (rotate)
                {
                    element.X = Canvas.X + Canvas.W - element.MarginTop;
                    element.Y = Canvas.Y + element.MarginLeft;
                    Canvas.W -= element.H + element.MarginTop + element.MarginBottom + VSpace;
                }
                else
                {
                    element.X = Canvas.X + element.MarginLeft;
                    element.Y = Canvas.Y + element.MarginTop;
                    Canvas.Y += element.H + element.MarginTop + element.MarginBottom + VSpace;
                    Canvas.H -= element.H + element.MarginTop + element.MarginBottom + VSpace;
                }
            }


        }
        internal void Draw(XGraphics gfx)
        {
            Console.WriteLine();
            Console.WriteLine($"Page {this.PageNo} - {this.Title}.");
            base.Draw(gfx);

            if (PageBanner != null)
            {
                Console.Write("  ");
                DrawElement(gfx, PageBanner, PageBanner.Rotate);
            }

            if (PageBorder != null)
            {
                Console.Write("  ");
                //PageBorder.Draw(gfx);
                DrawElement(gfx, PageBorder, PageBorder.Rotate);
            }

            // Implement drawing logic for the page and its elements
            foreach (BaseElement element in Elements)
            {
                // A rotated Row needs the transform applied.
                bool rotate = element is Row { Rotate: true } or Space { Rotate: true };

                Console.Write("  ");
                DrawElement(gfx, element, rotate);

                //// rotate
                //if (rotate)
                //{
                //    //gfx.TranslateTransform(pdfPage.Width / 2, pdfPage.Height / 2);
                //    //gfx.RotateTransform(90);
                //    //gfx.TranslateTransform(-pdfPage.Height / 2, -pdfPage.Width / 2);
                //    gfx.TranslateTransform(element.Pivot.X, element.Pivot.Y);
                //    gfx.RotateTransform(90);
                //    gfx.TranslateTransform(-element.Pivot.Y, -element.Pivot.X);
                //}
                //// draw
                //Console.Write("  ");
                //element.Draw(gfx);
                //// rotate back
                //if (rotate)
                //{
                //    //gfx.TranslateTransform(pdfPage.Height / 2, pdfPage.Width / 2);
                //    //gfx.RotateTransform(-90);
                //    //gfx.TranslateTransform(-pdfPage.Width / 2, -pdfPage.Height / 2);
                //    gfx.TranslateTransform(element.Pivot.Y, element.Pivot.X);
                //    gfx.RotateTransform(-90);
                //    gfx.TranslateTransform(-element.Pivot.X, -element.Pivot.Y);
                //}


                // If maore than one element has Rotate
                //bool shouldRotate = element switch
                //{
                //    Row r => r.Rotate,
                //    Column c => c.Rotate,  // if Column also has Rotate
                //    _ => false
                //};

                //if (shouldRotate)
                //{
                //    // rotation logic
                //}
            }

            // Draw remaining canvas
            //gfx.DrawRectangle(new XPen(Color, 0.5), new XSolidBrush(XColors.MistyRose), Canvas.X, Canvas.Y, Canvas.W, Canvas.H);
            gfx.DrawRectangle(new XSolidBrush(XColors.MistyRose), Canvas.X, Canvas.Y, Canvas.W, Canvas.H);

            // Label the canvas interior in the top-left corner
            XFont canvasFont = new XFont("Verdana", 12);
            XRect canvasLabelRect = new XRect(
                Canvas.X + XUnit.FromMillimeter(2),
                Canvas.Y + XUnit.FromMillimeter(1),
                Canvas.W - XUnit.FromMillimeter(2),
                Canvas.H - XUnit.FromMillimeter(1));
            gfx.DrawString($"Canvas - Page {this.PageNo} - {this.Title}.", canvasFont, XBrushes.Black, canvasLabelRect, XStringFormats.TopLeft);
            //Console.WriteLine($"Page {this.PageNo} - {this.Title}.");
        }
        #endregion

        #region Private methods
        private void ParseChild(XElement xElement)
        {
            switch (xElement.Name.LocalName)
            {
                case "banner":
                    PageBanner ??= new PageBanner();
                    PageBanner.Inherit(this);
                    PageBanner.ParseXml(xElement);
                    break;
                case "border":
                    PageBorder ??= new PageBorder();
                    PageBorder.Inherit(this);
                    PageBorder.ParseXml(xElement);
                    break;
                default:
                    var elem = CreateElement(xElement.Name.LocalName);
                    if (elem != null)
                    {
                        elem.Inherit(this);
                        elem.ParseXml(xElement);
                        Elements.Add(elem);
                    }
                    break;
            }
        }
        #endregion
    }
}
