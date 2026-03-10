using MyAlbum.Models.Xml;
using MyAlbum.Models.Xml.Styles;
using MyAlbum.Services;
using MyAlbum.Utils;
using PdfSharpCore;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;


namespace MyAlbum.Models.Layout
{
    internal class Page : BaseElement
    {
        // Store dependencies as properties
        private XmlPage _xmlPage;
        private List<PageStyle> _styles;

        public int Number { get; set; }
        public PageOrientation Orientation { get; set; }
        public PageSize Size { get; set; }
        //public XUnit VSpace { get; set; }
        public Border PageBorder { get; set; }
        public List<BaseElement> Elements { get; set; }
        public PdfPage pdfPage { get; set; }


        public Page()
        {
            Orientation = PageOrientation.Portrait;
            Size = PageSize.Letter;
            X = 0;
            Y = 0;
            Elements = new List<BaseElement>();
            BgColor = XColors.White;
        }

        internal void FromXml (XmlPage xmlPage, AlbumStyles styles)
        {
            PageNumber = Number = xmlPage.Number;
            
            // Find the appropriate page style
            PageStyle? style = StyleFactory.FindStyle<PageStyle>(xmlPage.Style, styles.PageStyles);
            if (style == null)
            {
                throw new InvalidOperationException(
                    $"Page style '{xmlPage.Style ?? "(default)"}' not found. " +
                    $"Ensure a matching PageStyle exists in the album styles or that a default PageStyle is defined.");
            }
            
            base.FromXml(xmlPage, style);

            // Apply the style
            Orientation = ParseOrientation(xmlPage.Orientation ?? style.Orientation);
            Size = ParsePageSize(xmlPage.Size ?? style.Size);
            //(MarginTop, MarginRight, MarginBottom, MarginLeft) = ParseMargin(xmlPage.Margin ?? style.Margin);
            //(PaddingTop, PaddingRight, PaddingBottom, PaddingLeft) = (0, 0, 0, 0);  // Page has no padding. Only Margin
            VSpace = ParseXUnit(xmlPage.VSpace ?? style.VSpace);
            Color = ParseColor(xmlPage.Color ?? style.Color ?? $"{Color.R},{Color.G},{Color.B}");
            BgColor = ParseColor(xmlPage.BgColor ?? style.BgColor ?? $"{BgColor.R},{BgColor.G},{BgColor.B}");



            // Add the Style elements
            foreach (XmlElement xmlElement in style.Elements)
            {
                // Add Element to the layoutPage
                switch (xmlElement)
                {
                    case XmlBorder xmlBorder:
                        PageBorder = new Border();
                        PageBorder.Inherit(this);
                        PageBorder.FromXml(xmlBorder, styles);
                        PageBorder.Orientation = Orientation; // Ensure the border knows the page orientation for calculations  
                        //this.Elements.Add(PageBorder);
                        break;

                    case XmlRow xmlRow:
                        Row row = new Row();
                        row.Inherit(this);
                        row.FromXml(xmlRow, styles);
                        this.Elements.Add(row);
                        break;

                    case XmlSpace xmlSpace:
                        Space space= new Space();
                        space.Inherit(this);
                        space.FromXml(xmlSpace, styles);
                        this.Elements.Add(space);
                        break;

                }
            }

            // Add the Page's own elements
            foreach (XmlElement xmlElement in xmlPage.Elements)
            {
                // Add Element to the layoutPage
                switch (xmlElement)
                {
                    case XmlBorder xmlBorder:
                        PageBorder = new Border();
                        PageBorder.Inherit(this);
                        PageBorder.FromXml(xmlBorder, styles);
                        //this.Elements.Add(PageBorder);
                        break;

                    case XmlRow xmlRow:
                        Row row = new Row();
                        row.Inherit(this);
                        row.FromXml(xmlRow, styles);
                        this.Elements.Add(row);
                        break;

                    case XmlSpace xmlSpace:
                        Space space = new Space();
                        space.Inherit(this);
                        space.FromXml(xmlSpace, styles);
                        this.Elements.Add(space);
                        break;
                }
            }
        }


        internal override void Calculate(XGraphics gfx, XUnit x, XUnit y, XUnit w, XUnit h)
        {
            // Ignore the parameters for the page itself 
            // The Page defines its own dimensions based on size and orientation
            pdfPage.Orientation = this.Orientation;
            pdfPage.Size = this.Size;

            // If Landscape, shift attributes 90 degrees counterclockwise
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
            PageBorder.Calculate(gfx, Canvas.X, Canvas.Y, Canvas.W, Canvas.H);

            // Adjust the canvas to border's internal space
            Canvas.X = PageBorder.Canvas.X;
            Canvas.Y = PageBorder.Canvas.Y;
            Canvas.W = PageBorder.Canvas.W;
            Canvas.H = PageBorder.Canvas.H;

            foreach (BaseElement element in Elements)
            {

                if (element is Row row)
                {
                    row.CalculateSize(gfx, Canvas.W, Canvas.H);

                    // If a row is added to the page, re-adjust the remaining canvas area
                    if (row.Rotate)
                    {
                        row.X = Canvas.Y;
                        row.Y = Canvas.X;
                        
                        Canvas.X = Canvas.X;           // no change here
                        Canvas.W -= row.H + VSpace;    // shrink the width
                    }
                    else
                    {
                        row.X = Canvas.X;
                        row.Y = Canvas.Y;
                        
                        Canvas.Y += row.H + VSpace;
                        Canvas.H -= row.H + VSpace;
                    }
                    row.CalculateInnerPositions();
                }

                if (element is Space space)
                {
                    space.CalculateSize(gfx, Canvas.W, XUnit.Zero);

                    space.X = Canvas.X;
                    space.Y = Canvas.Y;
                    Canvas.Y += space.H;
                    Canvas.H -= space.H;
                }

            }

        }

        internal override void Draw(XGraphics gfx)
        {

            #region test/debug drawing
            //Helper.Fill(gfx, this);
            //Helper.FillCanvas(gfx, this);
            //Helper.MarkCorners(gfx, this.Canvas, false);
            //Helper.Write(gfx, Canvas, "CANVAS");
            #endregion

            PageBorder.Draw(gfx);

            // Implement drawing logic for the page and its elements
            foreach (BaseElement element in Elements)
            {
                // rotate
                if (element.Rotate)
                {
                    gfx.TranslateTransform(pdfPage.Width / 2, pdfPage.Height / 2);
                    gfx.RotateTransform(90);
                    gfx.TranslateTransform(-pdfPage.Height / 2, -pdfPage.Width / 2);
                }
                // draw
                element.Draw(gfx);
                // rotate back
                if (element.Rotate)
                {
                    gfx.TranslateTransform(pdfPage.Height / 2, pdfPage.Width / 2);
                    gfx.RotateTransform(-90);
                    gfx.TranslateTransform(-pdfPage.Width / 2, -pdfPage.Height / 2);
                }
            }
            //Helper.DrawCorner(gfx, Canvas.X, Canvas.Y, XColors.Red);
            //XPen pen = new XPen(XColors.Red, XUnit.FromMillimeter(0.1));
            //gfx.DrawLine(pen, Canvas.X, Canvas.Y, Canvas.X + XUnit.FromMillimeter(10), Canvas.Y);
            //gfx.DrawLine(pen, Canvas.X, Canvas.Y, Canvas.X, Canvas.Y + XUnit.FromMillimeter(10));
        }

        private static PageOrientation ParseOrientation(string orientation)
        {
            if (!string.IsNullOrWhiteSpace(orientation) && Enum.TryParse(orientation, true, out PageOrientation result))
            {
                return result;
            }
            return default;
        }
        private static PageSize ParsePageSize(string size)
        {
            if (!string.IsNullOrWhiteSpace(size) && Enum.TryParse(size, true, out PageSize result))
            {
                return result;
            }
            return default;
        }

        #region helper and debug functions
        internal override XColor GetDebugColor() => XColors.Bisque;

        #endregion

    }
}
