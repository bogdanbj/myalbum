using MyAlbum.Utilities;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
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
        #endregion

        #region Other properties
        public List<BaseElement> Elements { get; set; }
        #endregion

        #region Constructors
        public Row()
        {
            Style = Styles.Row.GetStyle("default") ?? new RowStyle();
            Elements = new List<BaseElement>();

            //this.H = XUnit.FromMillimeter(20);
            this.H = XUnit.Zero;
        }
        #endregion

        #region Override methods
        internal override void ParseXml(XElement xElem)
        {
            base.ParseXml(xElem);
            Width = "100%";
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
                W = parentCanvas.H - (MarginLeft + MarginRight);
                this.Canvas = new Canvas
                {
                    X = MarginLeft,
                    Y = MarginTop,
                    W = W - (MarginLeft + MarginRight),
                    H = H - (MarginTop + MarginBottom)
                };
            }

            // Calculate the top and bottom height and get the max.
            //H = XUnit.Zero;
            XUnit 
                topHeight = XUnit.Zero, 
                bottomHeight = XUnit.Zero;
            foreach (var element in Elements)
            {
                element.Calculate(gfx, Canvas);

                switch (VAlign)
                {
                    case VerticalAlignment.Top:
                        TopAlign = XUnit.FromPoint(Math.Max(TopAlign.Point, element.TopAlign.Point));
                        topHeight = XUnit.FromPoint(Math.Max(topHeight.Point, (element.TopAlign).Point));
                        bottomHeight = XUnit.FromPoint(Math.Max(bottomHeight.Point, (element.H - element.TopAlign).Point));
                        break;
                    case VerticalAlignment.Center:
                        MiddleAlign = XUnit.FromPoint(Math.Max(MiddleAlign.Point, element.MiddleAlign.Point));
                        topHeight = XUnit.FromPoint(Math.Max(topHeight.Point, (element.MiddleAlign).Point));
                        bottomHeight = XUnit.FromPoint(Math.Max(bottomHeight.Point, (element.H - element.MiddleAlign).Point));
                        break;
                    case VerticalAlignment.Bottom:
                        BottomAlign = XUnit.FromPoint(Math.Max(BottomAlign.Point, element.BottomAlign.Point));
                        topHeight = XUnit.FromPoint(Math.Max(topHeight.Point, (element.BottomAlign).Point));
                        bottomHeight = XUnit.FromPoint(Math.Max(bottomHeight.Point, (element.H - element.BottomAlign).Point));
                        break;
                    default:
                        break;
                }
            }
            this.H = topHeight + bottomHeight;

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
            XUnit 
                contentWidth = XUnit.Zero,
                leadWidth = XUnit.Zero;
            foreach (BaseElement element in Elements)
            {
                contentWidth += element.W;
            }
            switch (SpacingMode)
            {   
                case SpacingMode.FS:
                    contentWidth += XUnit.FromPoint((Elements.Count - 1) * Space.Point);
                    leadWidth = (Canvas.W - contentWidth) / 2;
                    break;
                case SpacingMode.ES:
                    leadWidth = Space = XUnit.FromPoint( (Canvas.W - contentWidth).Point / (Elements.Count + 1) );
                    break;
                case SpacingMode.JS:
                    leadWidth = Elements.Count > 1 ? XUnit.Zero : (Canvas.W - contentWidth) / 2;
                    Space = Elements.Count > 1 ? XUnit.FromPoint((Canvas.W - contentWidth).Point / (Elements.Count - 1)) : XUnit.Zero;
                    break;
                default:
                    break;
            }
            foreach (BaseElement element in Elements)
            {
                element.X = leadWidth;
                leadWidth += element.W + Space;
            }

            if (Elements.Count == 1)
            {
                switch (Align)
                {
                    case Alignment.Left:
                        //Elements[0].X = Canvas.X;
                        Elements[0].X = XUnit.Zero;
                        break;
                    case Alignment.Center:
                        //Elements[0].X = Canvas.X + (Canvas.W - Elements[0].W) / 2;
                        Elements[0].X = (Canvas.W - Elements[0].W) / 2;
                        break;
                    case Alignment.Right:
                        //Elements[0].X = Canvas.X + Canvas.W - Elements[0].W;
                        Elements[0].X = Canvas.W - Elements[0].W;
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
                DrawElement(gfx, element);
            }
        }
        #endregion
    }
}
