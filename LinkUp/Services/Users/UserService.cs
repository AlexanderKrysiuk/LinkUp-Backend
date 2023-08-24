using ErrorOr;
using LinkUp.Infrastructure.Data;
using LinkUp.Models;
using LinkUp.ServiceErrors;
using LinkUp.Services.Users;
using LinkUp.Services.Users.Interfaces;

namespace LinkUp.Services.Clients;

public class UserService : IUserService
{
    private readonly AppDbContext _db;

    public UserService(AppDbContext db)
    {
        _db = db;
    }

    public ErrorOr<Created> CreateUser(User user)
    {
        //_db.Users.Add(user);
        //_db.SaveChanges();

        return Result.Created;
    }

    public ErrorOr<Deleted> DeleteUser(Guid id)
    {
       // var userFromDB = _db.Users.Find(id);
        //if (userFromDB != null)
        //{
            //_db.Users.Remove(userFromDB);
            //_db.SaveChanges();
            //return Result.Deleted;
        //}

        return Errors.User.NotFound;
    }

    public ErrorOr<User> GetUser(Guid id)
    {
        //var userFromDB = _db.Users.Find(id);
        //if (userFromDB != null)
        //{
        //    return userFromDB;
        //}

        return Errors.User.NotFound;
    }

    public ErrorOr<UpsertedUser> UpsertUser(User user)
    {
        //var userFromDBToUpsert = _db.Users.Find(user.Id);

        //var IsNewlyCreated = userFromDBToUpsert == null;
        var IsNewlyCreated = true;
        //if (IsNewlyCreated)
        //{
        //    _db.Users.Add(user); 
        //}
        //else
        //{
        //    userFromDBToUpsert.Name = user.Name;
        //    userFromDBToUpsert.Email = user.Email;
        //    userFromDBToUpsert.Password = user.Password;
        //}
        //_db.SaveChanges();

        //return new UpsertedUser(IsNewlyCreated);
        return new UpsertedUser(IsNewlyCreated);
    }
}