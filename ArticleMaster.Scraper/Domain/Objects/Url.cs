namespace ArticleMaster.Scraper.Domain.Objects;

public record Url(UrlDomain Domain, UrlLang Lang, UrlEntity EntitiesName, string Сoncluding)
{
    public override string ToString()
        => $"{Domain.DomainName}/{Lang.LangName}/{EntitiesName.EntityName}/{Сoncluding}/";
}