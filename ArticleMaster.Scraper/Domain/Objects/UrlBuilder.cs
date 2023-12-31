using ArticleMaster.Scraper.Contracts;

namespace ArticleMaster.Scraper.Domain.Objects;

public class UrlBuilder : IUrlBuilder
{
    public Url BuildUrl(UrlDomain urlDomain, UrlLang urlLang, UrlEntity urlEntity, string ending)
        => new (urlDomain, urlLang, urlEntity, ending);
}