namespace ArticleMaster.Domain;

public class Article
{
    public int Id { get; set; }
    public string AuthorId { get; set; } = null!;
    public DateTime DatePublished { get; set; }
    public string DownloadedFrom { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string Content { get; set; } = null!;
}

public class Author
{
    public int Id { get; set; }
    public string AuthorName { get; set; } = null!;
}