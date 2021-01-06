using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PdfSharp.Drawing;

namespace MyAlbum2
{
    public class Text : BasicType
    {

        #region Fields
        private string[] arr;
        string[] sep = { "\n" };
        private XPen UsePen = XPens.Green;
        #endregion

        #region Properties
        public bool Justified{ get; set; }
        
        private string _value;
        public string Value
        {
            get { return _value; }
            set 
            { 
                _value = value;
                needsProcess = true;
            }
        }
        
        private XFont _font;
        public XFont Font
        {
            get 
            {
                if (_font == null) { _font = new XFont("Calibri", 8, XFontStyle.Regular); }
                return _font;
            }
            set 
            { 
                _font = value; 
                needsProcess = true; 
            }
        }
        
        private XBrush _brush;
        public XBrush Brush
        {
            get
            {
                if (_brush == null) { _brush = XBrushes.Black; }
                return _brush;
            }
            set { _brush = value; }
        }

        private Alignments _alignment;
        public new Alignments Alignment
        {
            get 
            {
                if (_alignment == 0) { _alignment = Alignments.TopLeft;  }
                return _alignment; 
            }
            set 
            { 
                _alignment = value; 
                
                Format = new XStringFormat();
                switch (Alignment)
                {
                    case Alignments.TopLeft:
                        Format.Alignment = XStringAlignment.Near;
                        Format.LineAlignment = XLineAlignment.Near;
                        break;
                    case Alignments.TopCenter:
                        Format.Alignment = XStringAlignment.Center;
                        Format.LineAlignment = XLineAlignment.Near;
                        break;
                    case Alignments.TopRight:
                        Format.Alignment = XStringAlignment.Far;
                        Format.LineAlignment = XLineAlignment.Near;
                        break;
                    /*case Alignments.TopJustified:
                        Format.Alignment = XStringAlignment.Near;
                        Format.LineAlignment = XLineAlignment.Near;
                        break;
                    */
                    case Alignments.CenterLeft:
                        Format.Alignment = XStringAlignment.Near;
                        Format.LineAlignment = XLineAlignment.Center;
                        break;
                    case Alignments.CenterCenter:
                        Format.Alignment = XStringAlignment.Center;
                        Format.LineAlignment = XLineAlignment.Center;
                        break;
                    case Alignments.CenterRight:
                        Format.Alignment = XStringAlignment.Far;
                        Format.LineAlignment = XLineAlignment.Center;
                        break;
                    /*case Alignments.CenterJustified:
                        Format.Alignment = XStringAlignment.Near;
                        Format.LineAlignment = XLineAlignment.Center;
                        break;
                    */
                    case Alignments.BottomLeft:
                        Format.Alignment = XStringAlignment.Near;
                        Format.LineAlignment = XLineAlignment.Far;
                        break;
                    case Alignments.BottomCenter:
                        Format.Alignment = XStringAlignment.Center;
                        Format.LineAlignment = XLineAlignment.Far;
                        break;
                    case Alignments.BottomRight:
                        Format.Alignment = XStringAlignment.Far;
                        Format.LineAlignment = XLineAlignment.Far;
                        break;
                    /*case Alignments.BottomJustified:
                        Format.Alignment = XStringAlignment.Near;
                        Format.LineAlignment = XLineAlignment.Far;
                        break;
                    */
                    default:
                        break;

                }            
            }
        }
        
        private XStringFormat _format;
        public XStringFormat Format
        {
            get 
            {
                if (_format == null) { _format = new XStringFormat(); }
                return _format; 
            }
            set { _format = value; }
        }

        public XUnit TextHeight
        {
            get
            {
                if (needsProcess)
                    this.Split(this.Value);
                double result = 0;
                for (int i = 0; i < arr.Length; i++)
                    result += this.Font.Height;
                return XUnit.FromPoint(result);
            }
        }
        public XUnit TextWidth
        {
            get
            {
                if (needsProcess)
                    this.Split(this.Value);
                double result = 0;
                for (int i = 0; i < arr.Length; i++)
                    result = Math.Max(result, gfx.MeasureString(arr[i], Font).Width);
                return XUnit.FromPoint(result);
            }
        }
        #endregion

        #region Constructors
        public Text(XGraphics gfx) : base(gfx)
        { }
        public Text(XGraphics gfx, string value) : base(gfx)
        {
            Value = value;
        }
        public Text(XGraphics gfx, string value, double x, double y, double w, double h)
            : base(gfx, x, y, w, h)
        {
            Value = value;
            this.Split(this.Value);
        }
        public Text(XGraphics gfx, string value, double x, double y, double w, double h, XFont font, XBrush brush, Alignments align, bool canGrow)
            : base(gfx, x, y, w, h,align, canGrow)
        {
            Value = value;
            Font = font;
            Brush = brush;
            this.Split(this.Value);
        }
        #endregion

        #region Methods
        private string[] Split(string value)
        {
            needsProcess = false;
            string[] result = { };

            string[] arr = Value.Split(sep, StringSplitOptions.RemoveEmptyEntries);
            string[] words;
            string line;
            for (int i = 0; i < arr.Length; i++)
            {
                words = arr[i].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                line = words[0];
                for (int j = 1; j < words.Length; j++)
                {
                    if (gfx.MeasureString(line + " " + words[j], Font).Width < this.W)
                    {
                        line += (" " + words[j]);
                    }
                    else
                    {
                        Array.Resize(ref result, result.Length + 1);
                        result.SetValue(line, result.Length - 1);
                        line = words[j];
                    }
                }
                Array.Resize(ref result, result.Length + 1);
                result.SetValue(line, result.Length - 1);
            }

            this.arr = result;

            if (CanGrow)
            {
                this.H = XUnit.FromPoint(this.TextHeight);
            }
            else 
            {
                while (this.TextHeight >= this.H)
                {
                    Array.Resize(ref result, result.Length - 1);
                    this.arr = result;
                }
            }

            return result;
        }
        private XPoint LineStartPoint(int index, bool isJustified = false)
        {
            XPoint point = new XPoint();
            switch (Alignment)
            {
                case Alignments.TopLeft:
                    point = new XPoint(this.X,
                                        this.Y + index * this.Font.Height);
                    break;
                case Alignments.TopCenter:
                    point = new XPoint(this.X + this.W / 2,
                                        this.Y + index * this.Font.Height);
                    break;
                case Alignments.TopRight:
                    point = new XPoint(this.X + this.W,
                                        this.Y + index * this.Font.Height);
                    break;
                /*case Alignments.TopJustified:
                    point = new XPoint(this.X,
                                        this.Y + index * this.Font.Height);
                    break;
                */
                case Alignments.CenterLeft:
                    point = new XPoint(this.X,
                                        this.Y + (this.H - this.TextHeight) / 2 + (index + 0.5) * this.Font.Height);
                    break;
                case Alignments.CenterCenter:
                    point = new XPoint(this.X + this.W / 2,
                                        this.Y + (this.H - this.TextHeight) / 2 + (index + 0.5) * this.Font.Height);
                    break;
                case Alignments.CenterRight:
                    point = new XPoint(this.X + this.W,
                                        this.Y + (this.H - this.TextHeight) / 2 + (index + 0.5) * this.Font.Height);
                    break;
                /*case Alignments.CenterJustified:
                    point = new XPoint(this.X,
                                        this.Y + (this.H - this.TextHeight) / 2 + (index + 0.5) * this.Font.Height);
                    break;
                */
                case Alignments.BottomLeft:
                    point = new XPoint(this.X,
                                        this.Y + this.H - TextHeight + (index + 1) * this.Font.Height);
                    break;
                case Alignments.BottomCenter:
                    point = new XPoint(this.X + this.W / 2,
                                        this.Y + this.H - TextHeight + (index + 1) * this.Font.Height);
                    break;
                case Alignments.BottomRight:
                    point = new XPoint(this.X + this.W,
                                        this.Y + this.H - TextHeight + (index + 1) * this.Font.Height);
                    break;
                /*case Alignments.BottomJustified:
                    point = new XPoint(this.X, 
                                        this.Y + this.H - TextHeight + (index + 1) * this.Font.Height);
                    break;
                */
                default:
                    break;

            }
            if (isJustified) { point.X = this.X; }

            //DrawCross(gfx, point);
            return point;
        }
        public override void Draw()
        {

            arr = Split(Value);
            XPoint point;

            for (int i = 0; i < arr.Length; i++)
            {
                //if ((Alignment == Alignments.TopJustified || Alignment == Alignments.CenterJustified || Alignment == Alignments.BottomJustified) && (i < arr.Length - 1))
                if ((this.Justified) && (i < arr.Length - 1))
                {
                    point = LineStartPoint(i, true);
                    string[] words = arr[i].Split();
                    double wordsWidth = 0;
                    double spaceWidth = 0;
                    double space;
                    for (int j = 0; j < words.Length; j++)
                    {
                        wordsWidth += gfx.MeasureString(words[j], Font).Width;
                    }
                    spaceWidth = this.W.Point - wordsWidth;
                    space = spaceWidth / (words.Length - 1);
                    for (int j = 0; j < words.Length; j++)
                    {
                        //gfx.DrawString(words[j], Font, Brush, point, Format);
                        gfx.DrawString(words[j], Font, Brush, point, XStringFormats.TopLeft);
                        point.X += gfx.MeasureString(words[j], Font).Width + space;
                    }

                }
                else
                {
                    point = LineStartPoint(i, false);
                    gfx.DrawString(arr[i], Font, Brush, point, Format);
                    //DrawCross(gfx, point);
                }

            }

            if (HasBorder) 
                gfx.DrawRectangle(XPens.Black, X.Point, Y.Point, W.Point, H.Point);
        }
        #endregion    
    }

    
    
    public class TextElement : AlbumElement
    {
        #region Fields
        private Text innerText { get; set; }
        public new Text Body { get { return innerText; } }
        #endregion

        #region Properties
        public override XUnit X
        {
            get { return innerText.X; }
            set { innerText.X = value; }
        }
        public override XUnit Y
        {
            get { return innerText.Y; }
            set { innerText.Y = value; }
        }
        public override XUnit H
        {
            get { return innerText.H; }
            set { innerText.H = value; }
        }
        public override XUnit W
        {
            get { return innerText.W; }
            set { innerText.W = value; }
        }

        public bool Justified
        {
            get { return innerText.Justified; }
            set { innerText.Justified = value; }
        }

        public string Value
        {
            get { return innerText.Value; }
            set { innerText.Value = value; }
        }

        public XFont Font
        {
            get { return innerText.Font; }
            set { innerText.Font = value; }
        }

        public XBrush Brush
        {
            get { return innerText.Brush; }
            set { innerText.Brush = value; }
        }

        public new Alignments Alignment
        {
            get { return innerText.Alignment; }
            set { innerText.Alignment = value; }
        }

        public XStringFormat Format
        {
            get { return innerText.Format; }
            set { innerText.Format = value; }
        }

        public XUnit TextHeight
        {
            get { return innerText.TextHeight; }
        }
        public XUnit TextWidth
        {
            get { return innerText.TextWidth; }
        }
        #endregion

        #region Constructors
        public TextElement(XGraphics gfx) : base(gfx)
        {
            //innerText = new Text(gfx);
            this.VSpace = XUnit.Zero;
        }
        public TextElement(XGraphics gfx, string value) : base(gfx)
        {
            //innerText = new Text(gfx, value);
            this.VSpace = XUnit.Zero;
        }
        public TextElement(XGraphics gfx, string value, double x, double y, double w, double h)
            : base(gfx, x, y, w, h)
        {
            //innerText = new Text(gfx, value, x, y, w, h);
            this.VSpace = XUnit.Zero;
        }
        public TextElement(XGraphics gfx, string value, double x, double y, double w, double h, XFont font, XBrush brush, Alignments align, bool canGrow)
            : base(gfx/*, x, y, w, h, align, canGrow*/)
        {
            innerText = new Text(gfx, value, x, y, w, h,font, brush, align, canGrow);
            innerText.Alignment = align;
            this.VSpace = XUnit.Zero;
        }
        #endregion

        #region Methods
        public override void Draw()
        {
            innerText.HasBorder = this.HasBorder;
            innerText.CanGrow = this.CanGrow;
            //Text text = new Text(this.gfx, this.Value, this.X.Millimeter, this.Y.Millimeter, this.W.Millimeter, this.H.Millimeter, this.Font, XBrushes.Red, this.Alignment, this.CanGrow);
            //text.HasBorder = this.HasBorder;
            //text.Justified = this.Justified;
            //text.Draw();
            innerText.Draw();

            if (HasBorder)
                gfx.DrawRectangle(XPens.Black, X.Point, Y.Point, W.Point, H.Point);

        }
        #endregion

    }
}
