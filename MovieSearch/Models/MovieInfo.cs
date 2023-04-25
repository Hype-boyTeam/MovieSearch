using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using UuidExtensions;

namespace MovieSearch.Models;

public record MovieInfo
{
    [JsonPropertyName("id")] [Key] public Guid Id { get; set; } = Uuid7.Guid();

    [JsonPropertyName("name")] public required string Name { get; set; }

    [JsonPropertyName("released_at")] public required DateTime ReleasedAt { get; set; }

    [JsonPropertyName("director")] public required string Director { get; set; }

    [JsonPropertyName("details_url")] public required string DetailsUrl { get; set; }

    [JsonPropertyName("running_time")] public int RunningTime { get; set; }

    [JsonPropertyName("poster_url")] public string? PosterUrl { get; set; }
}
