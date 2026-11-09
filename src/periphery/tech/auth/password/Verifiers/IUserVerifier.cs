namespace Tmobis.Auth.Password.Verifiers;

using Tmobis.Business.Core.Models.Users;

public interface IUserVerifier
{
    ValueTask<bool> Verify(IUser user, string password);
}