using ErrorOr;

namespace LinkUpBackend.ServiceErrors;

public static class Errors
{
    public static class User
    {
        public static Error InvalidName => Error.Validation(
            code: "User.InvalidName",
            description: $"Name must not be longer than 256 characters"
        );
        public static Error InvalidEmail => Error.Validation(
            code: "User.InvalidEmail",
            description: "Mail must contain @"
        );
        public static Error NotFound => Error.NotFound(
            code: "User.NotFound",
            description: "User not found"
        );
    }
}
