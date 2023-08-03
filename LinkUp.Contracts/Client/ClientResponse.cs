namespace LinkUp.Contracts.Client;

public record ClientResponse(
    Guid Id,
    string Name,
    string Email,
    string Password
);