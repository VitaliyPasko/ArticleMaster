using ArticleMaster.Scraper.Contracts;
using ArticleMaster.Scraper.Domain;
using ArticleMaster.Scraper.Domain.Objects;
using ArticleMaster.Scraper.Infrastructure;
using Microsoft.Extensions.Configuration;

namespace ArticleMaster.Scraper;

public class Executor
{
    private readonly IParentParser _parentParser;
    private readonly IUrlBuilder _urlBuilder;
    private readonly IConfiguration _configuration;
    private readonly IChildParser _childParser;
    private readonly ArticleFieldsInitializer _articleFieldsInitializer;
    private readonly ArticleRepository _articleRepository;

    public Executor(
        IParentParser parentParser, 
        IUrlBuilder urlBuilder, 
        IConfiguration configuration, 
        IChildParser childParser, 
        ArticleFieldsInitializer articleFieldsInitializer, 
        ArticleRepository articleRepository)
    {
        _parentParser = parentParser;
        _urlBuilder = urlBuilder;
        _configuration = configuration;
        _childParser = childParser;
        _articleFieldsInitializer = articleFieldsInitializer;
        _articleRepository = articleRepository;
    }

    public async Task Do()
    {
        var articleNumbers = await _parentParser.GetArticleNumbersAsync();
        var childUrls = articleNumbers.Select(number => _urlBuilder.BuildUrl(
            urlDomain: new UrlDomain(_configuration["UrlParts:Domain"]!),
            urlLang: new UrlLang(_configuration["UrlParts:Lang"]!),
            urlEntity: new UrlEntity(_configuration["UrlParts:EntityNames"]!),
            number)).ToList();

        List<Article> articles = await _childParser.ParseProcessAsync(childUrls);
        Parallel.ForEach(articles, article => _articleFieldsInitializer.SetTitle(article));
        Parallel.ForEach(articles, article => _articleFieldsInitializer.SetDatePublished(article));
        Parallel.ForEach(articles, article => _articleFieldsInitializer.SetAuthorName(article));

        await _articleRepository.CreateAllAsync(articles);
    }
}