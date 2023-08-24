using ErrorOr;
using LinkUp.Contollers;
using LinkUp.Contracts.User;
using LinkUp.Models;
using LinkUp.Services.Users;
using LinkUp.Services.Users.Interfaces;
using Microsoft.AspNetCore.Mvc;
//using User = LinkUp.Models.User;

namespace LinkUp.Controllers;

public class UsersController : ApiController
{
    private readonly IUserService _userService;
    
    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost]
    public IActionResult CreateUser(CreateUserRequest request)
    {
        ErrorOr<User> requestToUserResult = Models.User.From(request);

        if (requestToUserResult.IsError)
        {
            return Problem(requestToUserResult.Errors);
        }

        var user = requestToUserResult.Value;

        ErrorOr<Created> createUserResult = _userService.CreateUser(user);

        return createUserResult.Match(
            created => CreatedAtGetUser(user),
            errors => Problem(errors)
        );
    }

    [HttpGet("{id:guid}")]
    public IActionResult GetUser(Guid id)
    {
        ErrorOr<User> getUserResult = _userService.GetUser(id);

        return getUserResult.Match(
            user => Ok(MapUserResponse(user)),
            errors => Problem(errors)
        );
    }

    [HttpPut("{id:guid}")]
    public IActionResult UpsertUser(Guid id, UpsertUserRequest request)
    {
        ErrorOr<User> requestToUserResult = Models.User.From(id, request);

        if (requestToUserResult.IsError)
        {
            return Problem(requestToUserResult.Errors);
        }

        var user = requestToUserResult.Value;

        ErrorOr<UpsertedUser> upsertedUserResult = _userService.UpsertUser(user);

        return upsertedUserResult.Match(
            upserted => upserted.IsNewlyCreated ? CreatedAtGetUser(user) : NoContent(),
            errors => Problem(errors)
        );
    }

    [HttpDelete("{id:guid}")]
    public IActionResult DeleteUser(Guid id)
    {
        ErrorOr<Deleted> deletedUserResult = _userService.DeleteUser(id);
        return deletedUserResult.Match(
            deleted => NoContent(),
            errors => Problem(errors)
        );
    }

    private static UserResponse MapUserResponse(User user)
    {
        return new UserResponse(
                    user.Id,
                    user.Name,
                    user.Email,
                    user.Password,
                    user.UserType
                );
    }

    private CreatedAtActionResult CreatedAtGetUser(User user)
    {
        return CreatedAtAction(
            actionName: nameof(GetUser),
            routeValues: new { id = user.Id },
            value: MapUserResponse(user)
        );
    }
}