namespace Tmobis.Files.PrimaryAdapter;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tmobis.Business.Core.Modules;
using Tmobis.Core.Modules;
using Tmobis.Medium.WebApi;

/// <summary>
/// Модуль Web API загрузки файлов.
/// Содержит обработку, валидацию и чтение входящих файловых запросов.
/// </summary>
[ModuleRequirement(typeof(WebApiListenerModule), typeof(BusinessCoreModule))]
public sealed class FilesPrimaryAdapterModule : ModuleBase
{
    /// <summary>
    /// Регистрация настроек и сервисов HTTP-приёма файлов.
    /// </summary>
    public override void Register(
        IServiceCollection services,
        IConfiguration configuration,
        IModuleRegister modules)
    {
        base.Register(services, configuration, modules);

        services.Configure<FilesPrimaryAdapterOptions>(
            configuration.GetSection(FilesPrimaryAdapterOptions.FilesPrimaryAdapter));
        services.AddTransient<IApiEndpointBuilder, FilesEndpointBuilder>();
        services.AddTransient<FilesEndpointHandler>();
        services.AddTransient<FileUploadRequestValidator>();
        services.AddTransient<RequestBodyReader>();
    }
}
