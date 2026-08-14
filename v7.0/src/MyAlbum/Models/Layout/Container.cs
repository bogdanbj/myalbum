using PdfSharp.Drawing;

namespace MyAlbum.Models.Layout;

public abstract class Container : Element
{
    public List<Element> Children { get; set; } = [];

    /// <summary>
    /// Calculate layout for all children. Derived classes should override
    /// to implement specific layout logic (vertical for Column, horizontal for Row).
    /// </summary>
    public override void Calculate(double availableWidth, double availableHeight)
    {
        // Base implementation - derived classes should override
        foreach (var child in Children)
        {
            child.Calculate(availableWidth, availableHeight);
        }
    }

    /// <summary>
    /// Draw background and all children.
    /// </summary>
    public override void Draw(XGraphics gfx)
    {
        // Draw background if set
        if (BgColor != XColors.White)
        {
            var rect = new XRect(
                XUnit.FromMillimeter(X),
                XUnit.FromMillimeter(Y),
                XUnit.FromMillimeter(Width),
                XUnit.FromMillimeter(Height));
            gfx.DrawRectangle(new XSolidBrush(BgColor), rect);
        }

        // Draw all children
        foreach (var child in Children)
        {
            child.Draw(gfx);
        }
    }
}
