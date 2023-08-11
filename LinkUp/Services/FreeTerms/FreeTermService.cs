using LinkUp.Models;
using LinkUp.Services.FreeTerms.Interfaces;

namespace LinkUp.Services.FreeTerms;

public class freeTermService : IFreeTermService
{
    private readonly Dictionary<Guid, FreeTerm> _freeTerms = new();
    public void CreateFreeTerm(FreeTerm freeTerm)
    {
        _freeTerms.Add(freeTerm.Id, freeTerm);
    }

    public FreeTerm GetFreeTerm(Guid id)
    {
        return _freeTerms[id];
    }
}