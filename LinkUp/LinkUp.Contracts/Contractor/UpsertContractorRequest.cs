namespace LinkUp.Contracts.Contractor;

public record UpsertContractorRequest(
    string Name,
    string Email,
    string Password
);