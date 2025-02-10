using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Xml.Linq;
using PdfSharp.Drawing;

namespace MyAlbum
{
    class Column: Container
    {
        #region Properties
        public Styles.SpacingModes Spacing { get; set; }
        public XUnit MinWidth { get; set; }
        #endregion
 
        #region Constructors
        public Column()
        {
            BoxColor = XColors.Red;
        }
        public Column(XElement xml) : this()
        {
            Xml = xml;
        }
        #endregion

        #region Public Methods
        public override void Parse()
        {
            if (Parent != null) { Inherit(); }
            ApplyStyle();
            ParseAttributes();
            ParseComponents();
        }
        public override void Calculate()
        {
            XUnit height = XUnit.Zero;
            //foreach (BaseElement element in Elements.Where(b => typeof(Container).IsAssignableFrom(b.GetType())))// b.Elements != null))
            if (Elements.Count() > 0)
            {
                foreach (BaseElement element in Elements)
                {
                    element.gfx = gfx;
                    element.Calculate();
                    this.w = Math.Max(this.w, element.w);
                }
                if (Parent.w != XUnit.Zero)
                {
                    this.w = Math.Min(this.w, Parent.w);
                }

                foreach (BaseElement element in Elements)
                {
                    height += element.h + VSpace;
                }
                height -= VSpace;
                height += MarginTop + MarginBottom;
                this.h = Math.Max(this.h, height);
                this.TopAlign = MarginTop + Elements.First().TopAlign;
                this.MiddleAlign = MarginTop + (h - MarginTop - MarginBottom) / 2;
                this.BottomAlign = h - MarginBottom - Elements.Last().h + Elements.Last().BottomAlign;
            }
            else
            {
                this.h += MarginTop + MarginBottom;
                this.TopAlign = MarginTop;
                this.MiddleAlign = MarginTop + (h - MarginTop - MarginBottom) / 2;
                this.BottomAlign = h - MarginBottom;
            }
        }

        public override void Draw()
        {
            XUnit yPos = this.y + MarginTop;

            DrawBackground();
            //DrawBox();

            
            // set y for each element
            foreach (BaseElement element in Elements)
            {
                element.y = yPos;
                switch (this.Align)
                {
                    case Styles.Alignments.Left:
                        element.x = this.x;
                        break;
                    case Styles.Alignments.Center:
                        element.x = this.x + (this.w - element.w) / 2;
                        break;
                    case Styles.Alignments.Right:
                        element.x = this.x + (this.w - element.w);
                        break;
                    default:
                        break;
                }
                element.Draw();
                yPos += element.h + VSpace;

            }


            //DrawBox();

        }
        #endregion

        #region Private Methods
        private void Inherit()
        {
            if (this.Parent != null)
            {
                //VAlign = Parent.VAlign;
                this.Color = Parent.Color;
            }
        }
        private void ApplyStyle()
        {
            Column styleColumn = null;

            // use specific style
            if (Xml.Attribute("style") != null)
            {
                styleColumn = Styles.ColumnStyles.Where(t => t.Style == Xml.Attribute("style").Value).FirstOrDefault();
            }

            // use default
            if (styleColumn == null)
            {
                styleColumn = Styles.ColumnStyles.Where(t => t.IsDefault == true).FirstOrDefault();
            }

            // copy style properties
            if (styleColumn != null)
            {
                Style = styleColumn.Style;
                MarginLeft = styleColumn.MarginLeft;
                MarginRight = styleColumn.MarginRight;
                MarginTop = styleColumn.MarginTop;
                MarginBottom = styleColumn.MarginBottom;
                if (!styleColumn.Color.IsEmpty)
                {
                    Color = styleColumn.Color;
                }
                Align = styleColumn.Align;
                //if (styleColumn.VAlign != Styles.VerticalAlignments.NotSet)
                //{
                //    this.VAlign = styleColumn.VAlign;
                //}
 
            }

        }
        private void ParseAttributes()
        {
            // base element attribute
            ParseBaseAttribute();

            // space attribute
            ParseSpaceAttribute();
        }
        private void ParseComponents()
        {
            //todo: Row.ParseComponents();
            IEnumerable<XElement> elements = Xml.Elements();
            foreach (XElement xElem in elements)
            {
               // BaseElement element = null;
                switch (xElem.Name.LocalName)
                {
                    case "row":
                        Row row = new Row(xElem);
                        row.Parent = this;
                        row.Parse();
                        this.Elements.Add(row);
                        break;
                    case "image":
                        Image img = new Image(xElem);
                        img.Parent = this;
                        img.Parse();
                        this.Elements.Add(img);
                        break;
                    case "stamp":
                        Stamp stamp = new Stamp(xElem);
                        this.Elements.Add(stamp);
                        stamp.Parent = this;
                        stamp.Parse();
                        break;
                    case "text":
                        Text text = new Text(xElem);
                        text.Parent = this;
                        text.Parse();
                        this.Elements.Add(text);
                        break;
                    default:
                        break;
                }
                //if (element != null)
                //{
                //    element.Parent = this;
                //    element.Parse();
                //    this.Elements.Add(element);
                //}
            }
        }
        private void ParseSpaceAttribute()
        {
            if (Xml.Attribute("space") != null)
            {
                try
                {
                    this.VSpace = XUnit.FromMillimeter(double.Parse(Xml.Attribute("space").Value));
                }
                catch (Exception)
                {
                    this.VSpace = Page.VSpace;
                }
            }
            else
            {
                this.VSpace = Page.VSpace;
            }
        }
        #endregion
    }
}
