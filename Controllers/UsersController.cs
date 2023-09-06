using LinkUpBackend.Configurations;
using LinkUpBackend.Domain;
using LinkUpBackend.DTOs;
using LinkUpBackend.ServiceErrors;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace LinkUpBackend.Controllers;

[ApiController]
[Route("api")]
[Authorize]
public class UsersController : ApiController
{
    
    private readonly UserManager<User> _userManager;

    private readonly JwtConfiguration _jwtConfiguration;

    private readonly SignInManager<User> _signInManager;

    //private readonly ILogger<UsersController> _logger;

    public UsersController(UserManager<User> userManager, SignInManager<User> signInManager, IOptions<JwtConfiguration> jwtConfiguration)
    {
        _userManager = userManager;
        _jwtConfiguration = jwtConfiguration.Value;
        _signInManager = signInManager;
    }


    [HttpOptions("register")]
    //[ResponseCache(CacheProfileName = "NoCache")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> RegisterAsync([FromBody] UserToRegisterDTO userToRegister)
    {
        var errorOrUser = Domain.User.Create(userToRegister);
        if (errorOrUser.IsError)
        {
            return Problem(errorOrUser.Errors);
        }
        var user = errorOrUser.Value;
        bool hasUserBeenCreated = false;
        List<ErrorOr.Error> errors = new();
        try
        {
            var userRegistrationResult = await _userManager.CreateAsync(user, userToRegister.Password);

            if (!userRegistrationResult.Succeeded)
            {
                errors.AddRange(Errors.MapIdentityErrorsToErrorOrErrors(userRegistrationResult.Errors));
                return Problem(errors);
            }
            hasUserBeenCreated = true;
            var userToRoleResult = await _userManager.AddToRoleAsync(user, userToRegister.Role);

            if (!userToRoleResult.Succeeded)
            {
                var userDeletionResult = await _userManager.DeleteAsync(user);
                if (!userDeletionResult.Succeeded)
                {
                    // TODO: Log cleaning user error here
                }
                errors.AddRange(Errors.MapIdentityErrorsToErrorOrErrors(userToRoleResult.Errors));
                return Problem(errors);
            }
        }
        catch (Exception e)
        {
            if (hasUserBeenCreated)
            {
                var userDeletionResult = await _userManager.DeleteAsync(user);
                if (!userDeletionResult.Succeeded)
                {
                    // TODO: Log cleaning user error here
                }
            }
            errors.Add(ErrorOr.Error.Failure(description:e.Message));
            return Problem(errors);
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
        return Unauthorized($"User {userToLogin.Email} is not authorized.");
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

}

