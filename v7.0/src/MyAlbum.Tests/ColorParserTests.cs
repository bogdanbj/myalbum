using PdfSharp.Drawing;
using MyAlbum.Parsing;

namespace MyAlbum.Tests;

public class ColorParserTests
{
    [Fact]
    public void Parse_NamedColor()
    {
        Assert.Equal(XColors.Red, ColorParser.Parse("Red"));
    }

    [Fact]
    public void Parse_NamedColor_CaseInsensitive()
    {
        Assert.Equal(XColors.CadetBlue, ColorParser.Parse("cadetblue"));
    }

    [Fact]
    public void Parse_RgbTuple()
    {
        var color = ColorParser.Parse("48,48,48");
        Assert.Equal(48, color.R);
        Assert.Equal(48, color.G);
        Assert.Equal(48, color.B);
    }

    [Fact]
    public void Parse_RgbTuple_WithSpaces()
    {
        var color = ColorParser.Parse("255, 128, 0");
        Assert.Equal(255, color.R);
        Assert.Equal(128, color.G);
        Assert.Equal(0, color.B);
    }

    [Fact]
    public void Parse_Unknown_FallsBackToBlack()
    {
        Assert.Equal(XColors.Black, ColorParser.Parse("not_a_color"));
    }
}
