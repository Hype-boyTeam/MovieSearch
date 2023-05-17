using Elastic.Clients.Elasticsearch;
using Microsoft.EntityFrameworkCore;

namespace MovieSearch.Models;

public class MovieService
{
    private const string IndexName = "subtitle";

    private readonly ILogger<MovieService> _logger;
    private readonly MovieDb _db;
    private readonly ElasticsearchClient _elastic;

    public MovieService(ILogger<MovieService> logger, MovieDb db, ElasticsearchClient elastic)
    {
        _logger = logger;
        _db = db;
        _elastic = elastic;
    }

    /// <summary>
    /// 영화를 MovieSearch 서비스에서 검색될 수 있도록 DB에 추가합니다.
    /// </summary>
    public async Task AddMovie()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// 주어진 <paramref name="movieId"/>들의 자세한 영화 정보(제목, 개봉일, 넷플릭스 링크) 등을 조회합니다.
    /// 반환하는 영화 정보 순서는 <paramref name="movieId"/>에 주어진 순서와 일치하는 것을 보장합니다.
    /// </summary>
    /// <param name="movieId">조회할 영화 id의 목록입니다.</param>
    public async Task<IList<MovieInfo>> GetDetails(IList<Guid> movieId)
    {
        return await _db.Infos
            .Where(x => movieId.Contains(x.Id))
            .ToListAsync();
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
    /// <returns>
    /// <paramref name="text"/>가 나온 적이 있는 영화의 id 목록입니다.
    /// 가장 관련성이 높은 것이 앞에 배치됩니다.
    /// </returns>
    public async Task<IList<Guid>> SearchMovies(string text, int limit = 10)
    {
        _logger.LogDebug("{Text} 검색 결과: TODO");
        throw new NotImplementedException();
    }
}
