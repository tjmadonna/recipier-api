using Api.Features.V1.Auth.Requests;
using Api.Features.V1.Auth.Responses;
using Api.Features.V1.Core;
using Microsoft.AspNetCore.Mvc;

namespace Api.Features.V1.Auth;

[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    [HttpPost(ApiRoutes.Auth.SignIn)]
    public async Task<IActionResult> SignInAsync([FromBody] SignInRequest request)
    {
        var authResult = await _authService.AuthenticateAsync(request.Email, request.Password);
        if (authResult.IsFailure)
            return Unauthorized(new ApiResponse<SignInResponse>(
                Errors: new ErrorResponse[1] {
                    new ErrorResponse("Credentials", new string[1] {"Unable to sign in with those credentials."})
                }
            ));

        var userId = authResult.Value;
        var accessToken = _authService.CreateAccessToken(userId);
        var refreshToken = await _authService.CreateRefreshTokenAsync(userId);
        if (accessToken.IsFailure || refreshToken.IsFailure)
            return Unauthorized(new ApiResponse<SignInResponse>(
                Errors: new ErrorResponse[1] {
                    new ErrorResponse("Credentials", new string[1] {"Unable to sign in with those credentials."})
                }
            ));

        return Ok(new ApiResponse<SignInResponse>(
            Data: new SignInResponse { Access = accessToken.Value, Refresh = refreshToken.Value }
        ));
    }
}