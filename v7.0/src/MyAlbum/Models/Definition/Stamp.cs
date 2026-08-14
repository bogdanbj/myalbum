using System.Xml.Linq;

namespace MyAlbum.Models.Definition;

/// <summary>
/// Stamp definition - composite element with frame, title, interior, footer
/// </summary>
public class Stamp : Element
{
    public double Width { get; set; }
    public double Height { get; set; }
    public string? Title { get; set; }
    public double TitlePadding { get; set; }
    public string? Image { get; set; }
    public string? I1 { get; set; }
    public string? I2 { get; set; }
    public string? I3 { get; set; }
    public string? F1 { get; set; }
    public string? F2 { get; set; }
    public string? F3 { get; set; }
    public double FooterPadding { get; set; }

    /// <summary>
    /// Create a Stamp from an XML element.
    /// </summary>
    public new static Stamp FromXml(XElement el)
    {
        return new Stamp
        {
            Style = (string?)el.Attribute("style"),
            Width = (double?)el.Attribute("width") ?? 0,
            Height = (double?)el.Attribute("height") ?? 0,
            Title = (string?)el.Attribute("title"),
            TitlePadding = (double?)el.Attribute("title_padding") ?? 0,
            Image = (string?)el.Attribute("image"),
            I1 = (string?)el.Attribute("i1"),
            I2 = (string?)el.Attribute("i2"),
            I3 = (string?)el.Attribute("i3"),
            F1 = (string?)el.Attribute("f1"),
            F2 = (string?)el.Attribute("f2"),
            F3 = (string?)el.Attribute("f3"),
            FooterPadding = (double?)el.Attribute("footer_padding") ?? 0,
        };
    }
}
