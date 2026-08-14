using PdfSharp.Drawing;

namespace MyAlbum.Models.Layout;

public class Row : Container
{
    public double[] Padding { get; set; } = [];
    public double Spacing { get; set; }
    public string Align { get; set; } = string.Empty;
    public string VAlign { get; set; } = string.Empty;

    /// <summary>
    /// Calculate row dimensions and layout children horizontally.
    /// </summary>
    public override void Calculate(double availableWidth, double availableHeight)
    {
        Width = availableWidth;
        
        // If height is already set (from definition), use it; otherwise calculate from children
        if (Height == 0)
        {
            // Calculate height based on tallest child
            double maxHeight = 0;
            foreach (var child in Children)
            {
                child.Calculate(availableWidth, availableHeight);
                maxHeight = Math.Max(maxHeight, child.Height);
            }
            Height = maxHeight;
        }

        // Position children horizontally
        double currentX = X;
        foreach (var child in Children)
        {
            child.X = currentX;
            child.Y = Y;
            child.Calculate(child.Width > 0 ? child.Width : availableWidth / Math.Max(1, Children.Count), Height);
            currentX += child.Width + Spacing;
        }
    }

    /// <summary>
    /// Draw row background and children.
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
