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
    internal class StyleFactory
    {

        private AlbumStyles _styles;

        public AlbumStyles Styles
        {
            get { return _styles; }
            set { _styles = value; }
        }

        #region Constructors
        public StyleFactory()
        {
            _styles = new AlbumStyles();
        }
        public StyleFactory(AlbumStyles styles)
        {
            _styles = styles;
        }
        #endregion

        internal void SetPageStyle(LayoutPage layoutPage, XmlPage xmlPage)
        {
            PageStyle pageStyle = GetStyle<PageStyle>(xmlPage.Style, Styles.PageStyles);
            if (pageStyle == null)
            {
                throw new InvalidOperationException(
                    $"Page style '{xmlPage.Style ?? "(default)"}' not found. " +
                    $"Ensure a matching PageStyle exists in the album styles or that a default PageStyle is defined.");
            }


            // Apply the style
            layoutPage.Orientation = xmlPage.Orientation ?? pageStyle.Orientation;
            //layoutPage.Size = xmlPage.Size ?? pageStyle.Size,
            //layoutPage.Title = xmlPage.Title ?? pageStyle.Title,
            //layoutPage.Number = xmlPage.Number != 0 ? xmlPage.Number : pageStyle.Number,
            //layoutPage.Margin = xmlPage.Margin ?? pageStyle.Margin,
            //layoutPage.Color = xmlPage.Color ?? pageStyle.Color,
            //layoutPage.VSpace = xmlPage.VSpace != 0 ? xmlPage.VSpace : pageStyle.VSpace,
            //layoutPage.Rows = xmlPage.Rows?.Select(r => CalculateRow(r, 0, 0)).ToList()

        }


        internal T GetStyle<T>(string styleName, List<T> styles) where T : class
        {
            // Validate styles
            if (styles == null || styles.Count == 0)
            {
                return null;
            }
            T style = null;
            // If styleName is provided, look for a matching style by name
            if (!string.IsNullOrWhiteSpace(styleName))
            {
                style = styles.FirstOrDefault(s =>
                {
                    var prop = s.GetType().GetProperty("Style");
                    return prop != null && (string)prop.GetValue(s) == styleName;
                });
            }
            // If no matching style found, look for the default style
            if (style == null)
            {
                style = styles.FirstOrDefault(s =>
                {
                    var prop = s.GetType().GetProperty("IsDefault");
                    return prop != null && (bool)prop.GetValue(s);
                });
            }
            return style;
        }

    }
}
