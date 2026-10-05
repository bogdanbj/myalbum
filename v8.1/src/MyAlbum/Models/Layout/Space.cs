using PdfSharp.Drawing;

namespace MyAlbum.Models.Layout;

public class Space : Element
{
    /// <summary>
    /// Calculate space dimensions.
    /// </summary>
    public override void Calculate(double availableWidth, double availableHeight)
    {
        if (Width == 0)
            Width = availableWidth;
        // Height should be set from definition
    }

    /// <summary>
    /// Draw space (empty, nothing to render unless background is set).
    /// </summary>
    public override void Draw(XGraphics gfx)
    {
        // Space is typically empty, but draw background if set
        if (BgColor != XColors.White)
        {
            var rect = new XRect(
                XUnit.FromMillimeter(X),
                XUnit.FromMillimeter(Y),
                XUnit.FromMillimeter(Width),
                XUnit.FromMillimeter(Height));
            gfx.DrawRectangle(new XSolidBrush(BgColor), rect);
        }
    }
}
