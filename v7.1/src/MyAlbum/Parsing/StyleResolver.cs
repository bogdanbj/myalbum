using MyAlbum.Models.Styles;
using Def = MyAlbum.Models.Definition;
using Lay = MyAlbum.Models.Layout;

namespace MyAlbum.Parsing;

/// <summary>
/// Transforms Definition models into Layout models, merging style properties.
/// Style properties act as defaults — inline attributes take precedence.
/// </summary>
public class StyleResolver
{
    private readonly StyleSheet _styleSheet;

    public StyleResolver(StyleSheet styleSheet)
    {
        _styleSheet = styleSheet;
    }

    /// <summary>
    /// Build a StyleSheet from a list of parsed styles
    /// </summary>
    public static StyleSheet BuildStyleSheet(List<Style> styles)
    {
        var sheet = new StyleSheet();
        foreach (var style in styles)
            sheet.Add(style);
        return sheet;
    }

    public Lay.Album Resolve(Def.Album def)
    {
        var album = new Lay.Album();

        foreach (var defPage in def.Pages)
            album.Pages.Add(ResolvePage(defPage));

        return album;
    }

    private Lay.Page ResolvePage(Def.Page def)
    {
        var style = FindStyle(def.Style, "page");

        var page = new Lay.Page
        {
            Number = def.Number,
            Title = def.Title ?? GetStringProp(style, "title"),
            Size = def.Size ?? GetStringProp(style, "size") ?? "letter",
            Orientation = def.Orientation ?? GetStringProp(style, "orientation") ?? "portrait",
            Padding = def.Padding.Length > 0 ? def.Padding : GetPaddingProp(style, "padding"),
            RowSpacing = def.RowSpacing > 0 ? def.RowSpacing : GetDoubleProp(style, "row_spacing"),
            ColumnSpacing = def.ColumnSpacing > 0 ? def.ColumnSpacing : GetDoubleProp(style, "column_spacing"),
        };

        foreach (var child in def.Children)
        {
            var resolved = ResolveElement(child);
            if (resolved != null)
                page.Children.Add(resolved);
        }

        return page;
    }

    private Lay.Element? ResolveElement(Def.Element def)
    {
        return def switch
        {
            Def.Row row => ResolveRow(row),
            Def.Column col => ResolveColumn(col),
            Def.Stamp stamp => ResolveStamp(stamp),
            Def.Text text => ResolveText(text),
            Def.Image image => ResolveImage(image),
            Def.Frame frame => ResolveFrame(frame),
            Def.Space space => ResolveSpace(space),
            _ => null
        };
    }

    private Lay.Row ResolveRow(Def.Row def)
    {
        var style = FindStyle(def.Style, "row");

        var row = new Lay.Row
        {
            Padding = def.Padding.Length > 0 ? def.Padding : GetPaddingProp(style, "padding"),
            Spacing = ParseSpacing(def.Spacing) ?? GetDoubleProp(style, "spacing"),
            Align = def.Align ?? GetStringProp(style, "align") ?? string.Empty,
            VAlign = def.VAlign ?? GetStringProp(style, "valign") ?? string.Empty,
            Height = def.Height ?? GetDoubleProp(style, "height"),
        };

        ApplyBgColor(row, def.BgColor, style);

        foreach (var child in def.Children)
        {
            var resolved = ResolveElement(child);
            if (resolved != null)
                row.Children.Add(resolved);
        }

        return row;
    }

    private Lay.Column ResolveColumn(Def.Column def)
    {
        var style = FindStyle(def.Style, "column");

        var col = new Lay.Column
        {
            Width = ParseWidth(def.Width) ?? GetDoubleProp(style, "width"),
            Padding = def.Padding.Length > 0 ? def.Padding : GetPaddingProp(style, "padding"),
            Spacing = def.Spacing > 0 ? def.Spacing : GetDoubleProp(style, "spacing"),
            Align = def.Align ?? GetStringProp(style, "align") ?? string.Empty,
        };

        ApplyBgColor(col, def.BgColor, style);

        foreach (var child in def.Children)
        {
            var resolved = ResolveElement(child);
            if (resolved != null)
                col.Children.Add(resolved);
        }

        return col;
    }

    private Lay.Stamp ResolveStamp(Def.Stamp def)
    {
        var style = FindStyle(def.Style, "stamp");

        return new Lay.Stamp
        {
            Width = def.Width > 0 ? def.Width : GetDoubleProp(style, "width"),
            Height = def.Height > 0 ? def.Height : GetDoubleProp(style, "height"),
        };
    }

    private Lay.Text ResolveText(Def.Text def)
    {
        var style = FindStyle(def.Style, "text");

        var text = new Lay.Text
        {
            Content = def.Content,
            FontName = def.FontName ?? GetStringProp(style, "font-name") ?? GetStringProp(style, "font_name") ?? string.Empty,
            FontSize = def.FontSize ?? GetDoublePropNullable(style, "font-size") ?? GetDoublePropNullable(style, "font_size") ?? 0,
            FontStyle = def.FontStyle ?? GetStringProp(style, "font-style") ?? GetStringProp(style, "font_style") ?? string.Empty,
            Align = def.Align ?? GetStringProp(style, "align") ?? string.Empty,
        };

        ApplyColor(text, def.Color, style);
        ApplyBgColor(text, def.BgColor, style);

        return text;
    }

    private Lay.Image ResolveImage(Def.Image def)
    {
        var style = FindStyle(def.Style, "image");

        return new Lay.Image
        {
            Src = def.Src,
            ScaleMode = def.ScaleMode ?? GetStringProp(style, "scale_mode") ?? GetStringProp(style, "scale-mode") ?? string.Empty,
        };
    }

    private Lay.Frame ResolveFrame(Def.Frame def)
    {
        var style = FindStyle(def.Style, "frame");

        var frame = new Lay.Frame
        {
            Width = def.Width > 0 ? def.Width : GetDoubleProp(style, "width"),
            Height = def.Height > 0 ? def.Height : GetDoubleProp(style, "height"),
            Lines = def.Lines.Length > 0 ? def.Lines : GetDoubleArrayProp(style, "frame-width"),
            Padding = def.Padding > 0 ? def.Padding : GetDoubleProp(style, "padding"),
        };

        ApplyColor(frame, def.Color, style);

        return frame;
    }

    private Lay.Space ResolveSpace(Def.Space def)
    {
        var style = FindStyle(def.Style, "space");

        return new Lay.Space
        {
            Width = def.Width ?? GetDoubleProp(style, "width"),
            Height = def.Height ?? GetDoubleProp(style, "height"),
        };
    }

    // --- Style lookup helpers ---

    private Style? FindStyle(string? styleName, string elementType)
    {
        if (styleName != null)
            return _styleSheet.Get(styleName);

        return _styleSheet.GetDefault(elementType);
    }

    // --- Property extraction helpers ---

    private static string? GetStringProp(Style? style, string key)
    {
        if (style == null) return null;
        return style.Properties.TryGetValue(key, out var val) && val is string s ? s : null;
    }

    private static double GetDoubleProp(Style? style, string key)
    {
        var s = GetStringProp(style, key);
        return s != null && double.TryParse(s, out var v) ? v : 0;
    }

    private static double? GetDoublePropNullable(Style? style, string key)
    {
        var s = GetStringProp(style, key);
        return s != null && double.TryParse(s, out var v) ? v : null;
    }

    private static double? ParseSpacing(string? spacing)
    {
        if (spacing == null) return null;
        return double.TryParse(spacing, out var v) ? v : null;
    }

    private static double? ParseWidth(string? width)
    {
        if (width == null) return null;
        return double.TryParse(width, out var v) ? v : null;
    }

    private static double[] GetPaddingProp(Style? style, string key)
    {
        var s = GetStringProp(style, key);
        if (s == null) return [];

        var parts = s.Split(',', ' ')
            .Where(p => !string.IsNullOrWhiteSpace(p))
            .Select(p => double.TryParse(p.Trim(), out var v) ? v : 0)
            .ToArray();

        return parts.Length is 1 or 2 or 4 ? parts : [];
    }

    private static double[] GetDoubleArrayProp(Style? style, string key)
    {
        var s = GetStringProp(style, key);
        if (s == null) return [];

        return s.Split(',', ' ')
            .Where(p => !string.IsNullOrWhiteSpace(p))
            .Select(p => double.TryParse(p.Trim(), out var v) ? v : 0)
            .ToArray();
    }

    // --- Color helpers ---

    private static void ApplyColor(Lay.Element element, string? inlineColor, Style? style)
    {
        var colorStr = inlineColor ?? GetStringProp(style, "color");
        if (colorStr != null)
            element.Color = ColorParser.Parse(colorStr);
    }

    private static void ApplyBgColor(Lay.Element element, string? inlineBgColor, Style? style)
    {
        var bgStr = inlineBgColor ?? GetStringProp(style, "bgcolor");
        if (bgStr != null)
            element.BgColor = ColorParser.Parse(bgStr);
    }
}
