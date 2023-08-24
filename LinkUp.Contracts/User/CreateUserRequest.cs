namespace LinkUp.Contracts.User;

public record CreateUserRequest(
    string Name,
    string Email,
    string Password,
    UserType UserType

);