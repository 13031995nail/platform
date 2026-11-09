namespace Tmobis.Auth;

public interface ITokenBuilder
{
    Task<string> GetToken(AuthTokenData data);
}
