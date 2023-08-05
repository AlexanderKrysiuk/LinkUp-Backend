using ErrorOr;
using LinkUp.Contollers;
using LinkUp.Contracts.Client;
using LinkUp.Infrastructure.Data;
using LinkUp.Models;
using LinkUp.ServiceErrors;
using LinkUp.Services.Clients;
using LinkUp.Services.Clients.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LinkUp.Controllers;

public class ClientsController : ApiController
{
    private readonly IClientService _clientService;
    private readonly AppDbContext _db;

    public ClientsController(IClientService clientService, AppDbContext db)
    {
        _clientService = clientService;
        _db = db;
    }

    [HttpPost]
    public IActionResult CreateClient(CreateClientRequest request)
    {
        ErrorOr<Client> requestToClientResult = Client.From(request);

        if(requestToClientResult.IsError)
        {
            return Problem(requestToClientResult.Errors);
        }

        var client = requestToClientResult.Value;

        _db.Clients.Add(client);
        _db.SaveChanges();

        ErrorOr<Created> createContratorResult = _clientService.CreateClient(client);

        return createContratorResult.Match(
            created => CreatedAtGetClient(client),
            errors => Problem(errors)
        );
    }

    [HttpGet("{id:guid}")]
    public IActionResult GetClient(Guid id)
    {
        ErrorOr<Client> getClientResult = _clientService.GetClient(id);

        return getClientResult.Match(
            client => Ok(MapClientResponse(client)),
            errors => Problem(errors)
        );
    }

    [HttpPut("{id:guid}")]
    public IActionResult UpsertClient(Guid id, UpsertClientRequest request)
    {
        ErrorOr<Client> requestToClientResult = Client.From(id, request);

        if (requestToClientResult.IsError)
        {
            return Problem(requestToClientResult.Errors);
        }

        var client = requestToClientResult.Value;
        ErrorOr<UpsertedClient> upsertClientResult = _clientService.UpsertClient(client);

        // TODO: return 201 if a new client was created
        return upsertClientResult.Match(
            upserted => upserted.IsNewlyCreated ? CreatedAtGetClient(client) : NoContent(),
            errors => Problem(errors)
        );
    }

    [HttpDelete("{id:guid}")]
    public IActionResult DeleteClient(Guid id)
    {
        ErrorOr<Deleted> deletedClientResult = _clientService.DeleteClient(id);
        return deletedClientResult.Match(
            deleted => NoContent(),
            errors => Problem(errors)
        );
    }

    private static ClientResponse MapClientResponse(Client client)
    {
        return new ClientResponse(
                    client.Id,
                    client.Name,
                    client.Email,
                    client.Password
                );
    }

    private CreatedAtActionResult CreatedAtGetClient(Client client)
    {
        return CreatedAtAction(
            actionName: nameof(GetClient),
            routeValues: new { id = client.Id },
            value: MapClientResponse(client)
        );
    }
}