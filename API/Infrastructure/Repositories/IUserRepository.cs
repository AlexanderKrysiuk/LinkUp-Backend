using API.Domain;
using API.DTOs;

namespace API.Infrastructure.Repositories
{
    public interface IUserRepository
    {
        IEnumerable<IUser> GetUsers(string? search);
        IUser? GetUser(int id);
        IUser CreateUser(UserRegistrationDTO user);
        IUser UpdateUser(IUser user);
        IUser DeleteUser(int id);
    }
}
