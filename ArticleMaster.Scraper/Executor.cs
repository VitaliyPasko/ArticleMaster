using ArticleMaster.Scraper.Contracts;
using ArticleMaster.Scraper.Domain;
using ArticleMaster.Scraper.Domain.Objects;
using Microsoft.Extensions.Configuration;

namespace ArticleMaster.Scraper;

public class Executor
{
    private readonly IParentParser _parentParser;
    private readonly IUrlBuilder _urlBuilder;
    private readonly IConfiguration _configuration;
    private readonly IChildParser _childParser;
    private readonly ArticleFieldsInitializer _articleFieldsInitializer;

    public Executor(
        IParentParser parentParser, 
        IUrlBuilder urlBuilder, 
        IConfiguration configuration, 
        IChildParser childParser, 
        ArticleFieldsInitializer articleFieldsInitializer)
    {
        _parentParser = parentParser;
        _urlBuilder = urlBuilder;
        _configuration = configuration;
        _childParser = childParser;
        _articleFieldsInitializer = articleFieldsInitializer;
    }

    public async Task Do()
    {
        var articleNumbers = await _parentParser.GetArticleNumbersAsync();
        var childUrls = articleNumbers.Select(number => _urlBuilder.BuildUrl(
            urlDomain: new UrlDomain(_configuration["UrlParts:Domain"]!),
            urlLang: new UrlLang(_configuration["UrlParts:Lang"]!),
            urlEntity: new UrlEntity(_configuration["UrlParts:EntityNames"]!),
            number)).ToList();

        List<Article> articles = await _childParser.ParSeProcessAsync(childUrls);
        Parallel.ForEach(articles, article => _articleFieldsInitializer.SetTitle(article));
        Parallel.ForEach(articles, article => _articleFieldsInitializer.SetDatePublished(article));
        Parallel.ForEach(articles, article => _articleFieldsInitializer.SetAuthorName(article));
    }
}