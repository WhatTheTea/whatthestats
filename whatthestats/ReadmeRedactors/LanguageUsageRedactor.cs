using System.Text;
using whatthestats.Primitives;

namespace whatthestats.ReadmeRedactors;

public sealed class LanguageUsageRedactor(Stream readmeStream, LanguagesUsage usage) : ReadmeRedactor(readmeStream)
{
    private readonly Stream readmeStream = readmeStream;
    public override string RedactorAlias { get; } = "wts-languages";

    public override async Task ApplyAsync()
    {
        var readme = reader.ReadToEnd();
        var block = GetBlockRange(readme);
        (var offset, var length) = block.GetOffsetAndLength(readme.Length);
        var languagesBlockBuilder = new StringBuilder();

        var topUsedLanguages = usage.Values.OrderByDescending(x => x.Usage).Take(10);
        var maxWidth = topUsedLanguages.Max(x => x.Language.Length);
        foreach (var use in topUsedLanguages)
        {
            var usageBar = ASCII.ProgressBar(use.Usage);
            languagesBlockBuilder.AppendLine($"{use.Language.PadRight(maxWidth)}: {usageBar}");
        }   
        readme = readme.Remove(block.Start.Value, length);
        readme = readme.Insert(block.Start.Value, Environment.NewLine + languagesBlockBuilder.ToString());

        readmeStream.Seek(0, SeekOrigin.Begin);
        writer.Write(readme);
        writer.Flush();
    }
}