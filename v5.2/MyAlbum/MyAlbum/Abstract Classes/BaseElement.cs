using PdfSharp;
using PdfSharp.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MyAlbum
{
    internal class BaseElement
    {
        #region fields
        private string? _styleName;
        #endregion

        #region properties
        public XElement xml { get; set; }
        public string? StyleName
        {
            get { return _styleName; }
            set { _styleName = value; }
        }
        #endregion

        #region constructors
        public BaseElement()
        {
            xml = new XElement("none");
        }
        #endregion

        #region methods
        public virtual void Parse()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the value of "style" attribute of an xml node, or null if not provided.
        /// </summary>
        /// <returns>The style name as a string.</returns>
        internal string? ParseStyleNameAttribute(XElement xml)
        {
            string? result = null;
            if (xml.Attribute("style") != null)
            {
                result = xml.Attribute("style")!.Value.ToLower();
            }
            return result;
        }
        /// <summary>
        /// Returns the value of "default" attribute of an xml node, or false if not provided.
        /// </summary>
        /// <returns>The default flag as a boolean.</returns>
        internal bool ParseDefaultAttribute(XElement xml)
        {
            bool result = false;
            if (xml.Attribute("default") != null)
            {
                if (xml.Attribute("default")!.Value.ToLower() == "true")
                {
                    result = true;
                }
            }
            return result;
        }
        /// <summary>
        /// Returns the values of "absolute" attribute of an xml node, or false if not provided.
        /// </summary>
        /// <returns>An absolue positioning flag as a boolean.</returns>
        internal bool ParseAbsoluteAttribute(XElement xml)
        {
            bool result = false;
            if (xml.Attribute("absolute") != null)
            {
                if (xml.Attribute("absolute")!.Value.ToLower() == "true")
                {
                    result = true;
                }
            }
            return result;
        }
        /// <summary>
        /// Returns the values of "margin" attribute of an xml node, or XUnitPt.Zero if not provided.
        /// </summary>
        /// <returns>An array of four XUnitPt [top, right, bottom, left].</returns>
        internal XUnitPt[] ParseMarginAttribute(XElement xml)
        {
            XUnitPt left = XUnitPt.Zero;
            XUnitPt top = XUnitPt.Zero;
            XUnitPt right = XUnitPt.Zero;
            XUnitPt bottom = XUnitPt.Zero;

            if (xml.Attribute("margin") != null)
            {
                try
                {
                    string[] arr = (xml.Attribute("margin")?.Value ?? "0").Split(',');

                    switch (arr.Length)
                    {
                        case 1:
                            top = right = bottom = left = XUnitPt.FromMillimeter(double.Parse(arr[0]));
                            break;
                        case 2:
                            top = bottom = XUnitPt.FromMillimeter(double.Parse(arr[0]));
                            left = right = XUnitPt.FromMillimeter(double.Parse(arr[1]));
                            break;
                        case 4:
                            top = XUnitPt.FromMillimeter(double.Parse(arr[0]));
                            right = XUnitPt.FromMillimeter(double.Parse(arr[1]));
                            bottom = XUnitPt.FromMillimeter(double.Parse(arr[2]));
                            left = XUnitPt.FromMillimeter(double.Parse(arr[3]));
                            break;
                        default:
                            top = right = bottom = left = XUnitPt.Zero;
                            break;
                    }
                }
                catch (Exception)
                {
                    top = right = bottom = left = XUnitPt.Zero;
                }
            }
            return new XUnitPt[] { top, right, bottom, left };
        }
        /// <summary>
        /// Returns the values of "padding" attribute of an xml node, or XUnitPt.Zero if not provided.
        /// </summary>
        /// <returns>An array of four XUnitPt [top, right, bottom, left].</returns>
        internal XUnitPt[] ParsePaddingAttribute(XElement xml)
        {
            XUnitPt left = XUnitPt.Zero;
            XUnitPt top = XUnitPt.Zero;
            XUnitPt right = XUnitPt.Zero;
            XUnitPt bottom = XUnitPt.Zero;

            if (xml.Attribute("padding") != null)
            {
                try
                {
                    string[] arr = (xml.Attribute("padding")?.Value ?? "0").Split(',');

                    switch (arr.Length)
                    {
                        case 1:
                            top = right = bottom = left = XUnitPt.FromMillimeter(double.Parse(arr[0]));
                            break;
                        case 2:
                            top = bottom = XUnitPt.FromMillimeter(double.Parse(arr[0]));
                            left = right = XUnitPt.FromMillimeter(double.Parse(arr[1]));
                            break;
                        case 4:
                            top = XUnitPt.FromMillimeter(double.Parse(arr[0]));
                            right = XUnitPt.FromMillimeter(double.Parse(arr[1]));
                            bottom = XUnitPt.FromMillimeter(double.Parse(arr[2]));
                            left = XUnitPt.FromMillimeter(double.Parse(arr[3]));
                            break;
                        default:
                            top = right = bottom = left = XUnitPt.Zero;
                            break;
                    }
                }
                catch (Exception)
                {
                    top = right = bottom = left = XUnitPt.Zero;
                }
            }
            return new XUnitPt[] { top, right, bottom, left };
        }
        /// <summary>
        /// Returns the values of "color" attribute of an xml node, or XColors.Black if not provided.
        /// </summary>
        /// <returns>The foreground color as an XColor.</returns>
        internal XColor ParseColorAttribute(XElement xml)
        {
            XColor result = XColors.Black;
            if (xml.Attribute("color") != null)
            {
                string[] rgb = (xml.Attribute("color")?.Value ?? "0,0,0").Split(',');
                try
                {
                    result = XColor.FromArgb(int.Parse(rgb[0]), int.Parse(rgb[1]), int.Parse(rgb[2]));
                }
                catch (Exception)
                {
                    result = XColors.Black;
                }
            }
            return result;
        }
        /// <summary>
        /// Returns the values of "bgcolor" attribute of an xml node, or XBrushes.White if not provided.
        /// </summary>
        /// <returns>The background color as an XBrush.</returns>
        internal XBrush ParseBackgroundColorAttribute(XElement xml)
        {
            XBrush result = XBrushes.White;
            if (xml.Attribute("bgcolor") != null)
            {
                string[] rgb = (xml.Attribute("bgcolor")?.Value ?? "255,255,255").Split(',');
                try
                {
                    result = new XSolidBrush(XColor.FromArgb(int.Parse(rgb[0]), int.Parse(rgb[1]), int.Parse(rgb[2])));
                }
                catch (Exception)
                {
                    result = XBrushes.White;
                }
            }
            return result;
        }
        /// <summary>
        /// Returns the values of "align" attribute of an xml node, or Alignments.Left if not provided.
        /// </summary>
        /// <returns>The horizontal alignment as a Styles.Alignment.</returns>
        internal Styles.Alignments ParseAlignAttribute(XElement xml)
        {
            Styles.Alignments result = Styles.Alignments.Left;
            if (xml.Attribute("align") != null)
            {
                switch (xml.Attribute("align")!.Value.ToLower())
                {
                    case "left":
                        result = Styles.Alignments.Left;
                        break;
                    case "center":
                        result = Styles.Alignments.Center;
                        break;
                    case "right":
                        result = Styles.Alignments.Right;
                        break;
                    default:
                        result = Styles.Alignments.Left;
                        break;
                }
            }
            return result;
        }
        /// <summary>
        /// Returns the values of "valign" attribute of an xml node or VerticalAlignments.Top if not provided.
        /// </summary>
        /// <returns>The horizontal alignment as a Styles.VerticalAlignment.</returns>
        internal Styles.VerticalAlignments ParseVAlignAttribute(XElement xml)
        {
            Styles.VerticalAlignments result = Styles.VerticalAlignments.Top;
            if (xml.Attribute("valign") != null)
            {
                switch (xml.Attribute("valign")!.Value.ToLower())
                {
                    case "top":
                        result = Styles.VerticalAlignments.Top;
                        break;
                    case "center":
                        result = Styles.VerticalAlignments.Center;
                        break;
                    case "bottom":
                        result = Styles.VerticalAlignments.Bottom;
                        break;
                    default:
                        result = Styles.VerticalAlignments.Top;
                        break;
                }
            }
            return result;
        }
        /// <summary>
        /// Returns the values of "orientation" attribute of an xml node or PageOrientation.Portrait if not provided.
        /// </summary>
        /// <returns>The page orientation as a PageOrientation.</returns>
        internal PageOrientation ParsePageOrientationAttribute(XElement xml)
        {
            PageOrientation result = PageOrientation.Portrait;
            if (xml.Attribute("orientation") != null)
            {
                switch (xml.Attribute("orientation")!.Value.ToLower())
                {
                    case "portrait":
                        result = PdfSharp.PageOrientation.Portrait;
                        break;
                    case "landscape":
                        result = PdfSharp.PageOrientation.Landscape;
                        break;
                    default:
                        result = PdfSharp.PageOrientation.Portrait;
                        break;
                }
            }
            return result;


        }
        /// <summary>
        /// Returns the values of "size" attribute of an xml node or PageSize.Letter if not provided.
        /// </summary>
        /// <returns>The page orientation as a PageSize.</returns>
        internal PageSize ParsePageSizeAttribute(XElement xml)
        {
            PageSize result = PageSize.Letter;
            if (xml.Attribute("size") != null)
            {
                switch (xml.Attribute("size")!.Value.ToLower())
                {
                    case "letter":
                        result = PdfSharp.PageSize.Letter;
                        break;
                    case "a4":
                        result = PdfSharp.PageSize.A4;
                        break;
                    default:
                        result = PdfSharp.PageSize.Letter;
                        break;
                }
            }
            return result;
        }
        /// <summary>
        /// Returns the values of "vspace" attribute of an xml node, or XUnitPt.Zero if not provided.
        /// </summary>
        /// <returns>Th vertical space as an XUnitPt.</returns>
        internal XUnitPt ParseVSpaceAttribute(XElement xml)
        {
            XUnitPt result =XUnitPt.Zero;
            if (xml.Attribute("vspace") != null)
            {
                try
                {
                    result = XUnitPt.FromMillimeter(double.Parse(xml.Attribute("vspace")!.Value));
                }
                catch (Exception)
                {
                    result = XUnitPt.Zero;
                }
            }
            return result;
        }
        /// <summary>
        /// Returns the values of "border_type" attribute of an xml node, or Styles.BorderTypes.None if not provided.
        /// </summary>
        /// <returns>An array of four Styles.BorderTypes [top, right, bottom, left].</returns>
        internal Styles.BorderTypes[] ParseBorderTypeAttribute(XElement xml)
        {
            Styles.BorderTypes top = Styles.BorderTypes.None;
            Styles.BorderTypes right = Styles.BorderTypes.None;
            Styles.BorderTypes bottom = Styles.BorderTypes.None;
            Styles.BorderTypes left = Styles.BorderTypes.None;

            if (xml.Attribute("border_type") != null)
            {
                try
                {
                    string[] arr = (xml.Attribute("border_type")!.Value).Split(',');

                    switch (arr.Length)
                    {
                        case 1:
                            top = right = bottom = left = Styles.GetBorderTypeFromString(arr[0]);
                            break;
                        case 2:
                            top = bottom = Styles.GetBorderTypeFromString(arr[0]);
                            left = right = Styles.GetBorderTypeFromString(arr[1]);
                            break;
                        case 4:
                            top = Styles.GetBorderTypeFromString(arr[0]);
                            right = Styles.GetBorderTypeFromString(arr[1]);
                            bottom = Styles.GetBorderTypeFromString(arr[2]);
                            left = Styles.GetBorderTypeFromString(arr[3]);
                            break;
                        default:
                            top = right = bottom = left = Styles.BorderTypes.None;
                            break;
                    }
                }
                catch (Exception)
                {
                    top = right = bottom = left = Styles.BorderTypes.None;
                }
            }
            return new Styles.BorderTypes[] { top, right, bottom, left };
        }
        /// <summary>
        /// Returns the values of "width" attribute of an xml node, or XUnitPt.Zero if not provided.
        /// </summary>
        /// <returns>An array of three XUnitPt [lineWidth1, offset, lineWidth2].</returns>
        internal XUnitPt[] ParseBorderLineWidthAttribute(XElement xml)
        {
            XUnitPt lineWidth1 = XUnitPt.Zero;
            XUnitPt lineWidth2 = XUnitPt.Zero;
            XUnitPt offset = XUnitPt.Zero;

            if (xml.Attribute("width") != null)
            {
                try
                {
                    string[] arr = (xml.Attribute("width")!.Value).Split(',');

                    switch (arr.Length)
                    {
                        case 1:
                            lineWidth1 = offset = lineWidth2  = XUnitPt.FromMillimeter(double.Parse(arr[0]));
                            break;
                        case 2:
                            lineWidth1 = lineWidth2 = XUnitPt.FromMillimeter(double.Parse(arr[0])); 
                            offset = XUnitPt.FromMillimeter(double.Parse(arr[1])); 
                            break;
                        case 3:
                            lineWidth1 = XUnitPt.FromMillimeter(double.Parse(arr[0]));
                            offset = XUnitPt.FromMillimeter(double.Parse(arr[1]));
                            lineWidth2 = XUnitPt.FromMillimeter(double.Parse(arr[2]));
                            break;
                        default:
                            lineWidth1 = lineWidth2 = offset = XUnitPt.Zero;
                            break;
                    }
                }
                catch (Exception)
                {
                    lineWidth1 = offset = lineWidth2 = XUnitPt.Zero;
                }
            }
            return new XUnitPt[] { lineWidth1, offset, lineWidth2 };
        }
        /// <summary>
        /// Returns the values of "spacing-mode" attribute of an xml node, or SpacingModes.ES if not provided.
        /// </summary>
        /// <returns>The row spacing mode as a Styles.SpacingMode.</returns>
        internal Styles.SpacingModes ParseSpacingModeAttribute(XElement xml)
        {
            Styles.SpacingModes result = Styles.SpacingModes.ES;
            if (xml.Attribute("spacing-mode") != null)
            {
                switch (xml.Attribute("spacing-mode")!.Value.ToLower())
                {
                    case "fs":
                        result = Styles.SpacingModes.FS;
                        break;
                    case "es":
                        result = Styles.SpacingModes.ES;
                        break;
                    case "js":
                        result = Styles.SpacingModes.JS;
                        break;
                    default:
                        result = Styles.SpacingModes.ES;
                        break;
                }
            }
            return result;
        }
        /// <summary>
        /// Returns the values of "space" attribute of an xml node, or XUnitPt.Zero if not provided.
        /// </summary>
        /// <returns>The space between child elements as an XUnitPt.</returns>
        internal XUnitPt ParseSpaceAttribute(XElement xml)
        {
            XUnitPt result = XUnitPt.Zero;
            if (xml.Attribute("space") != null)
            {
                try
                {
                    result = XUnitPt.FromMillimeter(double.Parse(xml.Attribute("space")!.Value));
                }
                catch (Exception)
                {
                    result = XUnitPt.Zero;
                }
            }
            return result;
        }
        /// <summary>
        /// Returns the values of "rotate" attribute of an xml node, or false if not provided.
        /// </summary>
        /// <returns>The rotate flag as a boolean.</returns>
        internal bool ParseRotateAttribute(XElement xml)
        {
            bool result = false;
            if (xml.Attribute("rotate") != null)
            {
                if (xml.Attribute("rotate")!.Value.ToLower() == "true")
                {
                    result = true;
                }
            }
            return result;
        }
        /// <summary>
        /// Returns the values of "width" attribute of an xml node, or XUnitPt.Zero if not provided.
        /// </summary>
        /// <returns>The width as an XUnitPt.</returns>
        internal XUnitPt ParseWidthAttribute(XElement xml)
        {
            XUnitPt result = XUnitPt.Zero;
            if (xml.Attribute("width") != null)
            {
                try
                {
                    result = XUnitPt.FromMillimeter(double.Parse(xml.Attribute("width")!.Value));
                }
                catch (Exception)
                {
                    result = XUnitPt.Zero;
                }
            }
            return result;
        }
        /// <summary>
        /// Returns the values of "height" attribute of an xml node, or XUnitPt.Zero if not provided.
        /// </summary>
        /// <returns>The height as an XUnitPt.</returns>
        internal XUnitPt ParseHeightAttribute(XElement xml)
        {
            XUnitPt result = XUnitPt.Zero;
            if (xml.Attribute("height") != null)
            {
                try
                {
                    result = XUnitPt.FromMillimeter(double.Parse(xml.Attribute("height")!.Value));
                }
                catch (Exception)
                {
                    result = XUnitPt.Zero;
                }
            }
            return result;
        }

        internal string? ParseStringAttribute(XElement xml, string attributeName)
        {
            string? result = null;
            if (xml.Attribute(attributeName) != null)
            {
                result = xml.Attribute(attributeName)!.Value.ToLower();
            }
            return result;
        }
        #endregion
    }
}