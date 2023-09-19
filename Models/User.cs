using ErrorOr;
using LinkUpBackend.DTOs;
using LinkUpBackend.ServiceErrors;
using Microsoft.AspNetCore.Identity;
using System.Text.RegularExpressions;


namespace LinkUpBackend.Models
{
    public class User : IdentityUser
    {
        public static ErrorOr<User> Create(UserToRegisterDTO userToRegister)
        {
            List<Error> errors = new();
            if (!IsTwoPartName(userToRegister.Username))
            {
                errors.Add(Errors.User.InvalidName);
            }
            if (!IsEmail(userToRegister.Email))
            {
                errors.Add(Errors.User.InvalidEmail);
            }
            if (!isPasswordValid(userToRegister.Password))
            {
                errors.Add(Errors.User.InvalidPassword);
            }
            if (errors.Count > 0)
            {
                return errors;
            }
            return new User() { Email = userToRegister.Email, UserName = userToRegister.Username };

        }

        static bool IsTwoPartName(string name)
        {
            return name.Count(a => a == ' ') == 1;
        }

        static bool isPasswordValid(string password)
        {
            return Regex.IsMatch(password, @"^(?=.*\d)(?=.*[A-Z])(?=.*[\W_]).{8,}$");
        }

        static bool IsEmail(string email)
        {
            return Regex.IsMatch(email, @"^[a-zA-Z0-9]+([.\-_]?[a-zA-Z0-9])*@[a-zA-Z0-9]+(-?[a-zA-Z0-9])*\.[a-zA-Z]{2,}$");
        }
    }
}
