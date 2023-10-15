using System.Text;
using ArticleMaster.Scraper.Domain;

namespace ArticleMaster.Scraper;

public class PageInfo
{
    public required Url Url { get; set; }
    public StringBuilder? HtmlPage { get; set; }
}