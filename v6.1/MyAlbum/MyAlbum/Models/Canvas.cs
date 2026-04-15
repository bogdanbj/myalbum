using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyAlbum.Models
{
    internal class Canvas
    {
        public XUnit X { get; set; }
        public XUnit Y { get; set; }
        public XUnit W { get; set; }
        public XUnit H { get; set; }

        public Canvas() { }

        public Canvas(XUnit x, XUnit y, XUnit w, XUnit h)
        {
            X = x;
            Y = y;
            W = w;
            H = h;
        }

        //public void Draw(XGraphics gfx) 
        //{
        //    gfx.DrawRectangle(
        //        new XSolidBrush(XColors.MistyRose), 
        //        X, Y, W, H);
        //}
    }

}
