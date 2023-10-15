using System.Text;
using ArticleMaster.Scraper.Contracts;
using ArticleMaster.Scraper.Domain.Objects;

namespace ArticleMaster.Scraper.Domain;

public class PageRecipient : IPageRecipient
{
    private readonly IHttpClientFactory _httpClientFactory;

    public PageRecipient(
        IHttpClientFactory httpClientFactory) => _httpClientFactory = httpClientFactory;

    public async Task<IEnumerable<PageInfo>> PullPagesAsync(IEnumerable<PageInfo> pageInfos)
    {
        var client = _httpClientFactory.CreateClient();
        var results = new List<PageInfo>();
        await Parallel.ForEachAsync(
            source: pageInfos, 
            body: async (pageInfo, cancellationToken) =>
            {
                var result = await GetAsync(client, pageInfo.Url, cancellationToken);
                results.Add(result);
            });
        
        return results;
    }
    
    static async Task<PageInfo> GetAsync(
        HttpClient client, 
        Url url, 
        CancellationToken cancellationToken)
    {
        using HttpResponseMessage response =
            await client.GetAsync(url.ToString(), cancellationToken);

        Console.WriteLine(
            $"URL: {url}, HTTP status code: {response.StatusCode} ({(int)response.StatusCode})");
        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        var builder = new StringBuilder(content);
        return new PageInfo
        {
            Url = url,
            HtmlPage = builder
        };
    }
}