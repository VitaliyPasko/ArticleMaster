using System.Text.Json.Serialization;

namespace ArticleMaster.Application.Dto;

public class ArticleDto
{
    [JsonPropertyName("id")] public int Id { get; set; }
    [JsonPropertyName("datePublished")] public DateTime DatePublished { get; set; }
    [JsonPropertyName("downloadedFrom")] public string DownloadedFrom { get; set; } = null!;
    [JsonPropertyName("title")] public string Title { get; set; } = null!;
    [JsonPropertyName("content")] public string Content { get; set; } = null!;
    [JsonPropertyName("authorId")] public int AuthorId { get; set; }
}