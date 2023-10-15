namespace ArticleMaster.Scraper.Domain;

public record Url(UrlDomain Domain, UrlLang Lang, UrlEntity EntitiesName, string Сoncluding)
{
    public override string ToString()
    {
        return $"{Domain.DomainName}/{Lang.LangName}/{EntitiesName.EntityName}/{Сoncluding}/";
    }
}