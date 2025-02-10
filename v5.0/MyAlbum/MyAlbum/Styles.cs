using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyAlbum
{
    static class Styles
    {
        #region Enumerations
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
        static List<Page> _pageStyles;
        static List<Border> _borderStyles;
        static List<Row> _rowStyles;
        static List<Column> _columnStyles;
        static List<Text> _textStyles;
        static List<Image> _imageStyles;
        static List<Stamp> _stampStyles;
        #endregion

        #region Static Properties
        public static List<Page> PageStyles
        {
            get
            {
                if (_pageStyles == null)
                {
                    _pageStyles = new List<Page>();
                }
                return _pageStyles;
            }
            set { _pageStyles = value; }
        }
        public static List<Border> BorderStyles
        {
            get
            {
                if (_borderStyles == null)
                {
                    _borderStyles = new List<Border>();
                }
                return _borderStyles;
            }
            set { _borderStyles = value; }
        }
        public static List<Row> RowStyles
        {
            get
            {
                if (_rowStyles == null)
                {
                    _rowStyles = new List<Row>();
                }
                return _rowStyles;
            }
            set { _rowStyles = value; }
        }
        public static List<Column> ColumnStyles
        {
            get
            {
                if (_columnStyles == null)
                {
                    _columnStyles = new List<Column>();
                }
                return _columnStyles;
            }
            set { _columnStyles = value; }
        }
        public static List<Text> TextStyles
        {
            get
            {
                if (_textStyles == null)
                {
                    _textStyles = new List<Text>();
                }
                return _textStyles;
            }
            set { _textStyles = value; }
        }
        public static List<Image> ImageStyles
        {
            get
            {
                if (_imageStyles == null)
                {
                    _imageStyles = new List<Image>();
                }
                return _imageStyles;
            }
            set { _imageStyles = value; }
        }
        public static List<Stamp> StampStyles
        {
            get
            {
                if (_stampStyles == null)
                {
                    _stampStyles = new List<Stamp>();
                }
                return _stampStyles;
            }
            set { _stampStyles = value; }
        }
        #endregion

    }
}
