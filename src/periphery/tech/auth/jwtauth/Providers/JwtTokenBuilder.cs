namespace Tmobis.Auth.JwtAuth.Providers;

using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Options;
using Tmobis.Auth;
using Tmobis.Business.Core.Providers.Users;
using Tmobis.Business.Core.TownLimit;
using Tmobis.Core.Types;

public class JwtTokenBuilder(
    IOptions<JwtAuthOptions> options,
    IUserProvider userProvider)
: ITokenBuilder
{
    private readonly JwtAuthOptions options = options.Value;

    protected virtual AuthTokenData Data { get; set; } = null!;

    public virtual async Task<string> GetToken(AuthTokenData data)
    {
        ArgumentNullException.ThrowIfNull(data);
        Data = data;
        var token = CreateToken();
        var encodedJwt = new JwtSecurityTokenHandler().WriteToken(token);
        var payload = await GetTokenPayload();
        payload?.Add("accessToken", encodedJwt);
        return JsonConvert.SerializeObject(payload);
    }

    protected virtual JwtSecurityToken CreateToken()
    {
        var identity = GetIdentity();
        var now = DateTime.UtcNow;
        var expires = options.IsLifetimeFixed
            ? new DateTime(
                now.Year, now.Month, now.Day,
                options.Lifetime.Hours, options.Lifetime.Minutes, 0, DateTimeKind.Utc)
            : now.Add(options.Lifetime);
        return new JwtSecurityToken(
                issuer: options.Issuer,
                audience: options.Audience,
                notBefore: now,
                claims: identity.Claims,
                expires: expires,
                signingCredentials: new(
                    options.SymmetricSecurityKey,
                    SecurityAlgorithms.HmacSha256));
    }

    protected virtual ClaimsIdentity GetIdentity()
    {
        var claims = new List<Claim>
        {
            new (ClaimsIdentity.DefaultNameClaimType, Data.User.Login)
        };

        foreach (var role in Data.Roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }
        
        // Подписанный филиал позволяет строить TownLimited Query без I/O.
        // Не добавляем пустой идентификатор: access policy должна отклонить его.
        if (Data.User.TownId != Guid.Empty)
        {
            claims.Add(new Claim(
                TownClaimTypes.TownId,
                Data.User.TownId.ToString("D")));
        }

        // Филиал вычисляем из доверенной модели пользователя: дополнительные
        // обработчики подготовки токена не должны подменять этот claim.
        claims.AddRange(Data.Claims.Where(
            claim => claim.Type != TownClaimTypes.TownId));

        ClaimsIdentity claimsIdentity = new (
            claims,
            "Token",
            ClaimsIdentity.DefaultNameClaimType,
            ClaimsIdentity.DefaultRoleClaimType);

        return claimsIdentity;
    }

    protected virtual async Task<JObject?> GetTokenPayload()
    {
        var user = Data.User;
        var jobj = new JObject
        {
            { "userName", user.Login },
            { "roles", string.Join(",", Data.Roles) },
            { "user", user.Id.ToStringTrim() },
            { "town", user.TownId.ToStringTrim() }
        };

        foreach (var keyValuePair in Data.Payload)
        {
            jobj.Add(keyValuePair.Key, keyValuePair.Value);
        }

        if (!string.IsNullOrEmpty(user.UserId))
        {
            var result = new List<object>();
            var multiUsers = await userProvider.GetMultiUsers(
                user.UserId, user.Login);
            multiUsers?
            .OrderBy(x => x.Town.Name)
            .ToList()
            .ForEach(x => result
                .Add(new
                {
                    login = x.Login,
                    town = x.Town.Name,
                    pk = x.Id.ToStringTrim(),
                })
            );
            jobj.Add("multi", JArray.FromObject(result));
        }

        return jobj;
    }
}
