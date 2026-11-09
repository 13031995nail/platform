namespace Tmobis.Integrations.Assistants.Gigachat;

public class GigachatOptions
{
    public const string Gigachat = "Gigachat";

    public string AuthUrl { get; set; } = string.Empty;

    public string FileUploadUrl { get; set; } = string.Empty;

    public string FileDeleteUrl { get; set; } = string.Empty;

    public string RequestUrl { get; set; } = string.Empty;

    public string Token { get; set; } = string.Empty;

    public string Scope { get; set; } = string.Empty;

    public string Model { get; set; } = string.Empty;

    public TimeSpan TokenExpiration { get; set; } = TimeSpan.FromMinutes(25);
}