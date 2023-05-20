using Microsoft.AspNetCore.Mvc;
using MovieSearch.Models;

namespace MovieSearch.Controllers;

[ApiController]
public class SearchController : ControllerBase
{
    private readonly ILogger<SearchController> _logger;
    private readonly MovieInfoService _movieInfoService;
    private readonly MovieSearchService _movieSearchService;

    public SearchController(ILogger<SearchController> logger, MovieInfoService movieInfoService,
        MovieSearchService movieSearchService)
    {
        _logger = logger;
        _movieInfoService = movieInfoService;
        _movieSearchService = movieSearchService;
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

        var movieIdList = await _movieSearchService.FindMovies(text);
        var movieInfo = await _movieInfoService.GetDetails(movieIdList);

        return Ok(movieInfo);
    }
}
