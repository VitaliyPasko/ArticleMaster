using System.Data;
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
    public async Task CreateAll(List<Article> entities)
    {
        var connection = OpenConnection();

        IEnumerable<ArticleModel> articleModels = entities.Select(article => article.MapToArticleModel());
        IEnumerable<AuthorModel> authorModels = entities.Select(article => article.ExtractAuthorModel());

        var articleDataTable = new DataTable();
        // articleDataTable.Columns.Add("Id", typeof(int));
        articleDataTable.Columns.Add("AuthorId", typeof(int));
        articleDataTable.Columns.Add("DatePublished", typeof(DateTime));
        articleDataTable.Columns.Add("DownloadedFrom", typeof(string));
        articleDataTable.Columns.Add("Title", typeof(string));
        articleDataTable.Columns.Add("Content", typeof(string));

        foreach (var article in articleModels)
            articleDataTable.Rows.Add(
                article.Id, 
                article.AuthorId, 
                article.DatePublished, 
                article.DownloadedFrom, 
                article.Title, 
                article.Content);

        var authorDataTable = new DataTable();
        // authorDataTable.Columns.Add("Id", typeof(int));
        authorDataTable.Columns.Add("Name", typeof(string));

        foreach (var author in authorModels)
            authorDataTable.Rows.Add(author.Id, author.Name);

        var parameters = new DynamicParameters();
        parameters.Add("@Articles", articleDataTable.AsTableValuedParameter("ArticleType"));
        parameters.Add("@Authors", authorDataTable.AsTableValuedParameter("AuthorType"));

        await connection.ExecuteAsync(
            $"{_configuration.GetSection("DbName").Value}.dbo.InsertArticlesAndAuthors", 
            parameters, 
            commandType: CommandType.StoredProcedure);

    }
}