using ErrorOr;
using LinkUp.ServiceErrors;
using static LinkUp.ServiceErrors.Errors;

namespace LinkUp.Models;

public class Contractor
{
    public Guid Id {get;}
    public string Name {get;}
    public string Email {get;}
    public string Password {get;}

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
}