namespace Tmobis.Auth.JwtAuth.Filters;

using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

public class JwtSecurityRequirementsOperationFilter
    : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var metadata = context.ApiDescription.ActionDescriptor.EndpointMetadata;
        var hasAllowAnonymous = metadata.Any(m => m is IAllowAnonymous);
        if (hasAllowAnonymous)
        {
            return;
        }

        operation.Responses ??= new OpenApiResponses();
        operation.Responses.TryAdd("401", new OpenApiResponse { Description = "Unauthorized" });
        var bearerScheme = new OpenApiSecuritySchemeReference("Bearer", context.Document);

        operation.Security =
        [
            new OpenApiSecurityRequirement
            {
                [bearerScheme] = []
            }
        ];
    }
}