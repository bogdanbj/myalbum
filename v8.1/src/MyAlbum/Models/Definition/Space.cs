using System.Xml.Linq;

namespace MyAlbum.Models.Definition;

/// <summary>
/// Space definition - empty space in layout
/// </summary>
public class Space : Element
{
    public double? Width { get; set; }
    public double? Height { get; set; }

    /// <summary>
    /// Create a Space from an XML element.
    /// </summary>
    public new static Space FromXml(XElement el)
    {
        return new Space
        {
            Style = (string?)el.Attribute("style"),
            Width = (double?)el.Attribute("width"),
            Height = (double?)el.Attribute("height"),
        };
    }
}
