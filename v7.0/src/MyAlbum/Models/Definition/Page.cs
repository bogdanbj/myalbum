using MyAlbum.Models.Layout;
using System.Xml.Linq;

namespace MyAlbum.Models.Definition;

/// <summary>
/// Page definition - a single album page
/// </summary>
public class Page
{
    /// <summary>
    /// Page number based on position in the album file (1-based).
    /// </summary>
    public int Number { get; set; }

    /// <summary>
    /// The "no" attribute from the album file. For debugging only - not used in processing.
    /// </summary>
    public int? No { get; set; }

    public string? Title { get; set; }
    public string? Style { get; set; }
    public string? Size { get; set; }
    public string? Orientation { get; set; }
    public double[] Padding { get; set; } = [];
    public double RowSpacing { get; set; }
    public double ColumnSpacing { get; set; }
    public List<Element> Children { get; set; } = [];

    /// <summary>
    /// Create a Page from an XML element.
    /// </summary>
    public static Page FromXml(XElement el, int pageNumber)
    {
        return new Page
        {
            Number = pageNumber,
            No = (int?)el.Attribute("no"),
            Title = (string?)el.Attribute("title"),
            Style = (string?)el.Attribute("style"),
            Size = (string?)el.Attribute("size"),
            Orientation = (string?)el.Attribute("orientation"),
            Padding = ParsePaddingString(el.Attribute("padding")?.Value),
            RowSpacing = (double?)el.Attribute("row_spacing") ?? 0,
            ColumnSpacing = (double?)el.Attribute("column_spacing") ?? 0,
            Children = CreateChildrenFromXml(el),
        };
    }

    private static List<Element> CreateChildrenFromXml(XElement parent)
    {
        var children = new List<Element>();

        foreach (var childEl in parent.Elements())
        {
            var element = Element.FromXml(childEl);
            if (element != null)
                children.Add(element);
        }

        return children;
    }

    private static double[] ParsePaddingString(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return [];

        var parts = value.Split(',', ' ')
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

    public void Print(string indent)
    {
        Console.WriteLine($"{indent}Page: {Number}, Title: {"\"" + Title + "\"" ?? "(untitled)"}, Style: {Style ?? "(none)"}");
        indent += "  ";
        Console.WriteLine($"{indent}Size: {Size ?? "(none)"}, Orientation: {Orientation ?? "(none)"}");
        Console.WriteLine($"{indent}Padding: {string.Join(", ", Padding)}, RowSpacing: {RowSpacing}, ColumnSpacing: {ColumnSpacing}");
        Console.WriteLine($"{indent}Children: {Children.Count}");
        foreach (var child in Children)
        {
            child.Print(indent + "  ");
        }

    }
}
