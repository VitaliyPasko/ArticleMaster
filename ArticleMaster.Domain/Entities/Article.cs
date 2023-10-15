using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArticleMaster.Domain.Entities;

public class Article
{
    [Key][Column("id")] public Guid Id { get; set; }
    [Column("date_published")] public DateTime DatePublished { get; set; }
    [Column("downloaded_from")] public string DownloadedFrom { get; set; } = null!;
    [Column("title")] public string Title { get; set; } = null!;
    [Column("content")] public string Content { get; set; } = null!;
    [Column("author_id")] public Guid AuthorId { get; set; }
    [Column("author_name")] public string AuthorName { get; set; } = null!;
}