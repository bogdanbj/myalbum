using System;
using System.Collections.Generic;
using System.Text;

using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

namespace MyAlbum
{
    class AlbumPage
    {
        private PdfPage _page;
        private bool _isMirror = false;
        private XUnit _marginTop, _marginRight, _marginBottom, _marginLeft;
        private XUnit _indentLeft, _indentRight, _vSpace;
        private XPen _pen = new XPen(XColors.Black);
        private double _xPos, _yPos;
        private XGraphics _gfx;
        private StampRow _row;

        public PageSize Size
        {
            get { return _page.Size; }
            set { _page.Size = value; }
        }
        public XGraphics gfx
        {
            get {
                if (_gfx == null)
                    _gfx = XGraphics.FromPdfPage(_page);
                return _gfx; 
            }
        }
        public PageOrientation Orientation
        {
            get { return _page.Orientation; }
            set { _page.Orientation = value; }
        }
        public bool IsMirror
        {
            get { return _isMirror; }
            set { _isMirror = value; }
        }
        public XUnit MarginTop
        {
            get { return _marginTop; }
            set { _marginTop = value; }
        }
        public XUnit MarginRight
        {
            get { return _marginRight; }
            set { _marginRight = value; }
        }
        public XUnit MarginBottom
        {
            get { return _marginBottom; }
            set { _marginBottom = value; }
        }
        public XUnit MarginLeft
        {
            get { return _marginLeft; }
            set { _marginLeft = value; }
        }
        public XUnit Width
        {
            get { return _page.Width - _marginLeft - _marginRight; }
        }
        public XUnit Height
        {
            get { return _page.Height - _marginTop - _marginBottom; }
        }
        public XUnit IndentLeft
        {
            get { return _indentLeft; }
            set { _indentLeft = value; }
        }
        public XUnit IndentRight
        {
            get { return _indentRight; }
            set { _indentRight = value; }
        }
        public XUnit VSpace
        {
            get { return _vSpace; }
            set { _vSpace = value; }
        }
        public double XLeft
        {
            get { return MarginLeft.Point + IndentLeft.Point; }
        }
        public double XRight
        {
            get { return _page.Width.Point - MarginRight.Point - IndentRight.Point; }
        }
        public double XCenter
        {
            get { return (this.XLeft + this.XRight) / 2; }
        }
        public double YTop
        {
            get { return MarginTop.Point + VSpace.Point; }
        }
        public double YBottom
        {
            get { return _page.Height.Point - MarginBottom.Point - VSpace.Point; }
        }
        public double YCenter
        {
            get { return (this.YTop + this.YBottom) / 2; }
        }
        public double XPos
        {
            get { return _xPos; }
            set { _xPos = value; }
        }
        public double YPos
        {
            get { return _yPos; }
            set { _yPos = value; }
        }


        public AlbumPage(PdfPage page)
        {
            _page = page;
            _marginTop = _marginRight = _marginBottom = _marginLeft = XUnit.FromMillimeter(5);
        }

        public void DrawBorder(double borderWidth1, double borderWidth2, double borderSpace)
        {
            double offset;

            if (borderWidth1 != 0)
            {
                _pen.Width = borderWidth1;
                gfx.DrawRectangle(_pen, MarginLeft.Point, MarginTop.Point, this.Width.Point, this.Height.Point);

                offset = borderWidth1;

                if (borderWidth2 != 0)
                {
                    offset += borderSpace;
                    _pen.Width = borderWidth2;
                    gfx.DrawRectangle(_pen, MarginLeft.Point + offset, MarginTop + offset, this.Width.Point - 2 * offset, this.Height.Point - 2 * offset);
                }
            }
        }

        public void DrawString(string text, XFont font, XStringFormat stringFormat)
        {
            string line = "";
            string[] words = text.Split(' ');
            double len = 0;

            XPos = (stringFormat.Alignment == XStringAlignment.Near ? XLeft : XCenter);

            for (int i = 0; i < words.Length; i++)
            {
                if (len + gfx.MeasureString(words[i] + ' ', font).Width < XRight - XLeft)
                {
                    line += words[i] + ' ';
                    len += gfx.MeasureString(words[i] + ' ', font).Width;
                }
                else
                {
                    gfx.DrawString(line, font, XBrushes.Black, new XPoint(XPos, YPos), stringFormat);
                    YPos += gfx.MeasureString(line, font).Height;
                    line = words[i] + ' ';
                    len = gfx.MeasureString(words[i] + ' ', font).Width;
                }
            }
            gfx.DrawString(line, font, XBrushes.Black, new XPoint(XPos, YPos), stringFormat);
            YPos += gfx.MeasureString(line, font).Height + VSpace.Point;
        }

        public void NewStampsRow(string[] args)
        {
            try
            {
                _row = new StampRow(_gfx, args[0].Trim(), args[1].Trim(), Convert.ToDouble(args[2]), Convert.ToDouble(args[3]),
                                    Convert.ToDouble(args[4]), Convert.ToDouble(args[5]), Convert.ToDouble(args[6]),
                                    GetFont(string.Join(", ", new string[] { args[7], args[8], args[9] })),
                                    GetFont(string.Join(", ", new string[] { args[10], args[11], args[12] })),
                                    YPos, XLeft, XRight);
            }
            catch (Exception ex)
            {
                throw new Exception("Invalid parameter", ex);
            }
        }

        public void SetFrameType(string arguments)
        {
            Int16 type;
            if (Int16.TryParse(arguments, out type))
                _row.FrameType = type;
            else
                _row.FrameType = 0;
        }
        
        public void AddStamp(string arguments)
        {
            string[] args = Program.ParseArguments(arguments);
            _row.AddStamp(args);
        }

        public void AddSpace(string arguments)
        {
            _row.AddSpace(arguments);
        }

        public void DrawStampsRow()
        {
            // Draw the stamp row
            //_gfx.DrawLine(new XPen(XColors.Red, 1), XLeft, YPos, XRight, YPos);
            _row.Draw(_gfx);

            // Advance the Y position
            YPos += _row.TotalHeight + VSpace.Point;
        }

        public void DrawLine(string arguments)
        {
            double indent;
            if (arguments.Contains("%"))
            {
                double d = Convert.ToDouble(arguments.Replace("%", ""));
                indent = (XRight - XLeft) * (1 - d / 100) / 2;
            }
            else
            {
                double d = XUnit.FromMillimeter(Convert.ToDouble(arguments)).Point;
                indent = (XRight - XLeft - d) / 2;
            }
            _gfx.DrawLine(new XPen(XColors.Black, 1), XLeft + indent, YPos, XRight - indent, YPos);
        }
        
        public static XFont GetFont(string arguments)
        {
            XPdfFontOptions options = new XPdfFontOptions(PdfFontEncoding.Unicode, PdfFontEmbedding.Always);

            XFont font = new XFont("Arial", 12, XFontStyle.Regular, options);

            string[] args = arguments.Split(',');

            if (args.Length != 3) { throw new Exception("Incorrect number of parameters"); }

            try
            {
                XFontStyle fontStyle;

                fontStyle = XFontStyle.Regular;

                if (args[2].ToUpper().IndexOf("BOLD") >= 0)
                {
                    fontStyle = XFontStyle.Bold;
                }
                if (args[2].ToUpper().IndexOf("ITALIC") >= 0)
                {
                    fontStyle = fontStyle | XFontStyle.Italic;
                }
                if (args[2].ToUpper().IndexOf("UNDERLINE") >= 0)
                {
                    fontStyle = fontStyle | XFontStyle.Underline;
                }
                if (args[2].ToUpper().IndexOf("STRIKEOUT") >= 0)
                {
                    fontStyle = fontStyle | XFontStyle.Strikeout;
                }

                font = new XFont(args[0].Trim(), Convert.ToDouble(args[1].Trim()), fontStyle, options);
            }
            catch (Exception)
            {
                throw new Exception(String.Format("GetFont({0}): Invalid parameter", arguments));
            }

            return font;
        }

        private void test()
        {
            //_page.Width;
        }
    }
}
