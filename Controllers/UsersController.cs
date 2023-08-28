
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

    private readonly ILogger<UsersController> _logger;

    public UsersController(UserManager<User> userManager, ILogger<UsersController> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }


    [HttpOptions("register")]
    [ResponseCache(CacheProfileName = "NoCache")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> RegisterAsync([FromBody] UserToRegisterDTO userToRegister)
    {
        var user = new User();
        user.UserName = userToRegister.Username;
        user.Email = userToRegister.Email;
        
        //TODO: nadać rolę! -> userToRegister.Roles

        var userRegistrationResult = await _userManager.CreateAsync(user, userToRegister.Password);

        if(!userRegistrationResult.Succeeded)
        {
            throw new Exception();
        }
        
        return Accepted();
    }


    [HttpOptions("login")]
    [ResponseCache(CacheProfileName = "NoCache")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login()
    {
        return Accepted();
    }


    [HttpOptions("logout")]
    [ResponseCache(CacheProfileName = "NoCache")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Logout()
    {
        return Accepted();
    }

    [HttpOptions("access-denied")]
    [ResponseCache(CacheProfileName = "NoCache")]
    [AllowAnonymous]
    public async Task<IActionResult> AccessDenied()
    {
        return Forbid();
    }

}

