using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;

namespace HelloWorld
{
    internal class Page : BaseElement
    {
        #region fields
        private PdfPage _pdfPage;
        private PageOrientation _orientation;
        private PageSize _size;
        private Frame _frame;
        private BaseElement _canvas;
        #endregion

        #region properties
        public PdfPage PdfPage
		{
			get { 
                if (_pdfPage == null)
                    _pdfPage = new PdfPage();
                return _pdfPage; 
            }
			set { _pdfPage = value; }
		}
        public PageOrientation Orientation
        {
            get { return _orientation; }
            set 
            { 
                _orientation = value;
                this.PdfPage.Orientation = _orientation;
            }
        }
        public PageSize Size
        {
            get { return _size; }
            set 
            { 
                _size = value;
                this.PdfPage.Size = _size;
            }
        }
        public Frame Frame
        { 
            get { return _frame; } 
        }
        public BaseElement Canvas
        { 
            get { return _canvas; } 
        }

        public XUnitPt MarginTop { get; set; }
        public XUnitPt MarginBottom { get; set; }
        public XUnitPt MarginLeft { get; set; }
        public XUnitPt MarginRight { get; set; }
        #endregion

        #region constructors
        public Page()
        {
            _pdfPage ??= new PdfPage();
            _frame ??= new Frame();
            _canvas ??= new BaseElement();

            x = XUnitPt.Zero;
            y = XUnitPt.Zero;
            w = _pdfPage.Width;
            h = _pdfPage.Height;
        }
        public Page(PdfPage pdfPage) : this()
        {
            _pdfPage = pdfPage;
        }
        #endregion

        #region public methods
        public void Calculate()
        {
            gfx = XGraphics.FromPdfPage(_pdfPage);
            Frame.gfx = gfx;
            if (Orientation == PageOrientation.Portrait)
            {
                Frame.x = x + MarginLeft;
                Frame.y = y + MarginTop;
                Frame.w = w - (MarginLeft + MarginRight);
                Frame.h = h - (MarginTop + MarginBottom);
            }
            else
            {
                Frame.x = x + MarginLeft;
                Frame.y = y + MarginTop;
                Frame.w = h - (MarginLeft + MarginRight);
                Frame.h = w - (MarginTop + MarginBottom);
            }

            Canvas.x = Frame.x; Canvas.y = Frame.y; Canvas.w = w; Canvas.h = h;

        }

        public void Draw()
        {
            // draw frame
            Frame.Draw();

            // draw banner

            // draw content
            if (Orientation == PageOrientation.Landscape)
            {
                //gfx.TranslateTransform(w/2, h/2);
                //gfx.RotateTransform(90);
                //gfx.TranslateTransform(-w/2, -h/2);
            }

            XPdfFontOptions options = new XPdfFontOptions(PdfFontEncoding.Unicode, PdfFontEmbedding.EmbedCompleteFontFile);
            XFont font = new XFont("Stymie Becker Light", 20, XFontStyleEx.Regular, options);

            // Draw the text
            gfx.DrawString("TREFFLÉ BERTHIAUME", font, XBrushes.Black,
                            new XRect(Canvas.x, Canvas.y, Canvas.w, Canvas.h),
                            XStringFormats.TopLeft);

            //XSolidBrush brush;
            //brush = new XSolidBrush(XColors.Azure);
            //gfx!.DrawRectangle(brush, 10, 10, 100, 100);
            //brush = new XSolidBrush(XColors.Plum);
            //gfx!.DrawRectangle(brush, 10, 10, 25, 25);

        }
        #endregion
    }
}
