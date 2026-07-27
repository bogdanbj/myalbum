# MyAlbum v7.0 Implementation Plan

## Overview

A phased approach focusing on "simple & working first" with incremental feature additions.

---

## Phase 1: Foundation (MVP) - COMPLETED

**Goal**: Generate a single-page PDF with basic elements

### Milestone 1.1: Project Setup
- [x] Create .NET 10 console project
- [x] Add PdfSharpCore package
- [x] Create folder structure
- [x] Basic Program.cs with argument parsing

### Milestone 1.2: Core Model
- [x] BaseElement with position/size properties
- [x] Page element with dimensions
- [x] Simple Text element
- [x] Simple Row container

### Milestone 1.3: Basic Rendering
- [x] PdfRenderer with PdfSharpCore
- [x] Render a page with hardcoded content
- [x] Generate working PDF output

### Milestone 1.4: XML Parsing
- [x] IAlbumParser interface
- [x] XmlAlbumParser for basic structure
- [x] Parse page, row, text elements

**Deliverable**: CLI that converts simple XML to PDF with text rows

---

## Phase 2: Stamp Support - COMPLETED

**Goal**: Add stamp element with frame and text areas

### Milestone 2.1: Stamp Model
- [x] Stamp composite element
- [x] Frame with border styles (single, double)
- [x] Title, inside text (3 lines), footer (3 positions)

### Milestone 2.2: Frame Rendering
- [x] Traditional single-line border
- [x] Traditional double-line border
- [x] White Ace border style
- [x] Padding and margin support

### Milestone 2.3: Stamp Layout
- [x] Calculate stamp dimensions
- [x] Position internal components
- [x] Row layout with multiple stamps

### Milestone 2.4: XML Parsing
- [x] Parse stamp elements
- [x] Sample XML with stamps
- [x] End-to-end test

**Deliverable**: Generate album page with framed stamps

---

## Phase 3: Styling System - PENDING

**Goal**: Named styles with inheritance and overrides

### Milestone 3.1: Style Model
- [ ] StyleManager for style registry
- [ ] PageStyle, FrameStyle, TextStyle classes
- [ ] Default styles

### Milestone 3.2: Style Resolution
- [ ] Named style lookup
- [ ] Inline property overrides
- [ ] Style inheritance chain

### Milestone 3.3: Style Parsing
- [ ] Parse `<styles>` block in XML
- [ ] External .styles file support
- [ ] Style references in elements

**Deliverable**: Styles defined once, applied to multiple elements

---

## Phase 4: Images & Layout - PENDING

**Goal**: Image support and advanced layout

### Milestone 4.1: Image Support
- [ ] Image element for decorations
- [ ] Stamp image (inside frame)
- [ ] Image scaling and positioning

### Milestone 4.2: Column Layout
- [ ] Column container element
- [ ] Nested row/column layouts
- [ ] Flexible spacing modes

### Milestone 4.3: Spacer & Separator
- [ ] Spacer element (empty space)
- [ ] Separator element (horizontal/vertical rules)

**Deliverable**: Complex page layouts with images

---

## Phase 5: Multi-Format & Polish - PENDING

**Goal**: JSON support, page features, polish

### Milestone 5.1: JSON Parser
- [ ] JsonAlbumParser implementation
- [ ] ParserFactory for format selection
- [ ] Equivalent feature parity with XML

### Milestone 5.2: Page Features
- [ ] Multiple pages
- [ ] Page selection (render specific pages)
- [ ] Mixed orientation support
- [ ] Duplex/book layout markers

### Milestone 5.3: Brand Styles
- [ ] White Ace frame style
- [ ] Additional border patterns
- [ ] Page header/banner support

**Deliverable**: Full-featured CLI application

---

## Phase 6: Future Enhancements - FUTURE

**Goal**: Advanced features for future releases

- [ ] YAML parser
- [ ] HTML output renderer
- [ ] Custom fonts
- [ ] Catalog number formatting
- [ ] Template library
- [ ] GUI application
- [ ] Web interface

---

## Progress Summary

| Phase | Status | Milestones |
|-------|--------|------------|
| Phase 1 | COMPLETED | 4/4 |
| Phase 2 | COMPLETED | 4/4 |
| Phase 3 | PENDING | 0/3 |
| Phase 4 | PENDING | 0/3 |
| Phase 5 | PENDING | 0/3 |
| Phase 6 | FUTURE | 0/7 |

---

## Testing

### How to Test Current Implementation

```powershell
cd C:\My\Git\myalbum\v7.0

# Run with sample XML
dotnet run --project src\MyAlbum\MyAlbum.csproj -- samples\test.xml

# Run stamps sample
dotnet run --project src\MyAlbum\MyAlbum.csproj -- samples\stamps.xml

# Run built-in test mode
dotnet run --project src\MyAlbum\MyAlbum.csproj -- --test

# View help
dotnet run --project src\MyAlbum\MyAlbum.csproj -- --help
```

### Sample Files

| File | Description |
|------|-------------|
| `samples/test.xml` | Basic text and rows |
| `samples/stamps.xml` | Stamps with frames and footers |
