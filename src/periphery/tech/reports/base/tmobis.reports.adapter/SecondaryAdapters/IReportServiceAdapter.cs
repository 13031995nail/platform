namespace Tmobis.Reports.Adapter.SecondaryAdapters;
using Tmobis.Reports.Models;

public interface IReportServiceAdapter
  {
      public Task<byte[]> BuildReport(GetReportRequest request,
        bool throwOnError = false);
  }
