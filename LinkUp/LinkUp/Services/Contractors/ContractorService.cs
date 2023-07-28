using LinkUp.Models;
using LinkUp.Services.Contractors.Interfaces;

namespace LinkUp.Services.Contractors;

public class ContractorService : IContractorService
{
    private readonly Dictionary<Guid, Contractor> _contractors = new ();
    public void CreateContractor(Contractor contractor)
    {
        _contractors.Add(contractor.Id, contractor);
    }

    public Contractor GetContractor(Guid id)
    {
        return _contractors[id];
    }
}