# MyAlbum v7.0 Class Diagram

## Overview

Class diagram for MyAlbum v7.0 based on the architecture design. Shows two model namespaces (Definition and Layout), parsing, style resolution, layout calculation, and rendering.

---

## 1. Definition Model (Parsed)

```mermaid
classDiagram
    direction TB

    namespace Definition {
        class Album {
            +Title: string?
            +Author: string?
            +Subject: string?
            +StyleFile: string?
            +Styles: List~StyleDef~
            +Pages: List~Page~
        }

        class StyleDef {
            +Name: string
            +Type: string
            +Default: bool
            +Properties: Dictionary~string,object~
        }

        class Page {
            +Number: int
            +Title: string?
            +Style: string?
            +Size: string?
            +Orientation: string?
            +Padding: double[]
            +RowSpacing: double
            +ColumnSpacing: double
            +Children: List~Element~
        }

        class Element {
            <<abstract>>
            +Style: string?
            +X: double?
            +Y: double?
        }

        class Row {
            +Padding: double[]
            +Spacing: string
            +Align: string?
            +Children: List~Element~
        }

        class Column {
            +Width: string?
            +Padding: double[]
            +Spacing: double
            +Children: List~Element~
        }

        class Stamp {
            +Width: double
            +Height: double
            +Title: string?
            +TitlePadding: double
            +Image: string?
            +I1: string?
            +I2: string?
            +I3: string?
            +F1: string?
            +F2: string?
            +F3: string?
            +FooterPadding: double
        }

        class Text {
            +Content: string
            +Width: string?
            +Align: string?
            +FontName: string?
            +FontSize: double?
            +FontStyle: string?
            +Color: string?
            +BgColor: string?
        }

        class Image {
            +Src: string
            +Width: string?
            +Height: double?
            +ScaleMode: string?
        }

        class Frame {
            +Width: double
            +Height: double
            +Lines: double[]
            +Padding: double
            +Color: string?
        }

        class Space {
            +Width: double?
            +Height: double?
        }
    }

    Element <|-- Row
    Element <|-- Column
    Element <|-- Stamp
    Element <|-- Text
    Element <|-- Image
    Element <|-- Frame
    Element <|-- Space

    Album "1" *-- "0..*" Page
    Album "1" *-- "0..*" StyleDef
    Page "1" *-- "0..*" Element
    Row "1" *-- "0..*" Element
    Column "1" *-- "0..*" Element
```

---

## 2. Layout Model (Computed)

```mermaid
classDiagram
    direction TB

    namespace Layout {
        class Album {
            +Pages: List~Page~
        }

        class Page {
            +Width: double
            +Height: double
            +MasterElements: MasterElements
            +ContentElements: List~Element~
            +Calculate()
            +Draw(gfx: XGraphics)
        }

        class MasterElements {
            +Background: Image?
            +Banner: Image?
            +Border: Frame?
            +Header: Text?
            +Footer: Text?
        }

        class Element {
            <<abstract>>
            +X: double
            +Y: double
            +Width: double
            +Height: double
            +Calculate()*
            +Draw(gfx: XGraphics)*
        }

        class Container {
            <<abstract>>
            +Children: List~Element~
            +Calculate()
            +Draw(gfx: XGraphics)
        }

        class IComposite {
            <<interface>>
            +GetSubElements(): List~Element~
        }

        class Row {
            +BgColor: XColor?
            +Calculate()
            +Draw(gfx: XGraphics)
        }

        class Column {
            +BgColor: XColor?
            +Calculate()
            +Draw(gfx: XGraphics)
        }

        class Stamp {
            +Title: Text?
            +Frame: Frame
            +Image: Image?
            +I1: Text?
            +I2: Text?
            +I3: Text?
            +F1: Text?
            +F2: Text?
            +F3: Text?
            +Calculate()
            +Draw(gfx: XGraphics)
            +GetSubElements(): List~Element~
        }

        class Text {
            +Content: string
            +FontName: string
            +FontSize: double
            +FontStyle: string
            +Color: XColor
            +BgColor: XColor?
            +Align: string
            +Calculate()
            +Draw(gfx: XGraphics)
        }

        class Image {
            +Src: string
            +ScaleMode: string
            +Calculate()
            +Draw(gfx: XGraphics)
        }

        class Frame {
            +Lines: double[]
            +Padding: double
            +Color: XColor
            +Calculate()
            +Draw(gfx: XGraphics)
        }

        class Space {
            +Calculate()
            +Draw(gfx: XGraphics)
        }
    }

    Element <|-- Container
    Element <|-- Text
    Element <|-- Image
    Element <|-- Frame
    Element <|-- Space
    Element <|-- Stamp

    Container <|-- Row
    Container <|-- Column

    IComposite <|.. Stamp
    IComposite <|.. Page

    Album "1" *-- "0..*" Page
    Page "1" *-- "1" MasterElements
    Page "1" *-- "0..*" Element
    Container "1" *-- "0..*" Element

    Stamp "1" *-- "0..1" Text : Title
    Stamp "1" *-- "1" Frame
    Stamp "1" *-- "0..1" Image
    Stamp "1" *-- "0..1" Text : I1
    Stamp "1" *-- "0..1" Text : I2
    Stamp "1" *-- "0..1" Text : I3
    Stamp "1" *-- "0..1" Text : F1
    Stamp "1" *-- "0..1" Text : F2
    Stamp "1" *-- "0..1" Text : F3
```

---

## 3. Parsing Layer

```mermaid
classDiagram
    direction LR

    class AlbumLoader {
        +Load(filePath: string): Definition.Album
        -LoadStyles(styleFile: string): List~StyleDef~
    }

    class FormatDetector {
        +Detect(content: string): Format
    }

    class Format {
        <<enumeration>>
        Xml
        Json
        Yaml
    }

    class IAlbumParser {
        <<interface>>
        +Parse(content: string): Definition.Album
    }

    class XmlAlbumParser {
        +Parse(content: string): Definition.Album
    }

    class JsonAlbumParser {
        +Parse(content: string): Definition.Album
    }

    class YamlAlbumParser {
        +Parse(content: string): Definition.Album
    }

    IAlbumParser <|.. XmlAlbumParser
    IAlbumParser <|.. JsonAlbumParser
    IAlbumParser <|.. YamlAlbumParser

    AlbumLoader ..> FormatDetector : uses
    AlbumLoader ..> IAlbumParser : uses
    FormatDetector ..> Format : returns
```

---

## 4. Style Resolution

```mermaid
classDiagram
    direction LR

    class StyleResolver {
        +Resolve(album: Definition.Album): void
        -ResolveElement(element: Definition.Element): void
        -MergeStyles(base: StyleDef, override: StyleDef): StyleDef
    }

    class StyleSheet {
        +Styles: Dictionary~string,StyleDef~
        +GetDefault(elementType: string): StyleDef?
        +Get(styleName: string): StyleDef?
    }

    class StyleLoader {
        +Load(filePath: string): StyleSheet
    }

    StyleResolver ..> StyleSheet : uses
    StyleLoader ..> StyleSheet : creates
```

---

## 5. Layout Engine

```mermaid
classDiagram
    direction LR

    class LayoutEngine {
        +Calculate(album: Definition.Album): Layout.Album
    }

    class PageCalculator {
        +Calculate(page: Definition.Page): Layout.Page
        -CalculateCanvas(): Rect
        -CalculateMasterElements(): MasterElements
    }

    class ElementCalculator {
        +Calculate(element: Definition.Element, availableWidth: double): Layout.Element
    }

    class TextMeasurer {
        +Measure(text: string, font: XFont, maxWidth: double): Size
        +WrapText(text: string, font: XFont, maxWidth: double): List~string~
    }

    LayoutEngine ..> PageCalculator : uses
    LayoutEngine ..> ElementCalculator : uses
    PageCalculator ..> ElementCalculator : uses
    ElementCalculator ..> TextMeasurer : uses
```

---

## 6. Rendering Pipeline

```mermaid
classDiagram
    direction LR

    class PdfRenderer {
        +Render(album: Layout.Album, outputPath: string): void
        -CreateDocument(): PdfDocument
        -SetMetadata(doc: PdfDocument, album: Layout.Album): void
        -RenderSummaryPage(doc: PdfDocument): void
    }

    class PageRenderer {
        +Render(gfx: XGraphics, page: Layout.Page): void
        -RenderMasterElements(gfx: XGraphics): void
        -RenderContentElements(gfx: XGraphics): void
    }

    class ElementRenderer {
        <<abstract>>
        +Render(gfx: XGraphics, element: Layout.Element)*
    }

    class RowRenderer {
        +Render(gfx: XGraphics, row: Layout.Row): void
    }

    class ColumnRenderer {
        +Render(gfx: XGraphics, column: Layout.Column): void
    }

    class StampRenderer {
        +Render(gfx: XGraphics, stamp: Layout.Stamp): void
    }

    class TextRenderer {
        +Render(gfx: XGraphics, text: Layout.Text): void
        +RenderWrapped(gfx: XGraphics, text: Layout.Text): void
    }

    class ImageRenderer {
        +Render(gfx: XGraphics, image: Layout.Image): void
        -RenderPlaceholder(gfx: XGraphics, bounds: Rect): void
    }

    class FrameRenderer {
        +Render(gfx: XGraphics, frame: Layout.Frame): void
        -DrawLines(gfx: XGraphics, lines: double[]): void
    }

    PdfRenderer ..> PageRenderer : uses
    PageRenderer ..> ElementRenderer : uses
    ElementRenderer <|-- RowRenderer
    ElementRenderer <|-- ColumnRenderer
    ElementRenderer <|-- StampRenderer
    ElementRenderer <|-- TextRenderer
    ElementRenderer <|-- ImageRenderer
    ElementRenderer <|-- FrameRenderer
```

---

## 7. Resources & Startup

```mermaid
classDiagram
    direction LR

    class ResourceManager {
        +GetFont(name: string, style: FontStyle): XFont
        +GetImage(filename: string): XImage?
        +ImageExists(filename: string): bool
    }

    class FontLoader {
        +LoadAll(fontsFolder: string): void
        +GetFont(name: string, style: FontStyle): XFont
    }

    class ImageLoader {
        +Load(filename: string): XImage?
        +Exists(filename: string): bool
    }

    class ConfigLoader {
        +Load(configPath: string): Config
    }

    class Config {
        +TemplatesFolder: string
        +OutputFolder: string
        +ImagesFolder: string
        +FontsFolder: string
    }

    class ArgsParser {
        +Parse(args: string[]): Options
    }

    class Options {
        +InputFile: string
        +OutputFile: string?
        +Verbose: bool
        +Quiet: bool
    }

    ResourceManager ..> FontLoader : uses
    ResourceManager ..> ImageLoader : uses
    ConfigLoader ..> Config : creates
    ArgsParser ..> Options : creates
```

---

## 8. Class Summary

### Definition Namespace (Parsed Model)

| Class | Type | Description |
|-------|------|-------------|
| `Album` | Model | Root container with metadata and pages |
| `StyleDef` | Model | Style definition with properties |
| `Page` | Model | Page definition with children |
| `Element` | Abstract | Base for all elements |
| `Row` | Container | Horizontal layout |
| `Column` | Container | Vertical layout |
| `Stamp` | Composite | Stamp with title, frame, interior, footer |
| `Text` | Leaf | Text content |
| `Image` | Leaf | Image reference |
| `Frame` | Leaf | Border/frame |
| `Space` | Leaf | Empty space |

### Layout Namespace (Computed Model)

| Class | Type | Description |
|-------|------|-------------|
| `Album` | Model | Computed album with pages |
| `Page` | Container + Composite | Computed page with master and content |
| `MasterElements` | Model | Page master elements (banner, border, etc.) |
| `Element` | Abstract | Base with X, Y, Width, Height, Calculate(), Draw() |
| `Container` | Abstract | Base for Row, Column with children |
| `IComposite` | Interface | For elements with fixed sub-elements |
| `Row` | Container | Computed row |
| `Column` | Container | Computed column |
| `Stamp` | Composite | Computed stamp with sub-elements |
| `Text` | Leaf | Computed text |
| `Image` | Leaf | Computed image |
| `Frame` | Leaf | Computed frame |
| `Space` | Leaf | Computed space |

### Other Layers

| Layer | Key Classes |
|-------|-------------|
| **Parsing** | AlbumLoader, FormatDetector, IAlbumParser, XmlAlbumParser, JsonAlbumParser, YamlAlbumParser |
| **Styles** | StyleResolver, StyleSheet, StyleLoader |
| **Layout** | LayoutEngine, PageCalculator, ElementCalculator, TextMeasurer |
| **Rendering** | PdfRenderer, PageRenderer, ElementRenderer (Row, Column, Stamp, Text, Image, Frame) |
| **Resources** | ResourceManager, FontLoader, ImageLoader |
| **Startup** | ConfigLoader, ArgsParser |

---

## 9. Key Relationships

1. **Definition.Element hierarchy**: Element (abstract) → Row, Column, Stamp, Text, Image, Frame, Space
2. **Layout.Element hierarchy**: Element (abstract) → Container (abstract) → Row, Column; Element → Text, Image, Frame, Space, Stamp
3. **IComposite implementers**: Layout.Stamp, Layout.Page
4. **Parser pattern**: AlbumLoader uses FormatDetector to select IAlbumParser implementation
5. **Two-pass processing**: StyleResolver (pass 1) → LayoutEngine (pass 2)
6. **Rendering delegation**: PdfRenderer → PageRenderer → ElementRenderer subclasses
