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

    public async Task<IEnumerable<Article>> GetArticlesBetweenDates(DateTime? from, DateTime? to)
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

    public async Task<IEnumerable<Article>> GetTopTenArticlesByWord()
    {
        var connection = OpenConnection();
        try
        {
            var result = await connection.QueryAsync<Article>("GetTopWordsInArticles",
                commandType: CommandType.StoredProcedure);
            return result;
        }
        finally
        {
            connection.Close();
        }
    }

    public async Task<IEnumerable<Article>> GetArticlesByText(string text)
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
        }
    }
}