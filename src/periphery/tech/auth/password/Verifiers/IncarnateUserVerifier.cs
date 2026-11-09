namespace Tmobis.Auth.Password.Verifiers;

using System.Threading.Tasks;
using Tmobis.Business.Core.Models.Users;
using Tmobis.Business.Core.Providers.Users;

public class IncarnateUserVerifier(
  ICurrentUserProvider currentUserProvider,
  IUserProvider userProvider)
: IUserVerifier
{
    private readonly ICurrentUserProvider currentUserProvider
    = currentUserProvider;

    private readonly IUserProvider userProvider = userProvider;

    public async ValueTask<bool> Verify(IUser user, string password)
    {
        var isUserId = Guid.TryParse(password, out var userId);
        if (!isUserId || user.Id != userId)
        {
            return false;
        }

        var currentUser = await currentUserProvider.GetCurrentUser();
        return await userProvider.IsSameUserIdOrAdmin(currentUser, user);
    }
}