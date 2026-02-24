using PdfSharpCore.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyAlbum.Models.Layout
{
    internal abstract class LayoutElement
    {
        public XUnit X { get; set; }
        public XUnit Y { get; set; }
        public XUnit Width { get; set; }
        public XUnit Height { get; set; }

    }
}
