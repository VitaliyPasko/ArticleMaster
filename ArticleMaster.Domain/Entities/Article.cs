namespace ArticleMaster.Domain.Entities;

public class Article
{
    public string Id { get; set; }
    public DateTime DatePublished { get; set; }
    public string DownloadedFrom { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string Content { get; set; } = null!;
    public string AuthorId { get; set; }
    public string AuthorName { get; set; } = null!;
}