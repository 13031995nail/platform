namespace Tmobis.Auth;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tmobis.Core.Medium.Modules;
using Tmobis.Core.Modules;

/// <summary>
/// Базовый модуль аутентификации.
/// Содержит <c>ITokenAuthenticator</c> и consumers сценариев аутентификации.
/// </summary>
public class AuthModule : ModuleBase, IConsumerModule
{
    public override void Register(
        IServiceCollection services,
        IConfiguration configuration,
        IModuleRegister moduleRegister)
    {
        base.Register(services, configuration, moduleRegister);
        services.AddTransient<ITokenAuthenticator, TokenAuthenticator>();
    }
}
