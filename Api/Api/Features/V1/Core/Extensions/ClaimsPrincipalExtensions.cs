using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Api.Features.V1.Core.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Guid? GetSub(this ClaimsPrincipal claimsPrincipal)
    {
        var guidStr = claimsPrincipal.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Sub).Value;
        if (guidStr == null || !Guid.TryParse(guidStr, out Guid guid))
            return null;
        return guid;
    }
}