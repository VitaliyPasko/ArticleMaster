using System.Text.Json.Serialization;

namespace ArticleMaster.Application.Dto;

public class ArticleDto
{
    [JsonPropertyName("id")] public required Guid Id { get; set; }
    [JsonPropertyName("datePublished")] public required DateTime DatePublished { get; set; }
    [JsonPropertyName("downloadedFrom")] public required string DownloadedFrom { get; set; } = null!;
    [JsonPropertyName("title")] public required string Title { get; set; } = null!;
    [JsonPropertyName("content")] public required string Content { get; set; } = null!;
    [JsonPropertyName("author")] public required AuthorDto Author { get; set; }
}