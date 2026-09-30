namespace MyAlbum.Utilities;

/// <summary>
/// Parses and applies page filter specifications.
/// Supports: explicit (1,2,6), ranges (1-5), open-ended (5+), all (all, *)
/// </summary>
public class PageFilter
{
    private readonly HashSet<int> _explicitPages = [];
    private readonly List<(int Start, int End)> _ranges = [];
    private int? _allAfter;
    private bool _all;

    public static PageFilter Parse(string spec)
    {
        var filter = new PageFilter();

        if (string.IsNullOrWhiteSpace(spec))
        {
            filter._all = true;
            return filter;
        }

        var trimmed = spec.Trim().ToLowerInvariant();
        if (trimmed == "all" || trimmed == "*")
        {
            filter._all = true;
            return filter;
        }

        var parts = spec.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        foreach (var part in parts)
        {
            if (part.EndsWith('+'))
            {
                // Open-ended: 5+
                var num = part[..^1];
                if (int.TryParse(num, out var start))
                    filter._allAfter = filter._allAfter.HasValue 
                        ? Math.Min(filter._allAfter.Value, start) 
                        : start;
            }
            else if (part.Contains('-'))
            {
                // Range: 1-5
                var rangeParts = part.Split('-', 2);
                if (rangeParts.Length == 2 &&
                    int.TryParse(rangeParts[0], out var rangeStart) &&
                    int.TryParse(rangeParts[1], out var rangeEnd))
                {
                    filter._ranges.Add((Math.Min(rangeStart, rangeEnd), Math.Max(rangeStart, rangeEnd)));
                }
            }
            else
            {
                // Explicit page number
                if (int.TryParse(part, out var page))
                    filter._explicitPages.Add(page);
            }
        }

        return filter;
    }

    public bool Includes(int pageNumber)
    {
        if (_all)
            return true;

        if (_explicitPages.Contains(pageNumber))
            return true;

        if (_allAfter.HasValue && pageNumber >= _allAfter.Value)
            return true;

        foreach (var (start, end) in _ranges)
        {
            if (pageNumber >= start && pageNumber <= end)
                return true;
        }

        return false;
    }

    public IEnumerable<T> Filter<T>(IEnumerable<T> pages, Func<T, int> getPageNumber)
    {
        return pages.Where(p => Includes(getPageNumber(p)));
    }

    public override string ToString()
    {
        if (_all) return "all";

        var parts = new List<string>();

        if (_explicitPages.Count > 0)
            parts.Add(string.Join(",", _explicitPages.OrderBy(p => p)));

        foreach (var (start, end) in _ranges)
            parts.Add($"{start}-{end}");

        if (_allAfter.HasValue)
            parts.Add($"{_allAfter}+");

        return string.Join(",", parts);
    }
}
