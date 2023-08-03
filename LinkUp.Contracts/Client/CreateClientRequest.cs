namespace LinkUp.Contracts.Client;

public record CreateClientRequest(
    string Name,
    string Email,
    string Password
);