using MyAlbum.Models.Xml;
using MyAlbum.Models.Xml.Styles;
using MyAlbum.Services;
//using PdfSharp.Pdf;
using PdfSharpCore.Pdf;
using PdfSharpCore.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace MyAlbum.Models.Layout
{
    internal class Album
    {
        public PdfDocument PdfDoc { get; set; }
        internal List<Page> Pages { get; set; } = new List<Page>();

        public Album()
        {
            PdfDoc = new PdfDocument();
        }
        internal void FromXml(XmlAlbum xmlAlbum)
        {
            foreach (var xmlPage in xmlAlbum.Pages)
            {
                AddPageFromXml(xmlPage, xmlAlbum.Styles);
            }
        }
        internal void FromXml(XmlAlbum xmlAlbum, List<int> pageList)
        {
            foreach (int pageNumber in pageList)
            {
                var xmlPage = xmlAlbum.Pages.FirstOrDefault(p => p.Number == pageNumber);
                if (xmlPage != null)
                {
                    AddPageFromXml(xmlPage, xmlAlbum.Styles);
                }
                else
                {
                    Console.WriteLine($"Page with number {pageNumber} not found in XML album.");
                }
            }
        }

        private void AddPageFromXml(XmlPage xmlPage, AlbumStyles styles)
        {
            // Create a new album page
            Page page = new Page();
            // Add the page to the album
            Pages.Add(page);
            // Populate the page from the XML definition
            page.FromXml(xmlPage, styles);
        }

        internal void Draw()
        {
            foreach (var page in Pages)
            {
                // Add the PDF page to the document
                page.pdfPage = PdfDoc.AddPage();

                // Get XGraphics context for calculating and drawing the page
                XGraphics gfx = XGraphics.FromPdfPage(page.pdfPage);

                // Calculate the page
                page.Calculate(gfx, XUnit.Zero, XUnit.Zero, XUnit.Zero, XUnit.Zero);

                // Draw the page content
                page.Draw(gfx);
            }
        }
        internal void Draw(List<int> pageList)
        {
            foreach (int pageNumber in pageList)
            {
                Page page = Pages.FirstOrDefault(p => p.Number == pageNumber);
                if (page == null)
                    throw new ArgumentException($"Page with number {pageNumber} not found.");

                // Add the PDF page to the document
                page.pdfPage = PdfDoc.AddPage();

                // Get XGraphics context for calculating and drawing the page
                XGraphics gfx = XGraphics.FromPdfPage(page.pdfPage);

                // Calculate the page
                page.Calculate(gfx, XUnit.Zero, XUnit.Zero, XUnit.Zero, XUnit.Zero);

                // Draw the page content
                page.Draw(gfx);
            }

        }
        internal void Save(string outputName)
        {
            PdfDoc.Save(outputName);
        }

        internal void Test()
        {
            throw new NotImplementedException();
        }
    }
}
