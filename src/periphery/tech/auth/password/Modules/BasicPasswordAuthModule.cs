namespace Tmobis.Auth.Password.Modules;

using Controllers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Models;
using Options;
using Providers;
using Services;
using Tmobis.Auth.JwtAuth.Modules;
using Tmobis.Core.DI;
using Tmobis.Core.Medium.Modules;
using Tmobis.Core.Modules;
using Verifiers;

/// <summary>
/// Модуль аутентификации по логину и паролю.
/// Содержит проверки пользователя, timeout-сервис и WebApi-команды управления паролем.
/// </summary>
[ModuleRequirement(typeof(JwtAuthModule))]
public sealed class BasicPasswordAuthModule : ModuleBase, IConsumerModule
{
  /// <summary>
  /// Регистрация сервисов в DI .Net.
  /// </summary>
  /// <param name="services">Коллекция сервисов.</param>
  public override void Register(
    IServiceCollection services,
    IConfiguration configuration,
    IModuleRegister modules)
  {
    base.Register(services, configuration, modules);
    services.AddTransient<PasswordAuthController>();
    services.AddTransient<IUserPasswordProvider, UserPasswordProvider>();
    services.AddTransient<IUserVerifier, IncarnateUserVerifier>();
    services.AddTransient<IUserVerifier, PasswordHashUserVerifier>();
    services.AddTransient<IAuthTimeoutService, AuthTimeoutService>();
    services.Configure<AuthOptions>(
            configuration.GetSection(AuthOptions.Auth));
    services.AddListenedMessage<SecurityChangePasswordRequest>("WebApi");
    services.AddListenedMessage<SetIsComplexPasswordRequest>("WebApi");
  }
}
