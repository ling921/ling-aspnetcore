using Microsoft.AspNetCore.Mvc.Filters;

namespace Ling.AspNetCore.Attributes;

/// <summary>
/// A filter attribute that handles exceptions from action.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
internal class ExceptionHandlerAttribute : Attribute, IAsyncExceptionFilter, IOrderedFilter, IFilterMetadata
{
    /// <inheritdoc/>
    public int Order { get; init; }

    public virtual Task OnExceptionAsync(ExceptionContext context)
    {
        return Task.CompletedTask;
    }
}
