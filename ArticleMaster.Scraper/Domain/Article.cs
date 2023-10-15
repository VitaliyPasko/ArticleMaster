namespace ArticleMaster.Scraper.Domain;

public class Article
{
    public int Id { get; set; }
    public Author? Author { get; set; }
    public DateTime? DatePublished { get; set; }
    public required string DownloadedFrom { get; set; }
    public string? Title { get; set; }
    public string? Content { get; set; }
}