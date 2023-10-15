namespace ArticleMaster.Scraper.Domain.Objects;

public class Article
{
    public int Id { get; set; }
    public Author? Author { get; } = new();
    public DateTime? DatePublished { get; set; }
    public required string DownloadedFrom { get; init; }
    public string? Title { get; set; }
    public string? Content { get; init; }

    public override string ToString()
    {
        return $"{Author?.Name} {DatePublished} {DownloadedFrom} {Title}";
    }
}