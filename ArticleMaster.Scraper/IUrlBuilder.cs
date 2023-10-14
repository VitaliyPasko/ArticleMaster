using ArticleMaster.Domain;

namespace ArticleMaster.Scraper;

public interface IUrlBuilder
{
    Url BuildUrl(Domain domain, Lang lang, Entity entity, string ending);
}