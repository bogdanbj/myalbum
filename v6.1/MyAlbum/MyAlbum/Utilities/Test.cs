using MyAlbum;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace MyAlbum.Samples
{
    internal class Test
    {
        private PdfDocument s_document = new();
        public PdfDocument PdfDoc
        {
            get { return s_document; }
            set { s_document = value; }
        }

        internal void Run()
        {
            // Test if a path is rooted and/or absolute.
            // TestPaths();

            // Test Graphics generation with PdfSharpCore.
            TestGraphics();

            //album.Save("test.pdf");
            //Process.Start("test.pdf");

            Console.WriteLine("Press any key to close...");
            Console.ReadKey();
        }



        void TestGraphics()
        {
            // Create a temporary file
            string filename = String.Format("Output\\Test\\{0}_tempfile.pdf", Guid.NewGuid().ToString("D").ToUpper());
            s_document.Info.Title = "PdfSharpCore XGraphic Sample";
            s_document.Info.Author = "Stefan Lange";
            s_document.Info.Subject = "Created with code snippets that show the use of graphical functions";
            s_document.Info.Keywords = "PdfSharpCore, XGraphics";

            // Create demonstration pages
            PdfPage page; 
            string pageTitle;

            page = s_document.AddPage();
            pageTitle = "Lines and Curves";
            page.Tag = new { PageTitle = pageTitle, PageNumber = s_document.PageCount };
            new LinesAndCurves().DrawPage(page);
            s_document.Outlines.Add(pageTitle, page, true);

            page = s_document.AddPage();
            pageTitle = "Shapes";
            page.Tag = new { PageTitle = pageTitle, PageNumber = s_document.PageCount };
            s_document.Outlines.Add(pageTitle, page, true);
            new Shapes().DrawPage(page);

            page = s_document.AddPage();
            pageTitle = "Paths";
            page.Tag = new { PageTitle = pageTitle, PageNumber = s_document.PageCount };
            s_document.Outlines.Add(pageTitle, page, true);
            new Paths().DrawPage(page);

            page = s_document.AddPage();
            pageTitle = "Text";
            page.Tag = new { PageTitle = pageTitle, PageNumber = s_document.PageCount };
            s_document.Outlines.Add(pageTitle, page, true);
            new Text().DrawPage(page);

            page = s_document.AddPage();
            pageTitle = "Images";
            page.Tag = new { PageTitle = pageTitle, PageNumber = s_document.PageCount };
            s_document.Outlines.Add(pageTitle, page, true);
            new Images().DrawPage(page);

            // Save the s_document...
            s_document.Save(filename);
            var psi = new ProcessStartInfo
            {
                FileName = filename,
                UseShellExecute = true
            };
            Process.Start(psi);

        }




        private static void TestPaths()
        {
            CheckPathRooted(@"C:\mydir\myfile.ext");

            CheckPathRooted(@"mydir\sudir\");

            CheckPathRooted(@"\Templates\myfile.ext");

            CheckPathRooted(@"c:Templates\myfile.ext");

            CheckPathRooted(@"d:Templates\myfile.ext");
        }
        private static void CheckPathRooted(string fileName)
        {
            Console.WriteLine(fileName);
            Console.WriteLine("IsRooted: {0}", Path.IsPathRooted(fileName));
            Console.WriteLine("IsAbsolute: {0}", Path.IsPathFullyQualified(fileName));

            fileName = Path.Combine("C:Documents\\MyAlbum\\Templates", fileName);
            Console.WriteLine(fileName);

            Console.WriteLine(Path.GetFullPath(fileName));
            Console.WriteLine();
        }
    }
}

#region Samples
#endregion
#region LinesAndCurves
#endregion

