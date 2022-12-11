using Ling.AspNetCore.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace Ling.AspNetCore.Abstractions;

/// <summary>
/// A
/// </summary>
[ApiController]
[ExceptionHandler]
public abstract class ApiController : ControllerBase
{
}
