using MyAlbum.Utilities;
using SixLabors.Fonts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using PdfSharpCore.Drawing;

namespace MyAlbum.Models
{
    internal class Text : BaseElement
    {
        #region fields
        protected Alignment? _align;
        protected string? _fontName;
        protected double? _fontSize;
        protected XFontStyle? _fontStyle;
        private XFont _font;
        #endregion

        #region style properties
        public new TextStyle Style 
        { 
            get => (TextStyle)base.Style;
            set => base.Style = value;
        }
        public Alignment Align 
        { 
            get => _align ?? Style.Align ?? Alignment.Center; 
            set => _align = value;
        }
        public string FontName 
        { 
            get => _fontName ?? Style.FontName ?? "Verdana"; 
            set => _fontName = value;
        }
        public double FontSize
        {
            get => _fontSize ?? Style.FontSize ?? 12; 
            set => _fontSize = value;
        }
        public XFontStyle FontStyle 
        { 
            get => _fontStyle ?? Style.FontStyle ?? XFontStyle.Regular;
            set => _fontStyle = value;
        }
        public XFont Font
        {
            get 
            { 
                if (_font == null)
                {
                    _font = new XFont(FontName, FontSize, FontStyle);
                }
                return _font; 
            }
            set => _font = value; 
        }

        #endregion

        #region constructors
        public Text()
        {
            Style = Styles.Text.GetStyle("default") ?? new TextStyle();
        }
        #endregion

        internal override void ParseXml(XElement xText)
        {
            base.ParseXml(xText);
            Style = Styles.Text.GetStyle(xText.Attribute("style")?.Value);

            _align = XmlParser.ParseAlignment(xText.Attribute("align")?.Value);
            _fontName = xText.Attribute("font-name")?.Value;
            _fontSize = XmlParser.ParseDouble(xText.Attribute("font-size")?.Value);
            _fontStyle = XmlParser.ParseFontStyle(xText.Attribute("font-style")?.Value);
            
            //PageNumber = int.Parse(pageElement.Attribute("no")?.Value ?? "0");
        }
    }
}
