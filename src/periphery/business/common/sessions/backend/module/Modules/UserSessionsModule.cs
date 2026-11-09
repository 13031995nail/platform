namespace Tmobis.Business.Common.Sessions.Modules;
using Tmobis.Business.Common.Sessions.Providers;
using Tmobis.Business.Common.Sessions.Messages;
using Microsoft.Extensions.DependencyInjection;
using Tmobis.Business.Common.Sessions.Options;
using Tmobis.Business.Common.Sessions.Jobs;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using Tmobis.Core.Medium.Modules;
using Tmobis.Jobs.Scheduled;
using Tmobis.Core.Modules;
using System;

/// <summary>
/// Бизнес-модуль сущности пользовательских сессий.
/// Содержит provider текущей сессии, предварительную загрузку cache и плановое завершение сессий.
/// </summary>
[ModuleRequirement([typeof(MessageScheduleModule)])]
public sealed class UserSessionsModule : ModuleBase,
  IScheduledMessagesModule,
  IMessageModule, IEntityModule, IConsumerModule
{
  public override void Register(
    IServiceCollection services,
    IConfiguration configuration,
    IModuleRegister modules)
  {
    base.Register(services, configuration, modules);
    services.AddScoped<IUserSessionProvider, UserSessionProvider>();
    services.Configure<UserSessionsOptions>(
      configuration.GetSection(UserSessionsOptions.UserSessions));
    services.AddHostedService<PreloadSessionCacheJob>();
  }

  public List<Type> GetScheduledMessages() =>
  [typeof(UserSessionExpireAll)];
}
