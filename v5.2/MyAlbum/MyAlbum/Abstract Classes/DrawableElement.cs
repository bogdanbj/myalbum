using MyAlbum;
using PdfSharp.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyAlbum
{
    internal class DrawableElement : BaseElement //StyleElement
    {
        #region fields
        private StyleElement? _style;
        #endregion

        #region properties
        public XGraphics? gfx { get; set; }
        public XUnitPt x { get; set; }
        public XUnitPt y { get; set; }
        public XUnitPt h { get; set; }
        public XUnitPt w { get; set; }
        public  XColor BoxColor { get; set; }
        public DrawableElement? Parent { get; set; }
        public StyleElement Style
        {
            get 
            {
                if (_style == null)
                    _style = new StyleElement();
                return _style; 
            }
            set { _style = value; }
        }
        #endregion

        #region virtual methods - must be overriden
        public virtual void Calculate()
        {
            throw new NotImplementedException();
        }
        public virtual void Draw()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region helper methods
        public void DrawBox()
        {
            if (BoxColor.IsEmpty)
            {
                DrawBox(Style.Color);
            }
            else
            {
                DrawBox(BoxColor);
            }
        }
        public void DrawBox(XColor color)
        {
            if (h >= 0)
                DrawBox(color, new XPoint(x, y), new XSize(w, h));
            else
                DrawBox(color, new XPoint(x, y + h), new XSize(w, -h));
        }
        public void DrawBox(XPoint point, XSize size)
        {
            if (BoxColor.IsEmpty)
            {
                DrawBox(Style.Color, point, size);
            }
            else
            {
                DrawBox(BoxColor, point, size);
            }
        }
        public void DrawBox(XColor color, XPoint point, XSize size)
        {
            gfx.DrawRectangle(new XPen(color, 0.1), point.X, point.Y, size.Width, size.Height);
        }
        public void DrawCross(XPoint point)
        {
            DrawCross(point, Style.Color);
        }
        public void DrawCross(XPoint point, XColor color)
        {
            XUnitPt d = XUnitPt.FromMillimeter(7);
            gfx.DrawLine(new XPen(color, 0.1), point.X - d, point.Y, point.X + d, point.Y);
            gfx.DrawLine(new XPen(color, 0.1), point.X, point.Y - d, point.X, point.Y + d);
        }
        public void DrawBackground()
        {
            if (Style.Brush is XSolidBrush solidBrush)
            {
                XColor color = solidBrush.Color;
                DrawBackground(color);
            }
        }
        public void DrawBackground(XColor color)
        {
            XBrush brush;
            brush = new XSolidBrush(color);
            if (h >= 0)
                gfx.DrawRectangle(brush, x, y, w, h);
            else
                gfx.DrawRectangle(brush, x, y + h, w, -h);
        }
        #endregion

    }

}

