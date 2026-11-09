namespace Tmobis.Auth;

using System.Security.Claims;
using Tmobis.Business.Core.Models.Users;

public record AuthTokenData(
    IUser User,
    Dictionary<string, string> Payload,
    List<Claim> Claims,
    List<string> Roles)
{
    public void Deconstruct(
        out IUser user,
        out Dictionary<string, string> payload,
        out List<Claim> claims,
        out List<string> roles)
    {
        user = User;
        payload = Payload;
        claims = Claims;
        roles = Roles;
    }
}
