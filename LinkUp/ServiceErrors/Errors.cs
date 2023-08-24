using ErrorOr;

namespace LinkUp.ServiceErrors;

public static class Errors
{
    public static class User
    {
        public static Error InvalidName => Error.Validation(
            code: "User.InvalidName",
            description: $"Name must contain two parts"
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