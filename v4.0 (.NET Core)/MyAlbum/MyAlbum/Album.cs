using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Xml.Linq;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;


namespace MyAlbum
{

    class Album : Container
    {
        #region Properties
        private PdfDocument _pdfDoc;
        private List<Page> _pages;
        public List<Page> Pages
        {
            get
            {
                if (_pages == null)
                {
                    _pages = new List<Page>();
                } 
                return _pages;
            }
            set { _pages = value; }
        }
        #endregion

        #region Constructors
        public Album()
        {
            _pdfDoc = new PdfDocument();
        }
        public Album(XElement xml) : this()
        {
            Xml = xml;
        }
        #endregion

        #region Public Methods
        public override void Parse()
        {
            ParseStyles();
            ParseComponents();
        }
        public void Save(string fileName)
        {
            _pdfDoc.Save(fileName);
        }
        public override void Draw()
        {
            foreach (Page page in Pages)
            {
                page.PdfPage = _pdfDoc.AddPage();
                page.Calculate();
                page.Draw();
            }
        }
        public override void Calculate()
        {
            // Do not implement. This is by design.
            Console.WriteLine("Album.Calcualte()");
            throw new NotImplementedException("Album.Calculate() is not implementeed.");
        }
        public void Test()
        {

            //PdfPage page = _doc.AddPage();
            //XGraphics gfx = XGraphics.FromPdfPage(page);

            ///*  TEXT  */
            //string[] lorem_ipsum = {
            //                           "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Quisque eget enim eget metus mollis rutrum ac id orci. Donec vestibulum varius ante a vestibulum. Ut facilisis augue vitae tellus pharetra quis ultrices dolor tincidunt. Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Sed sapien libero, lobortis vel iaculis at, pretium ut metus. Curabitur malesuada gravida suscipit. Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Sed a leo a neque commodo faucibus. Nulla auctor, ante ut dignissim hendrerit, mi est congue nisl, sit amet molestie ligula risus sed lorem. Nulla lacus nibh, dictum nec dictum eu, adipiscing sed arcu. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus.",
            //                           "Nullam non tincidunt felis. Aenean accumsan urna id justo lobortis venenatis. In sed tincidunt odio. Maecenas adipiscing magna quis mi porttitor non tincidunt nibh dictum. Maecenas vitae elit ante, interdum vestibulum nisl. Donec et vehicula metus. Etiam sollicitudin quam in elit dignissim bibendum. Duis venenatis, nulla vitae dictum rutrum, dolor velit fermentum est, sit amet interdum orci felis ut sapien.",
            //                           "Integer lacinia, ligula porta tincidunt malesuada, leo erat convallis enim, ac vehicula augue lectus placerat diam. Quisque quis urna sed neque viverra ornare. Cras ultrices, turpis molestie scelerisque facilisis, est elit congue nibh, ac imperdiet lorem nisl at neque. Donec eu dui nec erat sollicitudin pharetra. Integer eu orci arcu. Etiam in nisl eget enim fringilla adipiscing sed et mauris. Sed quis tempus nulla.",
            //                           "Quisque ipsum massa, mollis ac adipiscing quis, molestie vel neque. Vivamus ut nibh a nibh laoreet imperdiet vel in lectus. Integer quis mi non neque vulputate mollis vitae nec leo. Praesent vulputate enim nulla. Nunc facilisis nisi vitae elit fermentum id faucibus sem malesuada. Fusce ultricies congue quam, vitae porttitor ante egestas vitae. Ut sed quam at tellus faucibus euismod et eget ante. Suspendisse posuere, nisl non consequat rhoncus, felis nibh dignissim ligula, non sagittis urna lorem molestie nulla. In luctus vestibulum pretium. Integer leo nisi, viverra nec iaculis sit amet, laoreet quis mi.",
            //                           "Sed tristique vestibulum lectus vel interdum. Suspendisse in lorem ut dolor pretium facilisis eu sit amet dui. Aenean cursus diam eu lacus semper non dictum dui egestas. Praesent nisi libero, accumsan eget ullamcorper a, posuere a dolor. Praesent at nisi est, vel lobortis turpis. Proin consectetur venenatis elit quis convallis. Donec fermentum tortor quis est viverra ac luctus diam hendrerit. Nam et risus libero. Aenean vel metus nisi. Nam aliquam bibendum sagittis.",
            //                       };


            ////XPoint point = new XPoint(20, 100);
            //XFont font = new XFont("Calibri", 14, XFontStyle.Regular);

            ////XRect rect = new XRect(20, 100, 150, 20);
            ////gfx.DrawRectangle(XPens.PowderBlue, rect);
            ////gfx.DrawLine(XPens.PowderBlue, rect.X, rect.Y + rect.Height / 2, rect.X + rect.Width, rect.Y + rect.Height / 2);
            ////gfx.DrawLine(XPens.PowderBlue, rect.X + rect.Width / 2, rect.Y, rect.X + rect.Width / 2, rect.Y + rect.Height);

            //Text text1 = new Text(gfx, font, XBrushes.SlateGray, lorem_ipsum[0]);
            //text1.X = XUnit.FromMillimeter(20);
            //text1.Y = XUnit.FromMillimeter(100);
            //text1.W = XUnit.FromMillimeter(150);
            //text1.H = XUnit.FromMillimeter(20);
            //text1.Align = Alignments.Default;
            ////text1.IsJustified = true;
            ////Text text2 = new Text(gfx, lorem_ipsum[1], rect.X, rect.Y, 45, 50, font, XBrushes.SlateGray, Alignments.TopCenter, false);
            ////text2.IsJustified = true;
            ////Text text3 = new Text(gfx, lorem_ipsum[2], rect.X, rect.Y, 30, 60, font, XBrushes.SlateGray, Alignments.TopCenter, false);
            ////text3.IsJustified = true;
            ////Text text4 = new Text(gfx, lorem_ipsum[3], rect.X, rect.Y, 30, 55, font, XBrushes.SlateGray, Alignments.TopCenter, false);
            ////text4.IsJustified = true;
            ////Text text5 = new Text(gfx, lorem_ipsum[4], rect.X, rect.Y, 40, 45, font, XBrushes.SlateGray, Alignments.TopCenter, false);
            ////text5.IsJustified = true;

            //text1.Draw();
            ////text2.Draw();
            ////text3.Draw();
            ////text4.Draw();
            ////text5.Draw();
            ////BorderPlain border1 = new BorderPlain(gfx, 0, 0, 25, 25, 0.4, 0.1, 0.3, XColors.Black, XColors.Black);
            ////BorderGizmo1 border2 = new BorderGizmo1(gfx, 0, 0, 30, 22, 0.4, 0.1, 0.3, XColors.Maroon, XColors.Olive);
            ////text1.HasBorder = true;
            ////text2.HasBorder = true;
            ////text3.HasBorder = true;
            ////text4.HasBorder = true;
            ////text5.HasBorder = true;

        }
        #endregion

        #region Private Methods
        private void ParseStyles()
        {
            //Get the Styles
            IEnumerable<XElement> xStyles = Xml.Element("styles").Elements();
            foreach (XElement elem in xStyles)
            {
                switch (elem.Name.LocalName.ToLower())
                {
                    case "page":
                        Page newPage = new Page(elem);
                        newPage.Parse(isStyle:true);
                        Styles.PageStyles.Add(newPage);
                        break;
                    case "border":
                        Border newBorder = new Border(elem);
                        newBorder.Parse();
                        Styles.BorderStyles.Add(newBorder);
                        break;
                    case "row":
                        Row newRow = new Row(elem);
                        newRow.Parse(isStyle:true);
                        Styles.RowStyles.Add(newRow);
                        break;
                    case "column":
                        Column newColumn = new Column(elem);
                        newColumn.Parse();
                        Styles.ColumnStyles.Add(newColumn);
                        break;
                    case "text":
                        Text newText = new Text(elem);
                        newText.Parse();
                        Styles.TextStyles.Add(newText);
                        break;
                    case "image":
                        Image newImage = new Image(elem);
                        newImage.Parse();
                        Styles.ImageStyles.Add(newImage);
                        break;
                    case "stamp":
                        Stamp newStamp = new Stamp(elem);
                        newStamp.Parse();
                        Styles.StampStyles.Add(newStamp);
                        break;
                    default:
                        break;
                }
                //Console.WriteLine(elem.ToString());
            }
        }
        private void ParseComponents()
        {
            // Get the Album Pages
            IEnumerable<XElement> pages = Xml.Elements("page");
            foreach (XElement xPage in pages)
            {
                Page newPage = new Page(xPage);
                newPage.Parse();
                this.Pages.Add(newPage);
            }
        }
        #endregion
    }
}
