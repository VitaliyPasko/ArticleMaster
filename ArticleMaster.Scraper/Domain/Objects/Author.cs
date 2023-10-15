namespace ArticleMaster.Scraper.Domain.Objects;

public record Author
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = null!;
}