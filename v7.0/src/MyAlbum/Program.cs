using System.Configuration;
using MyAlbum.Resources;

namespace MyAlbum;

public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("MyAlbum v7.0");

        // Initialize fonts
        var fontsFolder = Path.GetFullPath(
            ConfigurationManager.AppSettings["FontsFolder"] ?? "Resources/Fonts");
        
        if (Directory.Exists(fontsFolder))
        {
            FontLoader.Initialize(fontsFolder);
            Console.WriteLine($"Fonts loaded from: {fontsFolder}");
            
            var families = FontLoader.GetAvailableFamilies().ToList();
            if (families.Count > 0)
            {
                Console.WriteLine($"Available fonts: {string.Join(", ", families)}");
            }
            else
            {
                Console.WriteLine("No fonts found. Use FontManager to add fonts.");
            }
        }
        else
        {
            Console.WriteLine($"Warning: Fonts folder not found: {fontsFolder}");
        }

        Console.WriteLine();

        if (args.Length > 0 && (args[0] == "--test" || args[0] == "-t"))
        {
            // TODO: Run tests
            Console.WriteLine("Tests not yet implemented.");
            return;
        }

        if (args.Length == 0)
        {
            Console.WriteLine("Usage: myalbum <album-file> [options]");
            Console.WriteLine("       myalbum --test | -t");
            return;
        }

        // TODO: Process album file
        Console.WriteLine($"Processing: {args[0]}");
    }
}
