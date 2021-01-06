using System;
using System.Collections.Generic;
using System.Text;

using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

namespace MyAlbum
{
    class Stamp
    {
        private string _title;
        private XFont _titleFont;
        private double _borderWidth1, _borderSpace, _borderWidth2;    //, _stampBorderWidth;
        private double _stampWidth, _stampHeight;
        private double _frameWidth, _frameHeight;
        private Int16 _frameType;
        private double _imgWidth, _imgHeight;
        private double _padding, _vspace;
        private string _img;
        private string _text1, _text2, _text3, _footnote1, _footnote2, _footnote3;
        private XFont _textFont;
        private double _posx = 0, _posy = 0;
        private double _scale;
        private XGraphics _gfx;

        private double _yTop = 0, _yCenter = 0, _yBottom = 0;
        private double _totalHeight = 0;
        private double _titleHeight = 0;
        private double _footerHeight = 0;

        public double XPos
        {
            get { return _posx; }
            set { _posx = value; }
        }
        public double YPos
        {
            get { return _posy; }
            set { _posy = value; }
        }
        public double Width
        {
            get { return _stampWidth; }
        }
        public double Height
        {
            get { return _stampHeight; }
        }
        public double VSpace
        {
            get { return _vspace; }
        }
        public double FrameWidth
        {
            get { return _frameWidth; }
        }
        public double FrameHeight
        {
            get { return _frameHeight; }
        }
        public double FrameType
        {
            get { return _frameType; }
        }
        public double TitleHeight
        {
            get { return _titleHeight; }
        }
        public double FooterHeight
        {
            get { return _footerHeight; }
        }
        public double TotalHeight
        {
            get {return _totalHeight;}
            //get { return _gfx.MeasureString(_title, _titleFont).Height + VSpace + FrameHeight + VSpace + _gfx.MeasureString(string.Concat(_footnote1, _footnote2, _footnote3), _textFont).Height + VSpace; }
        }
        public double YTop
        {
            get { return _yTop; }
        }
        public double YCenter
        {
            get { return _yCenter; }
        }
        public double YBottom
        {
            get { return _yBottom; }
        }
	

        public Stamp(XGraphics gfx,
                     /*double posX, double posY,*/
                     XFont titleFont, string title,
                     double stampWidth, double stampHeight, 
                     double stampBorderWidth1, double stampBorderWidth2, double stampBorderSpace, Int16 frameType,
                     double padding, double vSpace,
                     string image,
                     XFont textFont, string text1, string text2, string text3,
                     string footnote1, string footnote2, string footnote3)
        {
            _gfx = gfx;
            //_posx = posX;
            //_posy = posY;
            _titleFont = titleFont;
            _title = title.Trim();
            _stampWidth = stampWidth;
            _stampHeight = stampHeight;
            _borderWidth1 = stampBorderWidth1;
            _borderWidth2 = stampBorderWidth2;
            _borderSpace = stampBorderSpace;
            _frameType = frameType;
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


        private void DrawGizmo1(double x, double y, double w, double h)
        {
            XPen pen;
            pen = new XPen(XColors.Black, _borderWidth1 * 1.1);

            double adj;
            adj = _borderWidth1 / 4;

            //_gfx.DrawRectangle(XBrushes.DarkOrange, x, y, w, h);

            if (w > h)
            {
                _gfx.DrawRectangle(XBrushes.White, x, y - _borderWidth1, w, h + 2 * _borderWidth1);
                _gfx.DrawLine(pen, x - adj, y + adj, x + w / 2 + adj, y + h - adj);
                _gfx.DrawLine(pen, x - adj, y + h - adj, x + w / 2 + adj, y + adj);
                _gfx.DrawLine(pen, x + w / 2 - adj, y + adj, x + w + adj, y + h - adj);
                _gfx.DrawLine(pen, x + w / 2 - adj, y + h - adj, x + w + adj, y + adj);
            }
            else
            {
                _gfx.DrawRectangle(XBrushes.White, x - _borderWidth1, y, w + 2 * _borderWidth1, h);
                _gfx.DrawLine(pen, x + adj, y - adj, x + w - adj, y + h / 2 + adj);
                _gfx.DrawLine(pen, x + adj, y + h / 2 + adj, x + w - adj, y - adj);
                _gfx.DrawLine(pen, x + adj, y + h / 2 - adj, x + w - adj, y + h + adj);
                _gfx.DrawLine(pen, x + adj, y + h + adj, x + w - adj, y + h / 2 - adj);
            }

        }


        public void Draw(XGraphics gfx)
        {
            XPen pen;
            double x, y, w, h;

            x = XPos + FrameWidth / 2;
            y = YPos - VSpace - TitleHeight;

            double yo = YPos;

            // draw title
            string[] sep = { "\\n" };
            string[] arr = _title.Split(sep, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < arr.Length; i++)
            {
                gfx.DrawString(arr[i], _titleFont, XBrushes.Black, new XPoint(x, y), XStringFormats.TopCenter);
                y += _gfx.MeasureString(arr[i], _titleFont).Height;
            }

            // draw frame
            if (_borderWidth1 != 0)
            {
                x = XPos; 
                y = YPos; 
                w = FrameWidth; 
                h = FrameHeight;
                pen = new XPen(XColors.Black, _borderWidth1);
                gfx.DrawRectangle(pen, x, y, w, h);
            }

            if (_borderWidth2 != 0)
            {
                x = XPos + _borderWidth1 + _borderSpace;
                y = YPos + _borderWidth1 + _borderSpace;
                w = FrameWidth - 2 * (_borderWidth1 + _borderSpace);
                h = FrameHeight - 2 * (_borderWidth1 + _borderSpace);
                pen = new XPen(XColors.Black, _borderWidth2);
                gfx.DrawRectangle(pen, x, y, w, h);
            }

            if (_frameType != 0)
            {
                // frame type 1 
                if (_frameType == 1)
                {
                    //draw horizontal gizmos
                    if (FrameWidth > FrameHeight)
                    {
                        // first gizmo on the top border
                        x = XPos + (FrameWidth / 4) - _borderSpace;
                        y = YPos - _borderWidth1 / 2;
                        w = 2 * (_borderWidth1 + _borderSpace + _borderWidth2);
                        h = _borderWidth1 + _borderSpace + _borderWidth2;
                        DrawGizmo1(x, y, w, h);

                        // second gizmo on the top border
                        x = XPos + 3 * (FrameWidth / 4) - _borderSpace;
                        y = YPos - _borderWidth1 / 2;
                        w = 2 * (_borderWidth1 + _borderSpace + _borderWidth2);
                        h = _borderWidth1 + _borderSpace + _borderWidth2;
                        DrawGizmo1(x, y, w, h);

                        // first gizmo on the bottom border
                        x = XPos + (FrameWidth / 4) - _borderSpace;
                        y = YPos + FrameHeight - (_borderWidth1 + _borderSpace) - _borderWidth1 / 2;
                        w = 2 * (_borderWidth1 + _borderSpace + _borderWidth2);
                        h = _borderWidth1 + _borderSpace + _borderWidth2;
                        DrawGizmo1(x, y, w, h);

                        // second gizmo on the bottom border
                        x = XPos + 3 * (FrameWidth / 4) - _borderSpace;
                        y = YPos + FrameHeight - (_borderWidth1 + _borderSpace) - _borderWidth1 / 2;
                        w = 2 * (_borderWidth1 + _borderSpace + _borderWidth2);
                        h = _borderWidth1 + _borderSpace + _borderWidth2;
                        DrawGizmo1(x, y, w, h);
                    }
                    else
                    {
                        // one gizmo on the top border
                        x = XPos + (FrameWidth / 2) - _borderSpace;
                        y = YPos - _borderWidth1 / 2;
                        w = 2 * (_borderWidth1 + _borderSpace + _borderWidth2);
                        h = _borderWidth1 + _borderSpace + _borderWidth2;
                        DrawGizmo1(x, y, w, h);

                        // one gizmo on the bottom border
                        x = XPos + (FrameWidth / 2) - _borderSpace;
                        y = YPos + FrameHeight - (_borderWidth1 + _borderSpace) - _borderWidth1 / 2;
                        w = 2 * (_borderWidth1 + _borderSpace + _borderWidth2);
                        h = _borderWidth1 + _borderSpace + _borderWidth2;
                        DrawGizmo1(x, y, w, h);
                    }

                    // draw vertical gizmos
                    if (FrameHeight > FrameWidth)
                    {
                        // first gizmo on the left border
                        x = XPos - _borderWidth1 / 2;
                        y = YPos + (FrameHeight / 4) - _borderSpace;
                        w = _borderWidth1 + _borderSpace + _borderWidth2;
                        h = 2 * (_borderWidth1 + _borderSpace + _borderWidth2);
                        DrawGizmo1(x, y, w, h);

                        // second gizmo on the left border
                        x = XPos - _borderWidth1 / 2;
                        y = YPos + 3 * (FrameHeight / 4) - _borderSpace;
                        w = _borderWidth1 + _borderSpace + _borderWidth2;
                        h = 2 * (_borderWidth1 + _borderSpace + _borderWidth2);
                        DrawGizmo1(x, y, w, h);

                        // first gizmo on the right border
                        x = XPos + FrameWidth - (_borderWidth1 + _borderSpace) - _borderWidth1 / 2;
                        y = YPos + (FrameHeight / 4) - _borderSpace;
                        //y = YPos + FrameHeight - (_borderWidth1 + _borderSpace) - _borderWidth1 / 2;
                        w = _borderWidth1 + _borderSpace + _borderWidth2;
                        h = 2 * (_borderWidth1 + _borderSpace + _borderWidth2);
                        DrawGizmo1(x, y, w, h);

                        // second gizmo on the right border
                        x = XPos + FrameWidth - (_borderWidth1 + _borderSpace) - _borderWidth1 / 2;
                        y = YPos + 3 * (FrameHeight / 4) - _borderSpace;
                        //y = YPos + FrameHeight - (_borderWidth1 + _borderSpace) - _borderWidth1 / 2;
                        w = _borderWidth1 + _borderSpace + _borderWidth2;
                        h = 2 * (_borderWidth1 + _borderSpace + _borderWidth2);
                        DrawGizmo1(x, y, w, h);
                    }
                    else
                    {
                        // one gizmo on the left border
                        x = XPos - _borderWidth1 / 2;
                        y = YPos + (FrameHeight / 2) - _borderSpace;
                        w = _borderWidth1 + _borderSpace + _borderWidth2;
                        h = 2 * (_borderWidth1 + _borderSpace + _borderWidth2);
                        DrawGizmo1(x, y, w, h);

                        // one gizmo on the right border
                        x = XPos + FrameWidth - (_borderWidth1 + _borderSpace) - _borderWidth1 / 2;
                        y = YPos + (FrameHeight / 2) - _borderSpace;
                        w = _borderWidth1 + _borderSpace + _borderWidth2;
                        h = 2 * (_borderWidth1 + _borderSpace + _borderWidth2);
                        DrawGizmo1(x, y, w, h);
                    }

                }
            }

            // draw inside the frame
            if (XImage.ExistsFile(_img))
            {
                // draw image
                x = XPos + (FrameWidth - _imgWidth) / 2;
                y = YPos + (FrameHeight - _imgHeight) / 2;
                w = _imgWidth; h = _imgHeight;
                XImage image = XImage.FromFile(_img);
                gfx.DrawImage(image, x, y, w, h);
            }
            else 
            {
                // draw inside text
                XUnit cx, cy;
                cx = XPos + FrameWidth / 2;
                cy = YPos + FrameHeight / 2;
                if (_text1.Length > 0)
                {
                    while (gfx.MeasureString(_text1, _textFont).Width > _imgWidth)
                    {
                        _text1 = _text1.Substring(0, _text1.Length - 1);
                    }
                    gfx.DrawString(_text1, _textFont, XBrushes.Black,
                                   cx.Point, cy.Point - gfx.MeasureString(_text1, _textFont).Height,
                                   XStringFormats.Center);
                }
                if (_text2.Length > 0)
                {
                    while (gfx.MeasureString(_text2, _textFont).Width > _imgWidth)
                    {
                        _text2 = _text2.Substring(0, _text2.Length - 1);
                    }
                    gfx.DrawString(_text2, _textFont, XBrushes.Black,
                                   cx.Point, cy.Point,
                                   XStringFormats.Center);
                }
                if (_text3.Length > 0)
                {
                    while (gfx.MeasureString(_text2, _textFont).Width > _imgWidth)
                    {
                        _text2 = _text2.Substring(0, _text2.Length - 1);
                    }
                    gfx.DrawString(_text3, _textFont, XBrushes.Black,
                                   cx.Point, cy.Point + gfx.MeasureString(_text3, _textFont).Height,
                                   XStringFormats.Center);
                }
            }

            YPos += FrameHeight + VSpace;
            
            double y1,y2,y3;
            y1 = y2 = y3 = 0;

            // draw footnote
            if (_footnote1.Length > 0)
            {
                x = XPos;
                y1 = YPos;
                XStringFormat format = new XStringFormat();
                format.Alignment = XStringAlignment.Near;
                format.LineAlignment = XLineAlignment.Near;
                gfx.DrawString(_footnote1, _textFont, XBrushes.Black, x, y1, format);
                y1 += gfx.MeasureString(_footnote1, _textFont).Height + VSpace;
            }
            if (_footnote2.Length > 0)
            {
                x = XPos + FrameWidth / 2;
                y2 = YPos;
                XStringFormat format = new XStringFormat();
                format.Alignment = XStringAlignment.Center;
                format.LineAlignment = XLineAlignment.Near;
                //gfx.DrawString(_footnote2, _textFont, XBrushes.Black, x, y, format);

                arr = _footnote2.Split(sep, StringSplitOptions.RemoveEmptyEntries);

                for (int i = 0; i < arr.Length; i++)
                {
                    gfx.DrawString(arr[i], _textFont, XBrushes.Black, x, y2, format);
                    y2 += gfx.MeasureString(arr[i], _textFont).Height;
                }

                y2 += VSpace;

                //gfx.DrawString(_footnote2, _textFont, XBrushes.Black, x, y, format);
                //y2 = gfx.MeasureString(_footnote2, _textFont).Height + VSpace;
            }
            if (_footnote3.Length > 0)
            {
                x = XPos + FrameWidth;
                y3 = YPos;
                XStringFormat format = new XStringFormat();
                format.Alignment = XStringAlignment.Far;
                format.LineAlignment = XLineAlignment.Near;
                gfx.DrawString(_footnote3, _textFont, XBrushes.Black, x, y3, format);
                y3 += gfx.MeasureString(_footnote3, _textFont).Height + VSpace;
            }
            YPos = (y1 > y2 ? y1 : (y2 > y3 ? y2 : y3));
        }
    }
}
