using GestorExcursiones.Application.Core;
using Microsoft.AspNetCore.Mvc;

namespace GestorExcursiones.API.Controllers;

[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    protected IActionResult Respond<T>(ServiceResult<T> result, bool created = false)
    {
        var response = ApiResponse<T>.FromResult(result);
        if (result.Success)
        {
            return created ? StatusCode(StatusCodes.Status201Created, response) : Ok(response);
        }

        return StatusCode(GetErrorStatus(result), response);
    }

    protected IActionResult Respond(ServiceResult result)
    {
        var response = ApiResponse<object>.FromResult(result);
        return result.Success ? Ok(response) : StatusCode(GetErrorStatus(result), response);
    }

    private static int GetErrorStatus(ServiceResult result)
    {
        if (result.Errors.Any(e => e.StartsWith("No se encontró")))
        {
            return StatusCodes.Status404NotFound;
        }

        if (result.Errors.Any(e => e.StartsWith("Ya existe") || e.Contains("relacionados") || e.Contains("cupo suficiente")))
        {
            return StatusCodes.Status409Conflict;
        }

        return StatusCodes.Status400BadRequest;
    }
}
