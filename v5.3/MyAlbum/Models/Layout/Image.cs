using MyAlbum.Models.Xml;
using MyAlbum.Utils;
using PdfSharpCore.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyAlbum.Models.Layout
{
    internal class Image : BaseElement
    {
        internal void FromXml(XmlImage xmlImage, AlbumStyles styles)
        {
            //throw new NotImplementedException();
        }
        internal override void Calculate(XGraphics gfx, XUnit x, XUnit y, XUnit w, XUnit h)
        {
            base.Calculate(gfx, x, y, w, h);
        }

        internal override void Draw(XGraphics gfx)
        {
            // TEST : fill Image
            Helper.Fill(gfx, this);
            Helper.WriteMe(gfx, this);
        }
    }
}
