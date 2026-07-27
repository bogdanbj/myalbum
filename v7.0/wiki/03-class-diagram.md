# MyAlbum v7.0 Class Diagram

## Overview

This document contains the class diagram for the current implementation of MyAlbum v7.0 after completing Phase 1 (Foundation) and Phase 2 (Stamp Support).

---

## Class Diagram

```mermaid
classDiagram
    direction TB

    %% INTERFACES
    class IAlbumParser {
        <<interface>>
        +Parse(filePath: string) Album
        +Parse(stream: Stream) Album
        +CanParse(filePath: string) bool
    }

    class IRenderer {
        <<interface>>
        +Render(album: Album, outputPath: string) void
        +Render(album: Album, outputStream: Stream) void
    }

    %% ENUMERATIONS
    class BorderStyle {
        <<enumeration>>
        None
        Single
        Double
        WhiteAce
    }

    class HorizontalAlignment {
        <<enumeration>>
        Left
        Center
        Right
    }

    class VerticalAlignment {
        <<enumeration>>
        Top
        Center
        Bottom
    }

    %% ROOT MODEL
    class Album {
        +Title: string?
        +Version: string?
        +Pages: List~Page~
        +AddPage(page: Page) void
    }

    %% ELEMENT HIERARCHY
    class BaseElement {
        <<abstract>>
        +X, Y, Width, Height: XUnit
        +MarginTop/Right/Bottom/Left: XUnit
        +PaddingTop/Right/Bottom/Left: XUnit
        +StyleName: string?
        +Color: XColor
        +BackgroundColor: XColor
        +OuterWidth: XUnit
        +OuterHeight: XUnit
        +ContentWidth: XUnit
        +ContentHeight: XUnit
        +Calculate(gfx, availWidth, availHeight)* void
        +Draw(gfx: XGraphics)* void
    }

    class ContainerElement {
        <<abstract>>
        +Children: List~BaseElement~
        +Spacing: XUnit
        +AddChild(child: BaseElement) void
        #DrawChildren(gfx: XGraphics) void
    }

    class Page {
        +PageNumber: int
        +Title: string?
        +PageSize: PageSize
        +Orientation: PageOrientation
        +VerticalSpacing: XUnit
        +PdfPage: PdfPage?
        +Calculate() void
        +Draw() void
    }

    class Row {
        +HAlign: HorizontalAlignment
        +VAlign: VerticalAlignment
        +HorizontalSpacing: XUnit
        +FixedHeight: XUnit?
        +Calculate() void
        +Draw() void
    }

    class Text {
        +Content: string
        +FontFamily: string
        +FontSize: double
        +Bold: bool
        +Italic: bool
        +HAlign: HorizontalAlignment
        +VAlign: VerticalAlignment
        +Calculate() void
        +Draw() void
    }

    class Frame {
        +BorderStyle: BorderStyle
        +LineWidth: double
        +SecondaryLineWidth: double
        +LineGap: double
        +BorderColor: XColor
        +CornerRadius: double
        +BorderThickness: double
        +InnerX/Y/Width/Height: XUnit
        +Calculate() void
        +Draw() void
        -DrawSingleBorder() void
        -DrawDoubleBorder() void
        -DrawWhiteAceBorder() void
    }

    class Stamp {
        +Title: string?
        +InsideLine1/2/3: string?
        +FooterLeft/Center/Right: string?
        +StampWidth: double
        +StampHeight: double
        +Frame: Frame
        +FontFamily: string
        +TitleFontSize: double
        +InsideFontSize: double
        +FooterFontSize: double
        +Calculate() void
        +Draw() void
        -DrawTitle() void
        -DrawInsideText() void
        -DrawFooter() void
    }

    %% PARSING
    class ParserFactory {
        <<static>>
        -Parsers: List~IAlbumParser~
        +GetParser(filePath: string) IAlbumParser
        +SupportedExtensions: IEnumerable~string~
    }

    class XmlAlbumParser {
        +Parse(filePath: string) Album
        +Parse(stream: Stream) Album
        +CanParse(filePath: string) bool
        -ParseAlbum() Album
        -ParsePage() Page
        -ParseElement() BaseElement?
        -ParseRow() Row
        -ParseText() Text
        -ParseStamp() Stamp
        -ParseCommonAttributes() void
        -TryParseUnit() bool
        -TryParseColor() bool
    }

    %% RENDERING
    class PdfRenderer {
        +Render(album: Album, outputPath: string) void
        +Render(album: Album, outputStream: Stream) void
        -RenderPage(document: PdfDocument, page: Page) void
    }

    %% UTILITIES
    class ArgsParser {
        <<static>>
        +Parse(args: string[]) Dictionary~string,string~
        +GetInputFile(options) string
        +GetOutputFile(options, inputFile) string
        +PrintHelp() void
        +PrintVersion() void
    }

    %% INHERITANCE
    BaseElement <|-- ContainerElement : extends
    BaseElement <|-- Text : extends
    BaseElement <|-- Frame : extends
    BaseElement <|-- Stamp : extends
    ContainerElement <|-- Page : extends
    ContainerElement <|-- Row : extends

    %% INTERFACE IMPLEMENTATIONS
    IAlbumParser <|.. XmlAlbumParser : implements
    IRenderer <|.. PdfRenderer : implements

    %% COMPOSITION
    Album "1" *-- "0..*" Page : contains
    ContainerElement "1" o-- "0..*" BaseElement : Children
    Stamp "1" *-- "1" Frame : has

    %% DEPENDENCIES
    ParserFactory ..> XmlAlbumParser : creates
    XmlAlbumParser ..> Album : creates
    PdfRenderer ..> Album : renders

    %% ENUM USAGE
    Frame ..> BorderStyle : uses
    Text ..> HorizontalAlignment : uses
    Text ..> VerticalAlignment : uses
    Row ..> HorizontalAlignment : uses
    Row ..> VerticalAlignment : uses
```

---

## Class Summary

### Models Layer

| Class | Type | Description |
|-------|------|-------------|
| `Album` | Model | Root container holding pages and metadata |
| `BaseElement` | Abstract | Base class for all renderable elements |
| `ContainerElement` | Abstract | Base for elements that contain children |
| `Page` | Container | Represents a single album page |
| `Row` | Container | Horizontal layout of child elements |
| `Text` | Leaf | Text content with font styling |
| `Frame` | Leaf | Border/frame with multiple styles |
| `Stamp` | Composite | Philatelic stamp with title, frame, text, footer |

### Parsing Layer

| Class | Type | Description |
|-------|------|-------------|
| `IAlbumParser` | Interface | Contract for album file parsers |
| `ParserFactory` | Static | Selects appropriate parser by file extension |
| `XmlAlbumParser` | Implementation | Parses XML/album files |

### Rendering Layer

| Class | Type | Description |
|-------|------|-------------|
| `IRenderer` | Interface | Contract for output renderers |
| `PdfRenderer` | Implementation | Generates PDF using PdfSharpCore |

### Utilities Layer

| Class | Type | Description |
|-------|------|-------------|
| `ArgsParser` | Static | CLI argument parsing |

---

## Key Relationships

1. **Inheritance Chain**: `BaseElement` → `ContainerElement` → `Page` / `Row`
2. **Inheritance Chain**: `BaseElement` → `Text`, `Frame`, `Stamp`
3. **Composition**: `Album` contains `Page` list
4. **Composition**: `Stamp` contains `Frame`
5. **Aggregation**: `ContainerElement` holds `BaseElement` children
6. **Implementation**: `XmlAlbumParser` implements `IAlbumParser`
7. **Implementation**: `PdfRenderer` implements `IRenderer`
