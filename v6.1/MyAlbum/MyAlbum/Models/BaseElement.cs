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
    internal class BaseElement
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
            get => _color ?? Style?.Color ?? _parentColor ?? XColors.Black;
            set => _color = value;
        }
        public XColor BgColor
        {
            get => _bgColor ?? Style?.BgColor ?? _parentBgColor ?? XColors.Transparent;
            set => _bgColor = value;
        }
        public XUnit MarginTop
        {
            get => _marginTop ?? Style?.MarginTop ?? XUnit.Zero;
            set => _marginTop = value;
        }
        public XUnit MarginRight
        {
            get => _marginRight ?? Style?.MarginRight ?? XUnit.Zero;
            set => _marginRight = value;
        }
        public XUnit MarginBottom
        {
            get => _marginBottom ?? Style?.MarginBottom ?? XUnit.Zero;
            set => _marginBottom = value;
        }
        public XUnit MarginLeft
        {
            get => _marginLeft ?? Style?.MarginLeft ?? XUnit.Zero;
            set => _marginLeft = value;
        }
        #endregion

        #region Additional Properties
        public int PageNo { get; set; }
        #endregion

        public BaseElement()
        {
            Canvas = new Canvas();
        }

        internal void ParseXml(XElement element)
        {
            #region For testing only, remove later
            X = XUnit.FromMillimeter(double.Parse(element.Attribute("x")?.Value ?? "0"));
            Y = XUnit.FromMillimeter(double.Parse(element.Attribute("y")?.Value ?? "0"));
            W = XUnit.FromMillimeter(double.Parse(element.Attribute("width")?.Value ?? "0"));
            H = XUnit.FromMillimeter(double.Parse(element.Attribute("height")?.Value ?? "0"));
            #endregion
            _color = XmlParser.ParseColor(element.Attribute("color")?.Value);
            _bgColor = XmlParser.ParseColor(element.Attribute("bgcolor")?.Value);
            (_marginTop, _marginRight, _marginBottom, _marginLeft) = XmlParser.ParseMargin(element.Attribute("margin")?.Value);
        }
        internal void Inherit(BaseElement parent)
        {
            PageNo = parent.PageNo;
            _parentColor = parent.Color;
            _parentBgColor = parent.BgColor;
            //Align = parent.Align;
            //VAlign = parent.VAlign;
            //VSpace = parent.VSpace;
        }

        internal virtual void Draw(XGraphics gfx)
        {
            Console.WriteLine($"Drawing {this.GetType().Name} at ({X}, {Y}) with width {W} and height {H}.");
            gfx.DrawRectangle(new XPen(Color), new XSolidBrush(BgColor), X, Y, W, H);
        }


    }
}

