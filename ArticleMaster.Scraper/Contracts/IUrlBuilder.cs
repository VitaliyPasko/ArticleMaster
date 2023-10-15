using ArticleMaster.Scraper.Domain.Objects;

namespace ArticleMaster.Scraper.Contracts;

public interface IUrlBuilder
{
    Url BuildUrl(UrlDomain urlDomain, UrlLang urlLang, UrlEntity urlEntity, string ending);
}