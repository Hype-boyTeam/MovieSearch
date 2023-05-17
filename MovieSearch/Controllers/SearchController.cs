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
    private readonly ILogger<SearchController> _logger;
    private readonly MovieService _movieService;

    public SearchController(ILogger<SearchController> logger, MovieService movieService)
    {
        _logger = logger;
        _movieService = movieService;
    }

    [HttpGet("/search")]
    [ProducesResponseType(typeof(IList<MovieInfo>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SearchText(string text)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var movieId = await _movieService.SearchMovies(text);
        var movieInfo = await _movieService.GetDetails(movieId);

        return Ok(movieInfo);
    }
}
