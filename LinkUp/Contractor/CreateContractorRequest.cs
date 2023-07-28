namespace LinkUp.Contracts.Contactor;

public record CreateContractorRequest(
    string Name,
    string Surname,
    string Email,
    string Password
);