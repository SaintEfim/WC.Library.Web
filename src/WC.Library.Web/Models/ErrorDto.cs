using System.Text.Json.Serialization;

namespace WC.Library.Web.Models;

/// <summary>
/// Represents an error response DTO with details about the error.
/// </summary>
public class ErrorDto
{
    /// <summary>
    /// The type of the error.
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// The title or summary of the error.
    /// </summary>
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// The HTTP status code associated with the error.
    /// </summary>
    [JsonPropertyName("status")]
    public int Status { get; set; } = 500;

    /// <summary>
    /// Dictionary of field names and corresponding error messages.
    /// </summary>
    [JsonPropertyName("errors")]
    public Dictionary<string, string[]> Errors { get; set; } = new();

    /// <summary>
    /// Stack trace associated with the error.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("stackTrace")]
    public string? StackTrace { get; set; } = string.Empty;
}