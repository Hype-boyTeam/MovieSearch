namespace MovieSearch.Models;

public record AddMovieForm
{
    /// <summary>
    /// 영화 제목
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// 넷플릭스 링크
    /// </summary>
    public required string DetailsUrl { get; set; }

    /// <summary>
    /// 개봉일
    /// </summary>
    public DateTime ReleasedAt { get; set; }

    /// <summary>
    /// 감독
    /// </summary>
    public required string Director { get; set; }

    /// <summary>
    /// 포스터 이미지 파일
    /// </summary>
    public IFormFile? Poster { get; set; }

    /// <summary>
    /// 자막 파일
    /// </summary>
    public FormFile? Subtitle { get; set; }
}
