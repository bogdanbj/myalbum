using System.Xml.Linq;

namespace MyAlbum.Utilities;

/// <summary>
/// Extension methods for case-insensitive XML element lookups.
/// </summary>
public static class XmlHelper
{
    /// <summary>
    /// Gets the first child element with the specified name, ignoring case.
    /// </summary>
    public static XElement? ElementIgnoreCase(this XElement parent, string name)
    {
        return parent.Elements()
            .FirstOrDefault(e => e.Name.LocalName.Equals(name, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Gets all child elements with the specified name, ignoring case.
    /// </summary>
    public static IEnumerable<XElement> ElementsIgnoreCase(this XElement parent, string name)
    {
        return parent.Elements()
            .Where(e => e.Name.LocalName.Equals(name, StringComparison.OrdinalIgnoreCase));
    }
}
