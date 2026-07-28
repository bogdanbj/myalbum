# MyAlbum v7.0 - Requirements Specification

## Table of Contents

1. [Overview](#1-overview)
2. [Input](#2-input)
3. [Output](#3-output)
4. [Units of Measurement](#4-units-of-measurement)
5. [Layout Elements](#5-layout-elements)
6. [Page Structure](#6-page-structure)
7. [Stamp Structure](#7-stamp-structure)
8. [Style System](#8-style-system)
9. [Page Layout Features](#9-page-layout-features)
10. [Configuration](#10-configuration)
11. [Technical Stack](#11-technical-stack)
12. [Cross-Platform Support](#12-cross-platform-support)
13. [Architecture Principles](#13-architecture-principles)
14. [Error Handling](#14-error-handling)
15. [Decisions Summary](#15-decisions-summary)
16. [Alignment Guides](#16-alignment-guides)
17. [Row Positioning](#17-row-positioning)
18. [Column Positioning](#18-column-positioning)
19. [Text Width Rules](#19-text-width-rules)
20. [To Be Defined](#20-to-be-defined)

---

## 1. Overview

**MyAlbum v7.0** is a stamp album PDF generator for philatelists. Users provide album definition files (`.album`) describing content and styling, and the application generates professional PDF album pages.

| Aspect | Description |
|--------|-------------|
| **Target audience** | Personal use; future freeware distribution |
| **Application type** | CLI (Phase 1); GUI/web planned (Phase 2) |
| **Core design** | Logic decoupled from interface |

---

## 2. Input

### 2.1 Album Files

| Property | Value |
|----------|-------|
| **Extension** | `.album` |
| **Default folder** | `/MyAlbum/Templates` (configurable) |

**Format auto-detection** (based on content, not extension):

| Format | Detection |
|--------|-----------|
| XML | Starts with `<?xml` or `<` |
| JSON | Starts with `{` or `[` |
| YAML | Starts with `---` or key-value pattern |

### 2.2 Album File Structure

```yaml
album:
  title: "Canada - Queen Elizabeth II Definitives"
  author: "Collector Name"
  subject: "Stamp collection album"

styleFile: "classic.style"    # Optional: external style file

styles:                       # Optional: embedded styles
  - name: my_stamp
    type: stamp
    default: true
    frame: double
    width: 25
    height: 30

pages:
  - number: 1
    style: my_page
    children:
      - stamp:
          title: "1c Green"
      - stamp:
          style: large_stamp
          title: "5c Blue"
```

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

**Text Font Attributes:**

| Attribute | Description | Values |
|-----------|-------------|--------|
| `font-name` | Font family name | Name of loaded font |
| `font-size` | Size in points | Numeric value |
| `font-style` | Style variant | `regular`, `bold`, `italic`, `bold-italic` |

### 2.5 Color Specification

Colors are specified by name or RGB values:

| Format | Example | Description |
|--------|---------|-------------|
| Named | `color="DarkGreen"` | PDFsharp named color |
| RGB | `color="72, 43, 145"` | Red, Green, Blue (0-255 each) |

**Color Attributes:**

| Attribute | Description |
|-----------|-------------|
| `color` | Foreground/text color |
| `bgcolor` | Background color (exception to naming convention) |

---

## 3. Output

### 3.1 PDF Generation

| Property | Value |
|----------|-------|
| **Default folder** | `/MyAlbum/Output` (configurable) |
| **File naming** | Same as input with `.pdf` extension |
| **Conflict handling** | Append `_1`, `_2`, etc. |

### 3.2 Summary Page

First page of PDF (not for printing):
- Input file name
- MyAlbum version (e.g., v7.0)
- Total page count

### 3.3 PDF Metadata

Populated from album file: `title`, `author`, `subject`

---

## 4. Units of Measurement

All dimensions use **millimeters (mm)** only. Inches and points are not supported.

### 4.1 Margin and Padding Notation

Values can be expressed as tuples of 1, 2, or 4 values:

| Values | Meaning | Example |
|--------|---------|---------|
| 1 | All sides equal | `padding: 5` → all sides = 5 |
| 2 | Vertical, Horizontal | `padding: 5, 10` → top/bottom=5, left/right=10 |
| 4 | Top, Right, Bottom, Left | `padding: 5, 10, 15, 20` → clockwise from top |

---

## 5. Layout Elements

### 5.1 Container Elements

| Element | Description |
|---------|-------------|
| **Page** | Single album page |
| **Row** | Horizontal arrangement of children |
| **Column** | Vertical arrangement of children |

### 5.2 Content Elements

| Element | Description |
|---------|-------------|
| **Stamp** | Composite element (see [Section 7](#7-stamp-structure)) |
| **Text** | Free-form text block |
| **Image** | Decorative/informational image |
| **Frame** | Border element |
| **Space** | Empty space control |

#### Space Element

Inserts empty space in the layout. Orientation is determined by parent container:

| Parent | Attribute | Description |
|--------|-----------|-------------|
| Page, Column | `height` | Vertical space (mm) |
| Row | `width` | Horizontal space (mm) |

---

## 6. Page Structure

### 6.1 Page Properties

| Property | Description | Default |
|----------|-------------|---------|
| `number` | Page number (required) | - |
| `title` | Metadata only, not rendered | - |
| `style` | Reference to page style | - |
| `padding` | Interior spacing | 0 |
| `space` | Space between rows (row-based) | - |
| `vspace` | Space between columns (column-based) | - |
| `children` | Row, Column, or content elements | - |

**Note**: Pages have no margin (exterior spacing).

### 6.2 Page Style Elements

Defined in page style, applied to all pages using that style:

| Element | Description |
|---------|-------------|
| `banner` | Image at top of page |
| `border` | Frame around content area |
| `background` | Background image (absolute positioned, rendered first) |
| `header` | Text at top of content area |
| `footer` | Text at bottom of content area |

**Background Image**:
- Rendered before any other elements (lowest z-order)
- Absolute positioned image (uses `x`, `y`, `width`, `height` attributes)
- Page child elements are rendered on top of the background
- Standard image sizing rules apply (see [Image Element](#image-element))

**Orientation behavior**: Style elements are NOT affected by page orientation - they remain on the same physical edge regardless of portrait/landscape.

### 6.3 Page Canvas Calculation

The content area is calculated by reducing page dimensions in order:

| Step | Element | Reduction |
|------|---------|-----------|
| 1 | Page padding | All sides |
| 2 | Banner | Height (from top) |
| 3 | Border | Thickness + padding |
| 4 | Header | Height (from top) |
| 5 | Footer | Height (from bottom) |
| 6 | **Content area** | Remaining space |

**Banner**:
- Position: Top of page (portrait reference)
- Coordinates: `x=0, y=0` relative to top-left corner (after padding)
- If banner has `margin != 0`, its position and size are adjusted

**Border**:
- Has `thickness` and `padding` only (no margin)

### 6.4 Page Layout Modes

Layout mode is determined by the **first child element**:

| First Child | Layout Mode |
|-------------|-------------|
| Column | Column-based |
| Any other | Row-based |

#### Column-Based Layout

| Property | Description |
|----------|-------------|
| Column height | Sum of children heights + vspace between children |
| Column width | Specified (mm or %), last column = remainder |
| Column alignment | Always aligned at **top** |
| `vspace` | Space between columns |

Percentage widths calculated from: `canvas width − total vspace`

```yaml
pages:
  - number: 1
    vspace: 5
    children:
      - column:
          width: 40%
          children: [...]
      - column:
          width: 50
          children: [...]
      - column:           # width = remainder
          children: [...]
```

#### Row-Based Layout

| Property | Description |
|----------|-------------|
| Row width | 100% of canvas (default) |
| Row height | Calculated from children (see [Section 17](#17-row-positioning)) |
| Row alignment | Rows stack from top |
| `space` | Space between rows |

```yaml
pages:
  - number: 1
    space: 5
    children:
      - row:
          padding: 2
          children: [...]
      - row:
          children: [...]
```

### 6.5 Content Elements on Page

#### Text Element

| Condition | Behavior |
|-----------|----------|
| Default | Width = 100% of canvas |
| Custom `width` | Overrides default |
| `x`, `y` declared | Absolute position (page top-left) |

#### Image Element

**Size Calculation:**

| Condition | Behavior |
|-----------|----------|
| Neither `width` nor `height` specified | Rendered at original (native) size |
| Only `width` specified | Scale factor = width / native width; apply to both dimensions |
| Only `height` specified | Scale factor = height / native height; apply to both dimensions |
| Both `width` and `height` specified | Behavior depends on `scale-mode` attribute (see below) |

**`scale-mode` Attribute** (when both `width` and `height` are specified):

| Value | Behavior |
|-------|----------|
| `fit` | Scale to fit inside the box, preserving aspect ratio (default) |
| `fill` | Scale to fill the box, preserving aspect ratio (may crop) |
| `stretch` | Stretch to exact dimensions (may distort aspect ratio) |

**Positioning:**

| Condition | Behavior |
|-----------|----------|
| `width` is percent | Percentage of canvas width |
| `width` is absolute | Centered horizontally |
| `x`, `y` declared | Absolute position (page top-left) |

#### Stamp Element

- Always centered horizontally on canvas

#### Absolute Positioning

Elements with `x`, `y` coordinates are **absolutely positioned**:
- Position relative to original page top-left corner
- **Do not affect canvas** - taken out of flow
- **Do not affect following elements** - subsequent elements ignore them

---

## 7. Stamp Structure

```
┌────────────────────────────────────┐
│              [Title]               │  ← Text above frame, centered
├────────────────────────────────────┤
│  ┌──────────────────────────────┐  │
│  │                              │  │
│  │        [Stamp Image]         │  │  ← Interior: Image OR Text
│  │            -OR-              │  │
│  │        [Inside Text]         │  │
│  │                              │  │
│  └──────────────────────────────┘  │  ← Frame border
│  [Footer L] [Footer C] [Footer R]  │  ← Footer: 3 positions
└────────────────────────────────────┘
```

### 7.1 Size Calculation

**Stamp size** (`width`, `height`) is required for every stamp, in mm.

**Frame dimensions**:
```
Frame width  = stamp width  + (thickness × 2) + (padding × 2)
Frame height = stamp height + (thickness × 2) + (padding × 2)
```

**Total stamp dimensions**:
```
Total width  = frame width
Total height = title height (incl. padding) + frame height + footer height (incl. padding)
```

### 7.2 Title

| Property | Description |
|----------|-------------|
| Position | Above frame, centered horizontally |
| Height | Calculated from text + font size |
| Width | Not limited (user controls with `\n`) |
| Padding | Optional `titlePadding` |
| Spacing | No space between title and frame (other than padding) |

```yaml
- stamp:
    title: "King George VI\n1937 Coronation"
    titlePadding: 2
    width: 25
    height: 30
```

### 7.3 Interior

Contains either **image** OR **text lines** (mutually exclusive).

#### Inside Text

| Property | Description |
|----------|-------------|
| Lines | Up to 3: `i1`, `i2`, `i3` (all optional) |
| Width | Limited to 90% of stamp width |
| Wrapping | Follows text wrapping logic (TBD) |
| Alignment | Centered horizontally and vertically |
| Multiple lines | Treated as block, centered vertically |

```yaml
- stamp:
    width: 25
    height: 30
    i1: "1c Green"
    i2: "Perf 12"
    i3: "Mint NH"
```

#### Inside Image

```yaml
- stamp:
    width: 25
    height: 30
    image: "canada-1c-green.png"
```

### 7.4 Footer

| Property | Description |
|----------|-------------|
| Positions | `f1` (left), `f2` (center), `f3` (right) |
| Alignment | Relative to frame width |
| Padding | Optional `footerPadding` |
| Spacing | No space between footer and frame (other than padding) |
| Height | Calculated from text + font size |
| Width | Not limited (user controls with `\n`) |

### 7.5 Complete Stamp Example

```yaml
- stamp:
    style: classic_stamp       # Optional
    width: 25                  # Required (mm)
    height: 30                 # Required (mm)
    
    # Title
    title: "King George VI\n1937 Coronation"
    titlePadding: 2
    
    # Interior (image OR text)
    image: "canada-1c.png"
    # i1: "1c Green"
    # i2: "Perf 12"
    # i3: "Mint NH"
    
    # Footer
    footerPadding: 1
    f1: "Scott #123"
    f2: "1937"
    f3: "Michel #456"
```

---

## 8. Style System

### 8.1 Style Storage

Styles follow the **HTML/CSS paradigm** - content and presentation can be separated:

| Storage | Description |
|---------|-------------|
| External | `.style` files (recommended for reuse) |
| Embedded | Defined directly in `.album` file |
| Combined | Both; embedded processed after external |

**External style file**:
```yaml
# classic.style
styles:
  - name: classic_stamp
    type: stamp
    default: true
    frame: single
    padding: 2

  - name: standard_page
    type: page
    default: true
    size: Letter
```

**Album referencing external styles**:
```yaml
album:
  title: "Canada Definitives"

styleFile: "classic.style"

pages:
  - number: 1
    children:
      - stamp:
          title: "1c Green"
```

### 8.2 Style File Format

| Property | Value |
|----------|-------|
| Extension | `.style` |
| Format | Auto-detected (XML/JSON/YAML) |
| Location | Same folder as album, or `/MyAlbum/Templates` |

### 8.3 Style Attributes

| Attribute | Required | Description |
|-----------|----------|-------------|
| `name` | Yes | Unique identifier |
| `type` | Yes | `page`, `stamp`, `text`, `row`, `column`, `image`, `frame` |
| `default` | No | If `true`, used when no explicit style |
| *(other)* | No | Type-specific properties |

### 8.4 Style Resolution

Cascade order:

1. **Explicit style** - Referenced by name (`style: large_stamp`)
2. **Default style** - Style with `default: true` for element type
3. **Built-in defaults** - Hardcoded sensible values

**Rules**:
- One default per element type; if multiple, **last wins**
- Missing attributes fall through to next level

### 8.5 Frame/Border Styles

| Style | Description |
|-------|-------------|
| `single` | Single line border |
| `double` | Double line border (classic) |
| `white-ace` | White Ace album style |
| `custom` | User-defined parameters |

**Frame properties**: `thickness` and `padding` only (no margin)

---

## 9. Page Layout Features

| Feature | Status |
|---------|--------|
| Letter, A4, Legal sizes | Supported |
| Custom dimensions | Future |
| Portrait/Landscape | Supported |
| Mixed orientation | Supported |
| Duplex/book layout | Supported |

---

## 10. Configuration

### 10.1 Config File

**File**: `myalbum.config` (JSON, in application directory)

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

### 10.2 CLI Interface

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

## 11. Technical Stack

| Component | Choice | Rationale |
|-----------|--------|-----------|
| Framework | .NET 10 | Latest LTS, modern C# |
| PDF Library | PDFsharp 6.x | MIT license, coordinate-based API |
| Testing | xUnit + NSubstitute | Modern, parallel execution |
| Architecture | Layered | Parsing → Model → Layout → Render |

---

## 12. Cross-Platform Support

| Property | Value |
|----------|-------|
| **Platforms** | Windows, Linux, macOS (Intel + ARM) |
| **Distribution** | Self-contained single-file executable |

| Platform | Runtime ID |
|----------|------------|
| Windows | `win-x64` |
| Linux | `linux-x64` |
| macOS Intel | `osx-x64` |
| macOS ARM | `osx-arm64` |

---

## 13. Architecture Principles

1. **Simple & working first** - Core features before complexity
2. **Decoupled core** - Separate parsing, model, layout, rendering
3. **Testable** - Unit tests for calculations, integration tests for output
4. **Future-proof** - Design for GUI/web without over-engineering

---

## 14. Error Handling

**Strategy**: Fail fast - stop on first error with clear message

---

## 15. Decisions Summary

| Topic | Decision |
|-------|----------|
| Units | Millimeters (mm) only |
| Input format | Auto-detect XML/JSON/YAML |
| Style storage | External `.style` or embedded |
| Style resolution | Explicit → Default → Built-in |
| Image formats | JPEG, PNG, TIFF, WebP, GIF |
| Font handling | Embedded in PDF |
| Catalog numbers | Free-form text |
| Page numbering | Explicit only |
| Batch processing | Single file per invocation |
| Error handling | Fail fast |

---

## 16. Alignment Guides

Elements have alignment guides for positioning within containers (rows, columns).

### 16.1 Text and Image Elements

Guides relative to **bounding box**:

| Guide | Position | Description |
|-------|----------|-------------|
| `left` | x | Left edge |
| `right` | x + width | Right edge |
| `hcenter` | x + width/2 | Horizontal center |
| `top` | y | Top edge |
| `bottom` | y + height | Bottom edge |
| `vcenter` | y + height/2 | Vertical center |

### 16.2 Stamp Elements

Stamps align relative to **frame** (not total bounding box):

| Guide | Position | Description |
|-------|----------|-------------|
| `left` | frame x | Frame left edge |
| `right` | frame x + frame width | Frame right edge |
| `hcenter` | frame x + frame width/2 | Frame horizontal center |
| `top` | frame y | Frame top edge |
| `bottom` | frame y + frame height | Frame bottom edge |
| `vcenter` | frame y + frame height/2 | Frame vertical center |

### 16.3 Alignment Examples

**Example 1 - Top alignment**:

Frames align at top; titles sit directly on frames (no space):

```
Stamp A (1-line title)      Stamp B (2-line title)

                                  [Title Line 1]
         [Title]                  [Title Line 2]
    ┌─────────────┐          ┌─────────────┐      ← Frames aligned at top
    │             │          │             │
    │   Frame     │          │   Frame     │
    │             │          │             │
    └─────────────┘          └─────────────┘
```

**Example 2 - Vertical center alignment**:

Frame centers align; different heights centered:

```
Stamp A (short frame)       Stamp B (tall frame)

                                  [Title]
                            ┌─────────────┐
                            │             │
         [Title]            │             │
    ┌─────────────┐         │             │
    │             │  ←───→  │   Frame     │   ← Frame centers aligned
    │   Frame     │         │             │
    │             │         │             │
    └─────────────┘         │             │
                            │             │
                            └─────────────┘
```

---

## 17. Row Positioning

### 17.1 Row Children

Permitted child elements:

| Element | Notes |
|---------|-------|
| **Stamp** | Fixed width (frame width) |
| **Text** | Width must be specified |
| **Image** | Width must be specified |
| **Column** | If present, row should contain only columns |

**Column-based row**: Same rules as page column-based layout:
- Column height = sum of children heights + vspace
- Columns aligned at **top**
- Width rules same as page (see [Section 6.4](#64-page-layout-modes))

### 17.2 Row Dimensions

| Property | Value |
|----------|-------|
| Width | 100% of parent canvas (unless overridden) |
| Height | Calculated from children + alignment |

### 17.3 Row Height Calculation

Based on vertical alignment of children:

1. Determine alignment: `top`, `vcenter`, or `bottom`
2. For each child, calculate height **above** and **below** alignment line
3. Find **max above** and **max below** across all children
4. **Row height** = max above + max below

**Top alignment**:
```
Alignment line ───── ┌───────┐  ┌───────────┐
                     │ Child │  │           │
                     │   A   │  │  Child B  │
                     └───────┘  │           │
                                └───────────┘

Height above = 0
Height below = max(child heights)
Row height   = tallest child
```

**Center alignment**:
```
                     ┌───────┐
                     │ Child │  ┌─────────────┐
Alignment line ──────│── A ──│──│── Child B ──│───
                     │       │  └─────────────┘
                     └───────┘  

Height above = max(top halves)
Height below = max(bottom halves)
Row height   = max above + max below
```

**Bottom alignment**:
```
                                ┌───────────┐
                     ┌───────┐  |   Child   |
                     │ Child │  │     B     │
                     │   A   │  │           │
Alignment line ───── └───────┘  └───────────┘

Height above = max(child heights)
Height below = 0
Row height   = tallest child
```

### 17.4 Row Horizontal Spacing

**Child widths**:
- **Stamp**: Fixed (frame width)
- **Text/Image**: Must be specified

**Spacing modes**:

| Mode | Attribute | Formula |
|------|-----------|---------|
| **Fixed (FS)** | `spacing: 5` | Fixed mm between children |
| **Equal (ES)** | `spacing: equal` | `(row - children) / (n - 1)` |
| **Justified (JS)** | `spacing: justified` | `(row - children) / (n + 1)` |

**Fixed Spacing (FS)** - Children centered:
```
├─────[Child 1]─(5mm)─[Child 2]─(5mm)─[Child 3]─────┤
    ↑                                           ↑
    └──── remaining / 2 ────────────────────────┘
```

**Equal Spacing (ES)** - Edge to edge:
```
├[Child 1]────────[Child 2]────────[Child 3]┤
```

**Justified (JS)** - Equal gaps everywhere:
```
├────[Child 1]────[Child 2]────[Child 3]────┤
```

---

## 18. Column Positioning

### 18.1 Column Children

Permitted child elements:

| Element | Notes |
|---------|-------|
| **Stamp** | Fixed width (frame width) |
| **Text** | Width should be specified or defaults to column width |
| **Image** | Width should be specified |
| **Row** | Nested rows |

### 18.2 Column Dimensions

| Property | Value |
|----------|-------|
| Width | Specified (mm or %), last column = remainder |
| Height | Sum of children heights + vspace between |

### 18.3 Child Positioning

**Horizontal**: All children are **centered** relative to the column width.

**Vertical**: Children stack from top:
1. First element starts at top of column
2. Followed by `vspace`
3. Followed by next element
4. Repeat until last element

**Important distinction from rows**:
- In a **row**: Stamps align vertically based on their **frame** (top, center, bottom)
- In a **column**: The **top of the stamp** (including title) aligns with the column top

If a stamp is the first element in a column, the top of the stamp title aligns with the top of the column.

```
┌─────────────────┐
│    [Title]      │  ← Title top aligns with column top
│  ┌───────────┐  │
│  │   Frame   │  │
│  └───────────┘  │
│                 │
│    (vspace)     │
│                 │
│    [Child 2]    │
└─────────────────┘

Column height = Stamp total height + vspace + Child2 height + ...
```

---

## 19. Text Width Rules

Text width varies based on the container:

| Container | Default Width | Override |
|-----------|---------------|----------|
| **Page** | 100% of canvas | Percent (e.g., `width: 90%`) or fixed (mm) |
| **Column** | 100% of column width | Percent or fixed (mm) |
| **Row** | **Must be specified** | Fixed (mm) required |
| **Stamp** (title, i1, i2, i3, footer) | **Unlimited** | User controls with `\n` |

### 19.1 Page and Column Text

Text inherits canvas/column width by default, but can be overridden:

```yaml
# Default - full width
- text:
    content: "This spans full width"

# Percent width
- text:
    width: 90%
    content: "This spans 90% of canvas"

# Fixed width
- text:
    width: 150
    content: "This is exactly 150mm wide"
```

### 19.2 Row Text

Text in a row **must** have a fixed width specified:

```yaml
- row:
    children:
      - text:
          width: 50           # Required
          content: "Label"
      - stamp:
          width: 25
          height: 30
```

### 19.3 Stamp Text

All text within a stamp (title, i1, i2, i3, f1, f2, f3) has **unlimited width**. The user is responsible for controlling line breaks with `\n`:

```yaml
- stamp:
    title: "Long Title That User\nMust Break Manually"
    i1: "First line\nSecond line"
    f1: "Scott #123"
```

### 19.4 Text Alignment and Wrapping

Text alignment options:

| Alignment | Description |
|-----------|-------------|
| `left` | Text aligned to left edge |
| `right` | Text aligned to right edge |
| `center` | Text centered within width |
| `justified` | Text stretched to fill width |

**Word wrapping** (for left, right, center):
- Text wraps at word boundaries
- If next word does not fit the line width, it moves to next line
- `\n` forces a line break

**Justified alignment**:
- Text wraps at word boundaries (same as above)
- Word spacing is adjusted equally so line width matches text width
- **Last line** is treated as left-aligned (not stretched)
- `\n` forces a line break

**Manual line breaks**: In all alignment modes, `\n` forces a line break.

### 19.5 Text Height

Text height is **never limited** - it grows based on content.

**Height calculation** (in order):
1. **Font size** → determines base character height
2. **Line height** → height of each line (typically font size × multiplier)
3. **Line spacing** → additional space between lines

```
Total height = (line height × number of lines) + (line spacing × (number of lines - 1))
```

```yaml
- text:
    width: 100
    align: justified
    content: "This text will be stretched to fill the full 100mm width, with spacing adjusted between words. The last line remains left-aligned."
```

---

## 20. To Be Defined

| Item | Section | Notes |
|------|---------|-------|
| Default font | 2.4 | Default `font-name` when not specified |
| Default font-size | 2.4 | Default `font-size` value |
| Default font-style | 2.4 | Clarify `regular` is default |
| Inside text width | 7.3 vs 19.3 | Contradiction: 90% of stamp width vs unlimited |
| Color/bgcolor usage | 2.5 | Which elements support color attributes |
| Frame color | 8.5 | Frame stroke color attribute |
| Default page size | 9 | Letter, A4, or other default |
| Default orientation | 9 | Portrait or landscape default |
| Space element style | 8.3 | Add `space` to style types list |
| Banner sizing | 6.2 | Banner image sizing rules |
