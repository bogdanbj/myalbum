using PdfSharp.Drawing;

namespace MyAlbum.Models.Layout;

public abstract class Element
{
    public double X { get; set; }
    public double Y { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }

    private XColor? _color;
    public XColor Color
    {
        get => _color ?? XColors.Black;
        set => _color = value;
    }

    private XColor? _bgColor;
    public XColor BgColor
    {
        get => _bgColor ?? XColors.White;
        set => _bgColor = value;
    }

    /// <summary>
    /// Calculate the position and size of this element within the available space.
    /// </summary>
    /// <param name="availableWidth">Available width in mm</param>
    /// <param name="availableHeight">Available height in mm</param>
    public abstract void Calculate(double availableWidth, double availableHeight);

    /// <summary>
    /// Draw this element to the graphics context.
    /// </summary>
    /// <param name="gfx">PdfSharp graphics context</param>
    public abstract void Draw(XGraphics gfx);
}
