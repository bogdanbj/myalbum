using System.Xml.Linq;

namespace MyAlbum.Models.Definition;

/// <summary>
/// Container for all style definitions, with lookup by name or type default
/// </summary>
public class Styles
{
    private readonly Dictionary<string, Style> _styles = new();
    private readonly Dictionary<string, Style> _defaults = new();

    /// <summary>
    /// Parse styles from an XML element.
    /// </summary>
    public static Styles FromXml(XElement xStyles)
    {
        Styles styles = new();

        foreach (XElement xElement in xStyles.Elements())
        {
            string type = xElement.Name.LocalName.ToLowerInvariant();
            Style? style = Style.FromXml(xElement, type);
            if (style != null)
                styles.Add(style);
        }

        return styles;
    }

    public void Add(Style style)
    {
        _styles[style.Name] = style;
        if (style.Default)
            _defaults[style.Type] = style;
    }

    /// <summary>
    /// Merge another Styles container into this one.
    /// Styles from the other container override existing styles with the same name.
    /// </summary>
    public void Merge(Styles other)
    {
        foreach (Style style in other.All)
            Add(style);
    }

    public Style? Get(string name) =>
        _styles.GetValueOrDefault(name);

    public Style? GetDefault(string elementType) =>
        _defaults.GetValueOrDefault(elementType);

    /// <summary>
    /// Find a style by name, or fall back to the default style for the element type.
    /// </summary>
    public Style? Find(string? styleName, string elementType)
    {
        if (styleName != null)
            return Get(styleName);

        return GetDefault(elementType);
    }

    public bool Contains(string name) =>
        _styles.ContainsKey(name);

    public int Count => _styles.Count;

    public IEnumerable<string> Names => _styles.Keys;

    public IEnumerable<Style> All => _styles.Values;
}
