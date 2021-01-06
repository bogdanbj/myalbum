using System;
using System.Collections.Generic;
using System.Text;

using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

namespace MyAlbum
{
    class StampRow
    {
        public enum HAligns
        {
            FS, ES, JS
        }
        public enum VAligns
        {
            TOP, BOTTOM, CENTRE
        }

        private XGraphics _gfx;
        private VAligns _vAlign;
        private HAligns _hAlign;
        private double _hSpace, _vSpace;
        private double _borderWidth1, _borderWidth2, _borderSpace;
        private XFont _titleFont, _textFont;
        //private XStringFormat _titleStyle, _textStyle;
        private List<Stamp> _stamps;
        //private double /*_xPos,*/ _indentLeft, _indentRight;

        //private double _yTop, _yCenter, _yBottom;
        private double _xLeft, _xRight;
        private double _yPos;
        private double _totalHeight;
        private Int16 _frameType;

        public List<Stamp> Stamps
        {
            get 
            {
                if (_stamps == null) { _stamps = new List<Stamp>(); }
                return _stamps; 
            }
        }

        public HAligns HAlign
        {
            get { return _hAlign; }
        }
        public VAligns VAlign
        {
            get { return _vAlign; }
        }
        public double HSpace
        {
            get { return _hSpace; }
            set { _hSpace = value; }
        }
        public double VSpace
        {
            get { return _vSpace; }
            set { _vSpace = value; }
        }
        public double XLeft
        {
            get { return _xLeft; }
        }
        public double XRight
        {
            get { return _xRight; }
        }
        public double YPos
        {
            get { return _yPos; }
        }
        public double TotalHeight
        { 
            get { return _totalHeight; } 
        }
        public Int16 FrameType
        {
            get { return _frameType; }
            set { _frameType = value; }
        }



        public StampRow(XGraphics gfx, string hAlign, string vAlign, 
                        double hSpace, double vSpace, 
                        double borderWidth1, double borderWidth2, double borderSpace,
                        XFont titleFont, XFont textFont,
                        double ypos, double xLeft, double xRight)
        {
            try
            {
                _gfx = gfx;

                #region switch (hAlign)
                switch (hAlign.Trim().Substring(0, 2))
                {
                    case "FS":
                        _hAlign = HAligns.FS;
                        break;
                    case "ES":
                        _hAlign = HAligns.ES;
                        break;
                    case "JS":
                        _hAlign = HAligns.JS;
                        break;
                    default:
                        throw new Exception("Invalid hAlign parameter. Valid values are FS, ES, JS.");
                        break;
                }
                #endregion 

                #region switch(vAlign)
                switch (vAlign.Trim().Substring(0, 3))
                {
                    case "TOP":
                        _vAlign = VAligns.TOP;
                        break;
                    case "BOT":
                        _vAlign = VAligns.BOTTOM;
                        break;
                    case "CEN":
                        _vAlign = VAligns.CENTRE;
                        break;
                    default:
                        throw new Exception("Invalid vAlign parameter. Valid values are TOP, BOT(tom), CEN(tre).");
                        break;
                }
                #endregion

                _hSpace = XUnit.FromMillimeter(hSpace).Point;
                _vSpace = XUnit.FromMillimeter(vSpace).Point;
                _titleFont = titleFont;     
                _textFont = textFont;
                _yPos = ypos;
                _xLeft = xLeft;
                _xRight = xRight;
                _borderWidth1 = XUnit.FromMillimeter(borderWidth1);
                _borderWidth2 = XUnit.FromMillimeter(borderWidth2);
                _borderSpace = XUnit.FromMillimeter(borderSpace);
            }
            catch 
            {
                throw;
            }
        }

       
        public void AddStamp(string[] args)
        {
            Stamp mystamp = new Stamp(_gfx, /*0, 0,*/ _titleFont, args[0],
                                XUnit.FromMillimeter(Convert.ToDouble(args[1])).Point, XUnit.FromMillimeter(Convert.ToDouble(args[2])).Point,
                                _borderWidth1,_borderWidth2, _borderSpace, _frameType,
                                XUnit.FromMillimeter(Convert.ToDouble(args[3])).Point, _vSpace,
                                args[4], _textFont, 
                                args[5], args[6], args[7], args[8], args[9], args[10]);

            Stamps.Add(mystamp);
        }

        public void AddSpace(string arguments)
        {
            Stamp mystamp = new Stamp(_gfx, _titleFont, "",
                                XUnit.FromMillimeter(Convert.ToDouble(arguments)).Point, XUnit.Zero.Point,
                                XUnit.Zero.Point, XUnit.Zero.Point, XUnit.Zero.Point, 0,
                                XUnit.Zero.Point, _vSpace,
                                "", _textFont,
                                "", "", "", "", "", "");
            Stamps.Add(mystamp);
        }

        public void Draw(XGraphics gfx)
        {
            double rowWidth = 0;
            _totalHeight = GetHeight();

            #region Horizontal Alignment
            switch (HAlign)
            {
                #region HAligns.ES
                case StampRow.HAligns.ES:
                    foreach (Stamp stamp in Stamps)
                    {
                        rowWidth += stamp.FrameWidth;
                    }

                    HSpace = (XRight - XLeft - rowWidth) / (Stamps.Count + 1);

                    double XPos = XLeft + HSpace;

                    foreach (Stamp stamp in Stamps)
                    {
                        stamp.XPos = XPos;
                        XPos += stamp.FrameWidth + HSpace;
                    }
                    break;
                #endregion
                #region HAligns.FS
                case StampRow.HAligns.FS:
                    foreach (Stamp stamp in Stamps)
                    {
                        rowWidth += stamp.FrameWidth;
                    }

                    rowWidth += (Stamps.Count - 1) * HSpace;

                    XPos = (XLeft + XRight - rowWidth) / 2;

                    foreach (Stamp stamp in Stamps)
                    {
                        stamp.XPos = XPos;
                        XPos += stamp.FrameWidth + HSpace;
                    }
                    break;
                #endregion
                #region HAligns.JS
                case StampRow.HAligns.JS:
                    if (Stamps.Count == 1)
                    {
                        Stamps[0].XPos = (XRight + XLeft - Stamps[0].FrameWidth) / 2;
                    }
                    else
                    {
                        foreach (Stamp stamp in Stamps)
                        {
                            rowWidth += stamp.FrameWidth;
                        }

                        HSpace = (XRight - XLeft - rowWidth) / (Stamps.Count - 1);

                        XPos = XLeft;

                        foreach (Stamp stamp in Stamps)
                        {
                            stamp.XPos = XPos;
                            XPos += stamp.FrameWidth + HSpace;
                        }
                    }
                    break;
                #endregion

            }
            #endregion

            #region Vertical Alignment
            double max = 0;
            switch (VAlign)
            {
                #region VAligns.TOP
                case StampRow.VAligns.TOP:
                    // get max title height
                    foreach (Stamp stamp in Stamps)
                    {
                        double h = stamp.TitleHeight + stamp.VSpace;
                        max = (h > max ? h : max);
                    }

                    foreach (Stamp stamp in Stamps)
                    {
                        stamp.YPos = this.YPos + max;
                    }
                    break;
                #endregion
                #region VAligns.BOTTOM
                case StampRow.VAligns.BOTTOM:
                    foreach (Stamp stamp in Stamps)
                    {
                        double h = stamp.TitleHeight + stamp.VSpace + stamp.FrameHeight;
                        max = (h > max ? h : max);
                    }

                    foreach (Stamp stamp in Stamps)
                    {
                        stamp.YPos = this.YPos + max - stamp.FrameHeight;
                    }
                    break;
                #endregion
                #region VAligns.CENTRE
                case StampRow.VAligns.CENTRE:
                    foreach (Stamp stamp in Stamps)
                    {
                        double h = stamp.TitleHeight + stamp.VSpace + stamp.FrameHeight / 2;
                        max = (h > max ? h : max);
                    }

                    foreach (Stamp stamp in Stamps)
                    {
                        stamp.YPos = this.YPos + max - stamp.FrameHeight / 2;
                    }

                    break;
                #endregion
            }
            #endregion

            foreach (Stamp stamp in Stamps)
            {
                stamp.Draw(gfx);
            }
        }

        private double GetHeight()
        {
            double max1st = 0;
            double max2nd = 0;
            double max = 0;

            switch (this.VAlign)
            {
                case StampRow.VAligns.TOP:
                    //for (int i = 0; i < Stamps.Count; i++)
                    foreach (Stamp stamp in this.Stamps)
                    {
                        double h = stamp.TitleHeight + stamp.VSpace;
                        max1st = (h > max1st ? h : max1st);
                        max2nd = (stamp.TotalHeight - h > max2nd ? stamp.TotalHeight - h : max2nd);
                    }
                    break;
                case StampRow.VAligns.BOTTOM:
                    foreach (Stamp stamp in this.Stamps)
                    {
                        max1st = (stamp.YBottom > max1st ? stamp.YBottom : max1st);
                        max2nd = (stamp.TotalHeight - stamp.YBottom > max2nd ? stamp.TotalHeight - stamp.YBottom : max2nd);
                    }
                    break;
                case StampRow.VAligns.CENTRE:
                    foreach (Stamp stamp in this.Stamps)
                    {
                        max1st = (stamp.YCenter > max1st ? stamp.YCenter : max1st);
                        max2nd = (stamp.TotalHeight - stamp.YCenter > max2nd ? stamp.TotalHeight - stamp.YCenter : max2nd);
                    }
                    break;
            }

            max = max1st + max2nd;
            return max;
        }
    }
}
