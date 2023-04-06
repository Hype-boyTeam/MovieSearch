using System.ComponentModel.DataAnnotations;

namespace MovieSearch.Models;

public record MovieInfo
{
    [Key]
    public Guid Id { get; init; } = Guid.NewGuid();
    
    public required string Name { get; init; }
    
    public required string InfoUrl { get; init; } 
}
