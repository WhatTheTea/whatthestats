using whatthestats.Primitives;

namespace whatthestats;

internal static class ASCII
{
    public static string ProgressBar(Percentage percentage, int width = 20)
    {
        int filled = (int)Math.Round(percentage * width);
        int empty = width - filled;

        // Custom characters: █ (filled), ░ (empty)
        string bar = new string('█', filled - 1) + '░' + new string('-', empty);
        return $"[{bar}] {percentage}";
    }
}