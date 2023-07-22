using API.Domain;

namespace API.Infrastructure.Repositories
{
    public class UserMockRepository : IUserRepository
    {
        private List<IUser> _users;
        public UserMockRepository() { 
            _users = new List<IUser>() { new Provider() { Id = 1, Email = "provider@provide.com", Login = "Provider", Password = "Password123$" }, new Client() { Id = 2, Email = "client@use.com", Login = "Client", Password = "Password123$" } };
        }
        public void CreateUser(IUser user)
        {
            throw new NotImplementedException();
        }

        public bool DeleteUser(int id)
        {
            throw new NotImplementedException();
        }

        public IUser? GetUser(int id)
        {
            return _users.FirstOrDefault(user => user.Id == id);
        }

        public IEnumerable<IUser> GetUsers(string? search)
        {
            return _users;
        }

        public bool UpdateUser(IUser user)
        {
            throw new NotImplementedException();
        }
    }
}
