using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using UuidExtensions;

namespace MovieSearch.Models;

public record MovieInfo
{
    [Key] public Guid Id { get; set; } = Uuid7.Guid();

    public required string Name { get; set; }

    public string? PosterUrl { get; set; }

    public required DateTime ReleasedAt { get; set; }

    public int RunningTime { get; set; }

    public required string Director { get; set; }

    public required string DetailsUrl { get; set; }
}
