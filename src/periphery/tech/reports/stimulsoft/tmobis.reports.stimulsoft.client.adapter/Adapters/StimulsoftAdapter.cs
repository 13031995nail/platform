namespace Tmobis.Reports.Stimulsoft.Client.Adapter.Adapters;
using System.Net.Http.Headers;
using System.Text.Json;
using Tmobis.Reports.Models;
using Tmobis.Reports.Stimulsoft.Client.Adapter.Options;
using Microsoft.Extensions.Options;
using Tmobis.Reports.Adapter.SecondaryAdapters;

public class StimulsoftAdapter(
  IOptions<StimulsoftAdapterOptions> options,
  HttpClient httpClient) : IReportServiceAdapter
{
  private readonly StimulsoftAdapterOptions options = options.Value;

  public async Task<byte[]> BuildReport(GetReportRequest request,
        bool throwOnError = false)
  {
    var apiKey = options.ApiKey;
    Uri.TryCreate(options.ServiceUrl + options.BuildReportPath, UriKind.Absolute, out Uri? uri);
    var content = new StringContent(JsonSerializer.Serialize(request));
    content.Headers.Add("ApiKey", apiKey);
    content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
    var response = await httpClient.PostAsync(uri, content);
    if (throwOnError)
    {
      response.EnsureSuccessStatusCode();
    }
    var responseStream = await response.Content.ReadAsStreamAsync();
    using var memoryStream = new MemoryStream();
    await responseStream.CopyToAsync(memoryStream);
    return memoryStream.ToArray();
  }
}
