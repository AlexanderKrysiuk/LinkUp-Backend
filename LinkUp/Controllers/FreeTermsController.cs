using LinkUp.Contracts.FreeTerm;
using LinkUp.Models;
using LinkUp.Services.FreeTerms.Interfaces;
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
    public IActionResult CreateFreeTerm(Guid ContractorId, CreateFreeTermRequest request)
    {
        var freeTerm = new FreeTerm(
            Guid.NewGuid(),
            ContractorId,
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
        FreeTerm freeTerm = _freeTermService.GetFreeTerm(id);
        
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
        return Ok(request);
    }

    [HttpDelete("{id:guid}")]
    public IActionResult DeleteFreeTerm(Guid id)
    {
        return Ok(id);
    }
}