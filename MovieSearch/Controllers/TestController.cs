using Elastic.Clients.Elasticsearch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieSearch.Models;

namespace MovieSearch.Controllers;

[ApiController]
[Route("demo")]
public class TestController : ControllerBase
{
    private readonly ILogger<TestController> _logger;
    private readonly MovieDb _db;
    private readonly ElasticsearchClient _elastic;

    public TestController(ILogger<TestController> logger, MovieDb db, ElasticsearchClient elastic)
    {
        _logger = logger;
        _db = db;
        _elastic = elastic;
    }
    
    [HttpGet("test")]
    public string Demo()
    {
        return "asdf";
    }
}
