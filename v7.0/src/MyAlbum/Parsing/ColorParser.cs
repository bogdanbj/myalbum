using PdfSharp.Drawing;

namespace MyAlbum.Parsing;

/// <summary>
/// Parses color strings into XColor values.
/// Supports named colors (e.g., "Red", "CadetBlue") and RGB tuples (e.g., "48,48,48").
/// </summary>
public static class ColorParser
{
    public static XColor Parse(string value)
    {
        var trimmed = value.Trim();

        // Try RGB tuple: "r,g,b"
        var parts = trimmed.Split(',');
        if (parts.Length == 3
            && int.TryParse(parts[0].Trim(), out var r)
            && int.TryParse(parts[1].Trim(), out var g)
            && int.TryParse(parts[2].Trim(), out var b))
        {
            return XColor.FromArgb(r, g, b);
        }

        // Try named color via reflection on XColors
        var prop = typeof(XColors).GetProperty(trimmed,
            System.Reflection.BindingFlags.Public
            | System.Reflection.BindingFlags.Static
            | System.Reflection.BindingFlags.IgnoreCase);

        if (prop != null)
            return (XColor)prop.GetValue(null)!;

        // Fallback
        return XColors.Black;
    }
}
