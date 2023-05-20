using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using UuidExtensions;

namespace MovieSearch.Models;

public record MovieInfo
{
    [JsonPropertyName("id")] [Key] public Guid Id { get; init; } = Uuid7.Guid();

    [JsonPropertyName("name")] public required string Name { get; init; }

    [JsonPropertyName("released_at")] public required DateTime ReleasedAt { get; init; }

    [JsonPropertyName("director")] public required string Director { get; init; }

    [JsonPropertyName("details_url")] public required string DetailsUrl { get; init; }

    [JsonPropertyName("poster_url")] public required string PosterUrl { get; init; }
    
    [JsonPropertyName("running_time")] public int RunningTime { get; init; }
}
