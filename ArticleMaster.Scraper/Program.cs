using System.Text;
using ArticleMaster.Scraper;
using ArticleMaster.Scraper.Contracts;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var databaseName = "MyNewDatabase";
var connectionString = "Server=localhost;Database=master;User Id=sa;Password=_strongPassword01;TrustServerCertificate=true;";

await using (var connection = new SqlConnection(connectionString))
{

    connection.Open();

    const string checkDatabaseQuery = "SELECT COUNT(*) FROM sys.databases WHERE name = @databaseName";
    int count = connection.QueryFirstOrDefault<int>(checkDatabaseQuery, new { databaseName });

    if (count == 0)
    {
        var queryStringBuilder = new StringBuilder($"CREATE DATABASE {databaseName};");
        // queryStringBuilder.Append("");
        connection.Execute(queryStringBuilder.ToString());
        Console.WriteLine($"База данных {databaseName} успешно создана.");
    }
    else
        Console.WriteLine($"База данных {databaseName} уже существует.");
}
var services = new ServiceCollection();
services.AddScoped<IParentParser, ParentParser>();
services.AddScoped<IUrlBuilder, UrlBuilder>();
services.AddScoped<IPageRecipient, PageRecipient>();
services.AddHttpClient();
ServiceProvider serviceProvider = services.BuildServiceProvider();

var parentParser = serviceProvider.GetService<IParentParser>()!;
var urlBuilder = serviceProvider.GetService<IUrlBuilder>()!;
var configuration = serviceProvider.GetService<IConfiguration>()!;
var articleNumbers = await parentParser.GetArticleNumbersAsync();
var childUrls = articleNumbers.Select(n => urlBuilder.BuildUrl(
    domain: new Domain(configuration["UrlParts:Domain"]!),
    lang: new Lang(configuration["UrlParts:Lang"]!),
    entity: new Entity(configuration["UrlParts:EntityNames"]!),
    n));




