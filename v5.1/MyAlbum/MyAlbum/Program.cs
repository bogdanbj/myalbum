

using PdfSharp;
using PdfSharp.Fonts;
using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;
using System.Xml.Linq;




namespace MyAlbum
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {

                // Register the custom font resolver
                GlobalFontSettings.FontResolver = new FontResolver();

                string fileName = args[0];
                string outputName = GetOutputName(fileName);

                // Read xml
                XDocument xml = XDocument.Load(fileName);
                if (Convert.ToDouble(xml.Root.Attribute("ver").Value) < 5.0) { throw new FormatException("XML file is not compatible with this version."); }

                // Create, process, save album
                Album album = new Album(xml.Root);
                album.Parse();
                album.Draw();
                album.Save(outputName);

                // Show album pdf
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
                //Console.WriteLine(errXml);
                //Console.WriteLine();
                Console.WriteLine("Press any key to close...");
                Console.ReadKey();
            }
        }

        static string GetOutputName(string fileName)
        {
            string outputName;
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
            return outputName;
        }
    }
}