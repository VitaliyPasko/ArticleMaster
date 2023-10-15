using System.Text.Json.Serialization;

namespace ArticleMaster.Scraper.Domain.Models;

public class ArticleModel
{
    [JsonPropertyName("id")] public Guid Id { get; set; }
    [JsonPropertyName("author_id")] public required Guid AuthorId { get; set; }
    [JsonPropertyName("date_published")] public required DateTime DatePublished { get; set; }
    [JsonPropertyName("downloaded_from")] public required string DownloadedFrom { get; set; }
    [JsonPropertyName("title")] public required string Title { get; set; }
    [JsonPropertyName("content")] public required string Content { get; set; }
}