using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
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
    private readonly MovieDb _db;
    private readonly ElasticsearchClient _elastic;
    private readonly BlobContainerClient _blobContainer;

    public InternalController(ILogger<InternalController> logger, MovieDb db, ElasticsearchClient elastic,
        BlobContainerClient blobContainer)
    {
        _logger = logger;
        _db = db;
        _elastic = elastic;
        _blobContainer = blobContainer;
    }

    /// <summary>
    /// Azure Blob Storage에 파일을 업로드 합니다. (테스트 용)
    /// </summary>
    [HttpPost]
    public async Task UploadMoviePosterTest(string name, IFormFile poster)
    {
        _logger.LogDebug("Uploading {Name} (size: {Size})", name, poster.Length);
        await _blobContainer.UploadBlobAsync(name, poster.OpenReadStream());
    }

    [HttpPost]
    public async Task<ActionResult> AddMovie([FromForm] AddMovieForm form)
    {
        var movieId = Uuid7.Guid();
        var movieRecord = new MovieInfo
        {
            Id = movieId,
            Name = form.Name,
            DetailsUrl = form.DetailsUrl,
            ReleasedAt = form.ReleasedAt,
            Director = form.Director,
        };

        if (form.Poster.ContentType != "image/png")
        {
            _logger.LogWarning("TODO: should we reject {FileName} as it has {Type}?", form.Poster.FileName,
                form.Poster.ContentType);
        }

        _logger.LogInformation(
            "Uploading the poster for {Name} ({Id}, {ImageSize}bytes)",
            form.Name,
            movieId,
            form.Poster.Length
        );

        // 포스터 업로드
        var blob = _blobContainer.GetBlobClient($"posters/{movieId}.png");
        await blob.UploadAsync(form.Poster.OpenReadStream(), new BlobHttpHeaders
        {
            // TODO: png만?
            ContentType = "image/png"
        });

        _logger.LogInformation("Uploaded the poster for {Id} ({Url})", movieId, blob.Uri);

        movieRecord.PosterUrl = blob.Uri.ToString();

        // 자막 업로드 (테스트2)
        var subtitleBlob = _blobContainer.GetBlobClient($"subtitles/{movieId}.xml");
        await subtitleBlob.UploadAsync(form.Subtitle.OpenReadStream(), new BlobHttpHeaders
        {
            ContentType = "application/xml"
        });

        _db.Infos.Add(movieRecord);
        await _db.SaveChangesAsync();

        // TODO: elastic에도 삽입?
        // TODO: 지금 당장은 작동하기만 하면 족한 상황이지만.. 읽기 좋지 않은 코드를 정리할 필요가 있음
        using var subtitleReader = new StreamReader(form.Subtitle.OpenReadStream());
        var createRequest = new CreateRequest<MovieDocument>("subtitle", movieId)
        {
            Document = new MovieDocument
            {
                Id = movieId,
                Name = form.Name,
                Text = await subtitleReader.ReadToEndAsync(),
            }
        };
        var createResponse = await _elastic.CreateAsync(createRequest);
        if (!createResponse.IsSuccess())
        {
            _logger.LogError("Failed to create a document for movie {Id} ({Name}): {Debug}", movieId, form.Name,
                createResponse.DebugInformation);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        return Ok();
    }

    /// <summary>
    /// Moviesearch 동작에 필요한 Elasticsearch 인덱스를 생성합니다.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult> SetupElasticIndex()
    {
        var request = new CreateIndexRequest("subtitle")
        {
            Settings = new IndexSettings
            {
                Analysis = new IndexSettingsAnalysis
                {
                    Analyzers = new Analyzers
                    {
                        {"moviesearch_custom", new NoriAnalyzer()}
                    }
                    // TODO: strip html
                }
            },
            Mappings = new TypeMapping
            {
                Properties = new Properties<MovieDocument>
                {
                    {
                        x => x.Text, new TextProperty
                        {
                            Analyzer = "moviesearch_custom"
                        }
                    },
                    {
                        x => x.Name, new TextProperty
                        {
                            Analyzer = "moviesearch_custom"
                        }
                    }
                }
            }
        };

        var response = await _elastic.Indices.CreateAsync(request);

        if (!response.IsSuccess())
        {
            _logger.LogError("Failed to create Elasticsearch indices: {Debug}", response.DebugInformation);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        _logger.LogInformation("Created Elasticsearch indices");

        return Ok();
    }

    [HttpDelete]
    public async Task<ActionResult> DeleteElasticIndex()
    {
        var response = await _elastic.Indices.DeleteAsync("subtitle");
        if (!response.IsSuccess())
        {
            _logger.LogError("Failed to remove elastic indices: {Debug}", response.DebugInformation);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        _logger.LogInformation("Removed Elasticsearch indices");

        return Ok();
    }
}
