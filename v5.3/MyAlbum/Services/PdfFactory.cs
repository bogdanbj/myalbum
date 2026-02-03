using PdfSharpCore.Pdf;
using PdfSharpCore.Drawing;
using MyAlbum.Models.Xml;

namespace MyAlbum.Services
{
    public static class PdfFactory
    {
        public static void CreatePdfFromXmlAlbum(XmlAlbum xmlAlbum, string outputPath)
        {
            var document = new PdfDocument();

            foreach (var xmlPage in xmlAlbum.Pages)
            {
                var page = document.AddPage();
                var gfx = XGraphics.FromPdfPage(page);

                // Example: Draw page title (customize as needed)
                gfx.DrawString(xmlPage.Title ?? "Untitled",
                    new XFont("Arial", 20, XFontStyle.Bold),
                    XBrushes.Black,
                    new XRect(0, 0, page.Width, page.Height),
                    XStringFormats.TopCenter);

                // Add more content from xmlPage as needed
            }

            document.Save(outputPath);
        }
    }

}
