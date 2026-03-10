//using MyAlbum.Models.Pdf;
using MyAlbum.Models.Layout;
using MyAlbum.Models.Xml;
using MyAlbum.Utils;
using PdfSharpCore.Fonts;
using System.Diagnostics;




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

                var options = ParseArgs(args);

                Album album = new Album();
                if (options.ContainsKey("test"))
                {
                    album.Test();
                    album.Save("test.pdf");
                    Process.Start("test.pdf");
                    return;
                }

                string inputFile = options.ContainsKey("input") ? options["input"] : null;
                string outputFile = options.ContainsKey("output") ? options["output"] : null;
                int? pageNumber = options.ContainsKey("page") && int.TryParse(options["page"], out var pn) ? pn : null;

                if (string.IsNullOrEmpty(inputFile))
                {
                    Console.WriteLine("Error: --input <filename> is required.");
                    return;
                }

                String fileName = Path.Combine(Directory.GetCurrentDirectory(), inputFile);
                string outputName = outputFile ?? GetOutputName(fileName);

                // 1. Load the album XML as XDocument to extract the styles reference
                var albumDoc = System.Xml.Linq.XDocument.Load(fileName);
                var stylesAttr = albumDoc.Root?.Attribute("styles")?.Value;

                AlbumStyles styles = null;
                if (!string.IsNullOrEmpty(stylesAttr))
                {
                    // 2. Load and deserialize the styles XML first
                    string stylesPath = Path.Combine(Path.GetDirectoryName(fileName), stylesAttr);
                    styles = XmlFactory.DeserializeStyles(stylesPath);
                }


                // Deserialize XML file into XML Model objects
                var xmlAlbum = XmlFactory.Deserialize(fileName, styles);

                // Create, draw and save the Album
                album.FromXml(xmlAlbum);

                // Determine pages to draw
                string pageArg = options.ContainsKey("page") ? options["page"] : null;
                List<int> pagesToDraw = ParsePageSelection(pageArg);//, maxPage);

                if (pagesToDraw.Count == 0)
                {
                    album.Draw(); // fallback: draw all
                }
                else
                {
                    album.Draw(pagesToDraw);
                }

                album.Save(outputName);

                // Open the PDF file
                var psi = new ProcessStartInfo
                {
                    FileName = outputName,
                    UseShellExecute = true
                };
                Process.Start(psi);

            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"XML deserialization error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
                Console.WriteLine($"Inner exception: {ex.InnerException?.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }

#if DEBUG
            Console.WriteLine("Press any key to close...");
            Console.ReadKey();
#endif


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
                for (int i = 1; i < 10000; i++)
                {
                    ;
                    if (!(File.Exists(outputName = fileName.Replace("Templates", "Output").Replace(Path.GetExtension(fileName), "_" + i.ToString() + ".pdf"))))
                        break;
                }
            }
            return outputName;
        }
        static Dictionary<string, string> ParseArgs(string[] args)
        {
            var options = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            for (int i = 0; i < args.Length; i++)
            {
                string key = null;
                string value = null;

                if (args[i].StartsWith("--"))
                {
                    key = args[i].TrimStart('-');
                }
                else if (args[i].StartsWith("-"))
                {
                    // Map short option to long key
                    switch (args[i])
                    {
                        case "-i":
                            key = "input";
                            break;
                        case "-o":
                            key = "output";
                            break;
                        case "-p":
                            key = "page";
                            break;
                        case "-t":
                            key = "test";
                            break;
                    }
                }
                if (key != null)
                { 
                    // If next arg exists and is not another option, treat as value
                    if (i + 1 < args.Length && !args[i + 1].StartsWith("-"))
                    {
                        value = args[i + 1];
                        i++;
                    }
                    options[key] = value;
                }
            }
            return options;
        }
        static List<int> ParsePageSelection(string pageArg)//, int maxPage)
        {
            List<int> result = new List<int>();
            if (string.IsNullOrWhiteSpace(pageArg) || pageArg.Equals("all", StringComparison.OrdinalIgnoreCase))
            {
                return result;
            }

            string[] parts = pageArg.Split(',', StringSplitOptions.RemoveEmptyEntries);
            foreach (string? part in parts)
            {
                string? trimmed = part.Trim();
                if (trimmed.Contains('-'))
                {
                    string[] range = trimmed.Split('-', StringSplitOptions.RemoveEmptyEntries);
                    if (range.Length == 2 && int.TryParse(range[0], out int start) && int.TryParse(range[1], out int end))
                    {
                        for (int i = start; i <= end; i++)
                            result.Add(i);
                    }
                }
                else if (int.TryParse(trimmed, out int page))
                {
                    result.Add(page);
                }
            }
            return result.Distinct().OrderBy(x => x).ToList();
        }
    }

}