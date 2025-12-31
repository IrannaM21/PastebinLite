using System.Text.Json.Serialization;

namespace PastebinLite.DTO;

public class CreatePasteRequest
{
    [JsonPropertyName("content")]
    public string Content { get; set; } = string.Empty;

    [JsonPropertyName("ttl_seconds")]
    public int? TtlSeconds { get; set; }

    [JsonPropertyName("max_views")]
    public int? MaxViews { get; set; }
}
