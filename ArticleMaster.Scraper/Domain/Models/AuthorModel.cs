
namespace ArticleMaster.Scraper.Domain.Models;

public class AuthorModel
{
    public Guid Id { get; set; }
    public required string Name { get; set; } = null!;
}