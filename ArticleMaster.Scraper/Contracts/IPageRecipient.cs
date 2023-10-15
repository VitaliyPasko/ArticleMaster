namespace ArticleMaster.Scraper.Contracts;

public interface IPageRecipient
{
    Task<IEnumerable<PageInfo>> PullPagesAsync(IEnumerable<PageInfo> pageInfo);
}