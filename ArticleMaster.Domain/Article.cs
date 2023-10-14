namespace ArticleMaster.Domain;

public class Article
{
    public string Author { get; set; } = null!;
    public DateTime DatePublished { get; set; }
    public Url DownloadedFrom { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string Content { get; set; } = null!;
    public decimal UserRating { get; set; }
}