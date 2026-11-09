namespace Tmobis.Auth.Password.Verifiers;

using Tmobis.Business.Core.Models.Users;
using Tmobis.Core.Passwords;

public class PasswordHashUserVerifier(
    IPasswordHasher passwordHasher) : IUserVerifier
{
  public ValueTask<bool> Verify(IUser user, string password)
    => ValueTask.FromResult(passwordHasher.VerifyPassword(user.UserPwd, password));
}