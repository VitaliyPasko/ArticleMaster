using ArticleMaster.Scraper.Domain.Objects;

namespace ArticleMaster.Scraper.Infrastructure;

public class ArticleRepository : DbConnection
{
    protected ArticleRepository(string connectionString) : base(connectionString) { }
    public async Task Create(Article entity)
    {
        
    }
}