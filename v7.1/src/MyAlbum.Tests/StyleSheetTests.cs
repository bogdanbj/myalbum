using MyAlbum.Models.Styles;

namespace MyAlbum.Tests;

public class StyleSheetTests
{
    [Fact]
    public void Get_ReturnsAddedStyle()
    {
        var sheet = new StyleSheet();
        var style = new Style { Name = "heading", Type = "text" };
        sheet.Add(style);

        Assert.Same(style, sheet.Get("heading"));
    }

    [Fact]
    public void Get_ReturnsNull_WhenNotFound()
    {
        var sheet = new StyleSheet();
        Assert.Null(sheet.Get("nonexistent"));
    }

    [Fact]
    public void GetDefault_ReturnsDefaultForType()
    {
        var sheet = new StyleSheet();
        var style = new Style { Name = "std", Type = "stamp", Default = true };
        sheet.Add(style);

        Assert.Same(style, sheet.GetDefault("stamp"));
    }

    [Fact]
    public void GetDefault_ReturnsNull_WhenNoDefault()
    {
        var sheet = new StyleSheet();
        var style = new Style { Name = "custom", Type = "stamp", Default = false };
        sheet.Add(style);

        Assert.Null(sheet.GetDefault("stamp"));
    }

    [Fact]
    public void Contains_ReturnsTrueForExisting()
    {
        var sheet = new StyleSheet();
        sheet.Add(new Style { Name = "test", Type = "text" });

        Assert.True(sheet.Contains("test"));
        Assert.False(sheet.Contains("other"));
    }

    [Fact]
    public void Names_ReturnsAllStyleNames()
    {
        var sheet = new StyleSheet();
        sheet.Add(new Style { Name = "a", Type = "text" });
        sheet.Add(new Style { Name = "b", Type = "stamp" });

        Assert.Equal(["a", "b"], sheet.Names.OrderBy(n => n).ToArray());
    }

    [Fact]
    public void Add_OverwritesExistingStyleWithSameName()
    {
        var sheet = new StyleSheet();
        var first = new Style { Name = "x", Type = "text" };
        var second = new Style { Name = "x", Type = "stamp" };
        sheet.Add(first);
        sheet.Add(second);

        Assert.Same(second, sheet.Get("x"));
    }
}
