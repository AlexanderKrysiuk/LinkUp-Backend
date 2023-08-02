using LinkUp.Domain;

namespace LinkUp.DTOs
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string Login { get; set; } = string.Empty;
        public string UserType { get; set; } = string.Empty;
        public UserDTO(IUser user) { 
            Id = user.Id;
            Login = user.Login;
            UserType = user is Client ? typeof(Client).ToString() : user is Provider ? typeof(Provider).ToString() : "Not Detected";
        }
    }
}
