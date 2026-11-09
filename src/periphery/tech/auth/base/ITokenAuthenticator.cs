namespace Tmobis.Auth;

using Tmobis.Business.Core.Models.Users;

public interface ITokenAuthenticator
{
    Task<string> Authenticate(
        IUser user, List<string> roles);
}
