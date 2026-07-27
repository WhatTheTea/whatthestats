namespace whatthestats.ReadmeRedactors;

public abstract class ReadmeRedactor(Stream readmeStream) : IDisposable
{
    public abstract string RedactorAlias { get; }
    protected readonly StreamReader reader = new(readmeStream, leaveOpen: true);
    protected readonly StreamWriter writer = new(readmeStream, leaveOpen: true);
    private bool disposedValue;

    public abstract Task ApplyAsync();

    protected Range GetBlockRange(string readme)
    {
        string beginning = $"```{RedactorAlias}";
        var startIndex = readme.IndexOf(beginning);
        var start = startIndex > 0 ? startIndex + beginning.Length : readme.Length;
        var endIndex = readme[start..].IndexOf("```");
        var end = endIndex > 0 ? endIndex + start : readme.Length;

        return new Range(start, end);
    } 

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                reader.Dispose();
                writer.Dispose();
                readmeStream.Seek(0, SeekOrigin.Begin);
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            disposedValue = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}