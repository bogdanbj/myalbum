# MyAlbum v7.0 - Requirements Specification

## Table of Contents

1. [Overview](#1-overview)
2. [Input](#2-input)
3. [Output](#3-output)
4. [Layout Elements](#4-layout-elements)
5. [Album](#5-album)
6. [Page](#6-page)
7. [Row](#7-row)
8. [Column](#8-column)
9. [Stamp](#9-stamp)
10. [Text](#10-text)
11. [Image](#11-image)
12. [Frame](#12-frame)
13. [Space](#13-space)
14. [Style System](#14-style-system)
15. [Configuration](#15-configuration)
16. [Technical Implementation](#16-technical-implementation)
17. [To Be Defined](#17-to-be-defined)

---

## 1. Overview

**MyAlbum** is a stamp album PDF generator for philatelists. Users provide album definition files (`.album`) describing content and styling, and the application generates professional PDF album pages.

| Aspect | Description |
|--------|-------------|
| **Target audience** | Personal use; future freeware distribution |
| **Application type** | CLI (Phase 1); GUI/web planned (Phase 2) |
| **Core design** | Logic decoupled from interface |

---

## 2. Input

The application accepts the following input files:

| File Type | Extension | Description |
|-----------|-----------|-------------|
| Album files | `.album` | Album definition (content and structure) |
| Style files | `.style` | Reusable style definitions |
| Image files | `.jpg`, `.png`, `.tiff`, `.webp`, `.gif` | Images for stamps, banners, backgrounds |
| Font files | `.ttf`, `.otf` | Custom fonts for text rendering |

### 2.1 Album Files

| Property | Value |
|----------|-------|
| **Extension** | `.album` |
| **Default folder** | `/MyAlbum/Templates` (configurable) |
| **Format** | Auto-detected from content |

**Format detection rules:**

| Format | Detection |
|--------|-----------|
| XML | Starts with `<?xml` or `<` |
| JSON | Starts with `{` or `[` |
| YAML | Starts with `---` or key-value pattern |

### 2.2 Style Files

| Property | Value |
|----------|-------|
| **Extension** | `.style` |
| **Default folder** | `/MyAlbum/Templates` (configurable) |
| **Format** | Auto-detected (same rules as album files) |

See [14. Style System](#14-style-system) for details.

### 2.3 Image Files

| Property | Value |
|----------|-------|
| **Default folder** | `/MyAlbum/Images` (configurable) |
| **Supported formats** | JPEG, PNG, TIFF, WebP, GIF |

### 2.4 Font Files

| Property | Value |
|----------|-------|
| **Default folder** | `/MyAlbum/Fonts` (configurable) |
| **Loading** | All fonts loaded at program start |
| **Supported formats** | TrueType (.ttf), OpenType (.otf) |

See [14.4 Font Attributes](#144-font-attributes) for styling options.

---

## 3. Output

### 3.1 PDF Generation

| Property | Value |
|----------|-------|
| **Default folder** | `/MyAlbum/Output` (configurable) |
| **File naming** | Same as input with `.pdf` extension |
| **Conflict handling** | Append `_1`, `_2`, etc. |

### 3.2 Summary Page

First page of generated PDF (informational, not for printing):
- Input file name
- MyAlbum version
- Total page count

### 3.3 PDF Metadata

Populated from album properties: `title`, `author`, `subject`

---

## 4. Layout Elements

### 4.1 Element Hierarchy

```
Album
└── Page
    ├── Row
    │   ├── Stamp, Text, Image, Frame, Space
    │   └── Column (nested)
    ├── Column
    |   ├── Stamp, Text, Image, Frame, Space
    |   └── Row (nested)
    └── Stamp, Text, Image, Frame, Space
```

### 4.2 Container Elements

| Element | Description |
|---------|-------------|
| **Album** | Root container; holds metadata and pages |
| **Page** | Single album page |
| **Row** | Horizontal arrangement of children |
| **Column** | Vertical arrangement of children |

### 4.3 Drawable Elements

| Element | Description |
|---------|-------------|
| **Stamp** | Composite element with frame, title, interior, footer |
| **Text** | Text block with wrapping and alignment |
| **Image** | Rendered image with scaling options |
| **Frame** | Decorative border element |
| **Space** | Empty space (vertical in Page/Column, horizontal in Row) |

---

## 5. Album

### 5.1 Description

The Album is the root container representing an entire stamp album.

### 5.2 Properties

| Property | Type | Description | Required |
|----------|------|-------------|----------|
| `title` | string | Album title (PDF metadata) | No |
| `author` | string | Author name (PDF metadata) | No |
| `subject` | string | Subject description (PDF metadata) | No |
| `style-file` | string | Reference to external `.style` file | No |
| `styles` | list | Embedded style definitions | No |
| `pages` | list | Collection of Page elements | Yes |

### 5.3 Structure Example

```yaml
album:
  title: "Canada - Queen Elizabeth II Definitives"
  author: "Collector Name"
  subject: "Stamp collection album"

style-file: "classic.style"

styles:
  - name: my-stamp
    type: stamp
    default: true
    frame: double
    width: 25
    height: 30

pages:
  - number: 1
    style: my-page
    children:
      - stamp:
          title: "1c Green"
      - stamp:
          style: large-stamp
          title: "5c Blue"
```

---

## 6. Page

### 6.1 Description

The Page element represents a single album page in the PDF output.

### 6.2 Properties

| Property | Type | Description | Default |
|----------|------|-------------|---------|
| `number` | integer | Page number | Required |
| `title` | string | Metadata only (not rendered) | - |
| `style` | string | Reference to page style | - |
| `size` | string | `letter`, `a4`, `legal` | TBD |
| `orientation` | string | `portrait`, `landscape` | TBD |
| `padding` | number/list | Interior spacing (mm) | 0 |
| `row-spacing` | number | Space between rows (mm) | 0 |
| `column-spacing` | number | Space between columns (mm) | 0 |
| `children` | list | Child elements | - |

**Notes:**
- Pages have no margin (exterior spacing)
- `padding` accepts 1, 2, or 4 values (see [14.1 Units](#141-units-of-measurement))

### 6.3 Page Style Elements

Defined in page styles; applied to all pages using that style:

| Element | Description |
|---------|-------------|
| `background` | Background image (rendered first, lowest z-order) |
| `banner` | Image at top of page |
| `border` | Frame around content area |
| `header` | Text at top of content area |
| `footer` | Text at bottom of content area |

**Background behavior:**
- Absolute positioned (uses `x`, `y`, `width`, `height`)
- Page children render on top
- Standard image sizing rules apply

**Orientation behavior:**
Style elements remain on the same physical edge regardless of portrait/landscape orientation.

### 6.4 Canvas Calculation

The content area (canvas) is calculated by reducing page dimensions:

| Order | Element | Reduction |
|-------|---------|-----------|
| 1 | Page padding | All sides |
| 2 | Banner | Height from top |
| 3 | Border | Thickness + padding |
| 4 | Header | Height from top |
| 5 | Footer | Height from bottom |
| 6 | **Canvas** | Remaining space |

### 6.5 Layout Mode

Determined by the **first child element**:

| First Child | Mode | Behavior |
|-------------|------|----------|
| Column | Column-based | Children are columns arranged horizontally |
| Any other | Row-based | Children are rows stacked vertically |

### 6.6 Absolute Positioning

Elements with `x`, `y` coordinates are absolutely positioned:
- Position relative to page top-left corner (after padding)
- Removed from normal flow
- Do not affect subsequent elements

### 6.7 Example

```yaml
pages:
  - number: 1
    style: standard-page
    padding: 10
    row-spacing: 5
    children:
      - text:
          content: "Canada - 1937 Definitives"
          align: center
      - row:
          children:
            - stamp:
                title: "1c Green"
                width: 25
                height: 30
```

---

## 7. Row

### 7.1 Description

The Row element arranges its children horizontally.

### 7.2 Properties

| Property | Type | Description | Default |
|----------|------|-------------|---------|
| `style` | string | Reference to row style | - |
| `padding` | number/list | Interior spacing (mm) | 0 |
| `spacing` | number/string | Horizontal spacing between children | TBD |
| `align` | string | Vertical alignment: `top`, `center`, `bottom` | TBD |
| `children` | list | Child elements | - |

### 7.3 Permitted Children

| Element | Width Behavior |
|---------|----------------|
| Stamp | Fixed (frame width) |
| Text | Must be specified |
| Image | Must be specified |
| Frame | Must be specified |
| Space | Must be specified |
| Column | Percentage or fixed |

### 7.4 Dimensions

| Property | Value |
|----------|-------|
| Width | 100% of parent canvas |
| Height | Calculated from children + alignment |

### 7.5 Height Calculation

Based on vertical alignment:

1. For each child, calculate height above and below alignment line
2. Row height = max(height above) + max(height below)

**Top alignment:** Alignment line at top; height = tallest child
**Center alignment:** Alignment line at vertical center
**Bottom alignment:** Alignment line at bottom; height = tallest child

### 7.6 Spacing Modes

| Mode | Value | Behavior |
|------|-------|----------|
| Fixed | `spacing: 5` | Fixed mm between children; group centered |
| Equal | `spacing: equal` | `(row width - children) / (n - 1)`; edges touch row edges |
| Justified | `spacing: justified` | `(row width - children) / (n + 1)`; equal gaps including edges |

### 7.7 Stamp Alignment

In rows, stamps align based on their **frame** (not total bounding box including title/footer):

```
Stamp A (1-line title)      Stamp B (2-line title)

                                  [Title Line 1]
         [Title]                  [Title Line 2]
    ┌─────────────┐          ┌─────────────┐      ← Frames aligned at top
    │   Frame     │          │   Frame     │
    └─────────────┘          └─────────────┘
```

### 7.8 Example

```yaml
- row:
    spacing: 10
    align: top
    children:
      - stamp:
          title: "1c Green"
          width: 25
          height: 30
      - stamp:
          title: "2c Red"
          width: 25
          height: 30
```

---

## 8. Column

### 8.1 Description

The Column element arranges its children vertically.

### 8.2 Properties

| Property | Type | Description | Default |
|----------|------|-------------|---------|
| `style` | string | Reference to column style | - |
| `width` | number/string | Width in mm or percentage | Remainder |
| `padding` | number/list | Interior spacing (mm) | 0 |
| `spacing` | number | Vertical space between children (mm) | 0 |
| `children` | list | Child elements | - |

### 8.3 Permitted Children

| Element | Width Behavior |
|---------|----------------|
| Stamp | Fixed (frame width), centered |
| Text | Defaults to column width |
| Image | Should be specified |
| Frame | Should be specified |
| Space | Height must be specified |
| Row | 100% of column width |

### 8.4 Dimensions

| Property | Value |
|----------|-------|
| Width | Specified (mm or %); last column gets remainder |
| Height | Sum of children heights + spacing |

### 8.5 Child Positioning

**Horizontal:** All children centered within column width

**Vertical:** Children stack from top with spacing between

**Important:** In columns, stamps align by their total bounding box (including title), not by frame.

### 8.6 Example

```yaml
- column:
    width: 50%
    spacing: 5
    children:
      - stamp:
          title: "1c Green"
          width: 25
          height: 30
      - stamp:
          title: "2c Red"
          width: 25
          height: 30
```

---

## 9. Stamp

### 9.1 Description

The Stamp element is a composite drawable representing a philatelic stamp mount.

### 9.2 Structure

```
┌────────────────────────────────────┐
│              [Title]               │  ← Text above frame, centered
├────────────────────────────────────┤
│  ┌──────────────────────────────┐  │
│  │                              │  │
│  │        [Interior]            │  │  ← Image OR text lines
│  │                              │  │
│  └──────────────────────────────┘  │  ← Frame border
│  [Footer L] [Footer C] [Footer R]  │  ← Three footer positions
└────────────────────────────────────┘
```

### 9.3 Properties

| Property | Type | Description | Default |
|----------|------|-------------|---------|
| `style` | string | Reference to stamp style | - |
| `width` | number | Stamp interior width (mm) | Required |
| `height` | number | Stamp interior height (mm) | Required |
| `title` | string | Text above frame | - |
| `title-padding` | number | Space between title and frame (mm) | 0 |
| `image` | string | Interior image filename | - |
| `i1`, `i2`, `i3` | string | Interior text lines (max 3) | - |
| `f1`, `f2`, `f3` | string | Footer text (left, center, right) | - |
| `footer-padding` | number | Space between frame and footer (mm) | 0 |

**Notes:**
- `width` and `height` refer to the interior stamp area (not including frame thickness/padding)
- Interior contains either `image` OR text lines (`i1`-`i3`), not both

### 9.4 Size Calculation

```
Frame width  = stamp width  + (frame thickness × 2) + (frame padding × 2)
Frame height = stamp height + (frame thickness × 2) + (frame padding × 2)

Total width  = frame width
Total height = title height + title-padding + frame height + footer-padding + footer height
```

### 9.5 Title

| Property | Value |
|----------|-------|
| Position | Centered horizontally above frame |
| Width | Unlimited (user controls line breaks with `\n`) |
| Height | Calculated from text content |

### 9.6 Interior

**Image:** Scaled to fit stamp interior dimensions

**Text lines:** Up to 3 lines (`i1`, `i2`, `i3`), centered as a block

### 9.7 Footer

Three positions: `f1` (left), `f2` (center), `f3` (right)
- Aligned relative to frame width
- Unlimited width (user controls with `\n`)

### 9.8 Example

```yaml
- stamp:
    style: classic-stamp
    width: 25
    height: 30
    title: "King George VI\n1937 Coronation"
    title-padding: 2
    image: "canada-1c.png"
    footer-padding: 1
    f1: "Scott #123"
    f2: "1937"
    f3: "Michel #456"
```

---

## 10. Text

### 10.1 Description

The Text element renders a block of text with alignment and wrapping.

### 10.2 Properties

| Property | Type | Description | Default |
|----------|------|-------------|---------|
| `style` | string | Reference to text style | - |
| `content` | string | Text content | Required |
| `width` | number/string | Width in mm or % | Context-dependent |
| `align` | string | `left`, `right`, `center`, `justified` | TBD |
| `font-name` | string | Font family | TBD |
| `font-size` | number | Size in points | TBD |
| `font-style` | string | `regular`, `bold`, `italic`, `bold-italic` | `regular` |
| `color` | string | Text color | TBD |
| `bgcolor` | string | Background color | - |
| `x`, `y` | number | Absolute position (mm) | - |

### 10.3 Width Rules

| Context | Default Width |
|---------|---------------|
| Page | 100% of canvas |
| Column | 100% of column |
| Row | **Must be specified** |

### 10.4 Height

Text height is unlimited and grows based on content:

```
Height = (line height × lines) + (line spacing × (lines - 1))
```

### 10.5 Alignment and Wrapping

| Alignment | Behavior |
|-----------|----------|
| `left` | Left-aligned, wrap at word boundaries |
| `right` | Right-aligned, wrap at word boundaries |
| `center` | Centered, wrap at word boundaries |
| `justified` | Stretched to fill width; last line left-aligned |

Manual line breaks: `\n` forces a break in all modes.

### 10.6 Example

```yaml
- text:
    content: "Canada - Queen Elizabeth II Definitives"
    width: 90%
    align: center
    font-size: 14
    font-style: bold
```

---

## 11. Image

### 11.1 Description

The Image element renders an image from a file.

### 11.2 Properties

| Property | Type | Description | Default |
|----------|------|-------------|---------|
| `style` | string | Reference to image style | - |
| `src` | string | Image filename | Required |
| `width` | number/string | Width in mm or % | Native |
| `height` | number | Height in mm | Native |
| `scale-mode` | string | `fit`, `fill`, `stretch` | `fit` |
| `x`, `y` | number | Absolute position (mm) | - |

### 11.3 Size Calculation

| Condition | Behavior |
|-----------|----------|
| Neither specified | Rendered at native size |
| Only `width` | Scale both dimensions proportionally |
| Only `height` | Scale both dimensions proportionally |
| Both specified | Behavior depends on `scale-mode` |

### 11.4 Scale Mode

| Mode | Behavior |
|------|----------|
| `fit` | Scale to fit inside box, preserve aspect ratio (default) |
| `fill` | Scale to fill box, preserve aspect ratio, clipped to box |
| `stretch` | Stretch to exact dimensions (may distort) |

**Note:** `fill` mode uses clipping (not cropping) — image data is unchanged, rendering is bounded.

### 11.5 Missing Image

If the image file is not found, a placeholder is rendered:
- Light gray rectangle with border
- Two diagonal lines (corner to corner) in light gray

### 11.6 Positioning

- In Page/Column: Centered horizontally
- With `x`, `y`: Absolute position from page top-left

### 11.6 Example

```yaml
- image:
    src: "decorative.png"
    width: 50
    height: 30
    scale-mode: fit
```

---

## 12. Frame

### 12.1 Description

The Frame element renders a decorative border, typically used for visual grouping.

### 12.2 Properties

| Property | Type | Description | Default |
|----------|------|-------------|---------|
| `style` | string | Reference to frame style | - |
| `width` | number | Frame width (mm) | Required |
| `height` | number | Frame height (mm) | Required |
| `lines` | list | Line/gap pattern (mm) | `[]` (none) |
| `padding` | number | Interior padding (mm) | 0 |
| `color` | string | Stroke color | TBD |
| `x`, `y` | number | Absolute position (mm) | - |

### 12.3 Frame Lines Pattern

The `lines` property defines the frame structure as a list of widths in mm, alternating between line and gap, starting from the exterior toward the interior.

| Values | Result | Description |
|--------|--------|-------------|
| `[0.3]` | Single line | One 0.3mm line |
| `[0.3, 1, 0.3]` | Double line | 0.3mm line, 1mm gap, 0.3mm line |
| `[0.2, 0.5, 0.3, 0.5, 0.2]` | Triple line | Alternating line/gap/line/gap/line |

**Pattern:** line, gap, line, gap, ... (always starts and ends with line)

### 12.4 Example

```yaml
- frame:
    width: 100
    height: 50
    lines: [0.3, 1, 0.3]
    padding: 2
```

---

## 13. Space

### 13.1 Description

The Space element inserts empty space in the layout.

### 13.2 Properties

| Property | Type | Description | Default |
|----------|------|-------------|---------|
| `width` | number | Horizontal space in mm (for Row) | - |
| `height` | number | Vertical space in mm (for Page/Column) | - |

### 13.3 Behavior

| Parent | Required Property |
|--------|-------------------|
| Page, Column | `height` |
| Row | `width` |

### 13.4 Example

```yaml
# Vertical space
- space:
    height: 10

# Horizontal space (in row)
- row:
    children:
      - stamp: { width: 25, height: 30 }
      - space: { width: 15 }
      - stamp: { width: 25, height: 30 }
```

---

## 14. Style System

### 14.1 Units of Measurement

All dimensions use **millimeters (mm)**, except `font-size` which uses **points (pt)**.

**Margin/Padding notation:**

| Values | Meaning | Example |
|--------|---------|---------|
| 1 | All sides | `padding: 5` |
| 2 | Vertical, Horizontal | `padding: 5, 10` |
| 4 | Top, Right, Bottom, Left | `padding: 5, 10, 15, 20` |

### 14.2 Style Storage

Styles follow the HTML/CSS paradigm:

| Storage | Description |
|---------|-------------|
| External | `.style` files (recommended for reuse) |
| Embedded | Defined in `.album` file |
| Combined | Both; embedded processed after external |

### 14.3 Style Definition

| Attribute | Required | Description |
|-----------|----------|-------------|
| `name` | Yes | Unique identifier |
| `type` | Yes | `page`, `stamp`, `text`, `row`, `column`, `image`, `frame` |
| `default` | No | If `true`, used when no explicit style specified |
| *(other)* | No | Type-specific properties |

### 14.4 Font Attributes

| Attribute | Description | Values |
|-----------|-------------|--------|
| `font-name` | Font family | Name of loaded font |
| `font-size` | Size in points | Numeric |
| `font-style` | Variant | `regular`, `bold`, `italic`, `bold-italic` |

### 14.5 Color Attributes

**Formats:**

| Format | Example |
|--------|---------|
| Named | `color: DarkGreen` |
| RGB | `color: "72, 43, 145"` |

**Attributes:**

| Attribute | Description |
|-----------|-------------|
| `color` | Foreground/text color |
| `bgcolor` | Background color |

### 14.6 Style Resolution

Cascade order (highest to lowest priority):
1. **Inline** - Attributes on element
2. **Explicit style** - Referenced by `style` property
3. **Default style** - Style with `default: true` for element type
4. **Built-in defaults** - Hardcoded values

---

## 15. Configuration

### 15.1 Config File

**File:** `myalbum.config` (JSON, in application directory)

```json
{
  "folders": {
    "templates": "/MyAlbum/Templates",
    "output": "/MyAlbum/Output",
    "images": "/MyAlbum/Images",
    "fonts": "/MyAlbum/Fonts"
  }
}
```

### 15.2 CLI Interface

```
myalbum <input-file> [options]

Options:
  -o, --output <path>    Output file or folder
  -v, --verbose          Progress messages
  -q, --quiet            Errors only
  --version              Display version
  --help                 Display help
```

---

## 16. Technical Implementation

### 16.1 Technology Stack

| Component | Choice | Rationale |
|-----------|--------|-----------|
| Framework | .NET 10 | Latest LTS |
| PDF Library | PDFsharp 6.x | MIT license, coordinate-based API |
| Testing | xUnit + NSubstitute | Modern, parallel execution |
| Architecture | Layered | Parsing → Model → Layout → Render |

### 16.2 Cross-Platform Support

| Platform | Runtime ID |
|----------|------------|
| Windows | `win-x64` |
| Linux | `linux-x64` |
| macOS Intel | `osx-x64` |
| macOS ARM | `osx-arm64` |

**Distribution:** Self-contained single-file executable

### 16.3 Architecture Principles

1. **Simple & working first** - Core features before complexity
2. **Decoupled core** - Separate parsing, model, layout, rendering
3. **Testable** - Unit tests for calculations, integration tests for output
4. **Future-proof** - Design for GUI/web without over-engineering

### 16.4 Error Handling

**Strategy:** Fail fast - stop on first error with clear message

### 16.5 Supported Page Sizes

| Feature | Status |
|---------|--------|
| Letter, A4, Legal sizes | Supported |
| Custom dimensions | Future |
| Portrait/Landscape | Supported |
| Mixed orientation | Supported |
| Duplex/book layout | Supported |

### 16.6 Key Decisions

| Topic | Decision |
|-------|----------|
| Units | Millimeters (mm); font-size in points |
| Input format | Auto-detect XML/JSON/YAML |
| Style storage | External `.style` or embedded |
| Style resolution | Inline → Explicit → Default → Built-in |
| Image formats | JPEG, PNG, TIFF, WebP, GIF |
| Font handling | Embedded in PDF |
| Error handling | Fail fast |

---

## 17. To Be Defined

The following items require decisions before implementation:

### 17.1 Defaults

| Item | Context | Notes |
|------|---------|-------|
| Default page size | Page | `letter` or `a4`? |
| Default orientation | Page | `portrait` assumed |
| Default font-name | Text | System default or bundled font? |
| Default font-size | Text | Suggest 10pt or 12pt |
| Default text alignment | Text | `left` assumed |
| Default row spacing mode | Row | `fixed` with 0mm? |
| Default row vertical align | Row | `top` assumed |
| Default frame lines | Frame, Stamp | `[]` (none) |
| Default frame color | Frame, Stamp | Black assumed |

### 17.2 Clarifications Needed

| Item | Context | Issue |
|------|---------|-------|
| Inside text width | Stamp | Is `i1`-`i3` text width limited (90% of stamp) or unlimited? |
| Banner sizing | Page | How is banner sized? Image rules or custom? |
| Color support | Elements | Which elements support `color`/`bgcolor`? |
| Row/Column bgcolor | Row, Column | Should support `bgcolor`? (suggested: yes) |

### 17.3 Conventions

- Attribute naming uses **lowercase with dashes** (e.g., `font-size`, `title-padding`)
- Exception: `bgcolor` (not `bg-color`) for brevity

---

## Appendix A: Alignment Reference

### A.1 Alignment Guides

All elements use bounding box for alignment, except stamps in rows which align by frame.

| Guide | Position |
|-------|----------|
| `left` | x |
| `right` | x + width |
| `hcenter` | x + width/2 |
| `top` | y |
| `bottom` | y + height |
| `vcenter` | y + height/2 |

### A.2 Stamp Alignment in Rows

Stamps align by **frame**, not total bounding box:

| Guide | Position |
|-------|----------|
| `top` | frame.y |
| `bottom` | frame.y + frame.height |
| `vcenter` | frame.y + frame.height/2 |
