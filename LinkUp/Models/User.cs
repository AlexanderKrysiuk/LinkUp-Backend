using ErrorOr;
using LinkUp.Contracts.User;
using LinkUp.ServiceErrors;
using static LinkUp.ServiceErrors.Errors;

namespace LinkUp.Models;

public class User
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public UserType UserType { get; set; }

    private User(Guid Id, string Name, string Email, string Password, UserType UserType)
    {
        this.Id = Id;
        this.Name = Name;
        this.Email = Email;
        this.Password = Password;
        this.UserType = UserType;
    }

    public static ErrorOr<User> Create(string Name, string Email, string Password, UserType UserType, Guid? Id = null)
    {
        List<Error> errors = new();
        if (!IsTwoPartName(Name)){
            errors.Add(Errors.User.InvalidName);
        }
        if (!IsEmail(Email)){
            errors.Add(Errors.User.InvalidEmail);
        }
        if (errors.Count>0){
            return errors;
        }
        return new User(
            Id ?? Guid.NewGuid(),
            Name,
            Email,
            Password,
            UserType
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

    public static ErrorOr<User> From(CreateUserRequest request){
        return Create(
            request.Name,
            request.Email,
            request.Password,
            request.UserType
        );
    }

    public static ErrorOr<User> From(Guid id, UpsertUserRequest request){
        return Create(
            request.Name,
            request.Email,
            request.Password,
            request.UserType,
            id
        );
    }
}