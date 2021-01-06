using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using PdfSharp.Drawing;


namespace MyAlbum3
{
    class Gizmo1: BaseElement
    {
        public XUnit LineWidth { get; set; }

        public override void Draw()
        {
            Draw(false);
        }
        public void Draw(bool reverse)
        {
            XPen pen;
            pen = new XPen(this.Color, this.LineWidth);

            if (w > h)
            {

                //wipeout
                gfx.DrawLine(new XPen(XColors.White, LineWidth), x, y, x + w, y); ;
                gfx.DrawLine(new XPen(XColors.White, LineWidth), x, y + h, x + w, y + h); ;

                if (reverse)
                {
                    XPoint[] points;
                    points = new XPoint[4];
                    points[0].X = x;
                    points[0].Y = y + h;
                    points[1].X = x + 0.35 * w;
                    points[1].Y = y + h;
                    points[2].X = x + w;
                    points[2].Y = y;
                    points[3].X = x + 0.65 * w;
                    points[3].Y = y;
                    gfx.DrawPolygon(pen, new XSolidBrush(XColors.White), points, XFillMode.Winding);

                    points = new XPoint[4];
                    points[0].X = x;
                    points[0].Y = y;
                    points[1].X = x + 0.35 * w;
                    points[1].Y = y;
                    points[2].X = x + w;
                    points[2].Y = y + h;
                    points[3].X = x + 0.65 * w;
                    points[3].Y = y + h;
                    gfx.DrawPolygon(pen, new XSolidBrush(XColors.White), points, XFillMode.Winding);
                }
                else
                {
                    XPoint[] points;
                    points = new XPoint[4];
                    points[0].X = x;
                    points[0].Y = y;
                    points[1].X = x + 0.35 * w;
                    points[1].Y = y;
                    points[2].X = x + w;
                    points[2].Y = y + h;
                    points[3].X = x + 0.65 * w;
                    points[3].Y = y + h;
                    gfx.DrawPolygon(pen, new XSolidBrush(XColors.White), points, XFillMode.Winding);

                    points = new XPoint[4];
                    points[0].X = x;
                    points[0].Y = y + h;
                    points[1].X = x + 0.35 * w;
                    points[1].Y = y + h;
                    points[2].X = x + w;
                    points[2].Y = y;
                    points[3].X = x + 0.65 * w;
                    points[3].Y = y;
                    gfx.DrawPolygon(pen, new XSolidBrush(XColors.White), points, XFillMode.Winding);
                }
            }
            else
            {
                //wipeout
                gfx.DrawLine(new XPen(XColors.White, LineWidth), x, y, x, y + h); ;
                gfx.DrawLine(new XPen(XColors.White, LineWidth), x + w, y, x + w, y + h); ;

                if (reverse)
                {
                    XPoint[] points;
                    points = new XPoint[4];
                    points[0].X = x + w;
                    points[0].Y = y;
                    points[1].X = x + w;
                    points[1].Y = y + 0.35 * h;
                    points[2].X = x;
                    points[2].Y = y + h;
                    points[3].X = x;
                    points[3].Y = y + 0.65 * h;
                    gfx.DrawPolygon(pen, new XSolidBrush(XColors.White), points, XFillMode.Winding);

                    points = new XPoint[4];
                    points[0].X = x;
                    points[0].Y = y;
                    points[1].X = x;
                    points[1].Y = y + 0.35 * h;
                    points[2].X = x + w;
                    points[2].Y = y + h;
                    points[3].X = x + w;
                    points[3].Y = y + 0.65 * h;
                    gfx.DrawPolygon(pen, new XSolidBrush(XColors.White), points, XFillMode.Winding);
                }
                else
                {
                    XPoint[] points;
                    points = new XPoint[4];
                    points[0].X = x;
                    points[0].Y = y;
                    points[1].X = x;
                    points[1].Y = y + 0.35 * h;
                    points[2].X = x + w;
                    points[2].Y = y + h;
                    points[3].X = x + w;
                    points[3].Y = y + 0.65 * h;
                    gfx.DrawPolygon(pen, new XSolidBrush(XColors.White), points, XFillMode.Winding);

                    points = new XPoint[4];
                    points[0].X = x + w;
                    points[0].Y = y;
                    points[1].X = x + w;
                    points[1].Y = y + 0.35 * h;
                    points[2].X = x;
                    points[2].Y = y + h;
                    points[3].X = x;
                    points[3].Y = y + 0.65 * h;
                    gfx.DrawPolygon(pen, new XSolidBrush(XColors.White), points, XFillMode.Winding);
                }
            }
            // this.DrawCross(new XPoint(x, y), XColors.Blue);


            //_gfx.DrawRectangle(XBrushes.DarkOrange, x, y, w, h);

            //if (w > h)
            //{
            //    gfx.DrawRectangle(XBrushes.White, x, y - LineWidth, w, h + 2 * LineWidth);
            //    gfx.DrawLine(pen, x - adj, y + adj, x + w / 2 + adj, y + h - adj);
            //    gfx.DrawLine(pen, x - adj, y + h - adj, x + w / 2 + adj, y + adj);
            //    gfx.DrawLine(pen, x + w / 2 - adj, y + adj, x + w + adj, y + h - adj);
            //    gfx.DrawLine(pen, x + w / 2 - adj, y + h - adj, x + w + adj, y + adj);
            //}
            //else
            //{
            //    gfx.DrawRectangle(XBrushes.White, x - LineWidth, y, w + 2 * LineWidth, h);
            //    gfx.DrawLine(pen, x + adj, y - adj, x + w - adj, y + h / 2 + adj);
            //    gfx.DrawLine(pen, x + adj, y + h / 2 + adj, x + w - adj, y - adj);
            //    gfx.DrawLine(pen, x + adj, y + h / 2 - adj, x + w - adj, y + h + adj);
            //    gfx.DrawLine(pen, x + adj, y + h + adj, x + w - adj, y + h / 2 - adj);
            //}

        }

        public override void Parse()
        {
            throw new NotImplementedException();
        }

        public override void Calculate()
        {
            throw new NotImplementedException();
        }
    }
}
