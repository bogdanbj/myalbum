using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace MyAlbum
{
    internal class Album : BaseElement, IDrawable
    {
        #region fields
        private PdfDocument _pdfDoc;
        private List<Page> _pages;
        #endregion

        #region properties
        public PdfDocument Pdf
		{
			get 
			{ 
				_pdfDoc ??= new PdfDocument();
				return _pdfDoc; 
			}
			set { _pdfDoc = value; }
		}
		public List<Page> Pages
		{
			get 
			{
				_pages ??= new List<Page>();
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

        }
        public Album(XElement xElement) : this()
        {
            Xml = xElement;
        }
        #endregion

        #region public methods
        public void Draw()
        { }

        public override void Parse()
        {
            // Check for not null xml
            if (Xml != null)
            {
                ParseStyles();
                ParseComponents();
            }
            else
            {
                throw new ArgumentNullException(nameof(Xml), "The xml cannot be null.");
            }
        }

        public void Save(string fileName)
        {
            if (Pdf.PageCount == 0)
            {
                PdfPage page = Pdf.AddPage();
            }
            Pdf.Save(fileName);
        }
        #endregion

        #region privare methods
        private void ParseComponents()
        {
            // Get the Album Pages
            IEnumerable<XElement> xmlPages = Xml.Element("pages")?.Elements("page");
            foreach (XElement xmlPage in xmlPages)
            {
                Page newPage = new Page(Pdf.AddPage());
                newPage.Xml = xmlPage;
                newPage.Parse();
                Pages.Add(newPage);
            }
        }

        private void ParseStyles()
        {
            //Get the Styles
            IEnumerable<XElement> xStyles = Xml.Element("styles")?.Elements() ?? Enumerable.Empty<XElement>();
            foreach (XElement elem in xStyles)
            {
                switch (elem.Name.LocalName.ToLower())
                {
                    case "page":
                        PageStyle newPageStyle = new PageStyle();
                        newPageStyle.Xml = elem;
                        newPageStyle.Parse();
                        Styles.PageStyles.Add(newPageStyle);
                        break;
                    //case "border":
                    //    Border newBorder = new Border(elem);
                    //    newBorder.Parse();
                    //    Styles.BorderStyles.Add(newBorder);
                    //    break;
                    //case "row":
                    //    Row newRow = new Row(elem);
                    //    newRow.Parse(isStyle: true);
                    //    Styles.RowStyles.Add(newRow);
                    //    break;
                    //case "column":
                    //    Column newColumn = new Column(elem);
                    //    newColumn.Parse();
                    //    Styles.ColumnStyles.Add(newColumn);
                    //    break;
                    //case "text":
                    //    Text newText = new Text(elem);
                    //    newText.Parse();
                    //    Styles.TextStyles.Add(newText);
                    //    break;
                    //case "image":
                    //    Image newImage = new Image(elem);
                    //    newImage.Parse();
                    //    Styles.ImageStyles.Add(newImage);
                    //    break;
                    //case "stamp":
                    //    Stamp newStamp = new Stamp(elem);
                    //    newStamp.Parse();
                    //    Styles.StampStyles.Add(newStamp);
                    //    break;
                    default:
                        break;
                }
                //Console.WriteLine(elem.ToString());
            }
        }
        #endregion
    }
}
