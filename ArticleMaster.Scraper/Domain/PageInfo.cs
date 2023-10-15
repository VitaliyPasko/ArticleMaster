using System.Text;
using ArticleMaster.Scraper.Domain.Objects;

namespace ArticleMaster.Scraper.Domain;

public class PageInfo
{
    public required Url Url { get; set; }
    public StringBuilder? HtmlPage { get; set; }
}