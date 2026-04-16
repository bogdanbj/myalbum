using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using PdfSharpCore.Drawing;
using MyAlbum.Utilities;

namespace MyAlbum.Models
{
    internal class Row : BaseElement
    {
        protected Alignment? _align;
        protected VerticalAlignment? _vAlign;
        protected SpacingMode? _spacingMode;
        protected bool? _rotate;
        protected XUnit? _space;

        #region Properties accepting Styles 
        public new RowStyle Style
        {
            get => (RowStyle)base.Style;
            set => base.Style = value;
        }
        public Alignment Align 
        { 
            get => _align ?? Style.Align ?? Alignment.Center; 
            set => _align = value; 
        }
        public VerticalAlignment VAlignment 
        { 
            get => _vAlign ?? Style.VAlign ?? VerticalAlignment.Top; 
            set => _vAlign = value; 
        }
        public SpacingMode SpacingMode 
        { 
            get => _spacingMode ?? Style.SpacingMode ?? SpacingMode.FS; 
            set => _spacingMode = value; 
        }
        public XUnit Space 
        { 
            get => _space ?? Style.Space ?? XUnit.Zero;
            set => _space = value;
        }
        public bool Rotate
        {
            get => _rotate ?? Style.Rotate ?? false;
            set => _rotate = value;
        }
        #endregion

        #region other properties
        public List<BaseElement> Elements { get; set; }
        #endregion

        public Row()
        {
            this.H = XUnit.FromMillimeter(20);
        }
        internal new void ParseXml(XElement xRow)
        {
            base.ParseXml(xRow);
            Style = Styles.Row.GetStyle(xRow.Attribute("style")?.Value);

            _align = XmlParser.ParseAlignment(xRow.Attribute("align")?.Value);
            _vAlign = XmlParser.ParseVerticalAlignment(xRow.Attribute("valign")?.Value);
            _spacingMode = XmlParser.ParseSpacingMode(xRow.Attribute("spacing-mode")?.Value);
            _space = XmlParser.ParseXUnit(xRow.Attribute("space")?.Value);
            _rotate = XmlParser.ParseBool(xRow.Attribute("rotate")?.Value);

            // Then parse page-specific elements
            foreach (XElement xElement in xRow.Elements())
            {
                var elem = CreateElement(xElement.Name.LocalName);
                if (elem != null)
                {
                    elem.Inherit(this);
                    elem.ParseXml(xElement);
                    Elements.Add(elem);
                }
            }
        }
        internal override void Calculate(XGraphics gfx, Canvas parentCanvas)
        {
            base.Calculate(gfx, parentCanvas);

            //X = parentCanvas.X + MarginLeft;
            //Y = parentCanvas.Y + MarginTop;
            //W = parentCanvas.W - (MarginLeft + MarginRight);
            if (Rotate)
            {
                X = parentCanvas.Y + MarginLeft;
                Y = parentCanvas.X + MarginTop;
                W = parentCanvas.H - (MarginLeft + MarginRight);
            }

            foreach (var element in Elements)
            {
                element.Calculate(gfx, Canvas);
            }
            //else
            //{
            //    W = parentCanvas.W - (MarginLeft + MarginRight);
            //}

            // Reset the height. It will be calculated based on the content of the row.
            //H = XUnit.Zero;


            //if (element is Row row)
            //{
            //    row.CalculateSize(gfx, Canvas.W, Canvas.H);

            //    // If a row is added to the page, re-adjust the remaining canvas area
            //    if (row.Rotate)
            //    {
            //        row.X = Canvas.Y;
            //        row.Y = Canvas.X;

            //        Canvas.X = Canvas.X;           // no change here
            //        Canvas.W -= row.H + VSpace;    // shrink the width
            //    }
            //    else
            //    {
            //        row.X = Canvas.X;
            //        row.Y = Canvas.Y;

            //        Canvas.Y += row.H + VSpace;
            //        Canvas.H -= row.H + VSpace;
            //    }
            //    row.CalculateInnerPositions();
            //}
        }
    }
}
