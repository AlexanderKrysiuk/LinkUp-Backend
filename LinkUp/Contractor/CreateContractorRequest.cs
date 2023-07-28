namespace LinkUp.Contracts.Contactor;

public record CreateContractorRequest(
    string Name,
    string Email,
    string Password
);