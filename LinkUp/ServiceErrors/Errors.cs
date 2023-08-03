using ErrorOr;

namespace LinkUp.ServiceErrors;

public static class Errors
{
    public static class Contractor
    {
        public static Error InvalidName => Error.Validation(
            code: "Contractor.InvalidName",
            description: $"Name must contain two parts"
        );
        public static Error InvalidEmail => Error.Validation(
            code: "Contractor.InvalidEmail",
            description: "Mail must contain @"
        );
        public static Error NotFound => Error.NotFound(
            code: "Contractor.NotFound",
            description: "Contractor not found"
        );
    }

    public static class Client
    {
        public static Error InvalidName => Error.Validation(
            code: "Client.InvalidName",
            description: $"Name must contain two parts"
        );
        public static Error InvalidEmail => Error.Validation(
            code: "Client.InvalidEmail",
            description: "Mail must contain @"
        );
        public static Error NotFound => Error.NotFound(
            code: "Client.NotFound",
            description: "Client not found"
        );
    }
}