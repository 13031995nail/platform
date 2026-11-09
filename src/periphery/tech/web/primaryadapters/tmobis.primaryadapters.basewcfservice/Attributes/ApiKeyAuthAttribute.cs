namespace Tmobis.PrimaryAdapters.BaseWcfService.Attributes
{
  using System;
  using Auth.ApiKey;
  using CoreWCF;
  using CoreWCF.Channels;
  using CoreWCF.Description;
  using CoreWCF.Dispatcher;
  using Microsoft.AspNetCore.Http;
  using Microsoft.Extensions.DependencyInjection;
  using Microsoft.Extensions.Options;

  public class ApiKeyAuthAttribute : Attribute, IOperationBehavior, IParameterInspector
  {
    private const string ApiKeyHeaderName = "ApiKey";

    /// <inheritdoc/>>
    public override string ToString()
    {
      return "ApiKeyWcfAuthorize";
    }

    public void AddBindingParameters(OperationDescription operationDescription, BindingParameterCollection bindingParameters)
    {
    }

    public void ApplyClientBehavior(OperationDescription operationDescription, ClientOperation clientOperation)
    {
    }

    public void ApplyDispatchBehavior(OperationDescription operationDescription, DispatchOperation dispatchOperation)
    {
      dispatchOperation.ParameterInspectors.Add(this);
    }

    public void Validate(OperationDescription operationDescription)
    {
    }

    public object BeforeCall(string operationName, object[] inputs)
    {
      var httpContext = OperationContext.Current.IncomingMessageProperties["Microsoft.AspNetCore.Http.HttpContext"] as HttpContext;

      if (httpContext == null)
        throw new FaultException("HttpContext is not available");

      var options = httpContext.RequestServices.GetRequiredService<IOptions<ApiKeyAuthorizationOptions>>().Value;
      var apiKey = options.ApiKey;
      var keyExist = httpContext.Request.Headers.TryGetValue(ApiKeyHeaderName, out var extractedApiKey);
      if (!keyExist || !extractedApiKey.ToString().Equals(apiKey, StringComparison.InvariantCulture))
         throw new FaultException("Invalid or missing API Key");

      return null;
    }

    public void AfterCall(string operationName, object[] outputs, object returnValue, object correlationState)
    {
    }
  }
}
