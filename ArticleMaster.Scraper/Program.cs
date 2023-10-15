using ArticleMaster.Scraper;
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
services.AddScoped<Executor>();
ServiceProvider serviceProvider = services.BuildServiceProvider();

var dbInitializer = serviceProvider.GetService<DbInitializer>()!;
var executor = serviceProvider.GetService<Executor>()!;

await dbInitializer.CreateDatabase();
await dbInitializer.CreateTables();
await dbInitializer.CreateGetArticlesByDateRangeProcedure();
await dbInitializer.CreateGetTopArticlesByKeywordProcedure();
await dbInitializer.CreateSearchArticlesByTextProcedure();

await executor.Do();





