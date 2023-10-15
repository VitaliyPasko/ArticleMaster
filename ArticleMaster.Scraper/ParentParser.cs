using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using ArticleMaster.Scraper.Contracts;
using ArticleMaster.Scraper.Domain;
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
        var pageInfos = await _pageRecipient.PullPagesAsync(GetParentUrlCollection());
        return GetNumberOfArticleCollection(pageInfos);
    }

    private IEnumerable<PageInfo> GetParentUrlCollection()
    {
        for (int i = 1; i <= int.Parse(_configuration.GetSection("PageOfArticlesCount").Value?? "2"); i++)
            yield return new PageInfo{ Url = _urlBuilder.BuildUrl(
                urlDomain: new UrlDomain(_configuration["UrlParts:Domain"]!), 
                urlLang: new UrlLang(_configuration["UrlParts:Lang"]!), 
                urlEntity: new UrlEntity(_configuration["UrlParts:EntityNames"]!), 
                "page" + i)};
    }
    
    private IEnumerable<string> GetNumberOfArticleCollection(IEnumerable<PageInfo> pageInfos)
    {
        var subUrls = new ConcurrentDictionary<string, string>();

        Parallel.ForEach(pageInfos, pageInfo =>
        {
            var pattern = _configuration.GetSection("ChildUrlsPatternMatching").Value ?? @"/articles/(\d+)/";
            {
                MatchCollection matches = Regex.Matches(pageInfo.HtmlPage?.ToString() ?? string.Empty, pattern);
                Parallel.ForEach(matches, match =>
                {
                    string result = match.Groups[1].Value;
                    subUrls.TryAdd(result, string.Empty);
                });
            }
        });
        
        return subUrls.Select(x => x.Key);
    }
}