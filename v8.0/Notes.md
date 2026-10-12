# MyAlbum

Elements have Margin and Canvas. Paddings is obsolete. 

```
┌───────────────────────── Parent.Canvas ───────────────────────┐
│┌─────────────────────────── Element ─────────────────────────┐│  ← Element.Y
││  ↕ Element.MarginTop                                        ││
││  ┌───────────────────── Element.Canvas ──────────────────┐  ││  ← Element.Canvas.Y
││  │ [Element Content]                                     │  ││
││  └───────────────────────────────────────────────────────┘  ││
││  ↕ Element.MarginBottom                                     ││
│└─────────────────────────────────────────────────────────────┘│  ← Element.Height
└───────────────────────────────────────────────────────────────┘  ← Parent.Canvas.Height
```


## Element — Height

- **Height** = Content Height + MarginTop + MarginBottom
- **Content Height** = Canvas Height
- If Height is **explicitly set**, the element height is the larger of the explicit height and the content height. The exception is Page, whose height is fixed and defined by Page.Size.
- Element's X and Y are coordinates relative to Parent.Canvas
- Row's Height is calculatd based on the element heights below and above the alignment point and their margins
- Column's Height = 100% of Parent.Canvas.Height. If parent height is not defined the parent height extends to 100%.  For example a Row contains Columns. The Row's height is calculated from its content, but Column's height is calculated based on Row. Then bothe Row and Columns's height gets extended to 100% of the remaining Page Canvas.


## Element — Width

- Row width = 100% of Parent.Canvas.W
- Column width can be expressed either as a percentage of **Parent.Canvas.Width** or as an absolute value in millimeters. The last column may omit its width, in which case it is calculated to fill the remaining space in the parent canvas.
- Same coordinates / critical points apply as in height diagram.

## Element — Margin and Canvas

- Container elements have canvas

Element | Notes on width and height
--- | ---
Page | Width and Height are outbound.
Page Banner | Border draws at outbound edge.



Element | Margin | Padding | Canvas
---|---|---|---
Base Element | X | X | X
Page | O | X | X
Frame<br>Page Border<br>Stamp Frame | O | X | O
Image<br>Page Banner | X | O | O
Text<br>Paragraph | X | O | O
Space | O | O | O
Stamp | X | O | X
Row | O | X | X
Column | O | X | X


## Drawing

Parent: 
```csharp
DrawElement(gfx, element, rotate)
```

BaseElement: 
```csharp
gfx.TranslateTransform(elem.X.Point, elem.Y.Point);
if (rotate)
    gfx.RotateTransform(90); 
elem.Draw(gfx);
```

Element: 
```csharp
base.Draw(gfx);

// element drawing logic here. Needs to account for margins.
if (XImg != null)
{
    gfx.DrawImage(XImg, MarginLeft.Point, MarginTop.Point, Canvas.W.Point, Canvas.H.Point);
}
```

BaseELement:
```csharp
LogDraw();
gfx.DrawRectangle(new XPen(Color, 0.5), new XSolidBrush(BgColor), 0, 0, W.Point, H.Point);
```