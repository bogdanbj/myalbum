using MyAlbum.Models.Xml;
using MyAlbum.Utils;
using PdfSharpCore.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MyAlbum.Models.Layout
{
    internal class Space : BaseElement
    {
        internal void FromXml(XmlSpace xmlSpace, AlbumStyles styles)
        {
            Name = xmlSpace.Name;
            H = XUnit.FromMillimeter(xmlSpace.Height);
            W = XUnit.FromMillimeter(xmlSpace.Width);
        }
       internal override void CalculateSize(XGraphics gfx, XUnit w, XUnit h)
        {
            // Space dimensions are determined by Height and Width attributes in XML (set in FromXml)
            // This override is required by BaseElement but performs no additional calculation
 
            //W = W.Equals(XUnit.Zero) ? w : W;
            //H = H.Equals(XUnit.Zero) ? h : H;
            //W = !w.Equals(XUnit.Zero) ? w : W;
            //H = !h.Equals(XUnit.Zero) ? h : H;
        }
        internal override void Draw(XGraphics gfx)
        {
            #region test/debug drawing
            if ((Name ?? "").Contains("test", StringComparison.OrdinalIgnoreCase))
            {
                if (BgColor == XColors.White) // XColor.A == 0 means fully transparent (not set)
                {
                    BgColor = XColors.LightGray;
                    Helper.Fill(gfx, this);
                    Helper.WriteMySize(gfx, new Rect(X, Y, W, H));
                }
            }
            #endregion

            // draw the elements
            // do nothing
        }
    }
}
