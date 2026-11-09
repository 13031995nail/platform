namespace Tmobis.Files.Adapter.S3;

using Amazon.Runtime;
using Amazon.S3;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Tmobis.Business.Core.Modules;
using Tmobis.Core.Modules;
using Tmobis.Files.Provider;
using Tmobis.Files.Uploader;

/// <summary>
/// Модуль файлового хранилища S3.
/// Содержит S3 uploader, provider и общее файловое хранилище.
/// </summary>
[ModuleRequirement(typeof(BusinessCoreModule))]
public sealed class S3FileAdapterModule : ModuleBase
{
  /// <inheritdoc/>
  public override void Register(
    IServiceCollection services,
    IConfiguration configuration,
    IModuleRegister modules)
  {
    base.Register(services, configuration, modules);

    services.Configure<S3Options>(
      configuration.GetSection(S3Options.SectionName));

    services.AddSingleton<IAmazonS3>((provider) =>
    {
      var options = S3OptionsValidator.ValidateAndNormalize(
        provider.GetRequiredService<IOptions<S3Options>>().Value);
      var serviceUri = new Uri(options.ServiceUrl, UriKind.Absolute);
      var credentials = new BasicAWSCredentials(
        options.AccessKey, options.SecretKey);
      var clientConfig = new AmazonS3Config
      {
        ServiceURL = options.ServiceUrl,
        AuthenticationRegion = options.AuthenticationRegion,
        ForcePathStyle = options.ForcePathStyle,
        UseHttp = string.Equals(serviceUri.Scheme, Uri.UriSchemeHttp,
          StringComparison.OrdinalIgnoreCase),
        Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds),
        MaxErrorRetry = options.MaxErrorRetry
      };
      clientConfig.Validate();

      return new AmazonS3Client(credentials, clientConfig);
    });

    services.AddTransient<IFileUploader, S3FileUploader>();
    services.AddTransient<IFileProvider, S3FileProvider>();
    services.AddSingleton<S3FileStorage>();
  }
}
