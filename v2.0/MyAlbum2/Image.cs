using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PdfSharp.Drawing;

namespace MyAlbum2
{
    class Image : BasicType
    {
        #region Fields
        private XImage img;
        #endregion

        #region Properties
        private string _fileName;
        public string FileName
        {
            get { return _fileName; }
            set
            {
                if (XImage.ExistsFile(value))
                {
                    _fileName = value;
                    img = XImage.FromFile(_fileName);
                    needsProcess = true;
                }
            }
        }
        public override XUnit H
        { 
            get 
            {
                if (needsProcess) Calculate();
                return base.H;
            } 
            set 
            {
                base.H = value;
                needsProcess = true;
            } 
        }

        #endregion

        #region Constructors
        public Image(XGraphics gfx, string fileName) : base(gfx)
        {
            FileName = fileName;
            Alignment = Alignments.TopLeft;
        }
        public Image(XGraphics gfx, string fileName, double x, double y, double w, double h)
            : base(gfx, x, y, w, h)
        {
            FileName = fileName;
            Alignment = Alignments.TopLeft;
            needsProcess = true;
        }
        public Image(XGraphics gfx, string fileName, double x, double y, double w, double h, Alignments align)
            : base(gfx, x, y, w, h)
        {
            FileName = fileName;
            Alignment = align;
            needsProcess = true;
        }
        #endregion

        #region Methods
        private void Calculate()
        {
            needsProcess = false;

            double wRatio = base.W / img.PointWidth;
            double hRatio = base.H / img.PointHeight;

            base.W = img.PointWidth * (wRatio < hRatio ? wRatio : hRatio);
            base.H = img.PointHeight * (wRatio < hRatio ? wRatio : hRatio);
        }
        public override void Draw()
        {

            
            if (needsProcess) Calculate();

            switch (Alignment)
            {
                case Alignments.TopLeft:
                    gfx.DrawImage(img, X, Y, W, H);
                    break;
                case Alignments.TopCenter:
                /*case Alignments.TopJustified:*/
                    gfx.DrawImage(img, X - W / 2, Y, W, H);
                    break;
                case Alignments.TopRight:
                    gfx.DrawImage(img, X - W, Y, W, H);
                    break;
                case Alignments.CenterLeft:
                    gfx.DrawImage(img, X, Y - H / 2, W, H);
                    break;
                case Alignments.CenterCenter:
                /*case Alignments.CenterJustified:*/
                    gfx.DrawImage(img, X - W / 2, Y - H / 2, W, H);
                    break;
                case Alignments.CenterRight:
                    gfx.DrawImage(img, X - W, Y - H / 2, W, H);
                    break;
                case Alignments.BottomLeft:
                    gfx.DrawImage(img, X, Y - H, W, H);
                    break;
                case Alignments.BottomCenter:
                /*case Alignments.BottomJustified:*/
                    gfx.DrawImage(img, X - W / 2, Y - H, W, H);
                    break;
                case Alignments.BottomRight:
                    gfx.DrawImage(img, X - W, Y - H, W, H);
                    break;
                default:
                    break;
            }

            if (HasBorder)
                gfx.DrawRectangle(Album.pen, X.Point, Y.Point, W.Point, H.Point);

        }
        #endregion
    }
}
