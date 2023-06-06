using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Analysis;
using Elastic.Clients.Elasticsearch.IndexManagement;
using Elastic.Clients.Elasticsearch.Mapping;
using Elastic.Clients.Elasticsearch.QueryDsl;
using System.Text.RegularExpressions;


namespace MovieSearch.Models;

public sealed class MovieSearchService
{
    private const string IndexName = "subtitle";

    private readonly ILogger<MovieSearchService> _logger;
    private readonly ElasticsearchClient _elastic;

    public MovieSearchService(ILogger<MovieSearchService> logger, ElasticsearchClient elastic)
    {
        _logger = logger;
        _elastic = elastic;
    }

    public async Task AddSubtitle(MovieDocument document)
    {
        var createRequest = new CreateRequest<MovieDocument>(IndexName, document.Id)
        {
            Document = document
        };

        var createResponse = await _elastic.CreateAsync(createRequest);
        if (!createResponse.IsSuccess())
        {
            _logger.LogError("Failed to create a document for movie {Id} ({Name}): {Debug}", document.Id, document.Name,
                createResponse.DebugInformation);
            throw new ApplicationException(
                $"Failed to create an index for {document.Name}: {createResponse.DebugInformation}");
        }
    }

    /// <summary>
    /// 주어진 <paramref name="text"/>가 나온 적이 있는 영화를 검색합니다.
    /// </summary>
    /// <param name="text">
    /// 영화에 나온 적이 있는 대사입니다.
    /// </param>
    /// <param name="limit">
    /// 검색되는 영화의 최대 갯수를 제한합니다.
    /// </param>
    /// <param name="minimumScore">
    /// 검색되는 영화가 검색되기 위한 최소 점수입니다.
    /// </param>
    /// <returns>
    /// <paramref name="text"/>가 나온 적이 있는 영화의 id 목록입니다.
    /// 가장 관련성이 높은 것이 앞에 배치됩니다.
    /// </returns>
    public async Task<IList<Guid>> FindMovies(string text, int limit = 30, float minimumScore = 0.0f)
    {
        // 제목은 가중치 3배수
        var titleField = Infer.Field<MovieDocument>(f => f.Name, 3.0d);
        var textField = Infer.Field<MovieDocument>(f => f.Text);
        
        var request = new SearchRequest(IndexName)
        {
            From = 0,
            Size = limit,
            Query = new MultiMatchQuery
            {
                Query = text,
                Type = TextQueryType.Phrase,
                Fields = textField.And(titleField),
                Slop = 2,
            },
            MinScore = minimumScore,
        };

        var response = await _elastic.SearchAsync<MovieDocument>(request);

        if (!response.IsSuccess())
        {
            _logger.LogError("Failed to search {Text}: {Reason}", text, response.DebugInformation);
            throw new ApplicationException(
                $"Elasticsearch unexpectedly returned an error: {response.DebugInformation}");
        }

        _logger.LogInformation("Searched {Query} ({Count} entries)", text, response.Documents.Count);
        if (_logger.IsEnabled(LogLevel.Trace))
        {
            foreach (var searchHit in response.Hits)
            {
                if (searchHit.Source == null)
                {
                    continue;
                }

                _logger.LogTrace("Id={Id}, Name={Name}, Score={Score:F3}", searchHit.Id, searchHit.Source.Name,
                    searchHit.Score ?? -1);
            }
        }

        return response.Documents
            .Select(x => x.Id)
            .ToList();
    }

    public async Task CreateIndex()
    {
        var request = new CreateIndexRequest(IndexName)
        {
            Settings = new IndexSettings
            {
                Analysis = new IndexSettingsAnalysis
                {
                    Tokenizers = new Tokenizers
                    {
                        {
                            "nori_custom", new NoriTokenizer
                            {
                                DecompoundMode = NoriDecompoundMode.Mixed,
                            }
                        }
                    },
                    Analyzers = new Analyzers
                    {
                        {
                            "moviesearch_custom", new CustomAnalyzer
                            {
                                Tokenizer = "nori_custom",
                                // CharFilter = new[] {"html_strip"},
                            }
                        }
                    },
                }
            },
            Mappings = new TypeMapping
            {
                Properties = new Properties<MovieDocument>
                {
                    {
                        x => x.Text, new TextProperty
                        {
                            Analyzer = "moviesearch_custom",
                        }
                    },
                    {
                        x => x.Name, new TextProperty
                        {
                            Analyzer = "moviesearch_custom",
                        }
                    }
                }
            }
        };

        var response = await _elastic.Indices.CreateAsync(request);

        if (!response.IsSuccess())
        {
            _logger.LogError("Failed to create Elasticsearch indices: {Debug}", response.DebugInformation);
            throw new ApplicationException(
                $"Elasticsearch unexpectedly returned an error: {response.DebugInformation}");
        }

        _logger.LogInformation("Created Elasticsearch indices");
    }

    public async Task DeleteIndex()
    {
        var response = await _elastic.Indices.DeleteAsync(IndexName);
        if (!response.IsSuccess())
        {
            _logger.LogError("Failed to remove elastic indices: {Debug}", response.DebugInformation);
            throw new ApplicationException($"Could not delete index: {response.DebugInformation}");
        }

        _logger.LogInformation("Removed Elasticsearch indices");
    }
}
