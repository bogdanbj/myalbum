namespace MyAlbum.Models.Styles;

/// <summary>
/// Container for all style definitions, with lookup by name or type default
/// </summary>
public class StyleSheet
{
    private Dictionary<string, Style> _styles = new();
    private Dictionary<string, Style> _defaults = new();

    public void Add(Style style)
    {
        _styles[style.Name] = style;
        if (style.Default)
            _defaults[style.Type] = style;
    }

    public Style? Get(string name) =>
        _styles.GetValueOrDefault(name);

    public Style? GetDefault(string elementType) =>
        _defaults.GetValueOrDefault(elementType);

    public bool Contains(string name) =>
        _styles.ContainsKey(name);
    public IEnumerable<string> Names => _styles.Keys;
}
