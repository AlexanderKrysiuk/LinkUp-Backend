using ErrorOr;
using LinkUpBackend.Configurations;
using LinkUpBackend.DTOs;
using LinkUpBackend.Models;
using LinkUpBackend.ServiceErrors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LinkUpBackend.Services
{
    public class UsersService
    {
        private UserManager<User> _userManager;
        private SignInManager<User> _signInManager;
        private JwtConfiguration _jwtConfiguration;
        private IConfiguration _configuration;
        public UsersService(UserManager<User> userManager, SignInManager<User> signInManager, JwtConfiguration jwtConfiguration, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _jwtConfiguration = jwtConfiguration;
        }
        private async Task<ErrorOr<User>> GetUser(string? id = null, string? email = null)
        {
            try
            {
                User? foundUser = null;
                if (id is not null) {
                    foundUser = await _userManager.FindByIdAsync(id);
                }
                else if(email is not null) { 
                    foundUser = await _userManager.FindByEmailAsync(email);
                }
                if (foundUser == null)
                {
                    return Errors.User.NotFound;
                }
                else
                {
                    return foundUser;
                }
            }
            catch (Exception)
            {
                return Error.Failure(description:"Database error, contact site administrator");
            }
        }

        public async Task<ErrorOr<User>> GetUserById(string id)
        {
            if(id.Length == 0)
            {
                throw new ArgumentException(nameof(id));
            }
            return await GetUser(id: id);
        }

        public async Task<ErrorOr<User>> GetUserByEmail(string email)
        {
            if (email.Length == 0)
            {
                throw new ArgumentException(nameof(email));
            }
            return await GetUser(email: email);
        }

        private async Task<ErrorOr<string>> GetUserRole(string? id = null, string? email = null)
        {
            var errorOrUser = await GetUser(id, email);
            if (errorOrUser.IsError)
            {
                return errorOrUser.Errors;
            }
            var user = errorOrUser.Value;
            var userRole = await _userManager.GetRolesAsync(user);
            switch (userRole)
            {
                case { } when userRole.Contains("Admin"):
                    return "Admin";
                case { } when userRole.Contains("Contractor"):
                    return "Contractor";
                case { } when userRole.Contains("Client"):
                    return "Client";
                default:
                    return Error.NotFound(description: "Role not found");
            }
        }

        public async Task<ErrorOr<string>> GetUserRoleById(string id)
        {
            if (id.Length == 0)
            {
                throw new ArgumentException(nameof(id));
            }
            return await GetUserRole(id: id);
        }

        public async Task<ErrorOr<string>> GetUserRoleByEmail(string email)
        {
            if (email.Length == 0)
            {
                throw new ArgumentException(nameof(email));
            }
            return await GetUserRole(email: email);
        }

        public JwtSecurityToken GenerateToken(User user, string userRole)
        {
            var issuer = _configuration["Authentication:Jwt:Issuer"];
            var audience = _configuration["Authentication:Jwt:Audience"];
            var signingKey = _configuration["Authentication:Jwt:SigningKey"];

            // Pobierz role użytkownika
            var claims = new[]{
                new Claim(JwtRegisteredClaimNames.Sub, user.Email!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Role, userRole)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.Now.AddDays(1), // Ustal czas wygaśnięcia tokenu
                signingCredentials: creds
            );
            return token;
        }

        public async Task WriteTokenToUser(JwtSecurityToken token, User user)
        {
            var tokenValue = new JwtSecurityTokenHandler().WriteToken(token);
            await _userManager.SetAuthenticationTokenAsync(user, "MyAuthScheme", "JwtToken", tokenValue);
        }

        public async Task<ErrorOr<JwtSecurityToken>> GetLoginToken(UserToLoginDTO userToLogin)
        {
            var errorOrUser = await GetUserByEmail(userToLogin.Email);
            if (errorOrUser.IsError)
            {
                return errorOrUser.Errors;
            }
            var user = errorOrUser.Value;


            if (!await _userManager.CheckPasswordAsync(user, userToLogin.Password)){
                return Errors.User.WrongPassword;
            }

            var errorOrRole = await GetUserRoleById(user.Id);
            if (errorOrRole.IsError)
            {
                return errorOrRole.Errors;
            }
            string userRole = errorOrRole.Value;
            JwtSecurityToken token = GenerateToken(user, userRole);
            await WriteTokenToUser(token, user);
            return token;
        }
    }
}
