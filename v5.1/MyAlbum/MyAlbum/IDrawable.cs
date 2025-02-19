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
        public XGraphics? Gfx { get; set; }
        public void Draw();
    }
}
