using PdfSharp.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MyAlbum
{
    internal class StyleElement : BaseElement
    {
        #region fields
        string? _styleName;
        XColor? _color;
        XBrush? _brush;
        List<DrawableElement> _components;
        #endregion
        
        #region properties
        public new string StyleName
        { 
            get
            { 
                if (string.IsNullOrEmpty(_styleName))
                    _styleName = "default"; 
                return _styleName;
            }
            set { _styleName = value; } 
        }
        public bool IsDefault { get; set; }
        public bool Absolute { get; set; }
        public Styles.Alignments Align { get; set; }
        public Styles.VerticalAlignments VAlign { get; set; }
        public XColor Color
        {
            get
            {
                if (!_color.HasValue)
                    _color = XColors.Black;
                return _color.Value;
            }
            set { _color = value; }
        }
        public XBrush Brush
        {
            get
            {
                if (_brush == null)
                    _brush = XBrushes.White;
                return _brush;
            }
            set { _brush = value; }
        }
        public XUnitPt MarginLeft { get; set; }
        public XUnitPt MarginRight { get; set; }
        public XUnitPt MarginTop { get; set; }
        public XUnitPt MarginBottom { get; set; }
        public XUnitPt PaddingLeft { get; set; }
        public XUnitPt PaddingRight { get; set; }
        public XUnitPt PaddingTop { get; set; }
        public XUnitPt PaddingBottom { get; set; }
        public XColor BackgroundColor { get; set; }
        public XUnitPt Width { get; set; }
        public XUnitPt Height { get; set; }

        public List<DrawableElement> Components 
        {
            get
            {   
                if (_components == null)
                    _components = new List<DrawableElement>();
                return _components;
            }
            set { _components = value;  } 
        }
        #endregion

        #region constructors
        internal StyleElement() 
        { }
        #endregion

        #region methods
        public static StyleElement DefaultStyleElement()
        {
            return new StyleElement();
        }
        internal void ParseBaseStyleAttributes()
        {
            // style name
            StyleName = ParseStyleNameAttribute(this.xml) ?? throw new InvalidOperationException("The 'style' attribute of a style is missing or empty."); ;

            // default flag
            IsDefault = ParseDefaultAttribute(this.xml);

            // margins
            XUnitPt[] arr = ParseMarginAttribute(this.xml);
            MarginTop = arr[0];
            MarginRight = arr[1];
            MarginBottom = arr[2];
            MarginLeft = arr[3];


            // padding
            arr = ParsePaddingAttribute(this.xml);
            PaddingTop = arr[0];
            PaddingRight = arr[1];
            PaddingBottom = arr[2];
            PaddingLeft = arr[3];

            // color
            Color = ParseColorAttribute(this.xml);

            // background color 
            Brush = ParseBackgroundColorAttribute(this.xml);
            
            // align attribute
            Align = ParseAlignAttribute(this.xml);

            // valign attribute
            VAlign = ParseVAlignAttribute(this.xml);

            // absolute attribute
            Absolute = ParseAbsoluteAttribute(this.xml);
        }
        #endregion
    }
}
