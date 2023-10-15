using System.Text;
using Dapper;
using Microsoft.Extensions.Configuration;

namespace ArticleMaster.Scraper.Infrastructure;

public class DbInitializer : DbConnection
{
    private readonly IConfiguration _configuration;
    private readonly string DbName;
    public DbInitializer(string connectionString, IConfiguration configuration) : base(connectionString)
    {
        _configuration = configuration;
        DbName = _configuration.GetSection("DbName").Value ?? string.Empty;
    }

    public async Task CreateDatabase()
    {
        var connection = OpenConnection();
        try
        {
            var databaseName = _configuration.GetSection("DbName").Value;
            const string checkDatabaseQuery = "SELECT COUNT(*) FROM sys.databases WHERE name = @databaseName";
            int count = connection.QueryFirstOrDefault<int>(checkDatabaseQuery, new { databaseName });
            var queryStringBuilder = new StringBuilder();
            if (count == 0)
            {
                queryStringBuilder.Append($"CREATE DATABASE {databaseName};");
                await connection.ExecuteAsync(queryStringBuilder.ToString());
                Console.WriteLine($"База данных {databaseName} успешно создана.");
            }
            else
                Console.WriteLine($"База данных {databaseName} уже существует.");
        }
        catch (Exception e)
        {
            Console.WriteLine(e.StackTrace);
            Console.WriteLine(e.Message);
            throw;
        }
        finally
        {
            connection.Close();
        }
    }

    public async Task CreateTables()
    {
        var queryStringBuilder = new StringBuilder();
        var connection = OpenConnection();
        try
        {
            queryStringBuilder.Append($"""
                                       USE {DbName};
                                       IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'authors')
                                       BEGIN
                                           EXEC('
                                           BEGIN
                                               CREATE TABLE authors (
                                                   id INT PRIMARY KEY,
                                                   author_name NVARCHAR(255)
                                               );
                                           END;
                                       
                                           IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = ''articles'')
                                           BEGIN
                                               CREATE TABLE articles (
                                                   id INT PRIMARY KEY,
                                                   title NVARCHAR(255),
                                                   author_id INT,
                                                   downloaded_from NVARCHAR(255),
                                                   content NVARCHAR(max),
                                                   date_published DATETIME,
                                                   FOREIGN KEY (author_id) REFERENCES authors(id)
                                               );
                                           END;');
                                       END;
                                       
                                       """);
            await connection.ExecuteAsync(queryStringBuilder.ToString());
        }
        catch (Exception e)
        {
            Console.WriteLine(e.StackTrace);
            Console.WriteLine(e.Message);
            throw;
        }
        finally
        {
            connection.Close();
        }
    }

    public async Task CreateGetArticlesByDateRangeProcedure()
    {
        var connection = OpenConnection();
        try
        {
            var queryStringBuilder = new StringBuilder($"""
                                                        USE {DbName};
                                                        IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'GetArticlesByDateRange')
                                                        BEGIN
                                                             EXEC('
                                                                CREATE PROCEDURE GetArticlesByDateRange
                                                                    @From DATETIME,
                                                                    @To DATETIME
                                                                AS
                                                                BEGIN
                                                                    SELECT
                                                                        A.id,
                                                                        A.title,
                                                                        A.content,
                                                                        A.downloaded_from,
                                                                        A.date_published,
                                                                        Au.id AS author_id,
                                                                        Au.author_name
                                                                    FROM
                                                                        articles A
                                                                    INNER JOIN
                                                                        authors Au ON A.author_Id = Au.Id
                                                                    WHERE
                                                                        A.date_published BETWEEN @From AND @To;
                                                             END;');
                                                        END;
                                                        """);
            await connection.ExecuteAsync(queryStringBuilder.ToString());
        }
        catch (Exception e)
        {
            Console.WriteLine(e.StackTrace);
            Console.WriteLine(e.Message);
            throw;
        }
        finally
        {
            connection.Close();
        }
    }
    
    public async Task CreateGetTopArticlesByKeywordProcedure()
    {
        var connection = OpenConnection();
        try
        {
            var queryStringBuilder = new StringBuilder($"""
                                                        USE {DbName};
                                                        IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'GetArticlesByDateRange')
                                                        BEGIN 
                                                            EXEC('
                                                            CREATE PROCEDURE GetTopArticlesByKeyword
                                                                @Keyword NVARCHAR(255)
                                                            AS
                                                            BEGIN
                                                                SELECT TOP 10
                                                                    A.id,
                                                                    A.title,
                                                                    A.content,
                                                                    A.downloaded_from,
                                                                    A.date_published,
                                                                    Au.id AS author_id,
                                                                    Au.author_name
                                                                FROM
                                                                    articles A
                                                                INNER JOIN
                                                                    authors Au ON A.author_id = Au.id
                                                                WHERE
                                                                    CONTAINS((A.Title, A.Content), @Keyword)
                                                                ORDER BY
                                                                    RANK() OVER(ORDER BY (SELECT NULL)) DESC;
                                                            END;');
                                                        END;
                                                        """);
            await connection.ExecuteAsync(queryStringBuilder.ToString());
        }
        catch (Exception e)
        {
            Console.WriteLine(e.StackTrace);
            Console.WriteLine(e.Message);
            throw;
        }
        finally
        {
            connection.Close();
        }
    }
    
    public async Task CreateSearchArticlesByTextProcedure()
    {
        var connection = OpenConnection();
        try
        {
            var queryStringBuilder = new StringBuilder($"""
                                                        USE {DbName};
                                                        IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'SearchArticlesByText')
                                                        BEGIN 
                                                            EXEC('
                                                                CREATE PROCEDURE SearchArticlesByText
                                                                    @Text NVARCHAR(MAX)
                                                                AS
                                                                BEGIN
                                                                    SELECT
                                                                        A.id,
                                                                        A.title,
                                                                        A.content,
                                                                        A.downloaded_from,
                                                                        A.date_published,
                                                                        Au.id AS author_id,
                                                                        Au.author_name
                                                                    FROM
                                                                        articles A
                                                                    INNER JOIN
                                                                        authors Au ON A.author_id = Au.id
                                                                    WHERE
                                                                        CONTAINS((A.title, A.content), @Text);
                                                                END;');
                                                        END;
                                                        """);
            await connection.ExecuteAsync(queryStringBuilder.ToString());
        }
        catch (Exception e)
        {
            Console.WriteLine(e.StackTrace);
            Console.WriteLine(e.Message);
            throw;
        }
        finally
        {
            connection.Close();
        }
    }
    
    public async Task CreateFullTextSearchCatalogs()
    {
        var connection = OpenConnection();
        try
        {
            var queryStringBuilder = new StringBuilder($"""
                                                        USE {DbName};
                                                        CREATE FULLTEXT CATALOG EnglishFullTextCatalog AS DEFAULT;
                                                        
                                                        CREATE FULLTEXT INDEX ON articles
                                                        (
                                                            Title Language 1033,
                                                            Content Language 1033
                                                        )
                                                        KEY INDEX PK_articles;
                                                        
                                                        CREATE FULLTEXT CATALOG RussianFullTextCatalog AS DEFAULT;
                                                        
                                                        CREATE FULLTEXT INDEX ON articles
                                                        (
                                                            Title Language 1049,
                                                            Content Language 1049
                                                        )
                                                        KEY INDEX PK_articles;
                                                        """);
            await connection.ExecuteAsync(queryStringBuilder.ToString());
        }
        catch (Exception e)
        {
            Console.WriteLine(e.StackTrace);
            Console.WriteLine(e.Message);
            throw;
        }
        finally
        {
            connection.Close();
        }
    }
}