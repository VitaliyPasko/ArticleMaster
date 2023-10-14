using ArticleMaster.Domain;

namespace ArticleMaster.Scraper;

public class UrlBuilder : IUrlBuilder
{
    public Url BuildUrl(Domain domain, Lang lang, Entity entity, string ending)
    {
        return new Url(domain.DomainName, lang.LangName, entity.EntityName, ending);
    }
}