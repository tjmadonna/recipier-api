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
            return Unauthorized(new FailureResponse(
                Errors: new { Credentials = new string[1] { "Unable to sign in with those credentials." } }
            ));

        var userId = authResult.Value;
        var accessToken = _authService.CreateAccessToken(userId);
        var refreshToken = await _authService.CreateRefreshTokenAsync(userId);
        if (accessToken.IsFailure || refreshToken.IsFailure)
            return Unauthorized(new FailureResponse(
                Errors: new { Credentials = new string[1] { "Unable to sign in with those credentials." } }
            ));

        return Ok(new SuccessResponse<SignInResponse>(
            Data: new SignInResponse { Access = accessToken.Value, Refresh = refreshToken.Value }
        ));
    }

    [HttpPost(ApiRoutes.Auth.RefreshToken)]
    public IActionResult RefreshToken([FromBody] RefreshTokenRequest request)
    {
        var validateResult = _authService.ValidateRefreshToken(request.Refresh);
        if (validateResult.IsFailure)
            return Unauthorized(new FailureResponse(
                Errors: new { Credentials = new string[1] { "Unable to refresh token with those credentials." } }
            ));

        var userId = validateResult.Value;
        var accessToken = _authService.CreateAccessToken(userId);
        if (accessToken.IsFailure)
            return Unauthorized(new FailureResponse(
                Errors: new { Credentials = new string[1] { "Unable to refresh token with those credentials." } }
            ));

        return Ok(new SuccessResponse<RefreshTokenResponse>(
            Data: new RefreshTokenResponse { Access = accessToken.Value }
        ));
    }

    [HttpPost(ApiRoutes.Auth.SignOut)]
    public async Task<IActionResult> SignOutAsync([FromBody] SignOutRequest request)
    {
        var validateResult = await _authService.DeleteRefreshTokenAsync(request.Refresh);
        if (validateResult.IsFailure)
            return Unauthorized(new FailureResponse(
                Errors: new { Credentials = new string[1] { "Unable to sign out with those credentials." } }
            ));

        return Ok(new SuccessResponse<string>(
            Data: "Signed out successfully."
        ));
    }
}