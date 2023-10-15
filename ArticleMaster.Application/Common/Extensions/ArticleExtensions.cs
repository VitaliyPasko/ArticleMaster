using ArticleMaster.Application.Dto;
using ArticleMaster.Domain.Entities;

namespace ArticleMaster.Application.Common.Extensions;

public static class ArticleExtensions
{
    public static IEnumerable<ArticleDto> MapToArticleDto(this IEnumerable<Article> models)
    {
        return models.Select(self => new ArticleDto
        {
            Id = self.Id,
            Title = self.Title,
            Content = self.Content,
            DatePublished = self.DatePublished,
            DownloadedFrom = self.DownloadedFrom,
            Author = new AuthorDto
            {
                AuthorName = self.AuthorName,
                Id = self.AuthorId
            }
        });
    }
}