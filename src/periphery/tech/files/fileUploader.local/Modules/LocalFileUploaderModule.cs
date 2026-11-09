namespace Tmobis.Files.Uploader.Local.Modules;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tmobis.Business.Core.Modules;
using Tmobis.Core.Modules;
using Tmobis.Files.Uploader.Local.Options;
using Tmobis.Files.Uploader.Local.Providers;

/// <summary>
/// Модуль локального файлового хранилища.
/// Использует локальную реализацию <c>IFileUploader</c>.
/// </summary>
[ModuleRequirement([typeof(BusinessCoreModule)])]
public sealed class LocalFileUploaderModule : ModuleBase
{
  /// <inheritdoc/>
  public override void Register(
    IServiceCollection services,
    IConfiguration configuration,
    IModuleRegister modules)
  {
    base.Register(services, configuration, modules);
    services.AddTransient<IFileUploader, LocalFileUploader>();
    services.Configure<FileUploadOptions>(
      configuration.GetSection(FileUploadOptions.FileUpload));
  }
}
