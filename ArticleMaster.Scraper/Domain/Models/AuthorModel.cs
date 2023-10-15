using System.Text.Json.Serialization;

namespace ArticleMaster.Scraper.Domain.Models;

public class AuthorModel
{
    [JsonPropertyName("id")] public int Id { get; set; }
    [JsonPropertyName("name")] public required string Name { get; set; } = null!;
}