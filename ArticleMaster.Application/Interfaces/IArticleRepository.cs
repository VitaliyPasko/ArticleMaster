using ArticleMaster.Domain.Entities;

namespace ArticleMaster.Application.Interfaces;

public interface IArticleRepository :  IRepository<Article>
{
    Task<IEnumerable<Article>> GetArticlesBetweenDatesAsync(DateTime? from, DateTime? to);
    Task<IEnumerable<Article>> GetTopTenAsync();
    Task<IEnumerable<Article>> GetArticlesByTextAsync(string text);
}