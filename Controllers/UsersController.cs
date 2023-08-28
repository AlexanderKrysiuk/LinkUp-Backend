using LinkUpBackend.Configurations;
using LinkUpBackend.Domain;
using LinkUpBackend.DTOs;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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

    //private readonly ILogger<UsersController> _logger;

    public UsersController(UserManager<User> userManager, RoleManager<Role> roleManager, SignInManager<User> signInManager, JwtConfiguration jwtConfiguration)
    {
        _userManager = userManager;
        _jwtConfiguration = jwtConfiguration;
        _signInManager = signInManager;
    }


    [HttpOptions("sign-up")]
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


    [HttpOptions("sign-in")]
    //[ResponseCache(CacheProfileName = "NoCache")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> LoginAsync([FromBody] UserToLoginDTO userToLogin)
    {
       var userToLoginResult = await _userManager.FindByEmailAsync(userToLogin.Email);

        if (userToLoginResult != null && await _userManager.CheckPasswordAsync(userToLoginResult, userToLogin.Password))
        {
            var signInResult = await _signInManager.PasswordSignInAsync(userToLoginResult, userToLogin.Password, false, false);

            if (signInResult.Succeeded)
            {
                var claims = new List<Claim> { new Claim(ClaimTypes.Email, userToLoginResult.Email!) };

                var role = await _userManager.GetRolesAsync(userToLoginResult);

                var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfiguration.SigningKey)), SecurityAlgorithms.HmacSha256);

                var jwtObject = new JwtSecurityToken(issuer: _jwtConfiguration.Issuer, audience: _jwtConfiguration.Audience, claims: claims, expires: DateTime.Now.AddDays(1), signingCredentials: signingCredentials);

                var tokenToReturn = new JwtSecurityTokenHandler().WriteToken(jwtObject);

                return Accepted(new { Token = tokenToReturn });
            }
        }
        return Unauthorized(userToLoginResult);
    }


    [HttpOptions("sign-out")]
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

}

