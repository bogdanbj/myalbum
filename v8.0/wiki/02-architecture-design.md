# MyAlbum v7.0 Architecture Design

## 1. Overview

A layered architecture that separates concerns and enables future GUI/web interfaces.

---

## 2. High-Level Layers

```
┌─────────────────────────────────────────────────────────────┐
│                         Startup                             │
│  ┌─────────────────┐    ┌─────────────────────────────────┐ │
│  │   CLI Parser    │    │       Resource Loader           │ │
│  │ (args, config)  │    │ (fonts, images from app.config) │ │
│  └─────────────────┘    └─────────────────────────────────┘ │
└─────────────────────────────────────────────────────────────┘
                              │
┌─────────────────────────────────────────────────────────────┐
│                     Application Layer                       │
│      (orchestrates resources → parsing → layout → render)   │
└─────────────────────────────────────────────────────────────┘
                              │
   ┌──────────────┬───────────┼───────────┬──────────────┐
   ▼              ▼           ▼           ▼              ▼
┌────────┐  ┌──────────┐  ┌────────┐  ┌─────────┐ ┌───────────┐
│Parsing │  │  Style   │  │ Layout │  │Rendering│ │   PDF     │
│        │  │Resolution│  │        │  │         │ │  Output   │
└────────┘  └──────────┘  └────────┘  └─────────┘ └───────────┘
     │            │            │           │
     └────────────┴────────────┴───────────┘
                       ▼
┌─────────────────────────────────────────────────────────────┐
│                       Domain Models                         │
│         Definition.* (parsed)  |  Layout.* (computed)       │
└─────────────────────────────────────────────────────────────┘
```

**Layer responsibilities:**

| Layer | Responsibility |
|-------|----------------|
| **Startup** | Parse CLI args, load config, load resources (fonts) |
| **Application** | Coordinate parsing → style resolution → layout → rendering |
| **Parsing** | Read .album/.style files, detect format, build Definition.* model |
| **Style Resolution** | Resolve cascade: inline → explicit → default → built-in |
| **Layout** | Calculate positions, sizes → Layout.* model |
| **Rendering** | Generate PDF from Layout.* using PDFsharp |
| **Domain Models** | Definition.* (parsed) and Layout.* (computed) |

---

## 3. Domain Models

### 3.1 Two Separate Models

| Model | Purpose |
|-------|---------|
| **Definition.*** | Parsed from file; immutable input data |
| **Layout.*** | Computed for rendering; positions and resolved styles |

### 3.2 Definition Model (Parsed)

```
Definition.Album
├── title, author, subject (metadata)
├── style-file (reference)
├── styles[] (StyleDef)
└── pages[] (Definition.Page)

Definition.Page
├── number, title, style
├── size, orientation, padding
├── row-spacing, column-spacing
└── children[] (Definition.Element)

Definition.Element (abstract base)
├── style (reference)
├── x, y (optional absolute position)
└── ... element-specific properties

Concrete elements:
  Definition.Row      → padding, spacing, align, children[]
  Definition.Column   → width, padding, spacing, children[]
  Definition.Stamp    → width, height, title, title-padding, image, i1-i3, f1-f3, footer-padding
  Definition.Text     → content, width, align, font-*, color, bgcolor
  Definition.Image    → src, width, height, scale-mode
  Definition.Frame    → width, height, lines[], padding, color
  Definition.Space    → width, height

StyleDef
├── name, type, default
└── ... type-specific properties
```

### 3.3 Layout Model (Computed)

Layout model wraps Definition elements and adds computed properties.

**Coordinate model:** All positions are **relative to parent's content area**.

### 3.4 Element Categories

| Category | Elements | Characteristics |
|----------|----------|-----------------|
| **Leaf** | Text, Image, Frame, Space | No children; pure data |
| **Composite** | Stamp | Fixed named sub-elements (title, frame, interior, footer) |
| **Container** | Row, Column | Variable children list |
| **Container + Composite** | Page | Master elements (fixed) + content children (variable) |

### 3.5 Class Hierarchy

Layout classes are **pure data** — they hold computed positions and resolved styles only.
All calculation logic lives in the Layout Engine; all rendering logic lives in the Rendering layer.

```
Layout.Element (abstract base)
├── X, Y (relative to parent)
├── Width, Height
└── (resolved style properties)

Layout.Container : Layout.Element (abstract)
└── children[]

Concrete elements:
  Layout.Text   : Layout.Element
  Layout.Image  : Layout.Element
  Layout.Frame  : Layout.Element
  Layout.Space  : Layout.Element
  Layout.Row    : Layout.Container
  Layout.Column : Layout.Container
  Layout.Stamp  : Layout.Element  (sub-elements: Title, Frame, Image, I1-I3, F1-F3)
  Layout.Page   : Layout.Container (master elements + content children)
```

### 3.6 Layout.Page Structure

```
Layout.Page : Layout.Container
├── Width, Height (page dimensions)
├── masterElements (fixed sub-elements)
│   ├── Background : Layout.Image
│   ├── Banner : Layout.Image
│   ├── Border : Layout.Frame
│   ├── Header : Layout.Text
│   └── Footer : Layout.Text
└── contentElements[] (variable children)
    └── relative to canvas origin
```

### 3.7 Layout.Stamp Structure

```
Layout.Stamp : Layout.Element
├── X, Y, Width, Height (total bounding box)
├── Title : Layout.Text          (optional)
├── Frame : Layout.Frame
├── Image : Layout.Image         (interior image, optional)
├── I1 : Layout.Text             (interior text line 1, optional)
├── I2 : Layout.Text             (interior text line 2, optional)
├── I3 : Layout.Text             (interior text line 3, optional)
├── F1 : Layout.Text             (footer left, optional)
├── F2 : Layout.Text             (footer center, optional)
└── F3 : Layout.Text             (footer right, optional)

Note: Either Image is set, OR I1/I2/I3 are set — not both.
All sub-elements are Layout.Element instances with their own X, Y, Width, Height.
```

---

## 4. Parser Abstraction

Auto-detect format and parse to Definition model.

```
┌─────────────────────────────────────────────────────────────┐
│                      AlbumLoader                            │
│         (entry point: Load(filePath) → Definition.Album)    │
└─────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────┐
│                    FormatDetector                           │
│      (peek first chars → XML | JSON | YAML)                 │
└─────────────────────────────────────────────────────────────┘
                              │
            ┌─────────────────┼─────────────────┐
            ▼                 ▼                 ▼
     ┌───────────┐     ┌───────────┐     ┌───────────┐
     │ XmlParser │     │JsonParser │     │YamlParser │
     └───────────┘     └───────────┘     └───────────┘
            │                 │                 │
            └─────────────────┼─────────────────┘
                              ▼
                 ┌────────────────────────┐
                 │ Definition.Album       │
                 └────────────────────────┘
```

**Interface:**

```csharp
public interface IAlbumParser
{
    Definition.Album Parse(string content);
}
```

**Implementations:**
- `XmlAlbumParser` — uses `System.Xml.Linq`
- `JsonAlbumParser` — uses `System.Text.Json`
- `YamlAlbumParser` — uses `YamlDotNet`

**Format detection rules:**

| Format | Detection |
|--------|-----------|
| XML | Starts with `<?xml` or `<` |
| JSON | Starts with `{` or `[` |
| YAML | Starts with `---` or key-value pattern |

**StyleLoader** follows same pattern for `.style` files.

---

## 5. Style Resolution

Two-pass approach: resolve styles first, then calculate layout.

```
┌─────────────────────────────────────────────────────────────┐
│                     StyleResolver                           │
│    (entry point: Resolve(Definition.Album) → void)          │
│    (mutates Definition elements with resolved styles)       │
└─────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────┐
│                      StyleSheet                             │
│  (loaded from .style files + embedded styles in .album)     │
│                                                             │
│  - GetDefault(elementType) → StyleDef                       │
│  - Get(styleName) → StyleDef                                │
└─────────────────────────────────────────────────────────────┘
```

**Resolution cascade (highest to lowest priority):**

1. **Inline** — properties on the element itself
2. **Explicit** — referenced by `style` property
3. **Default** — style with `default: true` for that element type
4. **Built-in** — hardcoded fallback values

**Process:**

```
For each element in Definition tree:
  1. Start with built-in defaults for element type
  2. Merge default style (if exists)
  3. Merge explicit style (if referenced)
  4. Merge inline properties
  5. Store resolved values on element
```

---

## 6. Layout Engine

Transforms Definition.* (with resolved styles) into Layout.* (with computed positions).

```
┌─────────────────────────────────────────────────────────────┐
│                     LayoutEngine                            │
│        (entry point: Calculate(Definition.Album)            │
│                         → Layout.Album)                     │
└─────────────────────────────────────────────────────────────┘
                              │
         ┌────────────────────┼────────────────────┐
         ▼                    ▼                    ▼
┌─────────────────┐  ┌─────────────────┐  ┌─────────────────┐
│  PageCalculator │  │ ElementCalculator│ │  TextMeasurer   │
│ (canvas, master │  │ (row, column,   │  │ (text height,   │
│  elements)      │  │  stamp, etc.)   │  │  word wrap)     │
└─────────────────┘  └─────────────────┘  └─────────────────┘
```

**Process per page:**

1. Calculate canvas (page size − padding − banner − border − header − footer)
2. Determine layout mode (row-based or column-based from first child)
3. Calculate children recursively — all positions relative to parent

**Coordinate model:**

```
Layout.Page (origin: page top-left after padding)
│
├── masterElements[] (relative to page origin)
│   ├── Background: X=0, Y=0
│   ├── Banner: X=0, Y=0
│   └── Border: X=0, Y=bannerHeight
│
└── contentElements[] (relative to canvas origin)
    └── Layout.Row: X=0, Y=0
        ├── Layout.Stamp: X=0, Y=0 (relative to row)
        └── Layout.Stamp: X=35, Y=0 (relative to row)
```

---

## 7. Rendering Pipeline

Draws Layout.* elements to PDF using PDFsharp with coordinate transforms.

```
┌─────────────────────────────────────────────────────────────┐
│                      PdfRenderer                            │
│         (entry point: Render(Layout.Album) → PDF file)      │
└─────────────────────────────────────────────────────────────┘
                              │
         ┌────────────────────┼────────────────────┐
         ▼                    ▼                    ▼
┌─────────────────┐  ┌─────────────────┐  ┌─────────────────┐
│  PageRenderer   │  │ ElementRenderers│  │  TextRenderer   │
│                 │  │                 │  │                 │
│ - Summary page  │  │ - RowRenderer   │  │ - Word wrap     │
│ - Master elems  │  │ - ColumnRenderer│  │ - Alignment     │
│ - Content elems │  │ - StampRenderer │  │ - Font lookup   │
│                 │  │ - ImageRenderer │  │ - Measure       │
│                 │  │ - FrameRenderer │  │                 │
└─────────────────┘  └─────────────────┘  └─────────────────┘
```

**Process:**

1. Create `PdfDocument`
2. Add summary page (info, not for printing)
3. For each `Layout.Page`:
   - Create `PdfPage` with size/orientation
   - `gfx.Save()` + `gfx.TranslateTransform()` to page origin
   - Draw master elements
   - Translate to canvas origin
   - Draw content elements recursively (each container saves/translates/restores)
   - `gfx.Restore()`
4. Set PDF metadata (title, author, subject)
5. Save to file

**Rendering pattern for containers:**

```csharp
void RenderRow(XGraphics gfx, Layout.Row row)
{
    var state = gfx.Save();
    gfx.TranslateTransform(row.X, row.Y);
    
    // Draw optional bgcolor
    if (row.BgColor != null)
        gfx.DrawRectangle(new XSolidBrush(row.BgColor), 0, 0, row.Width, row.Height);
    
    foreach (var child in row.Children)
        RenderElement(gfx, child);  // child draws at its relative position
    
    gfx.Restore(state);
}
```

**Element rendering:**

| Element | Renderer draws |
|---------|----------------|
| **Row** | Optional bgcolor; translate & render children |
| **Column** | Optional bgcolor; translate & render children |
| **Stamp** | Title text, frame lines, interior (image or text), footer texts |
| **Text** | Wrapped text block with font, color, alignment, optional bgcolor |
| **Image** | Scaled/clipped image, or placeholder if missing |
| **Frame** | Line/gap/line pattern rectangles |
| **Space** | Nothing (layout only) |

**Frame rendering (lines pattern):**

```
lines: [0.3, 1, 0.3]

┌──────────────────────────────┐  ← outer line (0.3mm)
│                              │
│  ┌────────────────────────┐  │  ← gap (1mm)
│  │                        │  │
│  │  ┌──────────────────┐  │  │  ← inner line (0.3mm)
│  │  │    interior      │  │  │
│  │  └──────────────────┘  │  │
│  │                        │  │
│  └────────────────────────┘  │
│                              │
└──────────────────────────────┘
```

**Missing image placeholder:**

Light gray rectangle with border and two diagonal lines (corner to corner).

**Image clipping (fill mode):**

```csharp
gfx.Save();
gfx.IntersectClip(new XRect(0, 0, targetWidth, targetHeight));
gfx.DrawImage(image, x, y, scaledWidth, scaledHeight);
gfx.Restore();
```

---

## 8. Folder Structure

```
v7.0/
├── MyAlbum.slnx
├── wiki/                           # Documentation
├── samples/                        # Sample album files
└── src/
    └── MyAlbum/
        ├── Program.cs
        ├── MyAlbum.csproj
        │
        ├── Definition/             # Parsed model
        │   ├── Album.cs
        │   ├── Page.cs
        │   ├── Element.cs
        │   ├── Row.cs
        │   ├── Column.cs
        │   ├── Stamp.cs
        │   ├── Text.cs
        │   ├── Image.cs
        │   ├── Frame.cs
        │   ├── Space.cs
        │   └── StyleDef.cs
        │
        ├── Layout/                 # Computed model
        │   ├── Album.cs
        │   ├── Page.cs
        │   ├── Element.cs
        │   ├── Row.cs
        │   ├── Column.cs
        │   ├── Stamp.cs
        │   ├── Text.cs
        │   ├── Image.cs
        │   ├── Frame.cs
        │   └── Space.cs
        │
        ├── Parsing/
        │   ├── IAlbumParser.cs
        │   ├── AlbumLoader.cs
        │   ├── FormatDetector.cs
        │   ├── XmlAlbumParser.cs
        │   ├── JsonAlbumParser.cs
        │   └── YamlAlbumParser.cs
        │
        ├── Styles/
        │   ├── StyleSheet.cs
        │   ├── StyleResolver.cs
        │   └── StyleLoader.cs
        │
        ├── Layout/
        │   ├── LayoutEngine.cs
        │   ├── PageCalculator.cs
        │   ├── ElementCalculator.cs
        │   └── TextMeasurer.cs
        │
        ├── Rendering/
        │   ├── PdfRenderer.cs
        │   ├── PageRenderer.cs
        │   ├── ElementRenderers/
        │   │   ├── RowRenderer.cs
        │   │   ├── ColumnRenderer.cs
        │   │   ├── StampRenderer.cs
        │   │   ├── TextRenderer.cs
        │   │   ├── ImageRenderer.cs
        │   │   └── FrameRenderer.cs
        │   └── TextRenderer.cs
        │
        ├── Resources/
        │   ├── ResourceManager.cs
        │   ├── FontLoader.cs
        │   └── ImageLoader.cs
        │
        └── Cli/
            ├── ArgsParser.cs
            └── ConfigLoader.cs
```

---

## 9. Key Design Decisions

| Decision | Rationale |
|----------|-----------|
| Two separate models (Definition/Layout) | Clean separation; parsed is immutable, layout is computed |
| Pure data models | Models hold only data; calculation in LayoutEngine, rendering in *Renderer classes |
| Relative coordinates | Simpler calculations; matches PDFsharp transform model |
| Two-pass (styles then layout) | Styles are appearance, layout is positioning — separate concerns |
| Auto-detect input format | Single `.album` extension, flexible format choice |
| Style cascade (inline → explicit → default → built-in) | Familiar HTML/CSS paradigm |
| Frame lines as array | Implicit type from length; extensible to any pattern |
| Clipping not cropping | Image data unchanged; rendering bounded |
