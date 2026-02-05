using System;
using System.Collections.Generic;
using System.Xml.Linq;
using MyAlbum.Models.Xml;
using MyAlbum.Models.Xml.Styles;

namespace MyAlbum.Services
{
    /// <summary>
    /// Custom deserialization service for Xml* classes.
    /// Uses XDocument instead of XmlSerializer to avoid bidirectional mapping conflicts.
    /// </summary>
    public static class XmlDeserializer
    {
        public static XmlAlbum DeserializeAlbum(string filePath)
        {
            var doc = XDocument.Load(filePath);
            return DeserializeAlbum(doc.Root);
        }

        public static XmlAlbum DeserializeAlbum(XElement root)
        {
            var album = new XmlAlbum
            {
                Version = root.Attribute("ver")?.Value
            };

            var stylesElement = root.Element("styles");
            if (stylesElement != null)
            {
                album.Styles = DeserializeStyles(stylesElement);
            }

            var pages = new List<XmlPage>();
            foreach (var pageElem in root.Elements("page"))
            {
                pages.Add(DeserializePage(pageElem));
            }
            album.Pages = pages;

            return album;
        }

        public static AlbumStyles DeserializeStyles(XElement stylesElement)
        {
            var styles = new AlbumStyles();

            foreach (var elem in stylesElement.Elements())
            {
                switch (elem.Name.LocalName.ToLower())
                {
                    case "border":
                        styles.BorderStyles ??= new();
                        styles.BorderStyles.Add(DeserializeBorderStyle(elem));
                        break;
                    case "text":
                        styles.TextStyles ??= new();
                        styles.TextStyles.Add(DeserializeTextStyle(elem));
                        break;
                    case "image":
                        styles.ImageStyles ??= new();
                        styles.ImageStyles.Add(DeserializeImageStyle(elem));
                        break;
                    case "stamp":
                        styles.StampStyles ??= new();
                        styles.StampStyles.Add(DeserializeStampStyle(elem));
                        break;
                    case "row":
                        styles.RowStyles ??= new();
                        styles.RowStyles.Add(DeserializeRowStyle(elem));
                        break;
                    case "column":
                        styles.ColumnStyles ??= new();
                        styles.ColumnStyles.Add(DeserializeColumnStyle(elem));
                        break;
                    case "page":
                        styles.PageStyles ??= new();
                        styles.PageStyles.Add(DeserializePageStyle(elem));
                        break;
                }
            }

            return styles;
        }

        public static BorderStyle DeserializeBorderStyle(XElement elem)
        {
            var style = new BorderStyle();
            DeserializeBaseElement(elem, style);
            style.BorderType = elem.Attribute("border-type")?.Value;
            style.BorderWidth = elem.Attribute("border-width")?.Value;
            style.Margin = elem.Attribute("margin")?.Value;
            style.Padding = elem.Attribute("padding")?.Value;
            return style;
        }

        public static TextStyle DeserializeTextStyle(XElement elem)
        {
            var style = new TextStyle();
            DeserializeBaseElement(elem, style);
            style.FontName = elem.Attribute("font_name")?.Value;
            if (double.TryParse(elem.Attribute("font_size")?.Value, out var size))
                style.FontSize = size;
            style.FontStyle = elem.Attribute("font_style")?.Value;
            style.Justify = elem.Attribute("justify")?.Value; // FIX: assign string, not bool
            style.Width = elem.Attribute("width")?.Value;
            style.Margin = elem.Attribute("margin")?.Value;
            return style;
        }

        public static ImageStyle DeserializeImageStyle(XElement elem)
        {
            var style = new ImageStyle();
            DeserializeBaseElement(elem, style);
            style.Color = elem.Attribute("color")?.Value;
            style.Margin = elem.Attribute("margin")?.Value;
            return style;
        }

        public static StampStyle DeserializeStampStyle(XElement elem)
        {
            var style = new StampStyle();
            DeserializeBaseElement(elem, style);
            style.IsDefault = elem.Attribute("default")?.Value?.ToLower() == "true";

            // Deserialize nested elements
            foreach (var childElem in elem.Elements())
            {
                switch (childElem.Name.LocalName.ToLower())
                {
                    case "border":
                        style.BorderStyle = DeserializeBorder(childElem);
                        break;
                    case "image":
                        style.ImageStyle = DeserializeImage(childElem);
                        break;
                    case "title":
                        style.TitleStyle = DeserializeText(childElem);
                        break;
                    case "inside1":
                        style.Inside1Style = DeserializeText(childElem);
                        break;
                    case "inside2":
                        style.Inside2Style = DeserializeText(childElem);
                        break;
                    case "inside3":
                        style.Inside3Style = DeserializeText(childElem);
                        break;
                    case "footer1":
                        style.Footer1Style = DeserializeText(childElem);
                        break;
                    case "footer2":
                        style.Footer2Style = DeserializeText(childElem);
                        break;
                    case "footer3":
                        style.Footer3Style = DeserializeText(childElem);
                        break;
                }
            }

            return style;
        }

        public static RowStyle DeserializeRowStyle(XElement elem)
        {
            var style = new RowStyle();
            DeserializeBaseElement(elem, style);
            style.SpacingMode = elem.Attribute("spacing-mode")?.Value;
            style.Space = elem.Attribute("space")?.Value;
            return style;
        }

        public static ColumnStyle DeserializeColumnStyle(XElement elem)
        {
            var style = new ColumnStyle();
            DeserializeBaseElement(elem, style);
            style.Space = elem.Attribute("space")?.Value;
            return style;
        }

        public static PageStyle DeserializePageStyle(XElement elem)
        {
            var style = new PageStyle();
            DeserializeBaseElement(elem, style);
            style.Orientation = elem.Attribute("orientation")?.Value;
            style.Size = elem.Attribute("size")?.Value;
            style.Margin = elem.Attribute("margin")?.Value;
            style.VSpace = elem.Attribute("vspace")?.Value;
            return style;
        }

        public static XmlBorder DeserializeBorder(XElement elem)
        {
            var border = new XmlBorder();
            DeserializeBaseElement(elem, border);
            border.BorderType = elem.Attribute("border-type")?.Value;
            border.BorderWidth = elem.Attribute("border-width")?.Value;
            border.Margin = elem.Attribute("margin")?.Value;
            border.Padding = elem.Attribute("padding")?.Value;
            return border;
        }

        public static XmlImage DeserializeImage(XElement elem)
        {
            var image = new XmlImage();
            DeserializeBaseElement(elem, image);
            image.Color = elem.Attribute("color")?.Value;
            image.FileName = elem.Attribute("file_name")?.Value;
            image.Margin = elem.Attribute("margin")?.Value;
            if (double.TryParse(elem.Attribute("width")?.Value, out var w))
                image.Width = w;
            if (double.TryParse(elem.Attribute("height")?.Value, out var h))
                image.Height = h;
            return image;
        }

        public static XmlText DeserializeText(XElement elem)
        {
            var text = new XmlText();
            DeserializeBaseElement(elem, text);
            text.FontName = elem.Attribute("font_name")?.Value;
            if (double.TryParse(elem.Attribute("font_size")?.Value, out var size))
                text.FontSize = size;
            text.FontStyle = elem.Attribute("font_style")?.Value;
            text.Justify = elem.Attribute("justify")?.Value; // FIX: assign string, not bool
            text.Width = elem.Attribute("width")?.Value;
            text.Margin = elem.Attribute("margin")?.Value;
            text.Value = elem.Value;
            return text;
        }

        public static XmlPage DeserializePage(XElement elem)
        {
            var page = new XmlPage();
            DeserializeBaseElement(elem, page);
            page.Orientation = elem.Attribute("orientation")?.Value;
            page.Size = elem.Attribute("size")?.Value;
            page.Margin = elem.Attribute("margin")?.Value;
            if (int.TryParse(elem.Attribute("no")?.Value, out var number))
                page.Number = number;
            page.Title = elem.Attribute("title")?.Value;
            return page;
        }

        private static void DeserializeBaseElement(XElement elem, XmlElement xmlElement)
        {
            xmlElement.Style = elem.Attribute("style")?.Value;
            xmlElement.Name = elem.Attribute("name")?.Value;
            xmlElement.Color = elem.Attribute("color")?.Value;
            xmlElement.BgColor = elem.Attribute("bgcolor")?.Value;
            xmlElement.Align = elem.Attribute("align")?.Value;
            xmlElement.VAlign = elem.Attribute("valign")?.Value;
        }
    }
}