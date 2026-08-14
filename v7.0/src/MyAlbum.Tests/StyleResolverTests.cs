using System.Xml.Linq;
using PdfSharp.Drawing;
using MyAlbum.Models.Styles;
using MyAlbum.Parsing;
using Def = MyAlbum.Models.Definition;
using Lay = MyAlbum.Models.Layout;

namespace MyAlbum.Tests;

public class StyleResolverTests
{
    private static StyleResolver CreateResolver(params Style[] styles)
    {
        var sheet = StyleResolver.BuildStyleSheet(styles.ToList());
        return new StyleResolver(sheet);
    }

    // --- BuildStyleSheet ---

    [Fact]
    public void BuildStyleSheet_CreatesSheetFromStyles()
    {
        var styles = new List<Style>
        {
            new() { Name = "heading", Type = "text" },
            new() { Name = "std", Type = "stamp", Default = true },
        };

        var sheet = StyleResolver.BuildStyleSheet(styles);

        Assert.True(sheet.Contains("heading"));
        Assert.Same(styles[1], sheet.GetDefault("stamp"));
    }

    // --- Text resolution ---

    [Fact]
    public void ResolveText_AppliesStyleDefaults()
    {
        var resolver = CreateResolver(new Style
        {
            Name = "heading", Type = "text",
            Properties = new()
            {
                ["font-name"] = "Century Gothic",
                ["font-size"] = "14",
                ["font-style"] = "bold",
                ["align"] = "center",
            }
        });

        var def = new Def.Album
        {
            Pages = [new Def.Page
            {
                Number = 1,
                Children = [new Def.Row
                {
                    Children = [new Def.Text { Style = "heading", Content = "Hello" }]
                }]
            }]
        };

        var layout = resolver.Resolve(def);
        var row = Assert.IsType<Lay.Row>(layout.Pages[0].Children[0]);
        var text = Assert.IsType<Lay.Text>(row.Children[0]);

        Assert.Equal("Hello", text.Content);
        Assert.Equal("Century Gothic", text.FontName);
        Assert.Equal(14, text.FontSize);
        Assert.Equal("bold", text.FontStyle);
        Assert.Equal("center", text.Align);
    }

    [Fact]
    public void ResolveText_InlineOverridesStyle()
    {
        var resolver = CreateResolver(new Style
        {
            Name = "heading", Type = "text",
            Properties = new()
            {
                ["font-size"] = "14",
                ["align"] = "center",
            }
        });

        var def = new Def.Album
        {
            Pages = [new Def.Page
            {
                Number = 1,
                Children = [new Def.Row
                {
                    Children = [new Def.Text
                    {
                        Style = "heading",
                        Content = "Test",
                        FontSize = 20,
                        Align = "left",
                    }]
                }]
            }]
        };

        var layout = resolver.Resolve(def);
        var row = Assert.IsType<Lay.Row>(layout.Pages[0].Children[0]);
        var text = Assert.IsType<Lay.Text>(row.Children[0]);

        Assert.Equal(20, text.FontSize);
        Assert.Equal("left", text.Align);
    }

    [Fact]
    public void ResolveText_DefaultStyleAppliedWhenNoExplicitStyle()
    {
        var resolver = CreateResolver(new Style
        {
            Name = "text_default", Type = "text", Default = true,
            Properties = new()
            {
                ["font-name"] = "Arial",
                ["font-size"] = "10",
            }
        });

        var def = new Def.Album
        {
            Pages = [new Def.Page
            {
                Number = 1,
                Children = [new Def.Row
                {
                    Children = [new Def.Text { Content = "No style" }]
                }]
            }]
        };

        var layout = resolver.Resolve(def);
        var row = Assert.IsType<Lay.Row>(layout.Pages[0].Children[0]);
        var text = Assert.IsType<Lay.Text>(row.Children[0]);

        Assert.Equal("Arial", text.FontName);
        Assert.Equal(10, text.FontSize);
    }

    // --- Color resolution ---

    [Fact]
    public void ResolveText_AppliesNamedColor()
    {
        var resolver = CreateResolver(new Style
        {
            Name = "colored", Type = "text",
            Properties = new()
            {
                ["color"] = "Red",
                ["bgcolor"] = "CadetBlue",
            }
        });

        var def = new Def.Album
        {
            Pages = [new Def.Page
            {
                Number = 1,
                Children = [new Def.Row
                {
                    Children = [new Def.Text { Style = "colored", Content = "X" }]
                }]
            }]
        };

        var layout = resolver.Resolve(def);
        var row = Assert.IsType<Lay.Row>(layout.Pages[0].Children[0]);
        var text = Assert.IsType<Lay.Text>(row.Children[0]);

        Assert.Equal(XColors.Red, text.Color);
        Assert.Equal(XColors.CadetBlue, text.BgColor);
    }

    [Fact]
    public void ResolveText_AppliesRgbColor()
    {
        var resolver = CreateResolver(new Style
        {
            Name = "rgb", Type = "text",
            Properties = new() { ["color"] = "48,48,48" }
        });

        var def = new Def.Album
        {
            Pages = [new Def.Page
            {
                Number = 1,
                Children = [new Def.Row
                {
                    Children = [new Def.Text { Style = "rgb", Content = "X" }]
                }]
            }]
        };

        var layout = resolver.Resolve(def);
        var row = Assert.IsType<Lay.Row>(layout.Pages[0].Children[0]);
        var text = Assert.IsType<Lay.Text>(row.Children[0]);

        Assert.Equal(48, text.Color.R);
        Assert.Equal(48, text.Color.G);
        Assert.Equal(48, text.Color.B);
    }

    [Fact]
    public void ResolveText_InlineColorOverridesStyle()
    {
        var resolver = CreateResolver(new Style
        {
            Name = "s", Type = "text",
            Properties = new() { ["color"] = "Red" }
        });

        var def = new Def.Album
        {
            Pages = [new Def.Page
            {
                Number = 1,
                Children = [new Def.Row
                {
                    Children = [new Def.Text { Style = "s", Content = "X", Color = "Blue" }]
                }]
            }]
        };

        var layout = resolver.Resolve(def);
        var row = Assert.IsType<Lay.Row>(layout.Pages[0].Children[0]);
        var text = Assert.IsType<Lay.Text>(row.Children[0]);

        Assert.Equal(XColors.Blue, text.Color);
    }

    // --- Page resolution ---

    [Fact]
    public void ResolvePage_AppliesStyleDefaults()
    {
        var resolver = CreateResolver(new Style
        {
            Name = "portrait", Type = "page", Default = true,
            Properties = new()
            {
                ["size"] = "letter",
                ["orientation"] = "portrait",
                ["padding"] = "7,7,7,20",
            }
        });

        var def = new Def.Album
        {
            Pages = [new Def.Page { Number = 1 }]
        };

        var layout = resolver.Resolve(def);
        var page = layout.Pages[0];

        Assert.Equal("letter", page.Size);
        Assert.Equal("portrait", page.Orientation);
        Assert.Equal(new double[] { 7, 7, 7, 20 }, page.Padding);
    }

    [Fact]
    public void ResolvePage_InlineOverridesStyle()
    {
        var resolver = CreateResolver(new Style
        {
            Name = "portrait", Type = "page", Default = true,
            Properties = new()
            {
                ["size"] = "letter",
                ["orientation"] = "portrait",
            }
        });

        var def = new Def.Album
        {
            Pages = [new Def.Page
            {
                Number = 1,
                Size = "a4",
                Orientation = "landscape",
            }]
        };

        var layout = resolver.Resolve(def);
        var page = layout.Pages[0];

        Assert.Equal("a4", page.Size);
        Assert.Equal("landscape", page.Orientation);
    }

    [Fact]
    public void ResolvePage_DefaultsWhenNoStyle()
    {
        var resolver = CreateResolver(); // no styles

        var def = new Def.Album
        {
            Pages = [new Def.Page { Number = 1 }]
        };

        var layout = resolver.Resolve(def);
        var page = layout.Pages[0];

        Assert.Equal("letter", page.Size);
        Assert.Equal("portrait", page.Orientation);
    }

    // --- Row/Column resolution ---

    [Fact]
    public void ResolveRow_AppliesBgColorFromStyle()
    {
        var resolver = CreateResolver(new Style
        {
            Name = "colored_row", Type = "row",
            Properties = new() { ["bgcolor"] = "PaleGoldenrod" }
        });

        var def = new Def.Album
        {
            Pages = [new Def.Page
            {
                Number = 1,
                Children = [new Def.Row { Style = "colored_row" }]
            }]
        };

        var layout = resolver.Resolve(def);
        var row = Assert.IsType<Lay.Row>(layout.Pages[0].Children[0]);

        Assert.Equal(XColors.PaleGoldenrod, row.BgColor);
    }

    // --- Stamp resolution ---

    [Fact]
    public void ResolveStamp_AppliesDimensionsFromStyle()
    {
        var resolver = CreateResolver(new Style
        {
            Name = "std", Type = "stamp", Default = true,
            Properties = new()
            {
                ["width"] = "30",
                ["height"] = "40",
            }
        });

        var def = new Def.Album
        {
            Pages = [new Def.Page
            {
                Number = 1,
                Children = [new Def.Row
                {
                    Children = [new Def.Stamp()]
                }]
            }]
        };

        var layout = resolver.Resolve(def);
        var row = Assert.IsType<Lay.Row>(layout.Pages[0].Children[0]);
        var stamp = Assert.IsType<Lay.Stamp>(row.Children[0]);

        Assert.Equal(30, stamp.Width);
        Assert.Equal(40, stamp.Height);
    }

    // --- Frame resolution ---

    [Fact]
    public void ResolveFrame_AppliesStyleProperties()
    {
        var resolver = CreateResolver(new Style
        {
            Name = "stamp_frame", Type = "frame", Default = true,
            Properties = new()
            {
                ["frame-width"] = "0.4, 0.4, 0.1",
                ["padding"] = "2",
                ["color"] = "48,48,48",
            }
        });

        var def = new Def.Album
        {
            Pages = [new Def.Page
            {
                Number = 1,
                Children = [new Def.Row
                {
                    Children = [new Def.Frame()]
                }]
            }]
        };

        var layout = resolver.Resolve(def);
        var row = Assert.IsType<Lay.Row>(layout.Pages[0].Children[0]);
        var frame = Assert.IsType<Lay.Frame>(row.Children[0]);

        Assert.Equal(new double[] { 0.4, 0.4, 0.1 }, frame.Lines);
        Assert.Equal(2, frame.Padding);
        Assert.Equal(48, frame.Color.R);
    }

    // --- Nested hierarchy ---

    [Fact]
    public void Resolve_PreservesNestedHierarchy()
    {
        var resolver = CreateResolver();

        var def = new Def.Album
        {
            Pages = [new Def.Page
            {
                Number = 1,
                Children = [new Def.Row
                {
                    Children =
                    [
                        new Def.Column
                        {
                            Children = [new Def.Text { Content = "A" }]
                        },
                        new Def.Column
                        {
                            Children = [new Def.Text { Content = "B" }]
                        },
                    ]
                }]
            }]
        };

        var layout = resolver.Resolve(def);
        var row = Assert.IsType<Lay.Row>(layout.Pages[0].Children[0]);
        Assert.Equal(2, row.Children.Count);
        var col1 = Assert.IsType<Lay.Column>(row.Children[0]);
        var text = Assert.IsType<Lay.Text>(col1.Children[0]);
        Assert.Equal("A", text.Content);
    }

    // --- Integration with Canada6 ---

    private static Def.Album LoadCanada6()
    {
        const string templatesDir = @"C:\My\Git\myalbum\v7.0\Templates";
        var xml = File.ReadAllText(Path.Combine(templatesDir, "Canada6.album"));
        var album = Def.Album.FromXml(XDocument.Parse(xml).Root!);

        // Load external styles (kept separate from embedded)
        if (!string.IsNullOrEmpty(album.StyleFile))
        {
            var stylePath = Path.Combine(templatesDir, album.StyleFile);
            if (File.Exists(stylePath))
            {
                var stylesXml = File.ReadAllText(stylePath);
                album.ExternalStyles = Style.ParseStyles(XDocument.Parse(stylesXml).Root!);
            }
        }

        return album;
    }

    private static StyleSheet BuildStyleSheet(Def.Album album)
    {
        // External first, then embedded (embedded overrides)
        var sheet = StyleResolver.BuildStyleSheet(album.ExternalStyles);
        foreach (var style in album.Styles)
            sheet.Add(style);
        return sheet;
    }

    [Fact]
    public void Resolve_Canada6_ProducesLayoutAlbum()
    {
        var defAlbum = LoadCanada6();

        var sheet = BuildStyleSheet(defAlbum);
        var resolver = new StyleResolver(sheet);
        var layout = resolver.Resolve(defAlbum);

        Assert.Equal(37, layout.Pages.Count);
    }

    [Fact]
    public void Resolve_Canada6_PagesHaveResolvedSizeAndOrientation()
    {
        var defAlbum = LoadCanada6();

        var sheet = BuildStyleSheet(defAlbum);
        var resolver = new StyleResolver(sheet);
        var layout = resolver.Resolve(defAlbum);

        // All pages should have size and orientation resolved
        foreach (var page in layout.Pages)
        {
            Assert.False(string.IsNullOrEmpty(page.Size), $"Page {page.Number} has no size");
            Assert.False(string.IsNullOrEmpty(page.Orientation), $"Page {page.Number} has no orientation");
        }
    }
}
