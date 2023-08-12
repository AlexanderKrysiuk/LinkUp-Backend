using ErrorOr;
using LinkUp.Models;
namespace LinkUp.Services.FreeTerms.Interfaces;

public interface IFreeTermService
{
    void CreateFreeTerm(FreeTerm freeTerm);
    ErrorOr<FreeTerm> GetFreeTerm(Guid id);
    void UpsertFreeTerm(FreeTerm freeTerm);
    void DeleteFreeTerm(Guid id);
}