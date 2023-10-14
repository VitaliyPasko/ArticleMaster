using System.Text;
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
    
    public async Task<string> PullPageAsync(string url)
    {
        using var client = _httpClientFactory.CreateClient();
        try
        {
            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            await using var stream = await response.Content.ReadAsStreamAsync();
            using var reader = new StreamReader(stream);
            var stringBuilder = new StringBuilder();
            var buffer = new char[int.Parse(_configuration.GetSection("Buffer").Value ?? "8192")];

            int bytesRead;
            while ((bytesRead = await reader.ReadAsync(buffer, 0, buffer.Length)) > 0)
                stringBuilder.Append(buffer, 0, bytesRead);

            return stringBuilder.ToString();
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine("Ошибка HTTP-запроса: {0}. Url: {1}", ex.Message, url);
            return string.Empty;
        }
    }
}