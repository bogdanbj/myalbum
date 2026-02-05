using PdfSharpCore.Pdf;
using PdfSharpCore.Drawing;
using MyAlbum.Models.Xml;

namespace MyAlbum.Services
{
    public static class PdfFactory
    {
        public static void CreatePdfFromXmlAlbum(XmlAlbum xmlAlbum, string outputPath)
        {
            PdfDocument pdfDocument = new PdfDocument();

            // Create a page in the document for each XmlPage
            foreach (var xmlPage in xmlAlbum.Pages)
            {
                // Add a new page to the document
                PdfPage pdfPage = pdfDocument.AddPage();

                // Get XGraphics context for drawing on the page
                XGraphics gfx = XGraphics.FromPdfPage(pdfPage);

                // Calculate the PDF page from XML definition
                var layoutPage = PdfCalculator.CalculatePage(xmlPage, xmlAlbum.Styles);

                PdfRenderer.RenderPage(pdfPage, gfx, layoutPage);
                // Use gfx here to render page content
                // gfx.DrawString("Hello World", ...);
            }

            pdfDocument.Save(outputPath);
        }






    }

}
