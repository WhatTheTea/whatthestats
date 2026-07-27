using Octokit;
using whatthestats;
using whatthestats.ReadmeRedactors;

var github = new GitHubClient(new ProductHeaderValue("WhatTheStatus"));
var username = args.ElementAtOrDefault(0) ?? throw new ArgumentException("Missing username");
var workfile = args.ElementAtOrDefault(1) ?? "README.md";
var token = args.ElementAtOrDefault(2);

if (token is not null)
{
    github.Credentials = new(token);
}

var user = await github.User.Get(username);
var languageUsageProvider = new LanguageUsageProvider(github, user);
var languagesUsage = await languageUsageProvider.GetByteWeightedAsync();

using var readme = File.Open(workfile, System.IO.FileMode.Open);
using var redactor = new LanguageUsageRedactor(readme, languagesUsage);

await redactor.ApplyAsync();