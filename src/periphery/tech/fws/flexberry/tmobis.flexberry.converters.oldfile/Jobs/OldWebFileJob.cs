namespace Tmobis.Frameworks.Flexberry.Converters.OldFile.Jobs;

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ICSSoft.STORMNET;
using ICSSoft.STORMNET.Business;
using ICSSoft.STORMNET.Business.LINQProvider;
using Microsoft.Extensions.Hosting;
using Tmobis.Business.Core.Models.Files;
using Tmobis.Core.Repositories;
using Tmobis.Frameworks.Flexberry.Converters.OldFile.Converters;
using File = Business.Core.Flexberry.Models.File;

public class OldWebFileJob(
    IOldWebFileConverter converter, IDataService dataService)
: BackgroundService
{
  private readonly IOldWebFileConverter converter = converter;
  private readonly IDataService dataService = dataService;

  protected override async Task ExecuteAsync(CancellationToken stoppingToken)
  {
    var files = dataService
        .Query<File>(File.Views.FileOld)
        .Cast<File>()
        .ToList();
    files.ForEach(converter.ConvertOldWebFile);
    var updatedFiles = files.Cast<IFile>().Cast<DataObject>().ToArray();
    dataService.UpdateObjects(ref updatedFiles);
  }
}
