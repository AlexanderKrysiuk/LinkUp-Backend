using DBTest.Models;
using LinkUp.Data;

AppDbContext db = new AppDbContext();

var users = db.Users.ToList();

foreach (var user in users)
{
    Console.WriteLine(user.Name, user.Email, user.Id, '\n');
}

Console.WriteLine("Done!");
