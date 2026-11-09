namespace Tmobis.Business.Core.Providers.Users;
using Roles;
using Tmobis.Business.Core.Models.Users;
using Tmobis.Business.Core.Models.Towns;
using Microsoft.Extensions.Options;
using Options;
using System.Collections.Generic;
using Tmobis.Core.Repositories;
using System;
using Tmobis.Business.Core.Models.Employees;

/// <inheritdoc/>
public class UserProvider(
    IRoleProvider roleProvider,
    IReadRepository repository,
    IOptions<UserProviderOptions> userProviderOptions)
: IUserProvider
{
    private readonly UserProviderOptions options = userProviderOptions.Value;

    // TODO: Вернуть кэш?
    /// <inheritdoc/>
    public virtual async Task<IUser?> GetUserByLogin(string login)
    => await repository.FirstOrDefault<IUser>(
        user => user.Login == login && user.Actuality);

    // TODO: Вернуть кэш?
    /// <inheritdoc/>
    public virtual async ValueTask<IUser?> GetUserById(Guid id)
    => await repository.GetById<IUser>(id);

    /// <inheritdoc/>
    public virtual async Task<List<IUser>?> GetMultiUsers(
        string userId, string login)
    => await repository.Find<IUser>(
        user => user.UserId == userId);

    /// <inheritdoc/>
    public virtual async Task<IUser?> GetAdminByTown(ITown town)
    {
        var townId = town.Id;
        return await repository
            .FirstOrDefault<IUser>(x => x.UserId == options.AdminUserId
                && x.Town.Id.Equals(townId)
                && x.Login.Contains("admin"));
    }

    /// <inheritdoc/>
    public virtual async ValueTask<bool> IsSameUserIdOrAdmin(
        IUser actor, IUser otherUser)
    => await roleProvider.IsAdmin(actor) || actor?.UserId == otherUser.UserId;

    /// <inheritdoc/>
    public virtual async Task<IUser?> GetByEmployee(IEmployee employee)
    => await GetUserById(employee?.User?.Id
        ?? throw new ArgumentNullException(nameof(employee)));
}
