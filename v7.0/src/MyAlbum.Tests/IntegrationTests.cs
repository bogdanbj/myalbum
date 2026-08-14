using System.Xml.Linq;
using MyAlbum.Models.Definition;
using MyAlbum.Models.Styles;

namespace MyAlbum.Tests;

public class IntegrationTests
{
    private const string TemplatesDir = @"C:\My\Git\myalbum\v7.0\Templates";

    private static Album LoadAlbum(string fileName)
    {
        var filePath = Path.Combine(TemplatesDir, fileName);
        var xml = File.ReadAllText(filePath);
        var album = Album.FromXml(XDocument.Parse(xml).Root!);

        // Load external styles (kept separate from embedded)
        if (!string.IsNullOrEmpty(album.StyleFile))
        {
            var stylePath = Path.Combine(TemplatesDir, album.StyleFile);
            if (File.Exists(stylePath))
            {
                var stylesXml = File.ReadAllText(stylePath);
                album.ExternalStyles = Style.ParseStyles(XDocument.Parse(stylesXml).Root!);
            }
        }

        return album;
    }

    [Fact]
    public void Canada6_ParsesAll37Pages()
    {
        var album = LoadAlbum("Canada6.album");
        Assert.Equal(37, album.Pages.Count);
    }

    [Fact]
    public void Canada6_LoadsExternalStyles()
    {
        var album = LoadAlbum("Canada6.album");
        Assert.Equal(17, album.ExternalStyles.Count);
    }

    [Fact]
    public void Canada6_HasNoEmbeddedStyles()
    {
        var album = LoadAlbum("Canada6.album");
        Assert.Empty(album.Styles);
    }

    [Fact]
    public void Canada6_StyleFileIsReferenced()
    {
        var album = LoadAlbum("Canada6.album");
        Assert.Equal("Canada.styles", album.StyleFile);
    }

    [Fact]
    public void Canada6_FirstPageHasElements()
    {
        var album = LoadAlbum("Canada6.album");
        Assert.NotEmpty(album.Pages[0].Children);
    }

    [Fact]
    public void Canada6_HasVersionAttribute()
    {
        var album = LoadAlbum("Canada6.album");
        Assert.Equal("4.0", album.Version);
    }
}
