using Ling.AspNetCore.Models;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Ling.AspNetCore.Filters;

/// <summary>
/// A filter that handles exceptions from action to <see cref="UnifyResult{T}"/>.
/// </summary>
public class UnifyExceptionHandlerFilter : ExceptionHandlerFilter
{
    /// <summary>
    /// Initialize a new instance of <see cref="UnifyExceptionHandlerFilter"/>.
    /// </summary>
    public UnifyExceptionHandlerFilter() { }

    /// <summary>
    /// Initialize a new instance of <see cref="UnifyExceptionHandlerFilter"/>.
    /// </summary>
    /// <param name="order">The execution order.</param>
    public UnifyExceptionHandlerFilter(int order) : base(order) { }

    /// <inheritdoc/>
    public override Task OnExceptionAsync(ExceptionContext context)
    {

        return Task.CompletedTask;
    }
}
