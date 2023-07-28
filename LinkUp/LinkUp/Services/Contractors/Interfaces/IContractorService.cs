using LinkUp.Contracts.Contractor;
using LinkUp.Models;

namespace LinkUp.Services.Contractors.Interfaces;

public interface IContractorService
{
    void CreateContractor(Contractor contractor);
    Contractor GetContractor(Guid id);
}