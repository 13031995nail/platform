namespace Tmobis.Core.DI;
using Microsoft.Extensions.Configuration;

/// <summary>
/// Вспомогательные методы для работы со строками подключения.
/// </summary>
public static class ConnectionStringExtensions
{
    public static string? GetDefaultConnectionString(
        this IConfiguration configuration)
    => configuration.GetConnectionString("Default")
        ?? configuration["DefConnStr"];
}