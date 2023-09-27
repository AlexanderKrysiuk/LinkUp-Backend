using LinkUpBackend.Configurations;
using LinkUpBackend.DTOs;
using LinkUpBackend.Models;
using LinkUpBackend.ServiceErrors;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Linq;
using LinkUpBackend.Services;
using ErrorOr;

namespace LinkUpBackend.Controllers;

[ApiController]
[Route("api")]
[Authorize]
public class UsersController : ApiController
{
    private readonly UsersService _usersService;
    private readonly UserManager<User> _userManager;

    private readonly JwtConfiguration _jwtConfiguration;

    private readonly SignInManager<User> _signInManager;

    private readonly IConfiguration _configuration;

    //private readonly ILogger<UsersController> _logger;

    public UsersController(UserManager<User> userManager, SignInManager<User> signInManager, IOptions<JwtConfiguration> jwtConfiguration, IConfiguration configuration)
    {
        _usersService = new UsersService(userManager, signInManager, jwtConfiguration.Value, configuration);
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
        ErrorOr<User> errorOrRegisteredUser = await _usersService.RegisterUser(userToRegister);
        if (errorOrRegisteredUser.IsError)
        {
            return Problem(errorOrRegisteredUser.Errors);
        }
        var user = errorOrRegisteredUser.Value;
        return Accepted($"User {user.UserName} has been registered.");
    }


    [HttpOptions("login")]
    //[ResponseCache(CacheProfileName = "NoCache")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> LoginAsync([FromBody] UserToLoginDTO userToLogin)
    {
        ErrorOr<JwtSecurityToken> errorOrToken = await _usersService.GetLoginToken(userToLogin);
        if(errorOrToken.IsError) {
            return Problem(errorOrToken.Errors);
        }
        var token = errorOrToken.Value;
        return Ok(new
        {
            token = new JwtSecurityTokenHandler().WriteToken(token),
            expiration = token.ValidTo
        });
        /*var userToLoginResult = await _userManager.FindByEmailAsync(userToLogin.Email);

         if (userToLoginResult != null && await _userManager.CheckPasswordAsync(userToLoginResult, userToLogin.Password))
         {*/
    }

    [HttpGet("access-denied")]
    //[ResponseCache(CacheProfileName = "NoCache")]
    [AllowAnonymous]
    public IActionResult AccessDenied()
    {
        return Forbid();
    }

    [HttpGet("contractors")]
    [AllowAnonymous]
    public async Task<IActionResult> GetAllContractors(){

        var contractors = await _userManager.GetUsersInRoleAsync("Contractor");
        var contractorsInfo = contractors.Select(user => new{
            UserName = user.UserName,
            Email = user.Email
            });
        return Ok(contractorsInfo);
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpGet("user-role")]
    public async Task<IActionResult> GetLoggedInUserRole(){
        // Pobierz identyfikator użytkownika z kontekstu uwierzytelniania
        var userEmail = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!string.IsNullOrEmpty(userEmail))
        {
            // Użytkownik zalogowany
            // Zwróć identyfikator użytkownika
            var errorOrUserRole = await _usersService.GetUserRoleByEmail(userEmail);
            if (errorOrUserRole.IsError)
            {
                return Problem(errorOrUserRole.Errors);
            }
            string userRole = errorOrUserRole.Value;
            return Ok(userRole);
        }
        else
        {
            // Użytkownik nie jest zalogowany (brak identyfikatora użytkownika w kontekście uwierzytelniania)
            // Możesz zwrócić odpowiednią odpowiedź, np. Unauthorized
            return Unauthorized("Użytkownik nie jest zalogowany.");
        }
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpGet("user-details")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetActionResultAsync()
    {
        var userEmail = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!string.IsNullOrEmpty(userEmail))
        {
            var errorOrUser = await _usersService.GetUserByEmail(userEmail);
            if (errorOrUser.IsError)
            {
                return Problem(errorOrUser.Errors);
            }
            var user = errorOrUser.Value;
            return Ok(new UserDetailsDTO() { Email = user.Email!, Username = user.UserName! });
        }
        return Unauthorized("User is not logged.");
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpPost("user-photo")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UploadProfilePicture(IFormFile profilePicture)
    {
        var userEmail = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var user = await _userManager.FindByEmailAsync(userEmail);

        string storagePath = _configuration["AppSettings:LocalStoragePath"]!;
        var fileName = user.Id + ".jpg"; //or Guid.NewGuid().ToString() + ".jpg";
        var filePath = Path.Combine(storagePath, fileName);

        if (System.IO.File.Exists(filePath))
        {
            System.IO.File.Delete(filePath);
        }

        try
        {
            if (profilePicture != null && profilePicture.Length > 0)
            {
                if (profilePicture.ContentType != "image/jpeg")
                {
                    return BadRequest("Invalid profile picture format. Only JPG/JPEG files are allowed.");
                }

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await profilePicture.CopyToAsync(stream);
                }
                return Ok();
            }
            else
            {
                return BadRequest("Invalid profile picture."); //TODO
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, "An error occurred while processing the picture."); //TODO
        }
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpGet("user-photo")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProfilePicture()
    {
        var userEmail = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var user = await _userManager.FindByEmailAsync(userEmail);

        string storagePath = _configuration["AppSettings:LocalStoragePath"]!;
        var fileName = user.Id + ".jpg"; //or Guid.NewGuid().ToString() + ".jpg";
        var filePath = Path.Combine(storagePath, fileName);

        try
        {
            if (System.IO.File.Exists(filePath))
            {
                var mimeType = "image/jpeg";
                var file = new PhysicalFileResult(filePath, mimeType);
                file.FileDownloadName = user.UserName!.ToString().Replace(" ", "") + ".jpg";

                return file;
            }
            else
            {
                return NotFound("File not found.");
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, "An error occurred while processing the picture.");
        }
    }
}

