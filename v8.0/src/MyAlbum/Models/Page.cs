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
            /*  Width/Height is Outbound. */
            /*  Substract Margin to get Canvas. */

            // Size first, then orientation, then Base.Calculate().
            // Order is important because the orientation swaps the width and height and Base.Calculate uses pdfPage.Width and Height.
            pdfPage.Size = this.Size;
            pdfPage.Orientation = this.Orientation;
            base.Calculate(gfx, new Canvas());

            // Page Banner - banner/image have Margin but no Padding
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
                    PageBanner.X = Canvas.X;
                    PageBanner.Y = Canvas.Y;
                    Canvas.Y += PageBanner.H + PageBanner.MarginTop + PageBanner.MarginBottom + VSpace;
                    Canvas.H -= PageBanner.H + PageBanner.MarginTop + PageBanner.MarginBottom + VSpace;
                }
            }

            // Page Border - border/frame does not have Margin but has Padding
            if (PageBorder != null)
            {
                PageBorder.Calculate(gfx, Canvas);

                if (PageBorder.Rotate)
                {
                    PageBorder.X = Canvas.X + Canvas.W;
                    PageBorder.Y = Canvas.Y;
                    Canvas.X += PageBorder.WidthBottom + PageBorder.MarginBottom;
                    Canvas.Y += PageBorder.WidthLeft + PageBorder.MarginLeft;
                    Canvas.W = PageBorder.H - (PageBorder.WidthBottom + PageBorder.MarginBottom + PageBorder.MarginTop + PageBorder.WidthTop);
                    Canvas.H = PageBorder.W - (PageBorder.WidthLeft + PageBorder.MarginLeft + PageBorder.MarginRight + PageBorder.WidthRight);
                }
                else
                {
                    PageBorder.X = Canvas.X;
                    PageBorder.Y = Canvas.Y;
                    Canvas.X += PageBorder.Canvas.X;
                    Canvas.Y += PageBorder.Canvas.Y;
                    Canvas.W = PageBorder.Canvas.W;
                    Canvas.H = PageBorder.Canvas.H;
                }
            }

            // Calculate each element and adjust canvas for next element
            foreach (var element in Elements)
            {
                element.Calculate(gfx, Canvas);

                bool rotate = element is Row { Rotate: true } or Space { Rotate: true};
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
                    if (element is Column)
                    {
                        Canvas.X += element.W + element.MarginLeft + element.MarginRight;
                        Canvas.W -= element.W + element.MarginLeft + element.MarginRight;
                    }
                    else if (element is Space)
                    {
                        Canvas.X += element.W + element.MarginLeft + element.MarginRight;
                        Canvas.W -= element.W + element.MarginLeft + element.MarginRight;
                        Canvas.Y += element.H + element.MarginTop + element.MarginBottom;
                        Canvas.H -= element.H + element.MarginTop + element.MarginBottom;
                    }
                    else
                    {
                        Canvas.Y += element.H + element.MarginTop + element.MarginBottom + VSpace;
                        Canvas.H -= element.H + element.MarginTop + element.MarginBottom + VSpace;
                    }
                }
            }
        }

        internal override XUnit CalculateWidth(XUnit parentWidth)
        {
            return pdfPage.Width;
        }
        internal override XUnit CalculateHeight(XUnit parentHeight)
        {
            return pdfPage.Height;
        }

        internal override void Draw(XGraphics gfx)
        {
            Console.WriteLine();
            Console.WriteLine($"Page {this.PageNo} - {this.Title}.");
            base.Draw(gfx);

            if (PageBanner != null)
            {
                DrawElement(gfx, PageBanner, PageBanner.Rotate);
            }

            if (PageBorder != null)
            {
                DrawElement(gfx, PageBorder, PageBorder.Rotate);
            }

            foreach (BaseElement element in Elements)
            {
                bool rotate = element is Row { Rotate: true } or Space { Rotate: true };
                DrawElement(gfx, element, rotate);
            }

            // Draw remaining canvas
            //gfx.DrawRectangle(new XPen(Color, 0.5), new XSolidBrush(XColors.MistyRose), Canvas.X, Canvas.Y, Canvas.W, Canvas.H);
            if (IsDebug)
            {
                gfx.DrawRectangle(
                    new XSolidBrush(XColors.MistyRose),
                    Canvas.X.Point,
                    Canvas.Y.Point,
                    Canvas.W.Point,
                    Canvas.H.Point);

                //// Label the canvas interior in the top-left corner
                //XFont canvasFont = new XFont("Verdana", 12);
                //XRect canvasLabelRect = new XRect(
                //    (Canvas.X + XUnit.FromMillimeter(2)).Point,
                //    (Canvas.Y + XUnit.FromMillimeter(1)).Point,
                //    (Canvas.W - XUnit.FromMillimeter(2)).Point,
                //    (Canvas.H - XUnit.FromMillimeter(1)).Point);
                //gfx.DrawString($"Canvas - Page {this.PageNo} - {this.Title}.", canvasFont, XBrushes.Black, canvasLabelRect, XStringFormats.TopLeft);
            }
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
