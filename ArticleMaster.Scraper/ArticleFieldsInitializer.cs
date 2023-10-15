using ArticleMaster.Scraper.Domain;

namespace ArticleMaster.Scraper;

public class ArticleFieldsInitializer
{
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
}