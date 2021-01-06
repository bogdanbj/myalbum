using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Xml.Linq;
using PdfSharp.Drawing;

namespace MyAlbum3
{
    class Row : Container
    {
        #region Properties
        public Styles.SpacingModes Spacing { get; set; }
        public XUnit Space { get; set; }
        public string Rotate { get; set; }
        #endregion

        #region Constructors
        public Row()
        {
            BoxColor = XColors.Green;
        }
        public Row(XElement xml) : this()
        {
            Xml = xml;
        }
        #endregion

        #region Public Methods
        public override void Parse()
        {
            Parse(isStyle: false);
        }
        public void Parse(bool isStyle)
        {
            if (Parent != null) { Inherit(); }
            if (!isStyle)
                ApplyStyle();
            ParseAttributes();
            ParseComponents();
        }
        public override void Calculate()
        {
            if (w == XUnit.Zero)
            {
                if (Rotate.ToLower() == "true")
                {
                    w = GetHeight(this.Parent) - (MarginTop + MarginBottom);
                }
                else
                {
                    w = GetWidth(this.Parent) - (MarginLeft + MarginRight);
                }
            }
            
            if (Elements.Count==0)
            {
                this.TopAlign = XUnit.Zero;
                this.MiddleAlign = this.h / 2;
                this.BottomAlign = this.h;

            }
            foreach (BaseElement element in Elements)
            {
                element.gfx = gfx;
                
                element.Calculate();

                if (element.Absolute == false)
                {
                    this.TopAlign = Math.Max(this.TopAlign, element.TopAlign);
                    this.MiddleAlign = Math.Max(this.MiddleAlign, element.MiddleAlign);
                    this.BottomAlign = Math.Max(this.BottomAlign, element.BottomAlign);

                    switch (VAlign)
                    {
                        case Styles.VerticalAlignments.Top:
                            this.h = Math.Max(this.h, this.TopAlign + element.h - element.TopAlign);
                            break;
                        case Styles.VerticalAlignments.Center:
                            this.h = Math.Max(this.h, this.MiddleAlign + element.h - element.MiddleAlign);
                            break;
                        case Styles.VerticalAlignments.Bottom:
                            this.h = Math.Max(this.h, this.BottomAlign + element.h - element.BottomAlign);
                            break;
                        default:
                            break;
                    }
                }
            }

            this.h += this.MarginTop + this.MarginBottom;

        }
        public override void Draw()
        {
            //XSolidBrush brush;

            #region horizontal alignment
            if (Rotate.ToLower() == "true")
            {
                x = Parent.x + (Parent.w - Parent.h) / 2 + this.MarginLeft;
                w = Parent.h - (this.MarginLeft + this.MarginRight);
                
            }
            else
            {
                x = Parent.x + this.MarginLeft;
                w = Parent.w - (this.MarginLeft + this.MarginRight);
            }

            //brush = new XSolidBrush(XColors.MediumSlateBlue);
            //gfx.DrawRectangle(brush, x, y, w, h);

            XUnit xPos, yPos;
            XUnit width = XUnit.Zero;
            int elementCount = 0;
            switch (Spacing)
            {
                case Styles.SpacingModes.FS:
                    foreach (BaseElement element in Elements)
                    {
                        if (element.Absolute == false)
                        {
                            width += element.w;
                            elementCount++;
                        }
                    }

                    width += (elementCount - 1) * Space;

                    xPos = this.x + (w - width) / 2;

                    foreach (BaseElement element in Elements)
                    {
                        if (element.Absolute == false)
                        {
                            element.x = xPos;
                            xPos += element.w + Space;
                        }
                    }
                    break;
                case Styles.SpacingModes.ES:
                    foreach (BaseElement element in Elements)
                    {
                        if (element.Absolute == false)
                        {
                            width += element.w;
                            elementCount++;
                        }
                    }

                    Space = (w - width) / (elementCount + 1);

                    xPos = x + Space;

                    foreach (BaseElement element in Elements)
                    {
                        if (element.Absolute== false)
                        {
                            element.x = xPos;
                            xPos += element.w + Space;
                        }
                    }
                    break;
                case Styles.SpacingModes.JS:
                    if (Elements.Count == 1)
                    {
                        if (Elements[0].Absolute == false)
                        {
                            Elements[0].x = x + (w - Elements[0].w) / 2;
                        }
                    }
                    else
                    {
                        foreach (BaseElement element in Elements)
                        {
                            if (element.Absolute == false)
                            {
                                width += element.w;
                                elementCount++;
                            }
                        }

                        Space = (w - width) / (elementCount - 1);

                        xPos = x + Space;

                        foreach (BaseElement element in Elements)
                        {
                            if (element.Absolute == false)
                            {
                                element.x = xPos;
                                xPos += element.w + Space;
                            }
                        }
                    }
                    break;
                default:
                    break;
            }
            #endregion
            DrawBackground();
            //DrawBox();
            yPos = this.y + this.MarginTop;
            foreach (BaseElement element in Elements)
            {
                if (element.Absolute == false)
                {
                    switch (this.VAlign)
                    {
                        case Styles.VerticalAlignments.Top:
                            element.y = yPos + this.TopAlign - element.TopAlign;
                            break;
                        case Styles.VerticalAlignments.Center:
                            element.y = yPos + this.MiddleAlign - element.MiddleAlign;
                            break;
                        case Styles.VerticalAlignments.Bottom:
                            element.y = yPos + this.BottomAlign - element.BottomAlign;
                            break;
                    }
                }
                else
                {
                    if (this.Rotate.ToLower() == "true")
                    {
                        PdfSharp.Pdf.PdfPage page = element.GetPage();
                        element.x += (page.Width - page.Height) / 2;
                        element.y += (page.Height - page.Width) / 2;
                    }
                }
                element.Draw();
            }

        }
        #endregion

        #region Private Methods
        private void Inherit()
        {
            this.Color = Parent.Color;
            //this.VSpace = Parent.VSpace;
        }
        private void ApplyStyle()
        {
            Row styleRow = null;

            // style
            if (Xml.Attribute("style") != null)
            {
                styleRow = Styles.RowStyles.Where(x => x.Style == Xml.Attribute("style").Value).FirstOrDefault();
            }

            // use default
            if (styleRow == null)
            {
                styleRow = Styles.RowStyles.Where(x => x.IsDefault == true).FirstOrDefault();
            }

            // copy style properties
            if (styleRow != null)
            {
                Style = styleRow.Style;
                Align = styleRow.Align;
                VAlign = styleRow.VAlign;
                Spacing = styleRow.Spacing;
                Space = styleRow.Space;
                MarginLeft = styleRow.MarginLeft;
                MarginRight = styleRow.MarginRight;
                MarginTop = styleRow.MarginTop;
                MarginBottom = styleRow.MarginBottom;
                if (!styleRow.BackgroundColor.IsEmpty)    // because is inherited from parent
                {
                    BackgroundColor = styleRow.BackgroundColor;
                }
                if (!styleRow.Color.IsEmpty)    // because is inherited from parent
                {
                    Color = styleRow.Color;
                }
                h = styleRow.h;

                // copy components
                Elements.Clear();
                foreach (BaseElement element in styleRow.Elements)
                {
                    Elements.Add(element);
                }
            }
        }
        private void ParseAttributes()
        {
            // base element attribute
            ParseBaseAttribute();

            // spacing-mode attribute
            ParseSpacingModeAttribute();

            // space attribute
            ParseSpaceAttribute();

            // space attribute
            ParseRotateAttribute();
        }
        private void ParseSpacingModeAttribute()
        {
            if (Xml.Attribute("spacing-mode") != null)
            {
                switch (Xml.Attribute("spacing-mode").Value.ToLower())
                {
                    case "fs":
                        this.Spacing = Styles.SpacingModes.FS;
                        break;
                    case "es":
                        this.Spacing = Styles.SpacingModes.ES;
                        break;
                    case "js":
                        this.Spacing = Styles.SpacingModes.JS;
                        break;
                    default:
                        this.Spacing = Styles.SpacingModes.ES;
                        break;
                }
            }
        }
        private void ParseSpaceAttribute()
        {
            if (Xml.Attribute("space") != null)
            {
                try
                {
                    this.Space = XUnit.FromMillimeter(double.Parse(Xml.Attribute("space").Value));
                }
                catch (Exception)
                {
                    this.Space = XUnit.Zero;
                }
            }
        }
        private void ParseRotateAttribute()
        {
            if (Xml.Attribute("rotate") != null)
            {
                this.Rotate = Xml.Attribute("rotate").Value;
            }
            else
            {
                this.Rotate = "FALSE";
            }
        }
        private void ParseComponents()
        { 
            //todo: Row.ParseComponents();
            IEnumerable<XElement> xElements = Xml.Elements();
            foreach (XElement xElem in xElements)
            {
                BaseElement element = null; ;
                switch (xElem.Name.LocalName)
                {
                    case "column":
                        element = new Column(xElem);
                        break;
                    case "image":
                        element = new Image(xElem);
                        break;
                    case "stamp":
                        element = new Stamp(xElem);
                        break;
                    case "text":
                        element = new Text(xElem);
                        break;
                }
                if (element != null)
                {
                    element.Color = this.Color;
                    element.Parent = this;
                    element.Parse();
                    this.Elements.Add(element);
                }
            }
        }
        #endregion

    }
}
