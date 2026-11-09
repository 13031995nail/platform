namespace Tmobis.Files.Adapter.S3;

/// <summary>
/// Настройки подключения к S3-хранилищу.
/// </summary>
public class S3Options
{
  /// <summary>
  /// Название блока конфигурации.
  /// </summary>
  public const string SectionName = "S3";

  /// <summary>
  /// Базовый адрес S3-совместимого endpoint.
  /// </summary>
  public string ServiceUrl { get; set; } = string.Empty;

  /// <summary>
  /// Регион, используемый при подписывании запросов.
  /// </summary>
  public string AuthenticationRegion { get; set; } = "us-east-1";

  /// <summary>
  /// Название bucket.
  /// </summary>
  public string BucketName { get; set; } = string.Empty;

  /// <summary>
  /// Access key.
  /// </summary>
  public string AccessKey { get; set; } = string.Empty;

  /// <summary>
  /// Secret key.
  /// </summary>
  public string SecretKey { get; set; } = string.Empty;

  /// <summary>
  /// Необязательный префикс ключей внутри bucket.
  /// </summary>
  public string KeyPrefix { get; set; } = string.Empty;

  /// <summary>
  /// Принудительно использовать path-style addressing.
  /// Для SeaweedFS обычно требуется именно этот режим.
  /// </summary>
  public bool ForcePathStyle { get; set; } = true;

  /// <summary>
  /// Разрешить небезопасный HTTP endpoint.
  /// По умолчанию разрешен только HTTPS.
  /// </summary>
  public bool AllowInsecureHttp { get; set; }

  /// <summary>
  /// Включать chunked uploads.
  /// Для S3-совместимых хранилищ по умолчанию отключено ради совместимости.
  /// </summary>
  public bool UseChunkEncoding { get; set; }

  /// <summary>
  /// Время жизни presigned URL в минутах.
  /// </summary>
  public int PresignedUrlLifetimeMinutes { get; set; } = 15;

  /// <summary>
  /// Таймаут запроса к S3 в секундах.
  /// </summary>
  public int TimeoutSeconds { get; set; } = 100;

  /// <summary>
  /// Количество повторов на сетевые ошибки.
  /// </summary>
  public int MaxErrorRetry { get; set; } = 3;
}
