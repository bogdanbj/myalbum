using PdfSharpCore;
using PdfSharpCore.Drawing;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyAlbum.Utilities
{
    internal static class XmlParser
    {
        internal static XColor? ParseColor(string? value)
        {
            XColor? color = null;   // Returns null if parsing fails

            if (!string.IsNullOrWhiteSpace(value))
            {

                string[] arr = value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                switch (arr.Length)
                {
                    case 1:
                        try
                        {
                            PdfSharpCore.Drawing.XKnownColor knownColor = (PdfSharpCore.Drawing.XKnownColor)Enum.Parse(typeof(PdfSharpCore.Drawing.XKnownColor), arr[0], true);
                            color = XColor.FromKnownColor(knownColor);
                        }
                        catch { }
                        break;
                    case 3:
                        try
                        {
                            byte r = byte.Parse(arr[0]);
                            byte g = byte.Parse(arr[1]);
                            byte b = byte.Parse(arr[2]);
                            color = XColor.FromArgb(r, g, b);
                        }
                        catch { }
                        break;
                }
            }
            return color;
        }
        internal static PageOrientation? ParseOrientation(string? orientation)
        {
            if (!string.IsNullOrWhiteSpace(orientation) && Enum.TryParse(orientation, true, out PageOrientation result))
            {
                return result;
            }
            return null;
        }
        internal static PageSize? ParsePageSize(string? size)
        {
            if (!string.IsNullOrWhiteSpace(size) && Enum.TryParse(size, true, out PageSize result))
            {
                return result;
            }
            return null;
        }
        internal static (XUnit marginTop, XUnit marginRight, XUnit marginBottom, XUnit marginLeft) ParseMargin(string? margin)
        {
            XUnit marginTop = XUnit.Zero;
            XUnit marginRight = XUnit.Zero;
            XUnit marginBottom = XUnit.Zero;
            XUnit marginLeft = XUnit.Zero;

            if (!string.IsNullOrWhiteSpace(margin))
            {
                string[] arr = margin.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                switch (arr.Length)
                {
                    case 1:
                        marginTop = marginRight = marginBottom = marginLeft = ParseXUnit(arr[0]);
                        break;
                    case 2:
                        marginTop = marginBottom = ParseXUnit(arr[0]);
                        marginLeft = marginRight = ParseXUnit(arr[1]);
                        break;
                    case 4:
                        marginTop = ParseXUnit(arr[0]);
                        marginRight = ParseXUnit(arr[1]);
                        marginBottom = ParseXUnit(arr[2]);
                        marginLeft = ParseXUnit(arr[3]);
                        break;
                }
            }
            return (marginTop, marginRight, marginBottom, marginLeft);
        }
        internal static XUnit ParseXUnit(string value)
        {
            return XUnit.FromMillimeter(double.Parse(value));
        }


    }
}
