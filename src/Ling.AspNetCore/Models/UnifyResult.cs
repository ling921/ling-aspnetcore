using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Ling.AspNetCore.Models;

/// <summary>
/// A static class provider methods to create <see cref="UnifyResult{T}"/>.
/// </summary>
public static class UnifyResult
{
    /// <summary>
    /// Create a successful result without response data.
    /// </summary>
    /// <returns>A successful <see cref="UnifyResult"/>.</returns>
    public static UnifyResult<object?> Success()
    {
        return new UnifyResult<object?> { Status = "success" };
    }

    /// <summary>
    /// Create a successful result with response data.
    /// </summary>
    /// <param name="data">The response data.</param>
    /// <returns>A successful <see cref="UnifyResult"/>.</returns>
    public static UnifyResult<T?> Success<T>(T? data)
    {
        return new UnifyResult<T?> { Status = "success", Data = data };
    }

    /// <summary>
    /// Create a failed result with message.
    /// </summary>
    /// <param name="modelState">The validation errors.</param>
    /// <returns>A failed <see cref="UnifyResult"/>.</returns>
    public static UnifyResult<object?> Fail(ModelStateDictionary modelState)
    {
        return new UnifyResult<object?>
        {
            Status = "fail",
            Errors = new SerializableError(modelState)
        };
    }

    /// <summary>
    /// Create an exceptional result with message.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <returns>An exceptional <see cref="UnifyResult"/>.</returns>
    public static UnifyResult<object?> Error(string message)
    {
        return new UnifyResult<object?> { Status = "error", Message = message };
    }
}
