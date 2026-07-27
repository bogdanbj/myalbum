# MyAlbum v7.0 Architecture Design

## Overview

A layered architecture that separates concerns and enables future GUI/web interfaces.

---

## Components

### Layer 1: Input/Parsing
- **IAlbumParser** - Interface for format-specific parsers
- **XmlAlbumParser** - Parse XML album files
- **JsonAlbumParser** - Parse JSON album files (future)
- **YamlAlbumParser** - Parse YAML album files (future)
- **ParserFactory** - Select parser based on file extension

### Layer 2: Domain Model
- **Album** - Root container with metadata and pages
- **Page** - Single page with layout elements
- **Stamp** - Composite stamp element
- **Row/Column** - Layout containers
- **Text/Image/Spacer/Separator** - Leaf elements
- **Styles** - Named style definitions

### Layer 3: Layout Engine
- **LayoutCalculator** - Compute positions and sizes
- **Canvas** - Available drawing area tracker
- **Measurement** - Unit conversions (mm, pt, in)

### Layer 4: Rendering
- **IRenderer** - Interface for output format renderers
- **PdfRenderer** - Generate PDF using PdfSharpCore
- **HtmlRenderer** - Generate HTML (future)

### Layer 5: CLI
- **Program** - Entry point and argument parsing
- **CommandHandler** - Execute user commands

---

## Key Flows

### 1. Album Generation Flow
```
CLI Args → Parser Selection → Parse Input File
    → Build Domain Model → Apply Styles
    → Calculate Layout → Render to PDF
    → Save Output File
```

### 2. Style Resolution Flow
```
Element requests style → Check inline properties
    → Fall back to named style → Fall back to defaults
```

### 3. Layout Calculation Flow
```
Page.Calculate() → Set page dimensions
    → Calculate border/margins → Get content area
    → For each child element:
        → Calculate element size
        → Position within available space
        → Reduce available space for next element
```

---

## Interfaces

### IAlbumParser
```csharp
public interface IAlbumParser
{
    Album Parse(string filePath);
    Album Parse(Stream stream);
    bool CanParse(string filePath);
}
```

### IRenderer
```csharp
public interface IRenderer
{
    void Render(Album album, string outputPath);
    void Render(Album album, Stream outputStream);
}
```

---

## Folder Structure

```
v7.0/
├── MyAlbum.slnx
├── wiki/                       # Documentation
├── samples/                    # Sample XML files
└── src/
    └── MyAlbum/
        ├── Program.cs
        ├── MyAlbum.csproj
        ├── Models/
        │   ├── Album.cs
        │   └── Elements/
        │       ├── BaseElement.cs
        │       ├── ContainerElement.cs
        │       ├── Page.cs
        │       ├── Row.cs
        │       ├── Text.cs
        │       ├── Frame.cs
        │       └── Stamp.cs
        ├── Parsing/
        │   ├── IAlbumParser.cs
        │   ├── ParserFactory.cs
        │   └── XmlAlbumParser.cs
        ├── Rendering/
        │   ├── IRenderer.cs
        │   └── PdfRenderer.cs
        └── Utilities/
            └── ArgsParser.cs
```
