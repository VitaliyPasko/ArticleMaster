using ArticleMaster.Domain.Entities;

namespace ArticleMaster.Application.Interfaces;

public interface IArticleRepository :  IRepository<Article>
{
    Task<IEnumerable<Article>> GetArticlesBetweenDates(DateTime? from, DateTime? to);
    Task<IEnumerable<Article>> GetTopTenArticlesByWord();
    Task<IEnumerable<Article>> GetArticlesByText(string text);
}