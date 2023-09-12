using System.ComponentModel.DataAnnotations;

namespace LinkUpBackend.DTOs
{
    public class UserDetailsDTO
    {

        public string Username { get; set; } = string.Empty;
  
        public string Email { get; set; } = string.Empty;
    }
}
