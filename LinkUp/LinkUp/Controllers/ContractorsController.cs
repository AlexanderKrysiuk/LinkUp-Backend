using ErrorOr;
using LinkUp.Contollers;
using LinkUp.Contracts.Contractor;
using LinkUp.Models;
using LinkUp.ServiceErrors;
using LinkUp.Services.Contractors;
using LinkUp.Services.Contractors.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LinkUp.Controllers;

public class ContractorsController : ApiController
{
    private readonly IContractorService _contractorService;

    public ContractorsController(IContractorService contractorService)
    {
        _contractorService = contractorService;
    }

    [HttpPost]
    public IActionResult CreateContractor(CreateContractorRequest request)
    {
        ErrorOr<Contractor> requestToContractorResult = Contractor.Create(
            request.Name, 
            request.Email, 
            request.Password
        );

        if(requestToContractorResult.IsError)
        {
            return Problem(requestToContractorResult.Errors);
        }

        var contractor = requestToContractorResult.Value;
        // TODO: save contractor to database
        ErrorOr<Created> createContratorResult = _contractorService.CreateContractor(contractor);

        return createContratorResult.Match(
            created => CreatedAtGetContractor(contractor),
            errors => Problem(errors)
        );
    }

    [HttpGet("{id:guid}")]
    public IActionResult GetContractor(Guid id)
    {
        ErrorOr<Contractor> getContractorResult = _contractorService.GetContractor(id);

        return getContractorResult.Match(
            contractor => Ok(MapContractorResponse(contractor)),
            errors => Problem(errors)
        );
    }

    [HttpPut("{id:guid}")]
    public IActionResult UpsertContractor(Guid id, UpsertContractorRequest request)
    {
        ErrorOr<Contractor> requestToContractorResult = Contractor.Create(
            request.Name,
            request.Email,
            request.Password,
            id
        );

        if (requestToContractorResult.IsError)
        {
            return Problem(requestToContractorResult.Errors);
        }

        var contractor = requestToContractorResult.Value;
        ErrorOr<UpsertedContractor> upsertContractorResult = _contractorService.UpsertContractor(contractor);

        // TODO: return 201 if a new contractor was created
        return upsertContractorResult.Match(
            upserted => upserted.IsNewlyCreated ? CreatedAtGetContractor(contractor) : NoContent(),
            errors => Problem(errors)
        );
    }

    [HttpDelete("{id:guid}")]
    public IActionResult DeleteContractor(Guid id)
    {
        ErrorOr<Deleted> deletedContractorResult = _contractorService.DeleteContractor(id);
        return deletedContractorResult.Match(
            deleted => NoContent(),
            errors => Problem(errors)
        );
    }

    private static ContractorResponse MapContractorResponse(Contractor contractor)
    {
        return new ContractorResponse(
                    contractor.Id,
                    contractor.Name,
                    contractor.Email,
                    contractor.Password
                );
    }

    private CreatedAtActionResult CreatedAtGetContractor(Contractor contractor)
    {
        return CreatedAtAction(
            actionName: nameof(GetContractor),
            routeValues: new { id = contractor.Id },
            value: MapContractorResponse(contractor)
        );
    }
}