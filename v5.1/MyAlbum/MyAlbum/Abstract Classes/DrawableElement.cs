using MyAlbum;
using PdfSharp.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyAlbum
{
    abstract class DrawableElement : StyleElement
    {
        public XGraphics? gfx { get; set; }
        public XUnitPt x { get; set; }
        public XUnitPt y { get; set; }
        public XUnitPt h { get; set; }
        public XUnitPt w { get; set; }
        public DrawableElement? Parent { get; set; }

        public abstract void Calculate();
        public abstract void Draw();

    }
}

