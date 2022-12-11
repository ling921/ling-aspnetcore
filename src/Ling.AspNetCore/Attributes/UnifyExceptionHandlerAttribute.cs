using Microsoft.AspNetCore.Mvc.Filters;

namespace Ling.AspNetCore.Attributes;

/// <summary>
/// A filter attribute that handles exceptions from action.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
internal class UnifyExceptionHandlerAttribute : ExceptionHandlerAttribute
{
    public override Task OnExceptionAsync(ExceptionContext context)
    {
        return Task.CompletedTask;
    }
}
