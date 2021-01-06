using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//using PdfSharp;
using PdfSharp.Drawing;




namespace MyAlbum2 
{



    class Stamp : AlbumElement
    {
        #region Properties
        public BorderPlain Border { get; set; }
        public new BorderPlain Body { get { return Border; } }
        public Image Img { get; set; }
        public Text BodyNote { get; set; }
        public XBrush Brush
        {
            get { return Album.brush; }
        }

        public override XUnit X
        {
            get
            {   
                return base.X - Padding;
            }
            set
            {
                base.X = value + Padding;
            }
        }
        public override XUnit Y
        {
            get
            {
                return base.Y - Padding;
            }
            set
            {
                base.Y = value + Padding;
            }
        }
        public override XUnit W
        {
            get
            {
                return base.W + 2 * Padding;
            }
            set
            {
                base.W = value - 2 * Padding;
            }
        }
        public override XUnit H
        {
            get
            {
                return base.H + 2 * Padding;
            }
            set
            {
                base.H = value - 2 * Padding;
            }
        }

        public XUnit TotalHeight
        {
            get 
            {
                return (Title.Value.Length > 0 ? (XUnit)(Title.TextHeight + VSpace) : XUnit.Zero) + Border.H + VSpace + (FootNotes.Count > 0 ? (XUnit)(this.FootNotes.TextHeight() + VSpace) : XUnit.Zero); 
            }

            //set { base.H = value; }
        }

        public override Text Title
        {
            get
            {
                return base.Title;
            }
            set
            {
                base.Title = value;
                base.Title.Alignment = Alignments.BottomCenter;
            }
        }
                

        //public double FrameOffset { get; set; }
        //public double ImgOffset { get; set; }


        #endregion

        #region Constructors
        //public Stamp(double w, double h)
        //{
        //    Width = w;
        //    Height = h;
        //}
        public Stamp(XGraphics gfx,
                    double x, double y, double w, double h)
            : base(gfx, x, y, w, h)
        { }
        
        /*public Stamp(XGraphics gfx, 
                    double x, double y, double w, double h, 
                    double padding, double vSpace,
                    string title, double titlePadding, XFont titleFont,
                    string text, XFont textFont, 
                    string footLeft, string footCenter, string footRight, XFont footFont,
                    string imageFileName,
                    BorderBase border)
             //XFont titleFont, string title,
             //double stampWidth, double stampHeight,
             //double stampBorderWidth1, double stampBorderWidth2, double stampBorderSpace, Int16 frameType,
             //double padding, double vSpace,
             //string image,
             //XFont textFont, string text1, string text2, string text3,
             //string footnote1, string footnote2, string footnote3
            : base(gfx, x, y, w + 2 * padding, h)
        {
            // Width
            this.Padding = XUnit.FromMillimeter(padding);
            //this.W += 2 * Padding;
            //this.H += 2 * padding;
            this.VSpace = XUnit.FromMillimeter(vSpace);

            // Title
            Title = new Text(gfx, title, 0, 0, this.W.Millimeter, vSpace, titleFont, this.Brush, Alignments.BottomCenter, true);
            
            // Border
            Border = border;
            Border.W = XUnit.FromMillimeter(w + 2 * padding);
            Border.H = XUnit.FromMillimeter(h + 2 * padding);

            // BodyNote
            BodyNote = new Text(gfx, text, 0, 0, w, h, textFont, this.Brush, Alignments.CenterCenter, false);

            //Image
            if (XImage.ExistsFile(imageFileName))
            {
                Img = new Image(gfx, imageFileName, 0, 0, w - 2 * ImgOffset.Millimeter, h - 2 * ImgOffset.Millimeter, Alignments.CenterCenter);
            }

            //Inner Text
            BodyNote = new Text(gfx, text, 0, 0, w - 2 * ImgOffset.Millimeter, h - 2 * ImgOffset.Millimeter, textFont, this.Brush, Alignments.CenterCenter, false);            
            
            // FootNote
            FootNotes = new List<Text>();
            if (footLeft.Length > 0)
                FootNotes.Add(new Text(gfx, footLeft, 0, 0, Border.W.Millimeter, this.VSpace, footFont, this.Brush, Alignments.TopLeft, true));
            if (footCenter.Length > 0)
                FootNotes.Add(new Text(gfx, footCenter, 0, 0, Border.W.Millimeter, this.VSpace, footFont, this.Brush, Alignments.TopCenter, true));
            if (footRight.Length > 0)
                FootNotes.Add(new Text(gfx, footRight, 0, 0, Border.W.Millimeter, this.VSpace, footFont, this.Brush, Alignments.TopRight, true));

           
            //for (int i = 0; i < FootNote.Count; i++)
            //{
            //    this.FootHeight = this.FootHeight > FootNote[i].TextHeight ? this.FootHeight : FootNote[i].TextHeight;
            //}


            //_titleFont = titleFont;
            //_title = title.Trim();
            //_stampWidth = stampWidth;
            //_stampHeight = stampHeight;
            //_borderWidth1 = stampBorderWidth1;
            //_borderWidth2 = stampBorderWidth2;
            //_borderSpace = stampBorderSpace;
            //_frameType = frameType;
            //_stampBorderWidth = stampBorderWidth1 + stampBorderSpace + stampBorderWidth2;
            _padding = padding;
            _vspace = vSpace;
            _img = image.Trim();
            _textFont = textFont;
            _text1 = text1.Trim();
            _text2 = text2.Trim();
            _text3 = text3.Trim();
            _footnote1 = footnote1.Trim();
            _footnote2 = footnote2.Trim();
            _footnote3 = footnote3.Trim();

            _frameWidth = _stampWidth + 2 * _padding;
            _frameHeight = _stampHeight + 2 * _padding;
            _scale = XUnit.FromMillimeter(2).Point;

            double xScale, yScale;
            if (_stampWidth > _stampHeight)
            {
                yScale = _scale;
                xScale = _stampWidth * yScale / _stampHeight;
            }
            else
            {
                xScale = _scale;
                yScale = _stampHeight * xScale / _stampWidth;
            }
            _imgWidth = _stampWidth - 2 * xScale;
            _imgHeight = _stampHeight - 2 * yScale;

            _yTop = TitleHeight + VSpace;
            _yCenter = TitleHeight + VSpace + FrameHeight / 2;
            _yBottom = TitleHeight + VSpace + FrameHeight;


            string[] sep = { "\\n" };
            string[] arr, arr1, arr2, arr3;
            // _titleHeight
            arr = _title.Split(sep, StringSplitOptions.RemoveEmptyEntries);
            _titleHeight = 0;
            for (int i = 0; i < arr.Length; i++)
                _titleHeight += _gfx.MeasureString(arr[i], _titleFont).Height;

            // _footerHeight
            arr1 = _footnote1.Split(sep, StringSplitOptions.RemoveEmptyEntries);
            arr2 = _footnote2.Split(sep, StringSplitOptions.RemoveEmptyEntries);
            arr3 = _footnote3.Split(sep, StringSplitOptions.RemoveEmptyEntries);
            arr = (arr1.Length > arr2.Length ? (arr1.Length > arr3.Length ? arr1 : arr3) : (arr2.Length > arr3.Length ? arr2 : arr3));
            _footerHeight = 0;
            for (int i = 0; i < arr.Length; i++)
                _footerHeight += _gfx.MeasureString(arr[i], _textFont).Height;

            // _totalHeight
            _totalHeight = (TitleHeight > 0 ? TitleHeight + VSpace : 0)
                            + FrameHeight + VSpace
                            + (FooterHeight > 0 ? FooterHeight + VSpace : 0);


        }

*/

        #endregion

        #region Methods
        private void Calculate()
        {

            Border.W = this.W + 2 * Border.Padding;
            Border.H = this.H + 2 * Border.Padding;

            Title.W = Border.W;
            Title.Alignment = Alignments.BottomCenter;

            Img.W = this.W - 2 * Img.Padding;
            Img.H = this.H - 2 * Img.Padding;
            Img.Alignment = Alignments.CenterCenter;

            for (int i = 0; i < FootNotes.Count; i++)
            {
                switch (FootNotes[i].Alignment)
                {
                    case Alignments.TopLeft:
                    case Alignments.CenterLeft:
                    case Alignments.BottomLeft:
                        FootNotes[i].W = Border.W / 2 - FootNotes[i].Padding;
                        break;
                    case Alignments.TopCenter:
                    case Alignments.CenterCenter:
                    case Alignments.BottomCenter:
                        FootNotes[i].W = Border.W - 2 * FootNotes[i].Padding;
                        break;
                    case Alignments.TopRight:
                    case Alignments.CenterRight:
                    case Alignments.BottomRight:
                        FootNotes[i].W = Border.W / 2 - FootNotes[i].Padding;
                        break;
                    default:
                        break;
                }
            }

            needsProcess = false;
        }

        public override void Draw()
        {

            if (needsProcess) Calculate();

            // Border / Body
            Border.X = this.X;
            Border.Y = this.Y;
            Border.Draw();

            
            // Title
            Title.X = this.X + Title.Padding;
            Title.Y = this.Y - this.VSpace - Title.TextHeight;
            Title.Draw();


            if (Img != null)
            {
                // Image
                Img.X = Border.X + Border.W / 2;
                Img.Y = Border.Y + Border.H / 2;
                Img.Draw();
            }
            else 
            {
                // Inner text
                BodyNote.X = this.X + this.Padding + BodyNote.Padding;
                BodyNote.Y = this.Y + this.Padding + BodyNote.Padding;
                BodyNote.Draw();
            }
            
           
            // Foot Notes
            for (int i = 0; i < FootNotes.Count; i++)
            {
                FootNotes[i].Y = this.Y + Border.H + this.VSpace;
                switch (FootNotes[i].Alignment)
                {
                    case Alignments.TopLeft:
                    case Alignments.CenterLeft:
                    case Alignments.BottomLeft:
                        FootNotes[i].X = Border.X + FootNotes[i].Padding;
                        break;
                    case Alignments.TopCenter:
                    case Alignments.CenterCenter:
                    case Alignments.BottomCenter:
                        FootNotes[i].X = Border.X + FootNotes[i].Padding;
                        break;
                    case Alignments.TopRight:
                    case Alignments.CenterRight:
                    case Alignments.BottomRight:
                        FootNotes[i].X = Border.X + Border.W / 2;
                        break;
                    default:
                        break;
                }
                FootNotes[i].Draw();
            }

            //DrawCross(gfx, new XPoint(this.X, this.Y));
        }
        #endregion

    }
}
