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
        #region properties
        public bool IsDefault { get; set; }
        public string StyleName { get; set; }
        public Styles.Alignments Align { get; set; }
        public Styles.VerticalAlignments VAlign { get; set; }
        public XBrush Brush { get; set; }
        public XColor Color { get; set; }
        public XUnitPt MarginLeft { get; set; }
        public XUnitPt MarginRight { get; set; }
        public XUnitPt MarginTop { get; set; }
        public XUnitPt MarginBottom { get; set; }
        public XUnitPt PaddingLeft { get; set; }
        public XUnitPt PaddingRight { get; set; }
        public XUnitPt PaddingTop { get; set; }
        public XUnitPt PaddingBottom { get; set; }
        #endregion

        #region constructors
        protected StyleElement() 
        {
            Brush = XBrushes.White;
            Color = XColors.Black;
        }
        #endregion

        #region methods
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
            MarginTop = arr[0];
            MarginRight = arr[1];
            MarginBottom = arr[2];
            MarginLeft = arr[3];

            // color
            Color = ParseColorAttribute(this.xml);

            // background color 
            Brush = ParseBackgroundColorAttribute(this.xml);
            
            // align attribute
            Align = ParseAlignAttribute(this.xml);

            // valign attribute
            VAlign = ParseVAlignAttribute(this.xml);

        }
        #endregion
    }
}
