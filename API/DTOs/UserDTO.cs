using API.Domain;

namespace API.DTOs
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string Login { get; set; } = string.Empty;
        public string UserType { get; set; } = string.Empty;
        public static string DetermineUserType(IUser user) {
            return user is Client ? typeof(Client).FullName : user is Provider ? typeof(Provider).FullName : "Not Detected";
        }
    }
}
