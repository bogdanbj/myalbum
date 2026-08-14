using System.Xml.Linq;

namespace MyAlbum.Models.Definition;

/// <summary>
/// Image definition - renders an image from a file
/// </summary>
public class Image : Element
{
    public string Src { get; set; } = string.Empty;
    public string? Width { get; set; }
    public double? Height { get; set; }
    public string? ScaleMode { get; set; }

    /// <summary>
    /// Create an Image from an XML element.
    /// </summary>
    public new static Image FromXml(XElement el)
    {
        return new Image
        {
            Style = (string?)el.Attribute("style"),
            Src = (string?)el.Attribute("src") ?? (string?)el.Attribute("file_name") ?? string.Empty,
            Width = (string?)el.Attribute("width"),
            Height = (double?)el.Attribute("height"),
            ScaleMode = (string?)el.Attribute("scale_mode"),
        };
    }
}
