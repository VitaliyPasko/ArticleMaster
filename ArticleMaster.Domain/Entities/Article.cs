using System.Text.Json.Serialization;

namespace ArticleMaster.Domain.Entities;

public class Article
{
    [JsonPropertyName("id")] public int Id { get; set; }
    [JsonPropertyName("date_published")] public DateTime DatePublished { get; set; }
    [JsonPropertyName("downloaded_from")] public string DownloadedFrom { get; set; } = null!;
    [JsonPropertyName("title")] public string Title { get; set; } = null!;
    [JsonPropertyName("content")] public string Content { get; set; } = null!;
    [JsonPropertyName("author_id")] public int AuthorId { get; set; }
    [JsonPropertyName("author_name")] public string AuthorName { get; set; } = null!;
}