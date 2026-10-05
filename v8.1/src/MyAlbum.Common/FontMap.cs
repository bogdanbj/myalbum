using System.Text.Json;
using System.Text.Json.Serialization;

namespace MyAlbum.Common;

/// <summary>
/// Represents the fonts.json file structure.
/// Maps font family names to their style variants (regular, bold, italic, boldItalic).
/// </summary>
public class FontMap
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    [JsonPropertyName("version")]
    public int Version { get; set; } = 1;

    [JsonPropertyName("fonts")]
    public Dictionary<string, FontFamily> Fonts { get; set; } = new();

    /// <summary>
    /// Load FontMap from a fonts.json file.
    /// </summary>
    public static FontMap Load(string path)
    {
        if (!File.Exists(path))
            return new FontMap();

        var json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<FontMap>(json, JsonOptions) ?? new FontMap();
    }

    /// <summary>
    /// Save FontMap to a fonts.json file.
    /// </summary>
    public void Save(string path)
    {
        var json = JsonSerializer.Serialize(this, JsonOptions);
        File.WriteAllText(path, json);
    }

    /// <summary>
    /// Get the filename for a font family and style.
    /// Returns null if not found.
    /// </summary>
    public string? GetFontFile(string familyName, FontStyle style)
    {
        if (!Fonts.TryGetValue(familyName, out var family))
            return null;

        return style switch
        {
            FontStyle.Regular => family.Regular,
            FontStyle.Bold => family.Bold,
            FontStyle.Italic => family.Italic,
            FontStyle.BoldItalic => family.BoldItalic,
            _ => family.Regular
        };
    }

    /// <summary>
    /// Add or update a font file for a family and style.
    /// </summary>
    public void SetFontFile(string familyName, FontStyle style, string filename)
    {
        if (!Fonts.TryGetValue(familyName, out var family))
        {
            family = new FontFamily();
            Fonts[familyName] = family;
        }

        switch (style)
        {
            case FontStyle.Regular:
                family.Regular = filename;
                break;
            case FontStyle.Bold:
                family.Bold = filename;
                break;
            case FontStyle.Italic:
                family.Italic = filename;
                break;
            case FontStyle.BoldItalic:
                family.BoldItalic = filename;
                break;
        }
    }

    /// <summary>
    /// Remove a font family from the map.
    /// </summary>
    public bool RemoveFamily(string familyName)
    {
        return Fonts.Remove(familyName);
    }

    /// <summary>
    /// Get all font family names.
    /// </summary>
    public IEnumerable<string> GetFamilyNames()
    {
        return Fonts.Keys.OrderBy(k => k);
    }
}

/// <summary>
/// Represents a font family with its style variants.
/// </summary>
public class FontFamily
{
    [JsonPropertyName("regular")]
    public string? Regular { get; set; }

    [JsonPropertyName("bold")]
    public string? Bold { get; set; }

    [JsonPropertyName("italic")]
    public string? Italic { get; set; }

    [JsonPropertyName("boldItalic")]
    public string? BoldItalic { get; set; }

    /// <summary>
    /// Get all non-null filenames in this family.
    /// </summary>
    [JsonIgnore]
    public IEnumerable<string> AllFiles
    {
        get
        {
            if (Regular != null) yield return Regular;
            if (Bold != null) yield return Bold;
            if (Italic != null) yield return Italic;
            if (BoldItalic != null) yield return BoldItalic;
        }
    }
}

/// <summary>
/// Font style variants.
/// </summary>
public enum FontStyle
{
    Regular,
    Bold,
    Italic,
    BoldItalic
}
