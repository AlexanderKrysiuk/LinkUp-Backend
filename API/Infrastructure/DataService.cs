using API.Domain;

namespace API.Infrastructure
{
    public class DataService
    {
        public List<IUser> Users { get; }
        public DataService() {
            Users = new List<IUser>() { new Provider() { Id = 1, Email="provider@provide.com", Login="Provider", Password="Password123$" }, new Client() { Id=2, Email = "client@use.com", Login = "Client", Password = "Password123$" } };
        }
    }
}
