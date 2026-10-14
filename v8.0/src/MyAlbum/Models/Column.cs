using MyAlbum.Utilities;
using PdfSharp.Drawing;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace MyAlbum.Models
{
    internal class Column : BaseElement<ColumnStyle>
    {
        #region Fields
        protected Alignment? _align;
        protected XUnit? _vSpace;
        #endregion

        #region Style properties
        public Alignment Align
        {
            get => _align ?? Style.Align ?? Alignment.Center;
            set => _align = value;
        }
        public XUnit VSpace
        {
            get => _vSpace ?? Style.VSpace ?? XUnit.Zero;
            set => _vSpace = value;
        }
        #endregion

        #region Other properties
        public List<BaseElement> Elements { get; set; }
        #endregion

        #region Constructors
        public Column()
        {
            Style = Styles.Column.GetStyle("default") ?? new ColumnStyle();
            Elements = new List<BaseElement>();
        }
        #endregion

        #region Override methods
        internal override void ParseXml(XElement xElem)
        {
            base.ParseXml(xElem);
            Style = Styles.Column.GetStyle(xElem.Attribute("style")?.Value);

            _align = XmlParser.ParseAlignment(xElem.Attribute("align")?.Value);
            _vSpace = XmlParser.ParseXUnit(xElem.Attribute("vspace")?.Value);

            // Parse child elements
            foreach (XElement xInnerElem in xElem.Elements())
            {
                var inner = CreateElement(xInnerElem.Name.LocalName);
                if (inner != null)
                {
                    inner.Inherit(this);
                    inner.ParseXml(xInnerElem);
                    Elements.Add(inner);
                }
            }
        }

        internal override void Calculate(XGraphics gfx, Canvas parentCanvas)
        {
            base.Calculate(gfx, parentCanvas);

            // Measure children and derive column size (main axis = vertical).
            foreach (BaseElement element in Elements)
            {
                element.Calculate(gfx, Canvas);
                this.Canvas.H += element.H;
                this.Canvas.W = XUnit.FromPoint(Math.Max(this.Canvas.W.Point, element.W.Point));
            }

            if (Elements.Count > 1)
                this.Canvas.H += XUnit.FromPoint((Elements.Count - 1) * VSpace.Point);

            // Column occupies the full canvas width; height is driven by its children.
            H = Canvas.H + MarginTop + MarginBottom;
            W = Canvas.W + MarginLeft + MarginRight;

            // Stack children top-to-bottom (main axis = Y).
            XUnit y = MarginTop;
            foreach (BaseElement element in Elements)
            {
                element.Y = y;
                y += XUnit.FromPoint(element.H.Point + VSpace.Point);

                // Cross-axis (horizontal) placement by Align.
                switch (Align)
                {
                    case Alignment.Left:
                        element.X = XUnit.Zero;
                        break;
                    case Alignment.Center:
                        element.X = XUnit.FromPoint((Canvas.W.Point - element.W.Point) / 2);
                        break;
                    case Alignment.Right:
                        element.X = XUnit.FromPoint(Canvas.W.Point - element.W.Point);
                        break;
                    default:
                        break;
                }
            }
        }

        internal override void Draw(XGraphics gfx)
        {
            base.Draw(gfx);

            // Draw the column's elements.
            foreach (BaseElement element in Elements)
            {
                DrawElement(gfx, element);
            }
        }
        #endregion
    }
}