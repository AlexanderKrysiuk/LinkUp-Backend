using ErrorOr;
using LinkUp.Models;

namespace LinkUp.Services.Contractors.Interfaces;

public interface IContractorService
{
    ErrorOr<Created> CreateContractor(Contractor contractor);
    ErrorOr<Deleted> DeleteContractor(Guid id);
    ErrorOr<Contractor> GetContractor(Guid id);
    ErrorOr<UpsertedContractor> UpsertContractor(Contractor contractor);
}