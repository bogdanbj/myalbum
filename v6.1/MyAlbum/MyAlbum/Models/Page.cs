using PdfSharpCore;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using MyAlbum.Utilities;

namespace MyAlbum.Models
{
    internal class Page : BaseElement
    {
        #region Fields
        protected PageOrientation? _orientation;
        protected PageSize? _size;
        //margins
        //vspace
        #endregion

        #region Properties accepting Styles 
        public new PageStyle Style 
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
        //public XColor Color
        //{
        //    get => _color ?? Style?.Color ?? _parentColor ?? XColors.Black;
        //    set => _color = value;
        //}
        //public XColor BgColor
        //{
        //    get => _bgColor ?? Style?.BgColor ?? _parentBgColor ?? XColors.Transparent;
        //    set => _bgColor = value;
        //}
        #endregion

        #region Other properties
        public string? Title { get; set; }
        public Border PageBorder { get; set; }
        public List<BaseElement> Elements { get; set; }
        public PdfPage pdfPage { get; set; }
        #endregion

        #region Constructors
        public Page() : base()
        {
            PageBorder = new Border();
            Elements = new List<BaseElement>();
        }

        #endregion

        internal void ParseXml(XElement xPage)
        {
            base.ParseXml(xPage);
            Style = Styles.Page.GetStyle(xPage.Attribute("style")?.Value);
            Title = xPage.Attribute("title")?.Value ?? "";
            _orientation = XmlParser.ParseOrientation(xPage.Attribute("orientation")?.Value);
            _size = XmlParser.ParsePageSize(xPage.Attribute("size")?.Value);



            //PageNumber = int.Parse(pageElement.Attribute("no")?.Value ?? "0");

            foreach (XElement xElement in xPage.Elements())
            {
                switch (xElement.Name.LocalName)
                {
                    case "border":
                        Border border = new Border();
                        border.Inherit(this);
                        border.ParseXml(xElement);
                        this.Elements.Add(border);
                        break;
                    case "column":
                        Column column = new Column();
                        column.Inherit(this);
                        column.ParseXml(xElement);
                        this.Elements.Add(column);
                        break;
                    case "image":
                        Image image = new Image();
                        image.Inherit(this);
                        image.ParseXml(xElement);
                        this.Elements.Add(image);
                        break;
                    case "row":
                        Row row = new Row();
                        row.Inherit(this);
                        row.ParseXml(xElement);
                        this.Elements.Add(row);
                        break;
                    case "stamp":
                        Stamp stamp = new Stamp();
                        stamp.Inherit(this);
                        stamp.ParseXml(xElement);
                        this.Elements.Add(stamp);
                        break;
                    case "text":
                        Text text = new Text();
                        text.Inherit(this);
                        text.ParseXml(xElement);
                        this.Elements.Add(text);
                        break;
                }
                //// Add Element to the layoutPage
                //switch (xmlElement)
                //{
                //    case XmlBorder xmlBorder:
                //        PageBorder = new Border();
                //        PageBorder.Inherit(this);
                //        PageBorder.FromXml(xmlBorder, styles);
                //        //this.Elements.Add(PageBorder);
                //        break;

                //    case XmlRow xmlRow:
                //        Row row = new Row();
                //        row.Inherit(this);
                //        row.FromXml(xmlRow, styles);
                //        this.Elements.Add(row);
                //        break;

                //    case XmlSpace xmlSpace:
                //        Space space = new Space();
                //        space.Inherit(this);
                //        space.FromXml(xmlSpace, styles);
                //        this.Elements.Add(space);
                //        break;

                //    case XmlText xmlText:
                //        Text text = new Text();
                //        text.Inherit(this);
                //        text.FromXml(xmlText, styles);
                //        this.Elements.Add(text);
                //        break;
                //}
            }


        }

        internal /*override*/ void Calculate(XGraphics gfx)
        {
            pdfPage.Orientation = this.Orientation;
            pdfPage.Size = this.Size;

            // If Landscape, the page rotates counterclockwise. Shift attributes 90 degrees clockwise.
            if (Orientation == PageOrientation.Landscape)
            {
                // margins
                XUnit m = MarginTop;
                MarginTop = MarginLeft;
                MarginLeft = MarginBottom;
                MarginBottom = MarginRight;
                MarginRight = m;
            }
            
            // Adjust the Canvas area based on margins
            Canvas.X = MarginLeft;
            Canvas.Y = MarginTop;
            Canvas.W = pdfPage.Width - MarginLeft - MarginRight;
            Canvas.H = pdfPage.Height - MarginTop - MarginBottom;

            // Calculate the border
            PageBorder.Inherit(this);
            PageBorder.Calculate(gfx, Canvas);

            // Adjust the canvas to border's internal space
            Canvas.X = PageBorder.Canvas.X;
            Canvas.Y = PageBorder.Canvas.Y;
            Canvas.W = PageBorder.Canvas.W;
            Canvas.H = PageBorder.Canvas.H;
        }

        internal void Draw(XGraphics gfx)
        {
#if DEBUG
            Console.WriteLine($"Drawing page {PageNo} with title '{Title}'.");
            gfx.DrawRectangle(
                new XSolidBrush(BgColor),
                0,
                0,
                pdfPage.Width,
                pdfPage.Height);
#endif
            foreach (var element in Elements)
            {
                element.Draw(gfx);
            }
        }

        //private static PageOrientation ParseOrientation(string orientation)
        //{
        //    if (!string.IsNullOrWhiteSpace(orientation) && Enum.TryParse(orientation, true, out PageOrientation result))
        //    {
        //        return result;
        //    }
        //    return default;
        //}
        //private static PageSize ParsePageSize(string size)
        //{
        //    if (!string.IsNullOrWhiteSpace(size) && Enum.TryParse(size, true, out PageSize result))
        //    {
        //        return result;
        //    }
        //    return default;
        //}

    }
}
