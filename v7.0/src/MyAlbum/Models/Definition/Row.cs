using System.Xml.Linq;

namespace MyAlbum.Models.Definition;

/// <summary>
/// Row definition - arranges children horizontally
/// </summary>
public class Row : Element
{
    public double[] Padding { get; set; } = [];
    public string? Spacing { get; set; }
    public string? Align { get; set; }
    public string? VAlign { get; set; }
    public double? Height { get; set; }
    public string? BgColor { get; set; }
    public string? Rem { get; set; }
    public List<Element> Children { get; set; } = [];

    /// <summary>
    /// Create a Row from an XML element.
    /// </summary>
    public new static Row FromXml(XElement el)
    {
        return new Row
        {
            Style = (string?)el.Attribute("style"),
            Spacing = (string?)el.Attribute("space") ?? (string?)el.Attribute("spacing"),
            Align = (string?)el.Attribute("align"),
            VAlign = (string?)el.Attribute("valign"),
            Height = (double?)el.Attribute("height"),
            BgColor = (string?)el.Attribute("bgcolor"),
            Padding = ParsePadding(el.Attribute("padding")),
            Rem = (string?)el.Attribute("rem"),
            Children = CreateChildrenFromXml(el),
        };
    }
}
