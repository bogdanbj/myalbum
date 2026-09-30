using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyAlbum.Samples
{
    internal class Shapes : Samples
    {
        public override void DrawPage(PdfPage page)
        {
            XGraphics gfx = XGraphics.FromPdfPage(page);
            DrawTitle(page, gfx, "Shapes");

            DrawRectangle(gfx, 1);
            DrawRoundedRectangle(gfx, 2);
            DrawEllipse(gfx, 3);
            DrawPolygon(gfx, 4);
            DrawPie(gfx, 5);
            DrawClosedCurve(gfx, 6);
        }

        void DrawRectangle(XGraphics gfx, int number)
        {
            BeginBox(gfx, number, "DrawRectangle");

            XPen pen = new XPen(XColors.Navy, Math.PI);

            gfx.DrawRectangle(pen, 10, 0, 100, 60);
            gfx.DrawRectangle(XBrushes.DarkOrange, 130, 0, 100, 60);
            gfx.DrawRectangle(pen, XBrushes.DarkOrange, 10, 80, 100, 60);
            gfx.DrawRectangle(pen, XBrushes.DarkOrange, 150, 80, 60, 60);

            EndBox(gfx);
        }
        void DrawRoundedRectangle(XGraphics gfx, int number)
        {
            BeginBox(gfx, number, "DrawRoundedRectangle");

            XPen pen = new XPen(XColors.RoyalBlue, Math.PI);

            gfx.DrawRoundedRectangle(pen, 10, 0, 100, 60, 30, 20);
            gfx.DrawRoundedRectangle(XBrushes.Orange, 130, 0, 100, 60, 30, 20);
            gfx.DrawRoundedRectangle(pen, XBrushes.Orange, 10, 80, 100, 60, 30, 20);
            gfx.DrawRoundedRectangle(pen, XBrushes.Orange, 150, 80, 60, 60, 20, 20);

            EndBox(gfx);
        }
        void DrawEllipse(XGraphics gfx, int number)
        {
            BeginBox(gfx, number, "DrawEllipse");

            XPen pen = new XPen(XColors.DarkBlue, 2.5);

            gfx.DrawEllipse(pen, 10, 0, 100, 60);
            gfx.DrawEllipse(XBrushes.Goldenrod, 130, 0, 100, 60);
            gfx.DrawEllipse(pen, XBrushes.Goldenrod, 10, 80, 100, 60);
            gfx.DrawEllipse(pen, XBrushes.Goldenrod, 150, 80, 60, 60);

            EndBox(gfx);
        }
        void DrawPolygon(XGraphics gfx, int number)
        {
            BeginBox(gfx, number, "DrawPolygon");

            XPen pen = new XPen(XColors.DarkBlue, 2.5);

            gfx.DrawPolygon(pen, XBrushes.LightCoral, GetPentagramPoints(50, new XPoint(60, 70)), XFillMode.Winding);
            gfx.DrawPolygon(pen, XBrushes.LightCoral, GetPentagramPoints(50, new XPoint(180, 70)), XFillMode.Alternate);

            EndBox(gfx);
        }
        void DrawPie(XGraphics gfx, int number)
        {
            BeginBox(gfx, number, "DrawPie");

            XPen pen = new XPen(XColors.DarkBlue, 2.5);

            gfx.DrawPie(pen, 10, 0, 100, 90, -120, 75);
            gfx.DrawPie(XBrushes.Gold, 130, 0, 100, 90, -160, 150);
            gfx.DrawPie(pen, XBrushes.Gold, 10, 50, 100, 90, 80, 70);
            gfx.DrawPie(pen, XBrushes.Gold, 150, 80, 60, 60, 35, 290);

            EndBox(gfx);
        }
        void DrawClosedCurve(XGraphics gfx, int number)
        {
            BeginBox(gfx, number, "DrawClosedCurve");

            XPen pen = new XPen(XColors.DarkBlue, 2.5);
            gfx.DrawClosedCurve(
                pen, XBrushes.SkyBlue,
                new XPoint[]
                {
            new XPoint(10, 120), new XPoint(80, 30),
            new XPoint(220, 20), new XPoint(170, 110),
            new XPoint(100, 90)
                },
                XFillMode.Winding, 0.7);

            EndBox(gfx);
        }

        private XPoint[] GetPentagramPoints(double radius, XPoint point)
        {
            XPoint[] points = new XPoint[5];
            for (int i = 0; i < 5; i++)
            {
                // Calculate angle for each point (every 72 degrees, offset by 90)
                double angle = (i * 144 - 90) * Math.PI / 180;
                points[i] = new XPoint(
                    point.X + radius * Math.Cos(angle),
                    point.Y + radius * Math.Sin(angle)
                );
            }
            return points;
        }

    }
}
