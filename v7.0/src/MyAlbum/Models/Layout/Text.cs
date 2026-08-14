using PdfSharp.Drawing;

namespace MyAlbum.Models.Layout;

public class Text : Element
{
    public string Content { get; set; } = string.Empty;
    public string FontName { get; set; } = string.Empty;
    public double FontSize { get; set; }
    public string FontStyle { get; set; } = string.Empty;
    public string Align { get; set; } = string.Empty;

    /// <summary>
    /// Calculate text dimensions.
    /// </summary>
    public override void Calculate(double availableWidth, double availableHeight)
    {
        Width = availableWidth;
        // Estimate height based on font size (approximate line height)
        Height = FontSize > 0 ? FontSize * 1.2 : 10;
        // TODO: Measure actual text height using XGraphics.MeasureString
    }

    /// <summary>
    /// Draw the text.
    /// </summary>
    public override void Draw(XGraphics gfx)
    {
        if (string.IsNullOrEmpty(Content))
            return;

        var fontName = !string.IsNullOrEmpty(FontName) ? FontName : "Arial";
        var fontSize = FontSize > 0 ? FontSize : 10;
        var fontStyle = ParseFontStyle(FontStyle);

        XFont font;
        try
        {
            font = new XFont(fontName, fontSize, fontStyle);
        }
        catch
        {
            font = new XFont("Arial", fontSize, fontStyle);
        }

        var brush = new XSolidBrush(Color);
        var rect = new XRect(
            XUnit.FromMillimeter(X),
            XUnit.FromMillimeter(Y),
            XUnit.FromMillimeter(Width),
            XUnit.FromMillimeter(Height));

        var format = new XStringFormat
        {
            Alignment = Align.ToLowerInvariant() switch
            {
                "center" => XStringAlignment.Center,
                "right" => XStringAlignment.Far,
                _ => XStringAlignment.Near
            },
            LineAlignment = XLineAlignment.Near
        };

        gfx.DrawString(Content, font, brush, rect, format);
    }

    private static XFontStyleEx ParseFontStyle(string style)
    {
        if (string.IsNullOrEmpty(style))
            return XFontStyleEx.Regular;

        var lower = style.ToLowerInvariant();
        var result = XFontStyleEx.Regular;

        if (lower.Contains("bold"))
            result |= XFontStyleEx.Bold;
        if (lower.Contains("italic"))
            result |= XFontStyleEx.Italic;
        if (lower.Contains("underline"))
            result |= XFontStyleEx.Underline;
        if (lower.Contains("strikeout"))
            result |= XFontStyleEx.Strikeout;

        return result;
    }
}
