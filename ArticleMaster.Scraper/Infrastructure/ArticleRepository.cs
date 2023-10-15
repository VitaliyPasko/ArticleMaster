using ArticleMaster.Scraper.Domain.Models;
using ArticleMaster.Scraper.Domain.Objects;
using ArticleMaster.Scraper.Extensions;
using Dapper;
using Microsoft.Extensions.Configuration;

namespace ArticleMaster.Scraper.Infrastructure;

public class ArticleRepository : DbConnection
{
    private readonly IConfiguration _configuration;
    public ArticleRepository(string connectionString, IConfiguration configuration) : base(connectionString)
    {
        _configuration = configuration;
    }
    
    public async Task CreateAllAsync(List<Article> entities)
    {
        var connection = OpenConnection();
        try
        {
            IEnumerable<ArticleModel> articles = entities.Select(article => article.MapToArticleModel());
            IEnumerable<AuthorModel> authors = entities.Select(article => article.ExtractAuthorModel());
            await connection.ExecuteAsync($"USE {_configuration.GetSection("DbName").Value}; " +
                                          $"INSERT INTO authors (id, author_name) " +
                                          $"VALUES(@Id, @Name)", authors);
            await connection.ExecuteAsync($"USE {_configuration.GetSection("DbName").Value}; " +
                                          $"INSERT INTO articles (id, author_id, date_published, downloaded_from, title, content) " +
                                          $"VALUES(@Id, @AuthorId, @DatePublished, @DownloadedFrom, @Title, @Content)", articles);
        }
        finally
        {
            connection.Close();
            connection.Dispose();
        }
    }
}