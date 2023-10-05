using System.ComponentModel.DataAnnotations;

namespace LinkUpBackend.DTOs;

/// <summary>
/// User details required for signing up.
/// </summary>
public class UserToRegisterDTO
{
    [Required]
    [MaxLength(256)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(256)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MaxLength(64)]
    public string Password { get; set; } = string.Empty;

    [Required]
    public string Role { get; set; } = string.Empty;
}

