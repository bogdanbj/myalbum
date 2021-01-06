using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PdfSharp.Drawing;

namespace MyAlbum2
{
    public enum Alignments
    {
        TopLeft,
        TopCenter,
        TopRight,
        //TopJustified,
        CenterLeft,
        CenterCenter,
        CenterRight,
        //CenterJustified,
        BottomLeft,
        BottomCenter,
        BottomRight//,
        //BottomJustified
    }

    interface IDrawable
    {
        XUnit X { get; set; }
        XUnit Y { get; set; }
        XUnit H { get; set; }
        XUnit W { get; set; }
        XUnit Padding { get; set; }
        Alignments Alignment { get; set; }

        void Draw();
    }
}
