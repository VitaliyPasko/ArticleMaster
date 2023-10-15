using ArticleMaster.Scraper.Domain;

namespace ArticleMaster.Scraper.Contracts;

public interface IUrlBuilder
{
    Url BuildUrl(UrlDomain urlDomain, UrlLang urlLang, UrlEntity urlEntity, string ending);
}