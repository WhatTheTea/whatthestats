using NSubstitute;
using Octokit;
using Shouldly;

namespace whatthestats.tests;

public class LanguageUsageTests
{
    private readonly Account user = Substitute.For<Account>(); 
    private readonly IGitHubClient github = Substitute.For<IGitHubClient>();

    [Fact]
    public async Task UsagePercentagesCorrectOneRepo()
    {
        const long repoId = 123;
        github.Repository.GetAllForUser(Arg.Any<string>())
            .Returns([new Repository(repoId)]);
        github.Repository.GetAllLanguages(Arg.Is(repoId))
            .Returns([new("C#", 42), new("C++", 42), new("Holy C", 42), new("C", 42)]);
        var statisticsProvider = new LanguageUsageProvider(github, user);

        var statistics = await statisticsProvider.GetByteWeightedAsync();

        statistics.Values.ShouldNotBeEmpty();
        statistics.Values.ShouldAllBe(x => x.Usage == 0.25);
    }

    [Fact]
    public async Task UsagePercentagesCorrectTwoRepos()
    {
        const long firstRepoId = 123;
        const long secondRepoId = 234;

        github.Repository.GetAllForUser(Arg.Any<string>())
            .Returns([new Repository(firstRepoId), new Repository(secondRepoId)]);
        github.Repository.GetAllLanguages(Arg.Is(firstRepoId))
            .Returns([new("C#", 42), new("C++", 42)]);
        github.Repository.GetAllLanguages(Arg.Is(secondRepoId))
            .Returns([new("C#", 42), new("C++", 42)]);
        var statisticsProvider = new LanguageUsageProvider(github, user);

        var statistics = await statisticsProvider.GetByteWeightedAsync();

        statistics.Values.ShouldNotBeEmpty();
        statistics.Values.ShouldAllBe(x => x.Usage == 0.5);
    }
}
