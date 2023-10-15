using ArticleMaster.Scraper.Domain;

namespace ArticleMaster.Scraper.Contracts;

public interface IChildParser
{
    Task<List<Article>> ParSeProcessAsync(IEnumerable<Url> articleUrls);
}