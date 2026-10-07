using System.Configuration;
using System.Diagnostics;
using System.Xml.Linq;
using MyAlbum.Models;
using MyAlbum.Utilities;

namespace MyAlbum
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("MyAlbum v8.0");
            Console.WriteLine();

            try
            {
                // Initialize fonts via FontHandler (fonts.json / FontMap backed).
                InitializeFonts();
                Console.WriteLine();

                /****************************/
                /*  Command line options:   */
                /****************************/
                var options = ArgsParser.ParseArgs(args);

                // Option: -t, --test
                if (options.ContainsKey("test"))
                {
                    Console.WriteLine("Test mode not yet ported.");
                    return;
                }

                // Option: -i, --input <inputFile>
                string inputFile = ArgsParser.GetInputFileName(options);
                Console.WriteLine($"Input file: {inputFile}");
                Console.WriteLine();

                // Option: -o, --output <outputFile>
                string outputFile = ArgsParser.GetOutputFileName(options, inputFile);
                Console.WriteLine($"Output file: {outputFile}");
                Console.WriteLine();

                // Option : -p, --page <pages>. Ex: -p 1,3-5,8+
                PageSelection pageSelection = new();
                if (options.TryGetValue("page", out string? pages) && pages != null)
                {
                    pageSelection = ArgsParser.ParsePageSelection(pages);
                }

                /********************************/
                /*  Parse the XML album file    */
                /********************************/
                Album album = new Album();

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

                    if (File.Exists(stylesPath))
                    {
                        XDocument stylesDoc = XDocument.Load(stylesPath);
                        if (stylesDoc.Root != null)
                            album.ParseStyles(stylesDoc.Root);
                    }
                }

                // Parse the album XML, passing the page selection
                album.ParseXml(xDoc.Root, pageSelection);

                /****************************/
                /*  Draw the album pages    */
                /****************************/
                album.Draw();

                album.Save(outputFile);
                Console.WriteLine($"Generated: {outputFile}");

                // Open the PDF file
                Process.Start(new ProcessStartInfo
                {
                    FileName = outputFile,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            Console.WriteLine();
            Console.WriteLine("Press any key to close...");
            Console.ReadKey();
        }

        /// <summary>
        /// Resolve a path from App.config, making it absolute against the working directory
        /// when the configured value is relative.
        /// </summary>
        private static string ResolvePath(string configKey, string defaultValue)
        {
            var configPath = ConfigurationManager.AppSettings[configKey];
            if (string.IsNullOrWhiteSpace(configPath))
            {
                configPath = defaultValue;
            }

            return Path.IsPathFullyQualified(configPath)
                ? configPath
                : Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), configPath));
        }

        /// <summary>
        /// Initializes the font system by loading custom fonts from the configured fonts folder.
        /// Fonts are registered with PdfSharp's GlobalFontSettings via the FontHandler class,
        /// driven by the fonts.json map produced by the FontManager app.
        /// </summary>
        private static void InitializeFonts()
        {
            var fontsFolder = ResolvePath("FontsFolder", "Fonts");

            if (!Directory.Exists(fontsFolder))
            {
                Console.WriteLine($"Error: Fonts folder not found: {fontsFolder}");
                return;
            }

            FontHandler.Initialize(fontsFolder);
            Console.WriteLine($"Fonts loaded from: {fontsFolder}");

            var families = FontHandler.GetAvailableFamilies().ToList();
            if (families.Count > 0)
            {
                Console.WriteLine($"Available fonts: {string.Join(", ", families)}");
            }
            else
            {
                Console.WriteLine("No fonts found. Use FontManager to add fonts.");
            }
        }
    }
}
