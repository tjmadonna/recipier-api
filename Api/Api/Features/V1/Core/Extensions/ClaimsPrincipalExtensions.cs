using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Api.Features.V1.Core.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Guid? GetSub(this ClaimsPrincipal claimsPrincipal)
    {
        return claimsPrincipal.Claims.GetSub();
    }

    public static Guid? GetSub(this IEnumerable<Claim> claims)
    {
        var claim = claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sub);
        if (claim == null)
            return null;

        var guidStr = claim.Value;
        if (guidStr == null || !Guid.TryParse(guidStr, out Guid guid))
            return null;
        return guid;
    }

    public static Guid? GetJti(this IEnumerable<Claim> claims)
    {
        var claim = claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti);
        if (claim == null)
            return null;

        var guidStr = claim.Value;
        if (guidStr == null || !Guid.TryParse(guidStr, out Guid guid))
            return null;
        return guid;
    }
}