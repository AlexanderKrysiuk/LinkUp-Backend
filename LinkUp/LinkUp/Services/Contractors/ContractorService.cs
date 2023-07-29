using ErrorOr;
using LinkUp.Models;
using LinkUp.ServiceErrors;
using LinkUp.Services.Contractors.Interfaces;

namespace LinkUp.Services.Contractors;

public class ContractorService : IContractorService
{
    private static readonly Dictionary<Guid, Contractor> _contractors = new();
    public void CreateContractor(Contractor contractor)
    {
        _contractors.Add(contractor.Id, contractor);
    }

    public void DeleteContractor(Guid id)
    {
        _contractors.Remove(id);
    }

    public ErrorOr<Contractor> GetContractor(Guid id)
    {
        if (_contractors.TryGetValue(id, out var contractor))
        {
            return contractor;
        }

        return Errors.Contractor.NotFound;
    }

    public void UpsertContractor(Contractor contractor)
    {
        _contractors[contractor.Id] = contractor;
    }
}