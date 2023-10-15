namespace ArticleMaster.Scraper.Domain.Objects;

public class Article
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Author? Author { get; } = new();
    public DateTime? DatePublished { get; set; }
    public required string DownloadedFrom { get; init; }
    public string? Title { get; set; }
    public string? Content { get; set; }

    public override string ToString()
    {
        return $"{Author?.Name} {DatePublished} {DownloadedFrom} {Title}";
    }
}