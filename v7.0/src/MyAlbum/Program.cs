using System.Configuration;
using System.Diagnostics;
using System.Xml.Linq;
using MyAlbum.Common;
using Def = MyAlbum.Models.Definition;
using MyAlbum.Models.Styles;
using MyAlbum.Parsing;
using MyAlbum.Utilities;
using Lay = MyAlbum.Models.Layout;

namespace MyAlbum;

public class Program
{
    public static void Main(string[] args)
    {
        // Start
        Console.WriteLine("MyAlbum v7.0");
        Console.WriteLine();
        
        // Initialize Fontes
        InitializeFonts();
        Console.WriteLine();

        // Parse arguments
        #region Parse Arguments
        var arguments = ArgumentsHandler.Parse(args);

        if (arguments.ShowHelp)
        {
            ArgumentsHandler.PrintUsage();
            return;
        }

        if (arguments.TestMode)
        {
            RunTests();
            return;
        }

        if (arguments.Error != null)
        {
            Console.WriteLine($"Error: {arguments.Error}");
            return;
        }
        Console.WriteLine();
        #endregion // Parse Arguments

        try
        {
            #region Definition model
            Console.WriteLine("=== Definition Model ===");

            // Load Album
            var inputFile = arguments.InputFile!;
            if (!File.Exists(inputFile))
                throw new FileNotFoundException($"Album file not found: {inputFile}");

            
            // Parse Album
            var doc = XDocument.Parse(File.ReadAllText(inputFile));
            var root = doc.Root
                ?? throw new InvalidOperationException("XML document has no root element.");
            var defAlbum = Def.Album.FromXml(root, arguments.PageSpec);
            
            
            // Parse external styles
            if (!string.IsNullOrEmpty(defAlbum.StyleFile))
            {
                var baseFolder = Path.GetDirectoryName(inputFile) ?? ".";
                defAlbum.ParseExternalStyles(baseFolder);
            }
            defAlbum.Print("");
            Console.WriteLine();
            #endregion // Definition model

            // Resolve styles and build Layout model
            Console.WriteLine();
            Console.WriteLine("=== Layout Model ===");
            // Build stylesheet: external first, then embedded (embedded overrides)
            var styleSheet = StyleResolver.BuildStyleSheet(defAlbum.ExternalStyles);
            foreach (var style in defAlbum.Styles)
                styleSheet.Add(style);
            var resolver = new StyleResolver(styleSheet);
            var layoutAlbum = resolver.Resolve(defAlbum);

            var pageFilter = PageFilter.Parse(arguments.PageSpec ?? "all");
            var filteredLayoutPages = pageFilter.Filter(layoutAlbum.Pages, p => p.Number).ToList();
            Console.WriteLine($"Layout Pages: {filteredLayoutPages.Count}");

            foreach (var page in filteredLayoutPages)
            {
                Console.WriteLine($"  Page {page.Number}: {page.Title ?? "(untitled)"}" +
                    $" [{page.Size}, {page.Orientation}]" +
                    $" padding=[{string.Join(",", page.Padding)}]" +
                    $" ({page.Children.Count} children)");
                PrintLayoutElements(page.Children, "    ");
            }

            // Calculate layout positions
            Console.WriteLine();
            Console.WriteLine("=== Calculating Layout ===");
            
            // Create a filtered album for rendering
            var filteredAlbum = new Lay.Album { Pages = filteredLayoutPages };
            filteredAlbum.Calculate();
            Console.WriteLine($"Layout calculated for {filteredAlbum.Pages.Count} pages");

            // Save to PDF
            Console.WriteLine();
            Console.WriteLine("=== Saving PDF ===");
            var outputFolder = ResolvePath("OutputFolder", "Output");
            if (!Directory.Exists(outputFolder))
                Directory.CreateDirectory(outputFolder);
            var outputFileName = Path.ChangeExtension(Path.GetFileName(arguments.InputFile!), ".pdf");
            var outputPath = GetUniqueOutputPath(outputFolder, outputFileName);
            filteredAlbum.Save(outputPath);
            Console.WriteLine($"Generated: {outputPath}");

            // Open the PDF in default viewer
            Process.Start(new ProcessStartInfo
            {
                FileName = outputPath,
                UseShellExecute = true
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }

        Console.WriteLine();
        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }

    //private static void PrintElements(List<Element> elements, string indent)
    //{
    //    foreach (var el in elements)
    //    {
    //        switch (el)
    //        {
    //            case Row row:
    //                Console.WriteLine($"{indent}row{Attr("space", row.Spacing)}{Attr("valign", row.VAlign)}{Attr("height", row.Height)} ({row.Children.Count} children)");
    //                PrintElements(row.Children, indent + "  ");
    //                break;
    //            case Column col:
    //                Console.WriteLine($"{indent}column{Attr("width", col.Width)}{Attr("align", col.Align)} ({col.Children.Count} children)");
    //                PrintElements(col.Children, indent + "  ");
    //                break;
    //            case Stamp stamp:
    //                Console.WriteLine($"{indent}stamp {stamp.Width}x{stamp.Height}{Attr("title", stamp.Title)}");
    //                break;
    //            case Text text:
    //                var content = text.Content.Length > 30 ? text.Content[..30] + "..." : text.Content;
    //                Console.WriteLine($"{indent}text \"{content}\"{Attr("style", text.Style)}");
    //                break;
    //            case Image img:
    //                Console.WriteLine($"{indent}image \"{img.Src}\"");
    //                break;
    //            case Frame frame:
    //                Console.WriteLine($"{indent}frame {frame.Width}x{frame.Height}");
    //                break;
    //            case Space space:
    //                Console.WriteLine($"{indent}space{Attr("w", space.Width)}{Attr("h", space.Height)}");
    //                break;
    //        }
    //    }
    //}

    private static string Attr(string name, object? value) =>
        value != null && value.ToString() != "" ? $" {name}={value}" : "";

    private static void PrintLayoutElements(List<Lay.Element> elements, string indent)
    {
        foreach (var el in elements)
        {
            switch (el)
            {
                case Lay.Row row:
                    Console.WriteLine($"{indent}row" +
                        $" spacing={row.Spacing}" +
                        (row.Align != "" ? $" align={row.Align}" : "") +
                        (row.VAlign != "" ? $" valign={row.VAlign}" : "") +
                        (row.Height > 0 ? $" h={row.Height}" : "") +
                        $" ({row.Children.Count} children)");
                    PrintLayoutElements(row.Children, indent + "  ");
                    break;
                case Lay.Column col:
                    Console.WriteLine($"{indent}column" +
                        (col.Width > 0 ? $" w={col.Width}" : "") +
                        $" spacing={col.Spacing}" +
                        (col.Align != "" ? $" align={col.Align}" : "") +
                        $" ({col.Children.Count} children)");
                    PrintLayoutElements(col.Children, indent + "  ");
                    break;
                case Lay.Stamp stamp:
                    Console.WriteLine($"{indent}stamp {stamp.Width}x{stamp.Height}");
                    break;
                case Lay.Text text:
                    var content = text.Content.Length > 30 ? text.Content[..30] + "..." : text.Content;
                    Console.WriteLine($"{indent}text \"{content}\"" +
                        (text.FontName != "" ? $" font={text.FontName}" : "") +
                        (text.FontSize > 0 ? $" size={text.FontSize}" : "") +
                        (text.Align != "" ? $" align={text.Align}" : ""));
                    break;
                case Lay.Image img:
                    Console.WriteLine($"{indent}image \"{img.Src}\"");
                    break;
                case Lay.Frame frame:
                    Console.WriteLine($"{indent}frame {frame.Width}x{frame.Height}" +
                        (frame.Lines.Length > 0 ? $" lines=[{string.Join(",", frame.Lines)}]" : ""));
                    break;
                case Lay.Space space:
                    Console.WriteLine($"{indent}space w={space.Width} h={space.Height}");
                    break;
            }
        }
    }

    private static string GetUniqueOutputPath(string folder, string fileName)
    {
        var path = Path.Combine(folder, fileName);
        if (!File.Exists(path))
            return path;

        var baseName = Path.GetFileNameWithoutExtension(fileName);
        var extension = Path.GetExtension(fileName);
        var counter = 1;

        while (File.Exists(path))
        {
            path = Path.Combine(folder, $"{baseName}_{counter}{extension}");
            counter++;
        }

        return path;
    }

    private static string ResolvePath(string configKey, string defaultValue)
    {
        var configPath = ConfigurationManager.AppSettings[configKey] ?? defaultValue;
        return Path.IsPathFullyQualified(configPath)
            ? configPath
            : Path.Combine(AppContext.BaseDirectory, configPath);
    }

    /// <summary>
    /// Initializes the font system by loading custom fonts from the configured fonts folder.
    /// This must be called before any PDF rendering operations that use custom fonts.
    /// </summary>
    /// <remarks>
    /// The fonts folder path is read from App.config (FontsFolder key) or defaults to "Fonts".
    /// Fonts are registered with PdfSharp's GlobalFontSettings via the FontHandler class.
    /// The fonts.json file in the fonts folder maps font family names to their .ttf files.
    /// </remarks>
    private static void InitializeFonts()
    {
        // Resolve the fonts folder path from config or use default
        var fontsFolder = ResolvePath("FontsFolder", "Fonts");
        
        // Verify the fonts folder exists before attempting to load
        if (!Directory.Exists(fontsFolder))
        {
            Console.WriteLine($"Error: Fonts folder not found: {fontsFolder}");
            return;
        }

        // Initialize FontHandler which registers fonts with PdfSharp's font resolver
        FontHandler.Initialize(fontsFolder);
        Console.WriteLine($"Fonts loaded from: {fontsFolder}");
        
        // Display available font families for user reference
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

    private static void RunTests()
    {
        RunIteration2Tests();
        Console.WriteLine("Tests completed.");
    }
    private static void RunIteration2Tests()
    {
        Console.WriteLine("Running Iteration 2 Tests...");
        Console.WriteLine();

        var inputFolder = ResolvePath("InputFolder", "Templates");

        if (!Directory.Exists(inputFolder))
        {
            Console.WriteLine($"Error: Input folder not found: {inputFolder}");
            return;
        }

        // Only test XML files now
        var testFile = Path.Combine(inputFolder, "Test_Iteration2.xml");

        Console.WriteLine(new string('=', 60));
        Console.WriteLine($"Testing: {testFile}");
        Console.WriteLine(new string('=', 60));

        if (!File.Exists(testFile))
        {
            Console.WriteLine($"  SKIPPED - File not found");
        }
        else
        {
            try
            {
                var doc = XDocument.Parse(File.ReadAllText(testFile));
                var root = doc.Root
                    ?? throw new InvalidOperationException("XML document has no root element.");
                var album = Def.Album.FromXml(root);
                Console.WriteLine($"  Parsed: {album.Title ?? "(no title)"}");
                Console.WriteLine($"  Style file: {album.StyleFile ?? "(none)"}");
                Console.WriteLine($"  Pages: {album.Pages.Count}");
                foreach (var page in album.Pages)
                    Console.WriteLine($"    Page {page.Number}: {page.Title ?? "(untitled)"}" +
                        (page.Style != null ? $" [style={page.Style}]" : "") +
                        (page.Orientation != null ? $" [{page.Orientation}]" : ""));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  ERROR: {ex.Message}");
            }
        }

        Console.WriteLine();
        Console.WriteLine("Iteration 2 tests completed.");
    }
}
