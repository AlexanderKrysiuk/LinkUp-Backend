namespace LinkUp.Contracts.Contractor;

public record ContractorResponse(
    Guid Id,
    string Name,
    string Email,
    string Password
);