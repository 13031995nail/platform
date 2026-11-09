namespace Tmobis.Medium.WebApi;
using Microsoft.AspNetCore.Routing;

public abstract class ApiEndpointBuilder
: IApiEndpointBuilder
{
    public abstract void Build(IEndpointRouteBuilder endpoints);

    protected virtual string BuildEndpoint(string name, string? group = null)
    {
        var groupEndp = string.IsNullOrEmpty(group) ? "" : $"/{group}";
        return $"/api{groupEndp}/{name}";
    }
}