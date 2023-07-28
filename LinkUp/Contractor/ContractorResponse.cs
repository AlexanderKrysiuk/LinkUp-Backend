namespace LinkUp.Contracts.Contactor;

public record ContractorResponse(
    Guid Id,
    string Name,
    string Email,
    string Password
);