namespace MovieSearch.Models;

public record MovieDocument
{
    public Guid Id { get; set; } = Guid.Empty;

    /// <summary>
    /// 영화 제목
    /// </summary>
    public string Name { get; set; } = "";
    
    
    public string Text { get; set; } = "";
}
