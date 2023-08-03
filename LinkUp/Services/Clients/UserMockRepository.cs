// using LinkUp.Domain;
// using LinkUp.DTOs;

// namespace LinkUp.Infrastructure.Repositories
// {
//     public class UserMockRepository : IUserRepository
//     {
//         private List<IUser> _users;
//         public UserMockRepository() { 
//             _users = new List<IUser>() { new Provider() { Id = 1, Email = "provider@provide.com", Login = "Provider", Password = "Password123$" }, new Client() { Id = 2, Email = "client@use.com", Login = "Client", Password = "Password123$" } };
//         }

//         public IUser CreateUser(UserRegistrationDTO user)
//         {
//             IUser newUser;
//             if (user.ProviderToken is not null)
//             {
//                 newUser = new Provider() { Login=user.Login, Email=user.Email, Password=user.Password };
//                 newUser.Id= _users.Max(u=>u.Id)+1;
//             }
//             else
//             {
//                 newUser = new Client() { Login = user.Login, Email = user.Email, Password = user.Password };
//                 newUser.Id = _users.Max(u => u.Id) + 1;
//             }
//             _users.Add(newUser);
//             return newUser;
//         }

//         public IUser? GetUser(int id)
//         {
//             return _users.FirstOrDefault(user => user.Id == id);
//         }

//         public IEnumerable<IUser> GetUsers(string? search)
//         {
//             return _users;
//         }

//         IUser IUserRepository.DeleteUser(int id)
//         {
//             throw new NotImplementedException();
//         }

//         IUser IUserRepository.UpdateUser(IUser user)
//         {
//             throw new NotImplementedException();
//         }
//     }
// }
