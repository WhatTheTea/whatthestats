namespace whatthestats.Primitives;

public sealed class LanguagesUsage : Dictionary<string, LanguageUsage>, ICollection<LanguageUsage>
{
    public bool IsReadOnly { get; } = false;

    public void Add(LanguageUsage item) => this[item.Language] = item;

    public bool Contains(LanguageUsage item) => this.ContainsKey(item.Language);

    public void CopyTo(LanguageUsage[] array, int arrayIndex) => Values.CopyTo(array, arrayIndex);

    public bool Remove(LanguageUsage item) => this.Remove(item.Language);

    public LanguageUsage ElementAt(int index) => this.Values.ElementAt(index);

    IEnumerator<LanguageUsage> IEnumerable<LanguageUsage>.GetEnumerator() => this.Values.GetEnumerator();
}

public readonly struct LanguageUsage
{
    public string Language { get; } = string.Empty;
    public Percentage Usage { get; } = (Percentage)0;

    public LanguageUsage(string language, Percentage usage)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(language);
        ArgumentOutOfRangeException.ThrowIfLessThan((double)usage, 0);
        ArgumentOutOfRangeException.ThrowIfGreaterThan((double)usage, 1);

        Language = language;
        Usage = usage;
    }
}