using ErrorOr;
using LinkUp.Models;

namespace LinkUp.Services.Users.Interfaces;

public interface IUserService
{
    ErrorOr<Created> CreateUser(User user);
    ErrorOr<Deleted> DeleteUser(Guid id);
    ErrorOr<User> GetUser(Guid id);
    ErrorOr<UpsertedUser> UpsertUser(User user);
}