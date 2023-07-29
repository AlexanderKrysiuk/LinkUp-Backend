using ErrorOr;
using LinkUp.Models;
using LinkUp.ServiceErrors;
using LinkUp.Services.Contractors.Interfaces;

namespace LinkUp.Services.Contractors;

public class ContractorService : IContractorService
{
    private static readonly Dictionary<Guid, Contractor> _contractors = new();
    public ErrorOr<Created> CreateContractor(Contractor contractor)
    {
        _contractors.Add(contractor.Id, contractor);

        return Result.Created;
    }

    public ErrorOr<Deleted> DeleteContractor(Guid id)
    {
        _contractors.Remove(id);

        return Result.Deleted;
    }

    public ErrorOr<Contractor> GetContractor(Guid id)
    {
        if (_contractors.TryGetValue(id, out var contractor))
        {
            return contractor;
        }

        return Errors.Contractor.NotFound;
    }

    public ErrorOr<UpsertedContractor> UpsertContractor(Contractor contractor)
    {
        var IsNewlyCreated = !_contractors.ContainsKey(contractor.Id);
        _contractors[contractor.Id] = contractor;

        return new UpsertedContractor(IsNewlyCreated);
    }
}