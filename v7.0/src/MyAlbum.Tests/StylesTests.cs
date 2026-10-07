using MyAlbum.Models.Definition;

namespace MyAlbum.Tests;

public class StylesTests
{
    [Fact]
    public void Get_ReturnsAddedStyle()
    {
        Styles styles = new();
        Style style = new() { Name = "heading", Type = "text" };
        styles.Add(style);

        Assert.Same(style, styles.Get("heading"));
    }

    [Fact]
    public void Get_ReturnsNull_WhenNotFound()
    {
        Styles styles = new();
        Assert.Null(styles.Get("nonexistent"));
    }

    [Fact]
    public void GetDefault_ReturnsDefaultForType()
    {
        Styles styles = new();
        Style style = new() { Name = "std", Type = "stamp", Default = true };
        styles.Add(style);

        Assert.Same(style, styles.GetDefault("stamp"));
    }

    [Fact]
    public void GetDefault_ReturnsNull_WhenNoDefault()
    {
        Styles styles = new();
        Style style = new() { Name = "custom", Type = "stamp", Default = false };
        styles.Add(style);

        Assert.Null(styles.GetDefault("stamp"));
    }

    [Fact]
    public void Contains_ReturnsTrueForExisting()
    {
        Styles styles = new();
        styles.Add(new Style { Name = "test", Type = "text" });

        Assert.True(styles.Contains("test"));
        Assert.False(styles.Contains("other"));
    }

    [Fact]
    public void Names_ReturnsAllStyleNames()
    {
        Styles styles = new();
        styles.Add(new Style { Name = "a", Type = "text" });
        styles.Add(new Style { Name = "b", Type = "stamp" });

        Assert.Equal(["a", "b"], styles.Names.OrderBy(n => n).ToArray());
    }

    [Fact]
    public void Add_OverwritesExistingStyleWithSameName()
    {
        Styles styles = new();
        Style first = new() { Name = "x", Type = "text" };
        Style second = new() { Name = "x", Type = "stamp" };
        styles.Add(first);
        styles.Add(second);

        Assert.Same(second, styles.Get("x"));
    }
}
