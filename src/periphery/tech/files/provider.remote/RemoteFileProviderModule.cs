namespace Tmobis.Files.Provider.Remote;
using Tmobis.Core.Modules;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

/// <summary>
/// Модуль доступа к удалённому файловому сервису.
/// Использует typed <c>HttpClient</c> и настройки адреса/API key.
/// </summary>
public class RemoteFileProviderModule : ModuleBase
{
    public override void Register(
      IServiceCollection services,
      IConfiguration configuration,
      IModuleRegister modules)
    {
        base.Register(services, configuration, modules);
        services.Configure<RemoteFileProviderOptions>(
            configuration.GetSection(
                RemoteFileProviderOptions.RemoteFile));
        services.AddHttpClient<RemoteFileProvider>((provider, client) =>
        {
            var options = provider.GetService<
                IOptions<RemoteFileProviderOptions>>()
            ?.Value
                ?? throw new InvalidOperationException(
                    "Опции не зарегистрированы");

            client.DefaultRequestHeaders.Add(
                options.ApiKeyHeaderName, options.ApiKey);
            client.BaseAddress = new Uri(options.Url);
        });
    }
}
