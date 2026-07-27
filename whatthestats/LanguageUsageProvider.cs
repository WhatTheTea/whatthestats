using Octokit;
using whatthestats.Primitives;

namespace whatthestats;

public interface ILanguageUsageProvider
{
    Task<LanguagesUsage> GetByteWeightedAsync();
}

public sealed class LanguageUsageProvider(IGitHubClient github, Account user) : ILanguageUsageProvider
{
    public async Task<LanguagesUsage> GetByteWeightedAsync()
    {
        Dictionary<string, long> languageBytes = [];
        LanguagesUsage languagesUsage = [];
        var totalBytes = 0L;
        var repos = await github.Repository.GetAllForUser(user.Login);

        foreach (var repo in repos)
        {
            var languages = await github.Repository.GetAllLanguages(repo.Id);
            totalBytes += languages.Sum(x => x.NumberOfBytes);

            foreach (var language in languages)
            {
                if (!languageBytes.TryGetValue(language.Name, out _))
                {
                    languageBytes[language.Name] = 0;
                }

                languageBytes[language.Name] += language.NumberOfBytes;
            }
        }

        foreach (var language in languageBytes)
        {
            languagesUsage.Add(new(language.Key, (double)language.Value / totalBytes));
        }

        return languagesUsage;
    }
}