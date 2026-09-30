using PdfSharpCore.Fonts;
using SixLabors.Fonts;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

namespace MyAlbum.Utilities
{
    internal class MyFontResolver : IFontResolver
    {
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

        public byte[] GetFont(string faceName)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($">>> GetFont called with: {faceName}");

                if (!FontMap.TryGetValue(faceName, out string? fileName))
                {
                    Console.WriteLine($"Font '{faceName}' not found in FontMap. Using fallback: {DefaultFontName}");
                    if (!FontMap.TryGetValue(DefaultFontName, out fileName))
                        throw new FontException($"❌ Default font '{DefaultFontName}' not found in FontMap");
                }

                string? fontPath = ConfigurationManager.AppSettings["FontPath"];

                if (!string.IsNullOrWhiteSpace(fontPath))
                {
                    if (!Path.IsPathRooted(fontPath))
                    {
                        fontPath = Path.Combine(Directory.GetCurrentDirectory(), fontPath);
                    }
                }
                else
                {
                    fontPath = Path.Combine(Environment.CurrentDirectory, "Fonts");
                }

                if (!Directory.Exists(fontPath))
                    throw new DirectoryNotFoundException($"❌ Font directory not found: {fontPath}");

                string fullPath = Path.Combine(fontPath, fileName);

                if (!File.Exists(fullPath))
                    throw new FileNotFoundException($"❌ Font file not found: {fullPath}");

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
