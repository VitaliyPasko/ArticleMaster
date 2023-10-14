namespace ArticleMaster.Scraper;

public interface IPageRecipient
{
    Task<string> PullPageAsync(string url);
}