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
    internal class Page : BaseElement
    {
        #region Fields
        protected PageOrientation? _orientation;
        protected PageSize? _size;
        //margins
        protected XUnit? _vSpace;
        #endregion

        #region Properties accepting Styles 
        public new PageStyle? Style 
        { 
            get => (PageStyle)base.Style; 
            set => base.Style = value; 
        }
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
        public List<BaseElement> Elements { get; set; }
        public PdfPage pdfPage { get; set; }
        #endregion

        #region Constructors
        public Page() : base()
        {
            PageBorder = new PageBorder();
            Elements = new List<BaseElement>();
        }
        #endregion

        internal override void ParseXml(XElement xPage)
        {
            Style = Styles.Page.GetStyle(xPage.Attribute("style")?.Value);
            base.ParseXml(xPage);

            Title = xPage.Attribute("title")?.Value ?? "";
            _orientation = XmlParser.ParseOrientation(xPage.Attribute("orientation")?.Value);
            _size = XmlParser.ParsePageSize(xPage.Attribute("size")?.Value);
            _vSpace = XmlParser.ParseXUnit(xPage.Attribute("vspace")?.Value);

            // Add elements from style first
            if (Style?.ChildElements != null)
            {
                foreach (var styleElement in Style.ChildElements)
                {
                    var elem = CreateElement(styleElement.Name.LocalName);
                    if (elem != null)
                    {
                        elem.Inherit(this);
                        elem.ParseXml(styleElement);
                        Elements.Add(elem);
                    }
                }
            }

            // Then parse page-specific elements
            foreach (XElement xElement in xPage.Elements())
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
        //protected BaseElement? CreateElement(string elementName)
        //{
        //    return elementName switch
        //    {
        //        "frame" => new Frame(),
        //        "row" => new Row(),
        //        "column" => new Column(),
        //        "image" => new Image(),
        //        "stamp" => new Stamp(),
        //        "text" => new Text(),
        //        _ => null
        //    };
        //}
        //private void InstantiateElement(XElement xElement)
        //{
        //    switch (xElement.Name.LocalName)
        //    {
        //        case "banner":
        //            // Handle banner element
        //            break;
        //        case "frame":
        //            PageBorder = new PageBorder();
        //            PageBorder.Inherit(this);
        //            PageBorder.ParseXml(xElement);
        //            //this.Elements.Add(PageBorder);
        //            break;
        //        case "row":
        //            Row row = new Row();
        //            row.Inherit(this);
        //            row.ParseXml(xElement);
        //            this.Elements.Add(row);
        //            break;
        //        case "column":
        //            Column column = new Column();
        //            column.Inherit(this);
        //            column.ParseXml(xElement);
        //            this.Elements.Add(column);
        //            break;
        //        case "image":
        //            Image image = new Image();
        //            image.Inherit(this);
        //            image.ParseXml(xElement);
        //            this.Elements.Add(image);
        //            break;
        //        case "stamp":
        //            Stamp stamp = new Stamp();
        //            stamp.Inherit(this);
        //            stamp.ParseXml(xElement);
        //            this.Elements.Add(stamp);
        //            break;
        //        case "text":
        //            Text text = new Text();
        //            text.Inherit(this);
        //            text.ParseXml(xElement);
        //            this.Elements.Add(text);
        //            break;
        //    }
        //}
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

                // Adjust canvas for next element (assuming vertical stacking)
                if (element is Row { Rotate: true })
                {
                    // Shrink the Canvas width by rotated row height + margins
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


            Console.Write("  ");
            PageBorder.Draw(gfx);


            // Implement drawing logic for the page and its elements
            foreach (BaseElement element in Elements)
            {
                // rotate
                if (element is Row { Rotate: true })
                {
                    gfx.TranslateTransform(pdfPage.Width / 2, pdfPage.Height / 2);
                    gfx.RotateTransform(90);
                    gfx.TranslateTransform(-pdfPage.Height / 2, -pdfPage.Width / 2);
                }
                // draw
                Console.Write("  ");
                element.Draw(gfx);
                // rotate back
                if (element is Row { Rotate: true })
                {
                    gfx.TranslateTransform(pdfPage.Height / 2, pdfPage.Width / 2);
                    gfx.RotateTransform(-90);
                    gfx.TranslateTransform(-pdfPage.Width / 2, -pdfPage.Height / 2);
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
            gfx.DrawRectangle(new XPen(Color, 0.5), new XSolidBrush(XColors.MistyRose), Canvas.X, Canvas.Y, Canvas.W, Canvas.H);

        }
    }
}
