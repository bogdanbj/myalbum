# MyAlbum v7.0 XML Reference

## Overview

This document describes the XML format for defining stamp album pages.

---

## Root Element

```xml
<?xml version="1.0" encoding="utf-8"?>
<album title="Album Title" ver="7.0">
  <!-- pages go here -->
</album>
```

| Attribute | Description | Required |
|-----------|-------------|----------|
| `title` | Album title | No |
| `ver` / `version` | Format version | No |

---

## Page Element

```xml
<page title="Page Title" orientation="portrait" size="letter" margin="15">
  <!-- rows and elements go here -->
</page>
```

| Attribute | Description | Values | Default |
|-----------|-------------|--------|---------|
| `title` | Page title | text | none |
| `orientation` | Page orientation | `portrait`, `landscape` | `portrait` |
| `size` | Paper size | `letter`, `a4`, `legal` | `letter` |
| `margin` | Page margins (mm) | number | 15 |

---

## Row Element

```xml
<row align="center" valign="center" space="8" height="20">
  <!-- child elements -->
</row>
```

| Attribute | Description | Values | Default |
|-----------|-------------|--------|---------|
| `align` | Horizontal alignment | `left`, `center`, `right` | `left` |
| `valign` | Vertical alignment | `top`, `center`, `bottom` | `center` |
| `space` / `spacing` | Space between children (mm) | number | 3 |
| `height` | Fixed row height (mm) | number | auto |

---

## Text Element

```xml
<text font="Arial" size="12" style="bold" color="gray" align="center">
  Text content here
</text>
```

| Attribute | Description | Values | Default |
|-----------|-------------|--------|---------|
| `font` / `font_name` | Font family | font name | `Arial` |
| `size` / `font_size` | Font size (pt) | number | 10 |
| `style` / `font_style` | Font style | `bold`, `italic`, `bolditalic` | regular |
| `color` | Text color | name or `r,g,b` | `black` |
| `align` | Horizontal alignment | `left`, `center`, `right` | `left` |

---

## Stamp Element

```xml
<stamp title="1c Green" 
       width="22" height="26"
       border="double"
       inside1="King George VI"
       inside2="Mufti Issue"
       inside3=""
       footer1="Sc. 231" 
       footer2="Apr 1, 1937" 
       footer3="1c"
       border_color="black"
       font="Arial" />
```

| Attribute | Description | Values | Default |
|-----------|-------------|--------|---------|
| `title` | Title above frame | text | none |
| `width` | Frame width (mm) | number | 25 |
| `height` | Frame height (mm) | number | 30 |
| `border` / `frame` | Border style | `none`, `single`, `double`, `whiteace` | `double` |
| `inside1` | First inside line | text | none |
| `inside2` | Second inside line | text | none |
| `inside3` | Third inside line | text | none |
| `footer1` / `footer_left` | Left footer | text | none |
| `footer2` / `footer_center` | Center footer | text | none |
| `footer3` / `footer_right` | Right footer | text | none |
| `border_color` | Border color | name or `r,g,b` | `black` |
| `font` / `font_name` | Font for all text | font name | `Arial` |

---

## Common Attributes

These attributes can be used on most elements:

| Attribute | Description | Values |
|-----------|-------------|--------|
| `color` | Foreground color | name or `r,g,b` |
| `background` / `bg` | Background color | name or `r,g,b` |
| `margin` | Outer spacing (mm) | number |
| `padding` | Inner spacing (mm) | number |
| `style` | Named style reference | style name |

---

## Color Values

Colors can be specified as:

- **Named colors**: `black`, `white`, `red`, `green`, `blue`, `gray`, `darkgray`, `lightgray`
- **RGB values**: `255,0,0` (red), `0,128,0` (green), `48,48,48` (dark gray)

---

## Unit Values

Numeric values support optional unit suffixes:

| Suffix | Unit | Example |
|--------|------|---------|
| (none) | millimeters | `15` = 15mm |
| `mm` | millimeters | `15mm` |
| `pt` | points | `12pt` |
| `in` | inches | `0.5in` |

---

## Complete Example

```xml
<?xml version="1.0" encoding="utf-8"?>
<album title="Canada - 1937 Definitives" ver="7.0">
  
  <page title="Cover">
    <row align="center">
      <text font="Arial" size="28" style="bold">CANADA</text>
    </row>
    <row align="center">
      <text font="Arial" size="16">King George VI Definitives</text>
    </row>
  </page>

  <page title="Low Values">
    <row align="center">
      <text font="Arial" size="14" style="bold">1937 DEFINITIVES</text>
    </row>
    <row height="5" />
    <row align="center" space="8">
      <stamp title="1c Green" width="22" height="26"
             inside1="King George VI"
             footer1="Sc. 231" footer2="Apr 1, 1937" footer3="1c" />
      <stamp title="2c Brown" width="22" height="26"
             inside1="King George VI"
             footer1="Sc. 232" footer2="Apr 1, 1937" footer3="2c" />
      <stamp title="3c Carmine" width="22" height="26"
             inside1="King George VI"
             footer1="Sc. 233" footer2="Apr 1, 1937" footer3="3c" />
    </row>
  </page>

</album>
```
