namespace ArticleMaster.Domain.Entities;

public class Article
{
    public Guid Id { get; set; }
    public DateTime DatePublished { get; set; }
    public string DownloadedFrom { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string Content { get; set; } = null!;
    public Guid AuthorId { get; set; }
    public string AuthorName { get; set; } = null!;
}