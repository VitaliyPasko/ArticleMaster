using System.Text.Json.Serialization;

namespace ArticleMaster.Application.Dto;

public class AuthorDto
{
    [JsonPropertyName("id")] public required int Id { get; set; }
    [JsonPropertyName("authorName")] public required string AuthorName { get; set; } = null!;
}