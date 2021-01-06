using System;
using System.Collections.Generic;
using System.Text;
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
//using System.Windows.Media;

namespace MyAlbum 
{
    class Album
    {
        static XPen pen = new XPen(XColors.Black);

        //private PdfPage page;
        private PdfDocument _doc;
        private AlbumPage _page;
        private XGraphics _gfx;

        private PageSize pageSize = PageSize.Letter;
        private XUnit marginTop, marginRight, marginBottom, marginLeft;
        private XUnit pageBorderWidth1, pageBorderWidth2, pageBorderSpace, pageBorderWidth;
        //private XPen pen = new XPen(XColors.Black);
        private XBrush brush = XBrushes.Black;
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
            }
        }

        void PageStart(string arguments)
        {
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

        void Test()
        {
           
            XGraphics gfx  ;
            gfx = _page.gfx;


            //BeginBox(gfx, 3, "DrawCurve");

            XPoint[] points =
              new XPoint[] { new XPoint(20, 30), new XPoint(60, 120), new XPoint(90, 20), new XPoint(170, 90), new XPoint(230, 40) };
            pen = new XPen(XColors.RoyalBlue, 3.5);
            gfx.DrawCurve(pen, points, 1);


            gfx.DrawPolygon(pen, points);
            
            //EndBox(gfx);

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
