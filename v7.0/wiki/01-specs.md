# MyAlbum v7.0 Requirements & Decisions

## Project Vision

**MyAlbum v7.0** is a stamp album PDF generator for philatelists. Users provide metadata files describing album content (e.g., stamps, text, images, etc.) and styling, and the application generates professional PDF album pages.

---

## Target Audience

- **Primary**: Personal use for building stamp collection albums
- **Future**: Freeware distribution to small community of fellow collectors
- **Design implication**: Focus on usability and correctness over enterprise features

---

## Application Type

- **Phase 1**: CLI application (command-line tool)
- **Phase 2**: GUI or web interface (future)
- **Design implication**: Core logic must be decoupled from interface for future GUI/web

---

## Input Format

**Multiple formats supported**:
- XML (legacy compatibility with v3.0-v6.1)
- JSON (structured, widely supported)
- YAML (human-friendly editing)

**Default input folder**: `/myalbum/Templates`

**Design implication**: Need format-agnostic internal model with pluggable parsers

---

## Output Format

- **PDF** (required, primary output)
- Future consideration: HTML, images, print-ready PDF

**Default output folder**: `/myalbum/Output`

**Output file naming**:
- Same name as input file with `.pdf` extension
- If file already exists, append `_1`, `_2`, etc. before the extension
- Example: `Canada.xml` → `Canada.pdf` → `Canada_1.pdf` → `Canada_2.pdf`

**Design implication**: Use abstracted rendering layer for future format support

---

## Page Layout Capabilities

| Feature | Status | Notes |
|---------|--------|-------|
| Standard paper sizes | Required | Letter, A4, Legal |
| Custom page sizes | Future | User-defined dimensions |
| Mixed orientation | Required | Rotated content on portrait pages |
| Duplex/book layout | Required | Two-sided printing support |

---

## Layout Elements

### Container Elements
- **Container**: Abstract base for elements that hold children
- **Page** : Container - Represents a single album page (composite element)
  - **Border**: Frame element (optional) - defined in page style, inherited by all pages using that style
  - **Banner**: Image element (optional) - defined in page style, inherited by all pages using that style
  - **Title**: string (not displayed, metadata only)
  - **Number**: int (not displayed, metadata only)
  - **Children**: Can contain Row, Column containers or absolute-positioned content elements (Text, Image). Children are positioned inside the border, relative to the page border. (More details TBD)
  
  **Orientation behavior**:
  - Page border and banner are NOT affected by page orientation
  - Position remains consistent regardless of page orientation (e.g., banner on top stays on the short side for both portrait and landscape)
  
- **Row** : Container - Horizontal arrangement of child elements
- **Column** : Container - Vertical arrangement of child elements

### Content Elements
- **Stamp**: Composite element (see Stamp Structure below)
- **Text**: Free-form text blocks
- **Image**: Decorative or informational images (non-stamp)
- **Frame**: Border element (used as page border or stamp frame)
- **Separator/Rule**: Horizontal or vertical dividers
- **Spacer**: Empty space control

### Stamp Structure (Composite Element)
```
┌─────────────────────────────┐
│         [Title]             │
├─────────────────────────────┤
│  ┌─────────────────────┐    │
│  │                     │    │
│  │  [Stamp Image]      │    │
│  │       -OR-          │    │
│  │  [Inside Line 1]    │    │
│  │  [Inside Line 2]    │    │
│  │  [Inside Line 3]    │    │
│  │                     │    │
│  └─────────────────────┘    │
│ [Footer L] [Footer C] [Footer R] │
└─────────────────────────────┘
```

Components:
- **Title**: Text above the frame
- **Frame**: Border around stamp (traditional, brand-style)
- **Interior** (mutually exclusive):
  - **Inside Image**: Scanned stamp image, OR
  - **Inside Text**: 1-3 lines of text (denomination, description)
- **Footer Text**: 3 positions - left, center, right (catalog numbers, dates) - aligned relative to stamp frame

---

## Frame/Border Styles

| Style | Description |
|-------|-------------|
| Traditional single | Single line border |
| Traditional double | Double line border (classic look) |
| White Ace | Mimics White Ace album style |
| Custom | User-defined border parameters |

---

## Style System

**Named styles + inline overrides** (CSS-like approach)

```yaml
styles:
  stamp_default:
    frame: double
    padding: 3mm
    
pages:
  - stamps:
    - style: stamp_default      # Use named style
      title: "1c Green"
      padding: 5mm              # Override specific property
```

Style categories:
- Page styles (size, orientation, margins)
- Frame/border styles (line widths, colors, padding)
- Text styles (font, size, alignment)
- Row/Column styles (spacing, alignment)

---

## Architecture Principles

1. **Simple & working first** - Get core features working before adding complexity
2. **Decoupled core** - Separate parsing, model, layout, rendering
3. **Testable** - Unit tests for calculations, integration tests for output
4. **Future-proof** - Design for GUI/web without over-engineering now

---

## Technical Decisions

| Decision | Choice | Rationale |
|----------|--------|-----------|
| Framework | .NET 10 | Latest LTS, modern C# features |
| PDF Library | PdfSharpCore | Proven in v4-v6, cross-platform |
| Project Type | Console App | CLI-first, extract library later |
| Architecture | Layered | Parsing → Model → Layout → Render |

---

## Open Questions

- [ ] Should styles be in separate files or embedded in album file?
- [ ] What catalog number formats to support (Scott, Michel, SG)?
- [ ] Image format support (JPEG, PNG, TIFF, WebP)?
- [ ] Font handling (embedded, system, custom)?
