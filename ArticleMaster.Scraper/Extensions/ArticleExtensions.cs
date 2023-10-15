using ArticleMaster.Scraper.Domain.Models;
using ArticleMaster.Scraper.Domain.Objects;

namespace ArticleMaster.Scraper.Extensions;

public static class ArticleExtensions
{
    public static ArticleModel MapToArticleModel(this Article self)
    {
        return new ArticleModel
        {
            // Id = self.Id,
            Title = self.Title ?? string.Empty,
            Content = self.Content  ?? string.Empty,
            DatePublished = self.DatePublished!.Value ,
            DownloadedFrom = self.DownloadedFrom,
            AuthorId = self.Author!.Id
        };
    }
    
    public static AuthorModel ExtractAuthorModel(this Article self)
    {
        return new AuthorModel
        {
            // Id = self.Author!.Id,
            Name = self.Author.Name
        };
    }
}