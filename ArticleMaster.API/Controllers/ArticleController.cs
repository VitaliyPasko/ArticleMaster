using ArticleMaster.Application.Common.Exceptions;
using ArticleMaster.Application.Dto;
using ArticleMaster.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace ArticleMaster.API.Controllers;

[Route("api")]
[ApiController]
public class ArticleController : Controller
{
    private readonly IArticleService _articleService;
    private readonly ILogger<ArticleController> _logger;

    public ArticleController(IArticleService articleService, ILogger<ArticleController> logger)
    {
        _articleService = articleService;
        _logger = logger;
    }

    [HttpGet("posts")]
    public async Task<IActionResult> Get([FromQuery]DateTime? from, [FromQuery]DateTime? to)
    {
        _logger.LogInformation("{@Controller} Request: from: {@From}, to: {@To}", 
            typeof(ArticleController), from, to);
        var articles = await _articleService.GetByDatesAsync(from, to);
        _logger.LogInformation("{@Controller} Response: {@Response}", 
            typeof(ArticleController), articles.Select(a => 
            new { a.Id, a.DownloadedFrom, a.Author, a.DatePublished, a.Title}));
        return Ok(articles);
    }
    
    [HttpGet("topten")]
    public async Task<IActionResult> Get()
    {
        var result = await _articleService.GetTopTenWordsAsync();
        _logger.LogInformation("{@Controller}Response: {@Response}", 
            typeof(ArticleController), result);
        return Ok(result);
    }
    
    [HttpGet("search")]
    public async Task<IActionResult> Get([FromQuery] string text)
    {
        _logger.LogInformation("{@Controller} Request: text: {@Text})", 
            typeof(ArticleController), text);
        if (string.IsNullOrEmpty(text))
            throw new CustomValidationException(nameof(ArticleDto), text);
        var articles = await _articleService.GetArticlesByTextAsync(text);
        _logger.LogInformation("{@Controller} Response: {@Response}", 
            typeof(ArticleController), articles.Select(a => 
                new { a.Id, a.DownloadedFrom, a.Author, a.DatePublished, a.Title}));
        return Ok(articles);
    }
}