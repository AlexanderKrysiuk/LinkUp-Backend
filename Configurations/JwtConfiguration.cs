using System.ComponentModel.DataAnnotations;

namespace LinkUpBackend.Configurations;

public class JwtConfiguration
{
    public static readonly string SectionName = "Authentication:Jwt";

    [Required]
    [MaxLength(64)]
    public string Issuer { get; set; } = default!;

    [Required]
    [MaxLength(64)]
    public string Audience { get; set; } = default!;

    [Required]
    [MaxLength(256)]
    public string SigningKey { get; set; } = default!;
}