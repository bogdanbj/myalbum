using MyAlbum.Utilities;
using PdfSharpCore;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
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
        public PageBorder PageBorder { get; set; }
        public PageBanner PageBanner { get; set; }
        public List<BaseElement> Elements { get; set; }
        public PdfPage pdfPage { get; set; }
        #endregion

        #region Constructors
        public Page() : base()
        {
            Style = Styles.Page.GetStyle("default") ?? new PageStyle();
            Elements = new List<BaseElement>();

            PageBorder = new PageBorder();
            PageBanner = new PageBanner();
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
                {
                    switch(styleElement.Name.LocalName)
                    {
                        case "frame":
                            PageBorder.Inherit(this);
                            PageBorder.ParseXml(styleElement); 
                            break;
                        case "banner":
                            PageBanner.Inherit(this);
                            PageBanner.ParseXml(styleElement);
                            // handle banner element
                            break;
                        default:
                            var elem = CreateElement(styleElement.Name.LocalName);
                            if (elem != null)
                            {
                                elem.Inherit(this);
                                elem.ParseXml(styleElement);
                                Elements.Add(elem);
                            }
                            break;
                    }
                }
            }

            // Then parse page-specific elements
            foreach (XElement xElement in xElem.Elements())
            {
                var elem = CreateElement(xElement.Name.LocalName);
                if (elem != null)
                {
                    elem.Inherit(this);
                    elem.ParseXml(xElement);
                    Elements.Add(elem);
                }
            }
            //PageNumber = int.Parse(pageElement.Attribute("no")?.Value ?? "0");
        }
        internal void Calculate(XGraphics gfx)
        {
            pdfPage.Orientation = this.Orientation;
            pdfPage.Size = this.Size;
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
            PageBanner.Calculate(gfx, Canvas);

            // Reserve the banner's space so the border and page content start after it.
            if (PageBanner.Rotate)
            {
                // Rotated (landscape) banner occupies a vertical strip on the right
                // (the page rotates 90 deg clockwise), same as a rotated row.
                Canvas.W -= PageBanner.H + PageBanner.MarginLeft + PageBanner.MarginRight + VSpace;
            }
            else
            {
                // Portrait banner occupies a horizontal strip at the top.
                Canvas.Y += PageBanner.H + PageBanner.MarginTop + PageBanner.MarginBottom + VSpace;
                Canvas.H -= PageBanner.H + PageBanner.MarginTop + PageBanner.MarginBottom + VSpace;
            }

            // Calculate the border
            PageBorder.Calculate(gfx, Canvas, Orientation);

            // Adjust the canvas to border's internal space
            Canvas.X = PageBorder.Canvas.X;
            Canvas.Y = PageBorder.Canvas.Y;
            Canvas.W = PageBorder.Canvas.W;
            Canvas.H = PageBorder.Canvas.H;

            // Calculate each element and adjust canvas for next element
            foreach (var element in Elements)
            {
                element.Calculate(gfx, Canvas);

                bool rotate = element is Row { Rotate: true } or Space { Rotate: true};

                //if (element is Text)
                //{
                //    var a = element as Text;
                //}

                // Adjust canvas for next element (assuming vertical stacking)
                // A rotated Row occupies a vertical strip.
                if (rotate)
                {
                    // Shrink the Canvas width by rotated element height + margins
                    Canvas.W -= element.H + element.MarginTop + element.MarginBottom + VSpace;
                }
                else
                {
                    // Move Y down by element height + margins
                    Canvas.Y += element.H + element.MarginTop + element.MarginBottom + VSpace;
                    Canvas.H -= element.H + element.MarginTop + element.MarginBottom + VSpace;
                }
            }


        }
        internal void Draw(XGraphics gfx)
        {
            Console.WriteLine();
            Console.WriteLine($"Page {this.PageNo} - {this.Title}.");
            //Console.WriteLine($"Drawing {this.GetType().Name} at ({X.Millimeter:F1}, {Y.Millimeter:F1}) with width {W.Millimeter:F1} and height {H.Millimeter:F1}.");
            base.Draw(gfx);

            #region Testing only
            //gfx.DrawRectangle(
            //    new XSolidBrush(BgColor),
            //    0,
            //    0,
            //    pdfPage.Width,
            //    pdfPage.Height);
            //gfx.DrawRectangle(
            //    new XSolidBrush(XColors.MistyRose),
            //    Canvas.X,
            //    Canvas.Y,
            //    Canvas.W,
            //    Canvas.H);
            //// Debug: Draw page bounds
            //gfx.DrawRectangle(new XPen(XColors.Red, XUnit.FromMillimeter(1)), X, Y, W, H);
            //// Debug: Draw border dimensions
            //gfx.DrawRectangle(new XPen(XColors.Green, XUnit.FromMillimeter(1)), PageBorder.X, PageBorder.Y, PageBorder.W, PageBorder.H);
            //// Debug: Draw page canvas
            //gfx.DrawRectangle(new XPen(XColors.Blue, XUnit.FromMillimeter(1)), Canvas.X, Canvas.Y, Canvas.W, Canvas.H);
            #endregion


            // Draw the banner separately (it is not part of Elements), before the border.
            if (PageBanner.Rotate)
            {
                //gfx.TranslateTransform(pdfPage.Width / 2, pdfPage.Height / 2);
                //gfx.RotateTransform(90);
                //gfx.TranslateTransform(-pdfPage.Height / 2, -pdfPage.Width / 2);
                gfx.TranslateTransform(PageBanner.Pivot.X, PageBanner.Pivot.Y);
                gfx.RotateTransform(90);
                gfx.TranslateTransform(-PageBanner.Pivot.Y, -PageBanner.Pivot.X);
            }
            Console.Write("  ");
            PageBanner.Draw(gfx);
            if (PageBanner.Rotate)
            {
                //gfx.TranslateTransform(pdfPage.Height / 2, pdfPage.Width / 2);
                //gfx.RotateTransform(-90);
                //gfx.TranslateTransform(-pdfPage.Width / 2, -pdfPage.Height / 2);
                gfx.TranslateTransform(PageBanner.Pivot.Y, PageBanner.Pivot.X);
                gfx.RotateTransform(-90);
                gfx.TranslateTransform(-PageBanner.Pivot.X, -PageBanner.Pivot.Y);
            }

            Console.Write("  ");
            PageBorder.Draw(gfx);


            // Implement drawing logic for the page and its elements
            foreach (BaseElement element in Elements)
            {
                // A rotated Row needs the transform applied.
                bool rotate = element is Row { Rotate: true } or Space { Rotate: true };

                // rotate
                if (rotate)
                {
                    //gfx.TranslateTransform(pdfPage.Width / 2, pdfPage.Height / 2);
                    //gfx.RotateTransform(90);
                    //gfx.TranslateTransform(-pdfPage.Height / 2, -pdfPage.Width / 2);
                    gfx.TranslateTransform(element.Pivot.X, element.Pivot.Y);
                    gfx.RotateTransform(90);
                    gfx.TranslateTransform(-element.Pivot.Y, -element.Pivot.X);
                }
                // draw
                Console.Write("  ");
                element.Draw(gfx);
                // rotate back
                if (rotate)
                {
                    //gfx.TranslateTransform(pdfPage.Height / 2, pdfPage.Width / 2);
                    //gfx.RotateTransform(-90);
                    //gfx.TranslateTransform(-pdfPage.Width / 2, -pdfPage.Height / 2);
                    gfx.TranslateTransform(element.Pivot.Y, element.Pivot.X);
                    gfx.RotateTransform(-90);
                    gfx.TranslateTransform(-element.Pivot.X, -element.Pivot.Y);
                }


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
    }
}
