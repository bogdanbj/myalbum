using MyAlbum.Models.Pdf;
using MyAlbum.Models.Xml;
using MyAlbum.Models.Xml.Styles;
using MyAlbum.Models.Layout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyAlbum.Services
{
    internal class PdfCalculator
    {
       public static LayoutPage CalculatePage(XmlPage xmlPage, AlbumStyles styles)
        {
            // Find the appropriate page style
            PageStyle pageStyle = FindPageStyle(xmlPage.Style, styles.PageStyles);
            if (pageStyle == null) {
                throw new InvalidOperationException(
                    $"Page style '{xmlPage.Style ?? "(default)"}' not found. " +
                    $"Ensure a matching PageStyle exists in the album styles or that a default PageStyle is defined.");
            }



            // Apply the style
            var pdfPage = new LayoutPage
            {
                Orientation = xmlPage.Orientation ?? pageStyle.Orientation

                //Size = xmlPage.Size ?? pageStyle.Size,
                //Title = xmlPage.Title ?? pageStyle.Title,
                //Orientation = xmlPage.Orientation ?? pageStyle.Orientation,
                //Number = xmlPage.Number != 0 ? xmlPage.Number : pageStyle.Number,
                //Margin = xmlPage.Margin ?? pageStyle.Margin,
                //Color = xmlPage.Color ?? pageStyle.Color,
                //VSpace = xmlPage.VSpace != 0 ? xmlPage.VSpace : pageStyle.VSpace,
                //Rows = xmlPage.Rows?.Select(r => CalculateRow(r, 0, 0)).ToList()
            };
            return pdfPage;
        }

        private static PageStyle FindPageStyle(string styleName, List<PageStyle> pageStyles)
        {
            // Validate xmlAlbum and styles
            if (pageStyles == null || pageStyles.Count == 0)
            {
                return null;
            }

            PageStyle pageStyle = null;

            // If xmlPage has a style attribute, look for a matching PageStyle by name
            if (!string.IsNullOrWhiteSpace(styleName))
            {
                pageStyle = pageStyles.FirstOrDefault(ps => ps.Style == styleName);
            }

            // If no matching style found, look for the default PageStyle
            if (pageStyle == null)
            {
                pageStyle = pageStyles.FirstOrDefault(ps => ps.IsDefault);
            }

            return pageStyle;
        }
        //internal PdfRow CalculateRow(XmlRow xmlRow, double parentX, double parentY)
        //{
        //    // Calculate absolute coordinates, parse colors, fonts, etc.
        //    return new PdfRow { /* calculated properties */ };
        //}
    }
}
