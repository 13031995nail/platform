namespace Tmobis.Auth.JwtAuth.Options;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using Tmobis.Business.Core.Providers.Users;

public class ConfigureJwtBearerOptions(
    IOptions<JwtAuthOptions> appOptions,
    IHostEnvironment env)
: IConfigureNamedOptions<JwtBearerOptions>
{
    private readonly JwtAuthOptions appOptions = appOptions.Value;
    private readonly IHostEnvironment env = env;

    public virtual void Configure(string name, JwtBearerOptions options)
    {
        if (name == JwtBearerDefaults.AuthenticationScheme)
        {
            options.RequireHttpsMetadata = env.IsDevelopment();
            options.TokenValidationParameters = new ()
            {
                ValidateIssuer = true,
                ValidIssuer = appOptions.Issuer,
                ValidateAudience = true,
                ValidAudience = appOptions.Audience,
                ValidateLifetime = true,
                IssuerSigningKey = appOptions.SymmetricSecurityKey,
                ValidateIssuerSigningKey = true,
                ClockSkew = TimeSpan.Zero,
            };

            // Старые подписанные JWT не содержат town_id. Дозагружаем
            // филиал один раз при проверке токена, до выполнения endpoint.
            // Новые JWT не требуют обращения к БД.
            options.Events ??= new JwtBearerEvents();
            var previousOnTokenValidated = options.Events.OnTokenValidated;
            options.Events.OnTokenValidated = async context =>
            {
                await previousOnTokenValidated(context);

                if (context.Principal == null)
                {
                    context.Fail("Не удалось определить пользователя.");
                    return;
                }

                var resolved = await LegacyTownClaimInitializer.EnsureTownClaimAsync(
                    context.Principal,
                    async login =>
                    {
                        var services = context.HttpContext.RequestServices;
                        var cache = services.GetRequiredService<LegacyTownIdCache>();
                        return await cache.ResolveAsync(login, async cachedLogin =>
                        {
                            var userProvider = services.GetRequiredService<IUserProvider>();
                            var user = await userProvider.GetUserByLogin(cachedLogin);
                            return user?.TownId;
                        });
                    });

                if (!resolved)
                    context.Fail("Не удалось определить филиал пользователя.");
            };
        }
    }

    public void Configure(JwtBearerOptions options)
    => Configure(JwtBearerDefaults.AuthenticationScheme, options);
}