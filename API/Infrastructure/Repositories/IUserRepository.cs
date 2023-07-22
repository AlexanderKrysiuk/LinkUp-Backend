using API.Domain;

namespace API.Infrastructure.Repositories
{
    public interface IUserRepository
    {
        IEnumerable<IUser> GetUsers(string? search);
        IUser? GetUser(int id);
        void CreateUser(IUser user);
        bool UpdateUser(IUser user);
        bool DeleteUser(int id);
    }
}
