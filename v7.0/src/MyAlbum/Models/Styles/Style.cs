using System.Xml.Linq;

namespace MyAlbum.Models.Styles;

/// <summary>
/// Single style definition
/// </summary>
public class Style
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public bool Default { get; set; } = false;
    public Dictionary<string, object> Properties { get; set; } = new();

    /// <summary>
    /// Parse styles from a styles XML element.
    /// </summary>
    public static List<Style> ParseStyles(XElement stylesElement)
    {
        var styles = new List<Style>();

        foreach (var el in stylesElement.Elements())
        {
            var type = el.Name.LocalName.ToLowerInvariant();
            var style = Parse(el, type);
            if (style != null)
                styles.Add(style);
        }

        return styles;
    }

    /// <summary>
    /// Parse a single style from an XML element.
    /// </summary>
    public static Style? Parse(XElement el, string type)
    {
        var name = (string?)el.Attribute("style");
        if (name == null) return null;

        var style = new Style
        {
            Name = name,
            Type = type,
            Default = ParseBool(el.Attribute("default")),
        };

        // All other attributes become properties
        foreach (var attr in el.Attributes())
        {
            var attrName = attr.Name.LocalName;
            if (attrName is "style" or "default") continue;
            style.Properties[attrName] = attr.Value;
        }

        // Child elements become nested property dictionaries
        foreach (var child in el.Elements())
        {
            var childName = child.Name.LocalName.ToLowerInvariant();
            var childProps = new Dictionary<string, string>();

            foreach (var attr in child.Attributes())
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
