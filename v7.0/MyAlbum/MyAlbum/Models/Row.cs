using MyAlbum.Utilities;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace MyAlbum.Models
{
    internal class Row : BaseElement<RowStyle>
    {
        #region Fields
        protected Alignment? _align;
        protected VerticalAlignment? _vAlign;
        protected SpacingMode? _spacingMode;
        protected bool? _rotate;
        protected XUnit? _space;
        #endregion

        #region Style properties
        public Alignment Align 
        { 
            get => _align ?? Style.Align ?? Alignment.Center; 
            set => _align = value; 
        }
        public VerticalAlignment VAlign 
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

        #region Other properties
        public List<BaseElement> Elements { get; set; }
        #endregion

        #region Constructors
        public Row()
        {
            Style = Styles.Row.GetStyle("default") ?? new RowStyle();
            Elements = new List<BaseElement>();

            this.H = XUnit.FromMillimeter(20);
        }
        #endregion

        #region Override methods
        internal override void ParseXml(XElement xElem)
        {
            base.ParseXml(xElem);
            Style = Styles.Row.GetStyle(xElem.Attribute("style")?.Value);

            _align = XmlParser.ParseAlignment(xElem.Attribute("align")?.Value);
            _vAlign = XmlParser.ParseVerticalAlignment(xElem.Attribute("valign")?.Value);
            _spacingMode = XmlParser.ParseSpacingMode(xElem.Attribute("spacing-mode")?.Value);
            _space = XmlParser.ParseXUnit(xElem.Attribute("space")?.Value);
            _rotate = XmlParser.ParseBool(xElem.Attribute("rotate")?.Value);

            // Then parse page-specific elements
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

            if (Rotate)
            {
                X = parentCanvas.Y + MarginLeft;
                Y = parentCanvas.X + MarginTop;
                W = parentCanvas.H - (MarginLeft + MarginRight);
                this.Canvas = new Canvas
                {
                    X = X + PaddingLeft,
                    Y = Y + PaddingTop,
                    W = W - (PaddingLeft + PaddingRight),
                    H = H - (PaddingTop + PaddingBottom)
                };
            }

            foreach (var element in Elements)
            {
                element.Calculate(gfx, Canvas);
            }

            // At this point, we should have the calculated width and height of all elements in the row.
            // We can then adjust the row's height based on the tallest element and apply spacing if needed.

            // Calculate the maximum height of the elements in the row
            H = XUnit.Zero;
            foreach (var element in Elements)
            {     
                H = Math.Max(H, element.H);
            }

            // Calculate the position of the elements based on the row's alignment and spacing mode
            #region Vertical Alignment
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
            #endregion

            #region Horizontal Alignment
            if (Elements.Count == 1)
            {
                switch (Align)
                {
                    case Alignment.Left:
                        Elements[0].X = Canvas.X;
                        break;
                    case Alignment.Center:
                        Elements[0].X = Canvas.X + (Canvas.W - Elements[0].W) / 2;
                        break;
                    case Alignment.Right:
                        Elements[0].X = Canvas.X + Canvas.W - Elements[0].W;
                        break;
                    default:
                        break;
                }
            }

            //XUnit xPos;//, yPos;
            //XUnit contentWidth = XUnit.Zero;
            //int elementCount = 0;
            //switch (SpacingMode)
            //{
            //    case SpacingMode.FS:
            //        foreach (BaseElement element in Elements)
            //        {
            //            contentWidth += element.W;
            //            elementCount++;
            //        }

            //        contentWidth += (elementCount - 1) * Space;

            //        xPos = Canvas.X + (Canvas.W - contentWidth) / 2;

            //        foreach (BaseElement element in Elements)
            //        {
            //            element.X = xPos;
            //            xPos += element.W + Space;
            //        }
            //        break;
            //    case SpacingMode.ES:
            //        foreach (BaseElement element in Elements)
            //        {
            //            contentWidth += element.W;
            //            elementCount++;
            //        }

            //        Space = (Canvas.W - contentWidth) / (elementCount + 1);

            //        xPos = Canvas.X + Space;

            //        foreach (BaseElement element in Elements)
            //        {
            //            element.X = xPos;
            //            xPos += element.W + Space;
            //        }
            //        break;
            //    case SpacingMode.JS:
            //        if (Elements.Count == 1)
            //        {
            //            Elements[0].X = Canvas.X + (Canvas.W - Elements[0].W) / 2;
            //            break;
            //        }
            //        else
            //        {
            //            foreach (BaseElement element in Elements)
            //            {
            //                contentWidth += element.W;
            //                elementCount++;
            //            }

            //            Space = elementCount > 1 ? (Canvas.W - contentWidth) / (elementCount - 1) : 1;

            //            xPos = Canvas.X;

            //            foreach (BaseElement element in Elements)
            //            {
            //                element.X = xPos;
            //                xPos += element.W + Space;
            //            }
            //        }
            //        break;
            //}
            #endregion


        }
        internal override void Draw(XGraphics gfx)
        {
            base.Draw(gfx);

            // Draw the row's elements.
            foreach (BaseElement element in Elements)
            {
                Console.Write("  ");
                element.Draw(gfx);
            }
        }
        #endregion
    }
}
