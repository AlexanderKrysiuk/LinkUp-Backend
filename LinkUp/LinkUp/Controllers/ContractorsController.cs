using LinkUp.Contracts.Contractor;
using LinkUp.Models;
using LinkUp.Services.Contractors.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LinkUp.Controllers;

[ApiController]
[Route("[controller]")]
public class ContractorsController : ControllerBase
{
    private readonly IContractorService _contractorService;

    public ContractorsController(IContractorService contractorService)
    {
        _contractorService = contractorService;
    }

    [HttpPost]
    public IActionResult CreateContractor(CreateContractorRequest request)
    {
        var contractor = new Contractor(
            Guid.NewGuid(),
            request.Name,
            request.Email,
            request.Password
        );
        // TODO: save contractor to database
        _contractorService.CreateContractor(contractor);
        var response = new ContractorResponse(
            contractor.Id,
            contractor.Name,
            contractor.Email,
            contractor.Password
        );
        return CreatedAtAction(
            actionName: nameof(GetContractor),
            routeValues: new {id = contractor.Id},
            value: response
        );
    }

    [HttpGet("{id:guid}")]
    public IActionResult GetContractor(Guid id)
    {
        Contractor contractor = _contractorService.GetContractor(id);

        var response = new ContractorResponse(
            contractor.Id,
            contractor.Name,
            contractor.Email,
            contractor.Password
        );

        return Ok(response);
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