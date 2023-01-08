using Api.Features.V1.Core;
using Api.Features.V1.User.Requests;
using Api.Features.V1.User.Responses;
using Microsoft.AspNetCore.Mvc;

namespace Api.Features.V1.User;

[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    private readonly ILogger<UserController> _logger;

    public UserController(IUserService userService, ILogger<UserController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    [HttpPost(ApiRoutes.User.Create)]
    public async Task<IActionResult> CreateUserAsync([FromBody] CreateUserRequest request)
    {
        var newUserResult = await _userService.CreateUserAsync(new Models.User
        {
            Email = request.Email,
            Password = request.Password
        });
        if (newUserResult.IsFailure)
            return BadRequest(new FailureResponse(
                Errors: new { Credentials = new string[1] { "Unable to create user with those credentials." } }
            ));

        var newUser = newUserResult.Value;
        return CreatedAtAction(nameof(GetMeAsync), new SuccessResponse<CreateUserResponse>(
            Data: new CreateUserResponse { Id = newUser.Id, Email = newUser.Email }
        ));
    }

    [HttpGet(ApiRoutes.User.Me)]
    public async Task<IActionResult> GetMeAsync()
    {
        var userId = Guid.Empty;
        var userResult = await _userService.GetUserInfoAsync(userId);
        if (userResult.IsFailure)
            return Unauthorized(new FailureResponse(
                Errors: new { Credentials = new string[1] { "Unable to get user with those credentials." } }
            ));

        var user = userResult.Value;
        return Ok(new SuccessResponse<GetUserResponse>(
            Data: new GetUserResponse { Id = user.Id, Email = user.Email }
        ));
    }
}