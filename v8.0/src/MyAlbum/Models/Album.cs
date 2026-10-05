using System.Xml.Linq;
using PdfSharp.Drawing;
using PdfSharp.Pdf;

namespace MyAlbum.Models
{
    /// <summary>
    /// STAGE 1 SKELETON. Minimal Album that parses pages and produces a (near-empty)
    /// PDF so the pipeline builds and runs end-to-end. Real parsing/styling/drawing
    /// is ported in Stage 2.
    /// </summary>
    internal class Album
    {
        #region Properties
        public PdfDocument PdfDoc { get; set; }
        internal List<Page> Pages { get; set; } = new List<Page>();
        #endregion
        
        #region Constructors
        public Album()
        {
            PdfDoc = new PdfDocument();
        }
        #endregion

        #region Methods
        internal void ParseStyles(XElement styles)
        {
            if (styles == null)
                return;

            ParseStyleElements<PageStyle>(styles, "page", Styles.Page);
            ParseStyleElements<FrameStyle>(styles, "frame", Styles.Frame);
            ParseStyleElements<RowStyle>(styles, "row", Styles.Row);
            ParseStyleElements<TextStyle>(styles, "text", Styles.Text);
            ParseStyleElements<ImageStyle>(styles, "image", Styles.Image);
            ParseStyleElements<SpaceStyle>(styles, "space", Styles.Space);
            ParseStyleElements<StampStyle>(styles, "stamp", Styles.Stamp);
        }
        private void ParseStyleElements<T>(XElement styles, string elementName, Dictionary<string, T> styleDictionary)
            where T : BaseElementStyle, new()
        {
            foreach (XElement element in styles.Elements(elementName))
            {
                T style = new T();
                style.ParseXml(element);

                string? styleName = element.Attribute("style")?.Value;
                if (!string.IsNullOrEmpty(styleName))
                {
                    styleDictionary[styleName] = style;
                }

                bool isDefault = bool.Parse(element.Attribute("default")?.Value ?? "false");
                if (isDefault)
                {
                    styleDictionary["default"] = style;
                }
            }
        }
        internal void ParseXml(XElement root, Utilities.PageSelection pageSelection)
        {
            if (root == null || root.Name != "MyAlbum")
                throw new InvalidOperationException("Invalid album XML format. Root must be <MyAlbum>");

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

                // Skip if we're filtering pages and this isn't in the list
                if (!pageSelection.IsEmpty && !pageSelection.Includes(pageNo))
                    continue;

                Page page = new Page { PageNo = pageNo };
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
        #endregion
    }
}
