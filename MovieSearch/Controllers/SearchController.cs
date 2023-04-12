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
}
