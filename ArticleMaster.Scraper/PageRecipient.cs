using System.Text;
using ArticleMaster.Scraper.Contracts;
using ArticleMaster.Scraper.Domain;
using Microsoft.Extensions.Configuration;

namespace ArticleMaster.Scraper;

public class PageRecipient : IPageRecipient
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public PageRecipient(
        IHttpClientFactory httpClientFactory, 
        IConfiguration configuration) => 
        (_httpClientFactory,  _configuration) = 
        (httpClientFactory, configuration);
    
    public async Task<PageInfo> PullPageAsync(PageInfo pageInfo)
    {
        using var client = _httpClientFactory.CreateClient();
        try
        {
            HttpResponseMessage response = await client.GetAsync(pageInfo.Url.ToString());
            response.EnsureSuccessStatusCode();

            await using var stream = await response.Content.ReadAsStreamAsync();
            using var reader = new StreamReader(stream);
            var stringBuilder = new StringBuilder();
            var buffer = new char[int.Parse(_configuration.GetSection("Buffer").Value ?? "8192")];

            int bytesRead;
            while ((bytesRead = await reader.ReadAsync(buffer, 0, buffer.Length)) > 0)
                stringBuilder.Append(buffer, 0, bytesRead);
            pageInfo.HtmlPage = stringBuilder;
            return pageInfo;
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine(ex.StackTrace);
            Console.WriteLine("Ошибка HTTP-запроса: {0}. Url: {1}", ex.Message, pageInfo.Url);
            return pageInfo;
        }
    }

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