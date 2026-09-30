using System.Xml.Linq;

namespace MyAlbum.Models.Definition;

/// <summary>
/// Text definition - text block with alignment and wrapping
/// </summary>
public class Text : Element
{
    public string Content { get; set; } = string.Empty;
    public string? Width { get; set; }
    public string? Align { get; set; }
    public string? FontName { get; set; }
    public double? FontSize { get; set; }
    public string? FontStyle { get; set; }
    public string? Color { get; set; }
    public string? BgColor { get; set; }
    public string? Margin { get; set; }

    /// <summary>
    /// Create a Text from an XML element.
    /// </summary>
    public new static Text FromXml(XElement el)
    {
        return new Text
        {
            Style = (string?)el.Attribute("style"),
            Content = el.Value.Trim(),
            Width = (string?)el.Attribute("width"),
            Align = (string?)el.Attribute("align"),
            FontName = (string?)el.Attribute("font_name") ?? (string?)el.Attribute("font"),
            FontSize = (double?)el.Attribute("font_size"),
            FontStyle = (string?)el.Attribute("font_style"),
            Color = (string?)el.Attribute("color"),
            BgColor = (string?)el.Attribute("bgcolor"),
            Margin = (string?)el.Attribute("margin"),
        };
    }
}
