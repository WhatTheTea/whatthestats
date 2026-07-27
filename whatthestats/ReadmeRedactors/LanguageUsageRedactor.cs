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
        var start = GetWritingIndex(readme);
        var languagesBlockBuilder = new StringBuilder();

        languagesBlockBuilder.AppendLine("Languages:");
        foreach (var use in usage.Values.OrderByDescending(x => x.Usage).Take(10))
        {
            var usageBar = ASCII.ProgressBar(use.Usage);
            languagesBlockBuilder.AppendLine($"{use.Language}: {usageBar}");
        }   

        readme = readme.Insert(start, languagesBlockBuilder.ToString());

        readmeStream.Seek(0, SeekOrigin.Begin);
        writer.Write(readme);
        writer.Flush();
    }
}