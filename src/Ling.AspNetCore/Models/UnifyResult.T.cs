using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

namespace Ling.AspNetCore.Models;

/// <summary>
/// Represents a unified formatted result.
/// </summary>
/// <typeparam name="T">The type of response data.</typeparam>
public class UnifyResult<T>
{
    /// <summary>
    /// Gets the response status.
    /// </summary>
    /// <example>success</example>
    public string Status { get; init; } = null!;

    /// <summary>
    /// Gets the response data.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public T? Data { get; init; }

    /// <summary>
    /// Gets get error message.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Message { get; init; }

    /// <summary>
    /// Gets get errors.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public SerializableError? Errors { get; set; }
}
