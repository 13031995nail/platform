namespace Tmobis.Offline.UseCases.Flexberry.Consumers;
using System.Threading.Tasks;
using ICSSoft.STORMNET;
using ICSSoft.STORMNET.Business;
using ICSSoft.STORMNET.KeyGen;
using Microsoft.Extensions.Logging;
using Tmobis.Core.Entities;
using Tmobis.Core.Medium.Consumers;
using Tmobis.Offline.Flexberry.Messages;
using Tmobis.Utils.Lzw.Converters;

public class CheckOfflineEntitiesToDeleteConsumer(
    ILzwConverter lzw,
    IDataService dataService,
    IEntityRegister entityRegister,
    ILogger<CheckOfflineEntitiesToDeleteConsumer> logger)
: IMessageConsumer<
    CheckOfflineEntitiesToDelete, OfflineEntitiesToDelete>
{
    private readonly ILzwConverter lzw = lzw;
    private readonly IDataService dataService = dataService;

    private readonly ILogger<CheckOfflineEntitiesToDeleteConsumer> logger
    = logger;

    public async Task<OfflineEntitiesToDelete> ConsumeWithResult(
        CheckOfflineEntitiesToDelete message)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(message);
            var result = new OfflineEntitiesToDelete();
            foreach (var obj in message.Entities)
            {
                var pks = GetPks(obj).Select(x => x.ToLower()).ToList();
                var type = entityRegister.GetTypeByName(obj.ModelName)
                    ?? throw new InvalidOperationException(
                        "Тип не найден");
                var lcs = GetLcs(type);

                var dObjsPks = dataService
                    .LoadObjects(lcs)
                    .Select(DataObjectPKToString)
                    .ToList();

                var notExists = pks.Where(x => !dObjsPks.Contains(x)).ToArray();
                if (notExists.Length > 0)
                {
                    var data = new OfflineEntityDeleted()
                    {
                        ModelName = type.FullName,
                    };
                    data.Pks.AddRange(notExists);
                    result.Data.Add(data);
                }
            }

            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                @"Ошибка при получении данных синхронизации 
                для Справочников = {Message}",
                message);
            return new OfflineEntitiesToDelete { Error = true };
        }
    }

    protected virtual string DataObjectPKToString(DataObject dataObject)
    => (dataObject.__PrimaryKey as KeyGuid).Guid.ToString("D").ToLower();

    protected virtual LoadingCustomizationStruct GetLcs(Type type)
    => new(null)
    {
        View = new View(new ViewAttribute("DeleteCheckView", []), type),
        LoadingTypes = [type],
    };

    protected virtual List<string> GetPks(OfflineEntityToCheckDelete obj)
    => [.. lzw
        .Decompress(obj.LzwPks)
        .Replace("]", string.Empty)
        .Replace("[", string.Empty)
        .Replace("\"", string.Empty)
        .Replace("'", string.Empty)
        .Split(',')];
}