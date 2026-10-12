using MyAlbum.Utilities;
using PdfSharp.Drawing;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace MyAlbum.Models
{
    internal class Space : BaseElement<SpaceStyle>
    {
        #region Constructors
        public Space()
        {
            Style = Styles.Space.GetStyle("default") ?? new SpaceStyle();
        }
        #endregion

        #region Override methods
        internal override void ParseXml(XElement xElem)
        {
            Style = Styles.Space.GetStyle(xElem.Attribute("style")?.Value) ?? new SpaceStyle();
            base.ParseXml(xElem);

            (_marginTop, _marginRight, _marginBottom, _marginLeft) = XmlParser.ParseMargin("0");
            _rotate = XmlParser.ParseBool(xElem.Attribute("rotate")?.Value);
        }
        internal override void Calculate(XGraphics gfx, Canvas parentCanvas)
        {
            base.Calculate(gfx, parentCanvas);

            if (Rotate)
            {
                X = parentCanvas.Y + MarginLeft;
                Y = parentCanvas.X + MarginTop;
                W = parentCanvas.H - (MarginLeft + MarginRight);
                //this.Canvas = new Canvas
                //{
                //    X = PaddingLeft,
                //    Y = PaddingTop,
                //    W = W - (PaddingLeft + PaddingRight),
                //    H = H - (PaddingTop + PaddingBottom)
                //};
            }
        }
        internal override void Draw(XGraphics gfx)
        {
            base.Draw(gfx);
            //gfx.DrawRectangle(new XSolidBrush(BgColor), 0, 0, W.Point, H.Point);

            //base.Draw(gfx);
        }
        #endregion
    }
}
