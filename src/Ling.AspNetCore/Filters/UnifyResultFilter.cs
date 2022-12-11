using Ling.AspNetCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Ling.AspNetCore.Filters;

/// <summary>
/// A filter that handles exceptions from action to <see cref="UnifyResult{T}"/>.
/// </summary>
public class UnifyResultFilter : IAsyncActionFilter, IOrderedFilter, IFilterMetadata
{
    /// <inheritdoc/>
    public int Order { get; init; }

    /// <summary>
    /// Initialize a new instance of <see cref="UnifyResultFilter"/>.
    /// </summary>
    public UnifyResultFilter() { }

    /// <summary>
    /// Initialize a new instance of <see cref="UnifyResultFilter"/>.
    /// </summary>
    /// <param name="order">The execution order.</param>
    public UnifyResultFilter(int order) => Order = order;

    /// <inheritdoc/>
    public virtual async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (context.ModelState.IsValid)
        {
            await next();

            switch (context.Result)
            {
                case ObjectResult result:
                    result.Value = UnifyResult.Success(result.Value);
                    break;
                case JsonResult result:
                    result.Value = UnifyResult.Success(result.Value);
                    break;
                case ViewResult _:
                default:
                    break;
            }
        }
        else
        {
            context.Result = new BadRequestObjectResult(UnifyResult.Fail(context.ModelState));
        }
    }
}
