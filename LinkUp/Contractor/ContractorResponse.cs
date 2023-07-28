namespace LinkUp.Contracts.Contactor;

public record ContractorResponse(
    Guid Id,
    string Name,
    string Surname,
    string Email,
    string Password
);