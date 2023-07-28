using LinkUp.Contracts.Contractor;
using Microsoft.AspNetCore.Mvc;

namespace LinkUp.Controllers;

[ApiController]
[Route("[controller]")]
public class ContractorsController : ControllerBase
{
    [HttpPost()]
    public IActionResult CreateContractor(CreateContractorRequest request)
    {
        return Ok(request);
    }

    [HttpGet("{id:guid}")]
    public IActionResult GetContractor(Guid id)
    {
        return Ok(id);
    }

    [HttpPut("{id:guid}")]
    public IActionResult UpsertContractor(Guid id, UpsertContractorRequest request)
    {
        return Ok(request);
    }

    [HttpDelete("{id:guid}")]
    public IActionResult DeleteContractor(Guid id)
    {
        return Ok(id);
    }

}