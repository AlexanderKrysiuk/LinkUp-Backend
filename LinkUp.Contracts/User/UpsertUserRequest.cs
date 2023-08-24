namespace LinkUp.Contracts.User;

public record UpsertUserRequest(
    string Name,
    string Email,
    string Password,
    UserType UserType
);