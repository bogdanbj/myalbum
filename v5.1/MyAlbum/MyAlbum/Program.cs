// See https://aka.ms/new-console-template for more information
using PdfSharp.Fonts;
using System.Diagnostics;
using System.Xml.Linq;

namespace MyAlbum
{
    class Program
    {
        static Album album = new Album();
        
        static void Main(string[] args)
        {
            // Register the custom font resolver
            GlobalFontSettings.FontResolver = new FontResolver();

            string fileName;
            string outputName;
            string pageNumber;

            try
            {
                fileName = args[0];
                if (args.Length > 1)
                {
                    pageNumber= args[1];
                }

                XDocument XAlbum = XDocument.Load(fileName);
                // check xml version
                if (Convert.ToDouble(XAlbum.Root.Attribute("ver").Value) < 3.0) { throw new FormatException("XML file is not compatible with this version."); }

                album = new Album(XAlbum.Root);
                album.Parse();
                album.Draw();

                outputName = Path.ChangeExtension(fileName, ".pdf");
                outputName = outputName.Replace("Templates", "Output");
                if (!Directory.Exists(Path.GetDirectoryName(outputName)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(outputName));
                }
                if (File.Exists(outputName))
                {
                    for (int i = 0; i < 10000; i++)
                    {
                        ;
                        if (!(File.Exists(outputName = fileName.Replace("Templates", "Output").Replace(Path.GetExtension(fileName), i.ToString() + ".pdf"))))
                            break;
                    }
                }

                album.Save(outputName);

                var psi = new ProcessStartInfo
                {
                    FileName = outputName,
                    UseShellExecute = true
                };
                Process.Start(psi);

            }
            catch (Exception ex)
            {
                Console.WriteLine("*** ERROR **");
                Console.WriteLine();
                Console.WriteLine(ex.Message);
                Console.WriteLine();
                Console.WriteLine(ex.Source);
                Console.WriteLine();
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine();
                Console.WriteLine("Press any key to close...");
                Console.ReadKey();
            }

        }
    }
}





