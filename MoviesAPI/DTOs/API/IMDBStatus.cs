using System;
using System.Text.Json.Serialization;

namespace MoviesAPI.DTOs.API;

public class IMDBStatus
{
    [JsonPropertyName("up")]
    public bool Up { get; set; }

    [JsonPropertyName("last_call")]
    public DateTime LastCall { get; set; }
}
