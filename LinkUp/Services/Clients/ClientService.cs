using ErrorOr;
using LinkUp.Infrastructure.Data;
using LinkUp.Models;
using LinkUp.ServiceErrors;
using LinkUp.Services.Clients.Interfaces;

namespace LinkUp.Services.Clients;

public class ClientService : IClientService
{
    private readonly AppDbContext _db;

    public ClientService(AppDbContext db)
    {
        _db = db;
    }

    public ErrorOr<Created> CreateClient(Client client)
    {
        _db.Clients.Add(client);
        _db.SaveChanges();

        return Result.Created;
    }

    public ErrorOr<Deleted> DeleteClient(Guid id)
    {
        var clientFromDB = _db.Clients.Find(id);
        if (clientFromDB != null)
        {
            _db.Clients.Remove(clientFromDB);
            _db.SaveChanges();
            return Result.Deleted;
        }

        return Errors.Client.NotFound;
    }

    public ErrorOr<Client> GetClient(Guid id)
    {
        var clientFromDB = _db.Clients.Find(id);
        if (clientFromDB != null)
        {
            return clientFromDB;
        }

        return Errors.Client.NotFound;
    }

    public ErrorOr<UpsertedClient> UpsertClient(Client client)
    {
        var clientFromDBToUpsert = _db.Clients.Find(client.Id);

        var IsNewlyCreated = clientFromDBToUpsert == null;

        if (IsNewlyCreated)
        {
            _db.Clients.Add(client); 
        }
        else
        {
            clientFromDBToUpsert.Name = client.Name;
            clientFromDBToUpsert.Email = client.Email;
            clientFromDBToUpsert.Password = client.Password;
        }
        _db.SaveChanges();

        return new UpsertedClient(IsNewlyCreated);
    }
}