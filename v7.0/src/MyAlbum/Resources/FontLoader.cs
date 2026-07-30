using MyAlbum.Common;
using PdfSharp.Drawing;
using PdfSharp.Fonts;

namespace MyAlbum.Resources;

/// <summary>
/// Loads fonts from the Resources/Fonts folder using fonts.json mapping.
/// Implements PDFsharp's IFontResolver to provide custom font resolution.
/// </summary>
public class FontLoader : IFontResolver
{
    private readonly string _fontsFolder;
    private readonly FontMap _fontMap;
    private readonly Dictionary<string, byte[]> _fontDataCache = new();
    private readonly Dictionary<string, XFont> _fontCache = new();

    private static FontLoader? _instance;

    private FontLoader(string fontsFolder)
    {
        _fontsFolder = fontsFolder;
        var fontsJsonPath = Path.Combine(fontsFolder, "fonts.json");
        _fontMap = FontMap.Load(fontsJsonPath);
    }

    /// <summary>
    /// Initialize the FontLoader with the fonts folder path.
    /// Call this once at application startup.
    /// </summary>
    public static void Initialize(string fontsFolder)
    {
        _instance = new FontLoader(fontsFolder);
        GlobalFontSettings.FontResolver = _instance;
    }

    /// <summary>
    /// Get an XFont by family name, style, and size.
    /// Uses the fonts.json mapping to find the correct font file.
    /// </summary>
    public static XFont GetFont(string familyName, XFontStyleEx style, double size)
    {
        if (_instance == null)
            throw new InvalidOperationException("FontLoader not initialized. Call FontLoader.Initialize() first.");

        var key = $"{familyName}|{style}|{size}";

        if (!_instance._fontCache.TryGetValue(key, out var font))
        {
            font = new XFont(familyName, size, style);
            _instance._fontCache[key] = font;
        }

        return font;
    }

    /// <summary>
    /// Get all available font family names.
    /// </summary>
    public static IEnumerable<string> GetAvailableFamilies()
    {
        if (_instance == null)
            return Enumerable.Empty<string>();

        return _instance._fontMap.GetFamilyNames();
    }

    /// <summary>
    /// Check if a font family exists.
    /// </summary>
    public static bool HasFamily(string familyName)
    {
        if (_instance == null)
            return false;

        return _instance._fontMap.Fonts.ContainsKey(familyName);
    }

    #region IFontResolver Implementation

    public FontResolverInfo? ResolveTypeface(string familyName, bool isBold, bool isItalic)
    {
        // Determine the style
        var style = (isBold, isItalic) switch
        {
            (true, true) => Common.FontStyle.BoldItalic,
            (true, false) => Common.FontStyle.Bold,
            (false, true) => Common.FontStyle.Italic,
            _ => Common.FontStyle.Regular
        };

        // Get the font file from the map
        var filename = _fontMap.GetFontFile(familyName, style);

        // If exact style not found, try fallback to regular
        if (filename == null && style != Common.FontStyle.Regular)
        {
            filename = _fontMap.GetFontFile(familyName, Common.FontStyle.Regular);
        }

        if (filename == null)
            return null;

        // Use filename as the face name key
        return new FontResolverInfo(filename);
    }

    public byte[]? GetFont(string faceName)
    {
        // faceName is the filename from ResolveTypeface
        if (_fontDataCache.TryGetValue(faceName, out var cached))
            return cached;

        var filePath = Path.Combine(_fontsFolder, faceName);

        if (!File.Exists(filePath))
            return null;

        var data = File.ReadAllBytes(filePath);
        _fontDataCache[faceName] = data;
        return data;
    }

    #endregion
}
