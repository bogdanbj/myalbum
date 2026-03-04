using PdfSharpCore.Fonts;
using SixLabors.Fonts;
using System.Configuration;
using System.Diagnostics;
using System.Runtime.CompilerServices;


namespace MyAlbum.Utils
{
    internal class MyFontResolver : IFontResolver
    {
        private static readonly string CachedFontPath;

        //static MyFontResolver()
        //{
        //    //System.Diagnostics.Debug.WriteLine(">>> MyFontResolver static constructor called");
        //    //string? fontPath = ConfigurationManager.AppSettings["FontPath"];

        //    //if (!string.IsNullOrWhiteSpace(fontPath))
        //    //{
        //    //    if (!Path.IsPathRooted(fontPath))
        //    //        fontPath = Path.Combine(Directory.GetCurrentDirectory(), fontPath);
        //    //}
        //    //else
        //    //{
        //    //    fontPath = Path.Combine(Environment.CurrentDirectory, "Fonts");
        //    //}

        //    //if (!Directory.Exists(fontPath))
        //    //    throw new DirectoryNotFoundException(
        //    //        $"❌ Font directory not found: {fontPath}\n" +
        //    //        $"Please ensure the FontPath setting in app.config is correct or that a 'Fonts' folder exists.");

        //    //CachedFontPath = fontPath;
        //}

        public string DefaultFontName => "Verdana";

        private static readonly Dictionary<string, string> FontMap = new()
        {
            // Arial
            { "Arial",                      @"Arial Regular.ttf" },
            { "Arial Bold",                 @"Arial Bold.ttf" },
            { "Arial Italic",               @"Arial Italic.ttf" },
            { "Arial Bold Italic",          @"Arial Bold Italic.ttf" },

            // Century Gothic
            { "Century Gothic",             @"Century Gothic Regular.ttf" },
            { "Century Gothic Bold",        @"Century Gothic Bold.ttf" },
            { "Century Gothic Italic",      @"Century Gothic Italic.ttf" },
            { "Century Gothic Bold Italic", @"Century Gothic Bold Italic.ttf" },

            // Folio
            { "Folio",                      @"Folio BT Book.ttf" },
            { "Folio Bold",                 @"Folio BT Bold.ttf" },

            // Garamond
            { "Garamond",                   @"Garamond Regular.ttf" },
            { "Garamond Bold",              @"Garamond Bold.ttf" },
            { "Garamond Italic",            @"Garamond Italic.ttf" },
            { "Garamond Bold Italic",       @"Garamond Bold Italic.ttf" },

            // Lubalin Graph
            { "Lubalin Graph",              @"Lubalin Graph Regular.ttf" },
            { "Lubalin Graph Bold",         @"Lubalin Graph Bold.ttf" },
            { "Lubalin Graph Italic",       @"Lubalin Graph Italic.ttf" },
            { "Lubalin Graph Bold Italic",  @"Lubalin Graph Bold Italic.ttf" },

            // Stymie Becker
            { "Stymie Becker",              @"Stymie Becker.ttf" },
            { "Stymie Becker Light",        @"Stymie Becker Light.ttf" },
            { "Stymie Becker Light Italic", @"Stymie Becker Light Italic.otf" },

            // Verdana
            { "Verdana",                    @"Verdana.ttf" },
            { "Verdana Bold",               @"Verdana Bold.ttf" },
            { "Verdana Italic",             @"Verdana Italic.ttf" },
            { "Verdana Bold Italic",        @"Verdana Bold Italic.ttf" },
        };

        //public byte[] GetFonts(string faceName)
        //{
        //    System.Diagnostics.Debugger.Break();  // ← ADD THIS LINE
        //    System.Diagnostics.Debug.WriteLine($">>> GetFont called with: {faceName}");

        //    // Get the font file name.
        //    if (!FontMap.TryGetValue(faceName, out string? fileName))
        //    {
        //        Console.WriteLine($"⚠️  Font '{faceName}' not found in FontMap. Using fallback: {DefaultFontName}");
        //        if (!FontMap.TryGetValue(DefaultFontName, out fileName))
        //            throw new FontException($"❌ Default font '{DefaultFontName}' not found in FontMap");
        //    }

        //    // Get the fonts path
        //    string? fontPath = ConfigurationManager.AppSettings["FontPath"];
        //    //String fileName = Path.Combine(Directory.GetCurrentDirectory(), args[0]);
        //    if (!string.IsNullOrWhiteSpace(fontPath))
        //    {
        //        // if it's not an absolute path, combine it with the current directory
        //        if (!Path.IsPathRooted(fontPath))
        //            //fontPath = Path.Combine(Environment.CurrentDirectory, fontPath);
        //            fontPath = Path.Combine(Directory.GetCurrentDirectory(), fontPath);
        //    }
        //    else
        //    {
        //        //default to "Fonts" folder in the current directory
        //        fontPath = Path.Combine(Environment.CurrentDirectory, "Fonts");
        //    }
            
        //    // Check the fonts path exists
        //    if (!Directory.Exists(fontPath))
        //        throw new DirectoryNotFoundException($"❌ Font directory not found: {fontPath}");

        //    // Check the file font exists
        //    string fullPath = Path.Combine(fontPath, fileName);
        //    if (!File.Exists(fullPath))
        //        throw new FileNotFoundException($"❌ Font file not found: {fullPath}");

        //    // Get the content of the font file
        //    return File.ReadAllBytes(fullPath);
        //}
        [MethodImpl(MethodImplOptions.NoInlining)]
        public byte[] GetFont(string faceName)
        {
            try
            {
                //Debug.Assert(false, "Breakpoint: GetFont called");  // ← This should work
                System.Diagnostics.Debug.WriteLine($">>> GetFont called with: {faceName}");

                if (!FontMap.TryGetValue(faceName, out string? fileName))
                {
                    Console.WriteLine($"⚠️  Font '{faceName}' not found in FontMap. Using fallback: {DefaultFontName}");
                    if (!FontMap.TryGetValue(DefaultFontName, out fileName))
                        throw new FontException($"❌ Default font '{DefaultFontName}' not found in FontMap");
                }

                string? fontPath = ConfigurationManager.AppSettings["FontPath"];
                //System.Diagnostics.Debug.WriteLine($">>> FontPath from config: {fontPath}");

                if (!string.IsNullOrWhiteSpace(fontPath))
                {
                    //fontPath = Path.Combine(Directory.GetCurrentDirectory(), ConfigurationManager.AppSettings["FontPath"]);


                    if (!Path.IsPathRooted(fontPath))
                    {
                        //System.Diagnostics.Debug.WriteLine($">>> FontPath is not Rooted");
                        fontPath = Path.Combine(Directory.GetCurrentDirectory(), fontPath);
                        //System.Diagnostics.Debug.WriteLine($">>> FontPath from GetCurrentDirectory: {fontPath}");
                    }
                    //else
                        //System.Diagnostics.Debug.WriteLine($">>> FontPath is Rooted");
                }
                else
                {
                    fontPath = Path.Combine(Environment.CurrentDirectory, "Fonts");
                }

                //System.Diagnostics.Debug.WriteLine($">>> Resolved fontPath: {fontPath}");
                //System.Diagnostics.Debug.WriteLine($">>> Absolute fontPath: {Path.GetFullPath(fontPath)}");
                //System.Diagnostics.Debug.WriteLine($">>> Directory.Exists: {Directory.Exists(fontPath)}");

                if (!Directory.Exists(fontPath))
                    throw new DirectoryNotFoundException($"❌ Font directory not found: {fontPath}");

                string fullPath = Path.Combine(fontPath, fileName);
                //System.Diagnostics.Debug.WriteLine($">>> Full path: {fullPath}");
                //System.Diagnostics.Debug.WriteLine($">>> File.Exists: {File.Exists(fullPath)}");

                if (!File.Exists(fullPath))
                    throw new FileNotFoundException($"❌ Font file not found: {fullPath}");
                //System.Diagnostics.Debug.WriteLine($">>> Full path: {fullPath}");

                byte[] data = File.ReadAllBytes(fullPath);
                System.Diagnostics.Debug.WriteLine($">>> Successfully read {data.Length} bytes");
                return data;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"!!! EXCEPTION IN GetFont: {ex.GetType().Name}: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"!!! Stack: {ex.StackTrace}");
                throw;  // Re-throw to see what PdfSharpCore does with it
            }
        }

        public FontResolverInfo ResolveTypeface(string familyName, bool isBold, bool isItalic)
        {
            //System.Diagnostics.Debugger.Break();  // ← ADD THIS LINE
            //System.Diagnostics.Debug.WriteLine($">>> ResolveTypeface called: {familyName}, Bold={isBold}, Italic={isItalic}");

            string suffix = (isBold, isItalic) switch
            {
                (true, true) => " Bold Italic",
                (true, false) => " Bold",
                (false, true) => " Italic",
                _ => ""
            };

            return new FontResolverInfo($"{familyName}{suffix}");
        }
    }
}
