using PdfSharp.Drawing;

namespace MyAlbum.Models.Layout;

public class Image : Element
{
    public string Src { get; set; } = string.Empty;
    public string ScaleMode { get; set; } = string.Empty;

    /// <summary>
    /// Calculate image dimensions.
    /// </summary>
    public override void Calculate(double availableWidth, double availableHeight)
    {
        // TODO: Load image and calculate dimensions based on ScaleMode
        if (Width == 0)
            Width = availableWidth;
        if (Height == 0)
            Height = availableHeight;
    }

    /// <summary>
    /// Draw the image.
    /// </summary>
    public override void Draw(XGraphics gfx)
    {
        if (string.IsNullOrEmpty(Src))
            return;

        // TODO: Load and draw actual image
        // For now, draw a placeholder rectangle
        var rect = new XRect(
            XUnit.FromMillimeter(X),
            XUnit.FromMillimeter(Y),
            XUnit.FromMillimeter(Width),
            XUnit.FromMillimeter(Height));

        var pen = new XPen(XColors.Gray, 0.25);
        gfx.DrawRectangle(pen, rect);

        // Draw X through the placeholder
        gfx.DrawLine(pen, rect.TopLeft, rect.BottomRight);
        gfx.DrawLine(pen, rect.TopRight, rect.BottomLeft);
    }
}
