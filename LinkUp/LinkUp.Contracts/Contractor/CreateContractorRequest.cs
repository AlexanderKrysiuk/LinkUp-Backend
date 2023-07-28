namespace LinkUp.Contracts.Contractor;

public record CreateContractorRequest(
    string Name,
    string Email,
    string Password
);