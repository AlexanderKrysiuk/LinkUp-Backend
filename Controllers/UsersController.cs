
using LinkUpBackend.Domain;
using LinkUpBackend.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LinkUpBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
//Authorize
public class UsersController : ControllerBase
{

    private readonly UserManager<User> _userManager;

    private readonly RoleManager<Role> _roleManager;

    private readonly SignInManager<User> _signInManager;

    private readonly ILogger<UsersController> _logger;

    public UsersController(UserManager<User> userManager, ILogger<UsersController> logger, RoleManager<Role> roleManager, SignInManager<User> signInManager)
    {
        _userManager = userManager;
        _logger = logger;
        _roleManager = roleManager;
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
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> LoginAsync([FromBody] UserToLoginDTO userToLogin)
    {
       var userToLoginResult = await _userManager.FindByEmailAsync(userToLogin.Email);

        if (userToLoginResult != null && await _userManager.CheckPasswordAsync(userToLoginResult, userToLogin.Password))
        {
            var signInResult = await _signInManager.PasswordSignInAsync(userToLoginResult, userToLogin.Password, false, false);

            if (signInResult.Succeeded)
            {
                // Generate token or perform other login-related tasks
                // Return success response
                return Ok("Logged in successfully.");
            }
            else
            {
                return BadRequest("Invalid login attempt.");
            }
        }

        return Unauthorized(userToLoginResult);
    }


    [HttpOptions("logout")]
    //[ResponseCache(CacheProfileName = "NoCache")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Logout()
    {
        return Accepted();
    }

    [HttpOptions("access-denied")]
    //[ResponseCache(CacheProfileName = "NoCache")]
    [AllowAnonymous]
    public async Task<IActionResult> AccessDenied()
    {
        return Forbid();
    }

}

