using System;
using static System.Net.Mime.MediaTypeNames;
using System.Reflection.Metadata;
using System.Diagnostics;
using System.IO;
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using PdfSharp.Fonts;


namespace HelloWorld
{
    /// <summary>
    /// This sample is the obligatory Hello World program.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {

            // Register the custom font resolver
            GlobalFontSettings.FontResolver = new FontResolver();

            Album album = new Album();

            album.Parse();
            
            album.Draw();

            string outputName = "C:\\Git\\test\\PdfSharp\\HelloWorld\\output\\test.pdf";
            if (File.Exists(outputName))
            {
                for (int i = 0; i < 10000; i++)
                {
                    ;
                    if (! ( File.Exists(outputName.Replace(Path.GetExtension(outputName), i.ToString() + ".pdf" ))))
                    {
                        outputName = outputName.Replace(Path.GetExtension(outputName), i.ToString() + ".pdf");
                        break;
                    }
                }
            }

            album.Save(outputName);


            var psi = new ProcessStartInfo
            {
                FileName = outputName,
                UseShellExecute = true
            };
            Process.Start(psi);


            //GlobalFontSettings.UseWindowsFontsUnderWindows = true;

            //// Create a new PDF document
            //PdfDocument document = new PdfDocument();
            //document.Info.Title = "Created with PDFsharp";

            //// Create an empty page
            //PdfPage page = document.AddPage();

            // Get an XGraphics object for drawing
            //XGraphics gfx = XGraphics.FromPdfPage(page);


            //// Create a font
            //XPdfFontOptions options = new XPdfFontOptions(PdfFontEncoding.Unicode, PdfFontEmbedding.Always);
            //XFont font = new XFont("Stymie Becker Light", 20, XFontStyleEx.Regular, options);

            //// Draw the text
            //gfx.DrawString("TREFFLÉ BERTHIAUME", font, XBrushes.Black,
            //                new XRect(0, 0, page.Width.Point, page.Height.Point),
            //                XStringFormats.Center);

            //// Save the document...
            //const string filename = "HelloWorld.pdf";
            //document.Save(filename);

            //// ...and start a viewer.
            ////Process.Start(filename);
            //Process.Start(new ProcessStartInfo(filename) { UseShellExecute = true });
        }
    }
}