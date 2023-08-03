using ErrorOr;
using LinkUp.Models;
using LinkUp.ServiceErrors;
using LinkUp.Services.Clients.Interfaces;

namespace LinkUp.Services.Clients;

public class ClientService : IClientService
{
    private static readonly Dictionary<Guid, Client> _clients = new();
    public ErrorOr<Created> CreateClient(Client client)
    {
        _clients.Add(client.Id, client);

        return Result.Created;
    }

    public ErrorOr<Deleted> DeleteClient(Guid id)
    {
        _clients.Remove(id);

        return Result.Deleted;
    }

    public ErrorOr<Client> GetClient(Guid id)
    {
        if (_clients.TryGetValue(id, out var client))
        {
            return client;
        }

        return Errors.Client.NotFound;
    }

    public ErrorOr<UpsertedClient> UpsertClient(Client client)
    {
        var IsNewlyCreated = !_clients.ContainsKey(client.Id);
        _clients[client.Id] = client;

        return new UpsertedClient(IsNewlyCreated);
    }
}