namespace Tmobis.Auth.JwtAuth.Options;

using System.Text;
using Microsoft.IdentityModel.Tokens;

public class JwtAuthOptions
{
    public static string JwtAuth = "JwtAuth";

    public string Issuer { get; set; } = "TIMEPATHTMOBIS";

    public string Audience { get; set; } = "TIMEPATHTMOBIS";

    public TimeSpan Lifetime { get; set; } = TimeSpan.FromDays(365);

    /// <summary>
    /// Срок кэширования филиала для старых JWT без town_id.
    /// Нулевое значение отключает кэш.
    /// </summary>
    public TimeSpan LegacyTownCacheLifetime { get; set; } = TimeSpan.FromHours(12);

    public bool IsLifetimeFixed { get; set; }

    public SymmetricSecurityKey? SymmetricSecurityKey =>
    string.IsNullOrEmpty(SymmetricKey) ? null :
        new(Encoding.ASCII.GetBytes(SymmetricKey));

    private string SymmetricKey { get; set; }
    = "TIMEPATHTMOBISTIMEPATHTMOBISTIMEPATHTMOBISTIMEPATHTMOBISTIMEPATHTMOBIS";
}