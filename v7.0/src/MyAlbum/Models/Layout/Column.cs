using PdfSharp.Drawing;

namespace MyAlbum.Models.Layout;

public class Column : Container
{
    public double[] Padding { get; set; } = [];
    public double Spacing { get; set; }
    public string Align { get; set; } = string.Empty;

    /// <summary>
    /// Calculate column dimensions and layout children vertically.
    /// </summary>
    public override void Calculate(double availableWidth, double availableHeight)
    {
        // Use specified width or available width
        if (Width == 0)
            Width = availableWidth;

        // Layout children vertically and sum heights
        double currentY = Y;
        double totalHeight = 0;

        foreach (var child in Children)
        {
            child.X = X;
            child.Y = currentY;
            child.Calculate(Width, availableHeight - totalHeight);
            currentY += child.Height + Spacing;
            totalHeight += child.Height + Spacing;
        }

        // Set height based on children
        if (Height == 0)
            Height = totalHeight > 0 ? totalHeight - Spacing : 0;
    }

    /// <summary>
    /// Draw column background and children.
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

        // Draw children
        foreach (var child in Children)
        {
            child.Draw(gfx);
        }
    }
}
