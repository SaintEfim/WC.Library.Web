using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace WC.Library.Web.Models;

/// <summary>
/// Represents an error response DTO with details about the error.
/// </summary>
public class ErrorDto
{
    /// <summary>
    /// The title or summary of the error.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// The HTTP status code associated with the error.
    /// </summary>
    [Required]
    public int Status { get; set; } = StatusCodes.Status500InternalServerError;

    /// <summary>
    ///     A human-readable explanation specific to this occurrence of the problem.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Stack trace associated with the error.
    /// </summary>
    public IEnumerable<string> StackTrace { get; set; } = Enumerable.Empty<string>();

    /// <summary>
    ///     Problem type definitions may extend the problem details object with additional members.
    /// </summary>
    public Dictionary<string, object> Extensions { get; set; } = new();
}