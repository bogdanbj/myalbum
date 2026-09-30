using PdfSharp.Drawing;

namespace MyAlbum.Models.Layout;

public class Frame : Element
{
    public double[] Lines { get; set; } = [];
    public double Padding { get; set; }

    /// <summary>
    /// Calculate frame dimensions (usually set by parent).
    /// </summary>
    public override void Calculate(double availableWidth, double availableHeight)
    {
        if (Width == 0)
            Width = availableWidth;
        if (Height == 0)
            Height = availableHeight;
    }

    /// <summary>
    /// Draw the frame border(s).
    /// </summary>
    public override void Draw(XGraphics gfx)
    {
        var x = XUnit.FromMillimeter(X);
        var y = XUnit.FromMillimeter(Y);
        var width = XUnit.FromMillimeter(Width);
        var height = XUnit.FromMillimeter(Height);

        // Determine line widths from Lines array
        // Lines[0] = outer line width, Lines[1] = gap, Lines[2] = inner line width
        double outerWidth = Lines.Length > 0 ? Lines[0] : 0.5;
        double gap = Lines.Length > 1 ? Lines[1] : 0;
        double innerWidth = Lines.Length > 2 ? Lines[2] : 0;

        var pen = new XPen(Color, outerWidth);

        // Draw outer rectangle
        gfx.DrawRectangle(pen, x, y, width, height);

        // Draw inner rectangle if double-line frame
        if (gap > 0 && innerWidth > 0)
        {
            var innerPen = new XPen(Color, innerWidth);
            var gapMm = XUnit.FromMillimeter(gap);
            gfx.DrawRectangle(innerPen, 
                x + gapMm, 
                y + gapMm, 
                width - gapMm * 2, 
                height - gapMm * 2);
        }
    }
}
