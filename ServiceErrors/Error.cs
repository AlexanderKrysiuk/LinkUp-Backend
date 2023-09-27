using ErrorOr;
using Microsoft.AspNetCore.Identity;

namespace LinkUpBackend.ServiceErrors;

public static class Errors
{
    public static class User
    {
        public static Error InvalidPassword => Error.Validation(
            code: "User.InvalidPassword",
            description: $"Password must be at least 8 characters long and contain at least one of each: uppercase letter, lowercase letter, digit, special character"
        );

        public static Error InvalidName => Error.Validation(
            code: "User.InvalidName",
            description: $"Name must only be letters and a space, must not be longer than 256 characters and must be in exactly two parts e: 'Example Example'"
        );
        public static Error InvalidEmail => Error.Validation(
            code: "User.InvalidEmail",
            description: "E-mail address must be valid"
        );
        public static Error NotFound => Error.NotFound(
            code: "User.NotFound",
            description: "User not found"
        );

        
    }
    public static List<ErrorOr.Error> MapIdentityErrorsToErrorOrErrors(IEnumerable<IdentityError> errors)
    {
        return errors.Select(error =>
        {
            if (error.Code == "DuplicateEmail")
            {
                return ErrorOr.Error.Conflict(description: "Email conflict");
            }
            if (error.Code == "DuplicateUserName")
            {
                return ErrorOr.Error.Conflict(description: "Username conflict");
            }
            if (error.Code == "InvalidUserName")
            {
                return ServiceErrors.Errors.User.InvalidName;
            }
            return ErrorOr.Error.Failure(description: error.Code + ": " + error.Description);
        }).ToList();
    }
}
