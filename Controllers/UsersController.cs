using LinkUpBackend.Configurations;
using LinkUpBackend.Domain;
using LinkUpBackend.DTOs;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LinkUpBackend.Controllers;

[ApiController]
[Route("api")]
[Authorize]
public class UsersController : ControllerBase
{

    private readonly UserManager<User> _userManager;

    private readonly JwtConfiguration _jwtConfiguration;

    private readonly SignInManager<User> _signInManager;

    private readonly IConfiguration _configuration;

    //private readonly ILogger<UsersController> _logger;

    public UsersController(UserManager<User> userManager, SignInManager<User> signInManager, IOptions<JwtConfiguration> jwtConfiguration, IConfiguration configuration)
    {
        _userManager = userManager;
        _jwtConfiguration = jwtConfiguration.Value;
        _signInManager = signInManager; 
       _configuration = configuration;
    }


    [HttpOptions("register")]
    //[ResponseCache(CacheProfileName = "NoCache")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> RegisterAsync([FromBody] UserToRegisterDTO userToRegister)
    {
        var user = new User();
        user.UserName = userToRegister.Username;
        user.Email = userToRegister.Email;
               
        var userRegistrationResult = await _userManager.CreateAsync(user, userToRegister.Password);

        if(!userRegistrationResult.Succeeded)
        {
            throw new Exception(); //TODO: error handling
        }

        var userToRoleResult = await _userManager.AddToRoleAsync(user, userToRegister.Role);

        if(!userToRoleResult.Succeeded)
        {
            throw new Exception(); //TODO: error handling
        }

        return Accepted($"User {user.UserName} has been registered.");
    }


    [HttpOptions("login")]
    //[ResponseCache(CacheProfileName = "NoCache")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> LoginAsync([FromBody] UserToLoginDTO userToLogin)
    {
       var userToLoginResult = await _userManager.FindByEmailAsync(userToLogin.Email);

        if (userToLoginResult != null && await _userManager.CheckPasswordAsync(userToLoginResult, userToLogin.Password))
        {
            var issuer = _configuration["Authentication:Jwt:Issuer"];
            var audience = _configuration["Authentication:Jwt:Audience"];
            var signingKey = _configuration["Authentication:Jwt:SigningKey"];
            
            var claims = new[]{ 
                new Claim(JwtRegisteredClaimNames.Sub, userToLoginResult.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, userToLoginResult.Id)
            };
            
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.Now.AddDays(1), // Ustal czas wygaśnięcia tokenu
                signingCredentials: creds
            );
            var tokenValue = new JwtSecurityTokenHandler().WriteToken(token);
            await _userManager.SetAuthenticationTokenAsync(userToLoginResult, "MyAuthScheme", "JwtToken", tokenValue);
            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiration = token.ValidTo
            });
        }
        return Unauthorized();
    }

    //TODO: fix logout
    [HttpOptions("logout")]
    //[ResponseCache(CacheProfileName = "NoCache")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return Accepted();
    }

    [HttpOptions("access-denied")]
    //[ResponseCache(CacheProfileName = "NoCache")]
    [AllowAnonymous]
    public IActionResult AccessDenied()
    {
        return Forbid();
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpGet("user")]
    public async Task<IActionResult> GetLoggedInUser(){
        /*
        // Pobierz zalogowanego użytkownika na podstawie kontekstu uwierzytelniania
        var user = await _userManager.GetUserIdAsync(User.Id);
        if (user != null){
            // Użytkownik zalogowany
            // Możesz zwrócić cały obiekt użytkownika lub jego właściwości
            return Ok(user);
        }else{
            // Użytkownik nie jest zalogowany (nie znaleziono użytkownika w kontekście uwierzytelniania)
            // Możesz zwrócić odpowiednią odpowiedź, np. Unauthorized
            return Unauthorized("Użytkownik nie jest zalogowany.");
        }
        */
        // Pobierz identyfikator użytkownika z kontekstu uwierzytelniania
    var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    if (!string.IsNullOrEmpty(userId))
    {
        // Użytkownik zalogowany
        // Zwróć identyfikator użytkownika
        return Ok(new { UserId = userId });
    }
    else
    {
        // Użytkownik nie jest zalogowany (brak identyfikatora użytkownika w kontekście uwierzytelniania)
        // Możesz zwrócić odpowiednią odpowiedź, np. Unauthorized
        return Unauthorized("Użytkownik nie jest zalogowany.");
    }
    }   
}

