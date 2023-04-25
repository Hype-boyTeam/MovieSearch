using Azure.Storage.Blobs;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Analysis;
using Elastic.Clients.Elasticsearch.IndexManagement;
using Elastic.Clients.Elasticsearch.Mapping;
using Microsoft.AspNetCore.Mvc;
using MovieSearch.Models;

namespace MovieSearch.Controllers;

/// <summary>
/// 내부 디버그 용도로 사용되는 API를 제공합니다. 
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
    private readonly BlobContainerClient _blob;

    public InternalController(ILogger<InternalController> logger, MovieDb db, ElasticsearchClient elastic, BlobContainerClient blob)
    {
        _logger = logger;
        _db = db;
        _elastic = elastic;
        _blob = blob;
    }

    /// <summary>
    /// Azure Blob Storage에 파일을 업로드 합니다. (테스트 용)
    /// </summary>
    [HttpPost]
    public async Task UploadMoviePosterTest(string name, IFormFile poster)
    {
        _logger.LogDebug("Uploading {Name} (size: {Size})", name, poster.Length);
        await _blob.UploadBlobAsync(name, poster.OpenReadStream());
    }
    
    [HttpPost]
    public async Task AddMovie(string name, string detailUrl, DateTime? releasedAt, IFormFile? poster)
    {
        // _db.Infos.Add(new MovieInfo
        // {
        //     Name = name,
        //     
        // });
        // await _db.SaveChangesAsync();
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
