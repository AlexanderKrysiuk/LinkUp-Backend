using ErrorOr;
using LinkUpBackend.DTOs;
using LinkUpBackend.ServiceErrors;
using Microsoft.AspNetCore.Identity;
using System.Text.RegularExpressions;


namespace LinkUpBackend.Models;

/// <summary>
/// Class representing user profile based on Identity
/// </summary>
public class User : IdentityUser
{
    /// <summary>
    /// Creates a new user based on the provided registration data
    /// </summary>
    /// <param name="userToRegister">The registration data for the new user</param>
    /// <returns>
    /// A result that represents either a list of validation errors if the registration data is invalid,
    /// or a User object if the registration is successful
    /// </returns>
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

    /// <summary>
    /// Checks if the provided name consists of two parts separated by a space
    /// </summary>
    /// <param name="name">The name to be checked</param>
    /// <returns>True if the name consists of two parts; otherwise, false</returns>
    static bool IsTwoPartName(string name)
    {
        return name.Count(a => a == ' ') == 1;
    }

    /// <summary>
    /// Validates a password based on the specified criteria
    /// </summary>
    /// <param name="password">The password to be validated.</param>
    /// <returns>True if the password is valid; otherwise, false.</returns>
    static bool isPasswordValid(string password)
    {
        return Regex.IsMatch(password, @"^(?=.*\d)(?=.*[A-Z])(?=.*[\W_]).{8,}$");
    }

    /// <summary>
    /// Check if the provided string is a valid email address.
    /// </summary>
    /// <param name="email">The string to be validated as an email address</param>
    /// <returns>True if the string is a valid email address; otherwise, false</returns>

    static bool IsEmail(string email)
    {
        return Regex.IsMatch(email, @"^[a-zA-Z0-9]+([.\-_]?[a-zA-Z0-9])*@[a-zA-Z0-9]+(-?[a-zA-Z0-9])*\.[a-zA-Z]{2,}$");
    }
}
