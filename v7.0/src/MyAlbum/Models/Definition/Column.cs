using System.Xml.Linq;

namespace MyAlbum.Models.Definition;

/// <summary>
/// Column definition - arranges children vertically
/// </summary>
public class Column : Element
{
    public string? Width { get; set; }
    public double[] Padding { get; set; } = [];
    public double Spacing { get; set; }
    public string? Align { get; set; }
    public string? BgColor { get; set; }
    public string? Rem { get; set; }
    public List<Element> Children { get; set; } = [];

    /// <summary>
    /// Create a Column from an XML element.
    /// </summary>
    public new static Column FromXml(XElement el)
    {
        return new Column
        {
            Style = (string?)el.Attribute("style"),
            Width = (string?)el.Attribute("width"),
            Spacing = (double?)el.Attribute("space") ?? (double?)el.Attribute("spacing") ?? 0,
            Align = (string?)el.Attribute("align"),
            BgColor = (string?)el.Attribute("bgcolor"),
            Padding = ParsePadding(el.Attribute("padding")),
            Rem = (string?)el.Attribute("rem"),
            Children = CreateChildrenFromXml(el),
        };
    }
}
