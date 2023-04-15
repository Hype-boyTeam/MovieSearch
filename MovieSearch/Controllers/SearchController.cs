using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MovieSearch.Models;

namespace MovieSearch.Controllers;

[ApiController]
public class SearchController : ControllerBase
{
    private const string IndexName = "subtitle";

    private readonly ILogger<SearchController> _logger;
    private readonly ElasticsearchClient _elastic;

    public SearchController(ILogger<SearchController> logger, ElasticsearchClient elastic)
    {
        _logger = logger;
        _elastic = elastic;
    }

    [HttpGet("/search")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<MovieDocument>>> SearchText(string text)
    {
        var response = await _elastic.SearchAsync<MovieDocument>(s => s
            .Index(IndexName)
            .From(0)
            .Size(10)
            .Query(q => q
                .Match(t => t
                    .Field(f => f.Text)
                    .Query(text)
                )
            )
        );

        if (!response.IsValidResponse)
        {
            _logger.LogError("failed to search {Text}: {Reason}", text, response.DebugInformation);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        return response.Documents.ToList();
    }
}
