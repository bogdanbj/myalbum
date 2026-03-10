using MyAlbum.Models.Xml.Styles;

namespace MyAlbum.Services
{
    public static class StyleFactory
    {
        /// <summary>
        /// Generic method to find a style by name or return the default style.
        /// </summary>
        /// <typeparam name="T">The style type (PageStyle, BorderStyle, RowStyle, etc.)</typeparam>
        /// <param name="styleName">The name of the style to find. If null or empty, returns the default style.</param>
        /// <param name="styles">The list of styles to search.</param>
        /// <returns>The matching style, the default style, or null if no match is found.</returns>
        public static T? FindStyle<T>(string styleName, List<T> styles) where T : class, IStyle
        {
            // Validate styles list
            if (styles == null || styles.Count == 0)
            {
                return null;
            }

            T? style = null;

            // If styleName is provided, look for a matching style by name
            if (!string.IsNullOrWhiteSpace(styleName))
            {
                style = styles.FirstOrDefault(s => s.Style == styleName);
            }

            // If no matching style found, look for the default style
            if (style == null)
            {
                style = styles.FirstOrDefault(s => s.IsDefault);
            }

            return style;
        }
    }
}
