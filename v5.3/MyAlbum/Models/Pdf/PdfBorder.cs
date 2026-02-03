using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyAlbum.Models.Pdf
{
    internal class PdfBorder
    {
        //{
        //    get 
        //    {
        //        // All four sides are the same
        //        if (TypeTop == TypeRight && TypeTop == TypeBottom && TypeTop == TypeLeft)
        //        {
        //            return TypeTop.ToString().ToLower();
        //        }
        //        // Opposite sides are the same
        //        else if (TypeTop == TypeBottom && TypeRight == TypeLeft)
        //        {
        //            return string.Join(",",
        //                TypeTop.ToString().ToLower(),
        //                TypeRight.ToString().ToLower()
        //            );
        //        }
        //        // All sides are different
        //        else
        //        {
        //            return string.Join(",",
        //                TypeTop.ToString().ToLower(),
        //                TypeRight.ToString().ToLower(),
        //                TypeBottom.ToString().ToLower(),
        //                TypeLeft.ToString().ToLower()
        //            );
        //        }
        //    }
        //    set
        //    {
        //        BorderType Parse(string s) => 
        //            Enum.TryParse<BorderType>(s, true, out var result) ? result : default;

        //        if (string.IsNullOrWhiteSpace(value))
        //            TypeTop = TypeRight = TypeBottom = TypeLeft = BorderType.none;

        //        var parts = value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        //        if (parts.Length == 1)
        //        {
        //            TypeTop = TypeRight = TypeBottom = TypeLeft = Parse(parts[0]);
        //        }
        //        else if (parts.Length == 2)
        //        {
        //            TypeTop = TypeBottom = Parse(parts[0]);
        //            TypeRight = TypeLeft = Parse(parts[1]);
        //        }
        //        else if (parts.Length == 4)
        //        {
        //            TypeTop = Parse(parts[0]);
        //            TypeRight = Parse(parts[1]);
        //            TypeBottom = Parse(parts[2]);
        //            TypeLeft = Parse(parts[3]);
        //        }
        //    }
        //}
    }
}
