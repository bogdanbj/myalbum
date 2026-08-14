using PdfSharp.Drawing;
using PdfSharp.Pdf;

namespace MyAlbum.Models.Layout;

public class Album
{
    public string Title { get; set; } = "MyAlbum Output";
    public List<Page> Pages { get; set; } = [];

    /// <summary>
    /// Calculate layout for all pages.
    /// </summary>
    public void Calculate()
    {
        foreach (var page in Pages)
        {
            page.Calculate(0, 0); // Page determines its own dimensions
        }
    }

    /// <summary>
    /// Render album pages to PDF document.
    /// </summary>
    public void Draw(PdfDocument document)
    {
        document.Info.Title = Title;

        foreach (var page in Pages)
        {
            var pdfPage = document.AddPage();
            pdfPage.Width = XUnit.FromMillimeter(page.Width);
            pdfPage.Height = XUnit.FromMillimeter(page.Height);

            using var gfx = XGraphics.FromPdfPage(pdfPage);
            page.Draw(gfx);
        }
    }

    /// <summary>
    /// Save album to PDF file.
    /// </summary>
    public void Save(string outputPath)
    {
        using var document = new PdfDocument();
        Draw(document);
        document.Save(outputPath);
    }

    /// <summary>
    /// Save album to stream.
    /// </summary>
    public void Save(Stream outputStream)
    {
        using var document = new PdfDocument();
        Draw(document);
        document.Save(outputStream);
    }
}
