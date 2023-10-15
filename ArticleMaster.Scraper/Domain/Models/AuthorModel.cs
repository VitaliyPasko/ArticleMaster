using System.Text.Json.Serialization;

namespace ArticleMaster.Scraper.Domain.Models;

public class AuthorModel
{
    [JsonPropertyName("id")] public Guid Id { get; set; }
    [JsonPropertyName("author_name")] public required string Name { get; set; } = null!;
}