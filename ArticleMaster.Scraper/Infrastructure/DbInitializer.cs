using System.Text;
using Dapper;
using Microsoft.Extensions.Configuration;

namespace ArticleMaster.Scraper.Infrastructure;

public class DbInitializer : DbConnection
{
    private readonly IConfiguration _configuration;
    private readonly string _dbName;
    public DbInitializer(string connectionString, IConfiguration configuration) : base(connectionString)
    {
        _configuration = configuration;
        _dbName = _configuration.GetSection("DbName").Value ?? string.Empty;
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
            connection.Dispose();
        }
    }

    public async Task CreateTables()
    {
        var queryStringBuilder = new StringBuilder();
        var connection = OpenConnection();
        try
        {
            queryStringBuilder.Append($"""
                                       USE {_dbName};
                                       IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'authors')
                                       BEGIN
                                           EXEC('
                                           BEGIN
                                               CREATE TABLE authors (
                                                   Id NVARCHAR(40) PRIMARY KEY,
                                                   AuthorName NVARCHAR(255)
                                               );
                                           END;
                                       
                                           IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = ''articles'')
                                           BEGIN
                                               CREATE TABLE articles (
                                                   Id NVARCHAR(40) PRIMARY KEY,
                                                   Title NVARCHAR(255),
                                                   AuthorId NVARCHAR(40) ,
                                                   DownloadedFrom NVARCHAR(255),
                                                   Content NVARCHAR(max),
                                                   DatePublished DATETIME,
                                                   FOREIGN KEY (AuthorId) REFERENCES authors(Id)
                                               );
                                           END;')
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
            connection.Dispose();
        }
    }

    public async Task CreateGetArticlesByDateRangeProcedure()
    {
        var connection = OpenConnection();
        try
        {
            var queryStringBuilder = new StringBuilder($"""
                                                        USE {_dbName};
                                                        IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'GetArticlesByDateRange')
                                                        BEGIN
                                                             EXEC('
                                                                CREATE PROCEDURE GetArticlesByDateRange
                                                                @From DATETIME = NULL,
                                                                @To DATETIME = NULL
                                                            AS
                                                            BEGIN
                                                                SELECT
                                                                    A.Id,
                                                                    A.Title,
                                                                    A.Content,
                                                                    A.DownloadedFrom,
                                                                    A.DatePublished,
                                                                    Au.Id AS AuthorId,
                                                                    Au.AuthorName
                                                                FROM
                                                                    articles A
                                                                INNER JOIN
                                                                    authors Au ON A.AuthorId = Au.Id
                                                                WHERE
                                                                    (@From IS NULL OR A.DatePublished >= @From)
                                                                    AND (@To IS NULL OR A.DatePublished <= @To);
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
            connection.Dispose();
        }
    }
    
    public async Task CreateGetTopArticlesByKeywordProcedure()
    {
        var connection = OpenConnection();
        try
        {
            var queryStringBuilder = new StringBuilder($"""
                                                        USE {_dbName};
                                                        IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'GetTopWordsInArticles')
                                                        BEGIN
                                                            EXEC('
                                                           CREATE PROCEDURE GetTopWordsInArticles
                                                        AS
                                                        BEGIN
                                                            DECLARE @Keywords TABLE (Keyword NVARCHAR(max))
                                                        
                                                            INSERT INTO @Keywords (Keyword)
                                                            SELECT value
                                                            FROM string_split(
                                                                (SELECT STRING_AGG(content, '' '') FROM articles), '' ''
                                                            )
                                                        
                                                            SELECT TOP 10 Keyword, COUNT(*) AS Frequency
                                                            FROM @Keywords
                                                            WHERE LEN(Keyword) > 3
                                                            GROUP BY Keyword
                                                            ORDER BY Frequency DESC
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
            connection.Dispose();
        }
    }
    
    [Obsolete("для  nvarchar(max) невозможно добавить индекс")]
    public async Task CreateIndexes()
    {
        var connection = OpenConnection();
        try
        {
            var queryStringBuilder = new StringBuilder($"""
                                                        USE {_dbName};
                                                        IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Title' AND object_id = OBJECT_ID('articles'))
                                                        BEGIN
                                                            CREATE NONCLUSTERED INDEX IX_Title ON articles (Title);
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
            connection.Dispose();
        }
    }
    public async Task CreateSearchArticlesByTextProcedure()
    {
        var connection = OpenConnection();
        try
        {
            var queryStringBuilder = new StringBuilder($"""
                                                        USE {_dbName};
                                                        IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'SearchArticlesByText')
                                                        BEGIN 
                                                            EXEC('
                                                                   CREATE PROCEDURE SearchArticlesByText
                                                            @Text NVARCHAR(MAX)
                                                        AS
                                                        BEGIN
                                                            SELECT
                                                                A.Id,
                                                                A.Title,
                                                                A.Content,
                                                                A.DownloadedFrom,
                                                                A.DatePublished,
                                                                Au.Id AS AuthorId,
                                                                Au.AuthorName
                                                            FROM
                                                                articles A
                                                            INNER JOIN
                                                                authors Au ON A.AuthorId = Au.Id
                                                            WHERE
                                                                A.title LIKE ''%'' + @Text + ''%'' OR A.Content LIKE ''%'' + @Text + ''%'';
                                                        END;
                                                        ');
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
            connection.Dispose();
        }
    }
    public async Task CreateTableTypesAsync()
    {
        var connection = OpenConnection();
        try
        {
            var checkArticleTypeQuery = $@"
                                    USE {_dbName};
                                    IF NOT EXISTS (SELECT * FROM sys.types WHERE name = 'ArticleType' AND is_table_type = 1)
                                    CREATE TYPE ArticleType AS TABLE
                                    (
                                        Id UNIQUEIDENTIFIER ,
                                        AuthorId UNIQUEIDENTIFIER ,
                                        DatePublished DATETIME,
                                        DownloadedFrom NVARCHAR(255),
                                        Title NVARCHAR(255),
                                        Content NVARCHAR(MAX)
                                    );";
                                
                                    var checkAuthorTypeQuery = $@"
                                    USE {_dbName};
                                    IF NOT EXISTS (SELECT * FROM sys.types WHERE name = 'AuthorType' AND is_table_type = 1)
                                    CREATE TYPE AuthorType AS TABLE
                                    (
                                        Id UNIQUEIDENTIFIER ,
                                        AuthorName NVARCHAR(255)
                                    );";

            await connection.ExecuteAsync(checkArticleTypeQuery);
            await connection.ExecuteAsync(checkAuthorTypeQuery);
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
            connection.Dispose();
        }
    }
    
    public async Task CreateInsertArticlesAndAuthorsProcedure()
    {
        var connection = OpenConnection();
        try
        {
            var queryStringBuilder = new StringBuilder($"""
                                USE {_dbName};
                                IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'InsertArticlesAndAuthors')
                                BEGIN
                                    EXEC('
                                    CREATE PROCEDURE InsertArticlesAndAuthors
                                    @Articles ArticleType READONLY,
                                    @Authors AuthorType READONLY
                                AS
                                BEGIN
                                    INSERT INTO authors (Id, AuthorName)
                                    SELECT a.Id, a.AuthorName
                                    FROM @Authors a;
                                
                                    INSERT INTO articles (Id, AuthorId, DatePublished, DownloadedFrom, Title, Content)
                                    SELECT
                                        a.Id,
                                        a.AuthorId,
                                        a.DatePublished,
                                        a.DownloadedFrom,
                                        a.Title,
                                        a.Content
                                    FROM @Articles a;
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
            connection.Dispose();
        }
    }
}