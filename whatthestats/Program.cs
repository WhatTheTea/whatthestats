using Octokit;
using whatthestats;

var github = new GitHubClient(new ProductHeaderValue("WhatTheStatus"));
var username = args.ElementAtOrDefault(0) ?? throw new ArgumentException("Missing username");
var userRepos = await github.Repository.GetAllForUser(username);
var user = await github.User.Get(username);
var statistics = new LanguageUsageProvider(github, user);

var languagesUsage = await statistics.GetByteWeightedAsync();

Console.WriteLine("Language statistics:");
foreach (var item in languagesUsage.Values.OrderByDescending(x => x.Usage).Take(10))
{
    Console.WriteLine($"{item.Language}: {item.Usage * 100:F1}%");
}