namespace Tmobis.Offline.Backups.Consumers;

using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSSoft.STORMNET;
using ICSSoft.STORMNET.Business;
using ICSSoft.STORMNET.Business.Audit;
using ICSSoft.STORMNET.FunctionalLanguage;
using ICSSoft.STORMNET.KeyGen;
using ICSSoft.STORMNET.Windows.Forms;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Tmobis.Business.Core.Flexberry.BusinessServers;
using Tmobis.Business.Core.Models.Towns;
using Core.Entities;
using Tmobis.Core.Medium.Consumers;
using Core.Repositories;
using Helpers;
using Models;
using Options;
using Converters;
using Messages;
using Tmobis.Offline.Models;

public class MakeOfflineBackupConsumer(
    IOptions<OfflineBackupOptions> options,
    ILogger<MakeOfflineBackupConsumer> logger,
    IRepository repository,
    IOfflineJsonContractResolverProvider contractResolverProvider,
    IDataService dataService,
    IEntityRegister entityRegister,
    LocalDictionaryBS localDictionaryBS)
    : IMessageConsumer<MakeOfflineBackup>
{
    private readonly OfflineBackupOptions options = options.Value;
    private readonly ExternalLangDef lD = ExternalLangDef.LanguageDef;

    public async Task Consume(MakeOfflineBackup message)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentException.ThrowIfNullOrWhiteSpace(options.DirectoryPath);
        var backupDateTime = DateTime.UtcNow;
        try
        {
            DirectoryInfo backupDirectory = CreateBackupDirectory(backupDateTime);
            var towns = await GetTowns();
            var backupSuccess = true;
            foreach (var entity in options.EntityList)
            {
                try
                {
                    backupSuccess &= await LoadBackupsAndWrite(entity, backupDateTime, backupDirectory, towns);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex,
                        $"Ошибка при формировании и записи данных бэкапа для класса \"{entity.EntityType}\".");
                }
            }

            if (backupSuccess)
            {
                DeletePreviousBackups(backupDirectory);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ошибка при выполнении формирования офлайн-бэкапов.");
        }
    }

    private DirectoryInfo CreateBackupDirectory(DateTime backupDateTime)
    {
        var curDirPath = Path.Combine(options.DirectoryPath, backupDateTime.ToString("yyyy-MM-ddTHH.mm.ss"));
        DirectoryInfo backupDir = null;
        try
        {
            backupDir = Directory.CreateDirectory(curDirPath);
            return backupDir;
        }
        catch (Exception ex)
        {
            throw new Exception($"Не удалось создать папку {curDirPath} для бекапов", ex);
        }
    }

    private async Task<List<ITown>> GetTowns()
    {
        try
        {
            var towns = await repository.All<ITown>();
            return towns;
        }
        catch (Exception ex)
        {
            throw new Exception($"Ошибка при получении списка городов", ex);
        }
    }

    private async Task<bool> LoadBackupsAndWrite(OfflineBackupEntity entity, DateTime backupDateTime,
        DirectoryInfo backupDir, List<ITown> towns)
    {
        var classType = entityRegister.GetTypeByName(entity.EntityType);
        if (classType == null)
        {
            throw new Exception($"Для класса {entity.EntityType} из файла конфигурации не удалось получить тип.");
        }

        var classDirPath = backupDir.CreateSubdirectory(classType.FullName);
        if (!classDirPath.Exists)
        {
            throw new Exception($"Не удалось создать директорию \"{classType.FullName}\" для бэкапа класса.");
        }

        var subPartPath = entity.SubPartPath;
        var hasSubPart = !string.IsNullOrEmpty(subPartPath);
        var hasTown = localDictionaryBS.IsLocalDictionary(classType);
        var townPath = entity.TownPath;

        var ctProp = Information.ExtractPropertyName<IDataObjectWithAuditFields>(x => x.CreateTime);
        var view = GetViewForLoad(classType);
        var limitFunc = lD.GetFunction(lD.funcLEQ, new VariableDef(lD.DateTimeType, ctProp), backupDateTime);
        var propsForView = new List<string> { ctProp };
        var limits = new Dictionary<Guid, Function>();
        if (hasSubPart)
        {
            var subPartType = DOInfoHelper.CheckPropertyPathExists(classType, subPartPath);
            if (subPartType == null)
            {
                throw new Exception($"Для класса {classType.Name} путь к свойству ({subPartPath}) не найден.");
            }

            propsForView.Add(subPartPath);
            var subPartRecords = LoadSubPartDictionary(subPartType);

            foreach (var subPartRecord in subPartRecords)
            {
                limits.Add((subPartRecord.__PrimaryKey as KeyGuid).Guid,
                    lD.GetFunction(lD.funcEQ, new VariableDef(lD.GuidType, subPartPath), subPartRecord.__PrimaryKey));
            }
        }
        else if (hasTown)
        {
            propsForView.Add("Town");
            foreach (var town in towns)
            {
                limits.Add(town.Id,
                    lD.GetFunction(lD.funcOR, lD.GetFunction(lD.funcEQ, new VariableDef(lD.GuidType, "Town"), town.Id),
                        lD.GetFunction(lD.funcIsNull, new VariableDef(lD.GuidType, "Town"))));
            }
        }
        else if (!string.IsNullOrWhiteSpace(townPath))
        {
            propsForView.Add(townPath);
            foreach (var town in towns)
            {
                limits.Add(town.Id,
                    lD.GetFunction(lD.funcOR,
                        lD.GetFunction(lD.funcEQ, new VariableDef(lD.GuidType, townPath), town.Id),
                        lD.GetFunction(lD.funcIsNull, new VariableDef(lD.GuidType, townPath))));
            }
        }

        foreach (var prop in propsForView.Where(x => !view.CheckPropname(x)))
        {
            view.AddProperty(prop);
        }

        var lcs = LoadingCustomizationStruct.GetSimpleStruct(classType, view);
        lcs.ColumnsSort = new ColumnsSortDef[] { new ColumnsSortDef(ctProp, SortOrder.Asc) };
        var backupInfo = new JsonBackupInfo();
        var result = true;
        if (limits.Count > 0)
        {
            foreach (var additionLimit in limits)
            {
                var guidStr = additionLimit.Key.ToString("D");
                try
                {
                    var newPart = new JsonBackupPartsInfo(guidStr);
                    var subDirPath = classDirPath.CreateSubdirectory(guidStr);
                    if (!subDirPath.Exists)
                    {
                        logger.LogError(
                            $"Не удалось создать поддиректорию \"{subDirPath.FullName}\" для бэкапа класса \"{classType.FullName}\" по району/городу \"{guidStr}\".");
                        result = false;
                        continue;
                    }

                    var addResult = ReadFromDBAndWriteIntoDirectory(subDirPath, classType, entity, newPart, lcs,
                        limitFunc, additionLimit.Value);
                    result &= addResult;
                    if (addResult)
                    {
                        backupInfo.SubParts.Add(additionLimit.Key, newPart);
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex,
                        $"Ошибка при формировании и записи данных бэкапа для класса \"{classType.FullName}\" по району/городу \"{guidStr}\".");
                    result = false;
                }
            }
        }
        else
        {
            var newPart = new JsonBackupPartsInfo(null);
            var addResult = ReadFromDBAndWriteIntoDirectory(classDirPath, classType, entity, newPart, lcs, limitFunc);
            result &= addResult;
            if (addResult)
            {
                backupInfo.Parts = newPart;
            }
        }

        var backUpFileSerializeData = JsonConvert.SerializeObject(backupInfo);
        var filePath = Path.Combine(classDirPath.FullName, "JsonBackupInfo.json");

        try
        {
            await System.IO.File.WriteAllTextAsync(filePath, backUpFileSerializeData);
        }
        catch (Exception ex)
        {
            throw new Exception(
                $"Ошибка при записи данных о бэкапе в файл \"{filePath}\" для класса {classType.FullName}.", ex);
        }

        return result;
    }

    private List<DataObject> LoadSubPartDictionary(Type subPartType)
    {
        var view = Information.GetView($"{subPartType.Name}E", subPartType);
        LoadingCustomizationStruct lcs;
        if (view != null)
        {
            lcs = LoadingCustomizationStruct.GetSimpleStruct(subPartType, view);
        }
        else
        {
            throw new Exception($"Не найдено представление {subPartType.Name}E для класса-фильтра {subPartType.Name}");
        }

        return dataService.LoadObjects(lcs).ToList();
    }

    private void DeletePreviousBackups(DirectoryInfo currentDirectory)
    {
        try
        {
            DateTime dateEndLoc;
            var oldBackupsDirsPaths = Directory.GetDirectories(currentDirectory.Parent.FullName)
                .Where(x =>
                {
                    var dirName = Path.GetFileName(x);
                    return dirName != currentDirectory.Name
                           && DateTime.TryParseExact(dirName, "yyyy-MM-ddTHH.mm.ss", CultureInfo.InvariantCulture,
                               DateTimeStyles.AssumeUniversal, out dateEndLoc);
                }).ToArray();
            foreach (var oldBackupDirPath in oldBackupsDirsPaths)
            {
                Directory.Delete(oldBackupDirPath, true);
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Ошибка при удалении предыдущих бекапов в {currentDirectory.Parent.FullName}.", ex);
        }
    }

    private bool ReadFromDBAndWriteIntoDirectory(DirectoryInfo backupClassDir, Type classType,
        OfflineBackupEntity entity, JsonBackupPartsInfo backupPartInfo,
        LoadingCustomizationStruct lcs, Function limitFunction, Function additionLimit = null)
    {
        var lD = ExternalLangDef.LanguageDef;
        var usePaging = entity.PortionSize.HasValue && entity.PortionSize.Value > 0;
        var portionSize = usePaging ? entity.PortionSize.Value : 0;
        lcs.LimitFunction = additionLimit == null
            ? limitFunction
            : lD.GetFunction(lD.funcAND, limitFunction, additionLimit);
        lcs.RowNumber = null;

        return SerializeDataAndWriteInFile(entity, backupClassDir, classType, backupPartInfo, lcs, usePaging,
            portionSize);
    }

    private bool SerializeDataAndWriteInFile(OfflineBackupEntity entity, DirectoryInfo backupClassDir, Type classType,
        JsonBackupPartsInfo backupPartInfo, LoadingCustomizationStruct lcs, bool usePaging, int portionSize)
    {
        int batchSize = entity.BatchSize ?? 0;
        var serializer = CreateJsonSerializer(classType);
        var partIndex = 0;
        var itemsInCurrentFile = 0;
        var fileWriter = CreatePartWriter(backupClassDir, classType, partIndex);
        try
        {
            if (usePaging)
            {
                var pageIndex = 0;
                while (true)
                {
                    lcs.RowNumber = new RowNumberDef(pageIndex * portionSize + 1, (pageIndex + 1) * portionSize);
                    var page = dataService.LoadObjects(lcs);
                    if (page.Length == 0)
                    {
                        break;
                    }

                    foreach (var obj in page)
                    {
                        if (batchSize > 0 && itemsInCurrentFile >= batchSize)
                        {
                            if (!fileWriter.Commit())
                            {
                                return false;
                            }

                            backupPartInfo.PartsLocalPaths.Add(partIndex, fileWriter.FinalFileName);
                            partIndex++;
                            fileWriter = CreatePartWriter(backupClassDir, classType, partIndex);
                            itemsInCurrentFile = 0;
                        }

                        fileWriter.WriteItem(serializer, obj);
                        itemsInCurrentFile++;
                    }

                    pageIndex++;
                }
            }
            else
            {
                lcs.RowNumber = null;
                var page = dataService.LoadObjects(lcs);
                foreach (var obj in page)
                {
                    if (batchSize > 0 && itemsInCurrentFile >= batchSize)
                    {
                        if (!fileWriter.Commit())
                        {
                            return false;
                        }

                        backupPartInfo.PartsLocalPaths.Add(partIndex, fileWriter.FinalFileName);
                        partIndex++;
                        fileWriter = CreatePartWriter(backupClassDir, classType, partIndex);
                        itemsInCurrentFile = 0;
                    }

                    fileWriter.WriteItem(serializer, obj);
                    itemsInCurrentFile++;
                }
            }

            if (!fileWriter.Commit())
            {
                return false;
            }

            backupPartInfo.PartsLocalPaths.Add(partIndex, fileWriter.FinalFileName);
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                $"Ошибка при записи данных бэкапа в файл \"{fileWriter.FinalPath}\" для класса {classType.FullName}.");
            fileWriter.Abort();
            return false;
        }
        finally
        {
            fileWriter.Dispose();
        }
    }

    private JsonPartFileWriter CreatePartWriter(DirectoryInfo backupClassDir, Type classType, int partIndex)
    {
        var fileName = $"{classType.FullName}_{partIndex}.json";
        var finalPath = Path.Combine(backupClassDir.FullName, fileName);
        return new JsonPartFileWriter(fileName, finalPath, logger);
    }

    private sealed class JsonPartFileWriter : IDisposable
    {
        private readonly ILogger logger;
        private readonly string _tempPath;
        private FileStream _fileStream;
        private StreamWriter _streamWriter;
        private JsonTextWriter _jsonWriter;
        private bool _committed;
        private bool _disposed;

        public JsonPartFileWriter(string finalFileName, string finalPath, ILogger logger)
        {
            this.logger = logger;
            FinalFileName = finalFileName;
            FinalPath = finalPath;
            _tempPath = $"{finalPath}.tmp";
            _fileStream = new FileStream(_tempPath, FileMode.Create, FileAccess.Write, FileShare.Read);
            _streamWriter = new StreamWriter(_fileStream, new UTF8Encoding(false));
            _jsonWriter = new JsonTextWriter(_streamWriter);
            _jsonWriter.WriteStartArray();
        }

        public string FinalFileName { get; }

        public string FinalPath { get; }

        public void WriteItem(JsonSerializer serializer, DataObject obj)
        {
            serializer.Serialize(_jsonWriter, obj);
        }

        public bool Commit()
        {
            if (_committed)
            {
                return true;
            }

            try
            {
                _jsonWriter.WriteEndArray();
                _jsonWriter.Flush();
                _streamWriter.Flush();
                _fileStream.Flush(true);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Ошибка при завершении записи бэкапа в файл \"{FinalPath}\".");
                Abort();
                return false;
            }
            finally
            {
                Dispose();
            }

            try
            {
                if (File.Exists(FinalPath))
                {
                    File.Delete(FinalPath);
                }

                File.Move(_tempPath, FinalPath);
                _committed = true;
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Ошибка при сохранении файла бэкапа \"{FinalPath}\".");
                Abort();
                return false;
            }
        }

        public void Abort()
        {
            try
            {
                Dispose();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Ошибка при закрытии ресурсов для файла \"{FinalPath}\".");
            }

            try
            {
                if (File.Exists(_tempPath))
                {
                    File.Delete(_tempPath);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Ошибка при удалении временного файла бэкапа \"{_tempPath}\".");
            }
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            TryDispose(_jsonWriter);
            TryDispose(_streamWriter);
            TryDispose(_fileStream);
            _jsonWriter = null;
            _streamWriter = null;
            _fileStream = null;
        }

        private static void TryDispose(IDisposable disposable)
        {
            try
            {
                disposable?.Dispose();
            }
            catch
            {
                // ignored
            }
        }
    }

    private JsonSerializer CreateJsonSerializer(Type classType)
    {
        return JsonSerializer.Create(
            new JsonSerializerSettings
            {
                ContractResolver = contractResolverProvider.GetResolver(classType),
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Converters = new List<JsonConverter>
                {
                    new Newtonsoft.Json.Converters.StringEnumConverter(),
                    new OfflineJsonBoolConverter(),
                },
            });
    }

    private View GetViewForLoad(Type classType)
    {
        var viewName = $"{classType.FullName}({View.ReadType.WithRelated.ToString()})";
        var allStorProps = Information.GetStorablePropertyNames(classType);
        var simpleProps = new HashSet<string>();
        var detailProps = new List<string>();

        for (int i = 0; i < allStorProps.Length; i++)
        {
            if (Information.GetPropertyType(classType, allStorProps[i]).IsSubclassOf(typeof(DetailArray)))
                detailProps.Add(allStorProps[i]);
            else
            {
                if (!Information.GetPropertyDisableAutoViewing(classType, allStorProps[i]))
                    simpleProps.Add(allStorProps[i]);
            }
        }

        var notLoadFields = Information.GetNotStorablePropertyNames(classType);
        if (notLoadFields.Length > 0)
        {
            var additionProps = DOInfoHelper.GetRelationsPropertiesForCalculatedFields(classType);
            foreach (var additionProp in additionProps)
            {
                foreach (var expandedProp in ExpandMasterPath(additionProp))
                {
                    simpleProps.Add(expandedProp);
                }
            }
        }

        var result = new View(new ViewAttribute(viewName, simpleProps.ToArray()), classType);
        foreach (var detail in detailProps)
        {
            var itemType = Information.GetItemType(classType, detail);
            var additionPropsDetail = DOInfoHelper.GetDetailPropertiesForCalculatedFields(detail, classType);
            View detailView = new View(new ViewAttribute($"{itemType.FullName}(OnlyPrimaryKey)", new string[0]),
                itemType);
            if (additionPropsDetail.Count > 0)
            {
                viewName = $"{itemType.FullName}({View.ReadType.WithRelated.ToString()})";
                detailView = new View(new ViewAttribute(viewName, additionPropsDetail.ToArray()), itemType);
            }

            result.AddDetailInView(detail, detailView, true, string.Empty, true, string.Empty, null);
        }

        return result;
    }

    private static IEnumerable<string> ExpandMasterPath(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            yield break;
        }

        var parts = path.Split('.');
        for (var i = 0; i < parts.Length; i++)
        {
            var part = string.Join(".", parts.Take(i + 1));
            if (!string.IsNullOrWhiteSpace(part))
            {
                yield return part;
            }
        }
    }
}
