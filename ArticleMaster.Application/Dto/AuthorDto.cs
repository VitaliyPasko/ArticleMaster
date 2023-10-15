using System.Text.Json.Serialization;

namespace ArticleMaster.Application.Dto;

public class AuthorDto
{
    [JsonPropertyName("id")] public int Id { get; set; }
    [JsonPropertyName("authorName")] public string AuthorName { get; set; } = null!;
}