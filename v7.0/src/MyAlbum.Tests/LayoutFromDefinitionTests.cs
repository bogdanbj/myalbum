using System.Xml.Linq;
using PdfSharp.Drawing;
using Def = MyAlbum.Models.Definition;
using Lay = MyAlbum.Models.Layout;

namespace MyAlbum.Tests;

public class LayoutFromDefinitionTests
{
    private static Def.Styles CreateStyles(params Def.Style[] styles)
    {
        Def.Styles container = new();
        foreach (Def.Style style in styles)
            container.Add(style);
        return container;
    }

    // --- Styles.Find ---

    [Fact]
    public void Styles_Find_ReturnsStyleByName()
    {
        Def.Styles styles = CreateStyles(
            new() { Name = "heading", Type = "text" },
            new() { Name = "std", Type = "stamp", Default = true }
        );

        Assert.NotNull(styles.Find("heading", "text"));
        Assert.NotNull(styles.Find("std", "stamp"));
    }

    [Fact]
    public void Styles_Find_FallsBackToDefault()
    {
        Def.Styles styles = CreateStyles(
            new Def.Style { Name = "std", Type = "stamp", Default = true }
        );

        // No explicit style name, should find default
        Def.Style? found = styles.Find(null, "stamp");
        Assert.NotNull(found);
        Assert.Equal("std", found.Name);
    }

    // --- Page.FromDefinition ---

    [Fact]
    public void Page_FromDefinition_AppliesStyleSize()
    {
        Def.Style pageStyle = new()
        {
            Name = "wide",
            Type = "page",
            Properties = { ["size"] = "a4" }
        };
        Def.Styles styles = CreateStyles(pageStyle);

        Def.Page defPage = new() { Style = "wide" };
        Lay.Page page = Lay.Page.FromDefinition(defPage, styles);

        Assert.Equal("a4", page.Size);
    }

    [Fact]
    public void Page_FromDefinition_InlineSizeOverridesStyle()
    {
        Def.Style pageStyle = new()
        {
            Name = "wide",
            Type = "page",
            Properties = { ["size"] = "a4" }
        };
        Def.Styles styles = CreateStyles(pageStyle);

        Def.Page defPage = new() { Style = "wide", Size = "letter" };
        Lay.Page page = Lay.Page.FromDefinition(defPage, styles);

        Assert.Equal("letter", page.Size);
    }

    [Fact]
    public void Page_FromDefinition_AppliesStyleOrientation()
    {
        Def.Style pageStyle = new()
        {
            Name = "land",
            Type = "page",
            Properties = { ["orientation"] = "landscape" }
        };
        Def.Styles styles = CreateStyles(pageStyle);

        Def.Page defPage = new() { Style = "land" };
        Lay.Page page = Lay.Page.FromDefinition(defPage, styles);

        Assert.Equal("landscape", page.Orientation);
    }

    [Fact]
    public void Page_FromDefinition_AppliesDefaultStyle()
    {
        Def.Style defaultPage = new()
        {
            Name = "default-page",
            Type = "page",
            Default = true,
            Properties = { ["size"] = "legal" }
        };
        Def.Styles styles = CreateStyles(defaultPage);

        Def.Page defPage = new(); // No style specified
        Lay.Page page = Lay.Page.FromDefinition(defPage, styles);

        Assert.Equal("legal", page.Size);
    }

    [Fact]
    public void Page_FromDefinition_UsesBuiltinDefaults()
    {
        Def.Styles styles = new(); // No styles

        Def.Page defPage = new();
        Lay.Page page = Lay.Page.FromDefinition(defPage, styles);

        Assert.Equal("letter", page.Size);
        Assert.Equal("portrait", page.Orientation);
    }

    // --- Row.FromDefinition ---

    [Fact]
    public void Row_FromDefinition_AppliesStyleSpacing()
    {
        Def.Style rowStyle = new()
        {
            Name = "spaced",
            Type = "row",
            Properties = { ["spacing"] = "15" }
        };
        Def.Styles styles = CreateStyles(rowStyle);

        Def.Row defRow = new() { Style = "spaced" };
        Lay.Row row = Lay.Row.FromDefinition(defRow, styles);

        Assert.Equal(15.0, row.Spacing);
    }

    [Fact]
    public void Row_FromDefinition_AppliesStyleAlign()
    {
        Def.Style rowStyle = new()
        {
            Name = "centered",
            Type = "row",
            Properties = { ["align"] = "center", ["valign"] = "bottom" }
        };
        Def.Styles styles = CreateStyles(rowStyle);

        Def.Row defRow = new() { Style = "centered" };
        Lay.Row row = Lay.Row.FromDefinition(defRow, styles);

        Assert.Equal("center", row.Align);
        Assert.Equal("bottom", row.VAlign);
    }

    [Fact]
    public void Row_FromDefinition_InlineOverridesStyle()
    {
        Def.Style rowStyle = new()
        {
            Name = "std",
            Type = "row",
            Properties = { ["spacing"] = "10", ["align"] = "left" }
        };
        Def.Styles styles = CreateStyles(rowStyle);

        Def.Row defRow = new() { Style = "std", Spacing = "20", Align = "right" };
        Lay.Row row = Lay.Row.FromDefinition(defRow, styles);

        Assert.Equal(20.0, row.Spacing);
        Assert.Equal("right", row.Align);
    }

    // --- Column.FromDefinition ---

    [Fact]
    public void Column_FromDefinition_AppliesStyleWidth()
    {
        Def.Style colStyle = new()
        {
            Name = "narrow",
            Type = "column",
            Properties = { ["width"] = "50" }
        };
        Def.Styles styles = CreateStyles(colStyle);

        Def.Column defCol = new() { Style = "narrow" };
        Lay.Column col = Lay.Column.FromDefinition(defCol, styles);

        Assert.Equal(50.0, col.Width);
    }

    // --- Stamp.FromDefinition ---

    [Fact]
    public void Stamp_FromDefinition_AppliesStyleDimensions()
    {
        Def.Style stampStyle = new()
        {
            Name = "small",
            Type = "stamp",
            Properties = { ["width"] = "25", ["height"] = "30" }
        };
        Def.Styles styles = CreateStyles(stampStyle);

        Def.Stamp defStamp = new() { Style = "small" };
        Lay.Stamp stamp = Lay.Stamp.FromDefinition(defStamp, styles);

        Assert.Equal(25.0, stamp.Width);
        Assert.Equal(30.0, stamp.Height);
    }

    [Fact]
    public void Stamp_FromDefinition_InlineOverridesStyle()
    {
        Def.Style stampStyle = new()
        {
            Name = "small",
            Type = "stamp",
            Default = true,
            Properties = { ["width"] = "25", ["height"] = "30" }
        };
        Def.Styles styles = CreateStyles(stampStyle);

        Def.Stamp defStamp = new() { Width = 40, Height = 50 };
        Lay.Stamp stamp = Lay.Stamp.FromDefinition(defStamp, styles);

        Assert.Equal(40.0, stamp.Width);
        Assert.Equal(50.0, stamp.Height);
    }

    // --- Text.FromDefinition ---

    [Fact]
    public void Text_FromDefinition_AppliesStyleFont()
    {
        Def.Style textStyle = new()
        {
            Name = "heading",
            Type = "text",
            Properties = { ["font-name"] = "Arial", ["font-size"] = "14" }
        };
        Def.Styles styles = CreateStyles(textStyle);

        Def.Text defText = new() { Content = "Hello", Style = "heading" };
        Lay.Text text = Lay.Text.FromDefinition(defText, styles);

        Assert.Equal("Arial", text.FontName);
        Assert.Equal(14.0, text.FontSize);
    }

    [Fact]
    public void Text_FromDefinition_AppliesStyleColor()
    {
        Def.Style textStyle = new()
        {
            Name = "red-text",
            Type = "text",
            Properties = { ["color"] = "#FF0000" }
        };
        Def.Styles styles = CreateStyles(textStyle);

        Def.Text defText = new() { Content = "Red", Style = "red-text" };
        Lay.Text text = Lay.Text.FromDefinition(defText, styles);

        Assert.Equal(XColors.Red.R, text.Color.R);
        Assert.Equal(XColors.Red.G, text.Color.G);
        Assert.Equal(XColors.Red.B, text.Color.B);
    }

    [Fact]
    public void Text_FromDefinition_InlineColorOverridesStyle()
    {
        Def.Style textStyle = new()
        {
            Name = "red-text",
            Type = "text",
            Properties = { ["color"] = "#FF0000" }
        };
        Def.Styles styles = CreateStyles(textStyle);

        Def.Text defText = new() { Content = "Green", Style = "red-text", Color = "#00FF00" };
        Lay.Text text = Lay.Text.FromDefinition(defText, styles);

        Assert.Equal(XColors.Lime.R, text.Color.R);
        Assert.Equal(XColors.Lime.G, text.Color.G);
        Assert.Equal(XColors.Lime.B, text.Color.B);
    }

    // --- Image.FromDefinition ---

    [Fact]
    public void Image_FromDefinition_AppliesStyleScaleMode()
    {
        Def.Style imgStyle = new()
        {
            Name = "fit-img",
            Type = "image",
            Properties = { ["scale-mode"] = "fit" }
        };
        Def.Styles styles = CreateStyles(imgStyle);

        Def.Image defImg = new() { Src = "test.png", Style = "fit-img" };
        Lay.Image img = Lay.Image.FromDefinition(defImg, styles);

        Assert.Equal("fit", img.ScaleMode);
    }

    // --- Frame.FromDefinition ---

    [Fact]
    public void Frame_FromDefinition_AppliesStyleDimensions()
    {
        Def.Style frameStyle = new()
        {
            Name = "standard-frame",
            Type = "frame",
            Properties = { ["width"] = "80", ["height"] = "100" }
        };
        Def.Styles styles = CreateStyles(frameStyle);

        Def.Frame defFrame = new() { Style = "standard-frame" };
        Lay.Frame frame = Lay.Frame.FromDefinition(defFrame, styles);

        Assert.Equal(80.0, frame.Width);
        Assert.Equal(100.0, frame.Height);
    }

    // --- Space.FromDefinition ---

    [Fact]
    public void Space_FromDefinition_AppliesStyleDimensions()
    {
        Def.Style spaceStyle = new()
        {
            Name = "spacer",
            Type = "space",
            Properties = { ["width"] = "10", ["height"] = "20" }
        };
        Def.Styles styles = CreateStyles(spaceStyle);

        Def.Space defSpace = new() { Style = "spacer" };
        Lay.Space space = Lay.Space.FromDefinition(defSpace, styles);

        Assert.Equal(10.0, space.Width);
        Assert.Equal(20.0, space.Height);
    }

    // --- Album.FromDefinition Integration ---

    private static Def.Album LoadCanada6()
    {
        string templatesDir = System.Configuration.ConfigurationManager.AppSettings["InputFolder"]
            ?? @"C:\My\Git\myalbum\v7.0\Templates";

        string filePath = Path.Combine(templatesDir, "Canada6.album");
        string xml = File.ReadAllText(filePath);
        Def.Album album = Def.Album.FromXml(XDocument.Parse(xml).Root!);

        // Load external styles if referenced
        if (!string.IsNullOrEmpty(album.StyleFile))
        {
            string stylePath = Path.Combine(templatesDir, album.StyleFile);
            if (File.Exists(stylePath))
            {
                string stylesXml = File.ReadAllText(stylePath);
                album.ExternalStyles = Def.Styles.FromXml(XDocument.Parse(stylesXml).Root!);
            }
        }

        return album;
    }

    private static Def.Styles BuildStyles(Def.Album album)
    {
        Def.Styles styles = new();
        styles.Merge(album.ExternalStyles);
        styles.Merge(album.Styles);
        return styles;
    }

    [Fact]
    public void Album_FromDefinition_Canada6_ProducesLayoutAlbum()
    {
        Def.Album defAlbum = LoadCanada6();
        Def.Styles styles = BuildStyles(defAlbum);

        Lay.Album layout = Lay.Album.FromDefinition(defAlbum, styles);

        Assert.Equal(37, layout.Pages.Count);
    }

    [Fact]
    public void Album_FromDefinition_Canada6_PagesHaveResolvedSizeAndOrientation()
    {
        Def.Album defAlbum = LoadCanada6();
        Def.Styles styles = BuildStyles(defAlbum);

        Lay.Album layout = Lay.Album.FromDefinition(defAlbum, styles);

        foreach (Lay.Page page in layout.Pages)
        {
            Assert.False(string.IsNullOrEmpty(page.Size), $"Page {page.Number} has no size");
            Assert.False(string.IsNullOrEmpty(page.Orientation), $"Page {page.Number} has no orientation");
        }
    }
}
