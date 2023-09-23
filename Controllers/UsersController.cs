﻿using LinkUpBackend.Configurations;
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

    [HttpOptions("logout")]
    //[ResponseCache(CacheProfileName = "NoCache")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return Accepted();
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
    public async Task<IActionResult> GetLoggedInUser(){
        // Pobierz identyfikator użytkownika z kontekstu uwierzytelniania
        var userEmail = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!string.IsNullOrEmpty(userEmail)){
            // Użytkownik zalogowany
            // Zwróć identyfikator użytkownika
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
            return Ok(userRole);
        }else{
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
            var user = await _userManager.FindByEmailAsync(userEmail);
            UserDetailsDTO userDetails = new UserDetailsDTO { Username = user.UserName, Email = user.Email };
            return Ok(userDetails);
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

