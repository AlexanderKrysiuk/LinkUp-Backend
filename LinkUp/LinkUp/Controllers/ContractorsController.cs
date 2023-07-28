using LinkUp.Contracts.Contractor;
using Microsoft.AspNetCore.Mvc;

namespace LinkUp.Controllers;

[ApiController]
public class ContractorsController : ControllerBase
{
    [HttpPost("/contractors")]
    public IActionResult CreateContractor(CreateContractorRequest request)
    {
        return Ok(request);
    }

    [HttpGet("/contractors/{id:guid}")]
    public IActionResult GetContractor(Guid id)
    {
        return Ok(id);
    }

    [HttpPut("/contractors/{id:guid}")]
    public IActionResult UpsertContractor(Guid id, UpsertContractorRequest request)
    {
        return Ok(request);
    }

    [HttpDelete("/contractors/{id:guid}")]
    public IActionResult DeleteContractor(Guid id)
    {
        return Ok(id);
    }

}