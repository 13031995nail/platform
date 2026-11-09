namespace Tmobis.Frameworks.Web.Modules;

using Core.Modules;
using Swashbuckle.AspNetCore.SwaggerGen;

public interface ISwaggerOptionsModule
    : IModule
{
    void InitializeSwagger(SwaggerGenOptions options);
}
