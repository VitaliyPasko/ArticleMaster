using System.Text;
using ArticleMaster.Scraper.Contracts;
using ArticleMaster.Scraper.Domain;
using ArticleMaster.Scraper.Domain.Objects;
using ArticleMaster.Scraper.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
    .Build();
var connectionString = configuration.GetConnectionString("MsSql");
var services = new ServiceCollection();
services.AddScoped<IParentParser, ParentParser>();
services.AddScoped<IChildParser, ChildParser>();
services.AddScoped<IUrlBuilder, UrlBuilder>();
services.AddScoped<IPageRecipient, PageRecipient>();
services.AddScoped<ArticleFieldsInitializer>();
services.AddScoped<ArticleRepository>();
services.AddScoped<DbInitializer>(_ => new DbInitializer(connectionString!, configuration));
services.AddHttpClient();
services.AddSingleton<IConfiguration>(configuration);
ServiceProvider serviceProvider = services.BuildServiceProvider();

var parentParser = serviceProvider.GetService<IParentParser>()!;
var childParser = serviceProvider.GetService<IChildParser>()!;
var urlBuilder = serviceProvider.GetService<IUrlBuilder>()!;
var articleFieldsInitializer = serviceProvider.GetService<ArticleFieldsInitializer>()!;
var dbInitializer = serviceProvider.GetService<DbInitializer>()!;
var articleNumbers = await parentParser.GetArticleNumbersAsync();

await dbInitializer.CreateDatabase();
await dbInitializer.CreateTables();
await dbInitializer.CreateFullTextSearchCatalogs();
await dbInitializer.CreateGetArticlesByDateRangeProcedure();
await dbInitializer.CreateGetTopArticlesByKeywordProcedure();
await dbInitializer.CreateSearchArticlesByTextProcedure();

var childUrls = articleNumbers.Select(number => urlBuilder.BuildUrl(
    urlDomain: new UrlDomain(configuration["UrlParts:Domain"]!),
    urlLang: new UrlLang(configuration["UrlParts:Lang"]!),
    urlEntity: new UrlEntity(configuration["UrlParts:EntityNames"]!),
    number)).ToList();

List<Article> articles = await childParser.ParSeProcessAsync(childUrls);
Parallel.ForEach(articles, article => articleFieldsInitializer.SetTitle(article));
Parallel.ForEach(articles, article => articleFieldsInitializer.SetDatePublished(article));
Parallel.ForEach(articles, article => articleFieldsInitializer.SetAuthorName(article));
Console.WriteLine(articles.First());




