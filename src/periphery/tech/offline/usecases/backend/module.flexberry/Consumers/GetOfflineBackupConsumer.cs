namespace Tmobis.Offline.UseCases.Flexberry.Consumers;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Tmobis.Business.Core.Flexberry.BusinessServers;
using Tmobis.Core.Entities;
using Tmobis.Core.Medium.Consumers;
using Tmobis.Offline.Flexberry.Messages;
using Tmobis.Offline.Models;
using Tmobis.Offline.Options;

public class GetOfflineBackupConsumer(
    IOptions<OfflineOptions> options,
    IEntityRegister entityRegister,
    LocalDictionaryBS localDictionaryBS,
    ILogger<GetOfflineBackupConsumer> logger)
: IMessageConsumer<
    GetOfflineBackup, OfflineBackupData>
{
    private readonly OfflineOptions options = options.Value;
    private readonly IEntityRegister entityRegister = entityRegister;
    private readonly LocalDictionaryBS localDictionaryBS = localDictionaryBS;
    private readonly ILogger<GetOfflineBackupConsumer> logger
          = logger;

    public async Task<OfflineBackupData> ConsumeWithResult(
        GetOfflineBackup message)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(message);
            var backupDirPath = options.OfflineBackupDirectory;
            if (Directory.Exists(backupDirPath))
            {
                var partsCount = 0;
                var curPart = 0;
                var latestBackup = Directory.GetDirectories(backupDirPath)
                    .Select(x => Path.GetFileName(x))
                    .Where(x => DateTime.TryParse(x.Replace(".", ":"),
                        out var dateEndLoc))
                    .Select(x => new Tuple<string, DateTime>(
                        x, DateTime.Parse(x.Replace(".", ":"))))
                    .OrderByDescending(x => x.Item2)
                    .FirstOrDefault();

                if (latestBackup == null)
                {
                    return new OfflineBackupData { NotExists = true };
                }

                var classType = entityRegister.GetTypeByName(message.ModelName);
                var classBackupMainDirPath = Path.Combine(
                    backupDirPath, latestBackup.Item1, message.ModelName);

                var backupInfoPath = Path.Combine(
                    classBackupMainDirPath, options.OfflineBackupFileName);
                var data = new List<string>();

                if (File.Exists(backupInfoPath))
                {
                    var jsonBackupData = System.IO.File.ReadAllText(
                        backupInfoPath);
                    try
                    {
                        var backupInfo = JsonConvert.DeserializeObject<
                            JsonBackupInfo>(jsonBackupData)
                            ?? throw new Exception(
                                "Информация о бэкапе пуста.");

                        var netDistrictId = message.NetDistrictId;
                        if (!netDistrictId.HasValue &&
                            (localDictionaryBS.IsLocalDictionary(classType)
                                || message.LimitByTown))
                        {
                            netDistrictId = message.TownId;
                        }

                        var partsInfo = GetPartsInfo(
                            backupInfo, netDistrictId)
                            ?? throw new Exception(
                                "Не определена часть бэкапа.");
                        partsCount = partsInfo.PartsLocalPaths.Count;
                        curPart = message.Part ?? 0;
                        var curPath = GetPathForPart(
                            partsInfo, classBackupMainDirPath, curPart);
                        if (System.IO.File.Exists(curPath))
                        {
                            data.Add(System.IO.File.ReadAllText(curPath));
                        }
                        else
                        {
                            var timeStr = latestBackup.Item2;
                            var typeStr = classType.FullName;
                            logger.LogError(
                                @"Бэкап на ""{TimeStr}"" существует, 
                                но файла для части {CurPart} ""{TypeStr}"" 
                                по пути ""{CurPath}"" нет.",
                                timeStr,
                                curPart,
                                typeStr,
                                curPart);
                        }
                    }
                    catch (Exception ex)
                    {
                        var typeStr = classType.FullName;
                        logger.LogError(
                            ex,
                            @"Ошибка при десериализации файла с информацией 
                            о бэкапе для ""{TypeStr}"" по пути
                            ""{BackupInfoPath}"".",
                            typeStr,
                            backupInfoPath);
                    }
                }
                else
                {
                    var timeStr = latestBackup.Item2;
                    var typeStr = classType.FullName;
                    logger.LogError(
                        @"Бэкап на ""{TimeStr}"" существует, но файла с 
                        информацией о бэкапе для ""{TypeStr}"" по 
                        пути ""{BackupInfoPath}"" нет.",
                        timeStr,
                        typeStr,
                        backupInfoPath);
                }

                if (data.Count == 0)
                {
                    return new OfflineBackupData { NotExists = true };
                }

                var dateWithOffset = latestBackup.Item2
                    .Add(TimeZoneInfo.Local.GetUtcOffset(DateTime.UtcNow));
                var serializedDataArray = JsonConvert.SerializeObject(data);
                return new OfflineBackupData
                {
                    DateEnd = dateWithOffset.ToString("o"),
                    Data = $"{{\"{message.ModelName}\": {serializedDataArray}}}",
                    CurPart = curPart,
                    PartsCount = partsCount,
                };
            }
            else
            {
                return new OfflineBackupData { NotExists = true };
            }
        }
        catch (Exception ex)
        {
            var typeStr = message.ModelName;
            logger.LogError(
                ex,
                @"Ошибка при получении данных справочника
                {TypeStr} из JSON бэкапа.",
                typeStr);
            return new OfflineBackupData { Error = true };
        }
    }

    protected virtual JsonBackupPartsInfo? GetPartsInfo(
        JsonBackupInfo backupInfo, Guid? netDistrictId)
    {
        return netDistrictId.HasValue
            && backupInfo.SubParts
            .TryGetValue(netDistrictId.Value, out JsonBackupPartsInfo? value)
        ? value : backupInfo.Parts;
    }

    protected virtual string? GetPathForPart(
        JsonBackupInfo backupInfo, Guid? netDistrictId, int part)
    {
        var partsInfo = GetPartsInfo(backupInfo, netDistrictId);
        if (partsInfo == null
            || !partsInfo.PartsLocalPaths.TryGetValue(part, out string? value))
        {
            return null;
        }

        return value;
    }

    protected virtual string? GetPathForPart(
        JsonBackupPartsInfo partsInfo, string dirPath, int part)
    {
        if (!partsInfo.PartsLocalPaths.TryGetValue(part, out string? value))
        {
            return null;
        }

        return string.IsNullOrWhiteSpace(partsInfo.SubDir)
        ? Path.Combine(dirPath, value)
        : Path.Combine(dirPath, partsInfo.SubDir, value);
    }
}