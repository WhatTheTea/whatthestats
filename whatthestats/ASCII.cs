using whatthestats.Primitives;

namespace whatthestats;

internal static class ASCII
{
    public static string ProgressBar(Percentage percentage, int width = 20)
    {
        int filled = (int)Math.Round(percentage * width);
        int empty = width - filled - 1;

        // Custom characters: █ (filled), ░ (empty)
        string bar = new string('█', Math.Max(0, filled)) + '░' + new string('-', empty);
        return $"[{bar}] {percentage}";
    }
}