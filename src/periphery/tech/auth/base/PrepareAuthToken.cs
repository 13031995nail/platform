namespace Tmobis.Auth;

using System.Security.Claims;
using Tmobis.Business.Core.Models.Users;
using Tmobis.Core.Medium.Models;

public record PrepareAuthToken(IUser User)
: Message<PrepareAuthTokenResult>;

public record PrepareAuthTokenResult(
    List<Claim> Claims,
    Dictionary<string, string> Payload
): IChainMessageResult
{
    public void Deconstruct(
        out List<Claim> claims,
        out Dictionary<string, string> payload
    )
    {
        claims = Claims;
        payload = Payload;
    }

    public bool Continue { get; init; }
}