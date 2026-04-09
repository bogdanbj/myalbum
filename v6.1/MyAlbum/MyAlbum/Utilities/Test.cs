using System;
using System.Collections.Generic;
using System.Text;

namespace MyAlbum.Utilities
{
    internal class Test
    {
        internal static void Run()
        {
            CheckPathRooted(@"C:\mydir\myfile.ext");

            CheckPathRooted(@"mydir\sudir\");

            CheckPathRooted(@"\Templates\myfile.ext");

            CheckPathRooted(@"c:Templates\myfile.ext");

            CheckPathRooted(@"d:Templates\myfile.ext");

            //album.Save("test.pdf");
            //Process.Start("test.pdf");

            Console.WriteLine("Press any key to close...");
            Console.ReadKey();
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
