using MyAlbum.Models.Xml;
using MyAlbum.Models.Xml.Styles;
using MyAlbum.Services;
using MyAlbum.Utils;
using PdfSharpCore.Drawing;


namespace MyAlbum.Models.Layout
{
    internal class Column : BaseElement
    {
        //public XUnit VSpace { get; set; }
        public List<BaseElement> Elements { get; set; }
        public bool CanGrow { get; set; }

        public Column()
        {
            H = XUnit.Zero;
            Elements = new List<BaseElement>();
            //H = XUnit.FromMillimeter(20);
        }
        
        internal void FromXml(XmlColumn xmlColumn, AlbumStyles styles)
        {
            // Find the appropriate column style
            ColumnStyle? style = StyleFactory.FindStyle<ColumnStyle>(xmlColumn.Style, styles.ColumnStyles);
            if (style == null)
            {
                throw new InvalidOperationException(
                    $"Column style '{xmlColumn.Style ?? "(default)"}' not found. " +
                    $"Ensure a matching ColumnStyle exists in the album styles or that a default ColumnStyle is defined.");
            }
            base.FromXml(xmlColumn, style);

            // Apply the style
            Color = ParseColor(xmlColumn.Color ?? style.Color ?? $"{Color.R},{Color.G},{Color.B}");
            BgColor = ParseColor(xmlColumn.BgColor ?? style.BgColor ?? $"{BgColor.R},{BgColor.G},{BgColor.B}");
            Align = ParseAlignment(xmlColumn.Align ?? style.Align ?? "left");
            VAlign = ParseVerticalAlignment(xmlColumn.VAlign ?? style.VAlign ?? "top");
            VSpace = ParseXUnit(xmlColumn.Space ?? style.Space ?? $"{VSpace.Millimeter} mm");
            WidthPercent = 100;

            string width = xmlColumn.Width ?? style.Width; 

            if (width != null)
            {
                try
                {
                    if (width.Contains('%'))
                    {
                        WidthPercent = double.Parse(width.TrimEnd(new char[] { '%', ' ' }));
                    }
                    else
                    {
                        W = ParseXUnit(width);
                    }
                }
                catch (Exception)
                { }
            }

            foreach (XmlElement xmlElement in xmlColumn.Elements)
            {
                // Add Element to the layoutPage
                switch (xmlElement)
                {
                    case XmlRow xmlRow:
                        Row row = new Row();
                        row.Inherit(this);
                        row.FromXml(xmlRow, styles);
                        this.Elements.Add(row);
                        break;

                    case XmlImage xmlImage:
                        Image image = new Image();
                        //image.Inherit(this);
                        //image.FromXml(xmlImage, styles);
                        this.Elements.Add(image);
                        break;

                    case XmlStamp xmlStamp:
                        Stamp stamp = new Stamp();
                        stamp.Inherit(this);
                        stamp.FromXml(xmlStamp, styles);
                        this.Elements.Add(stamp);
                        break;

                    case XmlText xmlText:
                        Text text = new Text();
                        text.Inherit(this);
                        text.FromXml(xmlText, styles);
                        this.Elements.Add(text);
                        break;
                }
            }
        }
        internal override void CalculateSize(XGraphics gfx, XUnit w, XUnit h)
        {
            if (Elements.Count() > 0)
            {
                if (W == XUnit.Zero)
                {
                    W = w * WidthPercent / 100;
                }

                //XUnit xPos = 0;
                //XUnit yPos = 0;

                //XUnit maxWidth = XUnit.Zero;
                for (int i = 0; i < Elements.Count; i++)
                {
                    BaseElement element = Elements[i];
                    bool isLast = (i == Elements.Count - 1);
                    bool isSpace = element is Space;

                    // Calculate size. For Space CalculateSize does not do anything, for other elements it will calculate based on content and available width.
                    element.CalculateSize(gfx, W, h);
                    H += element.H;

                    // Add vertical space between non-Space, non-last elements
                    if (!isSpace && !isLast)
                    {
                        H += VSpace;
                    }
                }
                H += MarginTop + MarginBottom;
                TopAlign = MarginTop + Elements.First().TopAlign;
                MiddleAlign = MarginTop + (H - MarginTop - MarginBottom) / 2;
                BottomAlign = H - MarginBottom - Elements.Last().H + Elements.Last().BottomAlign;
            }
            else
            {
                H += MarginTop + MarginBottom;
                TopAlign = MarginTop;
                MiddleAlign = MarginTop + (H - MarginTop - MarginBottom) / 2;
                BottomAlign = H - MarginBottom;
            }
        }

        internal override void CalculateInnerPositions()
        {
            XUnit yPos = Y;

            foreach (BaseElement element in Elements)
            {
                #region X
                switch (Align)
                {
                    case Alignment.Left:
                        element.X = X;
                        break;
                    case Alignment.Center:
                        element.X = X + (W - element.W) / 2;
                        break;
                    case Alignment.Right:
                        element.X = X + W - element.W;
                        break;
                    default:
                        break;
                }
                #endregion

                #region Y;
                element.Y = yPos;
                yPos += element.H + VSpace;
                #endregion
            
                element.CalculateInnerPositions();
            }
        }

        //if (W == XUnit.Zero)
        //{
        //    W = w * WidthPercent / 100;
        //}

        // not more than parent width

        //XUnitPt height = XUnitPt.Zero;
        ////foreach (BaseElement element in Elements.Where(b => typeof(Container).IsAssignableFrom(b.GetType())))// b.Elements != null))
        //if (Elements.Count() > 0)
        //{
        //    foreach (BaseElement element in Elements)
        //    {
        //        element.gfx = gfx;
        //        element.Calculate();
        //        this.w = Math.Max(this.w, element.w);
        //    }
        //    if (Parent.w != XUnitPt.Zero)
        //    {
        //        this.w = Math.Min(this.w, Parent.w);
        //    }

        //    foreach (BaseElement element in Elements)
        //    {
        //        height += element.h + VSpace;
        //    }
        //    height -= VSpace;
        //    height += MarginTop + MarginBottom;
        //    this.h = Math.Max(this.h, height);
        //    this.TopAlign = MarginTop + Elements.First().TopAlign;
        //    this.MiddleAlign = MarginTop + (h - MarginTop - MarginBottom) / 2;
        //    this.BottomAlign = h - MarginBottom - Elements.Last().h + Elements.Last().BottomAlign;
        //}
        //else
        //{
        //    this.h += MarginTop + MarginBottom;
        //    this.TopAlign = MarginTop;
        //    this.MiddleAlign = MarginTop + (h - MarginTop - MarginBottom) / 2;
        //    this.BottomAlign = h - MarginBottom;
        //}
        //base.Calculate(gfx, x, y, w, h);

        internal override void Draw(XGraphics gfx)
        {
            if ((Name ?? "").Contains("test", StringComparison.OrdinalIgnoreCase))
            {
                // TEST : fill Image
                Helper.Fill(gfx, this);

                // TEST : draw text in the center of the image area
                //XFont font = new XFont("Arial", 12, XFontStyle.Regular);
                //XBrush textBrush = new XSolidBrush(Color);
                //string testString = $"{W.Millimeter:F2} X {H.Millimeter:F2}";

                //XSize textSize = gfx.MeasureString(testString, font);
                //double x = X + (W - textSize.Width) / 2;
                //double y = Y + (H - textSize.Height) / 2;
                //y = Y + textSize.Height;

                //gfx.DrawString(testString, font, textBrush, x, y);
            }

            //draw the elements
            foreach (BaseElement element in Elements)
            {
                try
                {
                    element.Draw(gfx);
                }
                catch
                {
                    continue;
                }
            }
        }
    }
}
