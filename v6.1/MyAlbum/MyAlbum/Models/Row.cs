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
        protected bool? _rotate;
        #region Properties accepting Styles 
        public new RowStyle Style
        {
            get => (RowStyle)base.Style;
            set => base.Style = value;
        }
        public Alignment Align { get; set; }
        public VerticalAlignment VAlignment { get; set; }
        public SpacingMode SpacingMode { get; set; }
        public XUnit Space { get; set; }
        public bool Rotate
        {
            get => _rotate ?? Style.Rotate ?? false;
            set => _rotate = value;
        }
        #endregion

        public Row()
        {
            this.H = XUnit.FromMillimeter(20);
        }
        internal new void ParseXml(XElement xRow)
        {
            base.ParseXml(xRow);
            Style = Styles.Row.GetStyle(xRow.Attribute("style")?.Value);

            Align = XmlParser.ParseAlignment(xRow.Attribute("align")?.Value ?? Style?.Align ?? "center");
            VAlignment = XmlParser.ParseVerticalAlignment(xRow.Attribute("valign")?.Value ?? Style?.VAlign ?? "top");
            SpacingMode = XmlParser.ParseSpacingMode(xRow.Attribute("spacing-mode")?.Value ?? Style?.SpacingMode ?? "FS");
            Space = XmlParser.ParseXUnit(xRow.Attribute("space")?.Value ?? Style?.Space ?? "0");
            _rotate = XmlParser.ParseBool(xRow.Attribute("rotate")?.Value);
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
            //else
            //{
            //    W = parentCanvas.W - (MarginLeft + MarginRight);
            //}

            // Reset the height. It will be calculated based on the content of the row.
            H = XUnit.Zero;

            H = XUnit.FromMillimeter(30);

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
