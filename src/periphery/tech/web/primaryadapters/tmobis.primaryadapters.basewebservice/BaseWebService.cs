using Tmobis.PrimaryAdapters.BaseWebService.Interfaces;

namespace Tmobis.PrimaryAdapters.BaseWebService
{
  public abstract class BaseWebService<T, V>
      where T : IWebServiceRequest
      where V : IWebServiceResponse, new()
  {
    protected abstract string ValidateRequest(T request);

    protected virtual V GetErrorObject(string message)
    {
      return new V()
      {
        Status = "ERROR",
        ErrorCode = "1",
        ErrorMessage = message
      };
    }
  }
}
