namespace Tmobis.Medium.WebApi;

using Microsoft.AspNetCore.Routing;

public interface IApiEndpointBuilder
{
    void Build(IEndpointRouteBuilder endpoints);
}