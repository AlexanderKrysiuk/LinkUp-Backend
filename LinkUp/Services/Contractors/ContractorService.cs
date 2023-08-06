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
    private static readonly Dictionary<Guid, Contractor> _contractors = new();

    //NOT NEEDED IF IN CONTROLLER
    private readonly AppDbContext _db;

    public ContractorService(AppDbContext db)
    {
        _db = db;
    }
    //ENDOF

    public ErrorOr<Created> CreateContractor(Contractor contractor)
    {
        //_contractors.Add(contractor.Id, contractor);

        //OR IN CONTROLLER
        _db.Contractors.Add(contractor);
        _db.SaveChanges();
        //ENDOF

        return Result.Created;
    }

    public ErrorOr<Deleted> DeleteContractor(Guid id)
    {
        //_contractors.Remove(id);

        //return Result.Deleted;

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
        //if (_contractors.TryGetValue(id, out var contractor))
        //{
        //    return contractor;
        //}
        var contractorFromDB = _db.Contractors.Find(id);
        if (contractorFromDB != null)
        {
            return contractorFromDB;
        }

        return Errors.Contractor.NotFound;
    }

    public ErrorOr<UpsertedContractor> UpsertContractor(Contractor contractor)
    {
        //var IsNewlyCreated = !_contractors.ContainsKey(contractor.Id);
        //_contractors[contractor.Id] = contractor;

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