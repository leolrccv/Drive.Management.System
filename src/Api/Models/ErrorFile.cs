using System.Text.Json.Serialization;

namespace Api.Models;

internal sealed class ErrorFile
{
    [JsonPropertyName("errors")]
    public Dictionary<string, string> Errors { get; set; } = [];
}