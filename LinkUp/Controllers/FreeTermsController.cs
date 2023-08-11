using LinkUp.Contracts.FreeTerm;
using Microsoft.AspNetCore.Mvc;
namespace LinkUp.Controllers;

[ApiController]
public class FreeTermsController : ControllerBase
{
    [HttpPost("/freeterms")]
    public IActionResult CreateFreeTerm(CreateFreeTermRequest request)
    {
        return Ok(request);
    }

    [HttpGet("/freeterms/{id:guid}")]
    public IActionResult GetFreeTerm(Guid id)
    {
        return Ok(id);
    }

    [HttpPut("/freeterms/{id:guid}")]
    public IActionResult UpsertFreeTerm(Guid id, UpsertFreeTermRequest request)
    {
        return Ok(request);
    }

    [HttpDelete("/freeterms/{id:guid}")]
    public IActionResult DeleteFreeTerm(Guid id)
    {
        return Ok(id);
    }
}