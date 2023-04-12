using Elastic.Clients.Elasticsearch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieSearch.Models;

namespace MovieSearch.Controllers;

[ApiController]
[Route("search")]
public class SearchController : ControllerBase
{
    private readonly ILogger<SearchController> _logger;
    private readonly MovieDb _db;
    private readonly ElasticsearchClient _elastic;

    public SearchController(ILogger<SearchController> logger, MovieDb db, ElasticsearchClient elastic)
    {
        _logger = logger;
        _db = db;
        _elastic = elastic;
    }

    [HttpGet("text")]
    public async Task<ActionResult> SearchByText(string query)
    {
        // TODO
        return Ok();
    }

    [HttpPost("db_test")]
    public async Task<ActionResult> TestAdd(string name, string url)
    {
        _db.Infos.Add(new MovieInfo
        {
            Name = name,
            InfoUrl = url,
        });
        await _db.SaveChangesAsync();

        return Ok();
    }

    [HttpGet("db_test_json")]
    public async Task<ActionResult<List<MovieInfo>>> TestGet(string name)
    {
        return await _db.Infos
            .Where(row => row.Name == name)
            .ToListAsync();
    }

    [HttpGet("db_test/{name}")]
    public async Task<ActionResult<string?>> TestGetUrl(string name)
    {
        _logger.LogTrace("로깅 예제: Searching for {Name}...", name);
        
        var url = await _db.Infos
            .Where(row => row.Name == name)
            .Select(row => row.InfoUrl)
            .FirstOrDefaultAsync();

        _logger.LogDebug("{Name}에 해당하는 url 검색 결과: {Url}", name, url ?? "결과 없음");

        return url;
    }

    [HttpGet("test_deploy")]
    public string TestStuff()
    {
        return "Hi3";
    }
}
