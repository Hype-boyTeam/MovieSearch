using Elastic.Clients.Elasticsearch;
using Microsoft.AspNetCore.Mvc;

namespace MovieSearch.Controllers;

[ApiController]
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
}
