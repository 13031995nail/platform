namespace Tmobis.Auth.JwtAuth.Modules;

using Filters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi;
using Options;
using Providers;
using Swashbuckle.AspNetCore.SwaggerGen;
using Tmobis.Business.Core.Modules;
using Tmobis.Core.Modules;
using Tmobis.Frameworks.Web.Modules;

/// <summary>
/// Модуль JWT-аутентификации и авторизации.
/// Определяет построение токенов, JWT bearer scheme, policy авторизации и Bearer-схему Swagger.
/// </summary>
[ModuleRequirement(
    typeof(AuthModule),
    typeof(BusinessCoreModule),
    typeof(WebFrameworkModule))]
public class JwtAuthModule : ModuleBase, ISwaggerOptionsModule
{
    public override void Register(
      IServiceCollection services,
      IConfiguration configuration,
      IModuleRegister modules)
    {
        base.Register(services, configuration, modules);
        services.AddTransient<ITokenBuilder, JwtTokenBuilder>();
        services.AddMemoryCache();
        services.AddSingleton<LegacyTownIdCache>();
        services.Configure<JwtAuthOptions>(
            configuration.GetSection(JwtAuthOptions.JwtAuth))
        .ConfigureOptions<ConfigureJwtBearerOptions>()
        .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer();

        var policy = new AuthorizationPolicyBuilder()
            .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
            .RequireAuthenticatedUser()
            .Build();
        services.AddAuthorizationBuilder()
            .SetDefaultPolicy(policy);
    }

    public void InitializeSwagger(SwaggerGenOptions options)
    {
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = "JWT Authorization header using the Bearer scheme.",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Scheme = "Bearer"
        });
        options.OperationFilter<JwtSecurityRequirementsOperationFilter>();
    }
}
