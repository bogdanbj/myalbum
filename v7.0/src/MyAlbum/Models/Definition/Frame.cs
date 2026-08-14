using System.Xml.Linq;

namespace MyAlbum.Models.Definition;

/// <summary>
/// Frame definition - decorative border element
/// </summary>
public class Frame : Element
{
    public double Width { get; set; }
    public double Height { get; set; }
    public double[] Lines { get; set; } = [];
    public double Padding { get; set; }
    public string? Color { get; set; }

    /// <summary>
    /// Create a Frame from an XML element.
    /// </summary>
    public new static Frame FromXml(XElement el)
    {
        return new Frame
        {
            Style = (string?)el.Attribute("style"),
            Width = (double?)el.Attribute("width") ?? 0,
            Height = (double?)el.Attribute("height") ?? 0,
            Lines = ParseDoubleArray(el.Attribute("lines")),
            Padding = (double?)el.Attribute("padding") ?? 0,
            Color = (string?)el.Attribute("color"),
        };
    }
}
