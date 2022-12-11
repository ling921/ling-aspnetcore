using Microsoft.AspNetCore.Mvc.Filters;

namespace Ling.AspNetCore.Filters;

/// <summary>
/// A filter that handles exceptions from action.
/// </summary>
public class ExceptionHandlerFilter : IAsyncExceptionFilter, IOrderedFilter, IFilterMetadata
{
    /// <inheritdoc/>
    public int Order { get; init; }

    /// <summary>
    /// Initialize a new instance of <see cref="ExceptionHandlerFilter"/>.
    /// </summary>
    public ExceptionHandlerFilter() { }

    /// <summary>
    /// Initialize a new instance of <see cref="ExceptionHandlerFilter"/>.
    /// </summary>
    /// <param name="order">The execution order.</param>
    public ExceptionHandlerFilter(int order) => Order = order;

    /// <inheritdoc/>
    public virtual Task OnExceptionAsync(ExceptionContext context)
    {
        return Task.CompletedTask;
    }
}
