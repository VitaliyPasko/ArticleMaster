namespace ArticleMaster.Scraper.Domain.Objects;

public record Author
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
}