using PdfSharp.Drawing;

namespace MyAlbum.Models.Layout;

public class Stamp : Element
{
    public Text? Title { get; set; }
    public Frame Frame { get; set; } = null!;
    public Image? Image { get; set; }
    public Text? I1 { get; set; }
    public Text? I2 { get; set; }
    public Text? I3 { get; set; }
    public Text? F1 { get; set; }
    public Text? F2 { get; set; }
    public Text? F3 { get; set; }

    /// <summary>
    /// Calculate stamp dimensions and internal layout.
    /// </summary>
    public override void Calculate(double availableWidth, double availableHeight)
    {
        // Width and Height should already be set from definition
        // Calculate frame position within stamp
        if (Frame != null)
        {
            Frame.X = X;
            Frame.Y = Y;
            Frame.Width = Width;
            Frame.Height = Height;
            Frame.Calculate(Width, Height);
        }

        // TODO: Calculate positions for Title, Image, I1-I3, F1-F3
    }

    /// <summary>
    /// Draw the stamp with frame and text.
    /// </summary>
    public override void Draw(XGraphics gfx)
    {
        // Draw frame
        Frame?.Draw(gfx);

        // TODO: Draw Title, Image, Inside text (I1-I3), Footer text (F1-F3)
    }
}
