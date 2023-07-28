namespace LinkUp.Contracts.Contactor;

public record UpsertContractorRequest(
    string Name,
    string Email,
    string Password
);