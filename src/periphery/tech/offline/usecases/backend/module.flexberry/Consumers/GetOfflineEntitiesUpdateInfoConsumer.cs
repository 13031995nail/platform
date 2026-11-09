namespace Tmobis.Offline.UseCases.Flexberry.Consumers;
using System.Threading.Tasks;
using ICSSoft.STORMNET;
using ICSSoft.STORMNET.Business;
using ICSSoft.STORMNET.Business.Audit;
using ICSSoft.STORMNET.FunctionalLanguage;
using Microsoft.Extensions.Logging;
using Tmobis.Core.Converters;
using Tmobis.Core.Entities;
using Tmobis.Core.Medium.Consumers;
using Tmobis.Frameworks.Flexberry.Converters;
using Tmobis.Offline.Flexberry.Messages;

public class GetOfflineEntitiesUpdateInfoConsumer(
    IDataService dataService,
    IEntityRegister entityRegister,
    IPropertyNameConverter propertyNameConverter,
    IFlexberryPredicateConverter predicateConverter,
    ILogger<GetOfflineEntitiesUpdateInfoConsumer> logger)
: IMessageConsumer<
    GetOfflineEntitiesUpdateInfo, OfflineEntitiesUpdateInfo>
{
    private readonly IDataService dataService = dataService;

    private readonly IPropertyNameConverter propertyNameConverter
    = propertyNameConverter;

    private readonly IFlexberryPredicateConverter predicateConverter
    = predicateConverter;

    private readonly ILogger<GetOfflineEntitiesUpdateInfoConsumer> logger
      = logger;

    public async Task<OfflineEntitiesUpdateInfo> ConsumeWithResult(
        GetOfflineEntitiesUpdateInfo message)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(message);
            var dateEnd = GetDateEnd();
            var objsForUpdateCheck = message.Dicts;
            var result = new OfflineEntitiesUpdateInfo
            {
                DateEnd = dateEnd.Item1.ToString("o"),
            };

            if (objsForUpdateCheck != null)
            {
                if (objsForUpdateCheck.Count > 0)
                {
                    var lD = ICSSoft.STORMNET.Windows
                        .Forms.ExternalLangDef.LanguageDef;
                    var ctProp = Information
                        .ExtractPropertyName<IDataObjectWithAuditFields>(
                            x => x.CreateTime);
                    var etProp = Information
                        .ExtractPropertyName<IDataObjectWithAuditFields>(
                            x => x.EditTime);
                    var propsArray = new[] { ctProp, etProp };
                    var limitWithEndDateCT = lD.GetFunction(
                        lD.funcLEQ,
                        new VariableDef(lD.DateTimeType, ctProp),
                        dateEnd.Item2);

                    foreach (var dic in objsForUpdateCheck)
                    {
                        var isFirstLoadCompleted = dic.IsFirstLoadCompleted;
                        var type = entityRegister.GetTypeByName(dic.ModelName)
                            ?? throw new InvalidOperationException(
                                "Тип не найден");

                        var dStart = dic.DateStart;
                        var eDateStart = dic.EditDateStart;
                        dStart = new DateTime(2024, 1, 1);
                        eDateStart = new DateTime(2024, 1, 1);

                        var lcs = GetLcsWithLimit(dic, type, ctProp, etProp);
                        var loadLimit = GetFunctionForLoad(
                            isFirstLoadCompleted,
                            dStart,
                            eDateStart,
                            dateEnd.Item2,
                            ctProp,
                            etProp,
                            dic.MastersForCheck);

                        var funcLimit = lcs.LimitFunction;
                        if (funcLimit == null)
                        {
                            lcs.LimitFunction = loadLimit;
                        }
                        else
                        {
                            lcs.LimitFunction = lD.GetFunction(
                                lD.funcAND, loadLimit, funcLimit);
                        }

                        var createdCount = dataService.GetObjectsCount(lcs);

                        var editedCount = 0;
                        if (dStart.HasValue
                            && (eDateStart.HasValue || !isFirstLoadCompleted))
                        {
                            var editLimit = GetFunctionForEdit(
                                isFirstLoadCompleted,
                                dStart,
                                eDateStart,
                                dateEnd.Item2,
                                ctProp,
                                etProp,
                                dic.MastersForCheck);

                            if (funcLimit == null)
                            {
                                lcs.LimitFunction = editLimit;
                            }
                            else
                            {
                                lcs.LimitFunction = lD.GetFunction(
                                    lD.funcAND, editLimit, funcLimit);
                            }

                            editedCount = dataService.GetObjectsCount(lcs);
                        }

                        if (createdCount > 0 || editedCount > 0)
                        {
                            result.Types.Add(new UpdatedObjectInfo
                            {
                                Type = type.FullName,
                                CreatedCount = createdCount,
                                EditedCount = editedCount,
                            });
                        }
                    }
                }
            }

            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                @"Ошибка при получении данных 
                синхронизации для Справочников = {Message}",
                message);
            return new OfflineEntitiesUpdateInfo { Error = true };
        }
    }

    protected virtual Tuple<DateTime, DateTime> GetDateEnd()
    {
        // Вычитаем 2 секунды, что бы не залезть в текущие транзакции.
        // Будем жить немного в прошлом, так надежнее.
        var dEnd = DateTime.Now.AddSeconds(-2);
        dEnd = dEnd.AddMilliseconds(-dEnd.Millisecond);
        var dEndWithOffset = dEnd.Subtract(TimeZoneInfo.Local.BaseUtcOffset);
        return new Tuple<DateTime, DateTime>(dEnd, dEndWithOffset);
    }

    protected virtual Function GetFunctionForLoad(
        bool isFirstLoadCompleted,
        DateTime? dStart,
        DateTime? eDateStart,
        DateTime dEndWOffset,
        string ctProp,
        string etProp,
        List<string> masters)
    {
        var lD = ICSSoft.STORMNET.Windows
            .Forms.ExternalLangDef.LanguageDef;
        Function result;
        if (dStart.HasValue)
        {
            if (isFirstLoadCompleted)
            {
                result = lD.GetFunction(
                    lD.funcAND,
                    lD.GetFunction(
                        lD.funcG,
                        new VariableDef(lD.DateTimeType, ctProp),
                        dStart.Value),
                    lD.GetFunction(
                        lD.funcLEQ,
                        new VariableDef(lD.DateTimeType, ctProp),
                        dEndWOffset),
                    lD.GetFunction(
                        lD.funcOR,
                        lD.GetFunction(
                            lD.funcIsNull,
                            new VariableDef(lD.DateTimeType, etProp)),
                        lD.GetFunction(
                            lD.funcG,
                            new VariableDef(lD.DateTimeType, etProp),
                            dEndWOffset)));
            }
            else
            {
                result = lD.GetFunction(
                    lD.funcAND,
                    lD.GetFunction(
                        lD.funcG,
                        new VariableDef(lD.DateTimeType, ctProp),
                        dStart.Value),
                    lD.GetFunction(
                        lD.funcLEQ,
                        new VariableDef(lD.DateTimeType, ctProp),
                        dEndWOffset));
            }
        }
        else
        {
            result = lD.GetFunction(
                lD.funcLEQ,
                new VariableDef(lD.DateTimeType, ctProp),
                dEndWOffset);
        }

        return result;
    }

    protected virtual Function GetFunctionForEdit(
        bool isFirstLoadCompleted,
        DateTime? dStart,
        DateTime? eDateStart,
        DateTime dEndWOffset,
        string ctProp,
        string etProp,
        List<string> masters)
    {
        var lD = ICSSoft.STORMNET.Windows
            .Forms.ExternalLangDef.LanguageDef;
        var minDate = dStart.HasValue && eDateStart.HasValue
            ? eDateStart
            : null;
        var result = isFirstLoadCompleted
            ? lD.GetFunction(
                lD.funcAND,
                lD.GetFunction(
                    lD.funcG,
                    new VariableDef(lD.DateTimeType, etProp),
                    eDateStart ?? DateTime.MinValue),
                lD.GetFunction(
                    lD.funcLEQ,
                    new VariableDef(lD.DateTimeType, etProp),
                    dEndWOffset))
            : lD.GetFunction(
                lD.funcAND,
                lD.GetFunction(
                    lD.funcG,
                    new VariableDef(lD.DateTimeType, etProp),
                    dStart ?? DateTime.MinValue),
                lD.GetFunction(
                    lD.funcLEQ,
                    new VariableDef(lD.DateTimeType, etProp),
                    dEndWOffset),
                lD.GetFunction(
                    lD.funcLEQ,
                    new VariableDef(lD.DateTimeType, ctProp),
                    dStart ?? DateTime.MinValue));

        result = AddMastersFunction(
            masters, result, minDate, dEndWOffset, ctProp, etProp);
        return result;
    }

    protected virtual Function GetFunctionForMaster(
        DateTime? dStart, DateTime dEndWOffset, string ctProp, string etProp)
    {
        var lD = ICSSoft.STORMNET.Windows
            .Forms.ExternalLangDef.LanguageDef;
        return dStart.HasValue
              ? lD.GetFunction(
                    lD.funcOR,
                    lD.GetFunction(
                        lD.funcAND,
                        lD.GetFunction(
                            lD.funcG,
                            new VariableDef(lD.DateTimeType, ctProp),
                            dStart.Value),
                        lD.GetFunction(
                            lD.funcLEQ,
                            new VariableDef(lD.DateTimeType, ctProp),
                            dEndWOffset)),
                    lD.GetFunction(
                        lD.funcAND,
                        lD.GetFunction(
                            lD.funcG,
                            new VariableDef(lD.DateTimeType, etProp),
                            dStart ?? DateTime.MinValue),
                        lD.GetFunction(
                            lD.funcLEQ,
                            new VariableDef(lD.DateTimeType, etProp),
                            dEndWOffset)))
              : lD.GetFunction(
                    lD.funcOR,
                    lD.GetFunction(
                        lD.funcLEQ,
                        new VariableDef(lD.DateTimeType, ctProp),
                        dEndWOffset),
                    lD.GetFunction(
                        lD.funcLEQ,
                        new VariableDef(lD.DateTimeType, etProp),
                        dEndWOffset));
    }

    protected virtual Function AddMastersFunction(
        List<string> masters,
        Function limit,
        DateTime? dStart,
        DateTime dEndWOffset,
        string ctProp,
        string etProp)
    {
        var lD = ICSSoft.STORMNET.Windows
            .Forms.ExternalLangDef.LanguageDef;
        Function result = limit;
        if (masters != null && masters.Count > 0)
        {
            var mastersFuncs = new object[masters.Count + 1];
            mastersFuncs[0] = limit;
            var sc = 1;
            foreach (var master in masters)
            {
                var masterPropPath = propertyNameConverter
                    .GetBackendPropertyName(master);
                mastersFuncs[sc] = GetFunctionForMaster(
                    dStart,
                    dEndWOffset,
                    masterPropPath + "." + ctProp,
                    masterPropPath + "." + etProp);
                sc++;
            }

            result = lD.GetFunction(lD.funcOR, mastersFuncs);
        }

        return result;
    }

    protected virtual LoadingCustomizationStruct GetLcsWithLimit(
        UpdatingObject dic, Type type, string ctProp, string etProp)
    {
        var propsForView = new List<string>() { ctProp, etProp };
        Function limit = null;
        if (!string.IsNullOrWhiteSpace(dic.Limit))
        {
            limit = predicateConverter.ConvertPredicate(
                type, dic.Limit, propsForView);
        }

        if (dic.MastersForCheck != null && dic.MastersForCheck.Count > 0)
        {
            foreach (var master in dic.MastersForCheck
                .Select(propertyNameConverter.GetBackendPropertyName)
                .Where(x => !propsForView.Contains(x)))
            {
                propsForView.Add(master);
                propsForView.Add(master + "." +
                    Information.ExtractPropertyName<
                    IDataObjectWithAuditFields>(x => x.CreateTime));
                propsForView.Add(master + "." +
                    Information.ExtractPropertyName<
                    IDataObjectWithAuditFields>(x => x.EditTime));
            }
        }

        var view = new View(
            new ViewAttribute("UpdateCheckView", [.. propsForView]), type);
        return new LoadingCustomizationStruct(null)
        {
            View = view,
            LimitFunction = limit,
            LoadingTypes = [type],
        };
    }
}