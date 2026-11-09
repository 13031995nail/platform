namespace Tmobis.Business.Core.Flexberry.Modules;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Reflection;
using Tmobis.Business.Core.Flexberry.BusinessServers;
using Tmobis.Business.Core.Flexberry.Consumers.Files;
using Tmobis.Business.Core.Flexberry.Providers.Roles;
using Tmobis.Business.Core.Models.Files;
using Tmobis.Business.Core.Modules;
using Tmobis.Business.Core.Providers.Roles;
using Tmobis.Core.DI;
using Tmobis.Core.Entities.Messages;
using Tmobis.Core.Medium.Consumers;
using Tmobis.Core.Medium.Modules;
using Tmobis.Core.Modules;

/// <summary>
/// Flexberry бизнес-модуль ядра бизнес-логики Платформы Т-Мобис.
/// Содержит Flexberry-реализации ролей, сотрудников и обработки связанных файлов.
/// </summary>
[ModuleRequirement([typeof(BusinessCoreModule)])]
public sealed class FlexberryBusinessCoreModule : ModuleBase, IEntityModule, IConsumerModule
{
  /// <inheritdoc/>
  public override void Register(
    IServiceCollection services,
    IConfiguration configuration,
    IModuleRegister modules)
  {
    base.Register(services, configuration, modules);
    services.AddOrReplaceTransient<IRoleProvider, FlexberryRoleProvider>();
    services.AddTransient<
      IMessageConsumer<EntityDeleted<IFile>, IEntityUpdatedResult>,
      OnFileDeletedConsumer>();
    services.AddTransient<EmployeeService>();
  }
}
