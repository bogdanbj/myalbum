using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyAlbum.Samples
{
    internal class Images : Samples
    {
        string jpegSamplePath = "C:\\Git\\myalbum\\Images\\0284.jpg";

        public override void DrawPage(PdfPage page)
        {
            XGraphics gfx = XGraphics.FromPdfPage(page);
            DrawTitle(page, gfx, "Images");

            DrawImage(gfx, 1);
            DrawImageScaled(gfx, 2);
            DrawImageRotated(gfx, 3);
            DrawImageSheared(gfx, 4);
            //DrawGif(gfx, 5);
            //DrawPng(gfx, 6);
            //DrawTiff(gfx, 7);
            //DrawFormXObject(gfx, 8);
        }

        void DrawImage(XGraphics gfx, int number)
        {
            BeginBox(gfx, number, "DrawImage (original)");

            XImage image = XImage.FromFile(jpegSamplePath);

            // Left position in point
            double x = (250 - image.PixelWidth * 72 / image.HorizontalResolution) / 2;
            gfx.DrawImage(image, x, 0);

            EndBox(gfx);
        }
        void DrawImageScaled(XGraphics gfx, int number)
        {
            BeginBox(gfx, number, "DrawImage (scaled)");

            XImage image = XImage.FromFile(jpegSamplePath);
            gfx.DrawImage(image, 0, 0, 250, 140);

            EndBox(gfx);
        }
        void DrawImageRotated(XGraphics gfx, int number)
        {
            BeginBox(gfx, number, "DrawImage (rotated)");

            XImage image = XImage.FromFile(jpegSamplePath);

            const double dx = 250, dy = 140;

            gfx.TranslateTransform(dx / 2, dy / 2);
            gfx.ScaleTransform(0.7);
            gfx.RotateTransform(-25);
            gfx.TranslateTransform(-dx / 2, -dy / 2);

            //XMatrix matrix = new XMatrix();  //XMatrix.Identity;

            double width = image.PixelWidth * 72 / image.HorizontalResolution;
            double height = image.PixelHeight * 72 / image.HorizontalResolution;

            gfx.DrawImage(image, (dx - width) / 2, 0, width, height);

            EndBox(gfx);
        }
        void DrawImageSheared(XGraphics gfx, int number)
        {
            BeginBox(gfx, number, "DrawImage (sheared)");

            XImage image = XImage.FromFile(jpegSamplePath);

            const double dx = 250, dy = 140;

            gfx.TranslateTransform(dx / 2, dy / 2);
            gfx.ScaleTransform(-0.7, 0.7);
            gfx.ShearTransform(-0.4, -0.3);
            gfx.TranslateTransform(-dx / 2, -dy / 2);

            double width = image.PixelWidth * 72 / image.HorizontalResolution;
            double height = image.PixelHeight * 72 / image.HorizontalResolution;

            gfx.DrawImage(image, (dx - width) / 2, 0, width, height);

            EndBox(gfx);
        }
        //void DrawGif(XGraphics gfx, int number)
        //{
        //    this.backColor = XColors.LightGoldenrodYellow;
        //    this.borderPen = new XPen(XColor.FromArgb(202, 121, 74), this.borderWidth);
        //    BeginBox(gfx, number, "DrawImage (GIF)");

        //    XImage image = XImage.FromFile(gifSamplePath);

        //    const double dx = 250, dy = 140;

        //    double width = image.PixelWidth * 72 / image.HorizontalResolution;
        //    double height = image.PixelHeight * 72 / image.HorizontalResolution;

        //    gfx.DrawImage(image, (dx - width) / 2, (dy - height) / 2, width, height);

        //    EndBox(gfx);
        //}
        //void DrawPng(XGraphics gfx, int number)
        //{
        //    BeginBox(gfx, number, "DrawImage (PNG)");

        //    XImage image = XImage.FromFile(pngSamplePath);

        //    const double dx = 250, dy = 140;

        //    double width = image.PixelWidth * 72 / image.HorizontalResolution;
        //    double height = image.PixelHeight * 72 / image.HorizontalResolution;

        //    gfx.DrawImage(image, (dx - width) / 2, (dy - height) / 2, width, height);

        //    EndBox(gfx);
        //}
        //void DrawTiff(XGraphics gfx, int number)
        //{
        //    XColor oldBackColor = this.backColor;
        //    this.backColor = XColors.LightGoldenrodYellow;
        //    BeginBox(gfx, number, "DrawImage (TIFF)");

        //    XImage image = XImage.FromFile(tiffSamplePath);

        //    const double dx = 250, dy = 140;

        //    double width = image.PixelWidth * 72 / image.HorizontalResolution;
        //    double height = image.PixelHeight * 72 / image.HorizontalResolution;

        //    gfx.DrawImage(image, (dx - width) / 2, (dy - height) / 2, width, height);

        //    EndBox(gfx);
        //    this.backColor = oldBackColor;
        //}
        //void DrawFormXObject(XGraphics gfx, int number)
        //{
        //    //this.backColor = XColors.LightSalmon;
        //    BeginBox(gfx, number, "DrawImage (Form XObject)");

        //    XImage image = XImage.FromFile(pdfSamplePath);

        //    const double dx = 250, dy = 140;

        //    gfx.TranslateTransform(dx / 2, dy / 2);
        //    gfx.ScaleTransform(0.35);
        //    gfx.TranslateTransform(-dx / 2, -dy / 2);

        //    double width = image.PixelWidth * 72 / image.HorizontalResolution;
        //    double height = image.PixelHeight * 72 / image.HorizontalResolution;

        //    gfx.DrawImage(image, (dx - width) / 2, (dy - height) / 2, width, height);

        //    EndBox(gfx);
        //}
    }
}
