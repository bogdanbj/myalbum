using PdfSharp.Drawing;

namespace MyAlbum.Models.Layout;

public class Page : Container
{
    // Page dimensions in mm
    private const double LetterWidthMm = 215.9;  // 8.5 inches
    private const double LetterHeightMm = 279.4; // 11 inches
    private const double A4WidthMm = 210;
    private const double A4HeightMm = 297;

    public int Number { get; set; }
    public string? Title { get; set; }
    public string Size { get; set; } = "letter";
    public string Orientation { get; set; } = "portrait";
    public double[] Padding { get; set; } = [];
    public double RowSpacing { get; set; }
    public double ColumnSpacing { get; set; }
    public Image? Background { get; set; }
    public Image? Banner { get; set; }
    public Frame? Border { get; set; }
    public Text? Header { get; set; }
    public Text? Footer { get; set; }

    /// <summary>
    /// Calculate page dimensions and layout all children.
    /// </summary>
    public override void Calculate(double availableWidth, double availableHeight)
    {
        // Set page dimensions based on size and orientation
        var (pageWidth, pageHeight) = GetPageDimensions();
        Width = pageWidth;
        Height = pageHeight;
        X = 0;
        Y = 0;

        // Calculate content area (page minus padding)
        var padding = NormalizePadding();
        var contentX = padding.Left;
        var contentY = padding.Top;
        var contentWidth = Width - padding.Left - padding.Right;
        var contentHeight = Height - padding.Top - padding.Bottom;

        // Calculate border if present
        if (Border != null)
        {
            Border.X = contentX;
            Border.Y = contentY;
            Border.Width = contentWidth;
            Border.Height = contentHeight;
            Border.Calculate(contentWidth, contentHeight);
        }

        // Layout children vertically within content area
        double currentY = contentY;
        foreach (var child in Children)
        {
            child.X = contentX;
            child.Y = currentY;
            child.Calculate(contentWidth, contentHeight - (currentY - contentY));
            currentY += child.Height + RowSpacing;
        }
    }

    /// <summary>
    /// Draw page background, border, and all children.
    /// </summary>
    public override void Draw(XGraphics gfx)
    {
        // Draw page background
        if (BgColor != XColors.White)
        {
            gfx.DrawRectangle(new XSolidBrush(BgColor), 0, 0, 
                XUnit.FromMillimeter(Width), XUnit.FromMillimeter(Height));
        }

        // Draw border if present
        Border?.Draw(gfx);

        // Draw children (commented out for now)
        // base.Draw(gfx);
    }

    private (double Width, double Height) GetPageDimensions()
    {
        var (width, height) = Size.ToLowerInvariant() switch
        {
            "a4" => (A4WidthMm, A4HeightMm),
            "legal" => (215.9, 355.6),
            _ => (LetterWidthMm, LetterHeightMm)
        };

        // Swap for landscape
        if (Orientation.ToLowerInvariant() == "landscape")
            return (height, width);

        return (width, height);
    }

    private (double Top, double Right, double Bottom, double Left) NormalizePadding()
    {
        return Padding.Length switch
        {
            0 => (0, 0, 0, 0),
            1 => (Padding[0], Padding[0], Padding[0], Padding[0]),
            2 => (Padding[0], Padding[1], Padding[0], Padding[1]),
            4 => (Padding[0], Padding[1], Padding[2], Padding[3]),
            _ => (0, 0, 0, 0)
        };
    }
}
