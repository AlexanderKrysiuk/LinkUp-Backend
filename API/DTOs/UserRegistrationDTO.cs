using API.Domain;

namespace API.DTOs
{
    public class UserRegistrationDTO
    {
        public string Login { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? ProviderToken { get; set; }  = null;
    }
}
