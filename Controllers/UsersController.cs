using LinkUpBackend.Configurations;
using LinkUpBackend.DTOs;
using LinkUpBackend.Models;
using LinkUpBackend.ServiceErrors;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
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

    private readonly IConfiguration _configuration;

    //private readonly ILogger<UsersController> _logger;

    public UsersController(UserManager<User> userManager, SignInManager<User> signInManager, IOptions<JwtConfiguration> jwtConfiguration, IConfiguration configuration)
    {
        _userManager = userManager;
        _jwtConfiguration = jwtConfiguration.Value;
        _signInManager = signInManager; 
       _configuration = configuration;
    }

    /// <summary>
    /// Signs up a new user
    /// </summary>
    /// <remarks>
    /// Sample request:
    /// 
    ///     OPTIONS api/register
    ///     {        
    ///       "username": "John Doe",
    ///       "email": "jdoe@gmail.com",
    ///       "password": "P@ssw0rd",
    ///       "role": "Contractor"
    ///     }
    /// </remarks>
    /// <param name="userToRegister"></param>
    /// <returns>A status code indicating the result of the operation.</returns>
    /// <response code="202">Request is accepted and further processed</response>
    /// <response code="400">Request parameters do not meet expected ones</response>
    /// <response code="409">Email has been in use</response>

    [HttpOptions("register")]
    //[ResponseCache(CacheProfileName = "NoCache")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> RegisterAsync([FromBody] UserToRegisterDTO userToRegister)
    {
        var errorOrUser = Models.User.Create(userToRegister);
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

    /// <summary>
    /// Signs user in
    /// </summary>
    /// <remarks>
    /// Sample request:
    /// 
    ///     OPTIONS api/login
    ///     {        
    ///       "email": "jdoe@gmail.com",
    ///       "password": "P@ssw0rd",
    ///     }
    /// </remarks>
    /// <param name="userToLogin"></param>
    /// <returns>A status code indicating the result of the operation or token in case of success</returns>
    /// <response code="202">Request is accepted and further processed</response>
    /// <response code="401">User has not been registered</response>
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

            // Pobierz role użytkownika
            var role = await _userManager.GetRolesAsync(userToLoginResult);
            var claims = new[]{ 
                new Claim(JwtRegisteredClaimNames.Sub, userToLoginResult.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, userToLoginResult.Id),
                new Claim(ClaimTypes.Role, role[0])
            };
            
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.Now.AddDays(1),
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

    /// <summary>
    /// Signs user out
    /// </summary>
    /// <returns>A status code indicating the result of the operation.</returns>
    /// <response code="202">Request is accepted and further processed</response>
    /// <response code="401">User has not been authorized for this action</response>
    [HttpOptions("logout")]
    //[ResponseCache(CacheProfileName = "NoCache")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return Accepted();
    }

    /// <summary>
    /// Handles access denied
    /// </summary>
    /// <remarks>
    /// This method handles HTTP GET requests at the "/access-denied" path.
    /// It is used to deny access to specific resources. 
    /// If the client lacks the necessary permissions or is unauthenticated, 
    /// it returns an HTTP status code of 403 Forbidden.
    /// </remarks>
    /// <returns>A status code indicating the result of the operation.</returns>

    [HttpGet("access-denied")]
    //[ResponseCache(CacheProfileName = "NoCache")]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [AllowAnonymous]
    public IActionResult AccessDenied()
    {
        return Forbid();
    }

    /// <summary>
    /// Gets the list of all contractors
    /// </summary>
    /// <returns>All contractors' details</returns>
    /// <response code="200">Returns a list of contractors</response>
    /// <response code="401">User has not been authorized for this action</response>
    [HttpGet("contractors")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [AllowAnonymous]
    public async Task<IActionResult> GetAllContractors(){

        var contractors = await _userManager.GetUsersInRoleAsync("Contractor");
        var contractorsInfo = contractors.Select(user => new{
            UserName = user.UserName,
            Email = user.Email
            });
        return Ok(contractorsInfo);
    }

    /// <summary>
    /// Gets user role
    /// </summary>
    /// <returns>User role</returns>
    /// <response code="200">Returns user role</response>
    /// <response code="401">User has not been authorized for this action</response>
    /// <response code="404">No role has been assigned to the user</response>
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpGet("user-role")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserRole(){
        var userEmail = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!string.IsNullOrEmpty(userEmail)){
            var user = await _userManager.FindByEmailAsync(userEmail);
            var userRole = await _userManager.GetRolesAsync(user);
            switch(userRole){
                case { } when userRole.Contains("Admin"):
                    return Ok("Admin");
                case { } when userRole.Contains("Contractor"):
                    return Ok("Contractor");
                case { } when userRole.Contains("Client"):
                    return Ok("Client");
                default:
                    return NotFound();
            }
        }else{
        return Unauthorized("User is not logged.");
        }
    }

    /// <summary>Gets user details</summary>
    /// <returns>User details</returns>
    /// <response code="200">Returns user details</response>
    /// <response code="401">User has not been authorized for this action</response>
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpGet("user-details")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetUserDetails()
    {
        var userEmail = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!string.IsNullOrEmpty(userEmail))
        {
            var user = await _userManager.FindByEmailAsync(userEmail);
            UserDetailsDTO userDetails = new UserDetailsDTO { Username = user.UserName, Email = user.Email };
            return Ok(userDetails);
        }
        return Unauthorized("User is not logged.");
    }

    /// <summary>
    /// Uploads user's profile picture to photo storage
    /// </summary>
    /// <remarks>
    /// Sample request:
    /// 
    ///     POST api/user-photo
    ///     {        
    ///       [Attach a JPG/JPEG file, example URL for testing: https://picsum.photos/200/300]
    ///     }
    /// </remarks>
    /// <param name="profilePicture">The user's profile picture to upload</param>
    /// <returns>A status code indicating the result of the operation</returns>
    /// <response code="200">Picture has been uploaded successfully</response>
    /// <response code="400">Picture format does not meet expectations</response>
    /// <response code="401">User has not been authorized for this action</response>
    /// <response code="500">Server side error</response>
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpPost("user-photo")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

    /// <summary>
    /// Gets user's profile picture
    /// </summary>
    /// <returns>A file in .jpg extension</returns>
    /// <response code="200">Returns profile picture</response>
    /// <response code="404">User has not uploaded any picture yet</response>
    /// <response code="500">Server side error</response>
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpGet("user-photo")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

