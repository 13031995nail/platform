namespace Tmobis.PrimaryAdapters.BaseWcfService.Helpers
{
  using CoreWCF.Channels;
  using CoreWCF;

  public static class HttpBindingHelper
  {
    /// <summary>
    /// Создает экземпляр BasicHttpBinding для WCF Сервиса
    /// </summary>
    /// <returns></returns>
    public static BasicHttpBinding GetHttpBinding()
    {
      return new BasicHttpBinding()
      {
        Security = new BasicHttpSecurity
        {
          Mode = BasicHttpSecurityMode.Transport,
          Transport = new HttpTransportSecurity
          {
            ClientCredentialType = HttpClientCredentialType.None,
          },
        },
        MaxBufferSize = int.MaxValue,
        MaxReceivedMessageSize = 10_000_000,
        ReaderQuotas = new System.Xml.XmlDictionaryReaderQuotas
        {
          MaxArrayLength = 10_000_000,
          MaxStringContentLength = 10_000_000,
        },
      };
    }
  }
}
