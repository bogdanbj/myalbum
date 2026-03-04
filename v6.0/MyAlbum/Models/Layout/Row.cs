using MyAlbum.Models.Xml;
using MyAlbum.Models.Xml.Styles;
using MyAlbum.Services;
using MyAlbum.Utils;
using PdfSharpCore.Drawing;


namespace MyAlbum.Models.Layout
{
    internal class Row : BaseElement
    {
        public SpacingMode Spacing { get; set; }
        public XUnit Space { get; set; }
        public List<BaseElement> Elements { get; set; }

        public Row()
        {
            W = XUnit.Zero;
            Elements = new List<BaseElement>();
            //H = XUnit.FromMillimeter(20);
        }



        internal void FromXml(XmlRow xmlRow, AlbumStyles styles)
        {
            // Find the appropriate row style
            RowStyle? style = StyleFactory.FindStyle<RowStyle>(xmlRow.Style, styles.RowStyles);
            if (style == null)
            {
                throw new InvalidOperationException(
                    $"Row style '{xmlRow.Style ?? "(default)"}' not found. " +
                    $"Ensure a matching RowStyle exists in the album styles or that a default RowStyle is defined.");
            }
            base.FromXml(xmlRow, style);

            // Apply the style
            Color = ParseColor(xmlRow.Color ?? style.Color ?? $"{Color.R},{Color.G},{Color.B}");
            BgColor = ParseColor(xmlRow.BgColor ?? style.BgColor ?? $"{BgColor.R},{BgColor.G},{BgColor.B}");
            Align = ParseAlignment(xmlRow.Align ?? style.Align ?? "left");
            VAlign = ParseVerticalAlignment(xmlRow.VAlign ?? style.VAlign ?? "top");
            Space = ParseXUnit(xmlRow.Space ?? style.Space ?? $"{Space.Millimeter} mm");
            Rotate = xmlRow.Rotate || style.Rotate;

            H = XUnit.FromMillimeter(xmlRow.Height > 0 ? xmlRow.Height : style.Height > 0 ? style.Height : H.Millimeter);



            foreach (XmlElement xmlElement in style.Elements)
            {
                // Add Element to the layoutPage
                switch (xmlElement)
                {
                    //case XmlBorder xmlBorder:
                    //    Border border = new Border();
                    //    border.Inherit(this);
                    //    border.FromXml(xmlBorder, styles);
                    //    this.Elements.Add(border);
                    //    break;

                    //case XmlRow xmlRow:
                    //    Row row = new Row();
                    //    row.Inherit(this);
                    //    row.FromXml(xmlRow, styles);
                    //    this.Elements.Add(row);
                    //    break;

                    case XmlColumn xmlColumn:
                        Column column = new Column();
                        column.Inherit(this);
                        column.FromXml(xmlColumn, styles);
                        this.Elements.Add(column);
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
            foreach (XmlElement xmlElement in xmlRow.Elements)
            {
                // Add Element to the layoutPage
                switch (xmlElement)
                {
                    //case XmlBorder xmlBorder:
                    //    Border border = new Border();
                    //    border.Inherit(this);
                    //    border.FromXml(xmlBorder, styles);
                    //    this.Elements.Add(border);
                    //    break;

                    //case XmlRow xmlRow:
                    //    Row row = new Row();
                    //    row.Inherit(this);
                    //    row.FromXml(xmlRow, styles);
                    //    this.Elements.Add(row);
                    //    break;

                    case XmlColumn xmlColumn:
                        Column column = new Column();
                        column.Inherit(this);
                        column.FromXml(xmlColumn, styles);
                        this.Elements.Add(column);
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

            // Width of the canvas 
            if (Rotate)
            {
                W = h - (MarginLeft + MarginRight);
            }
            else
            {
                W = w - (MarginLeft + MarginRight);
            }

            // Calculate nested elements dimensions
            foreach (BaseElement element in Elements)
            {
                element.CalculateSize(gfx, W, H);
            }

            #region Height and Vertical Alignment
            // Height of the canvas
            // TopAlign, MiddleAlign and BottomAlign are the horizontal lines on which the TopAlign, MiddleAlign or BottomAlign lines of the children will align too
            TopAlign = XUnit.Zero;
            MiddleAlign = H / 2;    // in case H was provided
            BottomAlign = H;

            // Calculate row's height
            foreach (BaseElement element in Elements)
            {
                // Update the row's TopAlign, MiddleAlign and BottomAlign based on the childrens' TopAlign, MiddleAlign and BottomAlign
                TopAlign = Math.Max(TopAlign, element.TopAlign);
                MiddleAlign = Math.Max(MiddleAlign, element.MiddleAlign);
                BottomAlign = Math.Max(BottomAlign, element.BottomAlign);

                switch (VAlign)
                {
                    case VerticalAlignment.Top:
                        H = Math.Max(H, TopAlign + element.H - element.TopAlign);
                        break;
                    case VerticalAlignment.Center:
                        H = Math.Max(H, MiddleAlign + element.H - element.MiddleAlign);
                        break;
                    case VerticalAlignment.Bottom:
                        H = Math.Max(H, BottomAlign + element.H - element.BottomAlign);
                        break;
                    default:
                        break;
                }
            }

            // Add margins to the row height
            H += MarginTop + MarginBottom;
            TopAlign += MarginTop;
            MiddleAlign += MarginTop;
            BottomAlign += MarginTop;
            #endregion
        }
        internal override void CalculateInnerPositions()
        {
            X += MarginLeft;

            // Align the elements vertically
            foreach (BaseElement element in Elements)
            {
                switch (VAlign)
                {
                    case VerticalAlignment.Top:
                        element.Y = Y + TopAlign - element.TopAlign;
                        break;
                    case VerticalAlignment.Center:
                        element.Y = Y + MiddleAlign - element.MiddleAlign;
                        break;
                    case VerticalAlignment.Bottom:
                        element.Y = Y + BottomAlign - element.BottomAlign;
                        break;
                    default:
                        break;
                }
            }

            #region Horizontal Alignment
            XUnit xPos;//, yPos;
            XUnit contentWidth = XUnit.Zero;
            int elementCount = 0;
            switch (Spacing)
            {
                case SpacingMode.FS:
                    foreach (BaseElement element in Elements)
                    {
                        contentWidth += element.W;
                        elementCount++;
                    }

                    contentWidth += (elementCount - 1) * Space;

                    xPos = X + (W - contentWidth) / 2;

                    foreach (BaseElement element in Elements)
                    {
                        element.X = xPos;
                        xPos += element.W + Space;
                    }
                    break;
                case SpacingMode.ES:
                    foreach (BaseElement element in Elements)
                    {
                        contentWidth += element.W;
                        elementCount++;
                    }

                    Space = (W - contentWidth) / (elementCount + 1);

                    xPos = X + Space;

                    foreach (BaseElement element in Elements)
                    {
                        element.X = xPos;
                        xPos += element.W + Space;
                    }
                    break;
                case SpacingMode.JS:
                    if (Elements.Count == 1)
                    {
                        Elements[0].X = X + (W - Elements[0].W) / 2;
                        break;
                    }
                    else
                    {
                        foreach (BaseElement element in Elements)
                        {
                            contentWidth += element.W;
                            elementCount++;
                        }

                        Space = elementCount > 1 ? (W - contentWidth) / (elementCount - 1) : 1;

                        xPos = X;

                        foreach (BaseElement element in Elements)
                        {
                            element.X = xPos;
                            xPos += element.W + Space;
                        }
                    }
                    break;
            }
            #endregion

            // Calculate inner positions for each child element
            foreach (BaseElement element in Elements)
            {
                element.CalculateInnerPositions();
            }
        }
        internal override void Draw(XGraphics gfx)
        {
            #region test/debug drawing
            if ((Name ?? "").Contains("test", StringComparison.OrdinalIgnoreCase))
            {
                if (Elements.Count == 0)
                {
                    // TEST : fill Image
                    Helper.Fill(gfx, this, XColors.MistyRose);
                    Helper.Write(gfx, this, Name ?? "EMPTY ROW");
                }
                else
                {
                    Helper.Fill(gfx, this);
                    Helper.WriteMySize(gfx, this.Canvas);
                }
                Helper.MarkCorners(gfx, this, true);
            }
            #endregion



            #region horizontal alignment
            //XUnit xPos, yPos;
            //XUnit width = XUnit.Zero;
            //int elementCount = 0;

            //switch (Spacing)
            //{
            //    case SpacingMode.FS:
            //        foreach (BaseElement element in Elements)
            //        {
            //            //if (element.Absolute == false)
            //            //{
            //            width += element.W;
            //            elementCount++;
            //            //}
            //        }

            //        width += (elementCount - 1) * Space;

            //        xPos = this.X + (W - width) / 2;

            //        foreach (BaseElement element in Elements)
            //        {
            //            //if (element.Absolute == false)
            //            //{
            //            element.X = xPos;
            //            xPos += element.W + Space;
            //            //}
            //        }
            //        break;
            //    case SpacingMode.ES:
            //        foreach (BaseElement element in Elements)
            //        {
            //            //if (element.Absolute == false)
            //            //{
            //            width += element.W;
            //            elementCount++;
            //            //}
            //        }

            //        Space = (W - width) / (elementCount + 1);

            //        xPos = X + Space;

            //        foreach (BaseElement element in Elements)
            //        {
            //            //if (element.Absolute == false)
            //            //{
            //            element.X = xPos;
            //            xPos += element.W + Space;
            //            //}
            //        }
            //        break;
            //    case SpacingMode.JS:
            //        if (Elements.Count == 1)
            //        {
            //            //if (Elements[0].Absolute == false)
            //            //{
            //            Elements[0].X = X + (W - Elements[0].W) / 2;
            //            //}
            //        }
            //        else
            //        {
            //            foreach (BaseElement element in Elements)
            //            {
            //                //if (element.Absolute == false)
            //                //{
            //                width += element.W;
            //                elementCount++;
            //                //}
            //            }

            //            Space = (W - width) / (elementCount - 1);

            //            xPos = X + Space;

            //            foreach (BaseElement element in Elements)
            //            {
            //                //if (element.Absolute == false)
            //                //{
            //                element.X = xPos;
            //                xPos += element.W + Space;
            //                //}
            //            }
            //        }
            //        break;
            //    default:
            //        break;
            //}
            #endregion

            //yPos = this.Y + this.MarginTop;
            //foreach (BaseElement element in Elements)
            //{
            //    //if (element.Absolute == false)
            //    //{
            //    switch (this.VAlign)
            //    {
            //        case VerticalAlignment.Top:
            //            element.Y = yPos + this.TopAlign - element.TopAlign;
            //            break;
            //        case VerticalAlignment.Middle:
            //            element.Y = yPos + this.MiddleAlign - element.MiddleAlign;
            //            break;
            //        case VerticalAlignment.Bottom:
            //            element.Y = yPos + this.BottomAlign - element.BottomAlign;
            //            break;
            //    }
            //    //}
            //    //else
            //    //{
            //    if (Rotate)
            //    {
            //        //PdfSharp.Pdf.PdfPage page = element.GetPage();
            //        //Page page = element.GetPage();
            //        element.X += (W - H) / 2;
            //        element.Y += (H - W) / 2;
            //        //element.X += (page.Width - page.Height) / 2;
            //        //element.y += (page.Height - page.Width) / 2;
            //    }
            //    //}
            //    try
            //    {
            //        element.Draw(gfx);
            //    }
            //    catch { continue; }
            //}

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

            //if (Rotate)
            //{
            //    //row.Y = yPosRotate;
            //    //row.Draw();
            //    //yPosRotate += row.h + VSpace;
            //    //this.w = this.w - row.h;
            //    gfx.TranslateTransform(W / 2, H / 2);
            //    gfx.RotateTransform(-90);
            //    gfx.TranslateTransform(-H / 2, -W / 2);
            //}


        }
        //public override void Draw()
        //{
        //    //XSolidBrush brush;

        //    #region horizontal alignment
        //    //if (Rotate.ToLower() == "true")
        //    //{
        //    //    x = Parent.x + (Parent.w - Parent.h) / 2 + this.MarginLeft;
        //    //    w = Parent.h - (this.MarginLeft + this.MarginRight);

        //    //}
        //    //else
        //    //{
        //    //    x = Parent.x + this.MarginLeft;
        //    //    w = Parent.w - (this.MarginLeft + this.MarginRight);
        //    //}

        //    //brush = new XSolidBrush(XColors.MediumSlateBlue);
        //    //gfx.DrawRectangle(brush, x, y, w, h);

        //    //XUnitPt xPos, yPos;
        //    //XUnitPt width = XUnitPt.Zero;
        //    //int elementCount = 0;
        //    //switch (Spacing)
        //    //{
        //    //    case Styles.SpacingModes.FS:
        //    //        foreach (BaseElement element in Elements)
        //    //        {
        //    //            if (element.Absolute == false)
        //    //            {
        //    //                width += element.w;
        //    //                elementCount++;
        //    //            }
        //    //        }

        //    //        width += (elementCount - 1) * Space;

        //    //        xPos = this.x + (w - width) / 2;

        //    //        foreach (BaseElement element in Elements)
        //    //        {
        //    //            if (element.Absolute == false)
        //    //            {
        //    //                element.x = xPos;
        //    //                xPos += element.w + Space;
        //    //            }
        //    //        }
        //    //        break;
        //    //    case Styles.SpacingModes.ES:
        //    //        foreach (BaseElement element in Elements)
        //    //        {
        //    //            if (element.Absolute == false)
        //    //            {
        //    //                width += element.w;
        //    //                elementCount++;
        //    //            }
        //    //        }

        //    //        Space = (w - width) / (elementCount + 1);

        //    //        xPos = x + Space;

        //    //        foreach (BaseElement element in Elements)
        //    //        {
        //    //            if (element.Absolute == false)
        //    //            {
        //    //                element.x = xPos;
        //    //                xPos += element.w + Space;
        //    //            }
        //    //        }
        //    //        break;
        //    //    case Styles.SpacingModes.JS:
        //    //        if (Elements.Count == 1)
        //    //        {
        //    //            if (Elements[0].Absolute == false)
        //    //            {
        //    //                Elements[0].x = x + (w - Elements[0].w) / 2;
        //    //            }
        //    //        }
        //    //        else
        //    //        {
        //    //            foreach (BaseElement element in Elements)
        //    //            {
        //    //                if (element.Absolute == false)
        //    //                {
        //    //                    width += element.w;
        //    //                    elementCount++;
        //    //                }
        //    //            }

        //    //            Space = (w - width) / (elementCount - 1);

        //    //            xPos = x + Space;

        //    //            foreach (BaseElement element in Elements)
        //    //            {
        //    //                if (element.Absolute == false)
        //    //                {
        //    //                    element.x = xPos;
        //    //                    xPos += element.w + Space;
        //    //                }
        //    //            }
        //    //        }
        //    //        break;
        //    //    default:
        //    //        break;
        //    //}
        //    #endregion
        //    DrawBackground();
        //    //DrawBox();
        //    yPos = this.y + this.MarginTop;
        //    foreach (BaseElement element in Elements)
        //    {
        //        if (element.Absolute == false)
        //        {
        //            switch (this.VAlign)
        //            {
        //                case Styles.VerticalAlignments.Top:
        //                    element.y = yPos + this.TopAlign - element.TopAlign;
        //                    break;
        //                case Styles.VerticalAlignments.Center:
        //                    element.y = yPos + this.MiddleAlign - element.MiddleAlign;
        //                    break;
        //                case Styles.VerticalAlignments.Bottom:
        //                    element.y = yPos + this.BottomAlign - element.BottomAlign;
        //                    break;
        //            }
        //        }
        //        else
        //        {
        //            if (this.Rotate.ToLower() == "true")
        //            {
        //                //PdfSharp.Pdf.PdfPage page = element.GetPage();
        //                Page page = element.GetPage();
        //                element.x += (page.Width - page.Height) / 2;
        //                element.y += (page.Height - page.Width) / 2;
        //            }
        //        }
        //        element.Draw();
        //    }

        //}

    }
}
