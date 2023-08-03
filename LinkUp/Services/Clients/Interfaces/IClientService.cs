using ErrorOr;
using LinkUp.Models;

namespace LinkUp.Services.Clients.Interfaces;

public interface IClientService
{
    ErrorOr<Created> CreateClient(Client client);
    ErrorOr<Deleted> DeleteClient(Guid id);
    ErrorOr<Client> GetClient(Guid id);
    ErrorOr<UpsertedClient> UpsertClient(Client client);
}