using System.Xml.Linq;
using MyAlbum.Models.Definition;

namespace MyAlbum.Tests;

public class XmlFormatParserAlbumTests
{

    [Fact]
    public void ParseAlbum_MinimalAlbum_ReturnsOnePage()
    {
        var xml = """
            <album>
              <page no="1">
                <row><stamp width="30" height="40" title="Test"/></row>
              </page>
            </album>
            """;

        var album = Album.FromXml(XDocument.Parse(xml).Root!);

        Assert.Single(album.Pages);
        Assert.Equal(1, album.Pages[0].Number);
    }

    [Fact]
    public void ParseAlbum_ExtractsStylesAttribute()
    {
        var xml = """<MyAlbum styles="Canada.styles"><page no="1"/></MyAlbum>""";

        var album = Album.FromXml(XDocument.Parse(xml).Root!);

        Assert.Equal("Canada.styles", album.StyleFile);
    }

    [Fact]
    public void ParseAlbum_ExtractsStylesSrcElement()
    {
        var xml = """
            <album>
              <styles src="mystyles.xml"/>
              <page no="1"/>
            </album>
            """;

        var album = Album.FromXml(XDocument.Parse(xml).Root!);

        Assert.Equal("mystyles.xml", album.StyleFile);
    }

    [Fact]
    public void ParseAlbum_ParsesPageAttributes()
    {
        var xml = """
            <album>
              <page no="5" title="Page Five" size="a4" orientation="landscape" padding="10,20" row_spacing="5" column_spacing="3"/>
            </album>
            """;

        var album = Album.FromXml(XDocument.Parse(xml).Root!);
        var page = album.Pages[0];

        Assert.Equal(1, page.Number);  // Position-based, not from "no" attribute
        Assert.Equal(5, page.No);      // "no" attribute stored for debugging
        Assert.Equal("Page Five", page.Title);
        Assert.Equal("a4", page.Size);
        Assert.Equal("landscape", page.Orientation);
        Assert.Equal([10, 20], page.Padding);
        Assert.Equal(5, page.RowSpacing);
        Assert.Equal(3, page.ColumnSpacing);
    }

    [Fact]
    public void ParseAlbum_ParsesNestedHierarchy()
    {
        var xml = """
            <album>
              <page no="1">
                <row>
                  <column width="50%">
                    <stamp width="30" height="40" title="S1"/>
                  </column>
                  <column width="50%">
                    <stamp width="30" height="40" title="S2"/>
                  </column>
                </row>
              </page>
            </album>
            """;

        var album = Album.FromXml(XDocument.Parse(xml).Root!);
        var page = album.Pages[0];
        var row = Assert.IsType<Row>(page.Children[0]);
        Assert.Equal(2, row.Children.Count);
        var col1 = Assert.IsType<Column>(row.Children[0]);
        var stamp = Assert.IsType<Stamp>(col1.Children[0]);
        Assert.Equal("S1", stamp.Title);
    }

    [Fact]
    public void ParseAlbum_ParsesStampAttributes()
    {
        var xml = """
            <album>
              <page no="1">
                <row>
                  <stamp width="35" height="45" title="Maple" image="maple.png"
                         i1="2024" i2="$1.00" i3="mint" f1="Scott 100" f2="perf 12" f3="unused"/>
                </row>
              </page>
            </album>
            """;

        var album = Album.FromXml(XDocument.Parse(xml).Root!);
        var stamp = Assert.IsType<Stamp>(album.Pages[0].Children[0].AsRow().Children[0]);

        Assert.Equal(35, stamp.Width);
        Assert.Equal(45, stamp.Height);
        Assert.Equal("Maple", stamp.Title);
        Assert.Equal("maple.png", stamp.Image);
        Assert.Equal("2024", stamp.I1);
        Assert.Equal("$1.00", stamp.I2);
        Assert.Equal("mint", stamp.I3);
        Assert.Equal("Scott 100", stamp.F1);
        Assert.Equal("perf 12", stamp.F2);
        Assert.Equal("unused", stamp.F3);
    }

    [Fact]
    public void ParseAlbum_ParsesTextElement()
    {
        var xml = """
            <album>
              <page no="1">
                <row>
                  <text style="heading" align="center" font_size="14">Hello World</text>
                </row>
              </page>
            </album>
            """;

        var album = Album.FromXml(XDocument.Parse(xml).Root!);
        var text = Assert.IsType<Text>(album.Pages[0].Children[0].AsRow().Children[0]);

        Assert.Equal("Hello World", text.Content);
        Assert.Equal("heading", text.Style);
        Assert.Equal("center", text.Align);
        Assert.Equal(14, text.FontSize);
    }

    [Fact]
    public void ParseAlbum_IgnoresUnknownElements()
    {
        var xml = """
            <album>
              <page no="1">
                <row>
                  <unknown_thing attr="val"/>
                  <stamp width="30" height="40" title="Real"/>
                </row>
              </page>
            </album>
            """;

        var album = Album.FromXml(XDocument.Parse(xml).Root!);
        var row = Assert.IsType<Row>(album.Pages[0].Children[0]);
        Assert.Single(row.Children);
        Assert.IsType<Stamp>(row.Children[0]);
    }

    [Fact]
    public void ParseAlbum_WorksWithAnyRootTagName()
    {
        var xml1 = """<MyAlbum ver="4.0"><page no="1"/></MyAlbum>""";
        var xml2 = """<album><page no="1"/></album>""";
        var xml3 = """<whatever><page no="1"/></whatever>""";

        Assert.Single(Album.FromXml(XDocument.Parse(xml1).Root!).Pages);
        Assert.Single(Album.FromXml(XDocument.Parse(xml2).Root!).Pages);
        Assert.Single(Album.FromXml(XDocument.Parse(xml3).Root!).Pages);
    }

    [Fact]
    public void ParseAlbum_ParsesVersionAttribute()
    {
        var xml = """<MyAlbum ver="4.0"><page no="1"/></MyAlbum>""";

        var album = Album.FromXml(XDocument.Parse(xml).Root!);

        Assert.Equal("4.0", album.Version);
    }

    [Fact]
    public void ParseAlbum_DefaultPageNumbering()
    {
        var xml = """
            <album>
              <page/>
              <page/>
              <page no="10"/>
            </album>
            """;

        var album = Album.FromXml(XDocument.Parse(xml).Root!);

        // Page numbers are position-based (1, 2, 3), not from "no" attribute
        Assert.Equal(1, album.Pages[0].Number);
        Assert.Equal(2, album.Pages[1].Number);
        Assert.Equal(3, album.Pages[2].Number);
        
        // "no" attribute is stored separately for debugging
        Assert.Null(album.Pages[0].No);
        Assert.Null(album.Pages[1].No);
        Assert.Equal(10, album.Pages[2].No);
    }

    [Fact]
    public void ParseAlbum_ParsesPaddingSingleValue()
    {
        var xml = """<album><page no="1" padding="15"/></album>""";

        var album = Album.FromXml(XDocument.Parse(xml).Root!);

        Assert.Equal([15], album.Pages[0].Padding);
    }

    [Fact]
    public void ParseAlbum_ParsesPaddingFourValues()
    {
        var xml = """<album><page no="1" padding="10 20 30 40"/></album>""";

        var album = Album.FromXml(XDocument.Parse(xml).Root!);

        Assert.Equal([10, 20, 30, 40], album.Pages[0].Padding);
    }
}

// Helper extension for cleaner test assertions
internal static class ElementTestExtensions
{
    public static Row AsRow(this Element el) => (Row)el;
}
