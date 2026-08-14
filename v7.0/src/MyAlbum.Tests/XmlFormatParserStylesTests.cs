using System.Xml.Linq;
using MyAlbum.Models.Styles;

namespace MyAlbum.Tests;

public class XmlFormatParserStylesTests
{

    [Fact]
    public void ParseStyles_ParsesSingleStyle()
    {
        var xml = """
            <styles>
              <stamp style="standard" width="30" height="40"/>
            </styles>
            """;

        var styles = Style.ParseStyles(XDocument.Parse(xml).Root!);

        Assert.Single(styles);
        Assert.Equal("standard", styles[0].Name);
        Assert.Equal("stamp", styles[0].Type);
        Assert.Equal("30", styles[0].Properties["width"]);
        Assert.Equal("40", styles[0].Properties["height"]);
    }

    [Fact]
    public void ParseStyles_ParsesDefaultMarker()
    {
        var xml = """
            <styles>
              <stamp style="std" default="true" width="30"/>
              <stamp style="large" width="50"/>
            </styles>
            """;

        var styles = Style.ParseStyles(XDocument.Parse(xml).Root!);

        Assert.True(styles[0].Default);
        Assert.False(styles[1].Default);
    }

    [Fact]
    public void ParseStyles_ParsesNestedProperties()
    {
        var xml = """
            <styles>
              <stamp style="fancy" width="30">
                <title font_size="8" font_style="bold"/>
                <footer font_size="6"/>
              </stamp>
            </styles>
            """;

        var styles = Style.ParseStyles(XDocument.Parse(xml).Root!);
        var props = styles[0].Properties;

        var title = Assert.IsType<Dictionary<string, string>>(props["title"]);
        Assert.Equal("8", title["font_size"]);
        Assert.Equal("bold", title["font_style"]);

        var footer = Assert.IsType<Dictionary<string, string>>(props["footer"]);
        Assert.Equal("6", footer["font_size"]);
    }

    [Fact]
    public void ParseStyles_MultipleTypes()
    {
        var xml = """
            <styles>
              <stamp style="s1" width="30"/>
              <text style="heading" font_size="14" align="center"/>
              <row style="spaced" space="5"/>
            </styles>
            """;

        var styles = Style.ParseStyles(XDocument.Parse(xml).Root!);

        Assert.Equal(3, styles.Count);
        Assert.Equal("stamp", styles[0].Type);
        Assert.Equal("text", styles[1].Type);
        Assert.Equal("row", styles[2].Type);
    }

    [Fact]
    public void ParseStyles_SkipsElementsWithoutStyleAttribute()
    {
        var xml = """
            <styles>
              <stamp style="valid" width="30"/>
              <stamp width="30"/>
            </styles>
            """;

        var styles = Style.ParseStyles(XDocument.Parse(xml).Root!);

        Assert.Single(styles);
        Assert.Equal("valid", styles[0].Name);
    }
}
