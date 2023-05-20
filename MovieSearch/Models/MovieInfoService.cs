using Elastic.Clients.Elasticsearch;
using Microsoft.EntityFrameworkCore;

namespace MovieSearch.Models;

public class MovieInfoService
{
    private readonly ILogger<MovieInfoService> _logger;
    private readonly MovieDb _db;

    public MovieInfoService(ILogger<MovieInfoService> logger, MovieDb db)
    {
        _logger = logger;
        _db = db;
    }

    /// <summary>
    /// 영화를 MovieSearch 서비스에서 검색될 수 있도록 DB에 추가합니다.
    /// </summary>
    public async Task AddMovie(MovieInfo info)
    {
        _db.Infos.Add(info);
        await _db.SaveChangesAsync();
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
}
