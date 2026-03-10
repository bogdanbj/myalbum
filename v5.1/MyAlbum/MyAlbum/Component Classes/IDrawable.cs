using PdfSharp.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyAlbum
{
    internal interface IDrawable
    {
        public XGraphics? gfx { get; set; }
        public XUnitPt x { get; set; }
        public XUnitPt y { get; set; }
        public XUnitPt h { get; set; }
        public XUnitPt w { get; set; }
        public StyleElement? Parent { get; set; }
        void Draw();
        void Parse();
        void Calculate();
    }
}
