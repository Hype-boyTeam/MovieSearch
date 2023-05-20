using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Analysis;
using Elastic.Clients.Elasticsearch.IndexManagement;
using Elastic.Clients.Elasticsearch.Mapping;
using Microsoft.AspNetCore.Mvc;
using MovieSearch.Models;
using UuidExtensions;

namespace MovieSearch.Controllers;

/// <summary>
/// 테스트 용도로 사용되는 API를 제공합니다. 
/// </summary>
/// <remarks>
/// 실제 서비스 개발 시에는 이 API를 외부에 노출하지 않거나 인증이 필요하지만
/// MovieSearch에는 프로젝트 성격상 구현하지 않았습니다.
/// </remarks>
[ApiController]
[Route("[controller]/[action]")]
public class InternalController : ControllerBase
{
    private readonly ILogger<InternalController> _logger;
    private readonly PosterService _posterService;
    private readonly MovieInfoService _movieInfoService;
    private readonly MovieSearchService _movieSearchService;

    public InternalController(ILogger<InternalController> logger, PosterService posterService,
        MovieInfoService movieInfoService, MovieSearchService movieSearchService)
    {
        _logger = logger;
        _posterService = posterService;
        _movieInfoService = movieInfoService;
        _movieSearchService = movieSearchService;
    }

    [HttpPost]
    public async Task<IActionResult> AddMovie([FromForm] AddMovieForm form)
    {
        var movieId = Uuid7.Guid();

        // 자막 파일 읽기
        using var subtitleReader = new StreamReader(form.Subtitle.OpenReadStream());
        var subtitle = await subtitleReader.ReadToEndAsync();

        // 포스터 이미지를 업로드
        var posterUri = await _posterService.UploadImage(movieId, form.Poster);

        // 영화 정보 추가
        var movieInfo = new MovieInfo
        {
            Id = movieId,
            Name = form.Name,
            DetailsUrl = form.DetailsUrl,
            ReleasedAt = form.ReleasedAt,
            Director = form.Director,
            RunningTime = form.RunningTime,
            PosterUrl = posterUri.ToString(),
        };
        await _movieInfoService.AddMovie(movieInfo);

        // 영화 검색 인덱스 추가
        var document = new MovieDocument
        {
            Id = movieId,
            Name = form.Name,
            Text = subtitle,
        };
        await _movieSearchService.AddSubtitle(document);

        return Ok();
    }

    /// <summary>
    /// Moviesearch 동작에 필요한 Elasticsearch 인덱스를 생성합니다.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> SetupElasticIndex()
    {
        await _movieSearchService.CreateIndex();
        return Ok();
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteElasticIndex()
    {
        await _movieSearchService.DeleteIndex();
        return Ok();
    }
}
