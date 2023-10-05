using System.ComponentModel.DataAnnotations;

namespace LinkUpBackend.DTOs;

/// <summary>
/// User details
/// </summary>
public class UserDetailsDTO
{

    public string Username { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;
}
