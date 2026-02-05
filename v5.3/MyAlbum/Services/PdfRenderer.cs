using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MyAlbum.Models.Layout;
using PdfSharpCore.Pdf;
using PdfSharpCore.Drawing;

namespace MyAlbum.Services
{
    internal class PdfRenderer
    {
        public static void RenderPage(PdfPage pdfPage, XGraphics gfx, LayoutPage layoutPage)
        {
            // Implement rendering logic here
            // This could involve using a PDF library to create a PDF document
            // and add content based on the layoutPage properties
            pdfPage.Orientation = layoutPage.PdfOrientation;


            // TEST
            XBrush greenBrush = new XSolidBrush(XColor.FromArgb(0, 128, 0));
            gfx.DrawRectangle(greenBrush, 0, 0, pdfPage.Width, pdfPage.Height);
        }
    }
}
