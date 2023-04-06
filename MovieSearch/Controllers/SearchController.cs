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
    private readonly ElasticsearchClient _elasticsearch;

    public SearchController(ILogger<SearchController> logger, ElasticsearchClient esClient)
    {
        _logger = logger;
        _elasticsearch = esClient;
    }

    [HttpGet("text")]
    public async Task<ActionResult> SearchByText(string query)
    {
        // TODO
        return Ok();
    }

    [HttpPost("db_test")]
    public async Task<ActionResult> TestAdd(MovieDb db, string name, string url)
    {
        db.Infos.Add(new MovieInfo
        {
            Name = name,
            InfoUrl = url,
        });
        await db.SaveChangesAsync();

        return Ok();
    }

    [HttpGet("db_test_json")]
    public async Task<ActionResult<List<MovieInfo>>> TestGet(MovieDb db, string name)
    {
        return await db.Infos
            .Where(row => row.Name == name)
            .ToListAsync();
    }

    [HttpGet("db_test/{name}")]
    public async Task<ActionResult<string?>> TestGetUrl(MovieDb db, string name)
    {
        return await db.Infos
            .Where(row => row.Name == name)
            .Select(row => row.InfoUrl)
            .FirstOrDefaultAsync();
    }
}
