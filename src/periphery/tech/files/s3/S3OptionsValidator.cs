namespace Tmobis.Files.Adapter.S3;

internal static class S3OptionsValidator
{
  public static S3Options ValidateAndNormalize(S3Options options)
  {
    ArgumentNullException.ThrowIfNull(options);

    if (!Uri.TryCreate(options.ServiceUrl, UriKind.Absolute, out var serviceUri))
    {
      throw new InvalidOperationException(
        "S3:ServiceUrl должен быть абсолютным URL.");
    }

    var isHttps = string.Equals(
      serviceUri.Scheme,
      Uri.UriSchemeHttps,
      StringComparison.OrdinalIgnoreCase);
    var isHttp = string.Equals(
      serviceUri.Scheme,
      Uri.UriSchemeHttp,
      StringComparison.OrdinalIgnoreCase);

    if (!isHttps && (!isHttp || !options.AllowInsecureHttp))
    {
      throw new InvalidOperationException(
        "Для S3 endpoint требуется HTTPS. "
        + "Для HTTP явно укажите S3:AllowInsecureHttp=true.");
    }

    ArgumentException.ThrowIfNullOrWhiteSpace(options.BucketName);
    ArgumentException.ThrowIfNullOrWhiteSpace(options.AccessKey);
    ArgumentException.ThrowIfNullOrWhiteSpace(options.SecretKey);

    if (options.PresignedUrlLifetimeMinutes <= 0)
    {
      throw new InvalidOperationException(
        "S3:PresignedUrlLifetimeMinutes должен быть больше 0.");
    }

    if (options.TimeoutSeconds <= 0)
    {
      throw new InvalidOperationException(
        "S3:TimeoutSeconds должен быть больше 0.");
    }

    if (options.MaxErrorRetry < 0)
    {
      throw new InvalidOperationException(
        "S3:MaxErrorRetry не может быть отрицательным.");
    }

    options.ServiceUrl = serviceUri.ToString().TrimEnd('/');
    options.AuthenticationRegion = string.IsNullOrWhiteSpace(
      options.AuthenticationRegion)
      ? "us-east-1"
      : options.AuthenticationRegion.Trim();
    options.BucketName = options.BucketName.Trim();
    options.AccessKey = options.AccessKey.Trim();
    options.SecretKey = options.SecretKey.Trim();
    options.KeyPrefix = options.KeyPrefix
      .Replace('\\', '/')
      .Trim()
      .Trim('/');

    return options;
  }
}
