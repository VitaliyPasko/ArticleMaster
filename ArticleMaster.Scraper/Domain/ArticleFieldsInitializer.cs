using System.Text.RegularExpressions;
using ArticleMaster.Scraper.Domain.Objects;
using Microsoft.Extensions.Configuration;

namespace ArticleMaster.Scraper.Domain;

public class ArticleFieldsInitializer
{
    private readonly IConfiguration _configuration;

    public ArticleFieldsInitializer(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void SetTitle(Article article)
    {
        var startTagName = "<title>";
        var endTagName = "</title>";
        int titleStartIndex = article.Content?.IndexOf(startTagName, StringComparison.Ordinal) + startTagName.Length ?? -1;
        int titleEndIndex = article.Content?.IndexOf(endTagName, titleStartIndex, StringComparison.Ordinal) ?? -1;

        if (titleStartIndex != -1 && titleEndIndex != -1)
        {
            string? title = article.Content?.Substring(titleStartIndex, titleEndIndex - titleStartIndex);
            Console.WriteLine($"Заголовок страницы: {title} ----------------------------------");
            article.Title = title;
        }
    }
    
    public void SetDatePublished(Article article)
    {
        string pattern = _configuration.GetSection("DateTimePatternMatching").Value ?? """
                         "datePublished":"([^"]+)"
                         """;
        Match match = Regex.Match(article.Content!, pattern);

        if (match.Success)
        {
            string datePublished = match.Groups[1].Value;
            article.DatePublished = DateTime.Parse(datePublished);
        }
        else
        {
            Console.WriteLine("Не удалось найти datePublished.");
        }
    }

    public void SetAuthorName(Article article)
    {
        string pattern = @"""name"":\s*""([^""]+)""";
        
        Match match = Regex.Match(article.Content!, pattern);

        if (match.Success)
        {
            article.Author.Name = match.Groups[1].Value;
            Console.WriteLine(article);
        }
        else
        {
            Console.WriteLine("Не удалось найти автора.");
        }
    }
}