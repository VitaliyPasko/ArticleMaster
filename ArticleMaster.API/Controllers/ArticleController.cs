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

    public ArticleController(IArticleService articleService)
    {
        _articleService = articleService;
    }

    [HttpGet("posts")]
    public async Task<IActionResult> Get([FromQuery]DateTime? from, [FromQuery]DateTime? to)
    {
        var articles = await _articleService.GetByDatesAsync(from, to);
        return Ok(articles);
    }
    
    [HttpGet("topten")]
    public async Task<IActionResult> Get()
    {
        var articles = await _articleService.GetTopTenWordsAsync();
        return Ok(articles);
    }
    
    [HttpGet("search")]
    public async Task<IActionResult> Get([FromQuery] string text)
    {
        if (string.IsNullOrEmpty(text))
            throw new CustomValidationException(nameof(ArticleDto), text);
        var articles = await _articleService.GetArticlesByTextAsync(text);
        return Ok(articles);
    }
}