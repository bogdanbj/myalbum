using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloWorld
{
    internal class Album
    {
        #region fields
        private PdfDocument _pdfDoc;
        private List<Page> _pages;
        #endregion

        #region properties
        public PdfDocument Pdf
        {
            get { 
                _pdfDoc ??= new PdfDocument(); // if it is null create a new instance
                return _pdfDoc; 
            }
            set { _pdfDoc = value; }
        }
        public List<Page> Pages
        {
            get 
            { 
                _pages ??= new List<Page>(); // if null create a new instance
                return _pages; 
            }
            set { _pages = value; }
        }
        #endregion

        #region constructors
        public Album()
        {
            _pdfDoc = new PdfDocument();
            _pages = new List<Page>();
            Page page;

            page = new Page(_pdfDoc.AddPage());
            page.Orientation = PageOrientation.Portrait;
            page.MarginLeft = XUnitPt.FromMillimeter(25);
            page.MarginTop = XUnitPt.FromMillimeter(7);
            page.MarginRight = XUnitPt.FromMillimeter(7);
            page.MarginBottom = XUnitPt.FromMillimeter(7);
            this.Pages.Add(page);

            page = new Page(_pdfDoc.AddPage());
            page.Orientation = PageOrientation.Landscape;
            page.MarginLeft = XUnitPt.FromMillimeter(7);
            page.MarginTop = XUnitPt.FromMillimeter(25);
            page.MarginRight = XUnitPt.FromMillimeter(7);
            page.MarginBottom = XUnitPt.FromMillimeter(7);
            this.Pages.Add(page);
        }
        #endregion

        #region public methods
        public void Parse()
        { 
        
        }
        
        public void Draw()
        {
            foreach (var page in this.Pages)
            {
                page.Calculate();
                page.Draw();
            }
        }

        public void Save(string fileName)
        { 
            this._pdfDoc.Save(fileName);
        }
        #endregion
    }
}
