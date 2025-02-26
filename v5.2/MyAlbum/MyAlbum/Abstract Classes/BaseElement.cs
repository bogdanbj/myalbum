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
        #region properties
        public XElement xml { get; set; }
        #endregion  

        protected BaseElement()
        {
            xml = new XElement("none");
        }

        public virtual void Parse()
        {
            throw new NotImplementedException();
        }

        internal string? ParseStyleNameAttribute(XElement xml)
        {
            string? result = null;
            if (xml.Attribute("style") != null)
            {
                result = xml.Attribute("style")!.Value.ToLower();
            }
            return result;
        }
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
        /// Returns an array with exactly four XUnitPt elements.
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
        /// Returns an array with exactly four XUnitPt elements.
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

    }
}
