using API.Domain;
using API.DTOs;
using System.Text.RegularExpressions;

namespace API.utils.validators
{
    public static class UserValidator
    {
        public static bool ValidateEmail(string email) {
            return Regex.IsMatch(email, @"^[a-zA-Z0-9]+([.\-_]?[a-zA-Z0-9])*@[a-zA-Z0-9]+(-?[a-zA-Z0-9])*\.[a-zA-Z]{2,}$");
        }
        public static bool ValidateLogin(string login) {
            return Regex.IsMatch(login, @"^[a-zA-Z0-9]{1}[\w\d]{2,}$");
        }
        public static bool ValidatePassword(string password) {
            return Regex.IsMatch(password, @"^(?=.*\d)(?=.*[A-Z])(?=.*[\W_]).{8,}$");
        }
        public static ValidationResult ValidateRegistrationData(UserRegistrationDTO newUserData, List<IUser> existingUsers)
        {
            if (existingUsers == null)
            {
                return new ValidationResult() { Message = "Couldn't use database", Success = false, Code = 500 };
            }
            if (newUserData == null) {
                return new ValidationResult() { Message="No user data provided", Success=false, Code=400};
            }
            if(newUserData.Email is null || newUserData.Email == string.Empty)
            {
                return new ValidationResult() { Message = "Missing email", Success = false, Code = 400 };
            }
            if (newUserData.Password is null || newUserData.Password == string.Empty)
            {
                return new ValidationResult() { Message = "Missing password", Success = false, Code = 400 };
            }
            if (newUserData.Login is null || newUserData.Login == string.Empty)
            {
                return new ValidationResult() { Message = "Missing login", Success = false, Code = 400 };
            }
            if (!ValidatePassword(newUserData.Password)) {
                return new ValidationResult() { Message = "Improper password, password must be at least 8 character long and have at least one:\n\t- lowercase letter\n\t- uppercase letter\n\t- digit\n\t- special character.", Success = false, Code = 400 };
            }
            if(!ValidateEmail(newUserData.Email))
            {
                return new ValidationResult() { Message = "Improper email format.", Success = false, Code = 400 };
            }
            if (!ValidateLogin(newUserData.Login))
            {
                return new ValidationResult() { Message = "Improper login format. Login must have at least 3 characters and consist of letters, digits and punctuation. First character must be latin letter or arabic digit", Success = false, Code = 400 };
            }
            if (existingUsers.Select(user => user.Email).Contains(newUserData.Email))
            {
                return new ValidationResult() { Message="Email already in use", Success=false, Code = 409};
            }
            if (existingUsers.Select(user => user.Login).Contains(newUserData.Login))
            {
                return new ValidationResult() { Message = "Login already in use", Success = false, Code = 409 };
            }
            // TODO: add validation for provider token
            if(newUserData.ProviderToken is not null)
            {
                return new ValidationResult() { Message = "Provider token has been delivered, but it is improper. Do not send provider token for client user registration\nUse proper token to register as a provider.", Success = false, Code = 401 };
            }
            return new ValidationResult() { Message = string.Empty, Success = true, Code = 201 };
        }

        public static IUser? AuthenticateUser(UserLoginDTO userData, List<IUser> users)
        {
            if (userData is null)
            {
                return null;
            }
            if (users is null) { return null; }
            IUser? foundUser = users.FirstOrDefault(user => user.Email == userData.Email && user.Password== userData.Password);
            return foundUser;
        }
    }
}