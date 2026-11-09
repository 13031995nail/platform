namespace Tmobis.Auth;

using System.Security.Claims;
using Tmobis.Business.Core.Models.Users;
using Tmobis.Core.Medium.Producers;
using Tmobis.Core.Repositories;

public class TokenAuthenticator(
    IUnitOfWork unitOfWork,
    IMessageDispatcher messageDispatcher,
    ITokenBuilder tokenBuilder)
: ITokenAuthenticator
{
    public async Task<string> Authenticate(
        IUser user, List<string> roles)
    {
        var tokenDataEntries = await messageDispatcher
            .PublishChainWithResult<
                PrepareAuthToken, PrepareAuthTokenResult>(
                new PrepareAuthToken(user));
        await unitOfWork.Commit();
        var tokenData = MakeTokenData(user, tokenDataEntries, roles);
        return await tokenBuilder.GetToken(tokenData);
    }

    private static AuthTokenData MakeTokenData(
        IUser user,
        List<PrepareAuthTokenResult> tokenDataEntries,
        List<string> roles)
    {
        var claims = new List<Claim>();
        var payloads = new Dictionary<string, string>();
        foreach (var entry in tokenDataEntries)
        {
            claims.AddRange(entry.Claims);
            foreach (var (key, value) in entry.Payload)
            {
                payloads[key] = value;
            }
        }
        
        return new (user, payloads, claims, roles);
    }
}
