using System.Drawing.Text;
using MyAlbum.Common;

namespace FontManager;

/// <summary>
/// Non-modal form that displays a preview of a font family.
/// </summary>
public class FontPreviewForm : Form
{
    private readonly string _familyName;
    private readonly MyAlbum.Common.FontFamily _fontFamily;
    private readonly string _fontsFolder;
    private readonly PrivateFontCollection _fontCollection;

    private const string SampleText = "The quick brown fox jumps over the lazy dog.";
    private const string SampleTextUpper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private const string SampleTextLower = "abcdefghijklmnopqrstuvwxyz";
    private const string SampleNumbers = "0123456789";

    public FontPreviewForm(string familyName, MyAlbum.Common.FontFamily fontFamily, string fontsFolder)
    {
        _familyName = familyName;
        _fontFamily = fontFamily;
        _fontsFolder = fontsFolder;
        _fontCollection = new PrivateFontCollection();

        SetupForm();
        LoadFonts();
        SetupUI();
    }

    private void SetupForm()
    {
        this.Text = _familyName;
        this.Size = new Size(600, 500);
        this.MinimumSize = new Size(400, 300);
        this.StartPosition = FormStartPosition.CenterScreen;
    }

    private void LoadFonts()
    {
        // Load all available font files for this family
        foreach (var file in _fontFamily.AllFiles)
        {
            var path = Path.Combine(_fontsFolder, file);
            if (File.Exists(path))
            {
                try
                {
                    _fontCollection.AddFontFile(path);
                }
                catch
                {
                    // Skip files that can't be loaded
                }
            }
        }
    }

    private void SetupUI()
    {
        var panel = new Panel
        {
            Dock = DockStyle.Fill,
            AutoScroll = true,
            Padding = new Padding(20)
        };

        var y = 20;
        var lineHeight = 10;

        // Get the loaded font family (if available)
        System.Drawing.FontFamily? drawingFamily = null;
        if (_fontCollection.Families.Length > 0)
        {
            drawingFamily = _fontCollection.Families[0];
        }

        // Show preview for each available style
        var styles = new[]
        {
            (Style: MyAlbum.Common.FontStyle.Regular, Name: "Regular", File: _fontFamily.Regular, DrawStyle: System.Drawing.FontStyle.Regular),
            (Style: MyAlbum.Common.FontStyle.Bold, Name: "Bold", File: _fontFamily.Bold, DrawStyle: System.Drawing.FontStyle.Bold),
            (Style: MyAlbum.Common.FontStyle.Italic, Name: "Italic", File: _fontFamily.Italic, DrawStyle: System.Drawing.FontStyle.Italic),
            (Style: MyAlbum.Common.FontStyle.BoldItalic, Name: "Bold Italic", File: _fontFamily.BoldItalic, DrawStyle: System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic)
        };

        foreach (var (style, name, file, drawStyle) in styles)
        {
            if (file == null)
                continue;

            // Style header
            var headerLabel = new Label
            {
                Text = $"{name} ({file})",
                Location = new Point(20, y),
                AutoSize = true,
                Font = new Font("Segoe UI", 9, System.Drawing.FontStyle.Bold)
            };
            panel.Controls.Add(headerLabel);
            y += 25;

            // Try to create font for preview
            Font? previewFont = null;
            
            if (drawingFamily != null && drawingFamily.IsStyleAvailable(drawStyle))
            {
                try
                {
                    previewFont = new Font(drawingFamily, 16, drawStyle);
                }
                catch
                {
                    // Fall back to loading font directly
                }
            }

            // If we couldn't get the font from collection, try loading directly
            if (previewFont == null)
            {
                var fontPath = Path.Combine(_fontsFolder, file);
                if (File.Exists(fontPath))
                {
                    var tempCollection = new PrivateFontCollection();
                    try
                    {
                        tempCollection.AddFontFile(fontPath);
                        if (tempCollection.Families.Length > 0)
                        {
                            var tempFamily = tempCollection.Families[0];
                            // Try regular first, then any available style
                            if (tempFamily.IsStyleAvailable(System.Drawing.FontStyle.Regular))
                                previewFont = new Font(tempFamily, 16, System.Drawing.FontStyle.Regular);
                            else if (tempFamily.IsStyleAvailable(System.Drawing.FontStyle.Bold))
                                previewFont = new Font(tempFamily, 16, System.Drawing.FontStyle.Bold);
                            else if (tempFamily.IsStyleAvailable(System.Drawing.FontStyle.Italic))
                                previewFont = new Font(tempFamily, 16, System.Drawing.FontStyle.Italic);
                        }
                    }
                    catch
                    {
                        // Use fallback
                    }
                }
            }

            // Use system font as fallback
            previewFont ??= new Font("Arial", 16, drawStyle);

            // Sample text
            var sampleLabel = new Label
            {
                Text = SampleText,
                Location = new Point(20, y),
                AutoSize = true,
                Font = previewFont,
                MaximumSize = new Size(this.Width - 80, 0)
            };
            panel.Controls.Add(sampleLabel);
            y += sampleLabel.PreferredHeight + lineHeight;

            // Uppercase
            var upperLabel = new Label
            {
                Text = SampleTextUpper,
                Location = new Point(20, y),
                AutoSize = true,
                Font = previewFont
            };
            panel.Controls.Add(upperLabel);
            y += upperLabel.PreferredHeight + lineHeight;

            // Lowercase
            var lowerLabel = new Label
            {
                Text = SampleTextLower,
                Location = new Point(20, y),
                AutoSize = true,
                Font = previewFont
            };
            panel.Controls.Add(lowerLabel);
            y += lowerLabel.PreferredHeight + lineHeight;

            // Numbers
            var numbersLabel = new Label
            {
                Text = SampleNumbers,
                Location = new Point(20, y),
                AutoSize = true,
                Font = previewFont
            };
            panel.Controls.Add(numbersLabel);
            y += numbersLabel.PreferredHeight + 30; // Extra space between styles
        }

        this.Controls.Add(panel);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _fontCollection.Dispose();
        }
        base.Dispose(disposing);
    }
}
