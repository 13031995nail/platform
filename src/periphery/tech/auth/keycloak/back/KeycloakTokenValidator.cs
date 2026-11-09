namespace Tmobis.Auth.Keycloak;

using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Options;

/// <summary>
/// Сервис валидации токенов Keycloak.
/// Используется OpenID Connect discovery document для получения публичных ключей.
/// </summary>
public class KeycloakTokenValidator(
    IOptions<KeycloakAuthOptions> options,
    HttpClient httpClient,
    ILogger<KeycloakTokenValidator> logger) : IKeycloakTokenValidator
{
    private readonly KeycloakAuthOptions options = options.Value;

    /// <inheritdoc/>
    public async Task<KeycloakTokenValidationResult> ValidateToken(string accessToken)
    {
        var claims = new Dictionary<string, string>();

        try
        {
            if (string.IsNullOrWhiteSpace(accessToken))
            {
                logger.LogWarning("Токен не предоставлен");
                return new(false, claims);
            }

            var discoveryDocument = await GetDiscoveryDocument();
            if (discoveryDocument == null)
            {
                logger.LogError("Не удалось получить discovery document Keycloak");
                return new(false, claims);
            }

            var securityKey = await GetSecurityKey(discoveryDocument);
            if (securityKey == null)
            {
                logger.LogError("Не удалось получить публичный ключ Keycloak");
                return new(false, claims);
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ClockSkew = TimeSpan.Zero,
                ValidateIssuer = true,
                ValidIssuer = options.KeycloakIssuer,
                ValidateAudience = true,
                ValidAudience = options.KeycloakAudience,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = securityKey,
            };
            var principal = tokenHandler.ValidateToken(accessToken, validationParameters, out var validatedToken);

            var now = DateTime.UtcNow;
            var validTo = validatedToken.ValidTo;
            if (validTo < now)
            {
                logger.LogWarning("Токен истек");
                return new(false, claims);
            }

            foreach (var claim in principal.Claims)
            {
                claims[claim.Type] = claim.Value;
            }

            logger.LogInformation("Токен Keycloak успешно валидирован для пользователя: {Username}",
                claims.GetValueOrDefault(options.UsernameClaimType));

            return new(true, claims);
        }
        catch (SecurityTokenExpiredException ex)
        {
            logger.LogError(ex, "Токен Keycloak истек");
            return new(false, claims);
        }
        catch (SecurityTokenValidationException ex)
        {
            logger.LogError(ex, "Ошибка валидации токена Keycloak");
            return new(false, claims);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ошибка при валидации токена Keycloak");
            return new(false, claims);
        }
    }

    private async Task<DiscoveryDocument?> GetDiscoveryDocument()
    {
        try
        {
            var url = options.OpenIdConfigUrl
                ?? $"{options.KeycloakIssuer.TrimEnd('/')}/.well-known/openid-configuration";

            logger.LogInformation("Получение discovery document из: {Url}", url);

            var response = await httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<DiscoveryDocument>();
            
            logger.LogError("Ошибка получения discovery document: {StatusCode}", response.StatusCode);
            return null;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ошибка при получении discovery document Keycloak");
            return null;
        }
    }

    private async Task<SecurityKey?> GetSecurityKey(DiscoveryDocument discoveryDocument)
    {
        var securityKey = await GetSecurityKeyFromDiscovery(discoveryDocument);
        if (securityKey != null)
        {
            return securityKey;
        }

        securityKey = GetSecurityKeyFromConfig();
        if (securityKey != null)
        {
            return securityKey;
        }

        logger.LogError("Не удалось получить публичный ключ - JWKS не настроен и RealmPublicKey пуст");
        return null;
    }

    private async Task<SecurityKey?> GetSecurityKeyFromDiscovery(DiscoveryDocument discoveryDocument)
    {
        if (string.IsNullOrEmpty(discoveryDocument.JwksUri))
            return null;
        
        logger.LogInformation("Получение ключей из JWKS: {JwksUri}", discoveryDocument.JwksUri);
        try
        {
            var response = await httpClient.GetAsync(discoveryDocument.JwksUri);
            if (!response.IsSuccessStatusCode)
            {
                logger.LogError("Ошибка при получении JWKS: {StatusCode}", response.StatusCode);
                return null;
            }
            
            var content = await response.Content.ReadAsStringAsync();
            var jwks = new JsonWebKeySet(content);
            logger.LogInformation("Получено {KeyCount} ключей из JWKS", jwks.Keys?.Count ?? 0);

            if (jwks.Keys?.Count == 0)
                return null;

            // Возвращаем ключ с типом sig, а не enc, т.к. токен использует его
            var key = jwks.Keys?.FirstOrDefault(key => key.Use == "sig");
            return key;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ошибка при получении ключей из JWKS");
            return null;
        }
    }

    private SecurityKey? GetSecurityKeyFromConfig()
    {
        if (string.IsNullOrEmpty(options.RealmPublicKey))
            return null;
        
        try
        {
            // RealmPublicKey из Keycloak - это RSA публичный ключ в PEM формате (Base64).
            // Он должен быть в формате RSA X.509 SubjectPublicKeyInfo
            var publicKeyBytes = Convert.FromBase64String(options.RealmPublicKey);

            var rsa = RSA.Create();
            rsa.ImportSubjectPublicKeyInfo(publicKeyBytes, out _);
            var securityKey = new RsaSecurityKey(rsa);
            logger.LogInformation("Используется статический RSA публичный ключ");
            return securityKey;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Неверный формат публичного ключа");
            return null;
        }
    }
}

/// <summary>
/// Данные discovery document Keycloak.
/// </summary>
public class DiscoveryDocument
{
    [JsonPropertyName("issuer")]
    public string? Issuer { get; set; }
    
    [JsonPropertyName("jwks_uri")]
    public string? JwksUri { get; set; }
    
    [JsonPropertyName("authorization_endpoint")]
    public string? AuthorizationEndpoint { get; set; }
    
    [JsonPropertyName("token_endpoint")]
    public string? TokenEndpoint { get; set; }
    
    [JsonPropertyName("userinfo_endpoint")]
    public string? UserInfoEndpoint { get; set; }
}