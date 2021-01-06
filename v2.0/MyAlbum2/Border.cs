using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PdfSharp.Drawing;

namespace MyAlbum2
{
    abstract class BorderBase : BasicType
    {

        #region Properties
        #endregion

        #region Constructors
        protected BorderBase(XGraphics gfx) : base(gfx)
        { }
        protected BorderBase(XGraphics gfx, double x, double y, double w, double h)
            : base(gfx, x, y, w, h)
        { }
        #endregion

        #region Methods
        #endregion


    }

    class BorderPlain : BorderBase
    {
        #region Properties
        public XUnit Width1 { get; set; }	// width of the exterior line
        public XUnit Width2 { get; set; }	// width of the interior line
        public XUnit Space { get; set; }	// the space between the two lines
        public XColor Color1 { get; set; }	// color of the exterior line
        public XColor Color2 { get; set; }	// color of the exterior line
        #endregion

        #region Constructors
        public BorderPlain(XGraphics gfx)
            : base(gfx)
        { }
//        public BorderPlain(XGraphics gfx, double x, double y, double w, double h)
//            : base(gfx, x, y, w, h)
//        {
//        }
        public BorderPlain(XGraphics gfx, double x, double y, double w, double h,
                                     double width1, double width2, double space,
                                     XColor color1, XColor color2)
            : base(gfx, x, y, w, h)
        {
            Width1 = XUnit.FromMillimeter(width1).Point;
            Width2 = XUnit.FromMillimeter(width2).Point;
            Color1 = color1;
            Color2 = color2;
            Space = XUnit.FromMillimeter(space);
        }
        #endregion

        #region Methods
        public override void Draw()
        {
            XPen pen;
            double offset;

            if (Width1 != 0)
            {
                pen = new XPen(Color1, Width1);
                gfx.DrawRectangle(pen, X, Y, W, H);

                if (Width2 != 0)
                {
                    offset = Width1 + Space;
                    pen = new XPen(Color2, Width2);
                    gfx.DrawRectangle(pen, X + offset, Y + offset, W - 2 * offset, H - 2 * offset);
                }
            }
        }
        #endregion

    }

    class BorderGizmo1 : BorderPlain
    {
        #region Constructors
        public BorderGizmo1(XGraphics gfx)
            : base(gfx)
        { }

        public BorderGizmo1(XGraphics gfx, double x, double y, double w, double h,
                                double width1, double width2, double space,
                                XColor color1, XColor color2)
            : base(gfx, x, y, w, h, width1, width2, space, color1, color2)
        { }
        #endregion

        #region Methods
        public override void Draw()
        {
            //XPen pen;
            double x, y, w, h;

            base.Draw();
            //draw horizontal gizmos
            #region draw horizontal gizmos
            if (W > H)
            {
                // first gizmo on the top border
                x = X + (W / 4) - Space;
                y = Y - Width1 / 2;
                w = 2 * (Width1 + Space + Width2);
                h = Width1 + Space + Width2;
                DrawGizmo(x, y, w, h);

                // second gizmo on the top border
                x = X + 3 * (W / 4) - Space;
                y = Y - Width1 / 2;
                w = 2 * (Width1 + Space + Width2);
                h = Width1 + Space + Width2;
                DrawGizmo(x, y, w, h);

                // first gizmo on the bottom border
                x = X + (W / 4) - Space;
                y = Y + H - (Width1 + Space) - Width1 / 2;
                w = 2 * (Width1 + Space + Width2);
                h = Width1 + Space + Width2;
                DrawGizmo(x, y, w, h);

                // second gizmo on the bottom border
                x = X + 3 * (W / 4) - Space;
                y = Y + H - (Width1 + Space) - Width1 / 2;
                w = 2 * (Width1 + Space + Width2);
                h = Width1 + Space + Width2;
                DrawGizmo(x, y, w, h);
            }
            else
            {
                // one gizmo on the top border
                x = X + (W / 2) - Space;
                y = Y - Width1 / 2;
                w = 2 * (Width1 + Space + Width2);
                h = Width1 + Space + Width2;
                DrawGizmo(x, y, w, h);

                // one gizmo on the bottom border
                x = X + (W / 2) - Space;
                y = Y + H - (Width1 + Space) - Width1 / 2;
                w = 2 * (Width1 + Space + Width2);
                h = Width1 + Space + Width2;
                DrawGizmo(x, y, w, h);
            }
            #endregion
            // draw vertical gizmos
            #region draw vertical gizmos
            if (H > W)
            {
                // first gizmo on the left border
                x = X - Width1 / 2;
                y = Y + (H / 4) - Space;
                w = Width1 + Space + Width2;
                h = 2 * (Width1 + Space + Width2);
                DrawGizmo(x, y, w, h);

                // second gizmo on the left border
                x = X - Width1 / 2;
                y = Y + 3 * (H / 4) - Space;
                w = Width1 + Space + Width2;
                h = 2 * (Width1 + Space + Width2);
                DrawGizmo(x, y, w, h);

                // first gizmo on the right border
                x = X + W - (Width1 + Space) - Width1 / 2;
                y = Y + (H / 4) - Space;
                //y = Y + H - (Width1 + Space) - Width1 / 2;
                w = Width1 + Space + Width2;
                h = 2 * (Width1 + Space + Width2);
                DrawGizmo(x, y, w, h);

                // second gizmo on the right border
                x = X + W - (Width1 + Space) - Width1 / 2;
                y = Y + 3 * (H / 4) - Space;
                //y = Y + H - (Width1 + Space) - Width1 / 2;
                w = Width1 + Space + Width2;
                h = 2 * (Width1 + Space + Width2);
                DrawGizmo(x, y, w, h);
            }
            else
            {
                // one gizmo on the left border
                x = X - Width1 / 2;
                y = Y + (H / 2) - Space;
                w = Width1 + Space + Width2;
                h = 2 * (Width1 + Space + Width2);
                DrawGizmo(x, y, w, h);

                // one gizmo on the right border
                x = X + W - (Width1 + Space) - Width1 / 2;
                y = Y + (H / 2) - Space;
                w = Width1 + Space + Width2;
                h = 2 * (Width1 + Space + Width2);
                DrawGizmo(x, y, w, h);
            }
            #endregion
        }
        private void DrawGizmo(double x, double y, double w, double h)
        {
            XPen pen;
            pen = new XPen(XColors.Black, Width1 * 1.1);

            double adj;
            adj = Width1 / 4;

            //gfx.DrawRectangle(XBrushes.DarkOrange, x, y, w, h);

            if (w > h)
            {
                gfx.DrawRectangle(XBrushes.White, x, y - Width1, w, h + 2 * Width1);
                gfx.DrawLine(pen, x - adj, y + adj, x + w / 2 + adj, y + h - adj);
                gfx.DrawLine(pen, x - adj, y + h - adj, x + w / 2 + adj, y + adj);
                gfx.DrawLine(pen, x + w / 2 - adj, y + adj, x + w + adj, y + h - adj);
                gfx.DrawLine(pen, x + w / 2 - adj, y + h - adj, x + w + adj, y + adj);
            }
            else
            {
                gfx.DrawRectangle(XBrushes.White, x - Width1, y, w + 2 * Width1, h);
                gfx.DrawLine(pen, x + adj, y - adj, x + w - adj, y + h / 2 + adj);
                gfx.DrawLine(pen, x + adj, y + h / 2 + adj, x + w - adj, y - adj);
                gfx.DrawLine(pen, x + adj, y + h / 2 - adj, x + w - adj, y + h + adj);
                gfx.DrawLine(pen, x + adj, y + h + adj, x + w - adj, y + h / 2 - adj);
            }

        }
        #endregion
    }

    /*
    class BorderGraphic : BorderBase
    {
        #region Properties
        public Image topLeft { get; set; }	// top-left corner
        public Image topRight { get; set; }	// top-right corner
        public Image bottomLeft { get; set; }	// bottom-left corner
        public Image bottomRight { get; set; }	// bottom-right corner
        public Image vElem { get; set; }	// vertical repeating element
        public Image hElem { get; set; }	// horizontal repeating element
        #endregion

        #region Constructors
        public BorderGraphic(XGraphics gfx)
            : base(gfx)
        { }

        public BorderGraphic(XGraphics gfx, double x, double y, double w, double h,
                                string tlFile = null, string trFile = null, string blFile = null, string brFile = null,
                                string vFile = null, string hFile = null)
            : base(gfx, x, y, w, h)
        {
            if (true)
            {
                if (tlFile != null)
                {
                    topLeft = new Image(gfx, tlFile, new XPoint(X, Y), Alignments.TopLeft);
                }
                if (trFile != null)
                {
                    topRight = new Image(gfx, trFile, new XPoint(X + W, Y), Alignments.TopRight);
                }
                if (blFile != null)
                {
                    bottomLeft = new Image(gfx, blFile, new XPoint(X, Y + H), Alignments.BottomLeft);
                }
                if (brFile != null)
                {
                    bottomRight = new Image(gfx, brFile, new XPoint(X + W, Y + H), Alignments.BottomRight);
                }
                if (vFile != null)
                {
                    vElem = new Image(gfx, vFile);
                }
                if (hFile != null)
                {
                    hElem = new Image(gfx, hFile);
                }
            }
        }
        #endregion

        #region Methods
        public override void Draw()
        {
            // corners
            if (topLeft != null)
            {
                topLeft.Draw();
            }
            if (topRight != null)
            {
                topRight.Draw();
            }
            if (bottomLeft != null)
            {
                bottomLeft.Draw();
            }
            if (bottomRight != null)
            {
                bottomRight.Draw();
            }

            // lines
            Image img;
            if (hElem != null)
            {
                // top line
                img = hElem;
                img.Point = new XPoint(X + topLeft.Size.Width, Y);
                img.Size = new XSize(W - topLeft.Size.Width - topRight.Size.Width, img.Size.Height);
                img.Draw();
                // bottom line
                img = hElem;
                img.Point = new XPoint(X + bottomLeft.Size.Width, Y + H);
                img.Size = new XSize(W - bottomLeft.Size.Width - bottomRight.Size.Width, img.Size.Height);
                img.Alignment = Alignments.BottomLeft;
                img.Draw();
            }
            if (vElem != null)
            {
                // left line
                img = vElem;
                img.Point = new XPoint(X, Y + topLeft.Size.Height);
                img.Size = new XSize(img.Size.Width, H - topLeft.Size.Height - bottomLeft.Size.Height);
                img.Draw();
                // right line
                img = vElem;
                img.Point = new XPoint(X + W, Y + topRight.Size.Height);
                img.Size = new XSize(img.Size.Width, H - topRight.Size.Height - bottomRight.Size.Height);
                img.Alignment = Alignments.TopRight;
                img.Draw();
            }
        }
        #endregion

    }
    */
}
