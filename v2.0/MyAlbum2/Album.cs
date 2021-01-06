using System;
using System.Collections.Generic;
using System.Text;
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
//using System.Windows.Media;

namespace MyAlbum2
{
    class Album
    {
        static public XPen pen = new XPen(XColors.Green);
        static public XBrush brush = XBrushes.Black;

        //private PdfPage page;
        private PdfDocument _doc;
        //private AlbumPage _page;
        //private XGraphics _gfx;

        private PageSize pageSize; // = PageSize.Letter;
        private XUnit marginTop, marginRight, marginBottom, marginLeft;
        private XUnit pageBorderWidth1, pageBorderWidth2, pageBorderSpace, pageBorderWidth;
        //private XPen pen = new XPen(XColors.Black);
        //private XBrush brush = XBrushes.Black;
        //private XFont font = new XFont("Verdana", 20, XFontStyle.Bold);
        private XFont font = new XFont("Arial", 12, XFontStyle.Regular);
        private XFont pageTitleFont = new XFont("Arial", 12, XFontStyle.Regular);
        private string pageTitle;
        private StampRow _row;




        private double indentLeft, indentRight, vSpace; //paragraph left indent, paragraph, right indent, vertical space between paragraphs 

        //private double posx, posy, posc; //current X position, current Y position, centre position (on X axis)

        public Album()
        { 
            _doc = new PdfDocument();
            //marginTop = marginRight = marginBottom = marginLeft = XUnit.FromMillimeter(5);
            //pageBorderWidth1 = pageBorderWidth2 = pageBorderSpace = XUnit.Zero;
        }

        public void Draw(string commandLine)
        {
            string command;
            string arguments = "";

            commandLine = commandLine.Trim();

            if (commandLine.IndexOf("#") >= 0)
                commandLine = commandLine.Substring(0, commandLine.IndexOf("#"));

            if (commandLine.IndexOf('(') > 1)
                command = commandLine.Substring(0, commandLine.IndexOf('(')).Trim();
            else
                command = commandLine.Trim();
            
            arguments = Program.TrimMatchingBrackets(commandLine.Remove(0,command.Length).Trim(),'(',')');


            switch (command.ToUpper())
            {
                case "ALBUM_PAGES_SIZE":
                    SetPageSize(arguments);
                    break;
                case "ALBUM_PAGES_MARGINS":
                    SetMargins(arguments);
                    break;
                case "ALBUM_PAGES_BORDER":
                    SetBorder(arguments);
                    break;
                case "ALBUM_PAGE_TITLE_FONT":
                    SetPageTitleFont(arguments);
                    break;
                case "ALBUM_PAGES_TITLE":
                    SetPageTitle(arguments);
                    break;
                case "ALBUM_PAGES_SPACING":
                    SetPageSpaces(arguments);
                    break;
                case "PAGE_START":
                    PageStart(arguments);
                    break;
                case "FONT":
                    SetFont(arguments);
                    break;
                /*
                case "PAGE_TEXT":
                    _page.DrawString(arguments, font, XStringFormats.TopLeft);
                    break;
                case "PAGE_TEXT_CENTRE":
                    _page.DrawString(arguments, font, XStringFormats.TopCenter);
                    break;
                case "SPACE":
                    _page.YPos += XUnit.FromMillimeter(Convert.ToDouble(arguments)).Point;
                    break;
                case "ROW_START":
                    StartRow(arguments);
                    break;
                case "ROW_END":
                    EndRow();
                    break;
                case "FRAME_TYPE":
                    SetFrameType(arguments);
                    break;
                case "STAMP_ADD":
                    AddStamp(arguments);
                    break;
                case "ROW_SPACE":
                    AddSpace(arguments);
                    break;
                case "LINE":
                    DrawLine(arguments);
                    break;
                case "TEST":
                    Test();
                    break;
                 */
            }
        }

        void PageStart(string arguments)
        {
            /*
            //NEW PAGE 
            _page = new AlbumPage(_doc.AddPage());
            
            _page.Size = pageSize;
            _gfx = _page.gfx;

            if (arguments.Trim().Length > 0)
            {
                if (arguments.ToUpper().IndexOf("LAND") >= 0) { _page.Orientation = PageOrientation.Landscape; }
                if (arguments.ToUpper().IndexOf("PORT") >= 0) { _page.Orientation = PageOrientation.Portrait; }
                if (arguments.ToUpper().IndexOf("MIRR") >= 0) { _page.IsMirror = true; }
            }
            //SET MARGINS
            if (_page.Orientation == PageOrientation.Portrait)
            {
                _page.MarginTop = marginTop;
                _page.MarginRight = (_page.IsMirror ? marginLeft : marginRight);
                _page.MarginBottom = marginBottom;
                _page.MarginLeft = (_page.IsMirror ? marginRight : marginLeft);
            }
            else
            {
                _page.MarginTop = (_page.IsMirror ? marginRight : marginLeft);
                _page.MarginRight = marginTop;
                _page.MarginBottom = (_page.IsMirror ? marginLeft : marginRight);
                _page.MarginLeft = marginBottom;
            }



            //DRAW BORDER
            _page.DrawBorder(pageBorderWidth1.Point, pageBorderWidth2.Point, pageBorderSpace.Point);

            _page.IndentLeft = indentLeft;
            _page.IndentRight = indentRight;
            _page.VSpace = vSpace;
            _page.XPos = _page.XLeft;
            _page.YPos = _page.YTop;

            //DRAW HEADER
            _page.DrawString(pageTitle, pageTitleFont, XStringFormats.TopCenter);

            //if (_page.Orientation == PageOrientation.Portrait)
            //{
            //    _page.DrawString(pageTitle, pageTitleFont, XStringFormats.TopCenter);
            //}
            //else
            //{
            //    _gfx.RotateAtTransform(90, new XPoint(_page.XCenter, _page.YCenter));
            //    _gfx.TranslateTransform(0, (_page.Height - _page.Width) / 2);
            //    _page.DrawString(pageTitle, pageTitleFont, XStringFormats.TopCenter);
            //    _gfx.TranslateTransform(0, -(_page.Height - _page.Width) / 2);
            //    _gfx.RotateAtTransform(-90, new XPoint(_page.XCenter, _page.YCenter));
            //}

            if (arguments.Trim().Length > 0)
            {
                if (arguments.ToUpper().IndexOf("LAND") >= 0) { _page.Orientation = PageOrientation.Landscape; }
                if (arguments.ToUpper().IndexOf("PORT") >= 0) { _page.Orientation = PageOrientation.Portrait; }
                if (arguments.ToUpper().IndexOf("MIRR") >= 0) { _page.IsMirror = true; }
            }
             */
        }

        void SetPageSize(string arguments)
        {
            switch (arguments.Trim().ToUpper())
            {
                case "LETTER":
                    pageSize = PageSize.Letter;
                    break;
                case "A4":
                    pageSize = PageSize.A4;
                    break;
                default:
                    pageSize = PageSize.Letter;
                    break;
            }
        }

        void SetMargins(string arguments)
        {
            string[] args = arguments.Split(',');

            if (args.Length != 4) { throw new Exception("Incorrect number of parameters"); }
            
            try
            {
                marginLeft = XUnit.FromMillimeter(Convert.ToDouble(args[0]));
                marginTop = XUnit.FromMillimeter(Convert.ToDouble(args[1]));
                marginRight = XUnit.FromMillimeter(Convert.ToDouble(args[2]));
                marginBottom = XUnit.FromMillimeter(Convert.ToDouble(args[3]));
            }
            catch (Exception)
            {
                throw new Exception("Invalid parameter");
            }
        }

        void SetBorder(string arguments)
        {
            string[] args = arguments.Split(',');

            if (args.Length != 3) { throw new Exception("Incorrect number of parameters"); }
            
            try
            {
                pageBorderWidth1 = XUnit.FromMillimeter(Convert.ToDouble(args[0]));
                pageBorderWidth2 = XUnit.FromMillimeter(Convert.ToDouble(args[1]));
                pageBorderSpace = XUnit.FromMillimeter(Convert.ToDouble(args[2]));
            }
            catch (Exception)
            {
                throw new Exception("Invalid parameter");
            }
        }

        void SetPageTitleFont(string arguments)
        {
            pageTitleFont =  AlbumPage.GetFont(arguments);
        }

        void SetFont(string arguments)
        {
            font = AlbumPage.GetFont(arguments);
        }

        void SetPageTitle(string arguments)
        { 
            pageTitle = arguments; 
        }

        void SetPageSpaces(string arguments)
        {
            string[] args = arguments.Split(',');

            if (args.Length != 3) { throw new Exception("Incorrect number of parameters"); }

            try
            {
                indentLeft = XUnit.FromMillimeter(Convert.ToDouble(args[0].Trim()));
                indentRight = XUnit.FromMillimeter(Convert.ToDouble(args[1].Trim()));
                vSpace = XUnit.FromMillimeter(Convert.ToDouble(args[2].Trim()));
            }
            catch (Exception)
            {
                throw new Exception("Invalid parameter");
            }
        }

/*        private void WriteText(string text, XFont font, XStringFormat stringFormat)
        {
            string line = "";
            string[] words;
            double len, pos;

            pos = (stringFormat.Alignment==XStringAlignment.Near ? posx : posc);
            
            words = text.Split(' ');
            //len = 2 * pageBorderWidth + indentLeft + indentRight;
            len = indentLeft + indentRight;

            for (int i = 0; i < words.Length; i++)
            {
                if (len + gfx.MeasureString(words[i] + ' ', font).Width < page.Width)
                {
                    line += words[i] + ' ';
                    len += gfx.MeasureString(words[i] + ' ', font).Width;
                }
                else
                {
                    gfx.DrawString(line, font, XBrushes.Black, new XPoint(pos, posy), stringFormat);
                    posy += gfx.MeasureString(line, font).Height;
                    line = words[i] + ' ';
                    //len = 2 * pageBorderWidth + indentLeft + indentRight + gfx.MeasureString(words[i] + ' ', font).Width;
                    len = indentLeft + indentRight + gfx.MeasureString(words[i] + ' ', font).Width;
                }
            }
            gfx.DrawString(line, font, XBrushes.Black, new XPoint(pos, posy), stringFormat);
            posy += gfx.MeasureString(line, font).Height + vSpace;
        }
*/

        public void Save (string fileName)
        { 
            _doc.Save(fileName); 
        }

        /*
        void StartRow(string arguments)
        {
            string[] args = arguments.Split(',');

            if (args.Length == 13)
                _page.NewStampsRow(args);
            else
                throw new Exception("Incorrect number of parameters"); 
        }

        void SetFrameType(string arguments)
        {
            _page.SetFrameType(arguments);
        }

        void AddStamp(string arguments)
        {
            _page.AddStamp(arguments);
        }

        void AddSpace(string arguments)
        {
            _page.AddSpace(arguments);
        }

        void EndRow()
        {
            _page.DrawStampsRow();
        }


        void DrawLine(string arguments)
        {
            _page.DrawLine(arguments);
        }
        */

        public void Test()
        {

            PdfPage page = _doc.AddPage();
            XGraphics gfx = XGraphics.FromPdfPage(page);
            
            /* BORDER 

            //BorderPlain border = new BorderPlain(gfx, 5, 5, page.Width.Millimeter - 10, page.Height.Millimeter - 10, 0.5, 0.1, 0.5, XColors.Black, XColors.Black);
            //border.Draw();

            BorderGraphic bg = new BorderGraphic(gfx, 5, 5, page.Width.Millimeter - 10, page.Height.Millimeter - 10, 
                                                tlFile: "..\\Images\\line1_tl.jpg", 
                                                trFile: "..\\Images\\line1_tr.jpg", 
                                                blFile: "..\\Images\\line1_bl.jpg", 
                                                brFile: "..\\Images\\line1_br.jpg",
                                                hFile: "..\\Images\\line1_h.jpg",
                                                vFile: "..\\Images\\line1_v.jpg");
            bg.Draw();

            BorderGizmo1 border1 = new BorderGizmo1(gfx, 50, 30, 40, 30, 0.2, 0.2, 0.5, XColors.Black, XColors.Black);
            border1.Draw();

            BorderGraphic bg1 = new BorderGraphic(gfx, 100, 27, 48, 36, tlFile: "..\\Images\\vine1_tl_1.jpg",
                                                brFile: "..\\Images\\vine1_br_1.jpg");
            bg1.Draw();
            */


            /* IMAGE
            XPoint pt1 = new XPoint(20, 100);
            XPoint pt2 = new XPoint(200, 100);

            Image img = new Image(gfx, "0073.jpg", pt1);
            img.Draw();
            img = new Image(gfx, "0073-trim.jpg", pt2, Alignments.CenterRight);
            img.Draw();

            gfx.DrawLine(XPens.DarkOrange, pt1, new XPoint(pt1.X + 400, pt1.Y));
            gfx.DrawLine(XPens.DarkOrange, pt1, new XPoint(pt1.X, pt1.Y + 200));
            gfx.DrawLine(XPens.DarkOrange, new XPoint(pt2.X, pt1.Y - 100), new XPoint(pt2.X, pt1.Y + 100));
            */

            /*  TEXT  */
            string[] lorem_ipsum = {
                                       "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Quisque eget enim eget metus mollis rutrum ac id orci. Donec vestibulum varius ante a vestibulum. Ut facilisis augue vitae tellus pharetra quis ultrices dolor tincidunt. Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Sed sapien libero, lobortis vel iaculis at, pretium ut metus. Curabitur malesuada gravida suscipit. Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Sed a leo a neque commodo faucibus. Nulla auctor, ante ut dignissim hendrerit, mi est congue nisl, sit amet molestie ligula risus sed lorem. Nulla lacus nibh, dictum nec dictum eu, adipiscing sed arcu. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus.",
                                       "Nullam non tincidunt felis. Aenean accumsan urna id justo lobortis venenatis. In sed tincidunt odio. Maecenas adipiscing magna quis mi porttitor non tincidunt nibh dictum. Maecenas vitae elit ante, interdum vestibulum nisl. Donec et vehicula metus. Etiam sollicitudin quam in elit dignissim bibendum. Duis venenatis, nulla vitae dictum rutrum, dolor velit fermentum est, sit amet interdum orci felis ut sapien.",
                                       "Integer lacinia, ligula porta tincidunt malesuada, leo erat convallis enim, ac vehicula augue lectus placerat diam. Quisque quis urna sed neque viverra ornare. Cras ultrices, turpis molestie scelerisque facilisis, est elit congue nibh, ac imperdiet lorem nisl at neque. Donec eu dui nec erat sollicitudin pharetra. Integer eu orci arcu. Etiam in nisl eget enim fringilla adipiscing sed et mauris. Sed quis tempus nulla.",
                                       "Quisque ipsum massa, mollis ac adipiscing quis, molestie vel neque. Vivamus ut nibh a nibh laoreet imperdiet vel in lectus. Integer quis mi non neque vulputate mollis vitae nec leo. Praesent vulputate enim nulla. Nunc facilisis nisi vitae elit fermentum id faucibus sem malesuada. Fusce ultricies congue quam, vitae porttitor ante egestas vitae. Ut sed quam at tellus faucibus euismod et eget ante. Suspendisse posuere, nisl non consequat rhoncus, felis nibh dignissim ligula, non sagittis urna lorem molestie nulla. In luctus vestibulum pretium. Integer leo nisi, viverra nec iaculis sit amet, laoreet quis mi.",
                                       "Sed tristique vestibulum lectus vel interdum. Suspendisse in lorem ut dolor pretium facilisis eu sit amet dui. Aenean cursus diam eu lacus semper non dictum dui egestas. Praesent nisi libero, accumsan eget ullamcorper a, posuere a dolor. Praesent at nisi est, vel lobortis turpis. Proin consectetur venenatis elit quis convallis. Donec fermentum tortor quis est viverra ac luctus diam hendrerit. Nam et risus libero. Aenean vel metus nisi. Nam aliquam bibendum sagittis.",
                                   };


            //XPoint point = new XPoint(20, 100);
            XFont font = new XFont("Calibri", 14, XFontStyle.Regular);

            XRect rect = new XRect(20, 100, 150, 20);
            //gfx.DrawRectangle(XPens.PowderBlue, rect);
            //gfx.DrawLine(XPens.PowderBlue, rect.X, rect.Y + rect.Height / 2, rect.X + rect.Width, rect.Y + rect.Height / 2);
            //gfx.DrawLine(XPens.PowderBlue, rect.X + rect.Width / 2, rect.Y, rect.X + rect.Width / 2, rect.Y + rect.Height);

            Text text1 = new Text(gfx, lorem_ipsum[0], rect.X, rect.Y, 25, 40, font, XBrushes.SlateGray, Alignments.TopCenter, false);
            text1.Justified = true;
            Text text2 = new Text(gfx, lorem_ipsum[1], rect.X, rect.Y, 45, 50, font, XBrushes.SlateGray, Alignments.TopCenter, false);
            text2.Justified = true;
            Text text3 = new Text(gfx, lorem_ipsum[2], rect.X, rect.Y, 30, 60, font, XBrushes.SlateGray, Alignments.TopCenter, false);
            text3.Justified = true;
            Text text4 = new Text(gfx, lorem_ipsum[3], rect.X, rect.Y, 30, 55, font, XBrushes.SlateGray, Alignments.TopCenter, false);
            text4.Justified = true;
            Text text5 = new Text(gfx, lorem_ipsum[4], rect.X, rect.Y, 40, 45, font, XBrushes.SlateGray, Alignments.TopCenter, false);
            text5.Justified = true;
            BorderPlain border1 = new BorderPlain(gfx, 0, 0, 25, 25, 0.4, 0.1, 0.3, XColors.Black, XColors.Black);
            //BorderGizmo1 border2 = new BorderGizmo1(gfx, 0, 0, 30, 22, 0.4, 0.1, 0.3, XColors.Maroon, XColors.Olive);
            text1.HasBorder = true;
            text2.HasBorder = true;
            text3.HasBorder = true;
            text4.HasBorder = true;
            text5.HasBorder = true;

            Image img1 = new Image(gfx, "..\\images\\50.jpg", 0, 0, 25, 25, Alignments.TopLeft);
            img1.HasBorder = true;
            Image img2 = new Image(gfx, "..\\images\\0073.jpg", 0, 0, 25, 25, Alignments.TopLeft);
            img2.HasBorder = true;


            Stamp stamp1 = new Stamp(gfx, 10, 30, 38, 26);
            stamp1.VSpace = XUnit.FromMillimeter(1);
            stamp1.Title = new Text(gfx, "Queen Victoria Diamond Jubilee");
            stamp1.Title.Font = new XFont("Calibri", 10, XFontStyle.Bold);

            stamp1.Border = border1;
            stamp1.Border.Padding = XUnit.FromMillimeter(2);

            stamp1.Img = new Image(gfx, "..\\images\\50.jpg");
            stamp1.Img.Padding = XUnit.FromMillimeter(2);

            Text note = new Text(gfx, "Two portraits of Queen Victoria 1837 and 1867");
            //note.Padding = XUnit.FromMillimeter(2);
            note.Alignment = Alignments.TopCenter;
            note.Font = new XFont("Calibri", 8, XFontStyle.Regular);
            //note.Justified = true;
            //note.HasBorder = true;
            stamp1.FootNotes.Add(note);
            
            //note = new Text(gfx, "b b b b b b b b b b b b b b b b b b b b b b b b b b b b b b b b b b b b b b b b b b b b");
            //note.Padding = XUnit.FromMillimeter(-2);
            //note.Alignment = Alignments.TopRight;
            //note.Font = new XFont("Calibri", 8, XFontStyle.Regular);
            //note.HasBorder = true;
            //stamp1.FootNotes.Add(note);

            //stamp1.Draw();

            font = new XFont("Calibri", 10, XFontStyle.Regular); 
            TextElement text = new TextElement(gfx, lorem_ipsum[0], 10, 80, 120, 10, font, XBrushes.SlateGray, Alignments.TopCenter, true);
            text.HasBorder = true;
            text.Justified = true;
            //text.Draw();

            //Stamp stamp1 = new Stamp(gfx, 10, 10, 38, 26,
            //                        2, 1,
            //                        "TITLE - boogie", new XFont("Calibri", 10, XFontStyle.Bold),
            //                        "INNER1", new XFont("Calibri", 8, XFontStyle.Regular),
            //                        "left", "center", "right", new XFont("Calibri", 8, XFontStyle.Regular),
            //                        "..\\images\\50.jpg", border1);
            //Stamp stamp2 = new Stamp(gfx, 60, 10, 38, 26,
            //                        2, 1,
            //                        "TITLE - boogie", new XFont("Calibri", 10, XFontStyle.Bold),
            //                        "INNER2", new XFont("Calibri", 8, XFontStyle.Regular),
            //                        "left", "center", "right", new XFont("Calibri", 8, XFontStyle.Regular),
            //                        "..\\images\\51.jpg", border1);
            //Stamp stamp3 = new Stamp(gfx, 120, 30, 38, 26,
            //                        2, 1,
            //                        "This is a long title of this stamp. Do not even try to read it", new XFont("Calibri", 10, XFontStyle.Bold),
            //                        lorem_ipsum[1], new XFont("Calibri", 8, XFontStyle.Regular),
            //                        "the left note is so big that overlaps to the right", "same goes for the center, just this was supposed to stand alone", "the right one is tricky again. we shall see what happens", new XFont("Calibri", 8, XFontStyle.Regular),
            //                        "..\\images\\51.jpg", border1);
            //stamp1.X = XUnit.FromMillimeter(100);
            //stamp1.Y = XUnit.FromMillimeter(50);
            //stamp1.Draw();
            //stamp2.Draw();
            //stamp3.Draw();


            
            Block block = new Block(gfx, 10, 30, page.Width.Millimeter-20, 20, Alignments.TopCenter, 10, true);
            //block.HasBorder = true;
            block.Add(stamp1);
            block.Add(text);
            block.HasBorder = true;
            //block.Add(stamp3);
            //block.Add(text4);
            ////block.Add(text5);
            block.Draw();
            



            //text2.Draw();
            
        }
        
        
        //public void BeginBox(XGraphics gfx, int number, string title)
        //{
        //    const int dEllipse = 15;
        //    XRect rect = new XRect(0, 20, 300, 200);
        //    if (number % 2 == 0)
        //        rect.X = 300 - 5;
        //    rect.Y = 40 + ((number - 1) / 2) * (200 - 5);
        //    rect.Inflate(-10, -10);
        //    XRect rect2 = rect;
        //    rect2.Offset(this.borderWidth, this.borderWidth);
        //    gfx.DrawRoundedRectangle(new XSolidBrush(this.shadowColor), rect2, new XSize(dEllipse + 8, dEllipse + 8));
        //    XLinearGradientBrush brush = new XLinearGradientBrush(rect, this.backColor, this.backColor2, XLinearGradientMode.Vertical);
        //    gfx.DrawRoundedRectangle(this.borderPen, brush, rect, new XSize(dEllipse, dEllipse));
        //    rect.Inflate(-5, -5);

        //    XFont font = new XFont("Verdana", 12, XFontStyle.Regular);
        //    gfx.DrawString(title, font, XBrushes.Navy, rect, XStringFormats.TopCenter);

        //    rect.Inflate(-10, -5);
        //    rect.Y += 20;
        //    rect.Height -= 20;

        //    this.state = gfx.Save();
        //    gfx.TranslateTransform(rect.X, rect.Y);
        //}
        //public void EndBox(XGraphics gfx)
        //{
        //    gfx.Restore(this.state);
        //}
    }
}
