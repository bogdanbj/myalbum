using MyAlbum.Models.Xml;
using MyAlbum.Models.Xml.Styles;
using MyAlbum.Services;
using MyAlbum.Utils;
using PdfSharpCore.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MyAlbum.Models.Layout
{
    internal class Image : BaseElement
    {
        public string FileName { get; set; }
        public bool Stretched { get; set; }

        public Image() 
        {
            FileName = "";
        }

        internal void FromXml(XmlImage xmlImage, AlbumStyles styles)
        {
            //ImageStyle? style = StyleFactory.FindStyle<ImageStyle>(xmlImage.Style, styles.ImageStyles);
            //if (style == null)
            //{
            //    throw new InvalidOperationException(
            //        $"Row style '{xmlImage.Style ?? "(default)"}' not found. " +
            //        $"Ensure a matching RowStyle exists in the album styles or that a default RowStyle is defined.");
            //}
            //base.FromXml(xmlImage, style);
            //H = XUnit.FromMillimeter(xmlImage.Height != 0 ? xmlImage.Height : style.Height > 0 ? style.Height : H.Millimeter);
            //W = XUnit.FromMillimeter(xmlImage.Width != 0 ? xmlImage.Width : style.Width > 0 ? style.Width : W.Millimeter);
            //Stretched = xmlImage.Stretched;

            //WidthPercent = 100;
            //if (xmlImage.Width != null)
            //{
            //    string width = xmlImage.Width;
            //    if (width.Contains('%'))
            //        WidthPercent = double.Parse(width.TrimEnd(new char[] { '%', ' ' }));
            //    else
            //        W = ParseXUnit(width);
            //}
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
