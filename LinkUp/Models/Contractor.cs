using ErrorOr;
using LinkUp.Contracts.Contractor;
using LinkUp.ServiceErrors;
using static LinkUp.ServiceErrors.Errors;

namespace LinkUp.Models;

public class Contractor
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }

    private Contractor(Guid Id, string Name, string Email, string Password)
    {
        this.Id = Id;
        this.Name = Name;
        this.Email = Email;
        this.Password = Password;
    }

    public static ErrorOr<Contractor> Create(string Name, string Email, string Password, Guid? Id = null)
    {
        List<Error> errors = new();
        if (!IsTwoPartName(Name)){
            errors.Add(Errors.Contractor.InvalidName);
        }
        if (!IsEmail(Email)){
            errors.Add(Errors.Contractor.InvalidEmail);
        }
        if (errors.Count>0){
            return errors;
        }
        return new Contractor(
            Id ?? Guid.NewGuid(),
            Name,
            Email,
            Password
        );
    }

    static bool IsTwoPartName(string name)
    {
        return name.Contains(" ");
    }

    static bool IsEmail(string email)
    {
        return email.Contains("@");
    }

    public static ErrorOr<Contractor> From(CreateContractorRequest request){
        return Create(
            request.Name,
            request.Email,
            request.Password
        );
    }

    public static ErrorOr<Contractor> From(Guid id, UpsertContractorRequest request){
        return Create(
            request.Name,
            request.Email,
            request.Password,
            id
        );
    }
}