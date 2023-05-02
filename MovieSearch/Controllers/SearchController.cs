using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieSearch.Models;

namespace MovieSearch.Controllers;

[ApiController]
public class SearchController : ControllerBase
{
    private const string IndexName = "subtitle";

    private readonly ILogger<SearchController> _logger;
    private readonly MovieDb _db;
    private readonly ElasticsearchClient _elastic;

    public SearchController(ILogger<SearchController> logger, MovieDb db, ElasticsearchClient elastic)
    {
        _logger = logger;
        _db = db;
        _elastic = elastic;
    }

    [HttpGet("/search")]
    public async Task<ActionResult<List<MovieInfo>>> SearchText(string text)
    {
        var request = new SearchRequest(IndexName)
        {
            From = 0,
            Size = 30,
            Query = new MatchQuery(Infer.Field<MovieDocument>(f => f.Text))
            {
                Query = text
            }
        };

        var response = await _elastic.SearchAsync<MovieDocument>(request);
        
        if (!response.IsSuccess())
        {
            _logger.LogError("Failed to search {Text}: {Reason}", text, response.DebugInformation);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        _logger.LogDebug("Searched {Query} ({Count} entries)", text, response.Documents.Count);
        
        var ids = response.Documents.Select(x => x.Id).ToArray();

        return await _db.Infos
            .Where(x => ids.Contains(x.Id))
            .ToListAsync();;
    }
}
