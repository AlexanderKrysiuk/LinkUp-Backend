using ErrorOr;
using LinkUp.Contracts.FreeTerm;
using LinkUp.Models;
using LinkUp.Services.FreeTerms.Interfaces;
using LinkUp.ServiceErrors;
using Microsoft.AspNetCore.Mvc;
namespace LinkUp.Controllers;

[ApiController]
[Route("[controller]")]
public class FreeTermsController : ControllerBase
{
    private readonly IFreeTermService _freeTermService;

    public FreeTermsController(IFreeTermService freeTermService)
    {
        _freeTermService = freeTermService;
    }

    [HttpPost]
    public IActionResult CreateFreeTerm(CreateFreeTermRequest request)
    {
        var freeTerm = new FreeTerm(
            Guid.NewGuid(),
            request.ContractorId,
            request.StartDateTime,
            request.EndDateTime
        );

        _freeTermService.CreateFreeTerm(freeTerm);

        //TODO: save freeterm to database
        var respone = new FreeTermResponse(
            freeTerm.Id,
            freeTerm.ContractorId,
            freeTerm.StartDateTime,
            freeTerm.EndDateTime
        );
        return CreatedAtAction(
            nameof(GetFreeTerm),
            routeValues: new { id = freeTerm.Id },
             value: respone);
    }

    [HttpGet("{id:guid}")]
    public IActionResult GetFreeTerm(Guid id)
    {
        ErrorOr<FreeTerm> getFreeTermResult = _freeTermService.GetFreeTerm(id);
        
        if(getFreeTermResult.IsError && getFreeTermResult.FirstError == Errors.FreeTerm.NotFound)
        {
            return NotFound();
        }

        var freeTerm = getFreeTermResult.Value;
       
        var response = new FreeTermResponse(
            freeTerm.Id,
            freeTerm.ContractorId,
            freeTerm.StartDateTime,
            freeTerm.EndDateTime
        );

        return Ok(response);
    }

    [HttpPut("{id:guid}")]
    public IActionResult UpsertFreeTerm(Guid id, UpsertFreeTermRequest request)
    {
        var freeTerm = new FreeTerm(
            id,
            request.ContractorId,
            request.StartDateTime,
            request.EndDateTime
        );

        _freeTermService.UpsertFreeTerm(freeTerm);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public IActionResult DeleteFreeTerm(Guid id)
    {
        _freeTermService.DeleteFreeTerm(id);
        return NoContent();
    }
}