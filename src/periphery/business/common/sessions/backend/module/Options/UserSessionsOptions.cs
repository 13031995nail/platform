namespace Tmobis.Business.Common.Sessions.Options;

public class UserSessionsOptions
{
    public const string UserSessions = "UserSessions";

    public TimeSpan EntityCacheLifetime { get; set; }
    = TimeSpan.FromMinutes(30);
}