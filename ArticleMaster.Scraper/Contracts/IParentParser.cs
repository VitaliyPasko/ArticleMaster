namespace ArticleMaster.Scraper.Contracts;

public interface IParentParser
{
    Task<IEnumerable<string>> GetArticleNumbersAsync();
}