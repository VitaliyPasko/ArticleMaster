using ArticleMaster.Application.Common.Extensions;
using ArticleMaster.Application.Dto;
using ArticleMaster.Application.Interfaces;
using ArticleMaster.Application.Interfaces.Services;

namespace ArticleMaster.Application.Implementations;

public class ArticleService : IArticleService
{
    private readonly IArticleRepository _articleRepository;

    public ArticleService(IArticleRepository articleRepository)
    {
        _articleRepository = articleRepository;
    }

    public async Task<IEnumerable<ArticleDto>> GetByDatesAsync(DateTime? from, DateTime? to)
    {
        var models = await _articleRepository.GetArticlesBetweenDatesAsync(from, to);
        return models.MapToArticleDto();
    }

    public async Task<IEnumerable<ArticleDto>> GetTopTenAsync()
    {
        var models = await _articleRepository.GetTopTenAsync();
        return models.MapToArticleDto();
    }

    public async Task<IEnumerable<ArticleDto>> GetArticlesByTextAsync(string text)
    {
        var models = await _articleRepository.GetArticlesByTextAsync(text);
        return models.MapToArticleDto();
    }
}