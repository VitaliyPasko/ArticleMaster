using ArticleMaster.Application.Interfaces;
using ArticleMaster.Persistence.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ArticleMaster.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IArticleRepository, ArticleRepository>(_ => new ArticleRepository(configuration.GetConnectionString("MsSql")!));
        return services;
    }
}