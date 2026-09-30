using MyAlbum.Models.Layout;
using MyAlbum.Models.Styles;
using MyAlbum.Utilities;
using System.Xml.Linq;

namespace MyAlbum.Models.Definition;

/// <summary>
/// Root album definition - parsed from .album file (XML format)
/// </summary>
public class Album
{
    public string? Version { get; set; }
    public string? Title { get; set; }
    public string? Author { get; set; }
    public string? Subject { get; set; }
    public string? StyleFile { get; set; }
    public List<Style> Styles { get; set; } = [];
    public List<Style> ExternalStyles { get; set; } = [];
    public List<Page> Pages { get; set; } = [];
    private string PageSpec { get; set; } = "all";
    private int TotalPages { get; set; }

    /// <summary>
    /// Create an Album from an XML document.
    /// </summary>
    public static Album FromXml(XElement root)
    {
        return FromXml(root, "all");
    }

    /// <summary>
    /// Create an Album from an XML document with page filtering.
    /// </summary>
    public static Album FromXml(XElement xRoot, string? pageSpec)
    {
        // Get the styles node if exists
        var xStyles = xRoot.ElementIgnoreCase("styles");

        // Create the album
        var album = new Album
        {
            Title = (string?)xRoot.Attribute("title"),
            Author = (string?)xRoot.Attribute("author"),
            Subject = (string?)xRoot.Attribute("subject"),
            Version = (string?)xRoot.Attribute("ver") ?? (string?)xRoot.Attribute("version"),
            StyleFile = (string?)xStyles?.Attribute("src") ?? (string?)xRoot.Attribute("styles"),
            PageSpec = pageSpec ?? "all"
        };

        // Create internally defined styles
        if (xStyles != null && xStyles.HasElements)
        {
            album.Styles = Style.ParseStyles(xStyles);
        }

        // Create pages with filtering
        var pageFilter = PageFilter.Parse(album.PageSpec);
        int pageNum = 0;
        foreach (var xPage in xRoot.ElementsIgnoreCase("page"))
        {
            if (pageFilter.Includes(++pageNum))
            {
                var page = Page.FromXml(xPage, pageNum);
                album.Pages.Add(page);
            }
        }
        album.TotalPages = pageNum;

        return album;
    }

    public void ParseExternalStyles(string baseFolder)
    {
        if (string.IsNullOrEmpty(StyleFile))
            return;

        var stylePath = Path.IsPathFullyQualified(StyleFile)
            ? StyleFile
            : Path.Combine(baseFolder, StyleFile);

        if (!File.Exists(stylePath))
            throw new FileNotFoundException($"Style file not found: {stylePath}");

        var styleDoc = XDocument.Parse(File.ReadAllText(stylePath));
        var styleRoot = styleDoc.Root
            ?? throw new InvalidOperationException("Style document has no root element.");
        ExternalStyles = Style.ParseStyles(styleRoot);
    }

    public void Print(string indent) 
    {
        Console.WriteLine($"Parsed: {Title ?? "(no title)"}");
        Console.WriteLine($"Style file: {StyleFile ?? "(none)"}");
        Console.WriteLine($"External styles: {ExternalStyles.Count}");
        Console.WriteLine($"Embedded styles: {Styles.Count}");
        foreach (var style in ExternalStyles.Concat(Styles))
        {
            var source = ExternalStyles.Contains(style) ? "ext" : "emb";
            var defaultTag = style.Default ? " (default)" : "";
            Console.WriteLine($"  [{source}] [{style.Type}] {style.Name}{defaultTag} - {style.Properties.Count} properties");
        }

        //var pageFilter = PageFilter.Parse(arguments.PageSpec ?? "all");
        //var filteredPages = pageFilter.Filter(Pages, p => p.Number).ToList();
        Console.WriteLine($"Pages: {Pages.Count} out of {TotalPages}" +
            (PageSpec != null ? $" (filter: {PageSpec})" : ""));

        foreach (var page in this.Pages)
        {
            Console.WriteLine();
            page.Print(indent);
            //Console.WriteLine($"  Page {page.Number}: {page.Title ?? "(untitled)"}" +
            //    (page.Style != null ? $" [style={page.Style}]" : "") +
            //    (page.Orientation != null ? $" [{page.Orientation}]" : "") +
            //    $" ({page.Children.Count} children)");
            //PrintElements(page.Children, "    ");
        }
    }
}
