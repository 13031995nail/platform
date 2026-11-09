namespace Tmobis.Frameworks.Web.Modules;
using Microsoft.AspNetCore.Mvc;
using Tmobis.Core.Modules;

public interface IWebOptionsModule : IModule
{
  void ConfigureWebOptions(MvcOptions options);
}
