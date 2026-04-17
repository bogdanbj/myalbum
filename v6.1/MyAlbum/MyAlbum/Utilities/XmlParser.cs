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
            if (string.IsNullOrWhiteSpace(value))
                return null;

            string[] arr = value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            switch (arr.Length)
            {
                case 1:
                    try
                    {
                        XKnownColor knownColor = (XKnownColor)Enum.Parse(typeof(XKnownColor), arr[0], true);
                        return XColor.FromKnownColor(knownColor);
                    }
                    catch
                    {
                        return null;
                    }

                case 3:
                    try
                    {
                        byte r = byte.Parse(arr[0]);
                        byte g = byte.Parse(arr[1]);
                        byte b = byte.Parse(arr[2]);
                        return XColor.FromArgb(r, g, b);
                    }
                    catch
                    {
                        return null;
                    }

                default:
                    return null;
            }
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
        internal static (XUnit? marginTop, XUnit? marginRight, XUnit? marginBottom, XUnit? marginLeft) ParseMargin(string? margin)
        {
            return ParseSpacing(margin);
        }
        internal static (XUnit? paddingTop, XUnit? paddingRight, XUnit? paddingBottom, XUnit? paddingLeft) ParsePadding(string? padding)
        {
            return ParseSpacing(padding);
        }
        private static (XUnit? top, XUnit? right, XUnit? bottom, XUnit? left) ParseSpacing(string? value)
        {
            XUnit? top = null;
            XUnit? right = null;
            XUnit? bottom = null;
            XUnit? left = null;

            if (!string.IsNullOrWhiteSpace(value))
            {

                string[] arr = value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                switch (arr.Length)
                {
                    case 1:
                        top = right = bottom = left = ParseXUnit(arr[0]);
                        break;
                    case 2:
                        top = bottom = ParseXUnit(arr[0]);
                        left = right = ParseXUnit(arr[1]);
                        break;
                    case 4:
                        top = ParseXUnit(arr[0]);
                        right = ParseXUnit(arr[1]);
                        bottom = ParseXUnit(arr[2]);
                        left = ParseXUnit(arr[3]);
                        break;
                }
            }
            return (top, right, bottom, left);
        }
        internal static XUnit? ParseXUnit(string? value)
        {
            if (!string.IsNullOrWhiteSpace(value) && double.TryParse(value, out double result))
            {
                return XUnit.FromMillimeter(result);
            }
            return null;
        }
        internal static (FrameType? typeTop, FrameType? typeRight, FrameType? typeBottom, FrameType? typeLeft) ParseFrameType(string? frameType)
        {

            FrameType? 
                typeTop=null, 
                typeRight=null, 
                typeBottom=null, 
                typeLeft=null;

            if (!string.IsNullOrWhiteSpace(frameType))
            {

                FrameType result;
                
                string[] arr = frameType.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                switch (arr.Length)
                {
                    case 1:
                        typeTop = typeRight = typeBottom = typeLeft = Enum.TryParse(arr[0], true, out result) ? result : null;
                        break;
                    case 2:
                        typeTop = typeBottom = Enum.TryParse(arr[0], true, out result) ? result : null;
                        typeLeft = typeRight = Enum.TryParse(arr[1], true, out result) ? result : null;
                        break;
                    case 4:
                        typeTop = Enum.TryParse(arr[0], true, out result) ? result : null;
                        typeRight = Enum.TryParse(arr[1], true, out result) ? result : null;
                        typeBottom = Enum.TryParse(arr[2], true, out result) ? result : null;
                        typeLeft = Enum.TryParse(arr[3], true, out result) ? result : null;
                        break;
                }
            }

            return (typeTop, typeRight, typeBottom, typeLeft);


            //FrameType
            //    typeTop =FrameType.None, 
            //    typeRight=FrameType.None, 
            //    typeBottom=FrameType.None, 
            //    typeLeft=FrameType.None;

            //if (!string.IsNullOrWhiteSpace(frameType))
            //{

            //    string[] arr = frameType.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            //    FrameType result;

            //    switch (arr.Length)
            //    {
            //        case 1:
            //            typeTop = typeRight = typeBottom = typeLeft = Enum.TryParse(arr[0], true, out result) ? result : FrameType.None;
            //            break;
            //        case 2:
            //            typeTop = typeBottom = Enum.TryParse(arr[0], true, out result) ? result : FrameType.None;
            //            typeLeft = typeRight = Enum.TryParse(arr[1], true, out result) ? result : FrameType.None;
            //            break;
            //        case 4:
            //            typeTop = Enum.TryParse(arr[0], true, out result) ? result : FrameType.None;
            //            typeRight = Enum.TryParse(arr[1], true, out result) ? result : FrameType.None;
            //            typeBottom = Enum.TryParse(arr[2], true, out result) ? result : FrameType.None;
            //            typeLeft = Enum.TryParse(arr[3], true, out result) ? result : FrameType.None;
            //            break;
            //    }


            //}
            //return (typeTop, typeRight, typeBottom, typeLeft);
        }
        internal static (XUnit? lineWidth1, XUnit? offset, XUnit? lineWidth2) ParseFrameWidth(string? frameWidth)
        {
            // Default values
            XUnit lineWidth1 = 0;
            XUnit offset = 0;
            XUnit lineWidth2 = 0;

            if (!string.IsNullOrWhiteSpace(frameWidth))
            {
                string[] arr = frameWidth.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                switch (arr.Length)
                {
                    case 1:
                        lineWidth1 = XUnit.FromMillimeter(double.Parse(arr[0]));
                        offset = lineWidth2 = 0;
                        break;
                    case 2:
                        lineWidth1 = lineWidth2 = XUnit.FromMillimeter(double.Parse(arr[0]));
                        offset = XUnit.FromMillimeter(double.Parse(arr[1]));
                        break;
                    case 3:
                        lineWidth1 = XUnit.FromMillimeter(double.Parse(arr[0]));
                        offset = XUnit.FromMillimeter(double.Parse(arr[1]));
                        lineWidth2 = XUnit.FromMillimeter(double.Parse(arr[2]));
                        break;
                }

            }

            return (lineWidth1, offset, lineWidth2);
        }
        internal static Alignment? ParseAlignment(string? value)
        {
            if (!string.IsNullOrWhiteSpace(value) && Enum.TryParse(value, true, out Alignment align))
            {
                return align;
            }
            return null;
        }
        internal static VerticalAlignment? ParseVerticalAlignment(string? value)
        {
            if (!string.IsNullOrWhiteSpace(value) && Enum.TryParse(value, true, out VerticalAlignment align))
            {
                return align;
            }
            return null;
        }
        internal static SpacingMode? ParseSpacingMode(string? value)
        {
            if (!string.IsNullOrWhiteSpace(value) && Enum.TryParse(value, true, out SpacingMode mode))
            {
                return mode;
            }
            return null;
        }
        internal static bool ParseBool(string? value)
        {
            return bool.TryParse(value, out bool result) && result;
        }
        internal static double? ParseDouble(string? value)
        {
            if (!string.IsNullOrWhiteSpace(value) && double.TryParse(value, out double result))
            {
                return result;
            }
            return null;
        }
        internal static XFontStyle? ParseFontStyle(string? value)
        {
            if (!string.IsNullOrWhiteSpace(value) && Enum.TryParse(value, true, out XFontStyle style))
            {
                return style;
            }
            return null;
        }
    }
}