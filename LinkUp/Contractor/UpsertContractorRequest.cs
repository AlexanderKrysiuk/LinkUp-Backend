namespace LinkUp.Contracts.Contactor;

public record UpsertContractorRequest(
    string Name,
    string Surname,
    string Email,
    string Password
);