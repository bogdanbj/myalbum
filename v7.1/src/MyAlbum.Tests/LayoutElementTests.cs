using PdfSharp.Drawing;

namespace MyAlbum.Tests;

public class LayoutElementTests
{
    private class TestElement : Models.Layout.Element
    {
        public override void Calculate(double availableWidth, double availableHeight) { }
        public override void Draw(XGraphics gfx) { }
    }

    [Fact]
    public void Color_DefaultsToBlack()
    {
        var el = new TestElement();
        Assert.Equal(XColors.Black, el.Color);
    }

    [Fact]
    public void BgColor_DefaultsToWhite()
    {
        var el = new TestElement();
        Assert.Equal(XColors.White, el.BgColor);
    }

    [Fact]
    public void Color_CanBeOverridden()
    {
        var el = new TestElement { Color = XColors.Red };
        Assert.Equal(XColors.Red, el.Color);
    }

    [Fact]
    public void BgColor_CanBeOverridden()
    {
        var el = new TestElement { BgColor = XColors.Blue };
        Assert.Equal(XColors.Blue, el.BgColor);
    }

    [Fact]
    public void Position_DefaultsToZero()
    {
        var el = new TestElement();
        Assert.Equal(0, el.X);
        Assert.Equal(0, el.Y);
        Assert.Equal(0, el.Width);
        Assert.Equal(0, el.Height);
    }
}
