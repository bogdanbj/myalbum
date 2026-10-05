using System.Xml.Linq;

namespace MyAlbum.Models.Definition;

/// <summary>
/// Abstract base for all album elements
/// </summary>
public abstract class Element
{
    public string? Style { get; set; }
    public double? X { get; set; }
    public double? Y { get; set; }

    /// <summary>
    /// Create an Element from an XML element, dispatching to the appropriate subtype.
    /// </summary>
    public static Element? FromXml(XElement el)
    {
        return el.Name.LocalName.ToLowerInvariant() switch
        {
            "row" => Row.FromXml(el),
            "column" => Column.FromXml(el),
            "stamp" => Stamp.FromXml(el),
            "text" => Text.FromXml(el),
            "image" => Image.FromXml(el),
            "frame" => Frame.FromXml(el),
            "space" => Space.FromXml(el),
            _ => null
        };
    }

    /// <summary>
    /// Create child Elements from a parent XML element.
    /// </summary>
    protected static List<Element> CreateChildrenFromXml(XElement parent)
    {
        var children = new List<Element>();

        foreach (var el in parent.Elements())
        {
            var element = FromXml(el);
            if (element != null)
                children.Add(element);
        }

        return children;
    }

    /// <summary>
    /// Parse a padding attribute into a double array.
    /// Supports 1, 2, or 4 values (CSS-style).
    /// </summary>
    protected static double[] ParsePadding(XAttribute? attr)
    {
        if (attr == null) return [];

        var parts = attr.Value.Split(',', ' ')
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .ToArray();

        var values = new List<double>();
        foreach (var part in parts)
        {
            if (double.TryParse(part.Trim(), out var val))
                values.Add(val);
        }

        return values.Count switch
        {
            1 or 2 or 4 => values.ToArray(),
            _ => []
        };
    }

    /// <summary>
    /// Parse an attribute into a double array.
    /// </summary>
    protected static double[] ParseDoubleArray(XAttribute? attr)
    {
        if (attr == null) return [];

        return attr.Value.Split(',', ' ')
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .Select(s => double.TryParse(s.Trim(), out var v) ? v : 0)
            .ToArray();
    }

    public void Print(string indent="")
    {
        Console.WriteLine($"{indent}Element: {GetType().Name}, Style: {Style}, X: {X}, Y: {Y}");
    }
}
