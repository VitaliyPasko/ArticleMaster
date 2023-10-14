using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using ArticleMaster.Domain;
using ArticleMaster.Scraper.Contracts;
using Microsoft.Extensions.Configuration;

namespace ArticleMaster.Scraper;

public class ParentParser : IParentParser
{
    private readonly IConfiguration _configuration;
    private readonly IUrlBuilder _urlBuilder;
    private readonly IPageRecipient _pageRecipient;

    public ParentParser( 
        IConfiguration configuration,
        IUrlBuilder urlBuilder,
        IPageRecipient pageRecipient) => 
        (_configuration, _urlBuilder, _pageRecipient) = 
        (configuration, urlBuilder, pageRecipient);

    public async Task<IEnumerable<string>> GetArticleNumbersAsync()
    {
        var tasks = new List<Task<string>>();
        Parallel.ForEach(GetParentUrlCollection(), u =>
        {
            tasks.Add(_pageRecipient.PullPageAsync(u.UrlString));
        });

        await Task.WhenAll(tasks);

        return GetNumberOfArticleCollection(tasks);
    }

    private IEnumerable<Url> GetParentUrlCollection()
    {
        for (int i = 1; i <= int.Parse(_configuration.GetSection("PageOfArticlesCount").Value?? "2"); i++)
            yield return _urlBuilder.BuildUrl(
                domain: new Domain(_configuration["UrlParts:Domain"]!), 
                lang: new Lang(_configuration["UrlParts:Lang"]!), 
                entity: new Entity(_configuration["UrlParts:EntityNames"]!), 
                "page" + i);
    }

    private IEnumerable<string> GetNumberOfArticleCollection(IEnumerable<Task<string>> completedTasks)
    {
        var subUrls = new ConcurrentDictionary<string, string>();

        Parallel.ForEach(completedTasks, completedTask =>
        {
            var pattern = _configuration.GetSection("ChildUrlsPatternMatching").Value ?? @"/articles/(\d+)/";
            if (completedTask.IsCompletedSuccessfully)
            {
                if (string.IsNullOrEmpty(completedTask.Result))
                    return;
                MatchCollection matches = Regex.Matches(completedTask.Result, pattern);
                Parallel.ForEach(matches, match =>
                {
                    string result = match.Groups[1].Value;
                    subUrls.TryAdd(result, string.Empty);
                });
            }
            else
                Console.WriteLine("Запрос не удался: {0}", completedTask.Exception);
        });
        
        return subUrls.Select(x => x.Key);
    }
}