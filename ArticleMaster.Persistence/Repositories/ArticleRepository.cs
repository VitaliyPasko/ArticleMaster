using ArticleMaster.Application.Interfaces;
using ArticleMaster.Domain;

namespace ArticleMaster.Persistence.Repositories;

public class ArticleRepository : IRepository<Article>
{
    public async Task Create(Article entity)
    {
        
    }
}