using LexiMon.Repository.Domains;

namespace LexiMon.Repository.Utils;

public static class LevelCalculator
{
    public static int CalculateLevel(int exp, IReadOnlyList<LevelRange> ranges)
    {
        if (ranges == null! || ranges.Count == 0) return 1;

        var ordered = ranges.OrderBy(r => r.FromExp).ToList();

        var match = ordered.LastOrDefault(r => exp >= r.FromExp && exp <= r.ToExp);
        if (match != null)
        {
            return ordered.IndexOf(match) + 1;
        }

        if (exp > ordered[^1].ToExp) return ordered.Count;

        return 1;
    }
}