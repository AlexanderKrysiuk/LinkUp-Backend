namespace LinkUp.Contracts.Client;

public record UpsertClientRequest(
    string Name,
    string Email,
    string Password
);