using PdfSharpCore.Drawing;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using MyAlbum.Utilities;

namespace MyAlbum.Models
{
    internal abstract class BaseElement
    {
        #region fields
        protected XColor? _color;
        protected XColor? _bgColor;
        protected XColor? _parentColor;
        protected XColor? _parentBgColor;
        protected XUnit? _marginTop;
        protected XUnit? _marginRight;
        protected XUnit? _marginBottom;
        protected XUnit? _marginLeft;
        protected XUnit? _paddingTop;
        protected XUnit? _paddingRight;
        protected XUnit? _paddingBottom;
        protected XUnit? _paddingLeft;
        #endregion

        #region Core Properties
        public string? StyleName { get; set; }
        public XUnit X { get; set; }
        public XUnit Y { get; set; }
        public XUnit W { get; set; }
        public XUnit H { get; set; }
        public Canvas Canvas { get; set; }
        //public double Rotate { get; set; } = 0;
        #endregion

        #region Properties accepting Styles 
        public BaseElementStyle Style { get; set; }
        public XColor Color 
        {
            get => _color ?? Style.Color ?? _parentColor ?? XColors.Black;
            set => _color=value;
        }
        public XColor BgColor 
        { 
            get => _bgColor ?? Style.BgColor ?? _parentBgColor ?? XColors.Transparent; 
            set => BgColor = value; 
        }
        public XUnit MarginTop 
        { 
            get => _marginTop ?? Style.MarginTop ?? XUnit.Zero; 
            set => _marginTop = value; 
        }
        public XUnit MarginRight 
        { 
            get => _marginRight ?? Style.MarginRight ?? XUnit.Zero; 
            set => _marginRight = value; 
        }
        public XUnit MarginBottom 
        { 
            get => _marginBottom ?? Style.MarginBottom ?? XUnit.Zero; 
            set => _marginBottom = value; 
        }
        public XUnit MarginLeft 
        { 
            get => _marginLeft ?? Style.MarginLeft ?? XUnit.Zero; 
            set => _marginLeft = value; 
        }
        public XUnit PaddingTop 
        { 
            get => _paddingTop ?? Style.PaddingTop ?? XUnit.Zero; 
            set => _paddingTop = value; 
        }
        public XUnit PaddingRight
        {
            get => _paddingRight ?? Style.PaddingRight ?? XUnit.Zero;
            set => _paddingRight = value;
        }
        public XUnit PaddingBottom  
        {
            get => _paddingBottom ?? Style.PaddingBottom ?? XUnit.Zero;
            set => _paddingBottom = value;
        }
        public XUnit PaddingLeft
        {
            get => _paddingLeft ?? Style.PaddingLeft ?? XUnit.Zero;
            set => _paddingLeft = value;
        }
        #endregion

        #region Additional Properties
        public int PageNo { get; set; }
        public int NestingLevel { get; set; }
        #endregion

        public BaseElement()
        {
            Style = new BaseElementStyle();
            Canvas = new Canvas();
        }

        internal virtual void ParseXml(XElement element)
        {
            //#region For testing only, remove later
            //X = XUnit.FromMillimeter(double.Parse(element.Attribute("x")?.Value ?? "0"));
            //Y = XUnit.FromMillimeter(double.Parse(element.Attribute("y")?.Value ?? "0"));
            //W = XUnit.FromMillimeter(double.Parse(element.Attribute("width")?.Value ?? "0"));
            //H = XUnit.FromMillimeter(double.Parse(element.Attribute("height")?.Value ?? "0"));
            //#endregion
            _color = XmlParser.ParseColor(element.Attribute("color")?.Value);
            _bgColor = XmlParser.ParseColor(element.Attribute("bgcolor")?.Value);
            (_marginTop, _marginRight, _marginBottom, _marginLeft) = XmlParser.ParseMargin(element.Attribute("margin")?.Value);// ?? Style?.Margin ?? "0");
            (_paddingTop, _paddingRight, _paddingBottom, _paddingLeft) = XmlParser.ParsePadding(element.Attribute("padding")?.Value);
        }
        internal virtual void Inherit(BaseElement parent)
        {
            PageNo = parent.PageNo;
            _parentColor = parent.Color;
            _parentBgColor = parent.BgColor;
            //Align = parent.Align;
            //VAlign = parent.VAlign;
            //VSpace = parent.VSpace;
        }
        internal virtual void Calculate(XGraphics gfx, Canvas parentCanvas)
        {
            // Default implementation - position at top of available canvas
            X = parentCanvas.X + MarginLeft;
            Y = parentCanvas.Y + MarginTop;
            W = parentCanvas.W - (MarginLeft + MarginRight);
            //H = parentCanvas.H - (MarginTop + MarginBottom);            
            // W and H should already be set or calculated by derived classes
        }
        internal virtual void Draw(XGraphics gfx)
        {
            Console.WriteLine($"Drawing {this.GetType().Name} at ({X.Millimeter:F1}, {Y.Millimeter:F1}) with width {W.Millimeter:F1} and height {H.Millimeter:F1}.");
            gfx.DrawRectangle(new XPen(Color, 0.5), new XSolidBrush(BgColor), X, Y, W, H);
        }


    }
}

