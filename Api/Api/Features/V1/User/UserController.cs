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
            return BadRequest(new ApiResponse<CreateUserResponse>(
                Errors: new ErrorResponse[1] {
                    new ErrorResponse("Credentials", new string[1] {"Unable to create user with those credentials."})
                }
            ));

        var newUser = newUserResult.Value;
        return CreatedAtAction(nameof(GetMeAsync), new ApiResponse<CreateUserResponse>(
            Data: new CreateUserResponse { Id = newUser.Id, Email = newUser.Email }
        ));
    }

    [HttpGet(ApiRoutes.User.Me)]
    public async Task<IActionResult> GetMeAsync()
    {
        return Ok();
    }
}