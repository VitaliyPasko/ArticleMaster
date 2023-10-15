using System.Data;
using ArticleMaster.Application.Interfaces;
using ArticleMaster.Domain.Entities;
using Dapper;

namespace ArticleMaster.Persistence.Repositories;

public class ArticleRepository : MsSqlDbConnectionProvider, IArticleRepository
{
    public ArticleRepository(string connectionString) : base(connectionString)
    {
        
    }

    public async Task<IEnumerable<Article>> GetArticlesBetweenDatesAsync(DateTime? from, DateTime? to)
    {
        var connection = OpenConnection();
        try
        {
            var parameters = new DynamicParameters();
            parameters.Add("@From", from);
            parameters.Add("@To", to);

            var result = await connection.QueryAsync<Article>("GetArticlesByDateRange", parameters,
                commandType: CommandType.StoredProcedure);
            return result;
        }
        finally
        {
            connection.Close();
        }
    }

    public async Task<IEnumerable<Article>> GetArticlesByTextAsync(string text)
    {
        var connection = OpenConnection();
        try
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Text", text);

            var result = await connection.QueryAsync<Article>("SearchArticlesByText", parameters,
                commandType: CommandType.StoredProcedure);
            return result;
        }
        finally
        {
            connection.Close();
            connection.Dispose();
        }
    }

    public async Task<IEnumerable<string>> GetTopTenWordsAsync()
    {
        var connection = OpenConnection();
        try
        {
            var result = await connection.QueryAsync<string>("GetTopWordsInArticles",
                commandType: CommandType.StoredProcedure);
            return result;
        }
        finally
        {
            connection.Close();
            connection.Dispose();
        }
    }
}