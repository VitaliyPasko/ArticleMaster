using ArticleMaster.Scraper.Contracts;
using ArticleMaster.Scraper.Domain;
using Article = ArticleMaster.Scraper.Domain.Article;

namespace ArticleMaster.Scraper;

public class ChildParser : IChildParser
{
    private readonly IPageRecipient _pageRecipient;

    public ChildParser(IPageRecipient pageRecipient)
    {
        _pageRecipient = pageRecipient;
    }

    public async Task<List<Article>> ParSeProcessAsync(IEnumerable<Url> articleUrls)
    {
        var pageInfos = await _pageRecipient
            .PullPagesAsync(articleUrls
                .Select(url => new PageInfo { Url = url }
                ).ToList());
        return BuildArticles(pageInfos);
    }
    
    public List<Article> BuildArticles(IEnumerable<PageInfo> pageInfos)
    {
        return pageInfos.Select(pageInfo => new Article
        {
            DownloadedFrom = pageInfo.Url.ToString(),
            Content = pageInfo.HtmlPage?.ToString()
        }).ToList();
    }
}