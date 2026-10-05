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
        #region Fields
        protected bool? _rotate;
        #endregion

        #region Style properties
        public bool Rotate
        {
            get => _rotate ?? Style?.Rotate ?? false;
            set => _rotate = value;
        }
        #endregion

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
                this.Canvas = new Canvas
                {
                    X = X + PaddingLeft,
                    Y = Y + PaddingTop,
                    W = W - (PaddingLeft + PaddingRight),
                    H = H - (PaddingTop + PaddingBottom)
                };
            }
        }
        internal override void Draw(XGraphics gfx)
        {
            LogDraw();
            gfx.DrawRectangle(new XSolidBrush(BgColor), X, Y, W, H);

            //base.Draw(gfx);
        }
        #endregion
    }
}
