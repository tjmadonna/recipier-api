using Api.Features.V1.Core.Extensions;
using Api.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Api.Events;

public class AuthEvents : JwtBearerEvents
{
    private readonly JwtSettings _jwtSettings;

    public AuthEvents(JwtSettings jwtSettings)
    {
        _jwtSettings = jwtSettings;
    }

    public override Task TokenValidated(TokenValidatedContext context)
    {
        var principal = context.Principal;
        if (principal == null)
        {
            context.Fail(new SecurityTokenValidationException("Provided token is invalid."));
            return Task.CompletedTask;
        }

        var type = principal.Claims.GetTyp();
        if (type == null || type != _jwtSettings.AccessType)
        {
            context.Fail(new SecurityTokenValidationException("Provided token is invalid."));
            return Task.CompletedTask;
        }

        return Task.CompletedTask;
    }

}