using ArticleMaster.Application.Dto;

namespace ArticleMaster.Application.Interfaces.Services;

public interface IArticleService
{
    Task<IEnumerable<ArticleDto>> GetByDatesAsync(DateTime? from, DateTime? to);
    Task<IEnumerable<ArticleDto>> GetTopTenAsync();
    Task<IEnumerable<ArticleDto>> GetArticlesByTextAsync(string text);
}