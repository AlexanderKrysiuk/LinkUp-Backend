using LinkUp.Models;
namespace LinkUp.Services.FreeTerms.Interfaces;

public interface IFreeTermService
{
    void CreateFreeTerm(FreeTerm freeTerm);
    FreeTerm GetFreeTerm(Guid id);
}