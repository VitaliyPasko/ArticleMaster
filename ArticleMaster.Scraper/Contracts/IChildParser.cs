using ArticleMaster.Scraper.Domain.Objects;

namespace ArticleMaster.Scraper.Contracts;

public interface IChildParser
{
    Task<List<Article>> ParseProcessAsync(IEnumerable<Url> articleUrls);
}