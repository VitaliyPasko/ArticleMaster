
namespace ArticleMaster.Scraper.Domain.Models;

public class ArticleModel
{
    public Guid Id { get; set; }
    public required Guid AuthorId { get; set; }
    public required DateTime DatePublished { get; set; }
    public required string DownloadedFrom { get; set; }
    public required string Title { get; set; }
    public required string Content { get; set; }
}