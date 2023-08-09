using ErrorOr;
using LinkUp.Contracts.Client;
using LinkUp.ServiceErrors;
using static LinkUp.ServiceErrors.Errors;

namespace LinkUp.Models;

public class Client
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }

    private Client(Guid Id, string Name, string Email, string Password)
    {
        this.Id = Id;
        this.Name = Name;
        this.Email = Email;
        this.Password = Password;
    }

    public static ErrorOr<Client> Create(string Name, string Email, string Password, Guid? Id = null)
    {
        List<Error> errors = new();
        if (!IsTwoPartName(Name)){
            errors.Add(Errors.Client.InvalidName);
        }
        if (!IsEmail(Email)){
            errors.Add(Errors.Client.InvalidEmail);
        }
        if (errors.Count>0){
            return errors;
        }
        return new Client(
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

    public static ErrorOr<Client> From(CreateClientRequest request){
        return Create(
            request.Name,
            request.Email,
            request.Password
        );
    }

    public static ErrorOr<Client> From(Guid id, UpsertClientRequest request){
        return Create(
            request.Name,
            request.Email,
            request.Password,
            id
        );
    }
}