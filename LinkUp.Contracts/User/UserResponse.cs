namespace LinkUp.Contracts.User;

public record UserResponse(
    Guid Id,
    string Name,
    string Email,
    string Password,
    UserType UserType
);