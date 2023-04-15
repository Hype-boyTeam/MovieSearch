using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieSearch.Models;

namespace MovieSearch.Controllers;

[ApiController]
public class MovieInfoController : ControllerBase
{
    private readonly ILogger<MovieInfoController> _logger;
    private readonly MovieDb _db;

    public MovieInfoController(ILogger<MovieInfoController> logger, MovieDb db)
    {
        _logger = logger;
        _db = db;
    }

    /// <summary>
    /// ㅁㄴㅇㄹ
    /// </summary>
    /// <param name="id">
    /// 테스트
    /// </param>
    /// <returns></returns>
    [HttpGet("/movie_details")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IList<MovieInfo>> GetInfo([FromQuery] IList<Guid> id)
    {
        return await _db.Infos
            .Where(r => id.Contains(r.Id))
            .ToListAsync();
    }
}
