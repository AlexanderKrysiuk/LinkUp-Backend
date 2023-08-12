using ErrorOr;
using LinkUp.Models;
using LinkUp.Services.FreeTerms.Interfaces;
using LinkUp.ServiceErrors;

namespace LinkUp.Services.FreeTerms;

public class FreeTermService : IFreeTermService
{
    private static readonly Dictionary<Guid, FreeTerm> _freeTerms = new();
    public void CreateFreeTerm(FreeTerm freeTerm)
    {
        _freeTerms.Add(freeTerm.Id, freeTerm);
    }

    public ErrorOr<FreeTerm> GetFreeTerm(Guid id)
    {
        if(_freeTerms.TryGetValue(id, out var freeTerm))
        {
            return freeTerm;
        }
        return Errors.FreeTerm.NotFound;
    }

    public void UpsertFreeTerm(FreeTerm freeTerm)
    {
        _freeTerms[freeTerm.Id] = freeTerm;
    }

    public void DeleteFreeTerm(Guid id)
    {
        _freeTerms.Remove(id);
    }
}