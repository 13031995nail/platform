namespace Tmobis.Business.Common.Sessions.Providers;

using Options;
using Models;
using Tmobis.Business.Core.Models.Users;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;
using Tmobis.Core.Repositories;
using System.Security.Claims;
using System;
using Core.Providers.Users;

/// <summary>
/// Сервис получения сессий.
/// </summary>
public class UserSessionProvider(
  IRepository repository,
  ICurrentUserProvider  currentUserProvider,
  IOptions<UserSessionsOptions> options,
  IHttpContextAccessor? httpContextAccessor = null)
: IUserSessionProvider
{
    private readonly UserSessionsOptions options = options.Value;

    /// <inheritdoc/>
    public virtual async Task<IUserSession?> GetById(Guid id)
    => await repository.GetById<IUserSession>(id);

    /// <inheritdoc/>
    public IUserSession CreateByUser(IUser user)
    {
        ArgumentNullException.ThrowIfNull(user);
        var session = repository.MakeEntity<IUserSession>();
        repository.Create(session);
        session.User = user;
        session.StartedAt = DateTime.UtcNow;
        session.Login = user.Login;
        session.Actuality = true;
        session.Ip = currentUserProvider.GetUserConnectionAddress()
            ?? "Unknown";
        return session;
    }

    /// <inheritdoc/>
    public void Expire(IUserSession session)
    {
        ArgumentNullException.ThrowIfNull(session);
        session.Actuality = false;
        session.ClosedAt = DateTime.UtcNow;
    }

    /// <inheritdoc/>
    public async Task<IUserSession?> GetCurrentSession()
    {
        var currentSessionId = GetCurrentSessionId();
        return currentSessionId != null
          ? await GetById(currentSessionId.Value)
          : null;
    }

    /// <inheritdoc/>
    public Guid? GetCurrentSessionId()
    {
        var sessionIdStr = httpContextAccessor?.HttpContext?
                .User?.FindFirstValue(ClaimTypes.UserData);

        if (string.IsNullOrEmpty(sessionIdStr))
        {
            return null;
        }

        if (!Guid.TryParse(sessionIdStr, out var result))
        {
            return null;
        }

        return result;
    }

    /// <inheritdoc/>
    public async Task ExpireAll()
    {
        var sessions = await repository
            .Find<IUserSession>(session => session.Actuality);

        foreach (var session in sessions)
        {
            Expire(session);
        }
    }

    /// <inheritdoc/>
    // public async Task PreloadSessions()
    // {
    //     var sessions = await repository
    //         .Find<IUserSession>(session => session.Actuality);
    //
    //     sessions.ForEach((userSession) => cachedProvider.SetToCache(
    //       userSession.Id.ToString(), userSession, options.EntityCacheLifetime));
    // }
}
