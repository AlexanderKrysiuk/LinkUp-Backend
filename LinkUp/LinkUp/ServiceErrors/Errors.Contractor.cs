using ErrorOr;

namespace LinkUp.ServiceErrors;

public static class Errors
{
    public static class Contractor
    {
        public static Error NotFound => Error.NotFound(
            code: "Contractor.NotFound",
            description: "Contractor not found"
        );
    }
}