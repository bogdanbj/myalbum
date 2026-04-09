using MyAlbum.Utilities;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace MyAlbum.Models
{
    internal class Album
    {
        public PdfDocument PdfDoc { get; set; }
        internal List<Page> Pages { get; set; } = new List<Page>();

        #region styles
        internal Dictionary<string, PageStyle> PageStyles { get; set; } = new();
        //internal Dictionary<string, BorderStyle> BorderStyles { get; set; } = new();

        #endregion

        public Album()
        {
            PdfDoc = new PdfDocument();
        }

        internal void ParseStyles(XElement styles)
        {
            if (styles == null)
                return;

            foreach (XElement xPage in styles.Elements("page"))
            {
                PageStyle pageStyle = new PageStyle();
                pageStyle.ParseXml(xPage);

                string? styleName = xPage.Attribute("style")?.Value;
                if (!string.IsNullOrEmpty(styleName))
                {
                    Styles.Page[styleName] = pageStyle;
                }

                bool isDefault = bool.Parse(xPage.Attribute("default")?.Value ?? "false");
                if (isDefault)
                {
                    Styles.Page["default"] = pageStyle;
                }
            }
        }

        internal void ParseXml(XElement root, PageSelection pageSelection)
        {
            if (root == null || root.Name != "MyAlbum")
                throw new InvalidOperationException("Invalid album XML format. Root node is null");

            //// Read album attributes (e.g., ver, styles)
            //string? version = root.Attribute("ver")?.Value;
            //string? styles = root.Attribute("styles")?.Value;

            //Check for inline <styles> element
            XElement? styles = root.Element("styles");
            if (styles != null)
            {
                this.ParseStyles(styles);
            }


            // Parse each page element
            int pageNo = 0;
            foreach (var pageElement in root.Elements("page"))
            {
                // Get page number
                pageNo++;
                //int pageNo = int.Parse(pageElement.Attribute("no")?.Value ?? "0");

                // Skip if we're filtering pages and this isn't in the list
                if (!pageSelection.IsEmpty && !pageSelection.Includes(pageNo))
                    continue;

                // Create and parse the page
                Page page = new Page();
                page.PageNo = pageNo;
                page.ParseXml(pageElement);
                Pages.Add(page);
            }
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
                page.Calculate(gfx);

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
        }
    }
}
