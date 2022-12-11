using Ling.AspNetCore.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace Ling.AspNetCore.Abstractions;

/// <summary>
/// A
/// </summary>
[ApiController]
[UnifyResult]
[UnifyExceptionHandler]
public abstract class ApiUnifyController : ControllerBase
{
}
