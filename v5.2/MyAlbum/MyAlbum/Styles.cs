using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyAlbum
{
    static class Styles
    {
        #region Enumerations
        public enum BorderTypes
        {
            None,
            Single, 
            Double
        }
        public enum Alignments
        {
            Left,
            Center,
            Right
        }
        public enum VerticalAlignments
        {
            NotSet,
            Top,
            Center,
            Bottom
        }
        public enum SpacingModes
        {
            FS, // fixed spacing
            ES, // equal spacing
            JS  // justify spacing 
        }
        #endregion

        #region Static Fields
        static List<PageStyle> _pageStyles;
        static List<BorderStyle> _borderStyles;
        static List<RowStyle> _rowStyles;
        //static List<Column> _columnStyles;
        //static List<Text> _textStyles;
        //static List<Image> _imageStyles;
        //static List<Stamp> _stampStyles;
        #endregion

        #region Static Properties
        public static List<PageStyle> PageStyles
        {
            get
            {
                if (_pageStyles == null)
                {
                    _pageStyles = new List<PageStyle>();
                }
                return _pageStyles;
            }
            set { _pageStyles = value; }
        }
        public static List<BorderStyle> BorderStyles
        {
            get
            {
                if (_borderStyles == null)
                {
                    _borderStyles = new List<BorderStyle>();
                }
                return _borderStyles;
            }
            set { _borderStyles = value; }
        }
        public static List<RowStyle> RowStyles
        {
            get
            {
                if (_rowStyles == null)
                {
                    _rowStyles = new List<RowStyle>();
                }
                return _rowStyles;
            }
            set { _rowStyles = value; }
        }
        //public static List<Column> ColumnStyles
        //{
        //    get
        //    {
        //        if (_columnStyles == null)
        //        {
        //            _columnStyles = new List<Column>();
        //        }
        //        return _columnStyles;
        //    }
        //    set { _columnStyles = value; }
        //}
        //public static List<Text> TextStyles
        //{
        //    get
        //    {
        //        if (_textStyles == null)
        //        {
        //            _textStyles = new List<Text>();
        //        }
        //        return _textStyles;
        //    }
        //    set { _textStyles = value; }
        //}
        //public static List<Image> ImageStyles
        //{
        //    get
        //    {
        //        if (_imageStyles == null)
        //        {
        //            _imageStyles = new List<Image>();
        //        }
        //        return _imageStyles;
        //    }
        //    set { _imageStyles = value; }
        //}
        //public static List<Stamp> StampStyles
        //{
        //    get
        //    {
        //        if (_stampStyles == null)
        //        {
        //            _stampStyles = new List<Stamp>();
        //        }
        //        return _stampStyles;
        //    }
        //    set { _stampStyles = value; }
        //}
        #endregion

        #region IEnumerable extension methods
        //These methods override the GetFirstOrDefault methods of IEnumerable List<T>
        public static PageStyle FirstOrDefault(this List<PageStyle> pages, Func<PageStyle, bool> predicate)
        {
            var result = System.Linq.Enumerable.FirstOrDefault(pages, predicate);
            if (result == null)
                result = System.Linq.Enumerable.FirstOrDefault(pages, s => s.IsDefault == true);
            if (result == null)
                result = PageStyle.Default;
            return result;
        }
        public static BorderStyle FirstOrDefault(this List<BorderStyle> borders, Func<BorderStyle, bool> predicate)
        {
            var result = System.Linq.Enumerable.FirstOrDefault(borders, predicate);
            if (result == null)
                result = System.Linq.Enumerable.FirstOrDefault(borders, s => s.IsDefault == true);
            if (result == null)
                result = BorderStyle.Default;
            return result;
        }
        public static RowStyle FirstOrDefault(this List<RowStyle> rows, Func<RowStyle, bool> predicate)
        {
            var result = System.Linq.Enumerable.FirstOrDefault(rows, predicate);
            if (result == null)
                result = System.Linq.Enumerable.FirstOrDefault(rows, s => s.IsDefault == true);
            if (result == null)
                result = RowStyle.Default;
            return result;
        }
        #endregion


        public static BorderTypes GetBorderTypeFromString(string borderType)
        {
            switch (borderType.ToLower())
            {
                case "none":
                    return BorderTypes.None;
                case "single":
                    return BorderTypes.Single;
                case "double":
                    return BorderTypes.Double;
                default:
                    return BorderTypes.None;
            }
        }
    }
}
