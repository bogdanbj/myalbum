using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.IO.Enumeration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MyAlbum
{
    internal class Album
    {
        #region fields
        private PdfDocument _pdfDoc;
        private List<Page> _pages;
        private XElement _xml;
        #endregion

        #region constructors
        public Album()
        {
            _pdfDoc = new PdfDocument();
            _pages = new List<Page>();
            _xml = new XElement("none");
        }
        public Album(XElement xml) : this()
        {
            _xml = xml;
        }
        #endregion

        #region public methods
        public void Parse()
        { }
        public void Draw()
        { }
        public void Save(string fileName)
        { }

        #endregion
    }
}
