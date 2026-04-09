using System;
using System.Collections.Generic;
using System.Text;

namespace MyAlbum.Utilities
{
    internal class PageSelection
    {
        public List<int> SpecificPages { get; set; } = new List<int>();
        public int? FromPageOnwards { get; set; } = null;

        public bool IsEmpty => SpecificPages.Count == 0 && !FromPageOnwards.HasValue;

        public bool Includes(int pageNumber)
        {
            // If no filtering, include all pages
            if (IsEmpty)
                return true;

            // Check if in specific pages list
            if (SpecificPages.Contains(pageNumber))
                return true;

            // Check if in open-ended range
            if (FromPageOnwards.HasValue && pageNumber >= FromPageOnwards.Value)
                return true;

            return false;
        }
    }
}
