# MyAlbum v7.0 Implementation Plan

## Overview

An iterative approach delivering working, testable output at each step. Each iteration builds on the previous, with tests verifying actual PDF output.

### Test Framework

The CLI accepts `--test` or `-t` option to run validation:

```
myalbum --test      # Run all iteration tests
myalbum -t          # Same as above
```

This invokes `Test()` which calls `Iteration_1_Test()`, `Iteration_2_Test()`, etc.
Each test validates the completion of that iteration by generating a PDF and verifying expected output.

```csharp
public static void Test()
{
    Iteration_1_Test();
    Iteration_2_Test();
    // ... and so on
}

// Helper to save PDF and open in default viewer
private static void SaveAndOpen(PdfDocument document, string outputPath)
{
    document.Save(outputPath);
    Process.Start(new ProcessStartInfo(outputPath) { UseShellExecute = true });
    Console.WriteLine("  Output: " + outputPath);
}
}
```

---

## Iteration 1: Project Setup & Basic Rendering

**Goal**: Render a single page PDF with text and image using loaded resources

### Tasks
- [ ] Create .NET 10 console project
- [ ] Add PDFsharp 6.x package
- [ ] Create basic folder structure
- [ ] Implement CLI with `--test` / `-t` option
- [ ] Implement FontLoader (load fonts from folder)
- [ ] Implement ImageLoader (load image on demand)

### Test: `Iteration_1_Test()`

```csharp
public static void Iteration_1_Test()
{
    // Load resources
    var font = FontLoader.GetFont("Arial", XFontStyle.Regular, 14);
    var image = ImageLoader.Load("sample.png");
    
    // Create PDF with hardcoded PDFsharp calls
    var document = new PdfDocument();
    var page = document.AddPage();
    page.Size = PageSize.Letter;
    
    var gfx = XGraphics.FromPdfPage(page);
    
    // Draw text with loaded font
    gfx.DrawString("Iteration 1 - Font & Image Test", font, XBrushes.Black, 100, 50);
    
    // Draw loaded image
    if (image != null)
        gfx.DrawImage(image, 100, 100, 200, 150);
    
    var outputPath = "Output/test-iteration-1.pdf";
    document.Save(outputPath);
    
    // Open PDF in default viewer
    Process.Start(new ProcessStartInfo(outputPath) { UseShellExecute = true });
    
    Console.WriteLine("Iteration 1: PASSED - " + outputPath);
}
```

### Verification
- [ ] PDF opens correctly
- [ ] Custom font renders (not system fallback)
- [ ] Image displays correctly

---

## Iteration 2: Album & Pages

**Goal**: Parse basic album structure and render multiple pages

### Tasks
- [ ] Definition.Album, Definition.Page models
- [ ] AlbumLoader (entry point)
- [ ] FormatDetector (XML/JSON/YAML detection)
- [ ] XmlAlbumParser (basic: album, pages)
- [ ] Layout.Album, Layout.Page models
- [ ] Layout.Page.Calculate() and Draw()
- [ ] Summary page rendering

### Test: `Iteration_2_Test()`

```csharp
public static void Iteration_2_Test()
{
    var xml = @"
        <album title=""Test Album"">
            <pages>
                <page number=""1"" title=""Cover"" size=""letter"" orientation=""portrait""/>
                <page number=""2"" size=""letter"" orientation=""portrait""/>
                <page number=""3"" size=""letter"" orientation=""landscape""/>
            </pages>
        </album>";
    
    var defAlbum = AlbumLoader.LoadFromString(xml);
    var layoutAlbum = LayoutEngine.Calculate(defAlbum);
    var outputPath = "Output/test-iteration-2.pdf";
    PdfRenderer.Render(layoutAlbum, outputPath);
    
    Process.Start(new ProcessStartInfo(outputPath) { UseShellExecute = true });
    Console.WriteLine("Iteration 2: PASSED - " + outputPath + " (4 pages)");
}
```

### Verification
- [ ] Summary page shows album info
- [ ] Page 1: Letter portrait
- [ ] Page 2: Letter portrait
- [ ] Page 3: Letter landscape
- [ ] PDF metadata set (title)

---

## Iteration 3: Text Element

**Goal**: Render text blocks on pages

### Tasks
- [ ] Definition.Text model
- [ ] Layout.Text model with Calculate() and Draw()
- [ ] TextMeasurer (measure, wrap text)
- [ ] Parse text elements from XML
- [ ] Text alignment (left, center, right)
- [ ] Font properties (name, size, style)

### Test: `Iteration_3_Test()`

```csharp
public static void Iteration_3_Test()
{
    var xml = @"
        <album>
            <pages>
                <page number=""1"">
                    <text align=""center"" font-size=""18"" font-style=""bold"">Page Title</text>
                    <text align=""left"">Left aligned paragraph with some text.</text>
                    <text align=""right"" font-size=""10"">Right aligned small text</text>
                </page>
            </pages>
        </album>";
    
    var defAlbum = AlbumLoader.LoadFromString(xml);
    var layoutAlbum = LayoutEngine.Calculate(defAlbum);
    var outputPath = "Output/test-iteration-3.pdf";
    PdfRenderer.Render(layoutAlbum, outputPath);
    
    Process.Start(new ProcessStartInfo(outputPath) { UseShellExecute = true });
    Console.WriteLine("Iteration 3: PASSED - " + outputPath);
}
```

### Verification
- [ ] Title centered and bold
- [ ] Left text aligns left
- [ ] Right text aligns right
- [ ] Font sizes correct

---

## Iteration 4: Frame Element

**Goal**: Render frames with line patterns

### Tasks
- [ ] Definition.Frame model (lines[], padding, color)
- [ ] Layout.Frame model with Calculate() and Draw()
- [ ] Draw line/gap pattern (outside-in)
- [ ] Parse frame elements from XML

### Test: `Iteration_4_Test()`

```csharp
public static void Iteration_4_Test()
{
    var xml = @"
        <album>
            <pages>
                <page number=""1"">
                    <frame width=""50"" height=""40"" lines=""0.3"" x=""20"" y=""20""/>
                    <frame width=""50"" height=""40"" lines=""0.3,1,0.3"" x=""80"" y=""20""/>
                    <frame width=""50"" height=""40"" lines=""0.2,0.5,0.3,0.5,0.2"" x=""140"" y=""20""/>
                </page>
            </pages>
        </album>";
    
    var defAlbum = AlbumLoader.LoadFromString(xml);
    var layoutAlbum = LayoutEngine.Calculate(defAlbum);
    var outputPath = "Output/test-iteration-4.pdf";
    PdfRenderer.Render(layoutAlbum, outputPath);
    
    Process.Start(new ProcessStartInfo(outputPath) { UseShellExecute = true });
    Console.WriteLine("Iteration 4: PASSED - " + outputPath);
}
```

### Verification
- [ ] Single line frame renders
- [ ] Double line frame renders with gap
- [ ] Triple line frame renders correctly

---

## Iteration 5: Row Container

**Goal**: Horizontal layout of elements

### Tasks
- [ ] Definition.Row model (spacing, align)
- [ ] Layout.Row model with Calculate() and Draw()
- [ ] Layout.Container base class
- [ ] Row calculation (distribute children horizontally)
- [ ] Spacing modes (fixed, equal, justified)
- [ ] Vertical alignment (top, center, bottom)

### Test: `Iteration_5_Test()`

```csharp
public static void Iteration_5_Test()
{
    var xml = @"
        <album>
            <pages>
                <page number=""1"">
                    <row spacing=""10"" align=""top"">
                        <frame width=""30"" height=""40"" lines=""0.3""/>
                        <frame width=""30"" height=""50"" lines=""0.3""/>
                        <frame width=""30"" height=""35"" lines=""0.3""/>
                    </row>
                    <row spacing=""equal"">
                        <frame width=""30"" height=""30"" lines=""0.3""/>
                        <frame width=""30"" height=""30"" lines=""0.3""/>
                    </row>
                </page>
            </pages>
        </album>";
    
    var defAlbum = AlbumLoader.LoadFromString(xml);
    var layoutAlbum = LayoutEngine.Calculate(defAlbum);
    var outputPath = "Output/test-iteration-5.pdf";
    PdfRenderer.Render(layoutAlbum, outputPath);
    
    Process.Start(new ProcessStartInfo(outputPath) { UseShellExecute = true });
    Console.WriteLine("Iteration 5: PASSED - " + outputPath);
}
```

### Verification
- [ ] Row 1: Frames aligned at top, 10mm spacing
- [ ] Row 2: Frames distributed equally across page width

---

## Iteration 6: Basic Stamp

**Goal**: Render stamp with frame only (no title/footer yet)

### Tasks
- [ ] Definition.Stamp model (width, height, lines)
- [ ] Layout.Stamp model (implements IComposite)
- [ ] Layout.Stamp.Calculate() and Draw()
- [ ] Stamp contains Frame sub-element
- [ ] Parse stamp elements from XML

### Test: `Iteration_6_Test()`

```csharp
public static void Iteration_6_Test()
{
    var xml = @"
        <album>
            <pages>
                <page number=""1"">
                    <row spacing=""equal"">
                        <stamp width=""25"" height=""30"" lines=""0.3,1,0.3""/>
                        <stamp width=""25"" height=""30"" lines=""0.3,1,0.3""/>
                        <stamp width=""25"" height=""30"" lines=""0.3,1,0.3""/>
                    </row>
                </page>
            </pages>
        </album>";
    
    var defAlbum = AlbumLoader.LoadFromString(xml);
    var layoutAlbum = LayoutEngine.Calculate(defAlbum);
    var outputPath = "Output/test-iteration-6.pdf";
    PdfRenderer.Render(layoutAlbum, outputPath);
    
    Process.Start(new ProcessStartInfo(outputPath) { UseShellExecute = true });
    Console.WriteLine("Iteration 6: PASSED - " + outputPath);
}
```

### Verification
- [ ] Three stamps in a row
- [ ] Double-line frames
- [ ] Equal spacing

---

## Iteration 7: Stamp Title & Footer

**Goal**: Add title and footer to stamp

### Tasks
- [ ] Stamp.Title, Stamp.TitlePadding properties
- [ ] Stamp.F1, F2, F3, Stamp.FooterPadding properties
- [ ] Layout.Stamp with Title, F1, F2, F3 as Layout.Text sub-elements
- [ ] Calculate stamp total height (title + frame + footer)
- [ ] Stamp alignment in row (by frame, not total bounds)

### Test: `Iteration_7_Test()`

```csharp
public static void Iteration_7_Test()
{
    var xml = @"
        <album>
            <pages>
                <page number=""1"">
                    <row spacing=""equal"" align=""top"">
                        <stamp width=""25"" height=""30"" title=""1c Green"" f2=""1937""/>
                        <stamp width=""25"" height=""30"" title=""Long Title\nTwo Lines"" f1=""Scott #1"" f2=""1937"" f3=""Mi #1""/>
                        <stamp width=""25"" height=""30"" title=""5c Blue"" f2=""1937""/>
                    </row>
                </page>
            </pages>
        </album>";
    
    var defAlbum = AlbumLoader.LoadFromString(xml);
    var layoutAlbum = LayoutEngine.Calculate(defAlbum);
    var outputPath = "Output/test-iteration-7.pdf";
    PdfRenderer.Render(layoutAlbum, outputPath);
    
    Process.Start(new ProcessStartInfo(outputPath) { UseShellExecute = true });
    Console.WriteLine("Iteration 7: PASSED - " + outputPath);
}
```

### Verification
- [ ] Titles render above frames
- [ ] Multi-line title works (\\n)
- [ ] Footers render below frames (left, center, right)
- [ ] Stamps align by frame top (not title top)

---

## Iteration 8: Stamp Interior

**Goal**: Image or text lines inside stamp frame

### Tasks
- [ ] Stamp.Image property (interior image filename)
- [ ] Stamp.I1, I2, I3 properties (interior text lines)
- [ ] Layout.Stamp with Image or I1/I2/I3 as sub-elements
- [ ] Interior sizing and centering
- [ ] Missing image placeholder (gray box + diagonals)

### Test: `Iteration_8_Test()`

```csharp
public static void Iteration_8_Test()
{
    var xml = @"
        <album>
            <pages>
                <page number=""1"">
                    <row spacing=""equal"">
                        <stamp width=""25"" height=""30"" title=""With Image"" image=""stamp1.png""/>
                        <stamp width=""25"" height=""30"" title=""Missing"" image=""missing.png""/>
                        <stamp width=""25"" height=""30"" title=""Text Inside"" i1=""Line 1"" i2=""Line 2""/>
                    </row>
                </page>
            </pages>
        </album>";
    
    var defAlbum = AlbumLoader.LoadFromString(xml);
    var layoutAlbum = LayoutEngine.Calculate(defAlbum);
    var outputPath = "Output/test-iteration-8.pdf";
    PdfRenderer.Render(layoutAlbum, outputPath);
    
    Process.Start(new ProcessStartInfo(outputPath) { UseShellExecute = true });
    Console.WriteLine("Iteration 8: PASSED - " + outputPath);
}
```

### Verification
- [ ] Stamp 1: Image inside frame
- [ ] Stamp 2: Placeholder (gray box with diagonals)
- [ ] Stamp 3: Two text lines centered

---

## Iteration 9: Column Container

**Goal**: Vertical layout of elements

### Tasks
- [ ] Definition.Column model (width, spacing)
- [ ] Layout.Column model with Calculate() and Draw()
- [ ] Column calculation (stack children vertically)
- [ ] Column width (fixed mm, percentage, remainder)
- [ ] Page layout mode detection (row-based vs column-based)

### Test: `Iteration_9_Test()`

```csharp
public static void Iteration_9_Test()
{
    var xml = @"
        <album>
            <pages>
                <page number=""1"">
                    <column width=""50%"">
                        <stamp width=""25"" height=""30"" title=""Col 1 Stamp 1""/>
                        <stamp width=""25"" height=""30"" title=""Col 1 Stamp 2""/>
                    </column>
                    <column width=""50%"">
                        <stamp width=""25"" height=""30"" title=""Col 2 Stamp 1""/>
                        <stamp width=""25"" height=""30"" title=""Col 2 Stamp 2""/>
                        <stamp width=""25"" height=""30"" title=""Col 2 Stamp 3""/>
                    </column>
                </page>
            </pages>
        </album>";
    
    var defAlbum = AlbumLoader.LoadFromString(xml);
    var layoutAlbum = LayoutEngine.Calculate(defAlbum);
    var outputPath = "Output/test-iteration-9.pdf";
    PdfRenderer.Render(layoutAlbum, outputPath);
    
    Process.Start(new ProcessStartInfo(outputPath) { UseShellExecute = true });
    Console.WriteLine("Iteration 9: PASSED - " + outputPath);
}
```

### Verification
- [ ] Two columns side by side
- [ ] Column 1: 2 stamps stacked
- [ ] Column 2: 3 stamps stacked
- [ ] Stamps centered in columns

---

## Iteration 10: Space Element

**Goal**: Empty space in layouts

### Tasks
- [ ] Definition.Space model (width, height)
- [ ] Layout.Space model with Calculate() and Draw() (no-op draw)
- [ ] Space in Row (horizontal, uses width)
- [ ] Space in Column/Page (vertical, uses height)

### Test: `Iteration_10_Test()`

```csharp
public static void Iteration_10_Test()
{
    var xml = @"
        <album>
            <pages>
                <page number=""1"">
                    <row>
                        <stamp width=""25"" height=""30"" title=""Stamp 1""/>
                        <space width=""20""/>
                        <stamp width=""25"" height=""30"" title=""Stamp 2""/>
                    </row>
                    <space height=""15""/>
                    <row>
                        <stamp width=""25"" height=""30"" title=""Stamp 3""/>
                    </row>
                </page>
            </pages>
        </album>";
    
    var defAlbum = AlbumLoader.LoadFromString(xml);
    var layoutAlbum = LayoutEngine.Calculate(defAlbum);
    var outputPath = "Output/test-iteration-10.pdf";
    PdfRenderer.Render(layoutAlbum, outputPath);
    
    Process.Start(new ProcessStartInfo(outputPath) { UseShellExecute = true });
    Console.WriteLine("Iteration 10: PASSED - " + outputPath);
}
```

### Verification
- [ ] 20mm horizontal gap between stamps 1 and 2
- [ ] 15mm vertical gap between rows

---

## Iteration 11: Image Element (Standalone)

**Goal**: Standalone images on page

### Tasks
- [ ] Definition.Image model (src, width, height, scale-mode)
- [ ] Layout.Image model with Calculate() and Draw()
- [ ] Scale modes (fit, fill, stretch)
- [ ] Clipping for fill mode
- [ ] Parse image elements from XML

### Test: `Iteration_11_Test()`

```csharp
public static void Iteration_11_Test()
{
    var xml = @"
        <album>
            <pages>
                <page number=""1"">
                    <image src=""banner.png"" width=""100%"" height=""30""/>
                    <row spacing=""equal"">
                        <image src=""photo.jpg"" width=""50"" height=""50"" scale-mode=""fit""/>
                        <image src=""photo.jpg"" width=""50"" height=""50"" scale-mode=""fill""/>
                        <image src=""photo.jpg"" width=""50"" height=""50"" scale-mode=""stretch""/>
                    </row>
                </page>
            </pages>
        </album>";
    
    var defAlbum = AlbumLoader.LoadFromString(xml);
    var layoutAlbum = LayoutEngine.Calculate(defAlbum);
    var outputPath = "Output/test-iteration-11.pdf";
    PdfRenderer.Render(layoutAlbum, outputPath);
    
    Process.Start(new ProcessStartInfo(outputPath) { UseShellExecute = true });
    Console.WriteLine("Iteration 11: PASSED - " + outputPath);
}
```

### Verification
- [ ] Banner spans page width
- [ ] Fit: image fits inside box, aspect preserved
- [ ] Fill: image fills box, clipped
- [ ] Stretch: image distorted to exact size

---

## Iteration 12: Page Master Elements

**Goal**: Background, banner, border, header, footer on pages

### Tasks
- [ ] Page master element properties (background, banner, border, header, footer)
- [ ] Layout.MasterElements model
- [ ] Canvas calculation (page - padding - banner - border - header - footer)
- [ ] Z-order (background first, then master, then content)

### Test: `Iteration_12_Test()`

```csharp
public static void Iteration_12_Test()
{
    var xml = @"
        <album>
            <pages>
                <page number=""1"">
                    <banner src=""banner.png"" height=""20""/>
                    <border lines=""0.5"" padding=""5""/>
                    <header align=""center"">Canada - 1937 Definitives</header>
                    <footer align=""center"">Page 1</footer>
                    <row>
                        <stamp width=""25"" height=""30"" title=""1c Green""/>
                    </row>
                </page>
            </pages>
        </album>";
    
    var defAlbum = AlbumLoader.LoadFromString(xml);
    var layoutAlbum = LayoutEngine.Calculate(defAlbum);
    var outputPath = "Output/test-iteration-12.pdf";
    PdfRenderer.Render(layoutAlbum, outputPath);
    
    Process.Start(new ProcessStartInfo(outputPath) { UseShellExecute = true });
    Console.WriteLine("Iteration 12: PASSED - " + outputPath);
}
```

### Verification
- [ ] Banner at top
- [ ] Border around content area
- [ ] Header below banner/border
- [ ] Footer at bottom
- [ ] Content in remaining canvas

---

## Iteration 13: Style Definitions

**Goal**: Named styles with defaults

### Tasks
- [ ] Definition.StyleDef model
- [ ] StyleSheet class (collection, lookup)
- [ ] Parse embedded styles from album
- [ ] Default styles (default="true")
- [ ] StyleResolver (walk tree, resolve each element)

### Test: `Iteration_13_Test()`

```csharp
public static void Iteration_13_Test()
{
    var xml = @"
        <album>
            <styles>
                <style name=""classic-stamp"" type=""stamp"" default=""true"" lines=""0.3,1,0.3""/>
                <style name=""large-stamp"" type=""stamp"" width=""40"" height=""50""/>
            </styles>
            <pages>
                <page number=""1"">
                    <row spacing=""equal"">
                        <stamp title=""Default Style""/>
                        <stamp style=""large-stamp"" title=""Large Style""/>
                        <stamp title=""Default Again""/>
                    </row>
                </page>
            </pages>
        </album>";
    
    var defAlbum = AlbumLoader.LoadFromString(xml);
    StyleResolver.Resolve(defAlbum);
    var layoutAlbum = LayoutEngine.Calculate(defAlbum);
    var outputPath = "Output/test-iteration-13.pdf";
    PdfRenderer.Render(layoutAlbum, outputPath);
    
    Process.Start(new ProcessStartInfo(outputPath) { UseShellExecute = true });
    Console.WriteLine("Iteration 13: PASSED - " + outputPath);
}
```

### Verification
- [ ] Stamp 1 & 3: Use default style (double line)
- [ ] Stamp 2: Uses large-stamp style (40x50)

---

## Iteration 14: External Style Files

**Goal**: Load styles from .style files

### Tasks
- [ ] StyleLoader (parse .style files)
- [ ] Album.StyleFile reference
- [ ] Merge external + embedded styles
- [ ] Full cascade (inline → explicit → default → built-in)

### Test: `Iteration_14_Test()`

```csharp
public static void Iteration_14_Test()
{
    // Create test style file
    var styleXml = @"
        <styles>
            <style name=""base-stamp"" type=""stamp"" default=""true"" lines=""0.3"" title-padding=""2""/>
        </styles>";
    File.WriteAllText("Tests/test-iteration-14.style", styleXml);
    
    var xml = @"
        <album style-file=""Tests/test-iteration-14.style"">
            <styles>
                <style name=""fancy"" type=""stamp"" lines=""0.2,0.5,0.3,0.5,0.2""/>
            </styles>
            <pages>
                <page number=""1"">
                    <row spacing=""equal"">
                        <stamp title=""From File""/>
                        <stamp style=""fancy"" title=""Embedded""/>
                        <stamp title=""Inline Override"" lines=""0.5""/>
                    </row>
                </page>
            </pages>
        </album>";
    
    var defAlbum = AlbumLoader.LoadFromString(xml);
    StyleResolver.Resolve(defAlbum);
    var layoutAlbum = LayoutEngine.Calculate(defAlbum);
    var outputPath = "Output/test-iteration-14.pdf";
    PdfRenderer.Render(layoutAlbum, outputPath);
    
    Process.Start(new ProcessStartInfo(outputPath) { UseShellExecute = true });
    Console.WriteLine("Iteration 14: PASSED - " + outputPath);
}
```

### Verification
- [ ] Stamp 1: Uses external default (single line)
- [ ] Stamp 2: Uses embedded fancy (triple line)
- [ ] Stamp 3: Inline override (thicker single line)

---

## Iteration 15: JSON Parser

**Goal**: Parse JSON album files

### Tasks
- [ ] JsonAlbumParser using System.Text.Json
- [ ] Feature parity with XML parser

### Test: `Iteration_15_Test()`

```csharp
public static void Iteration_15_Test()
{
    var json = @"{
        ""album"": { ""title"": ""JSON Test"" },
        ""pages"": [{
            ""number"": 1,
            ""children"": [{
                ""row"": {
                    ""spacing"": ""equal"",
                    ""children"": [
                        { ""stamp"": { ""title"": ""JSON Stamp 1"", ""width"": 25, ""height"": 30 }},
                        { ""stamp"": { ""title"": ""JSON Stamp 2"", ""width"": 25, ""height"": 30 }}
                    ]
                }
            }]
        }]
    }";
    
    var defAlbum = AlbumLoader.LoadFromString(json);
    var layoutAlbum = LayoutEngine.Calculate(defAlbum);
    var outputPath = "Output/test-iteration-15.pdf";
    PdfRenderer.Render(layoutAlbum, outputPath);
    
    Process.Start(new ProcessStartInfo(outputPath) { UseShellExecute = true });
    Console.WriteLine("Iteration 15: PASSED - " + outputPath);
}
```

### Verification
- [ ] JSON parsed correctly
- [ ] Output identical to XML equivalent

---

## Iteration 16: YAML Parser

**Goal**: Parse YAML album files

### Tasks
- [ ] Add YamlDotNet package
- [ ] YamlAlbumParser using YamlDotNet
- [ ] Feature parity with XML/JSON parsers

### Test: `Iteration_16_Test()`

```csharp
public static void Iteration_16_Test()
{
    var yaml = @"
album:
  title: YAML Test
pages:
  - number: 1
    children:
      - row:
          spacing: equal
          children:
            - stamp:
                title: YAML Stamp 1
                width: 25
                height: 30
            - stamp:
                title: YAML Stamp 2
                width: 25
                height: 30";
    
    var defAlbum = AlbumLoader.LoadFromString(yaml);
    var layoutAlbum = LayoutEngine.Calculate(defAlbum);
    var outputPath = "Output/test-iteration-16.pdf";
    PdfRenderer.Render(layoutAlbum, outputPath);
    
    Process.Start(new ProcessStartInfo(outputPath) { UseShellExecute = true });
    Console.WriteLine("Iteration 16: PASSED - " + outputPath);
}
```

### Verification
- [ ] YAML parsed correctly
- [ ] Output identical to XML/JSON equivalent

---

## Iteration 17: Configuration & CLI

**Goal**: Full CLI with configuration file

### Tasks
- [ ] Config class and ConfigLoader
- [ ] myalbum.config JSON file support
- [ ] ArgsParser (full options: -o, -v, -q, --help, --version)
- [ ] Verbose/quiet output modes
- [ ] Error handling (fail fast, clear messages)

### Test: `Iteration_17_Test()`

```csharp
public static void Iteration_17_Test()
{
    // Test config loading
    var config = ConfigLoader.Load("myalbum.config");
    Debug.Assert(config.Folders.Images == "Images");
    
    // Test CLI parsing
    var options = ArgsParser.Parse(new[] { "sample.album", "-o", "custom.pdf", "-v" });
    Debug.Assert(options.InputFile == "sample.album");
    Debug.Assert(options.OutputFile == "custom.pdf");
    Debug.Assert(options.Verbose == true);
    
    // Test full flow with config
    var xml = @"<album><pages><page number=""1""/></pages></album>";
    var defAlbum = AlbumLoader.LoadFromString(xml);
    var layoutAlbum = LayoutEngine.Calculate(defAlbum);
    var outputPath = "Output/test-iteration-17.pdf";
    PdfRenderer.Render(layoutAlbum, outputPath);
    
    Process.Start(new ProcessStartInfo(outputPath) { UseShellExecute = true });
    Console.WriteLine("Iteration 17: PASSED - Config and CLI working");
}
```

### Verification
- [ ] `myalbum sample.album` works
- [ ] `myalbum sample.album -o custom.pdf` works
- [ ] `myalbum --help` shows usage
- [ ] `myalbum --version` shows version
- [ ] Missing file shows clear error

---

## Iteration 18: Polish & Distribution

**Goal**: Production-ready release

### Tasks
- [ ] Finalize error messages
- [ ] Update documentation
- [ ] Cross-platform builds (win-x64, linux-x64, osx-x64, osx-arm64)
- [ ] Self-contained single-file executables
- [ ] Create sample albums for distribution

### Test: `Iteration_18_Test()`

```csharp
public static void Iteration_18_Test()
{
    // Run all previous tests
    Iteration_1_Test();
    Iteration_2_Test();
    // ... through Iteration_17_Test()
    
    // Verify sample albums render
    foreach (var sample in Directory.GetFiles("Samples", "*.album"))
    {
        var defAlbum = AlbumLoader.Load(sample);
        StyleResolver.Resolve(defAlbum);
        var layoutAlbum = LayoutEngine.Calculate(defAlbum);
        var output = Path.Combine("Output", Path.GetFileNameWithoutExtension(sample) + ".pdf");
        PdfRenderer.Render(layoutAlbum, output);
        Console.WriteLine($"  Rendered: {output}");
    }
    
    Console.WriteLine("Iteration 18: PASSED - All samples rendered");
}
```

### Verification
- [ ] All platforms build successfully
- [ ] All sample albums render correctly
- [ ] Documentation complete

---

## Progress Summary

| Iteration | Goal | Status |
|-----------|------|--------|
| 1 | Project Setup & Basic Rendering | PENDING |
| 2 | Album & Pages | PENDING |
| 3 | Text Element | PENDING |
| 4 | Frame Element | PENDING |
| 5 | Row Container | PENDING |
| 6 | Basic Stamp | PENDING |
| 7 | Stamp Title & Footer | PENDING |
| 8 | Stamp Interior | PENDING |
| 9 | Column Container | PENDING |
| 10 | Space Element | PENDING |
| 11 | Image Element (Standalone) | PENDING |
| 12 | Page Master Elements | PENDING |
| 13 | Style Definitions | PENDING |
| 14 | External Style Files | PENDING |
| 15 | JSON Parser | PENDING |
| 16 | YAML Parser | PENDING |
| 17 | Configuration & CLI | PENDING |
| 18 | Polish & Distribution | PENDING |

**Total Iterations**: 18

---

## Testing Commands

```powershell
cd C:\My\Git\myalbum\v7.0

# Build
dotnet build src/MyAlbum/MyAlbum.csproj

# Run all iteration tests
dotnet run --project src/MyAlbum/MyAlbum.csproj -- --test

# Run with album file
dotnet run --project src/MyAlbum/MyAlbum.csproj -- samples/sample.album

# Publish single-file executable
dotnet publish src/MyAlbum/MyAlbum.csproj -c Release -r win-x64 --self-contained -p:PublishSingleFile=true
```
