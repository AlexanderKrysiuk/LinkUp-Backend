using ErrorOr;
using LinkUp.Infrastructure.Data;
using LinkUp.Models;
using LinkUp.ServiceErrors;
using LinkUp.Services.Clients.Interfaces;

namespace LinkUp.Services.Clients;

public class ClientService : IClientService
{
    private static readonly Dictionary<Guid, Client> _clients = new();

    //NOT NEEDED IF IN CONTROLLER
    private readonly AppDbContext _db;

    public ClientService(AppDbContext db)
    {
        _db = db;
    }
    //ENDOF

    public ErrorOr<Created> CreateClient(Client client)
    {
        _clients.Add(client.Id, client);

        //OR IN CONTROLLER
        _db.Clients.Add(client);
        _db.SaveChanges();
        //ENDOF

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

        //OR IN CONTROLLER
        var clientFromDBToUpsert = _db.Clients.Find(id);
        if (clientFromDBToUpsert == null)
        {
            _db.Clients.Add(client);
            _db.SaveChanges();
        }
        else
        {
            clientFromDBToUpsert.Name = client.Name;
            clientFromDBToUpsert.Email = client.Email;
            clientFromDBToUpsert.Password = client.Password;
            _db.SaveChanges();
        }
        //ENDOF

        return new UpsertedClient(IsNewlyCreated);
    }
}