using ErrorOr;
using LinkUp.Infrastructure.Data;
using LinkUp.Models;
using LinkUp.ServiceErrors;
using LinkUp.Services.Contractors.Interfaces;
using static LinkUp.ServiceErrors.Errors;
using Contractor = LinkUp.Models.Contractor;

namespace LinkUp.Services.Contractors;

public class ContractorService : IContractorService
{
    private readonly AppDbContext _db;

    public ContractorService(AppDbContext db)
    {
        _db = db;
    }

    public ErrorOr<Created> CreateContractor(Contractor contractor)
    {
        _db.Contractors.Add(contractor);
        _db.SaveChanges();

        return Result.Created;
    }

    public ErrorOr<Deleted> DeleteContractor(Guid id)
    {
        var contractorFromDB = _db.Contractors.Find(id);
        if (contractorFromDB != null)
        {
            _db.Contractors.Remove(contractorFromDB);
            _db.SaveChanges();
            return Result.Deleted;
        }

        return Errors.Contractor.NotFound;
    }

    public ErrorOr<Contractor> GetContractor(Guid id)
    {
        var contractorFromDB = _db.Contractors.Find(id);
        if (contractorFromDB != null)
        {
            return contractorFromDB;
        }

        return Errors.Contractor.NotFound;
    }

    public ErrorOr<UpsertedContractor> UpsertContractor(Contractor contractor)
    {
        var contractorFromDBToUpsert = _db.Contractors.Find(contractor.Id);

        var IsNewlyCreated = contractorFromDBToUpsert == null;

        if (IsNewlyCreated)
        {
            _db.Contractors.Add(contractor);
        }
        else
        {
            contractorFromDBToUpsert.Name = contractor.Name;
            contractorFromDBToUpsert.Email = contractor.Email;
            contractorFromDBToUpsert.Password = contractor.Password;
        }
        _db.SaveChanges();

        return new UpsertedContractor(IsNewlyCreated);
    }
}