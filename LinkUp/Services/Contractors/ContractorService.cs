using ErrorOr;
using LinkUp.Infrastructure.Data;
using LinkUp.Models;
using LinkUp.ServiceErrors;
using LinkUp.Services.Contractors.Interfaces;

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
        _contractors.Add(contractor.Id, contractor);

        //OR IN CONTROLLER
        _db.Contractors.Add(contractor);
        _db.SaveChanges();
        //ENDOF

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

        //OR IN CONTROLLER
        var contractorFromDBToUpsert = _db.Contractors.Find(contractor.Id);
        if (contractorFromDBToUpsert == null)
        {
            _db.Contractors.Add(contractor);
            _db.SaveChanges();
        }
        else
        {
            contractorFromDBToUpsert.Name = contractor.Name;
            contractorFromDBToUpsert.Email = contractor.Email;
            contractorFromDBToUpsert.Password = contractor.Password;
            _db.SaveChanges();
        }
        //ENDOF

        return new UpsertedContractor(IsNewlyCreated);
    }
}