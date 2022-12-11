using Ling.AspNetCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Ling.AspNetCore.Attributes;

/// <summary>
/// A filter attribute that handles exceptions from action.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public class UnifyResultAttribute : Attribute, IAsyncActionFilter, IOrderedFilter, IFilterMetadata
{
    /// <inheritdoc/>
    public int Order { get; init; }

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
