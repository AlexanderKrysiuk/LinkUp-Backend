using System.ComponentModel.DataAnnotations;

namespace LinkUpBackend.DTOs;

/// <summary>
/// User data required for signing in
/// </summary>
public class UserToLoginDTO
{
    [Required]
    [MaxLength(256)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MaxLength(64)]
    public string Password { get; set; } = string.Empty;
}
