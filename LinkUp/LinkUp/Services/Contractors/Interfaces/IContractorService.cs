using LinkUp.Models;

namespace LinkUp.Services.Contractors.Interfaces;

public interface IContractorService
{
    void CreateContractor(Contractor contractor);
    void DeleteContractor(Guid id);
    Contractor GetContractor(Guid id);
    void UpsertContractor(Contractor contractor);
}