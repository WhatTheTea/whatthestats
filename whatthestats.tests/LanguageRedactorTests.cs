using System.Text;
using Shouldly;
using whatthestats.Primitives;
using whatthestats.ReadmeRedactors;

namespace whatthestats.tests;

public sealed class LanguageRedactorTests
{
    private readonly byte[] simpleReadmeBytes = Encoding.ASCII.GetBytes($"""
        begin
        ```wts-languages
        ```
        end
        """);

    private readonly LanguagesUsage simpleUsage = [new("C#", 1)];

    private readonly LanguagesUsage fourUsage = [
        new("C#", .25), 
        new("Holy C", .25),
        new("C++", .25),
        new("C", .25)
    ];

    private void CreateRedactor(out MemoryStream stream, 
        out StreamReader reader, 
        out LanguageUsageRedactor redactor, 
        LanguagesUsage? usage = null,
        byte[]? readmeBytes = null)
    {
        stream = new MemoryStream();
        stream.Write(readmeBytes ?? simpleReadmeBytes);
        stream.Flush();
        stream.Seek(0, SeekOrigin.Begin);
        reader = new StreamReader(stream, leaveOpen: true);
        redactor = new LanguageUsageRedactor(stream, usage ?? simpleUsage);
    }
    
    [Fact]
    public async Task WorksExactlyInSpecifiedBlock()
    {
        CreateRedactor(out var stream, out var reader, out var redactor);

        await redactor.ApplyAsync();
        redactor.Dispose();

        var redacted = reader.ReadToEnd();
        var contents = redacted.Trim()
            .Split('\n')
            .Select(x => x.Trim())
            .ToArray();
        
        contents.ShouldSatisfyAllConditions(redacted,
            () => contents[0].ShouldBe("begin"),
            () => contents[1].ShouldBe("```wts-languages"),
            () => contents[^1].ShouldBe("end"));
    }


    [Fact]
    public async Task OutputsGivenLanguages()
    {
        CreateRedactor(out var stream, 
            out var reader, 
            out var redactor, 
            fourUsage);

        await redactor.ApplyAsync();
        redactor.Dispose();
        
        var contents = reader.ReadToEnd();
        contents.ShouldSatisfyAllConditions(contents,
            () => contents.ShouldContain(fourUsage.ElementAt(0).Language),
            () => contents.ShouldContain(fourUsage.ElementAt(1).Language),
            () => contents.ShouldContain(fourUsage.ElementAt(2).Language),
            () => contents.ShouldContain(fourUsage.ElementAt(3).Language)
        );
    }

    [Fact]
    public async Task ShouldNotFreezeNorThrowOnMissingBlock()
    {
        CreateRedactor(out var stream, 
            out var reader, 
            out var redactor, 
            readmeBytes: Encoding.ASCII.GetBytes("""
                begin
                end
            """));

        Should.CompleteIn(redactor.ApplyAsync, TimeSpan.FromMilliseconds(500));
        redactor.Dispose();
        var content = reader.ReadToEnd();
        content.ShouldStartWith("""
            begin
            end
        """);
    }

    [Fact]
    public async Task OverridesPreviousData()
    {
        CreateRedactor(out var stream, 
            out var reader, 
            out var redactor);

        using (redactor)
        {
            await redactor.ApplyAsync();
            stream.Seek(0, SeekOrigin.Begin);
            await redactor.ApplyAsync();
        }
        
        var contents = reader.ReadToEnd();
        contents.Split(' ').ShouldContain(x => x.Contains("C#"), 1, contents);
    }
}