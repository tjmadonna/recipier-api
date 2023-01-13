using Api.Features.V1.Core;
using Microsoft.AspNetCore.Mvc;

namespace Api.Features.V1.Errors;

public class ErrorsController : ControllerBase
{
    [HttpGet(ApiRoutes.Errors)]
    public IActionResult Error(int code)
    {
        object? message = code switch
        {
            401 or 403 => new { Credentials = new string[1] { "Unable to complete action with those credientials." } },
            404 => new { Resource = new string[1] { "Unable to find the requested resource." } },
            _ => new { Resources = new string[1] { "An unknown error occurred." } }
        };

        return new ObjectResult(new FailureResponse(
            Errors: message
        ));
    }
}