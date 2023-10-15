using System.Text;
using ArticleMaster.Scraper;
using ArticleMaster.Scraper.Contracts;
using ArticleMaster.Scraper.Domain;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var databaseName = "articledb";
var connectionString = $"Server=localhost;Database=master;User Id=sa;Password=_strongPassword01;TrustServerCertificate=true;";

await using (var connection = new SqlConnection(connectionString))
{

    connection.Open();

    const string checkDatabaseQuery = "SELECT COUNT(*) FROM sys.databases WHERE name = @databaseName";
    int count = connection.QueryFirstOrDefault<int>(checkDatabaseQuery, new { databaseName });
    var queryStringBuilder = new StringBuilder();
    if (count == 0)
    {
        queryStringBuilder.Append($"CREATE DATABASE {databaseName};");
        connection.Execute(queryStringBuilder.ToString());
        Console.WriteLine($"База данных {databaseName} успешно создана.");
    }
    else
        Console.WriteLine($"База данных {databaseName} уже существует.");

    queryStringBuilder.Clear();
    queryStringBuilder.Append($"""
                               USE {databaseName};
                               IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'authors')
                               BEGIN
                                   CREATE TABLE authors (
                                       id INT PRIMARY KEY,
                                       author NVARCHAR(255),
                                       user_rating DECIMAL(5, 2)
                                   );
                               END;

                               IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'articles')
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
                               END;

                               """);
    connection.Execute(queryStringBuilder.ToString());
}
var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
    .Build();
var services = new ServiceCollection();
services.AddScoped<IParentParser, ParentParser>();
services.AddScoped<IChildParser, ChildParser>();
services.AddScoped<IUrlBuilder, UrlBuilder>();
services.AddScoped<IPageRecipient, PageRecipient>();
services.AddScoped<ArticleFieldsInitializer>();
services.AddHttpClient();
services.AddSingleton<IConfiguration>(configuration);
ServiceProvider serviceProvider = services.BuildServiceProvider();

var parentParser = serviceProvider.GetService<IParentParser>()!;
var childParser = serviceProvider.GetService<IChildParser>()!;
var urlBuilder = serviceProvider.GetService<IUrlBuilder>()!;
var articleFieldsInitializer = serviceProvider.GetService<ArticleFieldsInitializer>()!;
var articleNumbers = await parentParser.GetArticleNumbersAsync();

var childUrls = articleNumbers.Select(number => urlBuilder.BuildUrl(
    urlDomain: new UrlDomain(configuration["UrlParts:Domain"]!),
    urlLang: new UrlLang(configuration["UrlParts:Lang"]!),
    urlEntity: new UrlEntity(configuration["UrlParts:EntityNames"]!),
    number)).ToList();

List<Article> articles = await childParser.ParSeProcessAsync(childUrls);
//
articles.ForEach(articleFieldsInitializer.SetTitle);





