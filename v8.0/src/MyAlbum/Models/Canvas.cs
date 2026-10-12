using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyAlbum.Models
{
    internal class Canvas
    {
        #region Core Properties
        public XUnit X { get; set; }
        public XUnit Y { get; set; }
        public XUnit W { get; set; }
        public XUnit H { get; set; }
        #endregion

        #region Constructors
        public Canvas() { }
        public Canvas(XUnit x, XUnit y, XUnit w, XUnit h)
        {
            X = x;
            Y = y;
            W = w;
            H = h;
        }
        #endregion
    }

}
