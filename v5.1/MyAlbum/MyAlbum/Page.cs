using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyAlbum
{
    internal class Page : BaseElement, IDrawable
    {
        #region fields
        private PdfPage _pdfPage;
        #endregion

        #region properties
        public Frame? Frame { get; private set; }

        public PageOrientation Orientation
        {
            get { return _pdfPage.Orientation; }
            set { _pdfPage.Orientation = value; }
        }
        #endregion

        #region constructors
        public Page()
        {
            _pdfPage ??= new PdfPage();
            //Gfx = XGraphics.FromPdfPage(_pdfPage);
            //Frame ??= new Frame(Gfx);
            //_canvas ??= new BaseElement();

            X = XUnitPt.Zero;
            Y = XUnitPt.Zero;
            W = _pdfPage.Width;
            H = _pdfPage.Height;
        }
        public Page(PdfPage pdfPage) : this()
        {
            _pdfPage = pdfPage;
        }
        #endregion

        #region public methods
        public void Draw()
        {
            throw new NotImplementedException();
        }

        public override void Parse()
        {
            PageStyle? style = FindStyle();
            if (style != null)
                ApplyStyle(style);
            ParseAttributes();
            //ParseComponents();
        }
        #endregion

        #region private methods
        private void ApplyStyle(PageStyle style)
        { 
            Orientation = style.Orientation;
        }
        private PageStyle? FindStyle()
        {
            PageStyle? style = null;

            // use specific style
            //style = Styles.PageStyles.Where(t => t.Style == Xml.Attribute("style")?.Value).FirstOrDefault();

            style = Styles.PageStyles.FirstOrDefault(p => p.Style == Xml.Attribute("style")?.Value) 
                 ?? Styles.PageStyles.FirstOrDefault(p => p.IsDefault);

            return style;
        }

        private void ParseAttributes()
        {
            // base element attribute
            ParseBaseAttributes();

            //// title attribute
            //ParseTitleAttribute();

            //// number attribute
            //ParseNumberAttribute();

            //// orientation attribute
            ParseOrientationAttribute();

            //// size attribute
            //ParseSizeAttribute();

            //// vspace attribute
            //ParseVSpaceAttribute();
        }
        private void ParseOrientationAttribute()
        {
            switch (Xml.Attribute("orientation")?.Value.ToLower())
            {
                case "portrait":
                    this.Orientation = PdfSharp.PageOrientation.Portrait;
                    break;
                case "landscape":
                    this.Orientation = PdfSharp.PageOrientation.Landscape;
                    break;
                default:
                    //if not specified, might have been set up through style - do not overwrite!
                    break;
            }
        }
        #endregion
    }
}
