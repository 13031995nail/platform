namespace Tmobis.Auth.Password.Options;

public class AuthOptions
{
  public const string Auth = "Auth";

  public TimeoutsOptions Timeouts { get; set; } = new TimeoutsOptions();
}

public record TimeoutsOptions
{
  public string BlockTimeMinutes { get; set; } = "5";

  public string ExpirationTimeMinutes { get; set; } = "5";

  public string TriesLimit { get; set; } = "5";

  public bool IsInUse { get; set; }
}
