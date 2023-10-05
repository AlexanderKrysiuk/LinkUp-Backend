using Microsoft.AspNetCore.Mvc;

namespace LinkUpBackend.Controllers;

public class ErrorController : ApiController
{
    /// <summary>
    /// Handles errors and returns an appropriate response
    /// </summary>
    /// <remarks>
    /// This endpoint is used to handle errors and return the appropriate HTTP response based on the error type.
    /// </remarks>
    /// <response code="400">Bad Request: Request parameter(s) is/are not valid</response>
    /// <response code="404">Not Found: Resource in request has not been found</response>
    /// <response code="409">Conflict: Conflict error, eg. key parameter already in use</response>
    /// <response code="500">Internal Server Error: Unexpected server-side error</response>
    [HttpGet("/error")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult Error()
    {
        return Problem();
    }
}
