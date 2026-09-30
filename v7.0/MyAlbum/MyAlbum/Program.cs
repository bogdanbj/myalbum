using MyAlbum.Models;
using MyAlbum.Utilities;
using MyAlbum.Samples;
using PdfSharpCore.Fonts;
using System.Diagnostics;
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
                GlobalFontSettings.FontResolver = new MyFontResolver();

                /****************************/
                /*  Command line options:   */
                /****************************/
                var options = ArgsParser.ParseArgs(args); //Dictionary<string, string>

                // Option: -t, --test
                if (options.ContainsKey("test"))
                {
                    Test test = new Test();
                    test.Run();
                    return;
                }

                // Option: -i, --input <inputFile>
                string inputFile = ArgsParser.GetInputFileName(options);

                // Option: -o, --output <outputFile>
                string outputFile = ArgsParser.GetOutputFileName(options, inputFile);

                // Option : -p, --page <pages>. Ex: -p 1,3-5,8++
                PageSelection pageSelection = new();
                if (options.TryGetValue("page", out string? pages))
                {
                    pageSelection = ArgsParser.ParsePageSelection(pages);
                }

                /********************************/
                /*  Parse the XML album file    */
                /********************************/
                Album album = new Album();

                // Parse the album xml file
                XDocument xDoc = XDocument.Load(inputFile);
                if (xDoc.Root == null)
                    throw new InvalidOperationException($"XML file '{inputFile}' has no root element");

                // Extract the styles file name from the "styles" attribute of the root
                string? stylesFile = xDoc.Root.Attribute("styles")?.Value;
                if (!string.IsNullOrEmpty(stylesFile))
                {
                    string stylesPath = Path.Combine(
                        Path.GetDirectoryName(inputFile) ?? "",
                        stylesFile
                    );

                    // Parse the styles document
                    XDocument stylesDoc = XDocument.Load(stylesPath);
                    if (stylesDoc.Root == null)
                        throw new InvalidOperationException($"Styles file '{stylesPath}' has no root element");

                    album.ParseStyles(stylesDoc.Root);
                }

                // Parse the album XML, passing the page selection
                album.ParseXml(xDoc.Root, pageSelection);

                /****************************/
                /*  Draw the album pages    */
                /****************************/
                album.Draw(); // fallback: draw all

                album.Save(outputFile);

                // Open the PDF file
                var psi = new ProcessStartInfo
                {
                    FileName = outputFile,
                    UseShellExecute = true
                };
                Process.Start(psi);

            }
            catch (Exception)
            {

                throw;
            }
#if DEBUG
            Console.WriteLine("Press any key to close...");
            Console.ReadKey();
#endif    
        }
    }
}