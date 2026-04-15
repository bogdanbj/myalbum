using System;
using System.Collections.Generic;
using System.Text;

namespace MyAlbum.Models
{
    internal static class Styles
    {
        // Style collections accessible by all classes
        public static Dictionary<string, PageStyle> Page { get; } = new();
        public static Dictionary<string, FrameStyle> Frame { get; } = new();
        public static Dictionary<string, RowStyle> Row { get; } = new();
        //public static Dictionary<string, TextStyle> Text { get; } = new();
        //public static Dictionary<string, ColumnStyle> Column { get; } = new();
        //public static Dictionary<string, ImageStyle> Image { get; } = new();
        //public static Dictionary<string, StampStyle> Stamp { get; } = new();

        // Helper method to get style or default
        public static T GetStyle<T>(this Dictionary<string, T> styles, string? styleName) where T : class
        {
            if (!string.IsNullOrEmpty(styleName) && styles.TryGetValue(styleName, out T? style))
                return style;

            // Return default style if exists
            styles.TryGetValue("default", out T? defaultStyle);
            return defaultStyle;
        }
    }
}
