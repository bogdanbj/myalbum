using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MyAlbum
{
    internal class Album : BaseElement
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
        public override void Parse()
        {
            ParseStyles();
            ParsePages();
        }
        public void Draw()
        {
            foreach (Page page in _pages)
            {
                page.Calculate();
                page.Draw();
            }
        }
        public void Save(string file_name)
        {
            _pdfDoc.Save(file_name);
        }
        #endregion

        #region private methods
        private void ParseStyles()
        {
            IEnumerable<XElement> xStyles = _xml.Element("styles")?.Elements() ?? Enumerable.Empty<XElement>();
            foreach (XElement elem in xStyles)
            {
                switch (elem.Name.LocalName.ToLower())
                {
                    case "page":
                        PageStyle pageStyle = new PageStyle(elem);
                        pageStyle.Parse();
                        Styles.PageStyles.Add(pageStyle);
                        break;
                    //case "border":
                    //    BorderStyle borserStyle = new BorderStyle(elem);
                    //    borserStyle.Parse();
                    //    Styles.BorderStyles.Add(borserStyle);
                    //    break;
                    //case "row":
                    //    RowStyle rowStyle = new RowStyle(elem);
                    //    rowStyle.Parse();
                    //    Styles.RowStyles.Add(rowStyle);
                    //    break;
                }
            }
        }
        private void ParsePages()
        {
            // tread the xml and create pages
            IEnumerable<XElement> pages = _xml.Element("pages")?.Elements("page") ?? throw new InvalidOperationException("The xml does not contain a <pages> element.");
            if (pages.Count() == 0)
            {
                throw new InvalidOperationException("The <pages> node does not contain any <page> element.");
            }
            foreach (XElement xPage in pages)
            {
                Page newPage = new Page(xPage, _pdfDoc.AddPage());
                newPage.Parse();
                _pages.Add(newPage);
            }
        }
        #endregion
    }
}
