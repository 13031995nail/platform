namespace Tmobis.Auth.Keycloak.Modules;

using Controllers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Options;
using Tmobis.Auth.JwtAuth.Modules;
using Tmobis.Core.Modules;
using Tmobis.Business.Core.Modules;

/// <summary>
/// Модуль аутентификации через Keycloak.
/// Содержит HTTP-клиент, controller, providers проверки токена и пользователя, а также authenticator.
/// </summary>
[ModuleRequirement(typeof(JwtAuthModule), typeof(ClaimsRoleProviderModule))]
public class KeycloakAuthModule: ModuleBase
{
    public override void Register(
        IServiceCollection services,
        IConfiguration configuration,
        IModuleRegister modules)
    {
        base.Register(services, configuration, modules);
        services.Configure<KeycloakAuthOptions>(
            configuration.GetSection(KeycloakAuthOptions.KeycloakAuth));
        services.AddHttpClient("Keycloak");
        services.AddTransient<KeycloakAuthController>();
        services.AddTransient<IKeycloakTokenValidator, KeycloakTokenValidator>();
        services.AddTransient<IKeycloakUserProvider, KeycloakUserProvider>();
        services.AddScoped<IKeycloakAuthenticator, KeycloakAuthenticator>();
    }
}
