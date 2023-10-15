using ArticleMaster.Application.Implementations;
using ArticleMaster.Application.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ArticleMaster.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IArticleService, ArticleService>();
        return services;
    }
}