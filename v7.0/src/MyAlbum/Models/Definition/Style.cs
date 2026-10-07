using System.Xml.Linq;

namespace MyAlbum.Models.Definition;

/// <summary>
/// Single style definition
/// </summary>
public class Style
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public bool Default { get; set; } = false;
    public Dictionary<string, object> Properties { get; set; } = new();

    // --- Property extraction methods ---

    /// <summary>
    /// Get a string property value, or null if not found.
    /// </summary>
    public string? GetString(string key)
    {
        return Properties.TryGetValue(key, out object? val) && val is string s ? s : null;
    }

    /// <summary>
    /// Get a double property value, or 0 if not found.
    /// </summary>
    public double GetDouble(string key)
    {
        string? s = GetString(key);
        return s != null && double.TryParse(s, out double v) ? v : 0;
    }

    /// <summary>
    /// Get a nullable double property value, or null if not found.
    /// </summary>
    public double? GetDoubleOrNull(string key)
    {
        string? s = GetString(key);
        return s != null && double.TryParse(s, out double v) ? v : null;
    }

    /// <summary>
    /// Get a padding property (1, 2, or 4 values), or empty array if not found.
    /// </summary>
    public double[] GetPadding(string key)
    {
        string? s = GetString(key);
        if (s == null) return [];

        double[] parts = s.Split(',', ' ')
            .Where(p => !string.IsNullOrWhiteSpace(p))
            .Select(p => double.TryParse(p.Trim(), out double v) ? v : 0)
            .ToArray();

        return parts.Length is 1 or 2 or 4 ? parts : [];
    }

    /// <summary>
    /// Get a double array property (comma or space separated), or empty array if not found.
    /// </summary>
    public double[] GetDoubleArray(string key)
    {
        string? s = GetString(key);
        if (s == null) return [];

        return s.Split(',', ' ')
            .Where(p => !string.IsNullOrWhiteSpace(p))
            .Select(p => double.TryParse(p.Trim(), out double v) ? v : 0)
            .ToArray();
    }

    // --- XML parsing ---

    /// <summary>
    /// Parse a single style from an XML element.
    /// </summary>
    public static Style? FromXml(XElement xElement, string type)
    {
        string? name = (string?)xElement.Attribute("style");
        if (name == null) return null;

        Style style = new()
        {
            Name = name,
            Type = type,
            Default = ParseBool(xElement.Attribute("default")),
        };

        // All other attributes become properties
        foreach (XAttribute attr in xElement.Attributes())
        {
            string attrName = attr.Name.LocalName;
            if (attrName is "style" or "default") continue;
            style.Properties[attrName] = attr.Value;
        }

        // Child elements become nested property dictionaries
        foreach (XElement xChild in xElement.Elements())
        {
            string childName = xChild.Name.LocalName.ToLowerInvariant();
            Dictionary<string, string> childProps = new();

            foreach (XAttribute attr in xChild.Attributes())
                childProps[attr.Name.LocalName] = attr.Value;

            style.Properties[childName] = childProps;
        }

        return style;
    }

    private static bool ParseBool(XAttribute? attr)
    {
        if (attr == null) return false;
        return attr.Value.Equals("true", StringComparison.OrdinalIgnoreCase);
    }
}
